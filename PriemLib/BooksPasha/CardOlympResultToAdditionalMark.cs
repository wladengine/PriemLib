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
using System.Data.Entity.Core.Objects;

namespace PriemLib
{
    public partial class CardOlympResultToAdditionalMark : BookCardInt
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
        public int? OlympNameId
        {
            get { return ComboServ.GetComboIdInt(cbOlympName); }
            set { ComboServ.SetComboId(cbOlympName, value); }
        }
        public int? OlympSubjectId
        {
            get { return ComboServ.GetComboIdInt(cbOlympSubject); }
            set { ComboServ.SetComboId(cbOlympSubject, value); }
        }
        public int? OlympValueId
        {
            get { return ComboServ.GetComboIdInt(cbOlympValue); }
            set { ComboServ.SetComboId(cbOlympValue, value); }
        }
        public int? OlympYear
        {
            get { return ComboServ.GetComboIdInt(cbOlympYear); }
            set { ComboServ.SetComboId(cbOlympYear, value); }
        }
        public int? AdditionalMark
        {
            get 
            {
                int iRet = 0;
                int.TryParse(tbAdditionalMark.Text, out iRet);
                return iRet;
            }
            set 
            {
                if (value.HasValue)
                    tbAdditionalMark.Text = value.ToString();
            }
        }
        #endregion

        public CardOlympResultToAdditionalMark(Guid? EntryId)
            : this(null, EntryId, false)
        {
        }

        public CardOlympResultToAdditionalMark(string olId, Guid? EntryId, bool isReadOnly)
            : base(olId)
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

            _title = "Бонусный балл за олимпиаду";
            _tableName = "ed.OlympResultToAdditionalMark";
            this.MdiParent = MainClass.mainform;

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    List<KeyValuePair<string, string>> lstYears = new List<KeyValuePair<string, string>>();
                    for (int i = MainClass.iPriemYear; i > MainClass.iPriemYear - 5; i--)
                        lstYears.Add(new KeyValuePair<string, string>(i.ToString(), i.ToString()));
                    ComboServ.FillCombo(cbOlympYear, lstYears, false, false);

                    ComboServ.FillCombo(cbOlympType, HelpClass.GetComboListByTable("ed.OlympType", "ORDER BY Id"), false, false);
                    UpdateAfterType();
                    FillAfterOlympName();
                    FillAfterOlympSubject();
                    ComboServ.FillCombo(cbOlympValue, HelpClass.GetComboListByTable("ed.OlympValue"), false, false);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы ", exc);
            }
        }
        private void UpdateAfterType()
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<KeyValuePair<string, string>> lst =
                    ((from ob in context.extOlympBook
                      where ob.OlympTypeId == OlympTypeId
                      && ob.OlympYear == OlympYear
                      select new
                      {
                          Id = ob.OlympNameId,
                          Name = ob.OlympNameName
                      })
                      .Distinct())
                      .ToList()
                      .Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name))
                      .ToList();

                cbOlympName.Enabled = true;
                ComboServ.FillCombo(cbOlympName, lst, false, false);
                cbOlympName.SelectedIndex = 0;
            }

            if (OlympTypeId == 1 || OlympTypeId == 2 || OlympTypeId == 7)
            {
                cbOlympName.Enabled = false;
                cbOlympLevel.Enabled = false;
            }
        }
        private void FillAfterOlympName()
        {
            using (PriemEntities context = new PriemEntities())
            {
                if (!OlympTypeId.HasValue)
                    return;

                List<KeyValuePair<string, string>> lst =
                    ((from ob in context.extOlympBook
                      where ob.OlympTypeId == OlympTypeId && ob.OlympNameId == OlympNameId
                      && ob.OlympYear == OlympYear
                      select new
                      {
                          Id = ob.OlympSubjectId,
                          Name = ob.OlympSubjectName
                      }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                cbOlympSubject.Enabled = true;
                ComboServ.FillCombo(cbOlympSubject, lst, false, false);
                cbOlympSubject.SelectedIndex = 0;
            }
        }
        private void FillAfterOlympSubject()
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<KeyValuePair<string, string>> lst =
                        ((from ob in context.extOlympBook
                          where ob.OlympTypeId == OlympTypeId && ob.OlympNameId == OlympNameId
                          && ob.OlympSubjectId == OlympSubjectId
                          && ob.OlympYear == OlympYear
                          select new
                          {
                              Id = ob.OlympLevelId,
                              Name = ob.OlympLevelName
                          }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                cbOlympLevel.Enabled = true;
                ComboServ.FillCombo(cbOlympLevel, lst, false, false);
                cbOlympLevel.SelectedIndex = 0;

                if (OlympTypeId == 1 || OlympTypeId == 2 || OlympTypeId == 7)
                {
                    cbOlympLevel.Enabled = false;
                }
            }
        }
        protected override void InitHandlers()
        {
            cbOlympType.SelectedIndexChanged += new EventHandler(cbOlympType_SelectedIndexChanged);
            cbOlympYear.SelectedIndexChanged += cbOlympYear_SelectedIndexChanged;
            cbOlympName.SelectedIndexChanged += new EventHandler(cbOlympName_SelectedIndexChanged);
            cbOlympSubject.SelectedIndexChanged += new EventHandler(cbOlympSubject_SelectedIndexChanged);
        }

        void cbOlympYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAfterType();
        }
        void cbOlympSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillAfterOlympSubject();
        }
        void cbOlympName_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillAfterOlympName();
        }
        void cbOlympType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAfterType();
        }

        protected override void FillCard()
        {
            if (_Id == null)
                return;

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    OlympResultToAdditionalMark olymp =
                        (from ec in context.OlympResultToAdditionalMark
                         where ec.Id == IntId
                                                        
                         select ec).FirstOrDefault();

                    if (olymp == null)
                        return;

                    OlympYear = olymp.OlympYear;
                    OlympTypeId = olymp.OlympTypeId;
                    UpdateAfterType();
                    OlympNameId = olymp.OlympNameId;
                    FillAfterOlympName();
                    OlympSubjectId = olymp.OlympSubjectId;
                    FillAfterOlympSubject();
                    OlympLevelId = olymp.OlympLevelId;
                    OlympValueId = olymp.OlympValueId;
                    AdditionalMark = olymp.AdditionalMark;
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

            if (OlympTypeId == MainClass.olympSpbguId)
                cbOlympSubject.Enabled = false;
            else
                cbOlympLevel.Enabled = false;
        }

        protected override void InsertRec(PriemEntities context, ObjectParameter idParam)
        {
            context.OlympResultToAdditionalMark_Insert(_EntryId, OlympTypeId, OlympLevelId, OlympNameId, OlympSubjectId, OlympValueId, OlympYear, AdditionalMark, idParam);
        }
        protected override void UpdateRec(PriemEntities context, int id)
        {
            context.OlympResultToAdditionalMark_Update(_EntryId, OlympTypeId, OlympLevelId, OlympNameId, OlympSubjectId, OlympValueId, OlympYear, AdditionalMark, id);
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
