using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EducServLib;
using System.Data.Entity.Core.Objects;
using System.Transactions;

namespace PriemLib
{
    public partial class CardEntry : BookCard
    {
        #region Fields
        public int StudyFormId
        {
            get { return ComboServ.GetComboIdInt(cbStudyForm).Value; }
            set { ComboServ.SetComboId(cbStudyForm, value); }
        }
        public int StudyBasisId
        {
            get { return ComboServ.GetComboIdInt(cbStudyBasis).Value; }
            set { ComboServ.SetComboId(cbStudyBasis, value); }
        }
        public int StudyLevelId
        {
            get { return ComboServ.GetComboIdInt(cbStudyLevel).Value; }
            set { ComboServ.SetComboId(cbStudyLevel, value); }
        }
        public int? FacultyId
        {
            get { return ComboServ.GetComboIdInt(cbFaculty); }
            set { ComboServ.SetComboId(cbFaculty, value); }
        }
        public bool IsSecond
        {
            get { return chbIsSecond.Checked; }
            set { chbIsSecond.Checked = value; }
        }
        public bool IsParallel
        {
            get { return chbIsParallel.Checked; }
            set { chbIsParallel.Checked = value; }
        }
        public bool IsReduced
        {
            get { return chbIsReduced.Checked; }
            set { chbIsReduced.Checked = value; }
        }
        public bool IsForeign
        {
            get { return chbIsForeign.Checked; }
            set { chbIsForeign.Checked = value; }
        }
        public bool IsCrimea
        {
            get { return chbIsCrimea.Checked; }
            set { chbIsCrimea.Checked = value; }
        }
        public int? AggregateGroupId
        {
            get { return ComboServ.GetComboIdInt(cbAggregateGroup); }
            set { ComboServ.SetComboId(cbAggregateGroup, value); }
        }
        public int? LicenseProgramId
        {
            get { return ComboServ.GetComboIdInt(cbLicenseProgram); }
            set { ComboServ.SetComboId(cbLicenseProgram, value); }
        }
        public int? ObrazProgramId
        {
            get { return ComboServ.GetComboIdInt(cbObrazProgram); }
            set { ComboServ.SetComboId(cbObrazProgram, value); }
        }
        public int? ProfileId
        {
            get { return ComboServ.GetComboIdInt(cbProfile); }
            set { ComboServ.SetComboId(cbProfile, value); }
        }
        public int? ComissionId
        {
            get { return ComboServ.GetComboIdInt(cbComission); }
            set { ComboServ.SetComboId(cbComission, value); }
        }
        public Guid? CompetitionGroupId
        {
            get { return ComboServ.GetComboIdGuid(cbCompetitionGroup); }
            set { ComboServ.SetComboId(cbCompetitionGroup, value); }
        }
        public Guid? ParentEntryId
        {
            get { return ComboServ.GetComboIdGuid(cbParentEntry); }
            set { ComboServ.SetComboId(cbParentEntry, value); }
        }

        public DateTime DateOfStart
        {
            //get { return dtpDateOfStart.Value.Date.AddHours(10); }
            get { return dtpDateOfStart.Value; }
            set { dtpDateOfStart.Value = value; }
        }
        public DateTime DateOfClose
        {
            //get { return dtpDateOfClose.Value.Date.AddHours(18); }
            get { return dtpDateOfClose.Value; }
            set { dtpDateOfClose.Value = value; }
        }

        public int KCP
        {
            get
            {
                int j;
                int.TryParse(tbKC.Text.Trim(), out j);
                return j;
            }
            set 
            {
                tbKC.Text = value.ToString();
            }
        }
        public int? KCPCel
        {
            get
            {
                int? iRet;
                int j;
                if (int.TryParse(tbKCPCel.Text.Trim(), out j))
                    iRet = j;
                else
                    iRet = null;

                return iRet;
            }
            set
            {
                if (value.HasValue)
                    tbKC.Text = value.ToString();
                else
                    tbKC.Text = "";
            }
        }
        public int? KCPQuota
        {
            get
            {
                int? iRet;
                int j;
                if (int.TryParse(tbKCPQuota.Text.Trim(), out j))
                    iRet = j;
                else
                    iRet = null;

                return iRet;
            }
            set
            {
                if (value.HasValue)
                    tbKCPQuota.Text = value.ToString();
                else
                    tbKCPQuota.Text = "";
            }
        }
        #endregion

        public CardEntry(string id)
            : base(id)
        {
            InitializeComponent();            
            InitControls();            
        }

        public CardEntry(int iStudyLevelId)
            : base()
        {
            InitializeComponent();
            InitControls();
            StudyLevelId = iStudyLevelId;
            UpdateAfterStudyLevel();
            UpdateAfterAggregateGroup();
            UpdateAfterLicenseProgram();
            UpdateAfterObrazProgram();
        }

        protected override void ExtraInit()
        {
            base.ExtraInit();
            _title = "Прием";
            _tableName = "ed.[Entry]";
            InitCombos();

            btnSaveAsNew.Visible = true;

            //DoSmth();
        }

