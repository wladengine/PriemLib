using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PriemLib
{
    public partial class CardDocumentList : Form
    {
        Guid gPersonId;
        public CardDocumentList(Guid persid)
        {
            InitializeComponent();
            gPersonId = persid;
            FillCard();
        }
        public void FillCard()
        {
            using (PriemEntities context = new PriemEntities())
            {
                extPerson pers = context.extPerson.Where(x => x.Id == gPersonId).First();
                List<Person_EducationInfo> educ = context.Person_EducationInfo.Where(x=>x.PersonId == gPersonId).ToList();

                lblFIO.Text = pers.FIO;
                string Staff = MainClass.GetUserName();
                tbStaff.Text = Staff.Substring(0, Staff.LastIndexOf("("));

                DataGridViewCheckBoxCell cell1 = new DataGridViewCheckBoxCell();
                DataGridViewCheckBoxColumn chb_col = new DataGridViewCheckBoxColumn();
                chb_col.CellTemplate = cell1;
                chb_col.Name = "На_печать";
                chb_col.HeaderText = "Печать";
                dgv.Columns.Add(chb_col);

                dgv.Columns.Add("Документ", "Документ");
                DataGridViewCheckBoxColumn chb_col2 = new DataGridViewCheckBoxColumn();
                chb_col2.CellTemplate = cell1;
                chb_col2.Name = "OriginalIsNeed";
                chb_col2.HeaderText = "OriginalIsNeed";
                dgv.Columns.Add(chb_col2);
                DataGridViewCheckBoxColumn chb_col3 = new DataGridViewCheckBoxColumn();
                chb_col3.CellTemplate = cell1;
                chb_col3.Name = "Оригинал";
                chb_col3.HeaderText = "Оригинал";
                dgv.Columns.Add(chb_col3);

                dgv.Columns.Add("Номер_документа", "Номер документа");
                dgv.Columns.Add("Количество", "Количество");
                dgv.Columns["OriginalIsNeed"].Visible = false;

                foreach (var x in educ)
                {
                    dgv.Rows.Add();
                    bool isSchool = x.SchoolTypeId == 1;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[0].Value = false;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[1].Value = isSchool ? "Аттестат": "Документ об образовании";
                    dgv.Rows[dgv.Rows.Count - 1].Cells[2].Value = true;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[3].Value = false; 
                    dgv.Rows[dgv.Rows.Count - 1].Cells[4].Value = isSchool ? (x.AttestatSeries + " " + x.AttestatNum) : (x.DiplomSeries +  " " + x.DiplomNum);
                    dgv.Rows[dgv.Rows.Count - 1].Cells[5].Value = 1;
                }
                if (MainClass.dbType == PriemType.Priem)
                {
                    dgv.Rows.Add();
                    dgv.Rows[dgv.Rows.Count - 1].Cells[0].Value = false;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[1].Value = "Сертификат ЕГЭ";
                    dgv.Rows[dgv.Rows.Count - 1].Cells[2].Value = true;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[3].Value = false; 
                    dgv.Rows[dgv.Rows.Count - 1].Cells[4].Value = "";
                    dgv.Rows[dgv.Rows.Count - 1].Cells[5].Value = 1;
                }
                if (MainClass.dbType == PriemType.PriemMag)
                {
                    dgv.Rows.Add();
                    dgv.Rows[dgv.Rows.Count - 1].Cells[0].Value = false;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[1].Value = "Фотографии";
                    dgv.Rows[dgv.Rows.Count - 1].Cells[2].Value = false;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[3].Value = true;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[4].Value = "";
                    dgv.Rows[dgv.Rows.Count - 1].Cells[5].Value = 2;
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            List<string> lst = new List<string>();
            foreach (DataGridViewRow rw in dgv.Rows)
            {
                if ((bool)rw.Cells[0].Value)
                {
                    string doc = rw.Cells[1].Value.ToString();
                    if ((bool)rw.Cells[2].Value)
                        doc += " (" + ((bool)rw.Cells[3].Value ? "оригинал" : "копия") + ")";
                    doc += ", " + rw.Cells[5].Value.ToString() + " шт.";
                    lst.Add(doc);
                }
            }
            Print.PrintDocumentList(gPersonId, lst, tbStaff.Text);
        }
    }
}
