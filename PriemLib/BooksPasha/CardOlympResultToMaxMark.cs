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
using System.Data.Entity.Core.Objects;

namespace PriemLib
{
    public partial class CardOlympResultToMaxMark : BookCardInt
    {
        #region Fields
        public int? OlympTypeId
        {
            get { return ComboServ.GetComboIdInt(cbOlympType); }
            set { ComboServ.SetComboId(cbOlympType, value); }
        }
        public int? OlympYear
        {
            get { return ComboServ.GetComboIdInt(cbOlympYear); }
            set { ComboServ.SetComboId(cbOlympYear, value); }
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
        public int? OlympValueId
        {
            get { return ComboServ.GetComboIdInt(cbOlympValue); }
            set { ComboServ.SetComboId(cbOlympValue, value); }
        }
        public int? EgeExamId
        {
            get { return ComboServ.GetComboIdInt(cbEgeExam); }
            set { ComboServ.SetComboId(cbEgeExam, value); }
        }
        public decimal MinEge
        {
            get
            {
                decimal d = 0;
                decimal.TryParse(tbMinEge.Text.Trim(), out d);
                return d;
            }
            set { tbMinEge.Text = value.ToString(); }
        }
        #endregion

        private Guid _ExamInEntryBlockId;

        public CardOlympResultToMaxMark(string id, Guid ExamInEntryBlockId) : base(id)
        {
            InitializeComponent();
            _ExamInEntryBlockId = ExamInEntryBlockId;
            InitControls();
        }

        protected override void ExtraInit()
        {
            base.ExtraInit();
            _tableName = "ed.OlympResultToMaxMark";
            _title = "Результат олимпиады как 100 баллов за экзамен";
            btnSaveAsNew.Visible = true;

            using (PriemEntities context = new PriemEntities())
            {
                List<KeyValuePair<string, string>> lstYears = new List<KeyValuePair<string, string>>();
                for (int i = MainClass.iPriemYear; i > MainClass.iPriemYear - 5; i--)
                    lstYears.Add(new KeyValuePair<string, string>(i.ToString(), i.ToString()));
                ComboServ.FillCombo(cbOlympYear, lstYears, false, false);
                ComboServ.FillCombo(cbOlympType, HelpClass.GetComboListByTable("ed.OlympType", "ORDER BY Id"), false, false);
                UpdateAfterType();
                ComboServ.FillCombo(cbOlympValue, HelpClass.GetComboListByTable("ed.OlympValue"), false, false);

                var Ex = context.ExamInEntryBlock.Where(x => x.Id == _ExamInEntryBlockId).FirstOrDefault();
                if (Ex != null)
                {
                    tbExamInEntryBlockName.Text = Ex.Name;
                    var ExUnit = Ex.ExamInEntryBlockUnit.FirstOrDefault();
                    if (ExUnit != null)
                    {
                        var e = ExUnit.Exam;
                        if (e.IsAdditional)
                        {
                            var exams =
                                (from extEx in context.extExamInEntry
                                 join Ege in context.EgeToExam on extEx.ExamId equals Ege.ExamId
                                 where extEx.EntryId == Ex.EntryId
                                 select new
                                 {
                                     extEx.ExamId,
                                     extEx.ExamName
                                 }).Distinct()
                                 .ToList()
                                 .Select(x => new KeyValuePair<string, string>(x.ExamId.ToString(), x.ExamName))
                                 .ToList();

                            ComboServ.FillCombo(cbEgeExam, exams, false, false);
                        }
                        else
                        {
                            var exams =
                                (from extEx in context.extExamInEntry
                                 join Ege in context.EgeToExam on extEx.ExamId equals Ege.ExamId
                                 where extEx.ExamInEntryBlockId == _ExamInEntryBlockId
                                 select new
                                 {
                                     extEx.ExamId,
                                     extEx.ExamName
                                 }).Distinct()
                                 .ToList()
                                 .Select(x => new KeyValuePair<string, string>(x.ExamId.ToString(), x.ExamName))
                                 .ToList();

                            ComboServ.FillCombo(cbEgeExam, exams, false, false);
                        }
                    }
                }
            }
        }

        protected override void InitHandlers()
        {
            cbOlympType.SelectedIndexChanged += (a, b) => { UpdateAfterType(); };
            cbOlympYear.SelectedIndexChanged += (a, b) => { UpdateAfterType(); };
            cbOlympName.SelectedIndexChanged += (a, b) => { FillAfterOlympName(); };
            cbOlympProfile.SelectedIndexChanged += (a, b) => { FillAfterOlympProfile(); };
            cbOlympSubject.SelectedIndexChanged += (a, b) => { FillAfterOlympSubject(); }; ;
        }

        #region Handlers
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
        #endregion

        protected override void FillCard()
        {
            if (!IntId.HasValue)
                return;

            using (PriemEntities context = new PriemEntities())
            {
                var ent = context.OlympResultToMaxMark.Where(x => x.Id == IntId.Value).FirstOrDefault();
                if (ent == null)
                {
                    WinFormsServ.Error("Данные в базе не найдены!");
                    return;
                }

                var olymp = context.OlympBook.Where(x => x.Id == ent.OlympBookId).FirstOrDefault();
                if (olymp == null)
                {
                    WinFormsServ.Error("Не найдены данные олимпиады!");
                    return;
                }

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

                //
                OlympValueId = ent.OlympValueId;
                EgeExamId = ent.EgeExamId;
                MinEge = ent.MinEge;
            }
        }

        protected override bool CheckFields()
        {
            bool bRet = true;
            epError.Clear();

            if (!OlympYear.HasValue)
            {
                bRet = false;
                epError.SetError(cbOlympYear, "Не указано значение!");
            }
            if (!OlympTypeId.HasValue)
            {
                bRet = false;
                epError.SetError(cbOlympType, "Не указано значение!");
            }
            if (!OlympNameId.HasValue)
            {
                bRet = false;
                epError.SetError(cbOlympName, "Не указано значение!");
            }
            if (!OlympProfileId.HasValue)
            {
                bRet = false;
                epError.SetError(cbOlympProfile, "Не указано значение!");
            }
            if (!OlympSubjectId.HasValue)
            {
                bRet = false;
                epError.SetError(cbOlympSubject, "Не указано значение!");
            }
            if (!OlympValueId.HasValue)
            {
                bRet = false;
                epError.SetError(cbOlympValue, "Не указано значение!");
            }
            if (!EgeExamId.HasValue)
            {
                bRet = false;
                epError.SetError(cbEgeExam, "Не указано значение!");
            }

            using (PriemEntities context = new PriemEntities())
            {

            }

            return bRet;
        }

        protected override void InsertRec(PriemEntities context, ObjectParameter idParam)
        {
            OlympResultToMaxMark ent = new OlympResultToMaxMark();
            UpdateValues(ref ent);
            context.OlympResultToMaxMark.Add(ent);
            context.SaveChanges();
            idParam.Value = ent.Id;
        }
        protected override void UpdateRec(PriemEntities context, int id)
        {
            OlympResultToMaxMark ent = context.OlympResultToMaxMark.Where(x => x.Id == id).FirstOrDefault();
            if (ent != null)
            {
                UpdateValues(ref ent);
                context.SaveChanges();
            }
        }

        private void UpdateValues(ref OlympResultToMaxMark ent)
        {
            ent.ExamInEntryBlockId = _ExamInEntryBlockId;
            ent.OlympBookId = OlympProvider.GetOlympBookId(OlympYear.Value, OlympTypeId.Value, OlympNameId.Value, OlympProfileId.Value, OlympSubjectId.Value);
            ent.OlympValueId = OlympValueId.Value;
            ent.EgeExamId = EgeExamId.Value;
            ent.MinEge = MinEge;
        }
    }
}