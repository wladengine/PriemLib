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
    public partial class ListAbitRatingKofGroupChanging : SimpleBook
    {
        private int StudyLevelGroupId { get; set; }
        private int? FacultyId { get; set; }
        private int? LicenseProgramId { get; set; }
        private int? ObrazProgramId { get; set; }
        private int? ProfileId { get; set; }
        private int? StudyFormId { get; set; }
        private int? StudyBasisId { get; set; }

        public ListAbitRatingKofGroupChanging()
        {
            InitializeComponent();

            Title = "Групповое назначение рейтинговых коэффициентов";
            this.Text = Title + BaseFormsLib.Constants.CARD_READ_ONLY;

            dgv.CellDoubleClick += dgv_CellDoubleClick;

            //btnSave.Enabled = false;
            StudyLevelGroupId = MainClass.lstStudyLevelGroupId.First();

            InitControls();
        }

        protected override void ExtraInit()
        {
            if (_bdc != null)
                if (!MainClass.IsEntryChanger() || !MainClass.IsFacMain())
                    btnSave.Enabled = false;
        }

        void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > 0)
            {
                string sId = dgv["Id", e.RowIndex].Value.ToString();
                MainClass.OpenCardAbit(sId, null, null);
            }
        }

        private string GetWhereString()
        {
            string sWhere = " WHERE Abiturient.BackDoc = 0 AND Abiturient.NotEnabled = 0 AND Abiturient.EntryId IN ( SELECT Entry.Id FROM ed.Entry INNER JOIN ed.StudyLevel ON StudyLevel.Id = Entry.StudyLevelId WHERE StudyLevel.LevelGroupId = " + StudyLevelGroupId;

            if (FacultyId.HasValue)
                sWhere += " AND Entry.FacultyId = " + FacultyId;

            if (LicenseProgramId.HasValue)
                sWhere += " AND Entry.LicenseProgramId = " + LicenseProgramId;

            if (ObrazProgramId.HasValue)
                sWhere += " AND Entry.ObrazProgramId = " + ObrazProgramId;

            if (ProfileId.HasValue)
                sWhere += " AND Entry.ProfileId = " + ProfileId;

            if (StudyFormId.HasValue)
                sWhere += " AND Entry.StudyFormId = " + StudyFormId;

            if (StudyBasisId.HasValue)
                sWhere += " AND Entry.StudyBasisId = " + StudyBasisId;

            return sWhere + ")";
        }

        protected override DataTable GetSource()
        {
            return _bdc.GetAbitsForRatingKoef(GetWhereString());
        }

        protected override void UpdateSource(DataTable table)
        {
            _bdc.SetAbitsForRatingKoef(table, GetWhereString());
        }

        protected override void BindGrid(DataTable table)
        {
            dataTable = table;
            dgv.DataSource = table;
            dgv.Columns["Id"].Visible = false;
            dgv.Update();
        }

        private void btnFilters_Click(object sender, EventArgs e)
        {
            var crd = new ListAbitRatingKofGroupChangingFilters(StudyLevelGroupId, FacultyId, LicenseProgramId, ObrazProgramId, ProfileId, StudyFormId, StudyBasisId);
            crd.OnAbitRatingKofGroupChangingFiltersChanged += UpdateFilters;
            crd.Show();
        }

        private void UpdateFilters(AbitRatingKofGroupChangingFilters flt)
        {
            StudyLevelGroupId = flt.StudyLevelGroupId;
            FacultyId = flt.FacultyId;
            LicenseProgramId = flt.LicenseProgramId;
            ObrazProgramId = flt.ObrazProgramId;
            ProfileId = flt.ProfileId;
            StudyFormId = flt.StudyFormId;
            StudyBasisId = flt.StudyBasisId;

            InitControls();
        }
    }
}
