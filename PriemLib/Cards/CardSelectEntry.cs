using BaseFormsLib;
using EducServLib;
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
    public partial class CardSelectEntry : BaseCard
    {
        #region Fields
        public Guid? EntryId
        {
            get
            {
                try
                {
                    using (PriemEntities context = new PriemEntities())
                    {
                        Guid? entId = (from ent in context.qEntry
                                       where ent.IsSecond == IsSecond && ent.IsParallel == IsParallel && ent.IsReduced == IsReduced
                                       && ent.LicenseProgramId == LicenseProgramId
                                       && ent.ObrazProgramId == ObrazProgramId
                                       && (ProfileId == null ? ent.ProfileId == 0 : ent.ProfileId == ProfileId)
                                       && ent.StudyFormId == StudyFormId
                                       && ent.StudyBasisId == StudyBasisId
                                       && ent.IsForeign == IsForeign
                                       select ent.Id).FirstOrDefault();
                        return entId;
                    }
                }
                catch
                {
                    return null;
                }
            }
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
        public int? ObrazProgramId
        {
            get { return ComboServ.GetComboIdInt(cbObrazProgram); }
            set { ComboServ.SetComboId(cbObrazProgram, value); }
        }
        public int? ProfileId
        {
            get
            {
                return ComboServ.GetComboIdInt(cbProfile);
                //string prId = ComboServ.GetComboId(cbProfile);
                //if(string.IsNullOrEmpty(prId))
                //    return null;
                //else
                //    return new Guid(prId);
            }
            set
            {
                if (value == null)
                    ComboServ.SetComboId(cbProfile, (string)null);
                else
                    ComboServ.SetComboId(cbProfile, value.ToString());
            }
        }
        public int? StudyFormId
        {
            get { return ComboServ.GetComboIdInt(cbStudyForm); }
            set { ComboServ.SetComboId(cbStudyForm, value); }
        }
        public int? StudyBasisId
        {
            get { return ComboServ.GetComboIdInt(cbStudyBasis); }
            set { ComboServ.SetComboId(cbStudyBasis, value); }
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
        public bool IsForeign
        {
            get { return chbIsForeign.Checked; }
            set { chbIsForeign.Checked = value; }
        }
        #endregion
        
        public event Action<Guid> EntrySelected;
        public CardSelectEntry()
        {
            InitializeComponent();
            InitControls();
        }

        public CardSelectEntry(Guid EntryId)
        {
            InitializeComponent();
            InitControls();
        }

        protected override void ExtraInit()
        {
            FillLicenseProgram();
            FillObrazProgram();
            FillProfile();
            FillFaculty();
            FillStudyForm();
            FillStudyBasis();
        }

        private void SetEntryValues(Guid EntryId)
        {
            using (PriemEntities context = new PriemEntities())
            {
                var entry = context.Entry.Where(x => x.Id == EntryId).FirstOrDefault();
                if (entry != null)
                {
                    IsSecond = entry.IsSecond;
                    IsReduced = entry.IsReduced;
                    IsParallel = entry.IsParallel;
                    IsForeign = entry.IsForeign;
                    LicenseProgramId = entry.LicenseProgramId;
                    ObrazProgramId = entry.ObrazProgramId;
                    ProfileId = entry.ProfileId;
                    StudyFormId = entry.StudyFormId;
                    StudyBasisId = entry.StudyBasisId;
                }
            }
        }

        protected override void SetReadOnlyFields()
        {
            if (MainClass.IsFacMain() || MainClass.RightsSov_SovMain())
            {
                cbLicenseProgram.Enabled = true;
                cbObrazProgram.Enabled = true;
                cbProfile.Enabled = true;
                cbFaculty.Enabled = true;
                gbSecondType.Enabled = true;
                cbStudyForm.Enabled = true;
                cbStudyBasis.Enabled = true;
            }
            else
            {
                cbLicenseProgram.Enabled = false;
                cbObrazProgram.Enabled = false;
                cbProfile.Enabled = false;
                cbFaculty.Enabled = false;
                gbSecondType.Enabled = false;
                cbStudyForm.Enabled = false;
                cbStudyBasis.Enabled = false;
            }
        }

        #region Handlers
        protected override void InitHandlers()
        {
            chbIsReduced.CheckedChanged += chbIsReduced_CheckedChanged;
            chbIsParallel.CheckedChanged += chbIsParallel_CheckedChanged;
            chbIsSecond.CheckedChanged += chbIsSecond_CheckedChanged;
            cbLicenseProgram.SelectedIndexChanged += cbLicenseProgram_SelectedIndexChanged;
            cbObrazProgram.SelectedIndexChanged += cbObrazProgram_SelectedIndexChanged;
            cbProfile.SelectedIndexChanged += cbProfile_SelectedIndexChanged;
            cbStudyForm.SelectedIndexChanged += cbStudyForm_SelectedIndexChanged;
            chbIsForeign.CheckedChanged += chbIsForeign_CheckedChanged;
        }

        void chbIsForeign_CheckedChanged(object sender, EventArgs e)
        {

            cbLicenseProgram.Enabled = true;
            cbObrazProgram.Enabled = true;
            cbProfile.Enabled = true;
            cbStudyForm.Enabled = true;
            cbStudyBasis.Enabled = true;
            int? LPId = LicenseProgramId;
            int? OPId = ObrazProgramId;
            int? ProfId = ProfileId;
            int? StF = StudyFormId;
            int? StB = StudyBasisId;
            FillLicenseProgram();
            LicenseProgramId = LPId;
            ObrazProgramId = OPId;
            ProfileId = ProfId;
            StudyFormId = StF;
            StudyBasisId = StB;
        }
        void chbIsSecond_CheckedChanged(object sender, EventArgs e)
        {
            FillLicenseProgram();
        }
        void chbIsParallel_CheckedChanged(object sender, EventArgs e)
        {
            FillLicenseProgram();
        }
        void chbIsReduced_CheckedChanged(object sender, EventArgs e)
        {
            FillLicenseProgram();
        }
        void cbLicenseProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillObrazProgram();
        }
        void cbObrazProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillProfile();
            //FillFaculty();
            //FillStudyForm();
            //FillStudyBasis();
        }
        void cbProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillFaculty();
            FillStudyForm();
            //FillStudyBasis();
        }
        void cbStudyForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStudyBasis();
        }

        private IEnumerable<qEntry> GetEntry(PriemEntities context)
        {
            IEnumerable<qEntry> entry = MainClass.GetEntry(context);

            entry = entry.Where(c => c.IsReduced == IsReduced);
            entry = entry.Where(c => c.IsParallel == IsParallel);
            entry = entry.Where(c => c.IsSecond == IsSecond);
            entry = entry.Where(c => c.IsForeign == IsForeign);

            return entry;
        }

        private void FillLicenseProgram()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    List<KeyValuePair<string, string>> lst =
                        ((from ent in GetEntry(context)
                          orderby ent.LicenseProgramName
                          select new
                          {
                              Id = ent.LicenseProgramId,
                              Name = ent.LicenseProgramName,
                              Code = ent.LicenseProgramCode
                          }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name + ' ' + u.Code)).ToList();

                    ComboServ.FillCombo(cbLicenseProgram, lst, false, false);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы FillLicenseProgram", exc);
            }
        }
        private void FillObrazProgram()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    List<KeyValuePair<string, string>> lst =
                        ((from ent in GetEntry(context)
                          where ent.LicenseProgramId == LicenseProgramId
                          orderby ent.ObrazProgramName
                          select new
                          {
                              Id = ent.ObrazProgramId,
                              Name = ent.ObrazProgramName,
                              Crypt = ent.ObrazProgramCrypt
                          }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name + ' ' + u.Crypt)).ToList();

                    ComboServ.FillCombo(cbObrazProgram, lst, false, false);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы FillObrazProgram", exc);
            }
        }
        private void FillProfile()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    List<KeyValuePair<string, string>> lst =
                        ((from ent in GetEntry(context)
                          where ent.LicenseProgramId == LicenseProgramId && ent.ObrazProgramId == ObrazProgramId
                          orderby ent.ProfileName
                          select new
                          {
                              Id = ent.ProfileId,
                              Name = ent.ProfileName
                          }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                    if (lst.Count() > 0)
                    {
                        if (ObrazProgramId == 39)
                            ComboServ.FillCombo(cbProfile, lst, true, false);
                        else
                            ComboServ.FillCombo(cbProfile, lst, false, false);
                        cbProfile.Enabled = true;
                    }
                    else
                    {
                        ComboServ.FillCombo(cbProfile, new List<KeyValuePair<string, string>>(), true, false);
                        cbProfile.Enabled = false;
                    }
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы FillProfile", exc);
            }
        }
        private void FillFaculty()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    List<KeyValuePair<string, string>> lst =
                        ((from ent in GetEntry(context)
                          where ent.LicenseProgramId == LicenseProgramId
                          && ent.ObrazProgramId == ObrazProgramId
                          && (ProfileId == null ? ent.ProfileId == 0 : ent.ProfileId == ProfileId)
                          select new
                          {
                              Id = ent.FacultyId,
                              Name = ent.FacultyName
                          }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                    ComboServ.FillCombo(cbFaculty, lst, false, false);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы FillFaculty", exc);
            }
        }
        private void FillStudyForm()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    List<KeyValuePair<string, string>> lst =
                        ((from ent in GetEntry(context)
                          where
                          ent.LicenseProgramId == LicenseProgramId
                          && ent.ObrazProgramId == ObrazProgramId
                          && (ProfileId == null ? ent.ProfileId == 0 : ent.ProfileId == ProfileId)
                          && ent.FacultyId == FacultyId
                          orderby ent.StudyFormName
                          select new
                          {
                              Id = ent.StudyFormId,
                              Name = ent.StudyFormName
                          }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                    ComboServ.FillCombo(cbStudyForm, lst, false, false);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы FillStudyForm", exc);
            }
        }
        private void FillStudyBasis()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    List<KeyValuePair<string, string>> lst =
                        ((from ent in GetEntry(context)
                          where ent.LicenseProgramId == LicenseProgramId
                          && ent.ObrazProgramId == ObrazProgramId
                          && (ProfileId == null ? ent.ProfileId == 0 : ent.ProfileId == ProfileId)
                          && ent.FacultyId == FacultyId
                          && ent.StudyFormId == StudyFormId
                          orderby ent.StudyBasisName
                          select new
                          {
                              Id = ent.StudyBasisId,
                              Name = ent.StudyBasisName
                          }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                    ComboServ.FillCombo(cbStudyBasis, lst, false, false);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы FillStudyBasis", exc);
            }
        }
        #endregion

        protected override void FillCard()
        {
            
        }

        private Guid? GetEntryId()
        {
            using (PriemEntities context = new PriemEntities())
            {
                Guid entId = (from ent in context.qEntry
                              where ent.IsSecond == IsSecond && ent.IsParallel == IsParallel && ent.IsReduced == IsReduced
                              && ent.LicenseProgramId == LicenseProgramId
                              && ent.ObrazProgramId == ObrazProgramId
                              && (ProfileId == null ? ent.ProfileId == 0 : ent.ProfileId == ProfileId)
                              && ent.StudyFormId == StudyFormId
                              && ent.StudyBasisId == StudyBasisId
                              && ent.IsForeign == IsForeign
                              select ent.Id).DefaultIfEmpty(Guid.Empty).FirstOrDefault();

                if (entId != Guid.Empty)
                    return entId;
                else
                    return null;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Guid? EntryId = GetEntryId();
            if (!EntryId.HasValue)
            {
                WinFormsServ.Error("Не найдено конкурса!");
                return;
            }
            if (EntrySelected != null)
            {
                EntrySelected(EntryId.Value);
                Close();
            }
                
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void BaseCard_FormClosing(object sender, FormClosingEventArgs e)
        {
            //не делать ничего
        }
    }
}
