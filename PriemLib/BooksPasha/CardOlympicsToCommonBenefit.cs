using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using EducServLib;
using BaseFormsLib;
using System.Data.Entity.Core.Objects;

namespace PriemLib 
{
    public partial class CardOlympicsToCommonBenefit : BookCardInt
    {
        private Guid? _EntryId;
        private bool _isReadOnly;
        #region Fields
        public int? OlympTypeId
        {
            get { return ComboServ.GetComboIdInt(cbOlympType); }
            set { ComboServ.SetComboId(cbOlympType, value); }
        }
        public int? OlympLevelId
        {
            get { return ComboServ.GetComboIdInt(cbOlympLevel); }
            set { ComboServ.SetComboId(cbOlympLevel, value); }
        }
        public int? OlympSubjectId
        {
            get { return ComboServ.GetComboIdInt(cbOlympSubject); }
            set { ComboServ.SetComboId(cbOlympSubject, value); }
        }
        public int? OlympProfileId
        {
            get { return ComboServ.GetComboIdInt(cbOlympProfile); }
            set { ComboServ.SetComboId(cbOlympProfile, value); }
        }
        public int? OlympValueId
        {
            get { return ComboServ.GetComboIdInt(cbOlympValue); }
            set { ComboServ.SetComboId(cbOlympValue, value); }
        }
        public int? ExamId
        {
            get { return ComboServ.GetComboIdInt(cbExam); }
            set { ComboServ.SetComboId(cbExam, value); }
        }
        public decimal? MinEge
        {
            get 
            {
                string sVal = tbMinEge.Text.Trim();
                decimal iVal = 0;

                if (decimal.TryParse(sVal, out iVal))
                    return iVal;
                else
                    return null;
            }
            set
            {
                if (value.HasValue)
                    tbMinEge.Text = value.ToString();
            }
        }
        #endregion

        public CardOlympicsToCommonBenefit(Guid? EntryId)
            : this(null, EntryId, false)
        {
        }
        public CardOlympicsToCommonBenefit(string olId, Guid? EntryId, bool isReadOnly) : base(olId)
        {
            InitializeComponent();
            _EntryId = EntryId;
            _Id = olId;
            _isReadOnly = isReadOnly;           
            InitControls();
        }

        protected override void ExtraInit()
        {
            base.ExtraInit();
            btnSaveAsNew.Enabled = true;
            btnSaveAsNew.Visible = true;

            _title = "Олимпиады";
            _tableName = "ed.OlympResultToCommonBenefit";
            this.MdiParent = MainClass.mainform;

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    ComboServ.FillCombo(cbOlympType, HelpClass.GetComboListByTable("ed.OlympType"), false, true);
                    ComboServ.FillCombo(cbOlympLevel, HelpClass.GetComboListByTable("ed.OlympLevel"), false, false);
                    ComboServ.FillCombo(cbOlympValue, HelpClass.GetComboListByTable("ed.OlympValue"), false, false);
                    FillProfiles();
                    FillAfterOlympLevel();
                    FillOlympSubjects();
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы ", exc);
            }
        }
        private void cbOlympType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillProfiles();
        }

        private void FillAfterOlympLevel()
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<KeyValuePair<string, string>> lst =
                    ((from ob in context.extOlympBook
                      join exToOl in context.OlympSubjectToExam on ob.OlympSubjectId equals exToOl.OlympSubjectId
                      join ex in context.Exam on exToOl.ExamId equals ex.Id
                      join exInEnt in context.extExamInEntry on ex.Id equals exInEnt.ExamId
                      where ob.OlympLevelId == OlympLevelId && exInEnt.EntryId == _EntryId
                      select new
                      {
                          Id = ex.Id,
                          Name = ex.ExamName.Name,
                          exInEnt.OrderNumber
                      }).Distinct()).ToList().OrderBy(x => x.OrderNumber).Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                cbExam.Enabled = true;
                ComboServ.FillCombo(cbExam, lst, false, false);
                cbExam.SelectedIndex = 0;
            }
        }
        private void FillOlympSubjects()
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<KeyValuePair<string, string>> lst =
                    ((from ob in context.extOlympBook
                      select new
                      {
                          Id = ob.OlympSubjectId,
                          Name = ob.OlympSubjectName,
                      }).Distinct()).ToList()
                      .OrderBy(x => x.Name)
                      .Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                cbOlympSubject.Enabled = true;
                ComboServ.FillCombo(cbOlympSubject, lst, false, true);
            }
        }
        private void FillProfiles()
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<KeyValuePair<string, string>> lst =
                    ((from ob in context.extOlympBook
                      where OlympTypeId.HasValue ? ob.OlympTypeId == OlympTypeId : true
                      select new
                      {
                          Id = ob.OlympProfileId,
                          Name = ob.OlympProfileName,
                      }).Distinct()).ToList().OrderBy(x => x.Name).Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                ComboServ.FillCombo(cbOlympProfile, lst, false, true);
            }
        }

        protected override void InitHandlers()
        {
            cbOlympLevel.SelectedIndexChanged += new EventHandler(cbOlympName_SelectedIndexChanged);
        }
        private void cbOlympName_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillAfterOlympLevel();
        }
        
        protected override void FillCard()
        {
            if (_Id == null)
                return;

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    OlympResultToCommonBenefit olymp =
                        (from ec in context.OlympResultToCommonBenefit
                         where ec.Id == IntId
                         select ec).FirstOrDefault();

                    if (olymp == null)
                        return;

                    OlympTypeId = olymp.OlympTypeId;
                    OlympLevelId = olymp.OlympLevelId;
                    ExamId = olymp.ExamId;
                    OlympValueId = olymp.OlympValueId;
                    MinEge = olymp.MinEge;

                    OlympSubjectId = olymp.OlympSubjectId;
                    OlympProfileId = olymp.OlympProfileId;
                }
            }
            catch (DataException de)
            {
                WinFormsServ.Error("Ошибка при заполнении формы ", de);
            }
        }
        protected override void SetReadOnlyFieldsAfterFill()
        {
            base.SetReadOnlyFieldsAfterFill();

            if (_isReadOnly)
                btnSaveChange.Enabled = false;
        }
        protected override void SetAllFieldsEnabled()
        {
            base.SetAllFieldsEnabled();
        }
        protected override void InsertRec(PriemEntities context, ObjectParameter idParam)
        {
            context.OlympResultToCommonBenefit_Insert(_EntryId, OlympTypeId, OlympLevelId, OlympValueId, ExamId, OlympProfileId, OlympSubjectId, MinEge, idParam);
        }
        protected override void UpdateRec(PriemEntities context, int id)
        {
            context.OlympResultToCommonBenefit_Update(_EntryId, OlympTypeId, OlympLevelId, OlympValueId, ExamId, OlympProfileId, OlympSubjectId, MinEge, id);
        }
        protected override void OnSave()
        {
            base.OnSave();
            MainClass.DataRefresh();
        }
        protected override void CloseCardAfterSave()
        {
            if (!_isModified)
                this.Close();
        }

        
    }
}
