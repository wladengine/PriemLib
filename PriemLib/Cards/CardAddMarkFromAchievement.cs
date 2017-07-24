using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BaseFormsLib;
using EducServLib;

namespace PriemLib.Cards
{
    public partial class CardAddMarkFromAchievement : BaseCard
    {
        private Guid _AbiturientId;
        public event Action OK;
        private int? AchievementTypeId { get { return ComboServ.GetComboIdInt(cbAchievementType); } }
        private Guid? OlympiadId { get { return ComboServ.GetComboIdGuid(cbOlympiad); } }
        private int Mark
        {
            get
            {
                int tmp;
                int.TryParse(tbMark.Text.Trim(), out tmp);
                return tmp;
            }
        }
        
        public CardAddMarkFromAchievement(Guid AbiturientId)
        {
            InitializeComponent();

            _AbiturientId = AbiturientId;

            InitControls();
        }

        protected override void ExtraInit()
        {
            FillComboAchievementType();
            FillComboOlympiad();
        }

        protected override void FillCard()
        {
            return;
        }
        private void FillComboAchievementType()
        {
            using (PriemEntities context = new PriemEntities())
            {
                int iSLG = context.qAbitAll.Where(x => x.Id == _AbiturientId).Select(x => x.StudyLevelGroupId).DefaultIfEmpty(0).FirstOrDefault();
                
                var src = context.AchievementType.Where(x => iSLG == 0 ? MainClass.lstStudyLevelGroupId.Contains(x.StudyLevelGroupId) : x.StudyLevelGroupId == iSLG)
                    .Select(x => new { x.Id, x.Name, x.Mark })
                    .ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name + " (MAX " + x.Mark + ")"))
                    .ToList();

                ComboServ.FillCombo(cbAchievementType, src, false, false);
            }
        }

        private void FillComboOlympiad()
        {
            using (PriemEntities context = new PriemEntities())
            {
                Guid PersonId = context.Abiturient.Where(x => x.Id == _AbiturientId).Select(x => x.PersonId).First();
                var src = context.extOlympiads.Where(x => x.PersonId == PersonId)
                    .Select(x => new { x.Id, x.OlympName, x.OlympSubjectName, x.OlympYear, x.OlympValueName })
                    .ToList()
                    .OrderBy(x => x.OlympYear).ThenBy(x => x.OlympName).ThenBy(x => x.OlympSubjectName).ThenBy(x => x.OlympValueName)
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), "(" + x.OlympYear + ") " + x.OlympName + " (" + x.OlympSubjectName + ") - " + x.OlympValueName))
                    .ToList();

                ComboServ.FillCombo(cbOlympiad, src, false, false);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool CheckFields()
        {
            bool bRet = true;
            epError.Clear();
            if (!AchievementTypeId.HasValue)
            {
                epError.SetError(cbAchievementType, "не указано значение!");
                bRet = false;
            }
            else
            {
                using (PriemEntities context = new PriemEntities())
                {
                    var Ach = context.AchievementType.Where(x => x.Id == AchievementTypeId).FirstOrDefault();
                    if (Ach == null)
                    {
                        epError.SetError(cbAchievementType, "не найдено в базе");
                        return false;
                    }
                    if (Ach.NeedOlympiadToAccept && !OlympiadId.HasValue)
                    {
                        epError.SetError(cbOlympiad, "не указано обязательное значение!");
                        bRet = false;
                    }
                    if (Mark == 0)
                    {
                        epError.SetError(tbMark, "некорректное значение!");
                        bRet = false;
                    }
                    if (Ach.Mark < Mark)
                    {
                        epError.SetError(tbMark, "балл превышает максимально разрешённое значение!");
                        bRet = false;
                    }

                    int cnt = context.MarkFromAchievement.Where(x => x.AbiturientId == _AbiturientId && x.AchievementTypeId == AchievementTypeId).Count();
                    if (cnt > 0)
                    {
                        epError.SetError(cbAchievementType, "баллы по данному типу ИД уже указаны!");
                        bRet = false;
                    }
                }
            }

            return bRet;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (CheckFields())
            {
                using (PriemEntities context = new PriemEntities())
                {
                    MarkFromAchievement mrk = new MarkFromAchievement();
                    mrk.Id = Guid.NewGuid();
                    mrk.AbiturientId = _AbiturientId;
                    mrk.AchievementTypeId = AchievementTypeId.Value;
                    mrk.Value = Mark;
                    mrk.DateCreated = DateTime.Now;
                    mrk.CreateAuthor = MainClass.GetUserName();

                    context.MarkFromAchievement.Add(mrk);
                    context.SaveChanges();
                }

                if (OK != null)
                    OK();

                Close();
            }
        }
    }
}
