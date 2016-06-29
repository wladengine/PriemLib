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

namespace PriemLib
{
    public partial class CardOlympBook : BookCard
    {
        public CardOlympBook()
        {
            InitializeComponent();
            InitControls();
        }

        public CardOlympBook(string olId)
        {
            InitializeComponent();
            _Id = olId;                  

            InitControls();
        }

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
        public int? OlympProfileId
        {
            get { return ComboServ.GetComboIdInt(cbOlympProfile); }
            set { ComboServ.SetComboId(cbOlympProfile, value); }
        }
        public int? OlympSubjectId
        {
            get { return ComboServ.GetComboIdInt(cbOlympSubject); }
            set { ComboServ.SetComboId(cbOlympSubject, value); }
        }
        public int? OlympLevelId
        {
            get { return ComboServ.GetComboIdInt(cbOlympLevel); }
            set { ComboServ.SetComboId(cbOlympLevel, value); }
        }

        public int? OlympYear
        {
            get { return ComboServ.GetComboIdInt(cbOlympYear); }
            set { ComboServ.SetComboId(cbOlympYear, value); }
        }

        protected override void ExtraInit()
        {
            base.ExtraInit();
            
            _title = "Олимпиада";
            _tableName = "ed.OlympBook";
            this.MdiParent = null;

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    List<KeyValuePair<string, string>> lstYears = new List<KeyValuePair<string, string>>();
                    for (int i = MainClass.iPriemYear; i > MainClass.iPriemYear - 5; i--)
                        lstYears.Add(new KeyValuePair<string, string>(i.ToString(), i.ToString()));
                    ComboServ.FillCombo(cbOlympYear, lstYears, false, false);
                    ComboServ.FillCombo(cbOlympType, HelpClass.GetComboListByTable("ed.OlympType", "ORDER BY Id "), false, false);
                    ComboServ.FillCombo(cbOlympProfile, HelpClass.GetComboListByTable("ed.OlympProfile", "ORDER BY Name"), false, false);
                    ComboServ.FillCombo(cbOlympName, HelpClass.GetComboListByTable("ed.OlympName", "ORDER BY Number, Name"), false, false);
                    ComboServ.FillCombo(cbOlympSubject, HelpClass.GetComboListByTable("ed.OlympSubject", "ORDER BY Name"), false, false);
                    ComboServ.FillCombo(cbOlympLevel, HelpClass.GetComboListByTable("ed.OlympLevel", "ORDER BY Name"), false, false);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы ", exc);
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
                    OlympBook olymp = (from ec in context.OlympBook
                                       where ec.Id == GuidId
                                       select ec).FirstOrDefault();

                    if (olymp == null)
                        return;

                    OlympTypeId = olymp.OlympTypeId;
                    OlympNameId = olymp.OlympNameId;
                    OlympProfileId = olymp.OlympProfileId;
                    OlympSubjectId = olymp.OlympSubjectId;
                    OlympLevelId = olymp.OlympLevelId;
                    OlympYear = olymp.OlympYear;
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

            if (!MainClass.IsEntryChanger())
                btnSaveChange.Enabled = false;
        }        

        protected override void InsertRec(PriemEntities context, ObjectParameter idParam)
        {
            context.OlympBook_Insert(OlympTypeId, OlympNameId, OlympProfileId, OlympSubjectId, OlympLevelId, OlympYear, idParam);
            string query = "INSERT INTO OlympBook (Id, OlympTypeId, OlympNameId, OlympProfileId, OlympSubjectId, OlympLevelId, OlympYear) VALUES (@Id, @OlympTypeId, @OlympNameId, @OlympProfileId, @OlympSubjectId, @OlympLevelId, @OlympYear)";
            SortedList<string, object> slParams = new SortedList<string,object>();
            slParams.Add("@Id", idParam.Value);
            slParams.Add("@OlympTypeId", OlympTypeId);
            slParams.Add("@OlympNameId", OlympNameId);
            slParams.Add("@OlympProfileId", OlympProfileId);
            slParams.Add("@OlympSubjectId", OlympSubjectId);
            slParams.Add("@OlympLevelId", OlympLevelId);
            slParams.Add("@OlympYear", OlympYear);
            MainClass.BdcOnlineReadWrite.ExecuteQuery(query, slParams);
        }

        protected override void UpdateRec(PriemEntities context, Guid id)
        {
            context.OlympBook_Update(OlympTypeId, OlympNameId, OlympProfileId, OlympSubjectId, OlympLevelId, OlympYear, id);
            string query = "UPDATE OlympBook SET [OlympTypeId]=@OlympTypeId, [OlympNameId]=@OlympNameId, [OlympProfileId]=@OlympProfileId, [OlympSubjectId]=@OlympSubjectId, [OlympLevelId]=@OlympLevelId, OlympYear=@OlympYear WHERE Id=@id";
            SortedList<string, object> slParams = new SortedList<string, object>();
            slParams.Add("@Id", id);
            slParams.Add("@OlympTypeId", OlympTypeId);
            slParams.Add("@OlympNameId", OlympNameId);
            slParams.Add("@OlympProfileId", OlympProfileId);
            slParams.Add("@OlympSubjectId", OlympSubjectId);
            slParams.Add("@OlympLevelId", OlympLevelId);
            slParams.Add("@OlympYear", OlympYear);
            MainClass.BdcOnlineReadWrite.ExecuteQuery(query, slParams);
        }

        protected override void OnSave()
        {           
            MainClass.DataRefresh();            
        }

        protected override void CloseCardAfterSave()
        {
            if (!_isModified)
                this.Close();
        }
    }    
}
