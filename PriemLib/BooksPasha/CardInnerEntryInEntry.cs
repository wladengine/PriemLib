using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EducServLib;
using BaseFormsLib;
using System.Transactions;

namespace PriemLib
{
    public partial class CardInnerEntryInEntry : BaseCard
    {
        #region Fields
        public event Action ToUpdateList;
        private Guid? GuidId;
        private Guid EntryId;
        private int _licenseProgramId;
        private int LicenseProgramId 
        { 
            get { return _licenseProgramId; }
            set
            {
                using (PriemEntities context = new PriemEntities())
                {
                    _licenseProgramId = value;
                    tbLicenseProgram.Text = context.SP_LicenseProgram.Where(x => x.Id == _licenseProgramId)
                        .Select(x => new { x.Code, x.Name }).ToList().Select(x => x.Code + " " + x.Name).FirstOrDefault();
                }
            }
        }
        private int KCP
        {
            get
            {
                int iRet = 0;
                int.TryParse(tbKCP.Text, out iRet);
                return iRet;
            }
            set { tbKCP.Text = value.ToString(); }
        }
        private int ObrazProgramId
        {
            get { return ComboServ.GetComboIdInt(cbObrazProgram).Value; }
            set { ComboServ.SetComboId(cbObrazProgram, value); }
        }
        private int ProfileId
        {
            get { return ComboServ.GetComboIdInt(cbProfile).Value; }
            set { ComboServ.SetComboId(cbProfile, value); }
        }
        private int? EgeExamNameId
        {
            get { return ComboServ.GetComboIdInt(cbEgeExamName); }
            set { ComboServ.SetComboId(cbEgeExamName, value); }
        }
        private Guid? ExamInEntryBlockId
        {
            get { return ComboServ.GetComboIdGuid(cbExamInEntryBlock); }
            set { ComboServ.SetComboId(cbExamInEntryBlock, value); }
        }
        #endregion

        public CardInnerEntryInEntry(Guid entryId, int licenseProgramId)
        {
            InitializeComponent();
            EntryId = entryId;
            LicenseProgramId = licenseProgramId;
            UpdateCombo();
            InitControls();
        }
        public CardInnerEntryInEntry(Guid Id)
        {
            InitializeComponent();
            GuidId = Id;
            LoadValues();
            InitControls();
        }

        protected override void ExtraInit()
        {
            this.MdiParent = MainClass.mainform;
        }
        protected override void FillCard()
        {
            LoadValues();
        }
        private void LoadValues()
        {
            if (!GuidId.HasValue)
                return;
            using (PriemEntities context = new PriemEntities())
            {
                var z = context.InnerEntryInEntry.Where(x => x.Id == GuidId).FirstOrDefault();
                if (z == null)
                {
                    WinFormsServ.Error("Не удалось получить значение InnerEntryInEntry");
                    return;
                }

                EntryId = z.Entry.Id;
                LicenseProgramId = z.Entry.LicenseProgramId;
                UpdateCombo();
                ObrazProgramId = z.ObrazProgramId;
                ProfileId = z.ProfileId;

                EgeExamNameId = z.EgeExamNameId;
                ExamInEntryBlockId = z.ExamInEntryBlockId;
                KCP = z.KCP;
            }
        }
        private void UpdateCombo()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var Ent = context.qObrazProgram.Where(x => x.LicenseProgramId == LicenseProgramId).Select(x => new { x.Id, x.Crypt, x.Name }).ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Crypt + " " + x.Name)).ToList();
                ComboServ.FillCombo(cbObrazProgram, Ent, false, false);

                var Prof = context.SP_Profile.Select(x => new { x.Id, x.Name }).ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).ToList();
                ComboServ.FillCombo(cbProfile, Prof, false, false);

                var EGE = context.EgeExamName.Select(x => new { x.Id, x.Name }).ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).ToList();
                ComboServ.FillCombo(cbEgeExamName, EGE, true, false);

                var ExamsBlock = context.ExamInEntryBlock.Where(x => x.EntryId == EntryId && x.ParentExamInEntryBlockId == null).Select(x => new { x.Id, x.Name }).ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).ToList();
                ComboServ.FillCombo(cbExamInEntryBlock, ExamsBlock, true, false);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCard();
        }
        
        protected override bool SaveRecord()
        {
            return SaveCard();
        }
        private bool SaveCard()
        {
            try
            {
                if (GuidId.HasValue)
                    EntryProvider.InnerEntryInEntry_Update(GuidId.Value, EntryId, ObrazProgramId, ProfileId, KCP, EgeExamNameId);
                else
                    GuidId = EntryProvider.InnerEntryInEntry_Insert(EntryId, ObrazProgramId, ProfileId, KCP, EgeExamNameId);

                //кинуть событие, если не null
                ToUpdateList?.Invoke();

                return true;
            }
            catch (Exception ex)
            {
                WinFormsServ.Error(ex);
                return false;
            }
        }

        protected override void CloseCardAfterSave()
        {
            this.Close();
        }
    }
}
