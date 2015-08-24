﻿using BaseFormsLib;
using EducServLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

namespace PriemLib
{
    public partial class DisEntryFromReEnterViewList : BaseForm
    {
        private DBPriem _bdc;
        private string sQuery;
        protected ProtocolRefreshHandler prh = null;

        public DisEntryFromReEnterViewList()
        {            
            this.CenterToParent();
            this.MdiParent = MainClass.mainform;

            this.sQuery = @"SELECT DISTINCT ed.extPerson.Id, ed.extPerson.PersonNum as Ид_номер, ed.extPerson.Surname AS Фамилия, ed.extPerson.Name AS Имя, ed.extPerson.SecondName AS Отчество, 
                            ed.extPerson.BirthDate AS Дата_рождения FROM ed.extPerson INNER JOIN ed.ExamsVedHistory ON ed.ExamsVedHistory.PersonId = ed.extPerson.Id ";

            InitializeComponent();
            InitControls();

            if (!MainClass.IsPasha())
            {
                btnPrintOrder.Visible = btnPrintOrder.Enabled = btnCancelView.Enabled = btnCancelView.Visible = false;
                chbIsForeign.Visible = false;
                btnCreateOrder.Visible = btnCreateOrder.Enabled = false;
            }

            if (MainClass.IsReadOnly())
            {
                btnCreate.Visible = btnCreate.Enabled = false;
                btnPrintOrder.Visible = btnPrintOrder.Enabled = true;
            }
        }

        //дополнительная инициализация контролов
        private void InitControls()
        {
            InitFocusHandlers();
            _bdc = MainClass.Bdc;

            ComboServ.FillCombo(cbFaculty, HelpClass.GetComboListByTable("ed.qFaculty", "ORDER BY Acronym"), false, false);
            ComboServ.FillCombo(cbStudyBasis, HelpClass.GetComboListByTable("ed.StudyBasis", "ORDER BY Name"), false, false);

            cbStudyBasis.SelectedIndex = 0;
            FillStudyLevelGroup();
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

        void cbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStudyForm();
        }
        void cbStudyBasis_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStudyForm();
        }
        void cbStudyForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillLicenseProgram();
        }
        void cbLicenseProgram_SelectedIndexChanged(object sender, EventArgs e)
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
                var ent = MainClass.GetEntry(context).Select(x => new { x.StudyLevelGroupId, x.StudyLevelGroupName }).ToList();

                List<KeyValuePair<string, string>> lst = ent.Select(u => new KeyValuePair<string, string>(u.StudyLevelGroupId.ToString(), u.StudyLevelGroupName)).Distinct().ToList();

                ComboServ.FillCombo(cbStudyLevelGroup, lst, false, false);
            }
        }
        private void FillStudyForm()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var ent = MainClass.GetEntry(context).Where(c => c.FacultyId == FacultyId).Where(c => c.StudyBasisId == StudyBasisId);

                ent = ent.Where(c => c.IsSecond == IsSecond && c.IsReduced == IsReduced && c.IsParallel == IsParallel);

                List<KeyValuePair<string, string>> lst = ent.ToList().Select(u => new KeyValuePair<string, string>(u.StudyFormId.ToString(), u.StudyFormName)).Distinct().ToList();

                ComboServ.FillCombo(cbStudyForm, lst, false, false);
            }
        }
        private void FillLicenseProgram()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var ent = MainClass.GetEntry(context).Where(c => c.FacultyId == FacultyId);

                ent = ent.Where(c => c.IsSecond == IsSecond && c.IsReduced == IsReduced && c.IsParallel == IsParallel);

                if (StudyBasisId != null)
                    ent = ent.Where(c => c.StudyBasisId == StudyBasisId);
                if (StudyFormId != null)
                    ent = ent.Where(c => c.StudyFormId == StudyFormId);

                List<KeyValuePair<string, string>> lst = ent.ToList().Select(u => new KeyValuePair<string, string>(u.LicenseProgramId.ToString(), u.LicenseProgramName)).Distinct().ToList();

                ComboServ.FillCombo(cbLicenseProgram, lst, false, false);
            }
        }
        
        private void UpdateDataGrid()
        {
            if (StudyFormId == null || StudyBasisId == null)
            {
                dgvViews.DataSource = null;
                return;
            }
            string query = string.Format("SELECT DISTINCT Id, Number as 'Номер представления' FROM ed.extDisEntryFromReEnterView WHERE StudyFormId={0} AND StudyBasisId={1} AND FacultyId= {2} AND LicenseProgramId = {3} AND IsListener = {4} AND IsSecond = {5} AND IsReduced = {6} AND IsParallel = {7} order by 2", StudyFormId, StudyBasisId, FacultyId, LicenseProgramId, QueryServ.StringParseFromBool(IsListener), QueryServ.StringParseFromBool(IsSecond), QueryServ.StringParseFromBool(IsReduced), QueryServ.StringParseFromBool(IsParallel));
            HelpClass.FillDataGrid(dgvViews, _bdc, query, ""); 
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            new DisEntryFromReEnterViewProtocol(null, StudyLevelGroupId.Value, FacultyId.Value, StudyBasisId.Value, StudyFormId.Value, LicenseProgramId, IsSecond, IsReduced, IsParallel, IsListener).Show();                      
        }

        private void btnCancelView_Click(object sender, EventArgs e)
        {
            if (!MainClass.IsPasha())
                return;

            using (PriemEntities context = new PriemEntities())
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

        private void btnPrintOrder_Click(object sender, EventArgs e)
        {
            if (dgvViews.CurrentRow == null || dgvViews.CurrentRow.Index < 0)
                return;

            string protocolId = dgvViews.CurrentRow.Cells["Id"].Value.ToString();

            Print.PrintDisEntryFromReEnterOrder(protocolId, !chbIsForeign.Checked);
        }

        private void DisEntryViewList_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainClass.RemoveProtocolHandler(prh);
        }

        private void btnCreateOrder_Click(object sender, EventArgs e)
        {
            if (MainClass.IsPasha())
            {
                if (dgvViews.CurrentRow == null || dgvViews.CurrentRow.Index < 0)
                    return;

                if (MessageBox.Show("Отчислить людей из выбранного представления?", "Внимание!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (PriemEntities context = new PriemEntities())
                    {
                        Guid? protocolId = new Guid(dgvViews.CurrentRow.Cells["Id"].Value.ToString());

                        if (MessageBox.Show("Перенести оригиналы на другие доступные конкурсы (если это возможно, переносится на максимальный доступный приоритет)?",
                            "Внимание!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            var abitLst = context.ProtocolHistory.Where(x => x.ProtocolId == protocolId).Select(x => x.AbiturientId).ToList();
                            foreach (Guid abId in abitLst)
                            {
                                try
                                {
                                    ApplicationDataProvider.ChangeHasOriginalsDestination(abId, null);
                                }
                                catch (Exception ex)
                                {
                                    WinFormsServ.Error(ex);
                                }
                            }
                        }

                        using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            context.EntryView_UpdateDisEntry(protocolId);
                            context.Abiturient_UpdateBackDocByDisEntry(true, DateTime.Now.Date, protocolId);

                            MessageBox.Show("ОТЧИСЛЕНЫ! ГЫ-ГЫ");

                            transaction.Complete();
                        }
                    }
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (dgvViews.CurrentRow == null || dgvViews.CurrentRow.Index < 0)
                return;

            string protocolId = dgvViews.CurrentRow.Cells["Id"].Value.ToString();

            Print.PrintDisEntryFromReEnterView(protocolId);
        }

        private void chbIsListener_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }

        private void chbIsSecond_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }
    }
}
