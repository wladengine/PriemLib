using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BaseFormsLib;
using EducServLib;
using WordOut;

namespace PriemLib
{
    public partial class EntryViewList : BaseForm
    {
        public DBPriem _bdc;
        private string sQuery;
        protected ProtocolRefreshHandler prh = null;

        public EntryViewList()
        {            
            this.CenterToParent();
            this.MdiParent = MainClass.mainform;

            //this.sQuery = string.Format("SELECT DISTINCT Person.Id, {0} as Ид_номер, Person.Surname AS Фамилия, Person.Name AS Имя, Person.SecondName AS Отчество, Person.BirthDate AS Дата_рождения " +
            //                       "FROM Person INNER JOIN ExamsVedHistory ON ExamsVedHistory.PersonId = Person.Id ", MainClass.GetStringPersonNumber());

            InitializeComponent();
            InitControls();
            
            btnPrintOrder.Visible = btnPrintOrder.Enabled = btnCancelView.Enabled = btnCancelView.Visible = false;
            chbIsForeign.Visible = false;

            btnCreate.Enabled = true;

            if (MainClass.IsPasha())
            {                
                btnPrintOrder.Visible = btnPrintOrder.Enabled = btnCancelView.Enabled = btnCancelView.Visible = true;
                chbIsForeign.Visible = true;
               
                //btnCreate.Enabled = false;
            } 

            if (MainClass.IsPrintOrder())
            {
                btnPrintOrder.Visible = btnPrintOrder.Enabled = chbIsForeign.Visible = true;
                btnCreate.Visible = false;
            }

            //если кнопки "печать приказа" не видно, то галочка "иностранцы" съезжает на место кнопки
            if (!MainClass.IsPasha() && !MainClass.IsPrintOrder())
            {
                chbIsForeign.Location = new Point(28, 515);
                chbIsForeign.Visible = chbIsForeign.Enabled = true;
            }
            string qquery = "SELECT CONVERT(nvarchar, Id) AS Id, '№' + Number + ' от ' + CONVERT(nvarchar, [Date], 104) AS Name FROM ed.AdmissionProtocol";
            ComboServ.FillCombo(cbAdmissionProtocol, HelpClass.GetComboListByQuery(qquery), true, false);

            //// посомтреть, почему отдельные факультеты
            //if (_bdc.IsMed() || _bdc.GetFacultyId() == "9" || _bdc.GetFacultyId() == "14" || _bdc.GetFacultyId() == "20")
            //    btnCreate.Enabled = true;

            //if (_bdc.IsReadOnly())
            //    btnPrintOrder.Visible = btnPrintOrder.Enabled = true;
        }

        //дополнительная инициализация контролов
        public virtual void  InitControls()
        {
            InitFocusHandlers();
            _bdc = MainClass.Bdc;

            ComboServ.FillCombo(cbStudyLevelGroup, HelpClass.GetComboListByTable("ed.StudyLevelGroup", string.Format("WHERE Id IN ({0})", Util.BuildStringWithCollection(MainClass.lstStudyLevelGroupId))), false, false);
            ComboServ.FillCombo(cbFaculty, HelpClass.GetComboListByTable("ed.qFaculty", "ORDER BY Acronym"), false, false);
            ComboServ.FillCombo(cbStudyBasis, HelpClass.GetComboListByTable("ed.StudyBasis", "ORDER BY Name"), false, false);

            cbStudyBasis.SelectedIndex = 0;
            FillStudyForm();
            FillLicenseProgram();
            UpdateDataGrid();


            cbFaculty.SelectedIndexChanged += new EventHandler(cbFaculty_SelectedIndexChanged);
            cbStudyForm.SelectedIndexChanged += new EventHandler(cbStudyForm_SelectedIndexChanged);
            cbStudyBasis.SelectedIndexChanged += new EventHandler(cbStudyBasis_SelectedIndexChanged);
            cbLicenseProgram.SelectedIndexChanged += new EventHandler(cbLicenseProgram_SelectedIndexChanged);              

            prh = new ProtocolRefreshHandler(UpdateDataGrid);
            MainClass.AddProtocolHandler(prh);          
        }

