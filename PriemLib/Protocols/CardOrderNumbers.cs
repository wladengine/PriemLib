﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

using BaseFormsLib;
using EducServLib;

namespace PriemLib
{
    public partial class CardOrderNumbers : BaseForm
    {
        private DBPriem _bdc;
        private bool forUpdate;
        //конструктор
        public CardOrderNumbers()
        {
            InitializeComponent();
            
            this.CenterToParent();
            this.MdiParent = MainClass.mainform;
           
            InitControls();            
        }

        //дополнительная инициализация контролов
        private void InitControls()
        {
            InitFocusHandlers();
            _bdc = MainClass.Bdc;
            forUpdate = false;

            ComboServ.FillCombo(cbStudyLevelGroup, HelpClass.GetComboListByTable("ed.StudyLevelGroup", "ORDER BY Acronym"), false, false);
            ComboServ.FillCombo(cbStudyBasis, HelpClass.GetComboListByTable("ed.StudyBasis", "ORDER BY Name"), false, false);
            ComboServ.FillCombo(cbSigner, HelpClass.GetComboListByTable("ed.Signer", "ORDER BY Name"), false, false);

            cbStudyBasis.SelectedIndex = 0;
            
            FillFaculty();
            FillStudyForm();
            FillLicenseProgram();

            UpdateDataGrid();

            cbStudyLevelGroup.SelectedIndexChanged += cbStudyLevelGroup_SelectedIndexChanged;
            cbFaculty.SelectedIndexChanged += new EventHandler(cbFaculty_SelectedIndexChanged);
            cbStudyForm.SelectedIndexChanged += new EventHandler(cbStudyForm_SelectedIndexChanged);
            cbStudyBasis.SelectedIndexChanged += new EventHandler(cbStudyBasis_SelectedIndexChanged);
            cbLicenseProgram.SelectedIndexChanged += new EventHandler(cbLicenseProgram_SelectedIndexChanged); 
           
        }

        void cbStudyLevelGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillFaculty();
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
        public int? StudyLevelGroupId
        {
            get { return ComboServ.GetComboIdInt(cbStudyLevelGroup); }
            set { ComboServ.SetComboId(cbStudyLevelGroup, value); }
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

        public int? SignerId
        {
            get { return ComboServ.GetComboIdInt(cbSigner); }
            set { ComboServ.SetComboId(cbSigner, value); }
        }
        public string ComissionNumber
        {
            get { return tbComissionNumber.Text.Trim(); }
            set { tbComissionNumber.Text = value; }
        }
        public DateTime? ComissionDate
        {
            get 
            { 
                return dtpComissionDate.Value.Date; 
            }
            set 
            {
                if (value.HasValue)
                {
                    try
                    {
                        dtpComissionDate.Value = value.Value;
                    }
                    catch { }
                }
            }
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

        private void FillFaculty()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var ent = context.extEntry.Where(c => c.StudyLevelGroupId == StudyLevelGroupId);

                ent = ent.Where(c => c.IsSecond == IsSecond && c.IsReduced == IsReduced && c.IsParallel == IsParallel);

                List<KeyValuePair<string, string>> lst = ent.ToList().OrderBy(x => x.FacultyName)
                    .Select(u => new KeyValuePair<string, string>(u.FacultyId.ToString(), u.FacultyName))
                    .Distinct().ToList();

                ComboServ.FillCombo(cbFaculty, lst, false, false);
            }
        }
        private void FillStudyForm()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var ent = context.extEntry.Where(c => c.FacultyId == FacultyId).Where(c => c.StudyBasisId == StudyBasisId);

                ent = ent.Where(c => c.IsSecond == IsSecond && c.IsReduced == IsReduced && c.IsParallel == IsParallel);

                List<KeyValuePair<string, string>> lst = ent.ToList().Select(u => new KeyValuePair<string, string>(u.StudyFormId.ToString(), u.StudyFormName)).Distinct().ToList();

