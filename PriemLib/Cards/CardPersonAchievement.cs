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
        private Guid PersonId;
        private string sPersonId;

        public CardPersonAchievement(string _sPersonId)
        {
            InitializeComponent();

            _Id = null;
            formOwner = null;
            sPersonId = _sPersonId;

            InitControls();
        }
        public CardPersonAchievement(string sId, int? rowInd, BaseFormEx formOwner)
        {
            InitializeComponent();

            _Id = sId;

            this.formOwner = formOwner;
            if (rowInd.HasValue)
                ownerRowIndex = rowInd.Value;

            InitControls();
        }

        protected override void ExtraInit()
        {
            base.ExtraInit();
            _tableName = "ed.PersonAchievement";

            Guid.TryParse(sPersonId, out PersonId);

            try
            {
                ComboServ.FillCombo(cbAchievementType, HelpClass.GetComboListByTable("ed.AchievementType"), true, false);
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы ", exc);
            }
        }
        protected override void FillCard()
        {
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
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка при заполнении формы ", ex);
            }
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