        private void InitCombos()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = context.StudyForm.Select(x => new { x.Id, x.Name }).ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).ToList();
                ComboServ.FillCombo(cbStudyForm, src, false, false);
                src = context.StudyBasis.Select(x => new { x.Id, x.Name }).ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).ToList();
                ComboServ.FillCombo(cbStudyBasis, src, false, false);

                src = context.SP_Profile.Select(x => new { x.Id, x.Name }).ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).ToList();
                ComboServ.FillCombo(cbProfile, src, false, false);
                
                //src = context.SP_AggregateGroup.Select(x => new { x.Id, x.Name }).ToList()
                //    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).ToList();
                //ComboServ.FillCombo(cbAggregateGroup, src, false, false);

                src = context.StudyLevel.Select(x => new { x.Id, x.Name }).ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).ToList();
                ComboServ.FillCombo(cbStudyLevel, src, false, false);

                src = context.Comission.Select(x => new { x.Id, x.Name }).ToList()
                            .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).ToList();
                ComboServ.FillCombo(cbComission, src, true, false);

                UpdateAfterStudyLevel();
                UpdateAfterAggregateGroup();
                UpdateAfterLicenseProgram();
                UpdateAfterObrazProgram();
            }
        }

        //protected override void InitHandlers()
        //{
        //    cbStudyLevel.SelectedIndexChanged += cbStudyLevel_SelectedIndexChanged;
        //    cbLicenseProgram.SelectedIndexChanged += cbLicenseProgram_SelectedIndexChanged;
        //    cbObrazProgram.SelectedIndexChanged += cbObrazProgram_SelectedIndexChanged;
        //    cbProfile.SelectedIndexChanged += cbProfile_SelectedIndexChanged;
        //}
        //protected override void NullHandlers()
        //{
        //    cbStudyLevel.SelectedIndexChanged -= cbStudyLevel_SelectedIndexChanged;
        //    cbLicenseProgram.SelectedIndexChanged -= cbLicenseProgram_SelectedIndexChanged;
        //    cbObrazProgram.SelectedIndexChanged -= cbObrazProgram_SelectedIndexChanged;
        //    cbProfile.SelectedIndexChanged -= cbProfile_SelectedIndexChanged;
        //}

        private void UpdateAfterStudyLevel()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = context.SP_LicenseProgram.Where(x => x.StudyLevelId == StudyLevelId)
                    .Select(x => new { Id = x.AggregateGroupId, Name = x.SP_AggregateGroup.Name }).Distinct()
                    .ToList().Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).ToList();
                ComboServ.FillCombo(cbAggregateGroup, src, (src.Count == 0), false);
            }
        }
        private void UpdateAfterAggregateGroup()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = context.SP_LicenseProgram.Where(x => x.StudyLevelId == StudyLevelId && (AggregateGroupId.HasValue ? x.AggregateGroupId == AggregateGroupId : true))
                    .OrderBy(x => x.Code).Select(x => new { x.Id, x.Code, x.Name }).ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), "(" + x.Code + ") " + x.Name)).ToList();
                ComboServ.FillCombo(cbLicenseProgram, src, false, false);
            }
        }
        private void UpdateAfterLicenseProgram()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = context.SP_ObrazProgram.Where(x => x.LicenseProgramId == LicenseProgramId).Select(x => new { x.Id, Acr1 = x.SP_LicenseProgram.StudyLevel.Acronym, x.Number, x.Name })
                    .ToList().OrderBy(x => x.Acr1).ThenBy(x => x.Number)
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Acr1 + "." + x.Number + "." + MainClass.sPriemYear + " " + x.Name)).ToList();
                ComboServ.FillCombo(cbObrazProgram, src, false, false);
            }
        }
        private void UpdateAfterObrazProgram()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = context.SP_ObrazProgram.Where(x => x.LicenseProgramId == LicenseProgramId).Select(x => new { x.FacultyId, x.SP_Faculty.Name }).Distinct().ToList()
                    .Select(x => new KeyValuePair<string, string>(x.FacultyId.ToString(), x.Name)).ToList();
                ComboServ.FillCombo(cbFaculty, src, false, false);

                var src2 = context.CompetitiveGroup
                    .Where(x => x.LicenseProgramId == LicenseProgramId && x.ObrazProgramId == ObrazProgramId)
                    .Select(x => new { x.Id, x.Name })
                    .ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name))
                    .ToList();

                ComboServ.FillCombo(cbCompetitionGroup, src2, true, false);

                var src3 = context.extEntry
                    .Where(x => x.LicenseProgramId == LicenseProgramId && x.ObrazProgramId == ObrazProgramId && !x.IsForeign)
                    .Select(x => new { x.Id, x.LicenseProgramCode, x.ObrazProgramCrypt, x.ProfileName, x.StudyFormName, x.StudyBasisName, x.IsCrimea, x.IsParallel, x.IsReduced, x.IsSecond })
                    .ToList().Distinct()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), "(" + x.LicenseProgramCode + ") [" + x.ObrazProgramCrypt + "] " + x.ProfileName + " " + x.StudyFormName + " " 
                        + x.StudyBasisName + (x.IsCrimea ? " (крым)" : "") + (x.IsSecond ? " (для лиц с ВО)" : "") + (x.IsReduced ? " (сокр)" : "") + (x.IsParallel ? " (паралл)" : "")))
                    .ToList();

                ComboServ.FillCombo(cbParentEntry, src3, true, false);
            }
        }

        protected override void SetIsOpen()
        {
            return;
        }
        protected override void DeleteIsOpen()
        {
            return;
        }
        protected override bool GetIsOpen()
        {
            return false;
        }     

        protected override void FillCard()
        {
            if (_Id == null)
            {
                return;
            }
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                   Entry ent = (from ex in context.Entry
                                 where ex.Id == GuidId
                                       select ex).FirstOrDefault();

                    StudyLevelId = ent.StudyLevelId;
                    UpdateAfterStudyLevel();
                    //tbStudyLevel.Text = ent.StudyLevelName;
                    AggregateGroupId = ent.SP_LicenseProgram.AggregateGroupId;
                    UpdateAfterAggregateGroup();
                    LicenseProgramId = ent.LicenseProgramId;
                    UpdateAfterLicenseProgram();
                    ObrazProgramId = ent.ObrazProgramId;
                    UpdateAfterObrazProgram();
                    ProfileId = ent.ProfileId;
                    FacultyId = ent.FacultyId;
                    StudyFormId = ent.StudyFormId;
                    StudyBasisId = ent.StudyBasisId;
                    tbStudyPlan.Text = ent.StudyPlanNumber;
                    tbKC.Text = ent.KCP.ToString();
                    IsSecond = ent.IsSecond;
                    IsReduced = ent.IsReduced;
                    IsParallel = ent.IsParallel;
                    IsForeign = ent.IsForeign;
                    IsCrimea = ent.IsCrimea;

                    tbKCPCel.Text = ent.KCPCel.ToString();
                    KCPQuota = ent.KCPQuota;

                    ComissionId = ent.CommissionId;
                    var _toComp = ent.EntryToCompetitiveGroup.FirstOrDefault();
                    if (_toComp != null)
                        CompetitionGroupId = _toComp.CompetitiveGroupId;

                    ParentEntryId = ent.ParentEntryId;

                    DateOfStart = ent.DateOfStart;
                    DateOfClose = ent.DateOfClose;

                    UpdateExams();
                    UpdateOlympicsToCommonBenefit();
                    UpdateInnerEntryInEntry();
                    UpdateOlympResultToAdditionalMark();
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при заполнении формы ", exc);
            }
        }        

        protected override void SetReadOnlyFieldsAfterFill()
        {
            base.SetReadOnlyFieldsAfterFill();
            //if(_Id == null)
            //    btnSaveChange.Enabled = false;
            if (!MainClass.IsEntryChanger())
                btnSaveChange.Enabled = false;

            if (MainClass.IsEntryChanger())
            {
                WinFormsServ.SetSubControlsEnabled(tabPage4, true);
            }
        }

        protected override void SetAllFieldsEnabled()
        {
            base.SetAllFieldsEnabled();

            foreach (Control control in tcCard.Controls)
            {
                control.Enabled = true;
                foreach (Control crl in control.Controls)
                    crl.Enabled = true;
            }

            WinFormsServ.SetSubControlsEnabled(gbEntry, GetIsCanChangeEntry());
            tbKCPCel.Enabled = true;
            tbKC.Enabled = true;
            tbKCPQuota.Enabled = true;
            cbComission.Enabled = true;
            cbFaculty.Enabled = true;
            tbStudyPlan.Enabled = true;
        }

        private bool GetIsCanChangeEntry()
        {
            if (MainClass.IsPasha() || MainClass.IsOwner())
                return true;
            if (string.IsNullOrEmpty(_Id) && ((MainClass.IsPasha() || MainClass.IsOwner() || MainClass.IsEntryChanger())))
                return true;
            using (PriemEntities context = new PriemEntities())
            {
                return (context.Abiturient.Where(x => x.EntryId == GuidId).Count() == 0);
            }
        }

        protected override void SetAllFieldsNotEnabled()
        {
            base.SetAllFieldsNotEnabled();
            tcCard.Enabled = true;

            foreach (Control control in tcCard.Controls)
            {
                foreach (Control crl in control.Controls)
                    crl.Enabled = false;
            }

            gbBE.Enabled = true;
            //gbEntry.Enabled = true;
            gbExams.Enabled = true;
            gbOlympToAddMark.Enabled = true;
        }

        protected override void InsertRec(PriemEntities context, ObjectParameter idParam)
        {
            Guid Id = Guid.NewGuid();

            int _ProfileId = ComboServ.GetComboIdInt(cbProfile) ?? 0;


            var Entry = new Entry();
            Entry.Id = Id;
            Entry.FacultyId = FacultyId.Value;
            Entry.StudyLevelId = StudyLevelId;
            Entry.LicenseProgramId = LicenseProgramId.Value;
            Entry.ObrazProgramId = ObrazProgramId.Value;
            Entry.ProfileId = _ProfileId;
            Entry.StudyFormId = StudyFormId;
            Entry.StudyBasisId = StudyBasisId;
            if (!string.IsNullOrEmpty(tbStudyPlan.Text.Trim()))
                Entry.StudyPlanNumber = tbStudyPlan.Text.Trim();
            Entry.KCP = KCP;
            Entry.KCPCel = KCPCel;
            Entry.KCPQuota = KCPQuota;
            Entry.DateOfStart = DateOfStart;
            Entry.DateOfClose = DateOfClose;
            Entry.CommissionId = ComissionId;
            Entry.IsForeign = IsForeign;
            Entry.IsCrimea = IsCrimea;
            
            context.Entry.Add(Entry);

            if (CompetitionGroupId.HasValue)
            {
                var EntryToComp = new EntryToCompetitiveGroup();
                EntryToComp.Id = Guid.NewGuid();
                EntryToComp.EntryId = Id;
                EntryToComp.CompetitiveGroupId = CompetitionGroupId.Value;
                context.EntryToCompetitiveGroup.Add(EntryToComp);
            }

            context.SaveChanges();

            idParam.Value = Id;

            string query = @"INSERT INTO [_Entry] 
(Id, StudyLevelId, LicenseProgramId, ObrazProgramId, ProfileId, StudyFormId, StudyBasisId, FacultyId, SemesterId, CampaignYear,
DateOfStart, DateOfClose, ComissionId, IsForeign, IsCrimea) VALUES
(@Id, @StudyLevelId, @LicenseProgramId, @ObrazProgramId, @ProfileId, @StudyFormId, @StudyBasisId, @FacultyId, @SemesterId, @CampaignYear,
@DateOfStart, @DateOfClose, @ComissionId, @IsForeign, @IsCrimea)";
            SortedList<string, object> sl = new SortedList<string, object>();
            sl.Add("@Id", Id);

            sl.Add("@CampaignYear", MainClass.iPriemYear);
            sl.Add("@LicenseProgramId", LicenseProgramId);
            sl.Add("@ObrazProgramId", ObrazProgramId);
            sl.AddVal("@ProfileId", _ProfileId);
            sl.AddVal("@SemesterId", 1);

            sl.Add("@StudyLevelId", StudyLevelId);
            sl.Add("@StudyFormId", StudyFormId);
            sl.Add("@StudyBasisId", StudyBasisId);
            sl.AddVal("@FacultyId", FacultyId);
            sl.Add("@IsParallel", IsParallel);
            sl.Add("@IsReduced", IsReduced);
            sl.Add("@IsSecond", IsSecond);
            sl.Add("@IsForeign", IsForeign);
            sl.Add("@IsCrimea", IsCrimea);

            sl.AddVal("@DateOfStart", DateOfStart);
            sl.AddVal("@DateOfClose", DateOfClose);

            sl.AddVal("@ComissionId", ComissionId);

            MainClass.BdcOnlineReadWrite.ExecuteQuery(query, sl);
        }
        protected override void UpdateRec(PriemEntities context, Guid id)
        {
            context.Entry_UpdateCEl(GuidId, KCPCel);
            context.Entry_UpdateKC(GuidId, KCP, KCPQuota);
            context.Entry_Update(GuidId, StudyLevelId, StudyFormId, StudyBasisId, FacultyId, false, IsParallel, IsReduced, IsSecond, tbStudyPlan.Text.Trim(),
                DateOfStart, DateOfClose, ComissionId, IsForeign, IsCrimea);

            if (CompetitionGroupId.HasValue)
            {
                bool bIns = false;
                var EntryToComp = context.EntryToCompetitiveGroup.Where(x => x.EntryId == GuidId).FirstOrDefault();
                if (EntryToComp == null)
                {
                    bIns = true;
                    EntryToComp = new EntryToCompetitiveGroup();
                    EntryToComp.Id = Guid.NewGuid();
                    EntryToComp.EntryId = GuidId.Value;
                }
                
                EntryToComp.CompetitiveGroupId = CompetitionGroupId.Value;

                if (bIns)
                    context.EntryToCompetitiveGroup.Add(EntryToComp);
            }

            var Entry = context.Entry.Where(x => x.Id == GuidId).FirstOrDefault();
            if (Entry != null)
            {
                Entry.ParentEntryId = ParentEntryId;
                Entry.LicenseProgramId = LicenseProgramId.Value;
                Entry.ObrazProgramId = ObrazProgramId.Value;
                Entry.ProfileId = ProfileId.Value;
            }

            context.SaveChanges();

            try
            {
                string query = @"UPDATE [_Entry]
SET
    StudyLevelId=@StudyLevelId,
    StudyFormId=@StudyFormId,
    StudyBasisId=@StudyBasisId,
    FacultyId=@FacultyId,
    IsParallel=@IsParallel,
    IsReduced=@IsReduced,
    IsSecond=@IsSecond,
	DateOfStart=@DateOfStart,
	DateOfClose=@DateOfClose,
    CampaignYear=@CampaignYear,
    ComissionId=@ComissionId,
    IsForeign=@IsForeign,
    IsCrimea=@IsCrimea,
    LicenseProgramId=@LicenseProgramId,
    ObrazProgramId=@ObrazProgramId,
    ProfileId=@ProfileId
WHERE Id=@Id";
                SortedList<string, object> sl = new SortedList<string, object>();
                sl.Add("@Id", GuidId.Value);

                sl.Add("@CampaignYear", MainClass.iPriemYear);
                sl.Add("@StudyLevelId", StudyLevelId);
                sl.Add("@StudyFormId", StudyFormId);
                sl.Add("@StudyBasisId", StudyBasisId);
                sl.AddVal("@FacultyId", FacultyId);
                sl.Add("@IsParallel", IsParallel);
                sl.Add("@IsReduced", IsReduced);
                sl.Add("@IsSecond", IsSecond);

                sl.AddVal("@DateOfStart", DateOfStart);
                sl.AddVal("@DateOfClose", DateOfClose);

                sl.AddVal("@ComissionId", ComissionId);
                sl.AddVal("@IsForeign", IsForeign);
                sl.AddVal("@IsCrimea", IsCrimea);

                sl.AddVal("@LicenseProgramId", LicenseProgramId);
                sl.AddVal("@ObrazProgramId", ObrazProgramId);
                sl.AddVal("@ProfileId", ProfileId);

                MainClass.BdcOnlineReadWrite.ExecuteQuery(query, sl);
            }
            catch (Exception ex)
            {
                WinFormsServ.Error(ex);
            }
        }

        protected override void CloseCardAfterSave()
        {
            this.Close();
        }

        #region Exams

        public void UpdateExams()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var query = (from block in context.ExamInEntryBlock
                             join unit in context.ExamInEntryBlockUnit on block.Id equals unit.ExamInEntryBlockId
                             where block.EntryId == GuidId
                             orderby 
                             block.OrderNumber, 
                             block.IsProfil descending,
                             block.Name
                             select new
                             {
                                 block.Id,
                                 block.OrderNumber,
                                 BlockName = block.Name,
                                 IsProfil = block.IsProfil ? "да" : "нет",
                             }).ToList().OrderBy(x => x.OrderNumber).Distinct().ToList();
                dgvExams.DataSource = query;
                dgvExams.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvExams.Columns["Id"].Visible = false;
                dgvExams.Columns["BlockName"].HeaderText = "Название блока";
                dgvExams.Columns["IsProfil"].HeaderText = "Профильный";
                dgvExams.Columns["OrderNumber"].HeaderText = "№";
                dgvExams.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvExams.Columns["IsProfil"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void OpenCardExam(Guid? entryId, string id, bool isForModified)
        {
            CardExamInEntryBlock crd = new CardExamInEntryBlock(entryId, id, isForModified);
            crd.ToUpdateList += new UpdateListHandler(UpdateExams);
            crd.Show();
        }

        private void btnOpenExam_Click(object sender, EventArgs e)
        {
            if (dgvExams.CurrentCell != null && dgvExams.CurrentCell.RowIndex > -1)
            {
                string itemId = dgvExams.Rows[dgvExams.CurrentCell.RowIndex].Cells["Id"].Value.ToString();
                if (!string.IsNullOrEmpty(itemId))
                    OpenCardExam(GuidId, itemId, _isModified);
            }
        }

        private void btnAddExam_Click(object sender, EventArgs e)
        {
            OpenCardExam(GuidId, null, true);
        }

        private void btnDeleteExam_Click(object sender, EventArgs e)
        {
            if (dgvExams.CurrentCell != null && dgvExams.CurrentCell.RowIndex > -1)
            {
                if (MessageBox.Show("Удалить запись?", "Удаление", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string itemId = dgvExams.CurrentRow.Cells["Id"].Value.ToString();
                    try
                    {
                        using (PriemEntities context = new PriemEntities())
                        {
                            Guid id = Guid.Parse(itemId);
                            ExamInEntryBlock bl = context.ExamInEntryBlock.Where(x => x.Id == id).First();
                            context.ExamInEntryBlock.Remove(bl);
                            context.SaveChanges();

                            MainClass.BdcOnlineReadWrite.ExecuteQuery(String.Format("delete from dbo.ExamInEntryBlock where Id = '{0}'", id), null);
                        }
                    }
                    catch (Exception ex)
                    {
                        WinFormsServ.Error("Каскадное удаление запрещено: ", ex);
                    }

                    UpdateExams();
                }
            }
        }

        private void dgvExams_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnOpenExam_Click(null, null);
        }

        #endregion   

        #region InnerEntryInEntry
        private void UpdateInnerEntryInEntry()
        {
            if (string.IsNullOrEmpty(_Id))
                return;

            using (PriemEntities context = new PriemEntities())
            {
                var src = context.InnerEntryInEntry.Where(x => x.EntryId == GuidId)
                    .Select(x => new { x.Id, x.SP_ObrazProgram.Name, Profile = x.SP_Profile.Name, x.KCP }).ToArray();
                dgvInnerEntryInEntry.DataSource = Converter.ConvertToDataTable(src);
                dgvInnerEntryInEntry.Columns["Id"].Visible = false;
                dgvInnerEntryInEntry.Columns["Name"].HeaderText = "Образовательная программа";
                dgvInnerEntryInEntry.Columns["Profile"].HeaderText = "Профиль";
                dgvInnerEntryInEntry.Columns["KCP"].HeaderText = "КЦП";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!GuidId.HasValue)
            {
                WinFormsServ.Error("Сохраните сперва карточку!");
                return;
            }

            if (LicenseProgramId.HasValue)
            {
                var crd = new CardInnerEntryInEntry(GuidId.Value, LicenseProgramId.Value);
                crd.ToUpdateList += UpdateInnerEntryInEntry;
                crd.Show();
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvInnerEntryInEntry.SelectedCells.Count == 0)
                return;

            int rwInd = dgvInnerEntryInEntry.SelectedCells[0].RowIndex;
            Guid gId = (Guid)dgvInnerEntryInEntry["Id", rwInd].Value;

            using (PriemEntities context = new PriemEntities())
            {
                var OPIE = context.InnerEntryInEntry.Where(x => x.Id == gId).FirstOrDefault();
                context.InnerEntryInEntry.Remove(OPIE);
                context.SaveChanges();

                string query = "DELETE FROM InnerEntryInEntry WHERE Id=@Id";
                SortedList<string, object> slParams = new SortedList<string, object>();
                slParams.Add("@Id", gId);
                MainClass.BdcOnlineReadWrite.ExecuteQuery(query, slParams);
            }

            UpdateInnerEntryInEntry();
        }
        private void dgvObrazProgramInEntry_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            Guid gId = (Guid)dgvInnerEntryInEntry["Id", e.RowIndex].Value;
            var crd = new CardInnerEntryInEntry(gId);
            crd.ToUpdateList += UpdateInnerEntryInEntry;
            crd.Show();
        }

        #endregion

        protected override bool CheckFields()
        {
            bool bRet = true;
            if (!LicenseProgramId.HasValue)
            {
                epError.SetError(cbLicenseProgram, "не указано направление");
                bRet = false;
            }
            if (!ObrazProgramId.HasValue)
            {
                epError.SetError(cbObrazProgram, "не указана обр. программа");
                bRet = false;
            }
            if (!FacultyId.HasValue)
            {
                epError.SetError(cbFaculty, "не указано подразделение!");
                bRet = false;
            }
            if (!ProfileId.HasValue)
            {
                epError.SetError(cbProfile, "не указан профиль!");
                bRet = false;
            }
            return bRet;
        }

        private void cbStudyLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAfterStudyLevel();
        }
        private void cbLicenseProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAfterLicenseProgram();
        }
        private void cbObrazProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAfterObrazProgram();
        }
        private void cbProfile_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void cbAggregateGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAfterAggregateGroup();
        }

        #region OlympicsToCommonBenefit
        public void UpdateOlympicsToCommonBenefit()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var query = (from exEntry in context.OlympResultToCommonBenefit
                             where exEntry.EntryId == GuidId
                             select new
                             {
                                 exEntry.Id,
                                 OlympType = exEntry.OlympTypeId == null ? "все" : exEntry.OlympType.Name,
                                 ExamName = exEntry.ExamId == null? "нет" : exEntry.Exam.ExamName.Name,
                                 OlympLevelId = exEntry.OlympLevel.Name,
                                 OlympValue = exEntry.OlympValue.Name,
                                 exEntry.MinEge
                             }).ToList().OrderBy(x => x.ExamName).ThenBy(x => x.OlympLevelId).ThenBy(x => x.OlympValue).ToList();

                dgvOlympicsToCommonBenefit.DataSource = query;
                if (dgvOlympicsToCommonBenefit.Columns.Contains("Id"))
                    dgvOlympicsToCommonBenefit.Columns["Id"].Visible = false;
                if (dgvOlympicsToCommonBenefit.Columns.Contains("OlympType"))
                {
                    dgvOlympicsToCommonBenefit.Columns["OlympType"].HeaderText = "Тип";
                    dgvOlympicsToCommonBenefit.Columns["OlympType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                if (dgvOlympicsToCommonBenefit.Columns.Contains("ExamName"))
                {
                    dgvOlympicsToCommonBenefit.Columns["ExamName"].HeaderText = "Предмет";
                    dgvOlympicsToCommonBenefit.Columns["ExamName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                if (dgvOlympicsToCommonBenefit.Columns.Contains("OlympLevelId"))
                {
                    dgvOlympicsToCommonBenefit.Columns["OlympLevelId"].HeaderText = "Уровень олимпиады";
                    dgvOlympicsToCommonBenefit.Columns["OlympLevelId"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }
                if (dgvOlympicsToCommonBenefit.Columns.Contains("OlympValue"))
                {
                    dgvOlympicsToCommonBenefit.Columns["OlympValue"].HeaderText = "Уровень диплома";
                    dgvOlympicsToCommonBenefit.Columns["OlympValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }
                if (dgvOlympicsToCommonBenefit.Columns.Contains("MinEge"))
                {
                    dgvOlympicsToCommonBenefit.Columns["MinEge"].HeaderText = "Мин.балл";
                    dgvOlympicsToCommonBenefit.Columns["MinEge"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }
            }
        }
        private void btnAddOlympicsToCommonBenefit_Click(object sender, EventArgs e)
        {
            //OpenOlympicsToCommonBenefit(GuidId, null, false);
            var crd = new CardOlympicsToCommonBenefit_Master(GuidId.Value);
            crd.OnSave += UpdateOlympicsToCommonBenefit;
            crd.Show();
        }
        private void OpenOlympicsToCommonBenefit(Guid? entryId, string id, bool isForModified)
        {
            CardOlympicsToCommonBenefit crd = new CardOlympicsToCommonBenefit(id, entryId, isForModified);
            crd.ToUpdateList += new UpdateListHandler(UpdateOlympicsToCommonBenefit);
            crd.Show();
        }
        private void btnOpenOlympicsToCommonBenefit_Click(object sender, EventArgs e)
        {
            if (dgvOlympicsToCommonBenefit.CurrentCell != null && dgvOlympicsToCommonBenefit.CurrentCell.RowIndex > -1)
            {
                string itemId = dgvOlympicsToCommonBenefit.Rows[dgvOlympicsToCommonBenefit.CurrentCell.RowIndex].Cells["Id"].Value.ToString();
                if (!string.IsNullOrEmpty(itemId))
                    OpenOlympicsToCommonBenefit(GuidId, itemId, false);
            }
        }
        private void btnDeleteOlympicsToCommonBenefit_Click(object sender, EventArgs e)
        {
            if (dgvOlympicsToCommonBenefit.CurrentCell != null && dgvOlympicsToCommonBenefit.CurrentCell.RowIndex > -1)
            {
                if (MessageBox.Show("Удалить запись?", "Удаление", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string itemId = dgvOlympicsToCommonBenefit.CurrentRow.Cells["Id"].Value.ToString();
                    try
                    {
                        using (PriemEntities context = new PriemEntities())
                        {
                            int? id = int.Parse(itemId);
                            context.OlympResultToCommonBenefit_Delete(id);
                        }
                    }
                    catch (Exception ex)
                    {
                        WinFormsServ.Error("Каскадное удаление запрещено: ", ex);
                    }
                    UpdateOlympicsToCommonBenefit();
                }
            }
        }
        private void dgvOlympicsToCommonBenefit_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvOlympicsToCommonBenefit.CurrentCell != null && dgvOlympicsToCommonBenefit.CurrentCell.RowIndex > -1)
            {
                string itemId = dgvOlympicsToCommonBenefit.Rows[dgvOlympicsToCommonBenefit.CurrentCell.RowIndex].Cells["Id"].Value.ToString();
                if (!string.IsNullOrEmpty(itemId))
                    OpenOlympicsToCommonBenefit(GuidId, itemId, false);
            }
        }
        #endregion

        #region OlympResultToAdditionalMark
        public void UpdateOlympResultToAdditionalMark()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var query = (from exEntry in context.OlympResultToAdditionalMark
                             join OlLevel in context.OlympLevel on exEntry.OlympLevelId equals OlLevel.Id
                             join OlValue in context.OlympValue on exEntry.OlympValueId equals OlValue.Id
                             join Ex in context.Exam on exEntry.ExamId equals Ex.Id
                             join OlSubj in context.OlympSubject on exEntry.OlympSubjectId equals OlSubj.Id into OlSubj2
                             from OlSubj in OlSubj2.DefaultIfEmpty()
                             join OlProf in context.OlympProfile on exEntry.OlympProfileId equals OlProf.Id into OlProf2
                             from OlProf in OlProf2.DefaultIfEmpty()
                             where exEntry.EntryId == GuidId
                             orderby exEntry.OlympLevelId
                             select new
                             {
                                 exEntry.AdditionalMark,
                                 exEntry.Id,
                                 OlympLevelId = OlLevel.Name,
                                 OlympValue = OlValue.Name,
                                 ExamName = Ex.ExamName.Name,
                                 OlSubject = OlSubj == null ? "нет" : OlSubj.Name,
                                 OlProfile = OlProf == null ? "нет" : OlProf.Name,
                                 exEntry.MinEge
                             }).ToList().OrderBy(x => x.OlympLevelId).ToList();

                dgvOlympResultToAdditionalMark.DataSource = query;
                if (dgvOlympResultToAdditionalMark.Columns.Contains("Id"))
                    dgvOlympResultToAdditionalMark.Columns["Id"].Visible = false;
                if (dgvOlympResultToAdditionalMark.Columns.Contains("OlympLevelId"))
                {
                    dgvOlympResultToAdditionalMark.Columns["OlympLevelId"].HeaderText = "Уровень олимпиады";
                    dgvOlympResultToAdditionalMark.Columns["OlympLevelId"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }
                if (dgvOlympResultToAdditionalMark.Columns.Contains("OlympValue"))
                {
                    dgvOlympResultToAdditionalMark.Columns["OlympValue"].HeaderText = "Уровень диплома";
                    dgvOlympResultToAdditionalMark.Columns["OlympValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }

                if (dgvOlympResultToAdditionalMark.Columns.Contains("ExamName"))
                {
                    dgvOlympResultToAdditionalMark.Columns["ExamName"].HeaderText = "Предмет";
                    dgvOlympResultToAdditionalMark.Columns["ExamName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }
                if (dgvOlympResultToAdditionalMark.Columns.Contains("AdditionalMark"))
                {
                    dgvOlympResultToAdditionalMark.Columns["AdditionalMark"].HeaderText = "Баллы";
                    dgvOlympResultToAdditionalMark.Columns["AdditionalMark"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }
                if (dgvOlympResultToAdditionalMark.Columns.Contains("OlSubject"))
                {
                    dgvOlympResultToAdditionalMark.Columns["OlSubject"].HeaderText = "Предмет олимпиады";
                    dgvOlympResultToAdditionalMark.Columns["OlSubject"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }
                if (dgvOlympResultToAdditionalMark.Columns.Contains("OlProfile"))
                {
                    dgvOlympResultToAdditionalMark.Columns["OlProfile"].HeaderText = "Профиль олимпиады";
                    dgvOlympResultToAdditionalMark.Columns["OlProfile"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }
            }
        }
        private void btnOpenOlympResultToAdditionalMark_Click(object sender, EventArgs e)
        {
            if (dgvOlympResultToAdditionalMark.CurrentCell != null && dgvOlympResultToAdditionalMark.CurrentCell.RowIndex > -1)
            {
                string itemId = dgvOlympResultToAdditionalMark.Rows[dgvOlympResultToAdditionalMark.CurrentCell.RowIndex].Cells["Id"].Value.ToString();
                if (!string.IsNullOrEmpty(itemId))
                    OpenOlympResultToAdditionalMark(GuidId, itemId, false);
            }
        }
        private void btnAddOlympResultToAdditionalMark_Click(object sender, EventArgs e)
        {
            //OpenOlympResultToAdditionalMark(GuidId, null, false);
            var crd = new CardOlympResultToAdditionalMark_Master(GuidId.Value);
            crd.OnSave += UpdateOlympResultToAdditionalMark;
            crd.Show();
        }
        private void btnDeleteOlympResultToAdditionalMark_Click(object sender, EventArgs e)
        {
            if (dgvOlympResultToAdditionalMark.CurrentCell != null && dgvOlympResultToAdditionalMark.CurrentCell.RowIndex > -1)
            {
                if (MessageBox.Show("Удалить запись?", "Удаление", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string itemId = dgvOlympResultToAdditionalMark.CurrentRow.Cells["Id"].Value.ToString();
                    try
                    {
                        using (PriemEntities context = new PriemEntities())
                        {
                            int? id = int.Parse(itemId);
                            context.OlympResultToAdditionalMark_Delete(id);
                        }
                    }
                    catch (Exception ex)
                    {
                        WinFormsServ.Error("Каскадное удаление запрещено: ", ex);
                    }
                    UpdateOlympResultToAdditionalMark();
                }
            }
        }
        private void dgvOlympResultToAdditionalMark_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvOlympResultToAdditionalMark.CurrentCell != null && dgvOlympResultToAdditionalMark.CurrentCell.RowIndex > -1)
            {
                string itemId = dgvOlympResultToAdditionalMark.Rows[dgvOlympResultToAdditionalMark.CurrentCell.RowIndex].Cells["Id"].Value.ToString();
                if (!string.IsNullOrEmpty(itemId))
                    OpenOlympResultToAdditionalMark(GuidId, itemId, false);
            }
        }
        private void OpenOlympResultToAdditionalMark(Guid? entryId, string id, bool isForModified)
        {
            CardOlympResultToAdditionalMark crd = new CardOlympResultToAdditionalMark(id, entryId, isForModified);
            crd.ToUpdateList += new UpdateListHandler(UpdateOlympResultToAdditionalMark);
            crd.Show();
        }
        #endregion

        private void btnCopyExamsFromMain_Click(object sender, EventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                var CurrEnt = context.Entry.Where(x => x.Id == GuidId).FirstOrDefault();
                if (CurrEnt == null)
                {
                    WinFormsServ.Error("Не удаётся найти в базе текущий конкурс. Сохраните карточку и повторите запрос снова");
                    return;
                }

                Guid? ParentEntryId = context.extEntry
                    .Where(x => x.Id != this.GuidId
                        && !x.IsCrimea && !x.IsForeign
                        && x.LicenseProgramId == CurrEnt.LicenseProgramId
                        && x.ObrazProgramId == CurrEnt.ObrazProgramId
                        && x.ProfileId == CurrEnt.ProfileId
                        && x.StudyFormId == CurrEnt.StudyFormId
                        && x.StudyBasisId == CurrEnt.StudyBasisId
                        //&& x.IsCrimea == IsCrimea
                        //&& x.IsForeign == IsForeign
                        && x.IsSecond == IsSecond
                        && x.IsParallel == IsParallel
                        && x.IsReduced == IsReduced
                        )
                    .Select(x => (Guid?)x.Id).FirstOrDefault();

                if (ParentEntryId == null)
                {
                    WinFormsServ.Error("Не удаётся найти в базе аналог для текущего конкурса");
                    return;
                }

                var Exams = context.ExamInEntryBlockUnit
                    .Where(x => x.ExamInEntryBlock.EntryId == ParentEntryId)
                    .Select(x => new {
                        UnitId = x.Id,
                        BlockId = x.ExamInEntryBlockId,
                        ParentBlockId = x.ExamInEntryBlock.ParentExamInEntryBlockId,
                        x.ExamId,
                        x.EgeMin,
                        SortVal = x.ExamInEntryBlock.ParentExamInEntryBlockId == null ? 0 : 1,
                        x.ExamInEntryBlock.OrderNumber,
                        x.ExamInEntryBlock.Name
                    })
                    .ToList()
                    .OrderBy(x => x.SortVal).ThenBy(x => x.OrderNumber)
                    .ToList();

                Dictionary<Guid?, Guid?> dicExamBlock_OldToNew = new Dictionary<Guid?, Guid?>();

                var lstBlocks = Exams.Select(x => new { x.BlockId, x.Name, x.OrderNumber, x.ParentBlockId }).Distinct().ToList();

                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        foreach (var ExBlock in lstBlocks)
                        {
                            //Проверяем, нет ли уже такого блока.
                            int cnt = context.ExamInEntryBlock.Where(x => x.EntryId == CurrEnt.Id && x.Name == ExBlock.Name && x.OrderNumber == ExBlock.OrderNumber).Count();

                            if (cnt > 0)
                                continue;

                            //вставка блока
                            Guid gExBlockId = Guid.NewGuid();

                            dicExamBlock_OldToNew.Add(ExBlock.BlockId, gExBlockId);

                            Guid? ParentExamInEntryId = null;
                            if (ExBlock.ParentBlockId.HasValue)
                                dicExamBlock_OldToNew.TryGetValue(ExBlock.ParentBlockId, out ParentExamInEntryId);

                            string queryBlock = @" INSERT INTO dbo.ExamInEntryBlock ([Id], [EntryId], [Name]) VALUES (@Id, @EntryId, @Name)";
                            string queryBlockUnit = @" INSERT INTO dbo.ExamInEntryBlockUnit ([Id], [ExamInEntryBlockId], [ExamId], EgeMin) 
                                        VALUES (@Id, @ExamInEntryBlockId, @ExamId, @EgeMin)";

                            context.ExamInEntryBlock.Add(new ExamInEntryBlock()
                            {
                                Id = gExBlockId,
                                EntryId = CurrEnt.Id,
                                Name = ExBlock.Name,
                                IsCrimea = IsCrimea,
                                IsGosLine = IsForeign,
                                OrderNumber = ExBlock.OrderNumber,
                                ParentExamInEntryBlockId = ParentExamInEntryId,
                            });

                            SortedList<string, object> sl = new SortedList<string, object>();
                            sl.Add("@Id", gExBlockId);
                            sl.Add("@EntryId", CurrEnt.Id);
                            sl.Add("@Name", ExBlock.Name);
                            MainClass.BdcOnlineReadWrite.ExecuteQuery(queryBlock, sl);

                            var lstExams = Exams.Where(x => x.BlockId == ExBlock.BlockId).Select(x => new { x.ExamId, x.EgeMin }).Distinct().ToList();
                            foreach (var ExBlockUnit in lstExams)
                            {
                                Guid gUnitId = Guid.NewGuid();
                                //вставка юнитов
                                context.ExamInEntryBlockUnit.Add(new ExamInEntryBlockUnit()
                                {
                                    Id = gUnitId,
                                    ExamId = ExBlockUnit.ExamId,
                                    EgeMin = ExBlockUnit.EgeMin,
                                    ExamInEntryBlockId = gExBlockId,
                                });

                                SortedList<string, object> _sl = new SortedList<string, object>();
                                _sl.Add("@Id", gUnitId);
                                _sl.Add("@ExamInEntryBlockId", gExBlockId);
                                _sl.Add("@ExamId", ExBlockUnit.ExamId);
                                if (ExBlockUnit.EgeMin.HasValue)
                                    _sl.Add("@EgeMin", ExBlockUnit.EgeMin);
                                else
                                    _sl.Add("@EgeMin", DBNull.Value);

                                MainClass.BdcOnlineReadWrite.ExecuteQuery(queryBlockUnit, _sl);
                            }
                        }

                        context.SaveChanges();

                        tran.Complete();
                    }
                    catch (Exception ex)
                    {
                        WinFormsServ.Error(ex);
                    }
                }

                UpdateExams();
            }
        }

        private void btnCopyInnerEntryInEntryFromParent_Click(object sender, EventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                var CurrEnt = context.Entry.Where(x => x.Id == GuidId).FirstOrDefault();
                if (CurrEnt == null)
                {
                    WinFormsServ.Error("Не удаётся найти в базе текущий конкурс. Сохраните карточку и повторите запрос снова");
                    return;
                }

                Guid? ParentEntryId = context.extEntry
                    .Where(x => x.Id != this.GuidId
                        && !x.IsCrimea && !x.IsForeign
                        && x.LicenseProgramId == CurrEnt.LicenseProgramId
                        && x.ObrazProgramId == CurrEnt.ObrazProgramId
                        && x.ProfileId == CurrEnt.ProfileId
                        && x.StudyFormId == CurrEnt.StudyFormId
                        && x.IsCrimea == IsCrimea
                        && x.IsForeign == IsForeign
                        && x.IsSecond == IsSecond
                        && x.IsParallel == IsParallel
                        && x.IsReduced == IsReduced
                        && x.StudyBasisId != CurrEnt.StudyBasisId)
                    .Select(x => (Guid?)x.Id).FirstOrDefault();

                if (ParentEntryId == null)
                {
                    WinFormsServ.Error("Не удаётся найти в базе аналог для текущего конкурса");
                    return;
                }

                var lstInnerEnts = context.InnerEntryInEntry
                    .Where(x => x.EntryId == ParentEntryId)
                    .Select(x => new
                    {
                        x.ObrazProgramId,
                        x.ProfileId,
                        x.KCP,
                    }).ToList();

                Dictionary<Guid?, Guid?> dicInnerEntInEnt_OldToNew = new Dictionary<Guid?, Guid?>();

                foreach (var vInnEnt in lstInnerEnts)
                {
                    int cnt = context.InnerEntryInEntry
                        .Where(x => x.EntryId == GuidId && x.ObrazProgramId == vInnEnt.ObrazProgramId && x.ProfileId == vInnEnt.ProfileId)
                        .Count();

                    if (cnt == 0)
                    {
                        var zz = new InnerEntryInEntry();
                        zz.Id = Guid.NewGuid();
                        zz.EntryId = GuidId.Value;
                        zz.ObrazProgramId = vInnEnt.ObrazProgramId;
                        zz.ProfileId = vInnEnt.ProfileId;
                        zz.KCP = vInnEnt.KCP;
                        context.InnerEntryInEntry.Add(zz);

                        context.SaveChanges();
                    }
                }
            }
        }
    }
}
