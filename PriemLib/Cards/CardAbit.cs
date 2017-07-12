using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

using BaseFormsLib;
using EducServLib;
 
using System.Data.Entity.Core.Objects;
using System.Threading.Tasks;

namespace PriemLib
{
    public partial class CardAbit : CardFromList
    {
        private DataRefreshHandler _drh;

        private Guid? _personId;
        private bool inEnableProtocol;
        private bool inEntryView;
        private bool lockHasOrigin;
        private int? abitBarcode;
        private int? persBarcode;
        private LoadFromInet load;

        // конструктор нового заявления для человека
        public CardAbit(Guid? personId)
        {
            InitializeComponent();
            _Id = null;
            _personId = personId;
            formOwner = null;
            tcCard = tabCard;

            //InitControls();
            this.Load += (x, y) => InitControls();
        }

        public CardAbit(string abId, int? rowInd, BaseFormEx formOwner)
        {
            InitializeComponent();
            _Id = abId;
            _personId = null;
            tcCard = tabCard;

            this.formOwner = formOwner;
            if (rowInd.HasValue)
                ownerRowIndex = rowInd.Value;

            //InitControls();
            this.Load += (x, y) => InitControls();
        }

        protected override void InitControls()
        {
            //не понятно, зачем это было надо, но без него работает явно лучше
            //InitFocusHandlers();

            ExtraInit();
            FillCard();
            InitHandlers();

            if (_Id == null)
            {
                _isModified = true;
                btnSaveChange.Text = Constants.BTN_SAVE_TITLE;
                this.Text = Constants.CARD_TITLE + _title + Constants.CARD_MODIFIED;

                IsForeign = true;
            }
            else
            {
                _isModified = false;
                ReadOnlyCard();
            }

            SetReadOnlyFieldsAfterFill();
        }
        protected override void ExtraInit()
        {
            base.ExtraInit();
            _tableName = "ed.Abiturient";

            load = new LoadFromInet();

            _drh = new DataRefreshHandler(UpdateFIO);
            MainClass.AddHandler(_drh);

            tcCard = tabCard;
            if (tabCard.TabPages.Contains(tpEntry))
                tabCard.TabPages.Remove(tpEntry);
            abitBarcode = 0;

            if (MainClass.dbType == PriemType.Priem)
            {
                btnAddAbiturientAchievement.Visible = false;
                btnDeleteAbiturientAchievement.Visible = false;
            }
                
            if (MainClass.dbType == PriemType.PriemMag)
                gbSecondType.Visible = false;
            else if (MainClass.dbType == PriemType.PriemAG)
                chbIsForeign.Visible = false;

            btnDeleteAbiturientAchievement.Visible = MainClass.IsFacMain() || MainClass.IsPasha();

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    if (_Id != null && _personId == null)
                        _personId = (from ab in context.qAbiturient
                                     where ab.Id == GuidId
                                     select ab.PersonId).FirstOrDefault();

                    ComboServ.FillCombo(cbLanguage, HelpClass.GetComboListByTable("ed.Language"), true, false);
                    ComboServ.FillCombo(cbCelCompetition, HelpClass.GetComboListByTable("ed.CelCompetition"), true, false);

                    FillCompetition(context);

                    cbOtherCompetition.Visible = false;
                    lblCompetitionAddInfo.Visible = false;
                    cbCelCompetition.Visible = false;
                    lblCelCompetition.Visible = false;
                    tbCelCompetitionText.Visible = false;

                    dtBackDocDate.Enabled = false;
                    lblCompFromOlymp.Visible = false;
                    chbHasOriginals.Checked = false;
                    lockHasOrigin = false;
                    lblOtherOriginals.Visible = false;
                    chbChecked.Checked = false;
                    chbChecked.Enabled = false;
                    chbNotEnabled.Checked = false;
                    dtDocInsertDate.Enabled = false;

                    btnDocs.Visible = true;
                    btnDocInventory.Visible = true;

                    if (MainClass.IsPasha())
                    {
                        btnDeleteMark.Visible = btnDeleteMark.Enabled = true;
                        btnAddFix.Enabled = btnAddFix.Visible = true;
                        btnAddGreen.Enabled = btnAddGreen.Visible = true;
                    }
                    else
                    {
                        btnDeleteMark.Visible = btnDeleteMark.Enabled = false;
                        btnAddFix.Enabled = btnAddFix.Visible = false;
                        btnAddGreen.Enabled = btnAddGreen.Visible = false;
                    }

                    cbPrint.Items.Clear();
                    cbPrint.Items.Add("Заявление");
                    cbPrint.Items.Add("Согласие на зачисление");
                    //cbPrint.Items.Add("Наклейка для каждого заявления");
                    cbPrint.Items.Add("Наклейка для заявления");
                    cbPrint.Items.Add("Справка");
                    cbPrint.Items.Add("Выписка из приказа");
                    if (MainClass.RightsFacMain())
                        cbPrint.Items.Add("Экзам.лист");

                    cbPrint.SelectedIndex = 0;
                    btnPrint.Enabled = false;
                }
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
                    FillLockOrigin(context);

                    if (_Id == null)
                    {
                        var pers = (from per in context.Person_AdditionalInfo
                                    where per.PersonId == _personId
                                    select per).FirstOrDefault();

                        int? lanId = pers.LanguageId;
                        ComboServ.SetComboId(cbLanguage, lanId);
                        dtDocInsertDate.Value = DateTime.Now;

                        return;
                    }

                    UpdateFIO(context);
                    persBarcode = context.Person.Where(x => x.Id == _personId).Select(x => x.Barcode).FirstOrDefault();

                    var abit = context.qAbiturient.Where(x => x.Id == GuidId).FirstOrDefault();
                    if (abit == null)
                    {
                        WinFormsServ.Error("Не найдена запись в базе");
                        return;
                    }

                    string PersonNum = context.extPerson.Where(x => x.Id == abit.PersonId).Select(x => x.PersonNum).FirstOrDefault();

                    tbRegNum.Text = MainClass.GetAbitNum(abit.RegNum, PersonNum);

                    IsSecond = abit.IsSecond;
                    IsReduced = abit.IsReduced;
                    IsParallel = abit.IsParallel;
                    IsForeign = abit.IsForeign;

                    LicenseProgramId = abit.LicenseProgramId;
                    ObrazProgramId = abit.ObrazProgramId;
                    ProfileId = abit.ProfileId;
                    FacultyId = abit.FacultyId;
                    StudyFormId = abit.StudyFormId;
                    StudyBasisId = abit.StudyBasisId;

                    UpdateEntryData(context, abit.EntryId);

                    FillCompetition(context);
                    CompetitionId = abit.CompetitionId;
                    UpdateAfterCompetition();
                    UpdateBenefitOlympSource(context);
                    OlympiadId = abit.OlympiadId;

                    OtherCompetitionId = abit.OtherCompetitionId;
                    CelCompetitionId = abit.CelCompetitionId;
                    CelCompetitionText = abit.CelCompetitionText;
                    CompFromOlymp = abit.CompFromOlymp;
                    IsListener = abit.IsListener;
                    IsPaid = abit.IsPaid;
                    BackDoc = abit.BackDoc;
                    BackDocByAdmissionHigh = abit.BackDocByAdmissionHigh;

                    if (abit.BackDoc)
                    {
                        GetWhoSetBackDoc();
                    }
                    else
                    {
                        lblWhoBackDoc.Visible = false;
                        lblWhoBackDoc.Text = "";
                    }

                    HasEntryConfirm = abit.HasEntryConfirm;
                    if (abit.HasEntryConfirm && abit.DateEntryConfirm.HasValue)
                    {
                        dtpDateEntryConfirm.Enabled = false;
                        dtpDateEntryConfirm.Value = abit.DateEntryConfirm.Value;
                    }
                    else
                        dtpDateEntryConfirm.Enabled = true;

                    HasDisabledEntryConfirm = abit.HasDisabledEntryConfirm;

                    if (abit.HasDisabledEntryConfirm && abit.DateDisableEntryConfirm.HasValue)
                    {
                        dtpDateDisableEntryConfirm.Enabled = false;
                        dtpDateDisableEntryConfirm.Value = abit.DateDisableEntryConfirm.Value;
                    }
                    else
                        dtpDateDisableEntryConfirm.Enabled = true;

                    BackDocDate = abit.BackDocDate;
                    DocDate = abit.DocDate;
                    DocInsertDate = abit.DocInsertDate;
                    Checked = abit.Checked;
                    NotEnabled = abit.NotEnabled;
                    Coefficient = abit.Coefficient;
                    LanguageId = abit.LanguageId;
                    HasOriginals = abit.HasOriginals;

                    if (abit.HasOriginals)
                    {
                        GetWhoSetHasOriginals();

                        btnChangeOriginalsDestination.Visible = true;
                    }
                    else
                    {
                        btnChangeOriginalsDestination.Visible = false;

                        lblHasOriginalsUser.Visible = false;
                        lblHasOriginalsUser.Text = "";
                    }

                    Priority = abit.Priority;
                    abitBarcode = abit.Barcode;

