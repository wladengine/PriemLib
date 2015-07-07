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
    public partial class CardOlympicsToCommonBenefit : BookCard
    {
        private Guid? _EntryId;
        private bool _isReadOnly;
        #region Fields
        public int? OlympTypeId
        {
            get { return ComboServ.GetComboIdInt(cbOlympType); }
            set { ComboServ.SetComboId(cbOlympType, value); }
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
        protected int? IntId
        {
            get
            {
                int gRet;
                if (string.IsNullOrEmpty(_Id) || !int.TryParse(_Id, out gRet))
                    return null;
                else
                    return gRet;
            }
        }

        #endregion

        public CardOlympicsToCommonBenefit(Guid? EntryId)
            : this(null, EntryId, false)
        {
        }
        public CardOlympicsToCommonBenefit(string olId, Guid? EntryId, bool isReadOnly)
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

            _title = "Олимпиады";
            _tableName = "ed.Olympiads";
            this.MdiParent = null;

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    ComboServ.FillCombo(cbOlympType, HelpClass.GetComboListByTable("ed.OlympType", "ORDER BY Id"), false, false);
                    UpdateAfterType();
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

                FillAfterOlympName();
                FillAfterOlympSubject();
            }

            if (OlympTypeId == 1 || OlympTypeId == 2 || OlympTypeId == 7)
            {
                cbOlympName.Enabled = false;
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
        }
        protected override void InitHandlers()
        {
            cbOlympType.SelectedIndexChanged += new EventHandler(cbOlympType_SelectedIndexChanged);
            cbOlympName.SelectedIndexChanged += new EventHandler(cbOlympName_SelectedIndexChanged);
            cbOlympSubject.SelectedIndexChanged += new EventHandler(cbOlympSubject_SelectedIndexChanged);
        }

        private void cbOlympType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAfterType();
        }

        private void cbOlympName_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillAfterOlympName();
        }
        private void cbOlympSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillAfterOlympSubject();
        }

        protected override void FillCard()
        {
            if (_Id == null)
                return;

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    OlympResultToCommonBenefit olymp = (from ec in context.OlympResultToCommonBenefit
                                       where ec.Id == IntId
                                       
                                       select ec).FirstOrDefault();

                    if (olymp == null)
                        return;

                    OlympTypeId = olymp.OlympTypeId;
                    if (OlympTypeId != 1 || OlympTypeId != 2)
                        OlympNameId = olymp.OlympNameId;
                    OlympSubjectId = olymp.OlympSubjectId;
                    OlympValueId = olymp.OlympValueId;
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
            {
                cbOlympSubject.Enabled = false;
            }
            else
                cbOlympName.Enabled = false;
        }
        protected override void InsertRec(PriemEntities context, ObjectParameter idParam)
        {
            context.OlympResultToCommonBenefit_Insert(_EntryId, OlympTypeId, OlympNameId, OlympSubjectId, OlympValueId, idParam);
        }

        protected override void UpdateRec(PriemEntities context, int id)
        {
            context.OlympResultToCommonBenefit_Update(_EntryId, OlympTypeId, OlympNameId, OlympSubjectId, OlympValueId, id);
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