        public void cbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStudyForm();            
        }
        public void cbStudyBasis_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStudyForm();            
        }
        public void cbStudyForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillLicenseProgram();
        }
        public void cbLicenseProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }

        public int? FacultyId
        {
            get { return ComboServ.GetComboIdInt(cbFaculty); }
            set { ComboServ.SetComboId(cbFaculty, value); }
        }
        public int? LicenseProgramId
        {
            get { return ComboServ.GetComboIdInt(cbLicenseProgram); }
            set { ComboServ.SetComboId(cbLicenseProgram, value); }
        }
        public int? StudyBasisId
        {
            get { return ComboServ.GetComboIdInt(cbStudyBasis); }
            set { ComboServ.SetComboId(cbStudyBasis, value); }
        }
        public int? StudyFormId
        {
            get { return ComboServ.GetComboIdInt(cbStudyForm); }
            set { ComboServ.SetComboId(cbStudyForm, value); }
        }
        public int? StudyLevelGroupId
        {
            get { return ComboServ.GetComboIdInt(cbStudyLevelGroup); }
            set { ComboServ.SetComboId(cbStudyLevelGroup, value); }
        }

        public bool IsSecond
        {
            get { return chbIsSecond.Checked; }
            set { chbIsSecond.Checked = value; }
        }
        public bool IsReduced
        {
            get { return chbIsReduced.Checked; }
            set { chbIsReduced.Checked = value; }
        }
        public bool IsParallel
        {
            get { return chbIsParallel.Checked; }
            set { chbIsParallel.Checked = value; }
        }
        public bool IsListener
        {
            get { return chbIsListener.Checked; }
            set { chbIsListener.Checked = value; }
        }

        private void FillStudyLevelGroup()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var ent = MainClass.GetEntry(context).Select(x => new { x.StudyLevelGroupId, x.StudyLevelGroupName });
                List<KeyValuePair<string, string>> lst = ent.ToList().Select(u => new KeyValuePair<string, string>(u.StudyLevelGroupId.ToString(), u.StudyLevelGroupName)).Distinct().ToList();
                ComboServ.FillCombo(cbStudyLevelGroup, lst, false, false);
            }
        }
        public void FillStudyForm()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var ent = MainClass.GetEntry(context).Where(c => c.FacultyId == FacultyId).Where(c => c.StudyBasisId == StudyBasisId);

                ent = ent.Where(c => c.IsSecond == IsSecond && c.IsReduced == IsReduced && c.IsParallel == IsParallel);

                List<KeyValuePair<string, string>> lst = ent.ToList().Select(u => new KeyValuePair<string, string>(u.StudyFormId.ToString(), u.StudyFormName)).Distinct().ToList();

                ComboServ.FillCombo(cbStudyForm, lst, false, false);
            }
        }
        public virtual void FillLicenseProgram()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var ent = MainClass.GetEntry(context).Where(c => c.FacultyId == FacultyId);

                ent = ent.Where(c => c.IsSecond == IsSecond && c.IsReduced == IsReduced && c.IsParallel == IsParallel);

                if (StudyBasisId != null)
                    ent = ent.Where(c => c.StudyBasisId == StudyBasisId);
                if (StudyFormId != null)
                    ent = ent.Where(c => c.StudyFormId == StudyFormId);

                List<KeyValuePair<string, string>> lst = ent.ToList().Select(u => new KeyValuePair<string, string>(u.LicenseProgramId.ToString(), u.LicenseProgramCode + " " + u.LicenseProgramName)).Distinct().ToList();

                ComboServ.FillCombo(cbLicenseProgram, lst, false, false);
            }
        }

        public virtual void UpdateDataGrid()
        {
            if (StudyFormId == null || StudyBasisId == null)
            {
                dgvViews.DataSource = null;
                return;
            }

            string query = string.Format(@"SELECT DISTINCT Id, Number + ' (' + CONVERT(nvarchar, Date, 104) + ')' AS 'Номер представления' 
FROM ed.extEntryView 
WHERE StudyFormId={0} AND StudyBasisId={1} AND FacultyId= {2} AND LicenseProgramId = {3} AND IsListener = {4} AND IsSecond = {5} 
AND IsReduced = {6} AND IsParallel = {7} AND IsForeign = {8} 
order by 2", 
                               StudyFormId, 
                               StudyBasisId, 
                               FacultyId, 
                               LicenseProgramId, 
                               QueryServ.StringParseFromBool(IsListener), 
                               QueryServ.StringParseFromBool(IsSecond), 
                               QueryServ.StringParseFromBool(IsReduced), 
                               QueryServ.StringParseFromBool(IsParallel), 
                               QueryServ.StringParseFromBool(MainClass.dbType == PriemType.PriemForeigners));
            HelpClass.FillDataGrid(dgvViews, _bdc, query, "");

            if (dgvViews.Columns.Contains("Номер представления"))
                dgvViews.Columns["Номер представления"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (dgvViews.Rows.Count == 0)
            {
                dgvViews.CurrentCell = null;
                ViewProtocolInfo();
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            OpenCardProtocol();
        }
        protected virtual void OpenCardProtocol()
        {
            new EntryViewProtocol(null,
                   StudyLevelGroupId.Value,
                   FacultyId.Value,
                   StudyBasisId.Value,
                   StudyFormId.Value,
                   LicenseProgramId,
                   IsSecond,
                   IsReduced,
                   IsParallel,
                   IsListener,
                   chbCel.Checked).Show();
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {  
            if (dgvViews.CurrentRow == null)
                return;

            if (dgvViews.CurrentRow.Index < 0)
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "ADOBE Pdf files|*.pdf";
            sfd.FileName = "Представление к зачислению - " +  dgvViews.CurrentRow.Cells["Номер представления"].Value.ToString() + ".pdf";
            if (sfd.ShowDialog() == DialogResult.OK)
                Print.PrintEntryView(dgvViews.CurrentRow.Cells["Id"].Value.ToString(), sfd.FileName, !chbIsForeign.Checked);
        }
        protected virtual void btnPrintOrder_Click(object sender, EventArgs e)
        {
            if (dgvViews.CurrentRow == null)
                return;

            if (dgvViews.CurrentRow.Index < 0)
                return;

            Guid protocolId = (Guid)dgvViews.CurrentRow.Cells["Id"].Value;

            Print.PrintOrder(protocolId, !chbIsForeign.Checked, chbCel.Checked);
        }
        protected virtual void btnOrderReview_Click(object sender, EventArgs e)
        {
            if (dgvViews.CurrentRow == null)
                return;

            if (dgvViews.CurrentRow.Index < 0)
                return;

            Guid protocolId = (Guid)dgvViews.CurrentRow.Cells["Id"].Value;

            Print.PrintOrderReview(protocolId, null, !chbIsForeign.Checked);
        }
        private void btnCancelView_Click(object sender, EventArgs e)
        {
            if(!MainClass.IsPasha())
                return;

            using(PriemEntities context = new PriemEntities())
            {
                if (dgvViews.CurrentRow == null || dgvViews.CurrentRow.Index < 0)
                    return;

                if (MessageBox.Show("Отменить выделенное представление", "Внимание!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Guid? protocolId = new Guid(dgvViews.CurrentRow.Cells["Id"].Value.ToString());
                    context.Protocol_UpdateIsOld(true, protocolId);                   
                    MessageBox.Show("Представление отменено");
                    UpdateDataGrid();
                }
            }
        }

        private void EntryViewList_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainClass.RemoveProtocolHandler(prh);
        }
        
        private void chbIsListener_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }
        private void chbIsSecond_CheckedChanged(object sender, EventArgs e)
        {
            FillStudyForm();
        }
        private void chbIsReduced_CheckedChanged(object sender, EventArgs e)
        {
            FillStudyForm();
        }
        private void chbIsParallel_CheckedChanged(object sender, EventArgs e)
        {
            FillStudyForm();
        }

        private void dgvViews_SelectionChanged(object sender, EventArgs e)
        {
            ViewProtocolInfo();
        }
        
        public virtual void ViewProtocolInfo()
        {
            if (dgvViews.CurrentCell != null)
            {
                gbProtocolInfo.Visible = true;
                btnSetAdmissionProtocol.Visible = MainClass.IsPasha() || MainClass.IsEntryChanger() || MainClass.IsPrintOrder();
                btnSetAdmissionProtocol.Enabled = MainClass.IsPasha() || MainClass.IsEntryChanger() || MainClass.IsPrintOrder();

                if (dgvViews.CurrentCell.RowIndex >= 0)
                {
                    int rwInd = dgvViews.CurrentCell.RowIndex;
                    Guid ProtocolId = (Guid)dgvViews["Id", rwInd].Value;
                    var protinfo = ProtocolDataProvider.GetProtocolInfo(ProtocolId, 4);
                    if (protinfo != null)
                    {
                        var prot = ProtocolDataProvider.GetEntryViewData(ProtocolId, null, true);
                        var prot_for = ProtocolDataProvider.GetEntryViewData(ProtocolId, null, false);

                        ComboServ.SetComboId(cbAdmissionProtocol, protinfo.AdmissionProtocolId);

                        lblHasForeigners.Visible = prot_for.Count > 0;
                        lblProtocolPersonsCount.Text = (prot.Count + prot_for.Count).ToString();

                        List<int> lstComps = new List<int>();
                        lstComps.AddRange(prot.Select(x => x.CompetitionId).ToList().Distinct());

                        using (PriemEntities context = new PriemEntities())
                        {
                            var comps = context.Competition.Where(x => lstComps.Contains(x.Id)).Select(x => x.Name).ToList().DefaultIfEmpty("").Aggregate((x, tail) => x + ", " + tail);
                            lblProtocolCompetitions.Text = comps;
                        }
                    }
                    else
                        gbProtocolInfo.Visible = false;
                }
            }
            else
                gbProtocolInfo.Visible = false;
            
        }

        private void btnSetAdmissionProtocol_Click(object sender, EventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                if (dgvViews.CurrentCell.RowIndex >= 0)
                {
                    int rwInd = dgvViews.CurrentCell.RowIndex;
                    Guid ProtocolId = (Guid)dgvViews["Id", rwInd].Value;

                    var p = context.Protocol.Where(x => x.Id == ProtocolId).FirstOrDefault();
                    if (p != null)
                    {
                        int? AdmissionProtocolId = ComboServ.GetComboIdInt(cbAdmissionProtocol);
                            p.AdmissionProtocolId = AdmissionProtocolId;

                        context.SaveChanges();

                        MessageBox.Show("OK");
                    }
                }
            }
        }
    }
}