                    FillProtocols(context);

                    FillExams(context);
                    Sum = (abit.Sum ?? 0).ToString(); //GetAbitSum(context);

                    FillAdditionalAchievements(context);

                    inEnableProtocol = GetInEnableProtocol(context);
                    inEntryView = GetInEntryView(context);

                    if (!abit.IsViewed)
                        context.Abiturient_UpdateIsViewed(GuidId);

                    if (MainClass.dbType != PriemType.Priem)
                    {
                        chbHasEssay.Checked = false;
                        chbHasMotivationLetter.Checked = false;

                        GetHasMotivationLetter();
                        GetHasEssay();
                    }

                    GetHasInnerPriorities(context);

                    if (InnerEntryInEntryId.HasValue)
                        InnerEntryInEntryId = abit.InnerEntryInEntryId;

                    FillSelectedExams(context);
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка при заполнении формы ", ex);
            }
        }

        private void UpdateEntryData(Guid _EntryId)
        {
            using (PriemEntities context = new PriemEntities())
            {
                UpdateEntryData(context, _EntryId);
            }
        }
        private void UpdateEntryData(PriemEntities context, Guid _EntryId)
        {
            extEntry ent = context.extEntry.Where(x => x.Id == _EntryId).FirstOrDefault();
            if (ent != null)
            {
                tbLicenseProgram.Text = string.Format("({0}) {1}", ent.LicenseProgramCode, ent.LicenseProgramName);
                tbObrazProgram.Text = string.Format("({0}) {1}", ent.ObrazProgramCrypt, ent.ObrazProgramName);
                tbFaculty.Text = ent.FacultyName;
                tbProfile.Text = ent.ProfileName;
                tbStudyForm.Text = ent.StudyFormName;
                tbStudyBasis.Text = ent.StudyBasisName;

                LicenseProgramId = ent.LicenseProgramId;
                ObrazProgramId = ent.ObrazProgramId;
                ProfileId = ent.ProfileId;
                FacultyId = ent.FacultyId;
                StudyFormId = ent.StudyFormId;
                StudyBasisId = ent.StudyBasisId;

                chbIsListener.Visible = StudyBasisId == 2;
                chbIsPaid.Visible = StudyBasisId == 2;

                IsForeign = ent.IsForeign;
                IsParallel = ent.IsParallel;
                IsReduced = ent.IsReduced;
                IsSecond = ent.IsSecond;

                FillCompetition(context);

                this.EntryId = _EntryId;
            }
        }

        private void GetWhoSetHasOriginals()
        {
            BackgroundWorker bw_whoSetOriginals = new BackgroundWorker();
            bw_whoSetOriginals.DoWork += (sender, e) => {
                Guid _Id = (Guid)e.Argument;
                using (PriemEntities context = new PriemEntities())
                {
                    e.Result = context.qAbiturient_WhoSetHasOriginals.Where(x => x.Id == _Id).FirstOrDefault();
                }
            };
            bw_whoSetOriginals.RunWorkerCompleted += (sender, e) =>
            {
                dynamic _who = e.Result;
                if (_who == null)
                    return;

                string who = "", whoFac = "", whoDate = "";
                try
                {
                    who = _who.UserId;
                    whoFac = _who.FacultyName;
                    whoDate = _who.ActionTime.ToShortDateString() + " " + _who.ActionTime.ToShortTimeString();
                    who = MainClass.GetADUserName(who);
                }
                catch { lblHasOriginalsUser.Visible = false; }

                if (!string.IsNullOrEmpty(who))
                {
                    lblHasOriginalsUser.Text = "Проставлено: " + who + " (" + whoDate + " " + whoFac + ")";
                    lblHasOriginalsUser.Visible = true;
                }
            };

            bw_whoSetOriginals.RunWorkerAsync(GuidId);
        }

        private void GetWhoSetBackDoc()
        {
            BackgroundWorker bw_whoBackDoc = new BackgroundWorker();
            bw_whoBackDoc.DoWork += bw_whoBackDoc_DoWork;
            bw_whoBackDoc.RunWorkerCompleted += bw_whoBackDoc_RunWorkerCompleted;
            bw_whoBackDoc.RunWorkerAsync(GuidId);
        }

        void bw_whoBackDoc_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument == null)
            {
                e.Result = new qAbiturient_WhoSetBackDoc();
                return;
            }

            try
            {
                Guid _Id = (Guid)e.Argument;
                using (PriemEntities context = new PriemEntities())
                {
                    e.Result = context.qAbiturient_WhoSetBackDoc.Where(x => x.Id == GuidId).FirstOrDefault();
                }
            }
            catch
            {
                e.Result = new qAbiturient_WhoSetBackDoc();
            }
        }
        void bw_whoBackDoc_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dynamic _who = e.Result;
            if (_who == null)
                return;

            string who = "", whoFac = "", whoDate = "";
            try
            {
                who = _who.UserId;
                whoFac = _who.FacultyName;
                whoDate = _who.ActionTime.ToShortDateString() + " " + _who.ActionTime.ToShortTimeString();
                who = MainClass.GetADUserName(who);
            }
            catch { lblWhoBackDoc.Visible = false; }

            if (!string.IsNullOrEmpty(who))
            {
                lblWhoBackDoc.Text = "Проставлен отказ: " + who + " (" + whoDate + " " + whoFac + ")";
                lblWhoBackDoc.Visible = true;
            }
        }

        // если подал подлинники на одно заявления - то писать об этом
        private void FillLockOrigin(PriemEntities context)
        {
            string queryForLock = string.Format(@"SELECT Case when Exists 
                (SELECT Id FROM ed.Abiturient 
                WHERE ed.Abiturient.PersonId = '{0}' 
                {1} 
                AND ed.Abiturient.HasOriginals > 0 AND ed.Abiturient.BackDoc = 0) then 'true' else 'false' end ", _personId.ToString(), _Id == null ? "" : string.Format(" AND ed.Abiturient.Id <> '{0}'", _Id));
            lockHasOrigin = bool.Parse(context.Database.SqlQuery<string>(queryForLock).FirstOrDefault());

            if (lockHasOrigin)
            {
                lblOtherOriginals.Visible = true;
                chbHasOriginals.Enabled = false;
            }
            else
            {
                lblOtherOriginals.Visible = false;
                chbHasOriginals.Enabled = true;
            }
        }

        private async void GetHasMotivationLetter()
        {
            BDClassLib.SQLClass InetDB = new BDClassLib.SQLClass();
            InetDB.OpenDatabase(MainClass.connStringOnline);

            //ищем среди файлов, приложенных к конкретному конкурсу (т.е. по совпадению ApplicationBarcode) и из общих файлов (где есть PersonBarcode, а ApplicationBarcode NULL)
            string query = "SELECT COUNT(*) FROM qAbitFiles_OnlyEssayMotivLetter WHERE (ApplicationBarcode=@ApplicationBarcode OR (PersonBarcode=@PersonBarcode AND ApplicationBarcode IS NULL)) AND FileTypeId=2";
            int cnt = await Task.Run(() =>
            {
                return (int)InetDB.GetValue(query, new SortedList<string, object>()
                {
                    { "@ApplicationBarcode", QueryServ.ToNullDB(abitBarcode) },
                    { "@PersonBarcode", QueryServ.ToNullDB(persBarcode) }
                });
            });

            chbHasMotivationLetter.Checked = cnt > 0;
            //return new Task<bool>();
        }
        private async void GetHasEssay()
        {
            BDClassLib.SQLClass InetDB = new BDClassLib.SQLClass();
            InetDB.OpenDatabase(MainClass.connStringOnline);
            //ищем среди файлов, приложенных к конкретному конкурсу (т.е. по совпадению ApplicationBarcode) и из общих файлов (где есть PersonBarcode, а ApplicationBarcode NULL)
            string query = "SELECT COUNT(*) FROM qAbitFiles_OnlyEssayMotivLetter WHERE (ApplicationBarcode=@ApplicationBarcode OR (PersonBarcode=@PersonBarcode AND ApplicationBarcode IS NULL)) AND FileTypeId=3";
            int cnt = await Task.Run(() =>
            {
                return (int)InetDB.GetValue(query, new SortedList<string, object>()
                {
                    { "@ApplicationBarcode", QueryServ.ToNullDB(abitBarcode) },
                    { "@PersonBarcode", QueryServ.ToNullDB(persBarcode) }
                });
            });

            chbHasEssay.Checked = (cnt > 0);
        }

        // возвращает, есть ли человек в протоколе о допуске
        private bool GetInEnableProtocol(PriemEntities context)
        {
            int cntProt = (from ph in context.extProtocol
                           where ph.ProtocolTypeId == 1 && !ph.IsOld && !ph.Excluded && ph.AbiturientId == GuidId
                           select ph).Count();

            if (cntProt > 0)
                return true;
            else
                return false;
        }
        // возвращает, есть ли человек в представлении к зачислению
        private bool GetInEntryView(PriemEntities context)
        {
            int cntProt = (from ph in context.extEntryView
                           where ph.AbiturientId == GuidId
                           select ph).Count();

            if (cntProt > 0)
            {
                //if (!tabCard.TabPages.Contains(tpEntry))
                //   tabCard.TabPages.Add(tpEntry);
                return true;
            }
            else
            {
                //if (tabCard.TabPages.Contains(tpEntry))
                //    tabCard.TabPages.Remove(tpEntry);
                return false;
            }
        }

        private void FillProtocols(PriemEntities context)
        {
            try
            {
                tbEnabledProtocol.Text = Util.ToStr((from ph in context.extProtocol
                                                     where ph.ProtocolTypeId == 1 && !ph.IsOld && !ph.Excluded && ph.AbiturientId == GuidId
                                                     select ph.Number).FirstOrDefault());

                tbEntryProtocol.Text = Util.ToStr((from ph in context.extEntryView
                                                   where ph.AbiturientId == GuidId
                                                   select ph.Number).FirstOrDefault());
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Данное заявление находится одновременно в нескольких протоколах \n ", exc);
            }
        }

        #region ReadOnly

        protected override void SetReadOnlyFieldsAfterFill()
        {
            base.SetReadOnlyFieldsAfterFill();
        }
        protected override void SetAllFieldsNotEnabled()
        {
            base.SetAllFieldsNotEnabled();

            btnCardPerson.Enabled = true;

            if (StudyBasisId == 2)
                btnPaidData.Enabled = true;
            gbPrint.Enabled = true;
            btnPrint.Enabled = true;

            btnDocs.Enabled = true;

            if (MainClass.IsPasha())
            {
                dgvExams.Enabled = btnDeleteMark.Enabled = true;
                btnAddFix.Enabled = btnAddFix.Visible = true;
                btnAddGreen.Enabled = btnAddGreen.Visible = true;
            }

            btnDocInventory.Enabled = true;

            if (MainClass.IsFacMain() || MainClass.IsPasha())
                btnChangeOriginalsDestination.Enabled = true;
            else
                btnChangeOriginalsDestination.Enabled = false;

            gbAdditionalAchievements.Enabled = true;
        }
        protected override void SetAllFieldsEnabled()
        {
            base.SetAllFieldsEnabled();

            tbPriority.Enabled = true;
            tbRegNum.Enabled = false;
            if (StudyBasisId == 2)
                btnPaidData.Enabled = true;

            btnDocs.Enabled = true;

            tbEnabledProtocol.Enabled = false;
            tbEntryProtocol.Enabled = false;
            dtDocInsertDate.Enabled = false;
            tbSum.Enabled = false;
            btnDeleteMark.Enabled = false;

            btnDocInventory.Enabled = true;
            if (StudyBasisId == 1 && MainClass.dbType == PriemType.Priem)
                btnChangePriority.Enabled = true;

            tpExamBlock.Enabled = true;
            dgvAppExams.Enabled = true;
        }
        // закрытие части полей в зависимости от прав
        protected override void SetReadOnlyFields()
        {
            SetAllFieldsNotEnabled();

            if (!chbBackDoc.Checked)
                chbBackDoc.Enabled = true;

            if (StudyBasisId == 1 && !MainClass.IsReadOnly())
                btnChangePriority.Enabled = true;

            chbIsPaid.Enabled = true;
            cbLanguage.Enabled = true;
            chbIsForeign.Enabled = true;
            cbCelCompetition.Enabled = true;
            tbCelCompetitionText.Enabled = true;
            chbIsListener.Enabled = true;
            if (!chbHasOriginals.Checked)
                chbHasOriginals.Enabled = true;
            tbPriority.Enabled = true;
            tbCoefficient.Enabled = true;

            tpExamBlock.Enabled = true;
            dgvAppExams.Enabled = true;

            btnClose.Enabled = true;
            btnSaveChange.Enabled = true;
            btnObrazProgramInEntry.Enabled = true;

            WinFormsServ.SetSubControlsEnabled(tpEntry, true);

            if (MainClass.IsFacMain())
            {
                if (chbBackDoc.Checked)
                    chbBackDoc.Enabled = false;
                if (chbHasOriginals.Checked)
                    chbHasOriginals.Enabled = true;

                btnChangeEntry.Enabled = true;

                cbCompetition.Enabled = true;
                cbOtherCompetition.Enabled = true;
                chbChecked.Enabled = true;
                chbNotEnabled.Enabled = true;

                if (CompetitionId == 1 || CompetitionId == 2 || CompetitionId == 7 || CompetitionId == 8)
                    chbChecked.Enabled = false;

                btnAddAbiturientAchievement.Enabled = true;
                btnDeleteAbiturientAchievement.Enabled = true;
            }

            if (MainClass.RightsSov_SovMain())
            {
                cbCompetition.Enabled = true;
                cbOtherCompetition.Enabled = true;
                chbChecked.Enabled = true;
                chbNotEnabled.Enabled = true;

                btnChangeEntry.Enabled = true;

                if (chbBackDoc.Checked)
                    chbBackDoc.Enabled = true;

                //уточнить, кто может снимать эту галочку! 
                if (chbHasOriginals.Checked)
                    chbHasOriginals.Enabled = true;
            }

            if (MainClass.IsPasha())
            {
                cbCompetition.Enabled = true;
                chbHasOriginals.Enabled = true;
                chbBackDoc.Enabled = true;
                btnAddAbiturientAchievement.Enabled = true;
                btnDeleteAbiturientAchievement.Enabled = true;
            }

            if (inEnableProtocol)
            {
                chbChecked.Enabled = false;
                chbNotEnabled.Enabled = false;
                dtDocDate.Enabled = false;
                cbCompetition.Enabled = false;

                btnChangeEntry.Enabled = false;
            }

            //всем остальным нельзя изменять конкурс
            btnChangeEntry.Enabled = false;

            if (!inEnableProtocol)
            {
                if (MainClass.IsFacMain() || MainClass.IsPasha())
                    btnChangeEntry.Enabled = true;
            }
            if (inEntryView)
            {
                tbCoefficient.Enabled = false;

                chbChecked.Enabled = false;
                chbNotEnabled.Enabled = false;
                dtDocDate.Enabled = false;
                cbCompetition.Enabled = false;

                chbHasOriginals.Enabled = false;
                chbBackDoc.Enabled = false;
                dtBackDocDate.Enabled = false;

                btnChangeEntry.Enabled = false;

                cbLanguage.Enabled = false;
                btnAddAbiturientAchievement.Enabled = false;
                btnDeleteAbiturientAchievement.Enabled = false;
            }
            else
            {
                if (MainClass.IsFacMain() || MainClass.IsPasha())
                {
                    btnAddAbiturientAchievement.Enabled = true;
                    btnDeleteAbiturientAchievement.Enabled = true;

                    gbEntryConfirm.Enabled = true;

                    chbHasEntryConfirm.Enabled = true;
                    dtpDateEntryConfirm.Enabled = true;

                    if (chbHasEntryConfirm.Checked)
                    {
                        chbHasEntryConfirm.Enabled = false;
                        dtpDateEntryConfirm.Enabled = false;

                        chbHasDisabledEntryConfirm.Enabled = true;
                        dtpDateDisableEntryConfirm.Enabled = true;

                        if (chbHasDisabledEntryConfirm.Checked)
                        {
                            chbHasDisabledEntryConfirm.Enabled = false;
                            dtpDateDisableEntryConfirm.Enabled = false;
                        }
                    }
                }
            }

            if (MainClass.IsPasha())
                dtDocDate.Enabled = true;

            if (lockHasOrigin)
                if (!chbHasOriginals.Checked)
                    chbHasOriginals.Enabled = false;
        }

        #endregion

        #region Handlers

        //инициализация обработчиков мегакомбов
        protected override void InitHandlers()
        {
            chbHasOriginals.CheckedChanged += chbHasOriginals_CheckedChanged;
        }

        void cbCompetition_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAfterCompetition();
        }
        private void UpdateAfterCompetition()
        {
            if (CompetitionId == 1 || CompetitionId == 2 || CompetitionId == 7 || CompetitionId == 8)
            {
                chbChecked.Checked = false;
                chbChecked.Enabled = false;
            }

            cbBenefitOlympSource.Visible = (CompetitionId == 1 || CompetitionId == 8);
            cbBenefitOlympSource.Enabled = (CompetitionId == 1 || CompetitionId == 8);

            if (CompetitionId == 6)
            {
                if (_Id != null)
                    chbChecked.Enabled = true;

                cbOtherCompetition.SelectedIndex = 0;
                cbCelCompetition.SelectedIndex = 0;
                lblCompetitionAddInfo.Visible = true;
                cbOtherCompetition.Visible = true;

                lblCelCompetition.Visible = true;
                cbCelCompetition.Visible = true;
                tbCelCompetitionText.Visible = true;
            }
            else
            {
                if (_Id != null)
                    chbChecked.Enabled = true;

                cbOtherCompetition.SelectedIndex = 0;
                lblCompetitionAddInfo.Visible = false;
                cbOtherCompetition.Visible = false;
                tbCelCompetitionText.Text = "";
                tbCelCompetitionText.Visible = false;

                lblCelCompetition.Visible = false;
                cbCelCompetition.Visible = false;
                cbCelCompetition.SelectedIndex = 0;
            }
        }

        private void FillCompetition()
        {
            using (PriemEntities context = new PriemEntities())
            {
                FillCompetition(context);
            }
        }
        private void FillCompetition(PriemEntities context)
        {
            try
            {
                var compList = ((from cp in context.Competition
                                 where cp.StudyBasisId == StudyBasisId
                                 orderby cp.Name
                                 select new
                                 {
                                     cp.Id,
                                     cp.Name
                                 }).Distinct()).ToList();

                var lst = compList.Where(cp => cp.Id < 6 || cp.Id == 9).Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();
                ComboServ.FillCombo(cbOtherCompetition, lst, true, false);

                lst = compList.Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();
                ComboServ.FillCombo(cbCompetition, lst, false, false);

                if (StudyBasisId == 1)
                {
                    chbIsListener.Checked = false;
                    chbIsListener.Enabled = false;
                    chbIsPaid.Checked = false;
                    chbIsPaid.Enabled = false;
                    btnPaidData.Enabled = false;
                    ComboServ.SetComboId(cbCompetition, 4);
                }
                else
                {
                    chbIsListener.Enabled = true;
                    chbIsPaid.Enabled = true;
                    btnPaidData.Enabled = true;
                    ComboServ.SetComboId(cbCompetition, 3);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы FillCompetition", exc);
            }
        }

        private void FillSelectedExams()
        {
            using (PriemEntities context = new PriemEntities())
            {
                FillSelectedExams(context);
            }
        }
        private void FillSelectedExams(PriemEntities context)
        {
            try
            {
                var block = (from b in context.ExamInEntryBlock
                             join u in context.ExamInEntryBlockUnit on b.Id equals u.ExamInEntryBlockId
                             join ex in context.Exam on u.ExamId equals ex.Id
                             join exname in context.ExamName on ex.ExamNameId equals exname.Id
                             where b.EntryId == EntryId && b.ParentExamInEntryBlockId == null
                             select new { b.Id, b.Name, unitId = u.Id, unitname = exname.Name, }).ToList();

                var _block = (from b in block
                              group b by b.Id into ex
                              where ex.Count() > 1
                              select new
                              {
                                  Id = ex.Key,
                                  Name = ex.Select(x => x.Name).FirstOrDefault(),
                              }).ToList();

                lstExamInEntryBlock = new List<ExamenBlock>();
                HasManualExams = _block.Count != 0;

                if (!HasManualExams)
                {
                    if (tabCard.TabPages.Contains(tpExamBlock))
                        tabCard.TabPages.Remove(tpExamBlock);
                    return;
                }

                if (!tabCard.TabPages.Contains(tpExamBlock))
                    tabCard.TabPages.Add(tpExamBlock);


                dgvAppExams.Columns.Add("Id", "Id");
                dgvAppExams.Columns["Id"].Visible = false;
                dgvAppExams.Columns["Id"].CellTemplate = new DataGridViewTextBoxCell();

                dgvAppExams.Columns.Add("Name", "Название");
                dgvAppExams.Columns["Name"].CellTemplate = new DataGridViewTextBoxCell();


                DataGridViewComboBoxColumn column1 = new DataGridViewComboBoxColumn();
                DataGridViewComboBoxCell cell1 = new DataGridViewComboBoxCell();
                cell1.DisplayMember = "Value";
                cell1.ValueMember = "Key";
                column1.HeaderText = "Список экзаменов";
                column1.Name = "ExamsList";
                column1.CellTemplate = cell1;
                column1.Width = 250;
                dgvAppExams.Columns.Add(column1);

                foreach (var b in _block)
                {
                    var lst = (from bl in block
                               where bl.Id == b.Id
                               select new KeyValuePair<Guid, string>(bl.unitId, bl.unitname)).ToList();

                    Guid SelectedUnitId =
                        (from abit in context.AbiturientSelectedExam
                         join u in context.ExamInEntryBlockUnit on abit.ExamInEntryBlockUnitId equals u.Id
                         where abit.ApplicationId == GuidId 
                         && u.ExamInEntryBlockId == b.Id
                         select abit.ExamInEntryBlockUnitId).FirstOrDefault();

                    lstExamInEntryBlock.Add(new ExamenBlock()
                    {
                        BlockId = b.Id,
                        BlockName = b.Name,
                        UnitList = lst,
                        SelectedUnitId = SelectedUnitId,
                    });

                    dgvAppExams.CellValueChanged -= dgvAppExams_CellValueChanged;

                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dgvAppExams);
                    int icol = dgvAppExams.Columns.IndexOf(dgvAppExams.Columns["Id"]);
                    row.Cells[icol].Value = b.Id;
                    icol = dgvAppExams.Columns.IndexOf(dgvAppExams.Columns["Name"]);
                    row.Cells[icol].Value = b.Name;
                    icol = dgvAppExams.Columns.IndexOf(dgvAppExams.Columns["ExamsList"]);

                    DataGridViewComboBoxCell comboCell = row.Cells[icol] as DataGridViewComboBoxCell;
                    comboCell.DataSource = lst;
                    if (SelectedUnitId != Guid.Empty)
                        row.Cells[icol].Value = SelectedUnitId;

                    dgvAppExams.Rows.Add(row);
                    dgvAppExams.CellValueChanged += dgvAppExams_CellValueChanged;
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации FillSelectedExams", exc);
            }
        }

        private void UpdateBenefitOlympSource()
        {
            using (PriemEntities context = new PriemEntities())
            {
                UpdateBenefitOlympSource(context);
            }
        }
        private void UpdateBenefitOlympSource(PriemEntities context)
        {
            if (!GuidId.HasValue)
                return;

            bool bIsAG = MainClass.dbType == PriemType.PriemAG;
            List<int> lstOlympLevels = new List<int> { 1, 2, 3, 4 };

            List<KeyValuePair<string, string>> lst =
                    ((from Ol in context.extOlympiadsAll
                      where Ol.PersonId == _personId
                      && (bIsAG ? true : lstOlympLevels.Contains(Ol.OlympTypeId))
                      select new
                      {
                          Id = Ol.Id,
                          Ol.OlympName,
                          Ol.OlympSubjectName,
                          Ol.OlympValueName,
                          Ol.OlympYear
                      }).Distinct()).ToList()
                      .Select(u => new KeyValuePair<string, string>(u.Id.ToString(), string.Format("({0}) {1} ({2}) - {3}", u.OlympYear, u.OlympName, u.OlympSubjectName, u.OlympValueName)))
                      .ToList();

            ComboServ.FillCombo(cbBenefitOlympSource, lst, true, false);
        }

        // строка с ФИО если поменялись данные личной карточки
        private void UpdateFIO()
        {
            using (PriemEntities context = new PriemEntities())
            {
                UpdateFIO(context);
            }
        }
        private void UpdateFIO(PriemEntities context)
        {
            try
            {
                if (_personId == null)
                    lblFIO.Text = string.Empty;
                else
                {

                    lblFIO.Text = (from per in context.extPersonAll
                                   where per.Id == _personId
                                   select per.FIO).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка при обращении к базе", ex);
            }
        }

        private void chbHasOriginals_CheckedChanged(object sender, EventArgs e)
        {
            if (_isModified)
            {
                if (chbHasOriginals.Checked)
                {
                    if (MessageBox.Show("Я подтверждаю что все документы подлинные", "Внимание!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        //chbEgeDocOriginal.Checked = true;
                        chbHasOriginals.ForeColor = System.Drawing.Color.Black;
                    }
                    else
                    {
                        chbHasOriginals.Checked = false;
                    }
                }
                else
                {
                    chbHasOriginals.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
        private void chbBackDoc_CheckedChanged(object sender, EventArgs e)
        {
            if (_isModified)
            {
                if (chbBackDoc.Checked)
                {
                    if (HasOriginals)
                    {
                        if (MessageBox.Show(@"На данном конкурсе лежат подлинники. Хотите переложить их на другой конкурс?
В случае забора документов из карточки с подлинниками абитуриент считается не подавшим оригиналы", "Внимание!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            OpenCardChangeOriginalsDestination();
                            return;
                        }
                    }

                    if (MessageBox.Show(string.Format("Вы уверены, что абитуриент отказался от участия в конкурсе на образовательную программу \"{0}\", форму обучения \"{1}\", основу обучения \"{2}\"?????", 
                        tbObrazProgram.Text, tbStudyForm.Text, tbStudyBasis.Text), "Внимание!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        chbBackDoc.ForeColor = System.Drawing.Color.Red;
                        dtBackDocDate.Enabled = true;
                        chbHasOriginals.Checked = false;
                    }
                    else
                    {
                        chbBackDoc.Checked = false;
                    }
                }
                else
                {
                    chbBackDoc.ForeColor = System.Drawing.Color.Black;
                    dtBackDocDate.Enabled = false;
                }
            }
        }
        private void chbChecked_CheckedChanged(object sender, EventArgs e)
        {
            if (chbChecked.Checked)
                chbChecked.ForeColor = System.Drawing.Color.Black;
            else
                chbChecked.ForeColor = System.Drawing.Color.Red;
        }
        private void chbEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (chbNotEnabled.Checked)
                chbNotEnabled.ForeColor = System.Drawing.Color.Red;
            else
                chbNotEnabled.ForeColor = System.Drawing.Color.Black;
        }

        #endregion

        #region Save

        protected override bool CheckFields()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    if (LicenseProgramId == null || ObrazProgramId == null || FacultyId == null || StudyFormId == null || StudyBasisId == null)
                    {
                        epErrorInput.SetError(tbLicenseProgram, "Прием документов на данную программу не осуществляется!");
                        tabCard.SelectedIndex = 0;
                        return false;
                    }
                    else
                        epErrorInput.Clear();

                    if (EntryId == null)
                    {
                        epErrorInput.SetError(tbLicenseProgram, "Прием документов на данную программу не осуществляется!");
                        tabCard.SelectedIndex = 0;
                        return false;
                    }
                    else
                        epErrorInput.Clear();


                    if (_Id == null)
                    {
                        if (!CheckIsClosed(context))
                        {
                            epErrorInput.SetError(tbLicenseProgram, "Прием документов на данную программу закрыт!");
                            tabCard.SelectedIndex = 0;
                            return false;
                        }
                        else
                            epErrorInput.Clear();
                    }

                    if (EntryId == Guid.Empty)
                    {
                        WinFormsServ.Error("Не найдено конкурсной группы по указанным данным!");
                        return false;
                    }

                    List<int> lstCompetitionIdsForOriginals = context.Competition.Where(x => x.NeedOriginals).Select(x => x.Id).ToList();
                    var OrigApps = context.Abiturient.Where(x => x.PersonId == _personId && x.HasOriginals && !x.BackDoc);
                    bool HasOrigs = OrigApps.Count() > 0;
                    if (!HasOriginals && (CompetitionId.HasValue && lstCompetitionIdsForOriginals.Contains(CompetitionId.Value)) && !HasOrigs && !BackDoc && MainClass.dbType != PriemType.PriemAG)
                    {
                        WinFormsServ.Error("Для данного типа конкурса требуется обязательная подача оригиналов документов об образовании");
                        return false;
                    }

                    if (MainClass.dbType == PriemType.Priem && (CompetitionId == 1 || CompetitionId == 8))
                    {
                        if (!GuidId.HasValue)
                        {
                            WinFormsServ.Error("Сперва сохраните карточку!");
                            return false;
                        }

                        if (!OlympiadId.HasValue)
                        {
                            WinFormsServ.Error("Требуется указание основания для предоставления общей льготы");
                            return false;
                        }
                        else
                        {
                            var Ol = context.Olympiads.Where(x => x.Id == OlympiadId).FirstOrDefault();
                            if (Ol == null)
                            {
                                WinFormsServ.Error("Не найдена олимпиада!");
                                return false;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(Ol.DocumentNumber))
                                {
                                    WinFormsServ.Error("У олимпиады не указан номер подтверждающего документа!");
                                    return false;
                                }

                                List<int?> lstEx = context.OlympSubjectToExam.Where(x => x.OlympSubjectId == Ol.OlympSubjectId).Select(x => (int?)x.ExamId).ToList();
                                bool bNoExamId = lstEx.Where(x => x != 0).Count() == 0;
                                var lstBenefits = context.OlympResultToCommonBenefit
                                    .Where(x => x.EntryId == EntryId
                                        && (bNoExamId ? true : (lstEx.Contains(x.ExamId) || x.ExamId == null))
                                        && (x.OlympLevelId == Ol.OlympLevelId || Ol.OlympLevelId == 0) 
                                        && (x.OlympSubjectId == null ? true : x.OlympSubjectId == Ol.OlympSubjectId)
                                        && (x.OlympProfileId == null ? true : x.OlympProfileId == Ol.OlympProfileId) 
                                        && x.OlympValueId == Ol.OlympValueId).ToList();
                                int iCntBenefits = lstBenefits.Count();
                                if (iCntBenefits == 0)
                                {
                                    WinFormsServ.Error("Не найдено общей льготы для данной олимпиады в указанном конкурсе!");
                                    return false;
                                }
                                else if (Ol.OlympTypeId > 2)
                                {
                                    if (bNoExamId)
                                        lstEx = lstBenefits.Where(x => x.ExamId.HasValue).Select(x => x.ExamId).ToList();

                                    bNoExamId = lstEx.Where(x => x != 0).Count() == 0;
                                    if (bNoExamId)
                                    {
                                        WinFormsServ.Error("Не удаётся найти в базе предмета ЕГЭ для указания общей льготы!");
                                        return false;
                                    }

                                    decimal egeMin = lstBenefits.First().MinEge ?? 0;

                                    //проверяем мин. баллы
                                    var balls = from ege in context.extEgeMarkMaxAbitApproved
                                                join eee in context.EgeToExam on ege.EgeExamNameId equals eee.EgeExamNameId
                                                where ege.AbiturientId == GuidId && lstEx.Contains(eee.ExamId) && ege.Value >= egeMin
                                                select ege;

                                    if (balls.Count() == 0)
                                    {
                                        //проверяем прочие баллы в карточке
                                        var abitMarks = context.qMark.Where(x => x.AbiturientId == GuidId && lstEx.Contains(x.ExamId) && x.Value >= egeMin);
                                        if (abitMarks.Count() == 0)
                                        {
                                            WinFormsServ.Error("Не найдено подтверждающих баллов для общей льготы!");
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!CheckIdent(context))
                    {
                        WinFormsServ.Error("У абитуриента уже существует заявление на данный факультет, направление, профиль, форму и основу обучения!");
                        return false;
                    }

                    if (!CheckThreeAbits(context))
                    {
                        WinFormsServ.Error("Абитуриент уже участвует в конкурсах на 3 различных направлениях!");
                        return false;
                    }

                    if (MainClass.dbType == PriemType.Priem)
                        CheckTwoConfirms(context);

                    if (DocDate > DateTime.Now)
                    {
                        epErrorInput.SetError(dtDocDate, "Неправильная дата");
                        tabCard.SelectedIndex = 1;
                        return false;
                    }
                    else
                        epErrorInput.Clear();

                    //if (cbCompetition.Id != CheckCompetition(context))
                    //{
                    //    DialogResult res = MessageBox.Show(string.Format("Тип конкурса не соответствует льготам абитуриента? Установить тип конкурса {0}?", _bdc.GetStringValue(string.Format("SELECT Competition.Name FROM Competition WHERE Competition.Id = {0}", CheckCompetition()))), "Предупреждение", MessageBoxButtons.YesNoCancel);
                    //    if (res == DialogResult.Yes)
                    //        cbCompetition.SetItem(CheckCompetition());
                    //}

                    if (Priority.HasValue)
                    {
                        if (!BackDoc)
                        {
                            int priorcnt = context.Abiturient.Where(x => x.PersonId == _personId && !x.BackDoc && x.Entry.IsForeign == IsForeign && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId) && x.Priority == Priority && (GuidId.HasValue ? x.Id != GuidId.Value : true)).Count();
                            if (priorcnt > 0)
                            {
                                epErrorInput.SetError(tbPriority, "У абитуриента уже имеется заявление с заданным приоритетом");
                                tabCard.SelectedIndex = 0;
                                return false;
                            }
                            else
                                epErrorInput.Clear();
                        }
                    }
                    else
                    {
                        if (!BackDoc)
                        {
                            if (StudyBasisId.HasValue && StudyBasisId == 1)
                            {
                                epErrorInput.SetError(tbPriority, "Укажите приоритет");
                                tabCard.SelectedIndex = 0;
                                return false;
                            }
                        }
                    }

                    if (CompetitionId == 2)
                    {
                        if (!CheckCountCompetitionEqual2(context))
                        {
                            MessageBox.Show("У абитуриента уже есть конкурс 'в/к'","Ошибка при изменении типа конкурса",  MessageBoxButtons.OK);
                            return false; 
                        }
                        string s = ApplicationDataProvider.CheckPersonPriveleges(_personId);
                        if (!String.IsNullOrEmpty(s))
                        {
                            MessageBox.Show( s,"Ошибка при изменении типа конкурса", MessageBoxButtons.OK);
                            return false;
                        }
                    }
                    int cnt = context.ExamInEntryBlockUnit.Where(x => x.ExamInEntryBlock.EntryId == EntryId).Count();
                    if (lstExamInEntryBlock != null && cnt > 0)
                    {
                        foreach (var x in lstExamInEntryBlock)
                        {
                            if (x.SelectedUnitId == Guid.Empty)
                            {
                                MessageBox.Show("Не указан(ы) экзамены по выбору", "", MessageBoxButtons.OK);
                                break;
                            }
                        }
                    }

                    if (HasOriginals && _personId.HasValue)
                    {
                        int cnt_origs = context.Abiturient.Where(x => x.PersonId == _personId.Value && x.HasOriginals == true && x.BackDoc == false && x.Id != GuidId).Count();
                        if (cnt_origs > 0)
                        {
                            epError.SetError(chbHasOriginals, "!");
                            WinFormsServ.Error("Оригиналы документов уже находятся на другом конкурсе");
                            return false;
                        }
                    }

                    return true;
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при CheckFields", exc);
                return false;
            }
        }
        private bool CheckCountCompetitionEqual2(PriemEntities context)
        {
            int cnt = context.Abiturient.Where(x => x.PersonId == _personId && x.CompetitionId == 2 && (GuidId.HasValue ? x.Id != GuidId.Value : true) && !x.BackDoc).Count();
            if (cnt > 0)
                return false;
            else return true;
        }

        private bool CheckIsClosed(PriemEntities context)
        {
            bool isClosed = (from ent in context.Entry
                             where ent.Id == EntryId
                             select ent.IsClosed || ent.DateOfClose < DateTime.Now).FirstOrDefault();
            return !isClosed;
        }
        private string CheckCompetition(PriemEntities context)
        {
            if (StudyBasisId == 2)
                return "3";
            // проверка на олимпиады 
            List<int?> lstOlympsTmp = new List<int?>() { 1, 2, 3 };

            int cntOl = context.Olympiads
                .Where(x => x.AbiturientId == GuidId && (x.OlympLevelId == 1 || (x.OlympLevelId == 2 && lstOlympsTmp.Contains(x.OlympValueId))))
                .Count();
                //.Database.SqlQuery<int>(string.Format("SELECT Count(Olympiads.Id) FROM Olympiads WHERE Olympiads.AbiturientId = '{0}' 
                //AND ((Olympiads.OlympLevelId = 2 AND Olympiads.OlympValueId IN (1,2,3)) OR Olympiads.OlympLevelId = 1)", _Id)).FirstOrDefault();
            if (cntOl > 0)
                return "1";

            //проверка на льготы
            //if (chbRebSir.Checked || chbSir.Checked || chbInv.Checked || chbPSir.Checked || chbVoen.Checked || chbBoev.Checked || chbCher.Checked)
            //    return "2";   
            return "";
        }

        // проверка на уникальность заявления
        private bool CheckIdent(PriemEntities context)
        {
            if (BackDoc)
                return true;

            ObjectParameter boolPar = new ObjectParameter("result", typeof(bool));

            if (_Id == null)
                context.CheckAbitIdentWithGosLine(_personId, EntryId, IsForeign, boolPar);
            else
                context.CheckAbitIdentWithIdAndGosLine(GuidId, _personId, EntryId, IsForeign, boolPar);

            return Convert.ToBoolean(boolPar.Value);
        }

        private bool CheckThreeAbits(PriemEntities context)
        {
            if (!BackDoc)
                return SomeMethodsClass.CheckThreeAbits(context, _personId, LicenseProgramId);
            else
                return true;
        }

        private bool CheckTwoConfirms(PriemEntities context)
        {
            if (HasEntryConfirm && !HasDisabledEntryConfirm)
            {
                var abits =
                    (from Ab in context.Abiturient
                     join ent in context.extEntry on Ab.EntryId equals ent.Id
                     where Ab.PersonId == _personId && Ab.Id != GuidId && Ab.HasEntryConfirm
                     select new
                     {
                         Ab.HasDisabledEntryConfirm,
                         LicenseProgramCode = ent.LicenseProgramCode,
                         LicenseProgramName = ent.LicenseProgramName,
                         ObrazProgramName = ent.ObrazProgramName,
                     }).ToList();

                if (abits.Count == 0)
                    return true;
                else
                {
                    if (abits.Count == 1)
                    {
                        //if HasDisabledEntryConfirm
                        if (abits[0].HasDisabledEntryConfirm)
                            return true;
                        else
                        {
                            string sEntry = "Направление: " + abits[0].LicenseProgramCode + " " + abits[0].LicenseProgramName
                                + "\nОбразовательная программа:" + abits[0].ObrazProgramName;
                            WinFormsServ.Error("У абитуриента уже есть не отозванное согласие на зачисление по конкурсу:\n" + sEntry);
                            return false;
                        }
                    }
                    else
                    {
                        WinFormsServ.Error("У абитуриента уже есть 2 согласия на зачисление. Больше согласий не допускается.");
                        return false;
                    }
                }
            }
            else
                return true;
        }

        protected override void InsertRec(PriemEntities context, ObjectParameter idParam)
        {
            context.Abiturient_Insert(_personId, EntryId, CompetitionId, IsListener, false, IsPaid, BackDoc, BackDocDate, DocDate, DateTime.Now,
                Checked, NotEnabled, Coefficient, OtherCompetitionId, CelCompetitionId, CelCompetitionText, LanguageId, HasOriginals,
                Priority, abitBarcode, null, null, idParam);

            Guid gId = (Guid)idParam.Value;

            //context.Abiturient_UpdateIsCommonRussianCompetition(IsForeign, gId);
            UpdateSelectedExams(context, gId);
            UpdateIsApprovedByCommision(abitBarcode, Checked);
            UpdateEntryConfirm(context, gId);
        }

        protected override void UpdateRec(PriemEntities context, Guid id)
        {
            context.Abiturient_UpdateWithoutEntry(CompetitionId, IsListener, false, IsPaid, BackDoc, BackDocDate, DocDate,
                  Checked, NotEnabled, Coefficient, OtherCompetitionId, CelCompetitionId, CelCompetitionText, LanguageId, HasOriginals,
                  Priority, id);

            //если есть права на изменение конкурса 
            context.Abiturient_UpdateEntry(EntryId, id);

            //context.Abiturient_UpdateIsCommonRussianCompetition(IsForeign, id);

            // set InnerEntryInEntryId
            if (inEntryView)
            {
                if (InnerEntryInEntryId.HasValue)
                    context.Abiturient_UpdateInnerEntryInEntryId(InnerEntryInEntryId, GuidId);
            }
            
            UpdateSelectedExams(context, id);
            UpdateIsApprovedByCommision(abitBarcode, Checked);
            UpdateEntryConfirm(context, id);

            var abit = context.Abiturient.Where(x => x.Id == id).FirstOrDefault();
            if (abit != null)
            {
                abit.OlympiadId = OlympiadId;
                context.SaveChanges();
            }
        }

        protected override void OnSave()
        {
            MainClass.DataRefresh();
        }

        protected override void OnSaveNew()
        {
            UpdateApplications();
            btnPrint.Enabled = true;

            using (PriemEntities context = new PriemEntities())
            {
                string num = (from pr in context.qAbiturient
                              where pr.Id == GuidId
                              select pr.RegNum).FirstOrDefault().ToString();

                string personNum = (from pr in context.extAbit
                                    where pr.Id == GuidId
                                    select pr.PersonNum).FirstOrDefault().ToString();

                tbRegNum.Text = MainClass.GetAbitNum(num, personNum);
            }
        }

        protected void UpdateIsApprovedByCommision(int? Barcode, bool isApproved)
        {
            if (Barcode.HasValue)
            {
                string query = String.Format("update dbo.Application set IsApprovedByComission = {0}, ApproverName= '{2}'  where Barcode = '{1}'", isApproved ? "1" : "0", Barcode, MainClass.GetUserName());
                load.BDCInet.ExecuteQuery(query);
                //MainClass.BdcOnlineReadWrite.ExecuteQuery(query);
            }
        }
        protected void UpdateSelectedExams(PriemEntities context, Guid id)
        {
            var lst = context.AbiturientSelectedExam.Where(x => x.ApplicationId == id).Select(x => x).ToList();

            foreach (var x in lst)
                context.AbiturientSelectedExam.Remove(x);

            if (lstExamInEntryBlock != null)
            {
                foreach (var x in lstExamInEntryBlock)
                {
                    if (x.SelectedUnitId != Guid.Empty)
                    {
                        context.AbiturientSelectedExam.Add(new AbiturientSelectedExam()
                        {
                            ApplicationId = id,
                            ExamInEntryBlockUnitId = x.SelectedUnitId,
                        });
                    }

                }
            }
            context.SaveChanges();
        }

        protected void UpdateEntryConfirm(PriemEntities context, Guid id)
        {
            var abit = context.Abiturient.Where(x => x.Id == id).FirstOrDefault();
            if (abit != null)
            {
                if (HasEntryConfirm)
                {
                    abit.HasEntryConfirm = true;
                    abit.DateEntryConfirm = dtpDateEntryConfirm.Value;
                }
                else
                {
                    abit.HasEntryConfirm = false;
                    abit.DateEntryConfirm = null;
                }

                if (HasDisabledEntryConfirm)
                {
                    abit.HasDisabledEntryConfirm = true;
                    abit.DateDisableEntryConfirm = dtpDateDisableEntryConfirm.Value;
                }
                else
                {
                    abit.HasDisabledEntryConfirm = false;
                    abit.DateDisableEntryConfirm = null;
                }

                context.SaveChanges();
            }
            
        }

        #endregion

        // Грид Экзамены
        private void FillExams()
        {
            using (PriemEntities context = new PriemEntities())
            {
                FillExams(context);
            }
        }
        private void FillExams(PriemEntities context)
        {
            try
            {
                DataTable examTable = new DataTable();

                DataColumn clm;
                clm = new DataColumn();
                clm.ColumnName = "ExamInEntryId";
                clm.DataType = typeof(Guid);
                examTable.Columns.Add(clm);

                clm = new DataColumn();
                clm.ColumnName = "Id";
                clm.DataType = typeof(Guid);
                examTable.Columns.Add(clm);

                clm = new DataColumn();
                clm.ColumnName = "Экзамен";
                examTable.Columns.Add(clm);

                clm = new DataColumn();
                clm.ColumnName = "Оценка";
                examTable.Columns.Add(clm);

                clm = new DataColumn();
                clm.ColumnName = "Примечание";
                examTable.Columns.Add(clm);

                var lstExamInEntry = context.extExamInEntry
                    .Where(x => x.EntryId == EntryId && x.ParentExamInEntryBlockId == null)
                    .Select(x => new { x.Id, x.ExamName, x.OrderNumber })
                    .ToList()
                    .OrderBy(x => x.OrderNumber);

                var lstMarks =
                    (from mrk in context.Mark
                     join ex in context.extExamInEntry on mrk.ExamInEntryBlockUnitId equals ex.Id
                     where mrk.AbiturientId == GuidId
                     select new
                     {
                         mrk.Id,
                         mrk.Value,
                         ex.ExamName,
                         mrk.IsFromEge,
                         mrk.IsFromOlymp,
                         mrk.OlympiadId,
                         mrk.IsManual,
                         mrk.ExamInEntryBlockUnitId,
                         mrk.ExamVedId
                     });

                foreach (var exam in lstExamInEntry)
                {
                    DataRow newRow;
                    newRow = examTable.NewRow();
                    newRow["Экзамен"] = "[" + exam.OrderNumber + "] " + exam.ExamName;
                    newRow["ExamInEntryId"] = exam.Id;

                    var abMark = lstMarks.Where(x => x.ExamInEntryBlockUnitId == exam.Id).FirstOrDefault();
                    if (abMark != null)
                    {
                        newRow["Id"] = abMark.Id;
                        if (abMark != null && abMark.Value.ToString() != "")
                            newRow["Оценка"] = abMark.Value.ToString();
                        if (abMark.IsFromEge)
                            newRow["Примечание"] = "Из ЕГЭ ";
                        else if (abMark.IsFromOlymp)
                        {
                            string OlympName = "";
                            if (abMark.OlympiadId.HasValue)
                            {
                                var Olymp = context.extOlympiads.Where(x => x.Id == abMark.OlympiadId).FirstOrDefault();
                                if (Olymp == null)
                                    OlympName = " (олимпиада не найдена!)";
                                else
                                    OlympName = " (" + Olymp.OlympName + "; " + Olymp.OlympSubjectName + "; " + Olymp.OlympValueName + ";)";
                            }
                            else
                                OlympName = " (олимпиада не указана!)";

                            newRow["Примечание"] = "Олимпиада" + OlympName;
                        }
                        else if (abMark.IsManual)
                            newRow["Примечание"] = "Ручной ввод";
                        else if (abMark.ExamVedId != null && MainClass.IsPasha())
                        {
                            string vedNum = _bdc.GetStringValue(string.Format("SELECT extExamsVed.Number FROM ed.extExamsVed WHERE Id = '{0}'", abMark.ExamVedId.ToString()));
                            newRow["Примечание"] = "Ведомость № " + vedNum;
                        }
                    }
                    else
                    {
                        newRow["Оценка"] = "Ещё нет оценки";
                    }

                    examTable.Rows.Add(newRow);
                }

                DataView dv = new DataView(examTable);
                dv.AllowNew = false;

                dgvExams.DataSource = dv;
                dgvExams.ReadOnly = true;
                dgvExams.Columns["ExamInEntryId"].Visible = false;
                dgvExams.Columns["Id"].Visible = false;
                dgvExams.Update();

            }
            catch (DataException de)
            {
                WinFormsServ.Error("Ошибка при заполнении формы ", de);
            }
        }
        private void FillAdditionalAchievements(PriemEntities context)
        {
            var lstAch = context.extAbitAllAdditionalAchievements
                .Where(x => x.AbiturientId == GuidId && x.AchievementTypeId != null)
                .Select(x => new { x.Id, x.AchievementTypeId, x.AchievementType, x.Mark })
                .ToArray();

            dgvAdditionalAchievements.DataSource = Converter.ConvertToDataTable(lstAch);
            dgvAdditionalAchievements.Columns["Id"].Visible = false;
            dgvAdditionalAchievements.Columns["AchievementTypeId"].Visible = false;
            dgvAdditionalAchievements.Columns["AchievementType"].HeaderText = "ИД";
            dgvAdditionalAchievements.Columns["Mark"].HeaderText = "Балл";

            tbAdditionalAchievementsMark.Text = context.extAbitAdditionalMarksSum
                .Where(x => x.AbiturientId == GuidId)
                .Select(x => x.AdditionalMarksSum ?? 0)
                .DefaultIfEmpty(0).First().ToString();
        }

        // Печать документов
        #region Print

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Guid? AbitId = Guid.Parse(_Id);
            switch (cbPrint.SelectedIndex)
            {
                case 0:
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.FileName = lblFIO.Text + " - Заявление.pdf";
                    sfd.Filter = "ADOBE Pdf files|*.pdf";
                    if (sfd.ShowDialog() == DialogResult.OK)
                        Print.PrintApplication(chbPrint.Checked, sfd.FileName, _personId);
                    break;
                case 1:
                    Print.PrintApplicationAgreement(AbitId, chbPrint.Checked);
                    break;
                case 2:
                    Print.PrintStikerOne(AbitId, chbPrint.Checked);
                    break;
                case 3:
                    //Print.PrintStikerAll(_personId, AbitId, chbPrint.Checked);
                    //break;
                    Print.PrintSprav(AbitId, chbPrint.Checked);
                    break;
                case 4:
                    PrintEntryReviev();
                    break;
                case 5:
                    PrintExamList();
                    break;
                //case 4:
                    
            }
        }

        public void PrintExamList()
        {
            if (MainClass.RightsFacMain())
            {
                Guid? AbitId = Guid.Parse(_Id);

                if (tbEnabledProtocol.Text.Trim() != string.Empty)
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.FileName = lblFIO.Text + " - Экзаменационный лист.pdf";
                    sfd.Filter = "ADOBE Pdf files|*.pdf";
                    if (sfd.ShowDialog() == DialogResult.OK)
                        Print.PrintExamList(AbitId, chbPrint.Checked, sfd.FileName);
                }
                else
                    WinFormsServ.Error("Невозможно создание экзаменационного листа, абитуриент не внесен в протокол о допуске");
            }
        }

        public void PrintEntryReviev()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var ev = context.extEntryView.Where(x => x.AbiturientId == GuidId).FirstOrDefault();
                if (ev == null)
                {
                    MessageBox.Show("Абитуриент не зачислен по данному конкурсу", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    bool bIsRus = context.Person.Where(x => x.Id == _personId && x.NationalityId == 1).Count() > 0;
                    Print.PrintOrderReview(ev.Id, GuidId, bIsRus);
                }
            }
        }

        #endregion

        private void UpdateApplications()
        {
            foreach (Form frmChild in MainClass.mainform.MdiChildren)
            {
                if (frmChild is CardPerson)
                    if (((CardPerson)frmChild).Id.CompareTo(_personId.ToString()) == 0)
                        ((CardPerson)frmChild).FillApplications();
            }
        }

        private void tabCard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.D1)
                this.tabCard.SelectedIndex = 0;
            if (e.Control && e.KeyCode == Keys.D2)
                this.tabCard.SelectedIndex = 1;
            if (e.Control && e.KeyCode == Keys.D3)
                this.tabCard.SelectedIndex = 2;
        }

        private void CardAbit_Click(object sender, EventArgs e)
        {
            this.Activate();
        }

        protected override void OnClosed()
        {
            MainClass.RemoveHandler(_drh);
        }

        private void btnCardPerson_Click(object sender, EventArgs e)
        {
            MainClass.OpenCardPerson(_personId.ToString(), null, -1);
        }

        //данные по оплате
        private void btnPaidData_Click(object sender, EventArgs e)
        {
            CardPaidData pd;

            if (_Id == null)
            {
                if (MessageBox.Show("Данное действие приведет к сохранению записи, продолжить?", "Сохранить", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (SaveClick())
                    {
                        pd = new CardPaidData(GuidId, false);
                        pd.Show();
                    }
                }
            }
            else
            {
                pd = new CardPaidData(GuidId, !_isModified);
                pd.Show();
            }
        }

        protected override void btnNext_Click(object sender, EventArgs e)
        {
            _personId = null;
            base.btnNext_Click(sender, e);
        }
        protected override void btnPrev_Click(object sender, EventArgs e)
        {
            _personId = null;
            base.btnPrev_Click(sender, e);
        }

        private void btnDeleteMark_Click(object sender, EventArgs e)
        {
            if (MainClass.IsPasha() || MainClass.IsOwner())
            {
                if (MessageBox.Show("Удалить выбранную оценку?", "Удаление", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (PriemEntities context = new PriemEntities())
                    {
                        foreach (DataGridViewRow dgvr in dgvExams.SelectedRows)
                        {
                            Guid? markId = new Guid(dgvr.Cells["Id"].Value.ToString());

                            try
                            {
                                context.Mark_Delete(markId);
                            }
                            catch (Exception ex)
                            {
                                WinFormsServ.Error("Ошибка удаления данных", ex);
                                continue;
                            }
                        }

                        FillExams();
                    }
                }
            }
        }

        private void btnAddFix_Click(object sender, EventArgs e)
        {
            if (!MainClass.IsPasha())
                return;

            AddtoFixieren();
        }
        private void AddtoFixieren()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var abit = (from ab in context.extAbit
                            where ab.Id == GuidId
                            select ab).FirstOrDefault();

                context.Fixieren_DELETE(GuidId);

                Guid? fixViewId = (from fv in context.FixierenView
                                   where fv.StudyLevelGroupId == abit.StudyLevelGroupId && fv.IsReduced == abit.IsReduced && fv.IsParallel == abit.IsParallel && fv.IsSecond == abit.IsSecond
                                   && fv.FacultyId == abit.FacultyId && fv.LicenseProgramId == abit.LicenseProgramId
                                   && fv.ObrazProgramId == abit.ObrazProgramId
                                   && fv.ProfileId == abit.ProfileId
                                   && fv.StudyFormId == abit.StudyFormId
                                   && fv.StudyBasisId == abit.StudyBasisId
                                   && fv.IsCel == (abit.CompetitionId == 6)
                                   && fv.IsCrimea == (abit.CompetitionId == 11 || abit.CompetitionId == 12)
                                   && fv.IsQuota == (abit.CompetitionId == 2 || abit.CompetitionId == 7)
                                   select fv.Id).FirstOrDefault();

                int cnt = (from fx in context.Fixieren
                           where fx.AbiturientId == GuidId
                           select fx.Number).Count();

                if (cnt == 0)
                    context.Fixieren_Insert(66666, GuidId, fixViewId);
            }

            MessageBox.Show("DONE!");
        }

        private void btnAddGreen_Click(object sender, EventArgs e)
        {
            if (!MainClass.IsPasha())
                return;

            using (PriemEntities context = new PriemEntities())
            {
                if (context.C_FirstWave.Where(x => x.AbiturientId == GuidId).Count() == 0)
                    context.FirstWave_INSERT(GuidId, 9999);

                context.FirstWaveGreen_DeleteByAbId(GuidId);
                context.FirstWaveGreen_INSERT(GuidId, true);

                MessageBox.Show("DONE!");
            }
        }
        private void btnAddToSite_Click(object sender, EventArgs e)
        {
            if (!MainClass.IsPasha())
                return;

            using (PriemEntities context = new PriemEntities())
            {
                int? fixnum = (from fx in context.Fixieren
                               where fx.AbiturientId == GuidId
                               select fx.Number).FirstOrDefault();

                if (fixnum == null)
                    return;

                context.FirstWave_INSERT(GuidId, fixnum);
                MessageBox.Show("DONE!");
            }
        }

        private void btnDocs_Click(object sender, EventArgs e)
        {
            if (_Id == null)
                return;

            if (abitBarcode == null || abitBarcode == 0)
                return;

            using (PriemEntities context = new PriemEntities())
            {
                int? persBarcode = (from ab in context.extAbit
                                    join pers in context.Person
                                    on ab.PersonId equals pers.Id
                                    where ab.Id == GuidId
                                    select pers.Barcode).FirstOrDefault();

                if (persBarcode == null || persBarcode == 0)
                    return;

                new DocCard(persBarcode.Value, abitBarcode.Value, true, MainClass.dbType == PriemType.PriemForeigners).Show();
            }
        }
        private void btnDocInventory_Click(object sender, EventArgs e)
        {
            new CardDocInventory(GuidId, !_isModified).Show();
        }

        private void btnChangePriority_Click(object sender, EventArgs e)
        {
            if (GuidId.HasValue)
            {
                var crd = new CardChangePriorityInApplication(GuidId.Value);
                crd.OnSave += UpdatePriorityAfterRefresh;
                crd.Show();
            }
        }
        private void UpdatePriorityAfterRefresh(int val)
        {
            tbPriority.Text = val.ToString();
        }

        private void GetHasInnerPriorities(PriemEntities context)
        {
            var OPs = context.InnerEntryInEntry.Where(x => x.EntryId == EntryId).Count();

            if (OPs > 0)
            {
                btnObrazProgramInEntry.Visible = true;
                if (GetInEntryView(context))
                {
                    if (!tabCard.TabPages.Contains(tpEntry))
                    {
                        tabCard.TabPages.Add(tpEntry);
                        FillObrazProgramInEntry(context);
                        WinFormsServ.SetSubControlsEnabled(gbObrazProgramInEntry, true);
                    }
                }
            }
        }

        private void btnObrazProgramInEntry_Click(object sender, EventArgs e)
        {
            if (GuidId.HasValue)
            {
                var crd = new CardApplication_InnerEntryInEntryPriorities(GuidId.Value);
                crd.Show();
            }
        }

        private void FillObrazProgramInEntry(PriemEntities context)
        {
            List<KeyValuePair<string, string>> ObrazProgramInEntryList =
                            (from ent in context.InnerEntryInEntry
                             join SP_ObrazProgr in context.SP_ObrazProgram on ent.ObrazProgramId equals SP_ObrazProgr.Id
                             join SP_Prof in context.SP_Profile on ent.ProfileId equals SP_Prof.Id
                             where ent.EntryId == EntryId
                             select new { ent.Id, Name = "ОП: " + SP_ObrazProgr.Name + "; Профиль: " + SP_Prof.Name }).ToList()
                             .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).Distinct()
                             .ToList();
            ComboServ.FillCombo(cbInnerEntryInEntry, ObrazProgramInEntryList, false, false);
        }

        private void btnSaveInnerEntryInEntry_Click(object sender, EventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                context.Abiturient_UpdateInnerEntryInEntryId(InnerEntryInEntryId, GuidId);
            }
        }

        private void btnChangeOriginalsDestination_Click(object sender, EventArgs e)
        {
            OpenCardChangeOriginalsDestination();
        }

        private void OpenCardChangeOriginalsDestination()
        {
            var crd = new CardChangeOriginalsPlace(GuidId.Value);
            crd.OnUpdated += FillCard;
            crd.Show();
        }

        private void dgvAppExams_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvAppExams.CurrentCell == null)
                return;

            Guid BlockId = Guid.Parse(dgvAppExams.CurrentRow.Cells["Id"].Value.ToString());
            Guid ExamsList = Guid.Parse(dgvAppExams.CurrentRow.Cells["ExamsList"].Value.ToString());

            var bl = lstExamInEntryBlock.Where(x => x.BlockId == BlockId).First();
            bl.SelectedUnitId = ExamsList;
        }

        private void btnChangeEntry_Click(object sender, EventArgs e)
        {
            CardSelectEntry crd;
            if (EntryId.HasValue) 
                crd = new CardSelectEntry(EntryId.Value);
            else
                crd = new CardSelectEntry();

            crd.EntrySelected += (entId) => { UpdateEntryData(entId); };
            crd.Show();
            crd.Focus();
        }

        private void btnDeleteAbiturientAchievement_Click(object sender, EventArgs e)
        {
            if (MainClass.IsFacMain() || MainClass.IsPasha())
            {
                if (dgvAdditionalAchievements.SelectedCells.Count == 0)
                    return;

                int iRowInd = dgvAdditionalAchievements.SelectedCells[0].RowIndex;
                string sId = dgvAdditionalAchievements["Id", iRowInd].Value.ToString();
                if (string.IsNullOrEmpty(sId))
                    return;

                Guid gId = Guid.Empty;
                if (!Guid.TryParse(sId, out gId))
                    return;

                using (PriemEntities context = new PriemEntities())
                {
                    var ent = context.MarkFromAchievement.Where(x => x.Id == gId).FirstOrDefault();
                    if (ent != null)
                    {
                        string Message = "Вы уверены, что хотите удалить балл за указанное ИД?";
                        var dr = MessageBox.Show(Message, "", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            context.MarkFromAchievement.Remove(ent);
                            context.SaveChanges();

                            FillAdditionalAchievements(context);
                        }
                    }
                }
            }
            else
            {
                WinFormsServ.Error("Данный функционал доступен только для ответственных секретарей и администратора");
            }
        }

        private void btnAddAbiturientAchievement_Click(object sender, EventArgs e)
        {
            if (!GuidId.HasValue)
            {
                WinFormsServ.Error("Сперва сохраните карточку!");
                return;
            }

            Cards.CardAddMarkFromAchievement crd = new Cards.CardAddMarkFromAchievement(GuidId.Value);
            crd.OK += () =>
            {
                using (PriemEntities context = new PriemEntities())
                {
                    FillAdditionalAchievements(context);
                }
            };
            crd.Show();
        }
    }
}
