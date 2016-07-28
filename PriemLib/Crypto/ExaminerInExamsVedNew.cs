using PriemLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PriemLib
{
    public partial class CardExaminerInExamsVedNew : Form
    {
        public List<ExaminerAccountDataSourceItem> lst;

        private Guid ExamsVedId;

        public CardExaminerInExamsVedNew(Guid _ExamsVedId)
        {
            InitializeComponent();
            ExamsVedId = _ExamsVedId;
            this.MdiParent = MainClass.mainform;

            
            FillCard();
        }

        private void InitExaminerList()
        {
            using (PriemEntities context = new PriemEntities())
            {
                lst = (from x in context.ExaminerAccount
                           join l in context.ExaminerInExamsVed on
                           new { AccountName = x.AccountName, VedId = ExamsVedId } equals
                           new { AccountName = l.ExaminerAccount, VedId = l.ExamsVedId } into _l
                           from l in _l.DefaultIfEmpty()
                           select new ExaminerAccountDataSourceItem
                           {
                               IsChecked = (l != null),
                               RectoratLogin =  x.AccountName,
                               FIO = x.DisplayName + " (" + x.AccountName + ")",
                               IsMain = l.IsMain ?? false,
                           }).Distinct().ToList().OrderByDescending(x => x.IsChecked).ThenByDescending(x => x.IsMain).ThenBy(x => x.FIO).ToList();
                DgvUpdate();

                if (dgv.Columns.Contains("RectoratLogin"))
                    dgv.Columns["RectoratLogin"].Visible = false;
                if (dgv.Columns.Contains("IsChecked"))
                    dgv.Columns["IsChecked"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                if (dgv.Columns.Contains("FIO"))
                    dgv.Columns["FIO"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void FillCard()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var vedInfo = context.extExamsVed.Where(x => x.Id == ExamsVedId).FirstOrDefault();
                if (vedInfo != null)
                {
                    tbExamsVed.Text = vedInfo.ExamName + " " + vedInfo.Date;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Сохранить список экзаменаторов для ведомости?", "Сохранение", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
            {
                //SaveCard();
                //this.Close();
            }
        }

        private void SaveCard()
        {
            using (PriemEntities context = new PriemEntities())
            {
                context.ExamsVed_DeleteAllExaminerAccountInVed(ExamsVedId);

                //foreach (string accouns in lstExaminers)
                //{
                //    context.ExamsVed_SetExaminerAccount(accouns, ExamsVedId);
                //}
                foreach (DataGridViewRow rw in dgv.Rows)
                {
                    if ((bool)rw.Cells["Добавлен"].Value)
                    {
                        context.ExaminerInExamsVed.Add(new ExaminerInExamsVed()
                            {
                                ExamsVedId = ExamsVedId,
                                ExaminerAccount = rw.Cells["AccountName"].Value.ToString(),
                                IsMain = (bool)rw.Cells["IsMain"].Value,
                            });
                    }
                }
            }
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
        public void PaintRow(int rowind)
        {
            if ((bool)dgv.Rows[rowind].Cells["IsMain"].Value)
            {
                foreach (DataGridViewCell cell in dgv.Rows[rowind].Cells)
                    cell.Style.BackColor = Color.Green;
            }
            else if ((bool)dgv.Rows[rowind].Cells["IsChecked"].Value)
            {
                foreach (DataGridViewCell cell in dgv.Rows[rowind].Cells)
                    cell.Style.BackColor = Color.LightGreen;
            }
            else
            {
                foreach (DataGridViewCell cell in dgv.Rows[rowind].Cells)
                    cell.Style.BackColor = Color.Empty;
            }
        }

        private void CardExaminerInExamsVedNew_Shown(object sender, EventArgs e)
        {
            InitExaminerList();
            dgv.ReadOnly = false;
            foreach (DataGridViewRow rw in dgv.Rows)
            {
                rw.ReadOnly = false;
                foreach (DataGridViewCell c in rw.Cells)
                    c.ReadOnly = false;
                rw.Cells["FIO"].ReadOnly = true;
            }
            foreach (DataGridViewRow rw in dgv.Rows)
            {
                PaintRow(rw.Index);
            }
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (e.ColumnIndex == dgv.Columns["IsMain"].Index)
                {
                    if (!lst[e.RowIndex].IsChecked && !lst[e.RowIndex].IsMain)
                    {
                        lst[e.RowIndex].IsChecked = true;
                    }
                    lst[e.RowIndex].IsMain = !lst[e.RowIndex].IsMain;

                    PaintRow(e.RowIndex);
                }
                else if (e.ColumnIndex == dgv.Columns["IsChecked"].Index)
                {
                    lst[e.RowIndex].IsChecked = !lst[e.RowIndex].IsChecked;
                    PaintRow(e.RowIndex);
                }
                
            }
            

            DgvUpdate();
        }
        public void DgvUpdate()
        {
            BindingSource src = new BindingSource();
            lst = lst.OrderByDescending(x => x.IsChecked).ThenByDescending(x => x.IsMain).ThenBy(x => x.FIO).ToList();
            src.DataSource = lst;
            dgv.DataSource = src;
            foreach (DataGridViewRow rw in dgv.Rows)
                PaintRow(rw.Index);

            if (dgv.Columns.Contains("IsChecked"))
                dgv.Columns["IsChecked"].HeaderText = "Добавлен(а)";
            if (dgv.Columns.Contains("FIO"))
                dgv.Columns["IsChecked"].HeaderText = "Проверяющий";
            if (dgv.Columns.Contains("IsMain"))
                dgv.Columns["IsMain"].HeaderText = "Главный проверяющий";
            dgv.Update();
            dgv.ClearSelection();
        }
    }
    public class ExaminerAccountDataSourceItem
    {
        public bool IsChecked { get; set; }
        public string RectoratLogin { get; set; }
        public string FIO { get; set; }
        public bool IsMain { get; set; }
    }
}
