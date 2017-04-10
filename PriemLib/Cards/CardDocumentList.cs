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
            this.MdiParent = MainClass.mainform;
            InitializeComponent();
            gPersonId = persid;
            this.Text = "Печать расписки о приеме документов";
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
                chb_col.HeaderText = "Получено";
                dgv.Columns.Add(chb_col);

                dgv.Columns.Add("Документ", "Документ");
                dgv.Columns["Документ"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

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

                DataGridViewCheckBoxColumn chb_col4 = new DataGridViewCheckBoxColumn();
                chb_col4.CellTemplate = cell1;
                chb_col4.Name = "CountIsPrint";
                chb_col4.HeaderText = "CountIsPrint";
                dgv.Columns.Add(chb_col4);
                dgv.Columns.Add("Количество", "Количество");

                dgv.Columns["CountIsPrint"].Visible = false;
                dgv.Columns["OriginalIsNeed"].Visible = false;

                DataGridViewCheckBoxColumn chb_col5 = new DataGridViewCheckBoxColumn();
                chb_col5.CellTemplate = cell1;
                chb_col5.Name = "На_печать";
                chb_col5.HeaderText = "Получено";
                dgvApps.Columns.Add(chb_col5);
                dgvApps.Columns.Add("Программа", "Программа");
                dgvApps.Columns["Программа"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                if (1 == 1)
                {
                    dgv.Rows.Add();
                    dgv.Rows[dgv.Rows.Count - 1].Cells[0].Value = true;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[1].Value = "Заявление о приеме на основную образовательную программу";
                    dgv.Rows[dgv.Rows.Count - 1].Cells[2].Value = false;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[3].Value = false;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[4].Value = "";
                    dgv.Rows[dgv.Rows.Count - 1].Cells[5].Value = false;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[6].Value = "";
                }
                foreach (var x in educ)
                {
                    dgv.Rows.Add();
                    bool isSchool = x.SchoolTypeId == 1;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[0].Value = true;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[1].Value = "Документ об образовании";
                    dgv.Rows[dgv.Rows.Count - 1].Cells[2].Value = true;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[3].Value = false;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[4].Value = String.Format("{0} серия {1} № {2} от _________________ \nвыданный______________________________________________________________________________________________ _______________________________________________________________________________________________________"
                        , (isSchool ? "аттестат" : "диплом")
                        , (isSchool ? x.AttestatSeries : x.DiplomSeries), (isSchool ? x.AttestatNum : x.DiplomNum));
                    dgv.Rows[dgv.Rows.Count - 1].Cells[5].Value = false;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[6].Value = "";
                }
                if (MainClass.dbType == PriemType.Priem)
                {
                    dgv.Rows.Add();
                    dgv.Rows[dgv.Rows.Count - 1].Cells[0].Value = true;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[1].Value = "Свидетельство ЕГЭ";
                    dgv.Rows[dgv.Rows.Count - 1].Cells[2].Value = true;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[3].Value = false; 
                    dgv.Rows[dgv.Rows.Count - 1].Cells[4].Value = "";
                    dgv.Rows[dgv.Rows.Count - 1].Cells[5].Value = false;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[6].Value = 1;
                }
                if (1==1)
                {
                    dgv.Rows.Add();
                    dgv.Rows[dgv.Rows.Count - 1].Cells[0].Value = true;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[1].Value = "Фотокарточки";
                    dgv.Rows[dgv.Rows.Count - 1].Cells[2].Value = false;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[3].Value = true;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[4].Value = "";
                    dgv.Rows[dgv.Rows.Count - 1].Cells[5].Value = true;
                    dgv.Rows[dgv.Rows.Count - 1].Cells[6].Value = (MainClass.dbType == PriemType.Priem)? 4 : 2;
                }

                var Apps = context.extAbit.Where(x => x.PersonId == gPersonId).Select(x=>x.LicenseProgramName + " (" + x.ObrazProgramName+")").ToList();
                foreach (var app in Apps)
                {
                    dgvApps.Rows.Add();
                    dgvApps.Rows[dgvApps.Rows.Count - 1].Cells[0].Value = true;
                    dgvApps.Rows[dgvApps.Rows.Count - 1].Cells[1].Value = app;
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
                        doc += " (" + ((bool)rw.Cells[3].Value ? "оригинал" : "копия") + ") ";
                    if (!String.IsNullOrEmpty(rw.Cells[4].Value.ToString()))
                        doc += rw.Cells[4].Value.ToString();
                    if ((bool)rw.Cells[5].Value)
                        doc += ", " + rw.Cells[6].Value.ToString() + " шт.";

                    if (rw.Index == 0)
                    {
                        foreach (DataGridViewRow apprw in dgvApps.Rows)
                        {
                            if ((bool)apprw.Cells[0].Value)
                                doc += "\n" + apprw.Cells[1].Value.ToString();
                        }
                    }
                    lst.Add(doc);
                }
            }
            while (lst.Count < 6)
            {
                lst.Add("____________________________________________________________________________________________________");
            }
            Print.PrintDocumentList(gPersonId, lst, tbStaff.Text);
        }
    }
}
