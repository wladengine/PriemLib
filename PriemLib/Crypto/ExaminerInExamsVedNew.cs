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
                    int? Cnt = context.ExamsVed.Where(x => x.Id == ExamsVedId).Select(x=>x.ExaminerCount).FirstOrDefault();
                    if (Cnt.HasValue)
                        tbExaminatorCount.Text = Cnt.ToString();
                    else
                        tbExaminatorCount.Text = "1";
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int tmp;
            if (!String.IsNullOrEmpty(tbExaminatorCount.Text) && !int.TryParse(tbExaminatorCount.Text, out tmp))
            {
                MessageBox.Show("Неверное количество экзаменаторов на одну работу","Введите целое число");
                return;
            }
            if (MessageBox.Show("Сохранить список экзаменаторов для ведомости?", "Сохранение", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
            {
                SaveCard();
                this.Close();
            }
        }

        private void SaveCard()
        {
            using (PriemEntities context = new PriemEntities())
            {
                int tmp;
                if (!String.IsNullOrEmpty(tbExaminatorCount.Text))
                {
                    tmp = int.Parse(tbExaminatorCount.Text);
                    var Examsved = context.ExamsVed.Where(x => x.Id == ExamsVedId).First();
                    Examsved.ExaminerCount = tmp;
                    context.SaveChanges();
                }

                //context.ExamsVed_DeleteAllExaminerAccountInVed(ExamsVedId);
                foreach (var x in lst)
                {
                    var Examiner = context.ExaminerInExamsVed.Where(t => t.ExamsVedId == ExamsVedId && t.ExaminerAccount == x.RectoratLogin).FirstOrDefault();
                    
                    if (x.IsChecked)
                    {
                        if (Examiner == null)
                        {
                            context.ExaminerInExamsVed.Add(new ExaminerInExamsVed()
                                {
                                    ExamsVedId = ExamsVedId,
                                    ExaminerAccount = x.RectoratLogin,
                                    IsMain = x.IsMain,
                                });
                        }
                        else
                        {
                            Examiner.IsMain = x.IsMain;
                        }
                        context.SaveChanges();
                    }
                    else
                    {
                        if (Examiner != null)
                        {
                            context.ExaminerInExamsVed.Remove(Examiner);
                            context.SaveChanges();
                        }
                    }
                }
            }
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
                        lst[e.RowIndex].IsChecked = true;
                    
                    lst[e.RowIndex].IsMain = !lst[e.RowIndex].IsMain;
                    PaintRow(e.RowIndex);
                }
                else if (e.ColumnIndex == dgv.Columns["IsChecked"].Index)
                {
                    lst[e.RowIndex].IsChecked = !lst[e.RowIndex].IsChecked;
                    if (!lst[e.RowIndex].IsChecked)
                        lst[e.RowIndex].IsMain = false;

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

        private void tbFIO_TextChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow rw in dgv.Rows)
            {
                if (rw.Cells["FIO"].Value.ToString().ToLower().Contains(tbFIO.Text.ToString().ToLower()))
                {
                    dgv.CurrentCell = rw.Cells["FIO"];
                    break;
                }
            }
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