                ComboServ.FillCombo(cbStudyForm, lst, false, false);
            }
        }
        private void FillSigner()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lst = context.Signer.Select(x => new { x.Id, x.Name }).ToList().Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).ToList();
                ComboServ.FillCombo(cbSigner, lst, false, false);
            }
        }
        private void FillLicenseProgram()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var ent = context.extEntry.Where(c => c.FacultyId == FacultyId);

                ent = ent.Where(c => c.IsSecond == IsSecond && c.IsReduced == IsReduced && c.IsParallel == IsParallel);

                if (StudyBasisId != null)
                    ent = ent.Where(c => c.StudyBasisId == StudyBasisId);
                if (StudyFormId != null)
                    ent = ent.Where(c => c.StudyFormId == StudyFormId);
                if (StudyLevelGroupId != null)
                    ent = ent.Where(c => c.StudyLevelGroupId == StudyLevelGroupId);

                List<KeyValuePair<string, string>> lst = ent
                    .ToList()
                    .OrderBy(x => x.LicenseProgramCode)
                    .Select(u => new KeyValuePair<string, string>(u.LicenseProgramId.ToString(), u.LicenseProgramCode + " " + u.LicenseProgramName))
                    .Distinct()
                    .OrderBy(x => x.Value)
                    .ToList();

                ComboServ.FillCombo(cbLicenseProgram, lst, false, true);
            }
        }        

        private void UpdateDataGrid()
        {
            gbOrders.Visible = true;
            gbOrdersFor.Visible = true;

            if (StudyFormId == null || StudyBasisId == null)
            {
                dgvViews.DataSource = null;
                gbOrders.Visible = false;
                gbOrdersFor.Visible = false;
                return;
            }

            string query = string.Format(@"SELECT DISTINCT Id, Number as 'Номер представления' 
FROM ed.extEntryView 
WHERE StudyFormId={0} AND StudyBasisId = {1} AND FacultyId = {2} {3} AND IsListener = {4} AND IsSecond = {5} AND IsReduced = {6} AND IsParallel = {7} AND StudyLevelGroupId = {8}
AND IsForeign = {9}
order by 2", 
                               StudyFormId, 
                               StudyBasisId, 
                               FacultyId, 
                               LicenseProgramId.HasValue ? string.Format(" AND LicenseProgramId = {0}", LicenseProgramId) : "", 
                               QueryServ.StringParseFromBool(IsListener), 
                               QueryServ.StringParseFromBool(IsSecond), 
                               QueryServ.StringParseFromBool(IsReduced), 
                               QueryServ.StringParseFromBool(IsParallel),
                               StudyLevelGroupId,
                               MainClass.dbType == PriemType.PriemForeigners ? "1" : "0");
            HelpClass.FillDataGrid(dgvViews, _bdc, query, "");  

            dgvViews.Columns["Номер представления"].Width = 149;
            dgvViews.Update();

            if (dgvViews.RowCount == 0)
            {
                gbOrders.Visible = false;
                gbOrdersFor.Visible = false;
            }           
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvViews_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e == null || e.RowIndex < 0)
            {
                gbOrders.Visible = false;
                gbOrdersFor.Visible = false;                
                return;
            }
            else
            {
                gbOrders.Visible = true;
                gbOrdersFor.Visible = true;
                tbOrderNum.Text = "";
                tbOrderNumFor.Text = "";
                dtOrderDate.Value = DateTime.Now;
                dtOrderDateFor.Value = DateTime.Now;
                
                string protId = dgvViews.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                FillOrders(protId);
            }
        }

        private void FillOrders(string protId)
        {
            DataSet ds = _bdc.GetDataSet(
                string.Format(@"SELECT ed.OrderNumbers.OrderDateFor, ed.OrderNumbers.OrderNumFor, ed.OrderNumbers.OrderDate, ed.OrderNumbers.OrderNum, SignerId, ComissionDate, ComissionNumber 
                                FROM ed.OrderNumbers WHERE ed.OrderNumbers.ProtocolId = '{0}'", protId));
                       
            if (ds.Tables[0].Rows.Count == 0)
            {
                forUpdate = false;
                DeleteReadOnly();                
            }                
            else
            {
                forUpdate = true;

                DataRow rw = ds.Tables[0].Rows[0];
                if ((rw["OrderDate"].ToString()).Length > 0)
                    dtOrderDate.Value = DateTime.Parse(rw["OrderDate"].ToString());
                if ((rw["OrderDateFor"].ToString()).Length > 0)
                    dtOrderDateFor.Value = DateTime.Parse(rw["OrderDateFor"].ToString());
                if ((rw["ComissionDate"].ToString()).Length > 0)
                    dtpComissionDate.Value = DateTime.Parse(rw["ComissionDate"].ToString());

                int tmp = 0;
                if (int.TryParse(rw["SignerId"].ToString(), out tmp))
                    SignerId = tmp;
                
                tbOrderNum.Text = rw["OrderNum"].ToString();
                tbOrderNumFor.Text = rw["OrderNumFor"].ToString();
                tbComissionNumber.Text = rw["ComissionNumber"].ToString();

                Guid ProtocolId = Guid.Empty;
                Guid.TryParse(protId, out ProtocolId);

                var protinfo = ProtocolDataProvider.GetProtocolInfo(ProtocolId, 4);
                if (protinfo != null)
                {
                    var prot = ProtocolDataProvider.GetEntryViewData(ProtocolId, null, true);
                    var prot_for = ProtocolDataProvider.GetEntryViewData(ProtocolId, null, false);

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

                SetReadOnly();
            } 
        }

        private void SetReadOnly()
        {
            dtOrderDate.Enabled = false;
            dtOrderDateFor.Enabled = false;
            tbOrderNum.Enabled = false;
            tbOrderNumFor.Enabled = false;
            btnChange.Enabled = true;
            btnSave.Enabled = false;
        }
        private void DeleteReadOnly()
        {
            dtOrderDate.Enabled = true;
            dtOrderDateFor.Enabled = true;
            tbOrderNum.Enabled = true;
            tbOrderNumFor.Enabled = true;
            btnChange.Enabled = false;
            btnSave.Enabled = true;
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            DeleteReadOnly();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MainClass.IsPasha() && CheckInput())
            {
                using (PriemEntities context = new PriemEntities())
                {
                    Guid? protId = new Guid(dgvViews.CurrentRow.Cells["Id"].Value.ToString());

                    if (forUpdate)                   
                        context.OrderNumbers_Update(protId, dtOrderDate.Value.Date, tbOrderNum.Text.Trim(), dtOrderDateFor.Value.Date, tbOrderNumFor.Text.Trim(), SignerId, ComissionDate, ComissionNumber);                    
                    else
                        context.OrderNumbers_Insert(protId, dtOrderDate.Value.Date, tbOrderNum.Text.Trim(), dtOrderDateFor.Value.Date, tbOrderNumFor.Text.Trim(), SignerId, ComissionDate, ComissionNumber);
                   
                    forUpdate = true;
                    SetReadOnly();
                }
            }
        }

        private bool CheckInput()
        {
            List<string> lstProt = new List<string>();
            if (!string.IsNullOrEmpty(tbOrderNum.Text.Trim()))
                lstProt.Add(tbOrderNum.Text.Trim());
            if (!string.IsNullOrEmpty(tbOrderNumFor.Text.Trim()))
                lstProt.Add(tbOrderNumFor.Text.Trim());

            Guid? protId = new Guid(dgvViews.CurrentRow.Cells["Id"].Value.ToString());

            using (PriemEntities context = new PriemEntities())
            {
                foreach (string sProtNum in lstProt)
                {
                    var EVs = (from EV in context.extEntryView
                               join Ent in context.extEntry on EV.EntryId equals Ent.Id
                               where (EV.OrderNum == sProtNum || EV.OrderNumFor == sProtNum)
                               && EV.Id != protId
                               select new
                               {
                                   Ent.LicenseProgramCode,
                                   Ent.LicenseProgramName,
                                   Ent.StudyFormName,
                                   Ent.StudyBasisName
                               }).Distinct().ToList();

                    if (EVs.Count > 0)
                    {
                        WinFormsServ.Error("Данный номер приказа уже указан в представлении для:\n" 
                            + EVs.Select(x => x.LicenseProgramCode + " " + x.LicenseProgramName + " (" + x.StudyBasisName + "," + x.StudyFormName + ")").ToList().Aggregate((x, tail) => x + "\n" + tail));

                        return false;
                    }
                }
            }

            return true;
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
    }
}