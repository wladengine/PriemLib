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

namespace PriemLib
{
    public partial class CardPersonAchievement : CardFromList
    {
        private int? AchievementTypeId
        {
            get { return ComboServ.GetComboIdInt(cbAchievementType); }
            set { ComboServ.SetComboId(cbAchievementType, value); }
        }
        private string Author
        {
            get { return lblAuthor.Text.Trim(); }
            set { lblAuthor.Text = value; }
        }

        private Guid PersonId;
        private string sPersonId;

        public CardPersonAchievement(string _sPersonId)
        {
            InitializeComponent();

            _Id = null;
            formOwner = null;
            sPersonId = _sPersonId;

            tcCard = new TabControl();

            InitControls();
        }
        public CardPersonAchievement(string sId, int? rowInd, BaseFormEx formOwner)
        {
            InitializeComponent();

            _Id = sId;

            this.formOwner = formOwner;
            if (rowInd.HasValue)
                ownerRowIndex = rowInd.Value;
            tcCard = new TabControl();

            InitControls();
        }

        protected override void ExtraInit()
        {
            base.ExtraInit();
            _tableName = "ed.PersonAchievement";

            Guid.TryParse(sPersonId, out PersonId);

            try
            {
                string sTypesQuery = string.Format("ed.AchievementType WHERE AchievementType.StudyLevelGroupId IN ({0})", Util.BuildStringWithCollection(MainClass.lstStudyLevelGroupId));
                var lstTypes = HelpClass.GetComboListByTable(sTypesQuery);
                ComboServ.FillCombo(cbAchievementType, lstTypes, true, false);
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы ", exc);
            }
        }
        protected override void FillCard()
        {
            if (string.IsNullOrEmpty(_Id))
                return;

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    var PersAch = (from Ach in context.PersonAchievement
                                   where Ach.Id == GuidId
                                   select Ach).FirstOrDefault();

                    if (PersAch == null)
                    {
                        WinFormsServ.Error("Не найдена запись в базе");
                        return;
                    }

                    PersonId = PersAch.PersonId;
                    AchievementTypeId = PersAch.AchievementTypeId;
                    //Author = PersAch.Author;
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка при заполнении формы ", ex);
            }
        }
        protected override bool CheckFields()
        {
            if (!AchievementTypeId.HasValue)
            {
                epError.SetError(cbAchievementType, "Не указано достижение");
                return false;
            }
            else
                epError.Clear();

            using (PriemEntities context = new PriemEntities())
            {
                if (string.IsNullOrEmpty(_Id) && context.PersonAchievement.Where(x => x.AchievementTypeId == AchievementTypeId && x.PersonId == PersonId).Count() > 0)
                {
                    WinFormsServ.Error("У абитуриента уже имеется указанное достижение");
                    //update list
                    OnSave();
                    return false;
                }
            }

            return base.CheckFields();
        }

        protected override void InsertRec(PriemEntities context, System.Data.Entity.Core.Objects.ObjectParameter idParam)
        {
            context.PersonAchievement_insert(PersonId, AchievementTypeId, idParam);
        }
        protected override void UpdateRec(PriemEntities context, Guid id)
        {
            context.PersonAchievement_update(AchievementTypeId, id);
        }
    }
}
