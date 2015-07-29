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
    public partial class ListAbitRatingKofGroupChangingFilters : Form
    {
        #region Fields
        private int? StudyLevelGroupId
        {
            get { return ComboServ.GetComboIdInt(cbStudyLevelGroup); }
            set { ComboServ.SetComboId(cbStudyLevelGroup, value); }
        }
        private int? FacultyId
        {
            get { return ComboServ.GetComboIdInt(cbFaculty); }
            set { ComboServ.SetComboId(cbFaculty, value); }
        }
        private int? LicenseProgramId
        {
            get { return ComboServ.GetComboIdInt(cbLicenseProgram); }
            set { ComboServ.SetComboId(cbLicenseProgram, value); }
        }
        private int? ObrazProgramId
        {
            get { return ComboServ.GetComboIdInt(cbObrazProgram); }
            set { ComboServ.SetComboId(cbObrazProgram, value); }
        }
        private int? ProfileId
        {
            get { return ComboServ.GetComboIdInt(cbProfile); }
            set { ComboServ.SetComboId(cbProfile, value); }
        }
        private int? StudyFormId
        {
            get { return ComboServ.GetComboIdInt(cbStudyForm); }
            set { ComboServ.SetComboId(cbStudyForm, value); }
        }
        private int? StudyBasisId
        {
            get { return ComboServ.GetComboIdInt(cbStudyBasis); }
            set { ComboServ.SetComboId(cbStudyBasis, value); }
        }
        #endregion

        public event AbitRatingKofGroupChangingFiltersChangedHandler OnAbitRatingKofGroupChangingFiltersChanged;

        public ListAbitRatingKofGroupChangingFilters(int iStudyLevelGroupId, int? iFacultyId, int? iLicenseProgramId, int? iObrazProgramId, int? iProfileId, int? iStudyFormId, int? iStudyBasisId)
        {
            InitializeComponent();
            UpdateComboStudyLevelGroup();
            StudyLevelGroupId = iStudyLevelGroupId;
            this.MdiParent = MainClass.mainform;

            StudyLevelGroupId = iStudyLevelGroupId;
            if (iFacultyId.HasValue)
                FacultyId = iFacultyId;
            if (iLicenseProgramId.HasValue)
                LicenseProgramId = iLicenseProgramId;
            if (iObrazProgramId.HasValue)
                ObrazProgramId = iObrazProgramId;
            if (iProfileId.HasValue)
                ProfileId = iProfileId;
            if (iStudyFormId.HasValue)
                StudyFormId = iStudyFormId;
            if (iStudyBasisId.HasValue)
                StudyBasisId = iStudyBasisId;
        }

        private void UpdateComboStudyLevelGroup()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lst = context.extEntry
                    .Where(x => MainClass.lstStudyLevelGroupId.Contains(x.StudyLevelGroupId))
                    .Select(x => new { x.StudyLevelGroupId, x.StudyLevelGroupName })
                    .Distinct()
                    .ToList()
                    .OrderBy(x => x.StudyLevelGroupName)
                    .Select(x => new KeyValuePair<string, string>(x.StudyLevelGroupId.ToString(), x.StudyLevelGroupName))
                    .Distinct()
                    .ToList();

                ComboServ.FillCombo(cbStudyLevelGroup, lst, false, false);
            }
        }
        private void UpdateComboFaculty()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lst = context.extEntry
                    .Where(x => x.StudyLevelGroupId == StudyLevelGroupId)
                    .Select(x => new { x.FacultyId, x.FacultyName })
                    .Distinct()
                    .ToList()
                    .OrderBy(x => x.FacultyName)
                    .Select(x => new KeyValuePair<string, string>(x.FacultyId.ToString(), x.FacultyName))
                    .Distinct()
                    .ToList();

                ComboServ.FillCombo(cbFaculty, lst, false, false);
            }
        }
        private void UpdateComboLicenseProgram()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lst = context.extEntry
                    .Where(x => x.StudyLevelGroupId == StudyLevelGroupId)
                    .Where(x => FacultyId.HasValue ? x.FacultyId == FacultyId : true)
                    .Select(x => new { x.LicenseProgramId, x.LicenseProgramCode, x.LicenseProgramName })
                    .Distinct()
                    .ToList()
                    .OrderBy(x => x.LicenseProgramCode)
                    .Select(x => new KeyValuePair<string, string>(x.LicenseProgramId.ToString(), x.LicenseProgramCode + " " + x.LicenseProgramName))
                    .Distinct()
                    .ToList();

                ComboServ.FillCombo(cbLicenseProgram, lst, false, true);
            }
        }
        private void UpdateComboObrazProgram()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lst = context.extEntry
                    .Where(x => x.StudyLevelGroupId == StudyLevelGroupId)
                    .Where(x => FacultyId.HasValue ? x.FacultyId == FacultyId : true)
                    .Where(x => LicenseProgramId.HasValue ? x.LicenseProgramId == LicenseProgramId : true)
                    .Select(x => new { x.ObrazProgramId, x.ObrazProgramCrypt, x.ObrazProgramName })
                    .Distinct()
                    .ToList()
                    .OrderBy(x => x.ObrazProgramCrypt)
                    .Select(x => new KeyValuePair<string, string>(x.ObrazProgramId.ToString(), x.ObrazProgramCrypt + " " + x.ObrazProgramName))
                    .Distinct()
                    .ToList();

                ComboServ.FillCombo(cbObrazProgram, lst, false, true);
            }
        }
        private void UpdateComboProfile()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lst = context.extEntry
                    .Where(x => x.StudyLevelGroupId == StudyLevelGroupId)
                    .Where(x => FacultyId.HasValue ? x.FacultyId == FacultyId : true)
                    .Where(x => LicenseProgramId.HasValue ? x.LicenseProgramId == LicenseProgramId : true)
                    .Where(x => ObrazProgramId.HasValue ? x.ObrazProgramId == ObrazProgramId : true)
                    .Select(x => new { x.ProfileId, x.ProfileName })
                    .Distinct()
                    .ToList()
                    .OrderBy(x => x.ProfileName)
                    .Select(x => new KeyValuePair<string, string>(x.ProfileId.ToString(), x.ProfileName))
                    .Distinct()
                    .ToList();

                ComboServ.FillCombo(cbProfile, lst, false, true);
            }
        }
        private void UpdateComboStudyForm()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lst = context.extEntry
                    .Where(x => x.StudyLevelGroupId == StudyLevelGroupId)
                    .Where(x => FacultyId.HasValue ? x.FacultyId == FacultyId : true)
                    .Where(x => LicenseProgramId.HasValue ? x.LicenseProgramId == LicenseProgramId : true)
                    .Where(x => ObrazProgramId.HasValue ? x.ObrazProgramId == ObrazProgramId : true)
                    .Where(x => ProfileId.HasValue ? x.ProfileId == ProfileId : true)
                    .Select(x => new { x.StudyFormId, x.StudyFormName })
                    .Distinct()
                    .ToList()
                    .OrderBy(x => x.StudyFormId)
                    .Select(x => new KeyValuePair<string, string>(x.StudyFormId.ToString(), x.StudyFormName))
                    .Distinct()
                    .ToList();

                ComboServ.FillCombo(cbStudyForm, lst, false, true);
            }
        }
        private void UpdateComboStudyBasis()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lst = context.extEntry
                    .Where(x => x.StudyLevelGroupId == StudyLevelGroupId)
                    .Where(x => FacultyId.HasValue ? x.FacultyId == FacultyId : true)
                    .Where(x => LicenseProgramId.HasValue ? x.LicenseProgramId == LicenseProgramId : true)
                    .Where(x => ObrazProgramId.HasValue ? x.ObrazProgramId == ObrazProgramId : true)
                    .Where(x => ProfileId.HasValue ? x.ProfileId == ProfileId : true)
                    .Where(x => StudyFormId.HasValue ? x.StudyFormId == StudyFormId : true)
                    .Select(x => new { x.StudyBasisId, x.StudyBasisName })
                    .Distinct()
                    .ToList()
                    .OrderBy(x => x.StudyBasisId)
                    .Select(x => new KeyValuePair<string, string>(x.StudyBasisId.ToString(), x.StudyBasisName))
                    .Distinct()
                    .ToList();

                ComboServ.FillCombo(cbStudyBasis, lst, false, true);
            }
        }

        private void cbStudyLevelGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateComboFaculty();
        }
        private void cbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateComboLicenseProgram();
        }
        private void cbLicenseProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateComboObrazProgram();
        }
        private void cbObrazProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateComboProfile();
        }
        private void cbProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateComboStudyForm();
        }
        private void cbStudyForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateComboStudyBasis();
        }

        private void btnSaveFilters_Click(object sender, EventArgs e)
        {
            if (OnAbitRatingKofGroupChangingFiltersChanged != null)
                OnAbitRatingKofGroupChangingFiltersChanged(new AbitRatingKofGroupChangingFilters()
                {
                    StudyLevelGroupId = StudyLevelGroupId.Value,
                    FacultyId = FacultyId,
                    LicenseProgramId = LicenseProgramId,
                    ObrazProgramId = ObrazProgramId,
                    ProfileId = ProfileId,
                    StudyBasisId = StudyBasisId,
                    StudyFormId = StudyFormId
                });
        }
    }

    public delegate void AbitRatingKofGroupChangingFiltersChangedHandler(AbitRatingKofGroupChangingFilters flt);
    public class AbitRatingKofGroupChangingFilters
    {
        public int StudyLevelGroupId { get; set; }
        public int? FacultyId { get; set; }
        public int? LicenseProgramId { get; set; }
        public int? ObrazProgramId { get; set; }
        public int? ProfileId { get; set; }
        public int? StudyFormId { get; set; }
        public int? StudyBasisId { get; set; }
    }
}
