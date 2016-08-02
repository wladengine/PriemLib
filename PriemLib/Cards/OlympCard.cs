using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

using EducServLib;
using BaseFormsLib;
using System.Data.Entity.Core.Objects;

namespace PriemLib
{
    public partial class OlympCard : BookCard
    {
        private Guid? _abitId;
        private bool _isReadOnly;    
        
        // ����������� ������ �����
        public OlympCard(Guid? abitId)
            : this(null, abitId, false)
        {
        }  

        // ����������� ����� ��� ���������
        public OlympCard(string olId, Guid? abitId, bool isReadOnly)
        {
            InitializeComponent();                      
            _abitId = abitId;
            _Id = olId;
            _isReadOnly = isReadOnly;           

            InitControls();
        }       

        protected override void ExtraInit()
        {
            base.ExtraInit();
            
            _title = "������ ���������";
            _tableName = "ed.Olympiads";
            this.MdiParent = MainClass.mainform;

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    List<KeyValuePair<string, string>> lstYears = new List<KeyValuePair<string, string>>();
                    for (int i = MainClass.iPriemYear; i > MainClass.iPriemYear - 5; i--)
                        lstYears.Add(new KeyValuePair<string,string>(i.ToString(), i.ToString()));
                    ComboServ.FillCombo(cbOlympYear, lstYears, false, false);

                    ComboServ.FillCombo(cbOlympType, HelpClass.GetComboListByTable("ed.OlympType", "ORDER BY Id"), false, false);
                    UpdateAfterType();
                    ComboServ.FillCombo(cbOlympValue, HelpClass.GetComboListByTable("ed.OlympValue"), false, false);                    
                }                
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("������ ��� ������������� ����� ", exc);
            }
        }

        protected override void InitHandlers()
        {
            cbOlympType.SelectedIndexChanged += new EventHandler(cbOlympType_SelectedIndexChanged);
            cbOlympYear.SelectedIndexChanged += cbOlympYear_SelectedIndexChanged;
            cbOlympName.SelectedIndexChanged += new EventHandler(cbOlympName_SelectedIndexChanged);
            cbOlympProfile.SelectedIndexChanged += new EventHandler(cbOlympProfile_SelectedIndexChanged);
            cbOlympSubject.SelectedIndexChanged += new EventHandler(cbOlympSubject_SelectedIndexChanged);
            
        }

        void cbOlympYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAfterType();
        }
        void cbOlympProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillAfterOlympProfile();
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

                FillAfterOlympName();
                FillAfterOlympSubject();
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
                          Id = ob.OlympProfileId,
                          Name = ob.OlympProfileName
                      }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                cbOlympProfile.Enabled = true;
                ComboServ.FillCombo(cbOlympProfile, lst, false, false);
                cbOlympProfile.SelectedIndex = 0;
            }
        }
        private void FillAfterOlympProfile()
        {
            using (PriemEntities context = new PriemEntities())
            {
                if (!OlympTypeId.HasValue)
                    return;

                List<KeyValuePair<string, string>> lst =
                    ((from ob in context.extOlympBook
                      where ob.OlympTypeId == OlympTypeId && ob.OlympNameId == OlympNameId
                      && ob.OlympYear == OlympYear && ob.OlympProfileId == OlympProfileId
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
                          && ob.OlympSubjectId == OlympSubjectId && ob.OlympProfileId == OlympProfileId
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

        protected override void FillCard()
        {
            if (_Id == null)
                return;

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    Olympiads olymp = (from ec in context.Olympiads
                                       where ec.Id == GuidId
                                       select ec).FirstOrDefault();

                    if (olymp == null)
                        return;

                    OlympTypeId = olymp.OlympTypeId;
                    OlympYear = olymp.OlympYear;
                    UpdateAfterType();

                    if (OlympTypeId != 1 || OlympTypeId != 2)
                        OlympNameId = olymp.OlympNameId;
                    FillAfterOlympName();
                    OlympProfileId = olymp.OlympProfileId;
                    FillAfterOlympProfile();
                    OlympSubjectId = olymp.OlympSubjectId;
                    FillAfterOlympSubject();
                    if (OlympTypeId != 1 || OlympTypeId != 2)
                        OlympLevelId = olymp.OlympLevelId;
                    OlympValueId = olymp.OlympValueId;
                    OriginDoc = olymp.OriginDoc;

                    DocumentSeries = olymp.DocumentSeries;
                    DocumentNumber = olymp.DocumentNumber;
                    DocumentDate = olymp.DocumentDate;

                    bool readOnly = !string.IsNullOrEmpty(DocumentNumber) && (olymp.Abiturient1.Count() > 0 || olymp.Mark.Count() > 0);
                    _isReadOnly = readOnly;
                    lblReadOnly.Visible = readOnly;
                }
            }
            catch (DataException de)
            {
                WinFormsServ.Error("������ ��� ���������� ����� ", de);
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
                cbOlympLevel.Enabled = false;
            }
            else
                cbOlympName.Enabled = false;           
        }
        protected override bool CheckFields()
        {
            epError.Clear();
            bool res = true;

            if (string.IsNullOrEmpty(DocumentNumber))
            {
                string mes = "�� ������ ����� �������. ��� ��������, ��� �� �� ������� ������������ ������ ��������� ��� ��������� ������ 1 ��� 2 ������. ����������?";
                var dr = MessageBox.Show(mes, "", MessageBoxButtons.YesNo);
                if (dr == System.Windows.Forms.DialogResult.No)
                    res = false;
            }

            if (!string.IsNullOrEmpty(DocumentSeries) && DocumentSeries.Length > 10)
            {
                epError.SetError(tbSeries, "����� ���� �� ������ ��������� 10 ��������");
                res = false;
            }
            if (!string.IsNullOrEmpty(DocumentNumber) && DocumentNumber.Length > 20)
            {
                epError.SetError(tbNumber, "����� ���� �� ������ ��������� 20 ��������");
                res = false;
            }

            if (!OriginDoc)
                return res;

            using (PriemEntities context = new PriemEntities())
            {
                Guid? persId = (from ab in context.extAbit
                                where ab.Id == _abitId
                                select ab.PersonId).FirstOrDefault();
                int cnt;

                if (_Id == null)
                    cnt = (from ol in context.extOlympiadsAll
                           where ol.OlympLevelId == OlympLevelId && ol.OlympTypeId == OlympTypeId
                           && ol.OlympNameId == OlympNameId && ol.OlympSubjectId == OlympSubjectId && ol.OlympProfileId == OlympProfileId
                           && ol.OlympValueId == OlympValueId
                           && ol.PersonId == persId && ol.OriginDoc
                           select ol).Count();
                else
                    cnt = (from ol in context.extOlympiadsAll
                           where ol.OlympLevelId == OlympLevelId && ol.OlympTypeId == OlympTypeId
                           && ol.OlympNameId == OlympNameId && ol.OlympSubjectId == OlympSubjectId && ol.OlympProfileId == OlympProfileId
                           && ol.OlympValueId == OlympValueId
                           && ol.PersonId == persId && ol.Id != GuidId && ol.OriginDoc
                           select ol).Count();

                if (cnt > 0)
                {
                    epError.SetError(chbOriginDoc, "����� ��������� ������� ��������� �� ������ ���������!");                   
                    return false;
                }
                else
                    epError.Clear();

                return res;
            }
        }

        protected override void InsertRec(PriemEntities context, ObjectParameter idParam)
        {
            context.Olympiads_Insert(OlympTypeId, OlympNameId, OlympProfileId, OlympSubjectId, OlympLevelId, OlympValueId, OlympYear, OriginDoc, _abitId, DocumentSeries, DocumentNumber, DocumentDate, idParam);
        }
        protected override void UpdateRec(PriemEntities context, Guid id)
        {
            context.Olympiads_Update(OlympTypeId, OlympNameId, OlympProfileId, OlympSubjectId, OlympLevelId, OlympValueId, OlympYear, OriginDoc, DocumentSeries, DocumentNumber, DocumentDate, id);
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