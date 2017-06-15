using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Transactions;

using BaseFormsLib;
using EducServLib;
using System.Data.Entity.Core.Objects;
using System.Threading.Tasks;

namespace PriemLib
{
    public partial class CardPerson : CardFromList
    { 
        private int? personBarc;
        private int _currentEducRow;

        private int _currentEgeRequestId;

        private bool inEnableProtocol;
        private bool inEntryView;
        private List<Person_EducationInfo> lstEducationInfo;
        
        public CardPerson()
            : this(null, null, null)
        {
        }

        // конструктор формы открытие и создание в нашей базе
        public CardPerson(string id)
            : this(id, null, null)
        {                        
        }
        
        // конструктор формы
        public CardPerson(string id, int? rowInd, BaseFormEx formOwner)
        {
            InitializeComponent();
            _Id = id;             
            tcCard = tabCard;
            
            this.formOwner = formOwner;
            if(rowInd.HasValue)
                this.ownerRowIndex = rowInd.Value; 

            //InitControls();     
            this.Load += (x, y) => InitControls();
        }
        
        protected override void ExtraInit()
        {
            bw_EgeRequestCheck = new BackgroundWorker();
            bw_EgeRequestCheck.DoWork += bw_EgeRequestCheck_DoWork;
            bw_EgeRequestCheck.RunWorkerCompleted += bw_EgeRequestCheck_RunWorkerCompleted;
            bw_EgeRequestCheck.WorkerSupportsCancellation = true;

            base.ExtraInit();                        
            _tableName = "ed.Person";
            
            Dgv = dgvApplications;
            personBarc = 0;  

            if (_Id != null && (MainClass.IsPasha()))
                btnSetStatusPasha.Visible = tbCommentFBSPasha.Visible = true;
            else
                btnSetStatusPasha.Visible = tbCommentFBSPasha.Visible = false;
             
            rbMale.Checked = true;

            gbAtt.Visible = true;
            gbDipl.Visible = false;
            chbEkvivEduc.Visible = false;
            
            chbHostelAbitYes.Checked = false;
            chbHostelAbitNo.Checked = false;

            lblHasAssignToHostel.Visible = false;
            lblHasExamPass.Visible = false;
            btnPrintHostel.Enabled = false;
            btnPrintExamPass.Enabled = false;
            btnGetAssignToHostel.Enabled = false;
            btnGetExamPass.Enabled = false; 
            
            tbNum.Enabled = false;
            tbFBSStatus.Enabled = false;  
            
            btnAddAbit.Enabled = false;

            if (_Id == null)
                tpEge.Parent = null;

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    ComboServ.FillCombo(cbPassportType, HelpClass.GetComboListByTable("ed.PassportType", " ORDER BY Id "), false, false);
                    if (MainClass.dbType == PriemType.PriemForeigners)
                    {
                        ComboServ.FillCombo(cbCountry, HelpClass.GetComboListByTable("ed.ForeignCountry", "ORDER BY Distance, Name"), false, false);
                        ComboServ.FillCombo(cbNationality, HelpClass.GetComboListByTable("ed.ForeignCountry", "ORDER BY Distance, Name"), false, false);
                        ComboServ.FillCombo(cbCountryEduc, HelpClass.GetComboListByTable("ed.ForeignCountry", "ORDER BY Distance, Name"), false, false);
                    }
                    else
                    {
                        ComboServ.FillCombo(cbCountry, HelpClass.GetComboListByTable("ed.Country", "ORDER BY Distance, Name"), false, false);
                        ComboServ.FillCombo(cbNationality, HelpClass.GetComboListByTable("ed.Country", "ORDER BY Distance, Name"), false, false);
                        ComboServ.FillCombo(cbCountryEduc, HelpClass.GetComboListByTable("ed.Country", "ORDER BY Distance, Name"), false, false);
                        ComboServ.FillCombo(cbExitClass, HelpClass.GetComboListByTable("ed.SchoolExitClass", "ORDER BY Name"), false, false);
                        ComboServ.FillCombo(cbExtPoss, HelpClass.GetComboListByTable("ed.ExtPoss", "ORDER BY Code"), false, false);
                    }
                    UpdateAfterCountry();
                    ComboServ.FillCombo(cbLanguage, HelpClass.GetComboListByTable("ed.Language"), false, false);
                    ComboServ.FillCombo(cbMSStudyForm, HelpClass.GetComboListByTable("ed.StudyForm"), true, false);
                    ComboServ.FillCombo(cbHEStudyForm, HelpClass.GetComboListByTable("ed.StudyForm"), true, false);
                    ComboServ.FillCombo(cbSchoolType, HelpClass.GetComboListByTable("ed.SchoolType", "ORDER BY 1"), false, false);

                    rbReturnDocumentType1.Text = context.ReturnDocumentType.Where(x => x.Id == 1).Select(x => x.Name).First();
                    rbReturnDocumentType2.Text = context.ReturnDocumentType.Where(x => x.Id == 2).Select(x => x.Name).First();

                    //cbSchoolCity.DataSource = context.Database.SqlQuery<string>("SELECT DISTINCT ed.Person_EducationInfo.SchoolCity AS Name FROM ed.Person_EducationInfo WHERE ed.Person_EducationInfo.SchoolCity > '' ORDER BY 1");
                    //cbAttestatSeries.DataSource = context.Database.SqlQuery<string>("SELECT DISTINCT ed.Person_EducationInfo.AttestatSeries AS Name FROM ed.Person_EducationInfo WHERE ed.Person_EducationInfo.AttestatSeries > '' ORDER BY 1");
                    //cbHEQualification.DataSource = context.Database.SqlQuery<string>("SELECT DISTINCT ed.Person_EducationInfo.HEQualification AS Name FROM ed.Person_EducationInfo WHERE NOT ed.Person_EducationInfo.HEQualification IS NULL /*AND ed.Person_EducationInfo.HEQualification > ''*/ ORDER BY 1");
                    cbSchoolCity.DataSource = context.Person_EducationInfo.Where(x => x.SchoolCity != "")
                        .Select(x => x.SchoolCity).Distinct().ToList().OrderBy(x => x).ToList();
                    cbAttestatSeries.DataSource = context.Person_EducationInfo.Where(x => x.AttestatSeries != "")
                        .Select(x => x.AttestatSeries).Distinct().ToList().OrderBy(x => x).ToList();
                    cbHEQualification.DataSource = context.Person_EducationInfo.Where(x => x.HEQualification != null && x.HEQualification != "")
                        .Select(x => x.HEQualification).Distinct().ToList().OrderBy(x => x).ToList();

                    cbAttestatSeries.SelectedIndex = -1;
                    cbSchoolCity.SelectedIndex = -1;
                    cbHEQualification.SelectedIndex = -1;

                    ComboServ.FillCombo(cbScienceWorkType, HelpClass.GetComboListByTable("ed.ScienceWorkType", "ORDER BY 1"), false, false);
                    ComboServ.FillCombo(cbSportQualification, HelpClass.GetComboListByTable("ed.SportQualification", "ORDER BY Id"), false, false);
                }

                btnDocs.Visible = true;

                // магистратура!
                if (MainClass.dbType != PriemType.Priem)
                {
                    tpEge.Parent = null;
                    tpSecond.Parent = null;
                }
                else if (MainClass.dbType == PriemType.Priem)
                {
                    gbMainStudy.Visible = true;
                    btnDocs.Visible = false;
                }
                

                gbPashaTechInfo.Visible = MainClass.IsPasha();
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы ", exc);
            }
        }
        protected override bool IsForReadOnly()
        {
            return !(MainClass.RightsToEditCards() || MainClass.RightsSov_SovMain());
        }
        protected override void SetReadOnlyFieldsAfterFill()
        {
            base.SetReadOnlyFieldsAfterFill();

            if (_Id == null)
            {
                btnDocs.Enabled = false;
            }

            WinFormsServ.SetSubControlsEnabled(gbPashaTechInfo, MainClass.IsPasha());
        }
        
        #region handlers

        protected override void InitHandlers()
        {
            cbSchoolType.SelectedIndexChanged += new EventHandler(UpdateAfterSchool);
            cbCountry.SelectedIndexChanged += new EventHandler(UpdateAfterCountry);
            cbCountryEduc.SelectedIndexChanged += new EventHandler(UpdateAfterCountryEduc);
        }
        protected override void NullHandlers()
        {
            cbSchoolType.SelectedIndexChanged -= new EventHandler(UpdateAfterSchool);
            cbCountry.SelectedIndexChanged -= new EventHandler(UpdateAfterCountry);
            cbCountryEduc.SelectedIndexChanged -= new EventHandler(UpdateAfterCountryEduc);
        }        

        private void UpdateAfterSchool(object sender, EventArgs e)
        {
            if (SchoolTypeId == MainClass.educSchoolId)
            {
                gbAtt.Visible = true;
                gbDipl.Visible = false;
                gbFinishStudy.Visible = false;
                tbSchoolNum.Visible = true;
                lblSchoolNum.Visible = true;
                chbIsExcellent.Text = "Медалист (отличник)";
                btnAttMarks.Visible = true;
            }               
            else
            {
                gbAtt.Visible = false;
                gbDipl.Visible = true;
                gbFinishStudy.Visible = true;
                tbSchoolNum.Visible = false;
                lblSchoolNum.Visible = false;
                chbIsExcellent.Text = "Диплом с отличием";
                btnAttMarks.Visible = false;
            }
        }
        private void UpdateAfterCountry(object sender, EventArgs e)
        {
            if (CountryId == MainClass.countryRussiaId)
            {
                cbRegion.Enabled = true;
                cbRegion.SelectedItem = "нет";
            }
            else
            {
                cbRegion.Enabled = false;
                cbRegion.SelectedItem = "нет";
            }
        }
        private void UpdateAfterCountryEduc(object sender, EventArgs e)
        {
            if (CountryEducId == MainClass.countryRussiaId)           
                chbEkvivEduc.Visible = false;
            else
                chbEkvivEduc.Visible = true;
        }

        private void chbHostelAbitYes_CheckedChanged(object sender, EventArgs e)
        {
            chbHostelAbitNo.Checked = !chbHostelAbitYes.Checked;
            if (_Id != null)
                btnGetAssignToHostel.Enabled = chbHostelAbitYes.Checked;
        }
        private void chbHostelAbitNo_CheckedChanged(object sender, EventArgs e)
        {
            chbHostelAbitYes.Checked = !chbHostelAbitNo.Checked;
            if (_Id != null)
                btnGetAssignToHostel.Enabled = !chbHostelAbitNo.Checked;
        }

        #endregion

        private void FillPersonData(ref extPerson person)
        {
            CardTitle = Util.GetFIO(person.Surname, person.Name, person.SecondName);
            PersonName = person.Name;
            SecondName = person.SecondName;
            Surname = person.Surname;
            BirthDate = person.BirthDate;
            BirthPlace = person.BirthPlace;
            PassportTypeId = person.PassportTypeId;
            PassportSeries = person.PassportSeries;
            PassportNumber = person.PassportNumber;
            PassportAuthor = person.PassportAuthor;
            PassportDate = person.PassportDate;
            PassportCode = person.PassportCode;
            PersonalCode = person.PersonalCode;
            SNILS = person.SNILS;

            Sex = person.Sex;
            CountryId = person.CountryId;
            ForeignCountryId = person.ForeignCountryId;
            NationalityId = person.NationalityId;
            ForeignNationalityId = person.ForeignNationalityId;
            RegionId = person.RegionId;
            Phone = person.Phone;
            Mobiles = person.Mobiles;
            Email = person.Email;
            AddEmail = person.AddEmail;
            Code = person.Code;
            City = person.City;
            Street = person.Street;
            House = person.House;
            Korpus = person.Korpus;
            Flat = person.Flat;
            CodeReal = person.CodeReal;
            CityReal = person.CityReal;
            StreetReal = person.StreetReal;
            HouseReal = person.HouseReal;
            KorpusReal = person.KorpusReal;
            FlatReal = person.FlatReal;
            KladrCode = person.KladrCode;
            HostelAbit = person.HostelAbit;
            HostelEduc = person.HostelEduc;
            HasAssignToHostel = person.HasAssignToHostel;
            HostelFacultyId = person.HostelFacultyId;
            HasExamPass = person.HasExamPass;
            ExamPassFacultyId = person.ExamPassFacultyId;
            LanguageId = person.LanguageId;
            Stag = person.Stag;
            WorkPlace = person.WorkPlace;
            MSVuz = person.MSVuz;
            MSCourse = person.MSCourse;
            MSStudyFormId = person.MSStudyFormId;
            Privileges = person.Privileges;
            ExtPossId = person.ExtPossId;
            ExtraInfo = person.ExtraInfo;
            PersonInfo = person.PersonInfo;
            ScienceWork = person.ScienceWork;
            StartEnglish = person.StartEnglish;
            EnglishMark = person.EnglishMark;
            EgeInSpbgu = person.EgeInSPbgu;
            ReturnDocumentTypeId = person.ReturnDocumentTypeId;

            try
            {
                UpdateEducationData();
            }
            catch (Exception ex)
            {
                WinFormsServ.Error(ex);
            }

            personBarc = person.Barcode;
            tbGUID.Text = person.Id.ToString();
            btnSendToOnline.Enabled = personBarc.HasValue && personBarc > 0;
            if (personBarc.HasValue)
                tbBarcode.Text = personBarc.ToString();
            FillLanguageCertificates();
            FillCategoryForManualExam();
        }

        public void FillLanguageCertificates()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    var lang = (from x in context.PersonLanguageCertificates
                                join t in context.LanguageCertificatesType on 
                                x.LanguageCertificateTypeId equals t.Id
                                 
                                where x.PersonId == GuidId
                                select new 
                                {
                                     Id = x.Id,
                                     Name = t.Name,
                                     Number = x.Number,
                                     ResultType = t.BoolType,
                                     ResultValue = x.ResultValue,
                                     x.LanguageCertificateTypeId
                                }).ToList();
                    
                    var datasourse = (from l in lang
                                      select new
                                      {
                                          Id = l.Id,
                                          Название = l.Name,
                                          Номер = l.Number,
                                          Результат = l.ResultType ? "сдан" : (l.ResultValue).ToString(),
                                      }).ToList();
                    dgvCertificates.DataSource = datasourse;
                    if (dgvCertificates.Columns.Contains("Id"))
                    {
                        dgvCertificates.Columns["Id"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //данные из нашей базы
        protected override void FillCard()
        {
            if (_Id == null)
                return;                   
                        
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    extPerson person = (from pr in context.extPerson
                                     where pr.Id == GuidId
                                     select pr).FirstOrDefault();                   

                    tbNum.Text = person.PersonNum;
                    FillPersonData(ref person);

                    lblCardAuthorInfo.Text = person.DateCreated.ToShortDateString() + " " + person.DateCreated.ToShortTimeString() + " " + 
                        MainClass.GetADUserName(person.Author) + " (" + MainClass.GetFacultyForAccount(person.Author) + ")";
                    
                    FBSStatus = GetFBSStatus(GuidId);

                    //Обновление грида ЕГЭ желательно сделать асинхронным
                    UpdateDataGridEge();

                    //эти тоже стоит перевести в асинхронные
                    FillApplications();
                    UpdateGridBenefits();
                    UpdatePersonAchievement();
                    UpdateNoticies();
                    UpdateVedGrid();
                    FillPersonScienceWork();
                    FillPersonWork();
                    FillPersonParents();
                    FillSportQulification();
                    //Async functions
                    GetHasOriginals();
                    GetIsPaid();

                    if (GuidId.HasValue)
                    {
                        inEnableProtocol = PersonDataProvider.GetInEnableProtocol(GuidId.Value);
                        inEntryView = PersonDataProvider.GetInEntryView(GuidId.Value);
                    }

                    var EgeCheck = context.PersonEgeRequest.Where(x => x.PersonId == GuidId.Value && !x.IsChecked).FirstOrDefault();
                    if (EgeCheck != null)
                    {
                        _currentEgeRequestId = EgeCheck.IntId;
                        btnRequestEge.Enabled = false;
                        btnRequestEge.Visible = false;
                        lblHasRequest.Visible = true;

                        bw_EgeRequestCheck.RunWorkerAsync();
                    }
                }
            }
            catch (DataException de)
            {
                WinFormsServ.Error("Ошибка при заполнении формы (DataException)", de);
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка при заполнении формы ", ex);
            }
        }
        public void FillApplications()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    var sourceOwn =
                        (from abit in context.qAbiturient
                         join comp in context.Competition on abit.CompetitionId equals comp.Id
                         where !abit.BackDoc && abit.PersonId == GuidId
                         && MainClass.lstStudyLevelGroupId.Contains(abit.StudyLevelGroupId)
                         && (MainClass.dbType != PriemType.PriemForeigners ? abit.IsForeign == false : true)
                         orderby abit.Priority, abit.FacultyAcr, abit.ObrazProgramCrypt
                         select new
                         {
                             abit.Id,
                             Приор = abit.Priority,
                             Тип_конк = comp.Name,
                             Факультет = abit.FacultyAcr,
                             Направление = abit.LicenseProgramName,
                             Образ_программа = (abit.IsForeign ? "(иностр) " : "") + abit.ObrazProgramCrypt + (abit.IsCrimea ? "(крым) " : "") + abit.ObrazProgramName,
                             Профиль = abit.ProfileName,
                             Форма_обучения = abit.StudyBasisName,
                             Основа_обучения = abit.StudyFormName,
                             abit.IsViewed
                         }).ToList();

                    var sourceAll =
                        (from abit in context.qAbitAll
                         join comp in context.Competition on abit.CompetitionId equals comp.Id
                         where !abit.BackDoc && abit.PersonId == GuidId
                         //&& MainClass.lstStudyLevelGroupId.Contains(abit.StudyLevelGroupId)
                         //&& (MainClass.dbType != PriemType.PriemForeigners ? abit.IsForeign == false : true)
                         orderby abit.Priority, abit.FacultyAcr, abit.LicenseProgramName
                         select new
                         {
                             abit.Id,
                             Приор = abit.Priority,
                             Тип_конк = comp.Name,
                             Факультет = abit.FacultyAcr,
                             Направление = abit.LicenseProgramName,
                             Образ_программа = (abit.IsForeign ? "(иностр) " : "") + abit.ObrazProgramCrypt + (abit.IsCrimea ? "(крым) " : "") + abit.ObrazProgramName,
                             Профиль = abit.ProfileName,
                             Форма_обучения = abit.StudyBasisName,
                             Основа_обучения = abit.StudyFormName,
                             abit.IsViewed
                         }).ToList().Except(sourceOwn).ToList();

                    dgvApplications.DataSource = Converter.ConvertToDataTable(sourceOwn.ToArray());
                    dgvApplications.Columns["Id"].Visible = false;
                    dgvApplications.Columns["IsViewed"].Visible = false;

                    dgvOtherAppl.DataSource = Converter.ConvertToDataTable(sourceAll.ToArray());
                    dgvOtherAppl.Columns["Id"].Visible = false;
                    dgvOtherAppl.Columns["IsViewed"].Visible = false;

                    // после зачисления раскомментить
                    var entries =
                        (from ev in context.extEntryProtocols
                         join ab in context.extAbit
                         on ev.AbiturientId equals ab.Id
                         where !ab.BackDoc && ab.PersonId == GuidId
                         select ab).FirstOrDefault();

                    if(entries == null)                    
                        gbEnter.Visible = false;
                    else
                    {
                        gbEnter.Visible = true;
                        lblFaculty.Text = entries.FacultyName;
                        lblStudyForm.Text = entries.StudyFormName;
                        lblStudyBasis.Text = entries.StudyBasisName;
                        lblProfession.Text = entries.LicenseProgramCode + " " + entries.LicenseProgramName;
                        lblObrazProgram.Text = entries.ObrazProgramCrypt + " " + entries.ObrazProgramName;
                        lblProfile.Text = entries.ProfileName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void FillPersonScienceWork()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    var ScienceWork =
                        (from x in context.PersonScienceWork
                         where x.PersonId == GuidId
                         select new
                         {
                             x.Id,
                             Тип_работы = x.ScienceWorkType.Name,
                             Год = x.WorkYear,
                             Сведения = x.WorkInfo
                         }).ToList();

                    dgvPersonScienceWork.DataSource = ScienceWork;
                    foreach (DataGridViewColumn c in dgvPersonScienceWork.Columns)
                        c.HeaderText = c.Name.Replace("_", " ");
                    foreach (string s in new List<string>() { "Id" })
                        if (dgvPersonScienceWork.Columns.Contains(s))
                            dgvPersonScienceWork.Columns[s].Visible = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void FillPersonWork()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    var PersonWork =
                        (from x in context.PersonWork
                         where x.PersonId == GuidId
                         select new
                         {
                             x.Id,
                             Стаж = x.Stage,
                             Место_работы = x.WorkPlace,
                             Должность = x.WorkProfession,
                             Обязанности = x.WorkSpecifications
                         }).ToList();

                    dgvPersonWork.DataSource = PersonWork;
                    foreach (DataGridViewColumn c in dgvPersonWork.Columns)
                        c.HeaderText = c.Name.Replace("_", " ");
                    foreach (string s in new List<string>() { "Id" })
                        if (dgvPersonWork.Columns.Contains(s))
                            dgvPersonWork.Columns[s].Visible = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void dgvApplications_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && !(bool)dgvApplications["IsViewed", e.RowIndex].Value)
            {
                e.CellStyle.BackColor = Color.LightCoral;
            }
        }

        #region Async Operations
        BackgroundWorker bw;
        private void GetHasOriginals()
        {
            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            var arg = new { GuidId = GuidId };

            bw.RunWorkerAsync(arg);
            lblSearchingOriginals.Visible = true;
        }
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblSearchingOriginals.Visible = false;

            if (e.Result == null)
                return;

            var _who = ((qAbiturient_WhoSetHasOriginals)e.Result);
            string who = _who.UserId;
            string whoFac = _who.FacultyName;
            string whoDate = _who.ActionTime.ToShortDateString() + " " + _who.ActionTime.ToShortTimeString();
            who = MainClass.GetADUserName(who);

            if (!string.IsNullOrEmpty(who))
            {
                lblHasOriginalsUser.Text = "Проставлено: " + who + " (" + whoDate + " " + whoFac + ")";
                lblHasOriginalsUser.Visible = true;
                chbHasOriginals.Checked = true;
            }
            else
            {
                lblHasOriginalsUser.Text = "";
                lblHasOriginalsUser.Visible = false;
                chbHasOriginals.Checked = false;
            }
        }
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                try
                {
                    Guid gId = ((dynamic)(e.Argument)).GuidId;

                    e.Result = (from orig in context.qAbiturient_WhoSetHasOriginals
                                join Abit in context.Abiturient on orig.Id equals Abit.Id
                                where orig.PersonId == GuidId && !Abit.BackDoc
                                select orig).FirstOrDefault();
                }
                catch { e.Result = new qAbiturient_WhoSetHasOriginals(); }
            }
        }

        BackgroundWorker bw_ispaid;
        private void GetIsPaid()
        {
            bw_ispaid = new BackgroundWorker();
            bw_ispaid.DoWork += bw_ispaid_DoWork;
            bw_ispaid.RunWorkerCompleted += bw_ispaid_RunWorkerCompleted;

            var arg = GuidId;

            bw_ispaid.RunWorkerAsync(arg);
            lblSearchingOriginals.Visible = true;
        }
        void bw_ispaid_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblSearchingDogovor.Visible = false;

            if (e.Result == null)
                return;

            var _who = ((qAbiturient_WhoSetIsPaid)e.Result);
            string who = _who.UserId;
            string whoFac = _who.FacultyName;
            string whoDate = _who.ActionTime.ToShortDateString() + " " + _who.ActionTime.ToShortTimeString();
            who = MainClass.GetADUserName(who);

            if (!string.IsNullOrEmpty(who))
            {
                lblHasDogovorUser.Text = "Проставлено: " + who + " (" + whoDate + " " + whoFac + ")";
                lblHasDogovorUser.Visible = true;
                chbHasDogovor.Checked = true;
            }
            else
            {
                lblHasDogovorUser.Text = "";
                lblHasDogovorUser.Visible = false;
                chbHasDogovor.Checked = false;
            }
        }
        void bw_ispaid_DoWork(object sender, DoWorkEventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                Guid gId = (Guid)(e.Argument);

                e.Result = (from paid in context.qAbiturient_WhoSetIsPaid
                            join Abit in context.Abiturient on paid.Id equals Abit.Id
                            where paid.PersonId == gId && !Abit.BackDoc
                            select paid).FirstOrDefault();
            }
        }

        #endregion

        #region ReadOnly & IsOpen

        // карточка открывается в режиме read-only
        protected override void SetAllFieldsNotEnabled()
        {
            base.SetAllFieldsNotEnabled();
            
            dgvApplications.Enabled = true;
            dgvOtherAppl.Enabled = true;
            
            gbEge.Enabled = true;
            WinFormsServ.SetSubControlsEnabled(gbEge, false); 
            btnCardE.Enabled = true;           
            
            btnAttMarks.Enabled = true;           
                        
            if (HasAssignToHostel.Value && MainClass.RightsFacMain() && MainClass.HasRightsForFaculty(HostelFacultyId))
                SetBtnPrintHostelEnabled();            
            
            if (HasExamPass.Value && MainClass.RightsFacMain() && MainClass.HasRightsForFaculty(ExamPassFacultyId))
                SetBtnPrintExamPassEnabled();

            if (!IsForReadOnly() && !inEntryView)
                btnAddAbit.Enabled = true;

            btnDocs.Enabled = true;

            btnPrintApplication.Enabled = true;
            chbApplicationPrint.Enabled = true;

            btnDocsList.Enabled = true;

            if (MainClass.IsPasha())
            {
                btnSetStatusPasha.Enabled = tbCommentFBSPasha.Enabled = true;
            }

            if (MainClass.IsPasha() || MainClass.IsFacMain())
                btnRequestEge.Enabled = true;

            WinFormsServ.SetSubControlsEnabled(gbEducationDocuments, true);
            WinFormsServ.SetSubControlsEnabled(gbPersonAchievements, true);
            WinFormsServ.SetSubControlsEnabled(tabPage7, true);

            WinFormsServ.SetSubControlsEnabled(gbAllPrivileges, true);
            btnAddBenefitDocument.Enabled = false;
            btnDeleteBenefitDocument.Enabled = false;

            btnAddPersonAchievement.Enabled = !inEntryView && !MainClass.IsReadOnly();
            btnDeletePersonAchievement.Enabled = !inEntryView && !MainClass.IsReadOnly();
            dgvIndividualAchievements.Enabled = !inEntryView && !MainClass.IsReadOnly();

            btnAddEducDoc.Enabled = false;
            btnDeleteEducDoc.Enabled = false;

            tbWorkPlace.Enabled = tbWorkStag.Enabled = tbWorkProfession.Enabled = tbWorkSpecification.Enabled = false;
            tbScienceWork.Enabled = tbScienceWorkYear.Enabled = cbScienceWorkType.Enabled = false;

        }
        //убрать режим read-only
        protected override void SetAllFieldsEnabled()
        {
            base.SetAllFieldsEnabled();
            
            btnPrintHostel.Enabled = false;
            btnPrintExamPass.Enabled = false;

            chbHasOriginals.Enabled = false;
            chbHasDogovor.Enabled = false;

            if (HasAssignToHostel.Value)
            {
                chbHostelAbitYes.Enabled = chbHostelAbitNo.Enabled = false;
                btnGetAssignToHostel.Enabled = false;

                if (MainClass.RightsFacMain() && MainClass.HasRightsForFaculty(HostelFacultyId))
                    btnPrintHostel.Enabled = true;
            }
            else
            {
                if(chbHostelAbitYes.Checked)
                    btnGetAssignToHostel.Enabled = true;
                else
                    btnGetAssignToHostel.Enabled = false;
            }

            if (HasExamPass.Value)
            {
                btnGetExamPass.Enabled = false;

                if (MainClass.RightsFacMain() && MainClass.HasRightsForFaculty(ExamPassFacultyId))
                    btnPrintExamPass.Enabled = true;                    
            }
            else            
                btnGetExamPass.Enabled = true;              
          
            WinFormsServ.SetSubControlsEnabled(gbEge, true);
            WinFormsServ.SetSubControlsEnabled(gbAllPrivileges, true);
            
            btnAttMarks.Enabled = true;            

            tbNum.Enabled = false;
            tbFBSStatus.Enabled = false;
            gbEnter.Enabled = false;

            SetReadOnlyFields();
        }
        // закрытие части полей в зависимости от прав
        protected override void SetReadOnlyFields()
        {
            if (!inEnableProtocol)
            {
                btnAddEducDoc.Enabled = true;
                btnDeleteEducDoc.Enabled = true;
            }

            if (MainClass.RightsFaculty())
            {
                tbName.Enabled = false;
                tbSurname.Enabled = false;
                tbSecondName.Enabled = false;

                dtBirthDate.Enabled = false;

                cbPassportType.Enabled = false;
                tbPassportAuthor.Enabled = false;
                tbPassportNumber.Enabled = false;
                tbPassportSeries.Enabled = false;
                dtPassportDate.Enabled = false;

                //tbAttestatNum.Enabled = false;
                //cbAttestatSeries.Enabled = false;

                gbPrivileges.Enabled = false;

                //временная добавка, ибо очень уж просили               
                //btnAttMarks.Enabled = true;

                btnRemoveE.Enabled = false;
            }
            if (inEnableProtocol && MainClass.RightsFaculty())
            {
                SetAllFieldsNotEnabled();

                tbMobiles.Enabled = true;

                //tbDiplomNum.Enabled = true;
                //tbDiplomSeries.Enabled = true;
                
                btnSaveChange.Enabled = true;
                btnClose.Enabled = true;
                btnAddAbit.Enabled = true;

                //попросили, чтобы можно было добавлять даже зачисленным в протокол о допуске
                //gbEduc.Enabled = true;
                //btnAttMarks.Enabled = true;
            }

            if (inEnableProtocol && MainClass.RightsSov_SovMain_FacMain())
            {
                tbName.Enabled = false;
                tbSurname.Enabled = false;
                tbSecondName.Enabled = false;

                dtBirthDate.Enabled = false;

                cbPassportType.Enabled = false;
                tbPassportAuthor.Enabled = false;
                tbPassportNumber.Enabled = false;
                tbPassportSeries.Enabled = false;
                dtPassportDate.Enabled = false;

                //tbAttestatNum.Enabled = false;
                //cbAttestatSeries.Enabled = false;

                gbPrivileges.Enabled = false;
               
                btnRemoveE.Enabled = false; 
            }

            if (MainClass.RightsSov_SovMain())
            {
                cbPassportType.Enabled = true;
                tbPassportAuthor.Enabled = true;
                tbPassportNumber.Enabled = true;
                tbPassportSeries.Enabled = true;

                dtPassportDate.Enabled = true;

                //tbAttestatNum.Enabled = true;
                //cbAttestatSeries.Enabled = true;

                //tbDiplomNum.Enabled = true;
                //tbDiplomSeries.Enabled = true;
            }

            // закрываем для создания новых для уже зачисленных
            if (inEntryView)
            {
                btnAddAbit.Enabled = false;
                //chbIsExcellent.Enabled = false;
                //tbSchoolAVG.Enabled = false;

                btnAddE.Enabled = false;
                btnRemoveE.Enabled = false;

                //btnAddEducDoc.Enabled = false;
                //btnDeleteEducDoc.Enabled = false;
            }

            UpdateEducationInfoFieldsForEditing();
        }
        /// <summary>
        /// Обновляет доступ к полям вкладки "Образование" в зависимости от статуса пользователя
        /// </summary>
        private void UpdateEducationInfoFieldsForEditing()
        {
            if (MainClass.RightsFaculty())
            {
                tbAttestatNum.Enabled = false;
                cbAttestatSeries.Enabled = false;

                btnAttMarks.Enabled = true;
            }
            if (inEnableProtocol && MainClass.RightsFaculty())
            {
                tbDiplomNum.Enabled = true;
                tbDiplomSeries.Enabled = true;

                gbEduc.Enabled = true;
                btnAttMarks.Enabled = true;
            }
            if (inEnableProtocol && MainClass.RightsSov_SovMain_FacMain())
            {
                tbAttestatNum.Enabled = false;
                cbAttestatSeries.Enabled = false;
            }

            if (MainClass.RightsSov_SovMain())
            {
                tbAttestatNum.Enabled = true;
                cbAttestatSeries.Enabled = true;
                
                tbDiplomNum.Enabled = true;
                tbDiplomSeries.Enabled = true;
            }

            // закрываем для создания новых для уже зачисленных
            if (inEntryView)
            {
                chbIsExcellent.Enabled = false;
                tbSchoolAVG.Enabled = false;

                btnAddEducDoc.Enabled = false;
                btnDeleteEducDoc.Enabled = false;
            }

            if (MainClass.IsPasha() && !inEntryView)
            {
                tbAttestatNum.Enabled = true;
                cbAttestatSeries.Enabled = true;

                tbDiplomNum.Enabled = true;
                tbDiplomSeries.Enabled = true;

                btnAddEducDoc.Enabled = true;
                btnDeleteEducDoc.Enabled = true;

                tbSurname.Enabled = true;
                tbName.Enabled = true;
                tbSecondName.Enabled = true;
            }
        }

        private void SetBtnPrintHostelEnabled()
        {
            gbHostel.Enabled = true;
            btnGetAssignToHostel.Enabled = false;
            btnPrintHostel.Enabled = true;
        }
        private void SetBtnPrintExamPassEnabled()
        {
            gbExamPass.Enabled = true;
            btnGetExamPass.Enabled = false;
            btnPrintExamPass.Enabled = true;
        }
        
        #endregion

        #region Save

        // проверка на уникальность абитуриента
        private bool CheckIdent()
        {
            using (PriemEntities context = new PriemEntities())
            {
                ObjectParameter boolPar = new ObjectParameter("result", typeof(bool));

                if(_Id == null)
                    context.CheckPersonIdent(Surname, Name, SecondName, BirthDate, PassportSeries, PassportNumber, AttestatSeries, AttestatNum, boolPar);
                else
                    context.CheckPersonIdentWithId(Surname, Name, SecondName, BirthDate, PassportSeries, PassportNumber, AttestatSeries, AttestatNum, GuidId, boolPar);

                return Convert.ToBoolean(boolPar.Value);
            }
        }
        protected override bool CheckFields()
        { 
            // проверка на уникальность номера
            if (Surname.Length <= 0)
            {
                epErrorInput.SetError(tbSurname, "Отсутствует фамилия абитуриента");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();
            
            if (PersonName.Length <= 0)
            {
                epErrorInput.SetError(tbName, "Отсутствует имя абитуриента");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();

            

            //Для О'Коннор сделал добавку в регулярное выражение: \'
            if (!Regex.IsMatch(Surname, @"^[А-Яа-яёЁ\-\'\s]+$"))
            {
                epErrorInput.SetError(tbSurname, "Неправильный формат");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();

            if (!Regex.IsMatch(PersonName, @"^[А-Яа-яёЁ\-\s]+$"))
            {
                epErrorInput.SetError(tbName, "Неправильный формат");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();

            if (!Regex.IsMatch(SecondName, @"^[А-Яа-яёЁ\-\s]*$"))
            {
                epErrorInput.SetError(tbSecondName, "Неправильный формат");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();

            if (SecondName.StartsWith("-"))
            {
                SecondName = SecondName.Replace("-", "");                
            }           
            
            // проверка на англ. буквы
            if (!Util.IsRussianString(PersonName))
            {
                epErrorInput.SetError(tbName, "Имя содержит английские символы, используйте только русскую раскладку");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();

            if (!Util.IsRussianString(Surname))
            {
                epErrorInput.SetError(tbSurname, "Фамилия содержит английские символы, используйте только русскую раскладку");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();

            if (!Util.IsRussianString(SecondName))
            {
                epErrorInput.SetError(tbSecondName, "Отчество содержит английские символы, используйте только русскую раскладку");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();           
                       
            if (BirthDate == null)
            {
                epErrorInput.SetError(dtBirthDate, "Неправильно указана дата");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();

            int checkYear = DateTime.Now.Year - 12;
            if (BirthDate.Value.Year > checkYear || BirthDate.Value.Year < 1920)            
            {
                epErrorInput.SetError(dtBirthDate, "Неправильно указана дата");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();

            if (string.IsNullOrEmpty(BirthPlace))
            {
                epErrorInput.SetError(tbBirthPlace, "Не указано место рождения");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();

            if (PassportDate.HasValue && (PassportDate.Value > DateTime.Now || PassportDate.Value.Year < 1970))
            {
                epErrorInput.SetError(dtPassportDate, "Неправильно указана дата");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();

            if (!PassportTypeId.HasValue)
            {
                epErrorInput.SetError(cbPassportType, "Не указан тип паспорта абитуриента");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();

            if (PassportTypeId == MainClass.pasptypeRFId)
            {
                if (!(PassportSeries.Length == 4))
                {
                    epErrorInput.SetError(tbPassportSeries, "Неправильно введена серия паспорта абитуриента");
                    tabCard.SelectedIndex = 0;
                    return false;
                }
                else
                    epErrorInput.Clear();

                if (!(PassportNumber.Length == 6))
                {
                    epErrorInput.SetError(tbPassportNumber, "Неправильно введен номер паспорта абитуриента");
                    tabCard.SelectedIndex = 0;
                    return false;
                }
                else
                    epErrorInput.Clear();
            }

            if (NationalityId == MainClass.countryRussiaId)
            {

                if (PassportSeries.Length <= 0)
                {
                    epErrorInput.SetError(tbPassportSeries, "Отсутствует серия паспорта абитуриента");
                    tabCard.SelectedIndex = 0;
                    return false;
                }
                else
                    epErrorInput.Clear();

                if (PassportNumber.Length <= 0)
                {
                    epErrorInput.SetError(tbPassportNumber, "Отсутствует номер паспорта абитуриента");
                    tabCard.SelectedIndex = 0;
                    return false;
                }
                else
                    epErrorInput.Clear();
            }
            else
            {
                if (PassportTypeId == 1 || PassportTypeId == 2 || PassportTypeId == 4)
                {
                    epErrorInput.SetError(cbPassportType, "Для указанного типа документа должно быть российское гражданство");
                    tabCard.SelectedIndex = 0;
                    return false;
                }
                else
                    epErrorInput.Clear();
            }

            if (!NationalityId.HasValue)
            {
                epErrorInput.SetError(cbNationality, "Не указано гражданство!");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();

            if (!CountryId.HasValue)
            {
                epErrorInput.SetError(cbCountry, "Не указана страна проживания!");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();

            if (!RegionId.HasValue)
            {
                epErrorInput.SetError(cbRegion, "Не указан регион!");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();
              
            if (PassportSeries.Length > 10)
            {
                epErrorInput.SetError(tbPassportSeries, "Слишком длинное значение серии паспорта абитуриента");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();
           

            if (PassportNumber.Length > 20)
            {
                epErrorInput.SetError(tbPassportNumber, "Слишком длинное значение номера паспорта абитуриента");
                tabCard.SelectedIndex = 0;
                return false;
            }
            else
                epErrorInput.Clear();

            if (Email.Length <= 0)
            {
                epErrorInput.SetError(tbEmail, "Отсутствует Email абитуриента");
                tabCard.SelectedIndex = 1;
                return false;
            }
            else
                epErrorInput.Clear();

            if (!chbHostelAbitYes.Checked && !chbHostelAbitNo.Checked)
            {
                epErrorInput.SetError(chbHostelAbitNo, "Не указаны данные о предоставлении общежития");
                tabCard.SelectedIndex = 1;
                return false;
            }
            else
                epErrorInput.Clear();

            if (!chbHostelEducYes.Checked && !chbHostelEducNo.Checked)
            {
                epErrorInput.SetError(chbHostelEducNo, "Не указаны данные о предоставлении общежития");
                tabCard.SelectedIndex = 1;
                return false;
            }
            else
                epErrorInput.Clear();

            if (chbExtPoss.Checked && !ExtPossId.HasValue)
            {
                epErrorInput.SetError(cbExtPoss, "Не указана категория лиц с ОВЗ!");
                if (MainClass.dbType == PriemType.Priem)
                    tabCard.SelectedIndex = 5;
                else
                    tabCard.SelectedIndex = 3;

                return false;
            }
            else
                epErrorInput.Clear();

            if (!CheckEducationInfoFields())
            {
                //если нет док-тов об образовании, то пропускать ошибку
                if (lstEducationInfo != null)
                    return false;
            }

            if (!CheckIdent())
            {
                WinFormsServ.Error("В базе уже существует абитуриент с такими же либо ФИО, либо данными паспорта, либо данными аттестата!");
                return false;
            }

            return true;
        }
        private bool CheckEducationInfoFields()
        {
            if (!SchoolTypeId.HasValue)
            {
                epErrorInput.SetError(cbSchoolType, "Не указан тип образовательного учреждения!");
                tabCard.SelectedIndex = 2;
                return false;
            }
            else
                epErrorInput.Clear();

            if (string.IsNullOrEmpty(tbSchoolExitYear.Text.Trim()))
            {
                epErrorInput.SetError(tbSchoolExitYear, "Не указан год");
                tabCard.SelectedIndex = 2;
                return false;
            }
            else
                epErrorInput.Clear();

            if (!Regex.IsMatch(SchoolExitYear.ToString(), @"^\d{0,4}$"))
            {
                epErrorInput.SetError(tbSchoolExitYear, "Неправильно указан год");
                tabCard.SelectedIndex = 2;
                return false;
            }
            else
                epErrorInput.Clear();

            if (gbAtt.Visible && AttestatNum.Length <= 0 && MainClass.dbType != PriemType.PriemAG)
            {
                epErrorInput.SetError(tbAttestatNum, "Отсутствует номер аттестата абитуриента");
                tabCard.SelectedIndex = 2;
                return false;
            }
            else
                epErrorInput.Clear();

            double d = 0;
            if (tbSchoolAVG.Text.Trim() != "")
            {
                if (!double.TryParse(tbSchoolAVG.Text.Trim().Replace(".", ","), out d))
                {
                    epErrorInput.SetError(tbSchoolAVG, "Неправильный формат");
                    tabCard.SelectedIndex = 2;
                    return false;
                }
                else
                    epErrorInput.Clear();
            }

            return true;
        }

        protected override void InsertRec(PriemEntities context, ObjectParameter idParam)
        {
            bool HasTRKI = false;
            string TRKICertificateNumber = "";

            context.Person_insert(personBarc, PersonName, SecondName, Surname, BirthDate, BirthPlace, PassportTypeId, PassportSeries, PassportNumber,
                PassportAuthor, PassportDate, Sex, CountryId, NationalityId, RegionId, Phone, Mobiles, Email,
                Code, City, Street, House, Korpus, Flat, CodeReal, CityReal, StreetReal, HouseReal, KorpusReal, FlatReal, KladrCode, HostelAbit, HostelEduc, HasAssignToHostel,
                HostelFacultyId, HasExamPass, ExamPassFacultyId, LanguageId, Stag, WorkPlace, MSVuz, MSCourse, MSStudyFormId, Privileges, PassportCode,
                PersonalCode, PersonInfo, ExtraInfo, ScienceWork, StartEnglish, EnglishMark, EgeInSpbgu, SNILS, HasTRKI, TRKICertificateNumber, idParam);

            if (MainClass.dbType == PriemType.PriemForeigners)
            {
                Guid PersonId = (Guid)idParam.Value;
                context.Person_UpdateForeignNationality(ForeignCountryId, ForeignNationalityId, PersonId);
            }

            _Id = idParam.Value.ToString();

            SaveCurrentEducationInfo();
            SavePersonParents();
            SaveSportQualification();
            SaveEgeManualExams();
        }
        protected override void UpdateRec(PriemEntities context, Guid id)
        {
            context.Person_UpdateWithoutMain(BirthPlace, Sex, CountryId, ForeignCountryId, NationalityId, ForeignNationalityId, RegionId, Phone, Mobiles, Email,
                Code, City, Street, House, Korpus, Flat, CodeReal, CityReal, StreetReal, HouseReal, KorpusReal, FlatReal, KladrCode, HostelAbit, HostelEduc, HasAssignToHostel,
                HostelFacultyId, HasExamPass, ExamPassFacultyId, LanguageId, Stag, WorkPlace, MSVuz, MSCourse, MSStudyFormId, PassportCode,
                PersonalCode, PersonInfo, ExtraInfo, ScienceWork, StartEnglish, EnglishMark, EgeInSpbgu, id);

            if (MainClass.RightsSov_SovMain_FacMain() || MainClass.IsPasha())
                context.Person_UpdateMain(PersonName, SecondName, Surname, BirthDate, PassportTypeId, PassportSeries, PassportNumber,
                PassportAuthor, PassportDate, Privileges, SNILS, id);

            SaveCurrentEducationInfo();
            SavePersonParents();
            SaveSportQualification();
            SaveEgeManualExams();
        }
                 
        protected override void OnSave()
        {
            CardTitle = Util.GetFIO(Surname, PersonName, SecondName);          
            btnAddAbit.Enabled = true;
           
            MainClass.DataRefresh();
        }
        protected override void OnSaveNew()
        {
            using (PriemEntities context = new PriemEntities())
            {
                string num = (from pr in context.extPerson
                             where pr.Id == GuidId
                             select pr.PersonNum).FirstOrDefault().ToString();

                tbNum.Text = num;
            }
        }

        #endregion 

        private void tabCard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.D1)
                this.tabCard.SelectedIndex = 0;
            if (e.Control && e.KeyCode == Keys.D2)
                this.tabCard.SelectedIndex = 1;
            if (e.Control && e.KeyCode == Keys.D3)
                this.tabCard.SelectedIndex = 2;
            if (e.Control && e.KeyCode == Keys.D4)
                this.tabCard.SelectedIndex = 3;
            if (e.Control && e.KeyCode == Keys.D5)
                this.tabCard.SelectedIndex = 4;
            if (e.Control && e.KeyCode == Keys.D6)
                this.tabCard.SelectedIndex = 5;

            if (e.Control && e.KeyCode == Keys.S)
                SaveClick();                
            if (e.Control && e.KeyCode == Keys.N)
                AddAbitClick();
        }
        private void CardPerson_Click(object sender, EventArgs e)
        {
            this.Activate();
        }

        #region Abit

        private void dgvApplications_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                OpenCardAbit();
        }
        private void OpenCardAbit()
        {
            if (dgvApplications.CurrentCell != null && dgvApplications.CurrentCell.RowIndex > -1)
            {
                string abId = dgvApplications.Rows[dgvApplications.CurrentCell.RowIndex].Cells["Id"].Value.ToString();
                if (abId != "")
                {
                    MainClass.OpenCardAbit(abId, this, dgvApplications.CurrentRow.Index);
                }
            }
        }
        private void AddAbitClick()
        {
            if (btnAddAbit.Visible && btnAddAbit.Enabled)
            {
                CardAbit crd = new CardAbit(GuidId);
                crd.Show();
            }
        }
        private void btnAddAbit_Click(object sender, EventArgs e)
        {
            AddAbitClick();
        }

        #endregion

        private void btnAttMarks_Click(object sender, EventArgs e)
        {
            CardAttMarks am;
            if (CheckCardForNewAndSave())
            {
                am = new CardAttMarks(GuidId, !_isModified);
                am.ShowDialog();
            }
        }
        
        // Грид ЕГЭ
        #region EGE

        private async void FillEgeMarks()
        {
            if (MainClass.dbType == PriemType.PriemMag || MainClass.dbType == PriemType.PriemAspirant || MainClass.dbType == PriemType.PriemForeigners)
                return;

            try
            {
                gbEgeLoading.Visible = true;
                DataTable examTable = await Task.Run<DataTable>(() => GetEgeExamTable());

                DataView dv = new DataView(examTable);
                dv.AllowNew = false;
                dgvExams.DataSource = examTable;
                dgvExams.DataBindingComplete += ((sender, e) => UpdateDGVExam());
                dgvExams.Update();
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка заполения грида Ege: ", exc);
            }
            finally
            {
                gbEgeLoading.Visible = false;
            }
        }

        private DataTable GetEgeExamTable()
        {
            using (PriemEntities context = new PriemEntities())
            {
                //заполнение грида с оценками
                DataTable examTable = new DataTable();

                DataColumn clm;
                clm = new DataColumn();
                clm.ColumnName = "Экзамен";
                clm.ReadOnly = true;
                examTable.Columns.Add(clm);

                clm = new DataColumn();
                clm.ColumnName = "ExamId";
                clm.ReadOnly = true;
                clm.DataType = typeof(int);
                examTable.Columns.Add(clm);

                clm = new DataColumn();
                clm.ColumnName = "Баллы";
                clm.DataType = typeof(int);
                examTable.Columns.Add(clm);

                clm = new DataColumn();
                clm.ColumnName = "Зачетная";
                clm.DataType = typeof(bool);
                examTable.Columns.Add(clm);

                clm = new DataColumn();
                clm.ColumnName = "Номер_сертификата";
                examTable.Columns.Add(clm);

                var examNames =
                    from en in context.EgeExamName
                    select new { en.Id, en.Name };

                foreach (var eName in examNames)
                {
                    DataRow newRow;
                    newRow = examTable.NewRow();
                    newRow["Экзамен"] = eName.Name;
                    newRow["ExamId"] = eName.Id;
                    examTable.Rows.Add(newRow);
                }

                // оценки
                var egeMarks =
                    from em in context.extEgeMarkMax
                    where em.PersonId == GuidId
                    select new { em.EgeExamNameId, em.Value, em.Number, em.IsCurrent };

                foreach (var eMark in egeMarks)
                {
                    for (int i = 0; i < examTable.Rows.Count; i++)
                    {
                        if (examTable.Rows[i]["ExamId"].ToString() == eMark.EgeExamNameId.ToString())
                        {
                            examTable.Rows[i]["Баллы"] = eMark.Value;
                            examTable.Rows[i]["Номер_сертификата"] = eMark.Number;
                            examTable.Rows[i]["Зачетная"] = eMark.IsCurrent;
                        }
                    }
                }

                return examTable;
            }
        }

        private void UpdateDGVExam()
        {
            dgvExams.Columns["Баллы"].ValueType = typeof(int);
            dgvExams.Columns["ExamId"].Visible = false;
            dgvExams.ReadOnly = true;
        }
        private void UpdateDataGridEge()
        {
            if (MainClass.dbType == PriemType.PriemMag || MainClass.dbType == PriemType.PriemAspirant || MainClass.dbType == PriemType.PriemForeigners)
                return;

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    var source = from ec in context.EgeCertificate
                                 where ec.PersonId == GuidId
                                 select new
                                 {
                                     ec.Id,
                                     ec.Number
                                 };

                    dgvEGE.DataSource = Converter.ConvertToDataTable(source.ToArray());
                    dgvEGE.DataBindingComplete += ((sender, e) =>
                    {
                        dgvEGE.Columns["Id"].Visible = false;
                        dgvEGE.Columns["Number"].HeaderText = "Номер_сертификата";
                        dgvEGE.Columns["Number"].Width = 110;

                        //после биндинга заполняет оценками
                        FillEgeMarks();
                    });

                    btnCardE.Enabled = dgvEGE.RowCount != 0;
                    if (MainClass.RightsSov_SovMain_FacMain())
                        btnRemoveE.Enabled = dgvEGE.RowCount != 0;

                    FBSStatus = GetFBSStatus(GuidId);

                    //закомментил, т.к. по-видимому отрабатывает ещё до окончания биндинга
                    //FillEgeMarks();
                }
            }
            catch(Exception exc)
            {
                WinFormsServ.Error("Ошибка заполения грида Ege: ", exc);
            }
        }

        private void btnAddE_Click(object sender, EventArgs e)
        {
            EgeCard crd;
            if (CheckCardForNewAndSave())
            {
                crd = new EgeCard(GuidId);
                crd.ToUpdateList += UpdateDataGridEge;
                crd.ShowDialog();
            }
        }
        private void btnCardE_Click(object sender, EventArgs e)
        {
            OpenCardEge();
        }
        private void dgvEGE_DoubleClick(object sender, EventArgs e)
        {
            OpenCardEge();
        }

        private void OpenCardEge()
        {
            if (dgvEGE.CurrentCell != null && dgvEGE.CurrentCell.RowIndex > -1)
            {
                string egeId = dgvEGE.Rows[dgvEGE.CurrentCell.RowIndex].Cells["Id"].Value.ToString();
                if (egeId != "")
                {
                    EgeCard crd = new EgeCard(egeId, GuidId, GetReadOnlyEge());
                    crd.ToUpdateList += UpdateDataGridEge;
                    crd.ShowDialog();
                }
            }
        }
        private bool GetReadOnlyEge()
        {
            if (!_isModified)
                return true;            
            
            if (inEnableProtocol && MainClass.RightsFaculty())
                return true;

            if (inEntryView)
                return true;

            //// закрываем уже всем на изменение кроме ограниченного набора         
            //if (!isMedCollOnly && !_bdc.RightsPashaSuper())
            //    return true;
                        
            return false;
        }

        private void btnRemoveE_Click(object sender, EventArgs e)
        {
            if (IsForReadOnly())
                return;
            
            if (MessageBox.Show("Удалить запись?", "Удаление", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Guid certId = (Guid)dgvEGE.CurrentRow.Cells["Id"].Value;
                try
                {
                    using (PriemEntities context = new PriemEntities())
                    {
                        context.EgeCertificate_Delete(certId);
                    }                    
                }
                catch (Exception ex)
                {
                    WinFormsServ.Error("Ошибка удаления данных", ex);
                }

                UpdateDataGridEge();
            }
        }
        private void btnSetStatusPasha_Click(object sender, EventArgs e)
        {
            if (MainClass.IsPasha() || MainClass.IsOwner())
            {
                if (_Id == null)
                    return;

                if (dgvEGE.SelectedCells.Count == 0)
                {
                    WinFormsServ.Error("Нужно выбрать сертификат");
                    return;
                }

                int rwInd = dgvEGE.SelectedCells[0].RowIndex;
                Guid egeCertId = (Guid)dgvEGE["Id", rwInd].Value;

                using (PriemEntities context = new PriemEntities())
                {
                    var cert = (from ec in context.EgeCertificate
                                where ec.PersonId == GuidId && ec.Id == egeCertId && (ec.FBSStatusId == 0 || ec.FBSStatusId == 2)
                                select ec).FirstOrDefault();

                    if (cert != null)
                    {
                        if (MessageBox.Show(string.Format("Проставить статус 'Проверено' для свидетельства {0}?", cert.Number), "Внимание", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            context.EgeCertificate_UpdateFBSStatus(4, tbCommentFBSPasha.Text.Trim(), cert.Id);
                            MessageBox.Show("Выполнено");
                            FBSStatus = GetFBSStatus(GuidId);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Нет свидетельств, удовлетворяющих критериям");
                    }
                }
            }
        }
        #endregion
        
        private void btnDocs_Click(object sender, EventArgs e)
        {
            if (_Id == null)
                return;

            if (personBarc == 0)
            {
                MessageBox.Show("Даная анкета была заведена вручную");
                return;
            }

            if(personBarc != null)
                new DocCard(personBarc.Value, null, true, MainClass.dbType == PriemType.PriemForeigners || MainClass.dbType == PriemType.PriemAG).Show();
        }

        #region Print

        private void btnGetAssignToHostel_Click(object sender, EventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                if (_Id == null)
                    return;

                if (HasAssignToHostel.Value)
                    return;

                int facId = MainClass.GetFacultyIdInt();

                string facName = (from fac in context.qFaculty
                                  where fac.Id == facId
                                  select fac.Name).FirstOrDefault();
               
                if (MessageBox.Show(string.Format("Будет выдано направление на поселение. Факультет: {0}, продолжить?", facName), "Сохранить", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    context.Person_UpdateHostel(true, facId, GuidId);                   
                    HasAssignToHostel = true;
                    HostelFacultyId = facId;

                    btnGetAssignToHostel.Enabled = false;

                    if (MainClass.RightsFacMain())
                        btnPrintHostel.Enabled = true;
                    
                    btnPrintHostel_Click(null, null);
                }
            }
        }
        private void btnGetExamPass_Click(object sender, EventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                if (_Id == null)
                    return;

                if (HasExamPass.Value)
                    return;

                int facId = MainClass.GetFacultyIdInt();

                string facName = (from fac in context.qFaculty
                                  where fac.Id == facId
                                  select fac.Name).FirstOrDefault();                
               
                if (MessageBox.Show(string.Format("Будет выдан экзаменационный пропуск. Факультет: {0}, продолжить?", facName), "Сохранить", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {                    
                    context.Person_UpdateExamPass(true, facId, GuidId);                   
                    HasExamPass = true;
                    ExamPassFacultyId = facId;

                    btnGetExamPass.Enabled = false;

                    if (MainClass.RightsFacMain())
                        btnPrintExamPass.Enabled = true;
                    btnPrintExamPass_Click(null, null);                    
                }
            }
        }      

        private void btnPrintHostel_Click(object sender, EventArgs e)
        {
            if (_Id == null)
                return;

            sfdPrint.FileName = string.Format("{0} - направление на поселение.pdf", tbSurname.Text);
            sfdPrint.Filter = "ADOBE Pdf files|*.pdf";
            if (sfdPrint.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Print.PrintHostelDirection(GuidId, chbPrint.Checked, sfdPrint.FileName);
        }
        private void btnPrintExamPass_Click(object sender, EventArgs e)
        {
            sfdPrint.FileName = string.Format("{0} - ЭкзПропуск.pdf", tbSurname.Text);
            sfdPrint.Filter = "ADOBE Pdf files|*.pdf";
            if (sfdPrint.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Print.PrintExamPass(GuidId, sfdPrint.FileName, chbPrint.Checked);
        }

        private void btnPrintApplication_Click(object sender, EventArgs e)
        {
            if (!GuidId.HasValue)
            {
                WinFormsServ.Error("Сохраните сперва карточку!");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = (Surname + " " ?? "") + (PersonName + " " ?? "") + (SecondName ?? "") + " - Заявление.pdf";
            sfd.Filter = "ADOBE Pdf files|*.pdf";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Print.PrintApplication(chbApplicationPrint.Checked, sfd.FileName, GuidId.Value);
            }
        }
        #endregion

        #region Benefits
        private void btnAddBenefitDocument_Click(object sender, EventArgs e)
        {
            if (CheckCardForNewAndSave())
            {
                var crd = new CardBenefintDocument_Select(GuidId.Value);
                crd.ToUpdateList += UpdateGridBenefits;
                crd.Show();
            }
        }
        private void UpdateGridBenefits()
        {
            if (_Id == null)
                return;
            using (PriemEntities context = new PriemEntities())
            {
                var src =
                    (from PBD in context.PersonBenefitDocument
                     where PBD.PersonId == GuidId
                     select new
                     {
                         Id = PBD.Id,
                         TYPE = PBD.BenefitDocumentTypeId.ToString(),
                         BenefitDocument = PBD.BenefitDocument.Name,
                         PBD.Series,
                         PBD.Number,
                         PBD.HasOriginals
                     }).ToList().
                     Select(x => new
                     {
                         x.Id,
                         x.TYPE,
                         BenefitDocument = x.BenefitDocument,
                         DocumentSeries = x.Series,
                         DocumentNumber = x.Number,
                         HasOriginals = x.HasOriginals ? "да" : "нет"
                     });

                var srcOl = 
                    (from OL in context.Olympiads
                     where OL.PersonId == GuidId
                     select new
                     {
                         Id = OL.Id,
                         TYPE = "OLYMP",
                         BenefitDocument = OL.OlympName.Name + " (" + OL.OlympSubject.Name + ") - " + OL.OlympValue.Name,
                         OL.DocumentSeries,
                         OL.DocumentNumber,
                         HasOriginals = OL.OriginDoc
                     }).ToList().
                     Select(x => new
                     {
                         x.Id,
                         x.TYPE,
                         BenefitDocument = x.BenefitDocument,
                         x.DocumentSeries,
                         x.DocumentNumber,
                         HasOriginals = x.HasOriginals ? "да" : "нет"
                     });

                dgvBenefitDocument.DataSource = Converter.ConvertToDataTable(src.Union(srcOl).ToArray());
            }

            dgvBenefitDocument.Columns["Id"].Visible = false;
            dgvBenefitDocument.Columns["TYPE"].Visible = false;
            dgvBenefitDocument.Columns["BenefitDocument"].HeaderText = "Документ";
            dgvBenefitDocument.Columns["BenefitDocument"].Width = 350;
            dgvBenefitDocument.Columns["DocumentSeries"].HeaderText = "Серия";
            dgvBenefitDocument.Columns["DocumentSeries"].Width = 70;
            dgvBenefitDocument.Columns["DocumentNumber"].HeaderText = "Номер";
            dgvBenefitDocument.Columns["DocumentNumber"].Width = 70;
            dgvBenefitDocument.Columns["HasOriginals"].HeaderText = "Оригиналы";
            dgvBenefitDocument.Columns["HasOriginals"].Width = 70;
        }
        private void btnDeleteBenefitDocument_Click(object sender, EventArgs e)
        {
            if (dgvBenefitDocument.SelectedCells.Count == 0)
                return;

            int rwInd = dgvBenefitDocument.SelectedCells[0].RowIndex;
            if (MessageBox.Show("Удалить запись?", "Внимание", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                
                using (PriemEntities context = new PriemEntities())
                {
                    Guid gId = (Guid)dgvBenefitDocument["Id", rwInd].Value;
                    string sTYPE = dgvBenefitDocument["TYPE", rwInd].Value.ToString();
                    if (sTYPE.Equals("Olymp", StringComparison.OrdinalIgnoreCase))
                        context.Olympiads_Delete(gId);
                    else
                        context.PersonBenefitDocument_delete(gId);
                }
                UpdateGridBenefits();
            }
        }
        private void btnOpenCardBenefitDocument_Click(object sender, EventArgs e)
        {
            OpenCardBenefits();
        }
        private void dgvBenefitDocument_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            OpenCardBenefits();
        }
        private void OpenCardBenefits()
        {
            if (dgvBenefitDocument.CurrentCell != null)
            {
                int rwInd = dgvBenefitDocument.CurrentCell.RowIndex;
                string sId = dgvBenefitDocument["Id", rwInd].Value.ToString();
                string sTYPE = dgvBenefitDocument["TYPE", rwInd].Value.ToString();
                if (!string.IsNullOrEmpty(sId))
                {
                    BookCard crd;
                    int tmp;
                    if (int.TryParse(sTYPE, out tmp))
                    {
                        if (tmp == 1)
                            crd = new CardDisabilityDocument(sId, GuidId.Value);
                        else
                            crd = new CardBenefitDocument(sId, GuidId.Value, tmp);
                    }
                    else
                        crd = new OlympCard(sId, GuidId, _isModified);

                    crd.ToUpdateList += UpdateGridBenefits;
                    crd.Show();
                }
            }
        }

        #endregion

        #region EducationInfo

        private void dgvEducationDocuments_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvEducationDocuments.CurrentRow != null)
                if (dgvEducationDocuments.CurrentRow.Index != _currentEducRow)
                {
                    _currentEducRow = dgvEducationDocuments.CurrentRow.Index;
                    SaveCurrentEducationInfo();
                    ViewEducationInfo(lstEducationInfo[_currentEducRow].Id);
                }
        }

        private void UpdateEducationData()
        {
            FillEducationData(PersonDataProvider.GetPersonEducationDocumentsById(GuidId.Value));
        }
        private void FillEducationData(List<Person_EducationInfo> lstVals)
        {
            lstEducationInfo = lstVals;

            dgvEducationDocuments.DataSource = lstVals.Select(x => new
            {
                x.Id,
                School = x.SchoolName,
                Series = (x.SchoolTypeId == 1 ? x.AttestatSeries : x.DiplomSeries),
                Num = x.SchoolTypeId == 1 ? x.AttestatNum : x.DiplomNum,
            }).ToList();

            dgvEducationDocuments.Columns["Id"].Visible = false;
            dgvEducationDocuments.Columns["School"].HeaderText = "Уч. учреждение";
            dgvEducationDocuments.Columns["School"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvEducationDocuments.Columns["Series"].HeaderText = "Серия";
            dgvEducationDocuments.Columns["Series"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dgvEducationDocuments.Columns["Num"].HeaderText = "Номер";
            dgvEducationDocuments.Columns["Num"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            if (lstVals.Count > 0)
                ViewEducationInfo(lstVals.First().Id);

            _currentEducRow = 0;
        }
        private void ViewEducationInfo(int id)
        {
            int ind = lstEducationInfo.FindIndex(x => x.Id == id);

            if (id == 0)
            {
                tbAttestatNum.Enabled = true;
                cbAttestatSeries.Enabled = true;
                tbSchoolAVG.Enabled = true;
                tbDiplomSeries.Enabled = true;
                tbDiplomNum.Enabled = true;
            }
            else
            {
                UpdateEducationInfoFieldsForEditing();
            }

            CountryEducId = lstEducationInfo[ind].CountryEducId;
            if (MainClass.dbType == PriemType.PriemForeigners)
                ForeignCountryEducId = lstEducationInfo[ind].ForeignCountryEducId;
            UpdateAfterCountryEduc();

            RegionEducId = lstEducationInfo[ind].RegionEducId;
            CurrEducationId = lstEducationInfo[ind].Id;
            
            IsEqual = lstEducationInfo[ind].IsEqual;
            EqualDocumentNumber = lstEducationInfo[ind].EqualDocumentNumber;

            SchoolTypeId = lstEducationInfo[ind].SchoolTypeId;
            if (SchoolTypeId == 1)
            {
                gbAtt.Visible = true;
                gbDipl.Visible = false;
                AttestatSeries = lstEducationInfo[ind].AttestatSeries;
                AttestatNum = lstEducationInfo[ind].AttestatNum;

                pnExitClass.Visible = true;
                SchoolExitClassId = lstEducationInfo[ind].SchoolExitClassId;

                gbFinishStudy.Visible = false;
                lblSchoolNum.Visible = true;
                tbSchoolNum.Visible = true;
                btnAttMarks.Visible = true;
                chbIsExcellent.Text = "Медалист (отличник)";
            }
            else
            {
                gbAtt.Visible = false;
                gbDipl.Visible = true;
                DiplomSeries = lstEducationInfo[ind].DiplomSeries;
                DiplomNum = lstEducationInfo[ind].DiplomNum;

                pnExitClass.Visible = false;
                SchoolExitClassId = null;

                gbFinishStudy.Visible = true;
                lblSchoolNum.Visible = false;
                tbSchoolNum.Visible = false;
                btnAttMarks.Visible = false;
                chbIsExcellent.Text = "Диплом с отличием";

                HighEducation = lstEducationInfo[ind].HighEducation;
                HEProfession = lstEducationInfo[ind].HEProfession;
                HEQualification = lstEducationInfo[ind].HEQualification;
                HEEntryYear = lstEducationInfo[ind].HEEntryYear;
                HEExitYear = lstEducationInfo[ind].HEExitYear;
                HEWork = lstEducationInfo[ind].HEWork;
                HEStudyFormId = lstEducationInfo[ind].HEStudyFormId;
            }

            SchoolAVG = lstEducationInfo[ind].SchoolAVG;
            IsExcellent = lstEducationInfo[ind].IsExcellent;
            SchoolCity = lstEducationInfo[ind].SchoolCity;
            SchoolName = lstEducationInfo[ind].SchoolName;
            SchoolNum = lstEducationInfo[ind].SchoolNum;
            SchoolExitYear = lstEducationInfo[ind].SchoolExitYear;
        }

        private void btnAddEducDoc_Click(object sender, EventArgs e)
        {
            if (CheckCardForNewAndSave())
            {
                var EducInfo = new Person_EducationInfo();
                EducInfo.PersonId = GuidId.Value;
                EducInfo.CountryEducId = CountryId.Value;
                EducInfo.RegionEducId = RegionId.Value;

                if (lstEducationInfo == null)
                    lstEducationInfo = new List<Person_EducationInfo>();

                lstEducationInfo.Add(EducInfo);

                FillEducationData(lstEducationInfo);
            }
        }

        private void SaveCurrentEducationInfo()
        {
            if (_isModified && CheckEducationInfoFields())
            {
                if (lstEducationInfo == null)
                    lstEducationInfo = new List<Person_EducationInfo>();

                int ind = lstEducationInfo.FindIndex(x => x.Id == CurrEducationId);
                if (ind >= 0)
                {
                    lstEducationInfo[ind].SchoolTypeId = SchoolTypeId.Value;
                    lstEducationInfo[ind].SchoolCity = SchoolCity;
                    lstEducationInfo[ind].SchoolName = SchoolName;
                    lstEducationInfo[ind].SchoolNum = SchoolNum;
                    lstEducationInfo[ind].SchoolAVG = SchoolAVG;
                    lstEducationInfo[ind].SchoolExitYear = SchoolExitYear;
                    lstEducationInfo[ind].SchoolExitClassId = SchoolExitClassId;
                    lstEducationInfo[ind].CountryEducId = CountryEducId.Value;
                    lstEducationInfo[ind].ForeignCountryEducId = ForeignCountryEducId.Value;
                    lstEducationInfo[ind].RegionEducId = RegionEducId.Value;
                    lstEducationInfo[ind].AttestatSeries = AttestatSeries;
                    lstEducationInfo[ind].AttestatNum = AttestatNum;
                    lstEducationInfo[ind].DiplomSeries = DiplomSeries;
                    lstEducationInfo[ind].DiplomNum = DiplomNum;
                    lstEducationInfo[ind].HEEntryYear = HEEntryYear;
                    lstEducationInfo[ind].HEExitYear = HEExitYear;
                    lstEducationInfo[ind].HEProfession = HEProfession;
                    lstEducationInfo[ind].HEQualification = HEQualification;
                    lstEducationInfo[ind].HEStudyFormId = HEStudyFormId;
                    lstEducationInfo[ind].HEWork = HEWork;
                    lstEducationInfo[ind].HighEducation = HighEducation;
                    lstEducationInfo[ind].IsEqual = IsEqual;
                    lstEducationInfo[ind].IsExcellent = IsExcellent;
                    lstEducationInfo[ind].EqualDocumentNumber = EqualDocumentNumber;
                    PersonDataProvider.SaveEducationDocument(lstEducationInfo[ind], false);

                    dgvEducationDocuments["School", ind].Value = SchoolName;
                    dgvEducationDocuments["Series", ind].Value = SchoolTypeId.Value == 1 ? AttestatSeries : DiplomSeries;
                    dgvEducationDocuments["Num", ind].Value = SchoolTypeId.Value == 1 ? AttestatNum : DiplomNum;
                }
                else if (ind == -1)
                {
                    PersonDataProvider.SaveEducationDocument(
                    new Person_EducationInfo()
                    {
                        PersonId = GuidId.Value,
                        SchoolTypeId = SchoolTypeId.Value,
                        SchoolCity = SchoolCity,
                        SchoolName = SchoolName,
                        SchoolNum = SchoolNum,
                        SchoolAVG = SchoolAVG,
                        SchoolExitYear = SchoolExitYear,
                        SchoolExitClassId = SchoolExitClassId,
                        CountryEducId = CountryEducId.Value,
                        ForeignCountryEducId = ForeignCountryEducId.Value,
                        RegionEducId = RegionEducId.Value,
                        AttestatSeries = AttestatSeries,
                        AttestatNum = AttestatNum,
                        DiplomSeries = DiplomSeries,
                        DiplomNum = DiplomNum,
                        HEEntryYear = HEEntryYear,
                        HEExitYear = HEExitYear,
                        HEProfession = HEProfession,
                        HEQualification = HEQualification,
                        HEStudyFormId = HEStudyFormId,
                        HEWork = HEWork,
                        HighEducation = HighEducation,
                        IsEqual = IsEqual,
                        IsExcellent = IsExcellent,
                        EqualDocumentNumber = EqualDocumentNumber,
                    }, true);

                }
            }
        }

        private void btnDeleteEducDoc_Click(object sender, EventArgs e)
        {
            if (dgvEducationDocuments.CurrentRow != null)
            {
                int _currentEducRow = dgvEducationDocuments.CurrentRow.Index;

                string message = "Вы хотите удалить выбранный документ об образовании?";

                if (MessageBox.Show(message, "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    PersonDataProvider.DeletePersonEducationDocument(GuidId.Value, lstEducationInfo[_currentEducRow].Id);
                    UpdateEducationData();
                }
            }
        }

        private void cbCountryEduc_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAfterCountryEduc();
        }
        private void UpdateAfterCountryEduc()
        {
            //если используется иностр приём, то следует обновить значение CountryId
            if (MainClass.dbType == PriemType.PriemForeigners)
            {
                using (PriemEntities context = new PriemEntities())
                {
                    CountryEducId = context.ForeignCountry.Where(x => x.Id == ForeignCountryEducId)
                        .Select(x => x.PriemDictionaryId)
                        .DefaultIfEmpty(MainClass.countryRussiaId)
                        .First();
                }
            }
            else
            {
                using (PriemEntities context = new PriemEntities())
                {
                    ForeignCountryEducId = context.ForeignCountry.Where(x => x.PriemDictionaryId == CountryEducId)
                        .Select(x => x.Id)
                        .DefaultIfEmpty(MainClass.countryRussiaId)
                        .First();
                }
            }

            tbEqualityDocumentNumber.Visible = CountryEducId != MainClass.countryRussiaId;
            chbEkvivEduc.Visible = CountryEducId != MainClass.countryRussiaId;
            cbRegion.Enabled = CountryEducId != MainClass.countryRussiaId;

            if (CountryEducId.HasValue)
                ComboServ.FillCombo(cbRegionEduc, CommonDataProvider.GetRegionListForCountryId(CountryEducId.Value), false, false);

            if (CountryEducId != MainClass.countryRussiaId)
            {
                try
                {
                    //Region fix
                    int iRegionEducId = CommonDataProvider.GetRegionIdForCountryId(CountryEducId.Value);
                    if (iRegionEducId != 0)
                        RegionEducId = iRegionEducId;
                }
                catch { }
            }
        }

        #endregion

        private void cbNationality_SelectedIndexChanged(object sender, EventArgs e)
        {
            //если используется иностр приём, то следует обновить значение NationalityId
            if (MainClass.dbType == PriemType.PriemForeigners)
            {
                using (PriemEntities context = new PriemEntities())
                {
                    NationalityId = context.ForeignCountry.Where(x => x.Id == ForeignNationalityId)
                        .Select(x => x.PriemDictionaryId)
                        .DefaultIfEmpty(MainClass.countryRussiaId)
                        .First();
                }
            }
        }
        private void cbCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            //если используется иностр приём, то следует обновить значение CountryId
            if (MainClass.dbType == PriemType.PriemForeigners)
            {
                using (PriemEntities context = new PriemEntities())
                {
                    CountryId = context.ForeignCountry.Where(x => x.Id == ForeignCountryId)
                        .Select(x => x.PriemDictionaryId)
                        .DefaultIfEmpty(MainClass.countryRussiaId)
                        .First();
                }
            }
            UpdateAfterCountry();
        }
        private void UpdateAfterCountry()
        {
            if (CountryId.HasValue)
                ComboServ.FillCombo(cbRegion, CommonDataProvider.GetRegionListForCountryId(CountryId.Value), false, false);

            if (CountryId != MainClass.countryRussiaId)
            {
                try
                {
                    //Region fix
                    int iRegionId = CommonDataProvider.GetRegionIdForCountryId(CountryId.Value);
                    if (iRegionId != 0)
                        RegionId = iRegionId;
                }
                catch { }
            }
        }

        #region EgeRequest
        private void btnRequestEge_Click(object sender, EventArgs e)
        {
            if (!GuidId.HasValue)
                return;
            try
            {

                using (PriemEntities context = new PriemEntities())
                {
                    int reqId = context.PersonEgeRequest.Where(x => x.PersonId == GuidId.Value && !x.IsChecked).Select(x => x.IntId).DefaultIfEmpty(0).FirstOrDefault();
                    if (reqId != 0)
                        return;

                    ObjectParameter idParam = new ObjectParameter("id", typeof(int));
                    context.PersonEgeRequest_insert(GuidId.Value, idParam);

                    _currentEgeRequestId = (int)idParam.Value;
                    lblHasRequest.Visible = true;
                    lblHasRequestFinished.Visible = false;

                    bw_EgeRequestCheck.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error(ex);
            }
        }
        private BackgroundWorker bw_EgeRequestCheck;
        void bw_EgeRequestCheck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblHasRequest.Visible = false;
            lblHasRequestFinished.Visible = true;
            UpdateDataGridEge();
            UpdatePersonAchievement();
        }
        void bw_EgeRequestCheck_DoWork(object sender, DoWorkEventArgs e)
        {
            bool bFlag = true;
            using (PriemEntities context = new PriemEntities())
            {
                while (bFlag)
                {
                    if (e.Cancel)
                        return;

                    System.Threading.Thread.Sleep(1000);

                    if (context.PersonEgeRequest.Where(x => x.IntId == _currentEgeRequestId && x.IsChecked).Count() == 0)
                        continue;
                    else
                        return;
                }
            }
        }
        #endregion

        protected override void OnClosed()
        {
            base.OnClosed();
            try
            {
                bw_EgeRequestCheck.CancelAsync();
            }
            catch (Exception ex)
            {
                WinFormsServ.Error(ex);
            }
        }

        #region PersonAchievements
        private void btnAddPersonAchievement_Click(object sender, EventArgs e)
        {
            var crd = new CardPersonAchievement(_Id);
            crd.ToUpdateList += UpdatePersonAchievement;
            crd.Show();
        }
        private void btnDeletePersonAchievement_Click(object sender, EventArgs e)
        {
            if (dgvIndividualAchievements.SelectedCells.Count == 0)
                return;

            int rwInd = dgvIndividualAchievements.SelectedCells[0].RowIndex;
            Guid id = (Guid)dgvIndividualAchievements["Id", rwInd].Value;

            using (PriemEntities context = new PriemEntities())
            {
                string message = "Вы действительно хотите удалить указанное достижение?";
                if (MessageBox.Show(message, "Удаление", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    context.PersonAchievement_delete(id);
                    UpdatePersonAchievement();
                }
            }
        }
        private void dgvIndividualAchievements_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            string sId = dgvIndividualAchievements["Id", e.RowIndex].Value.ToString();

            var crd = new CardPersonAchievement(sId, e.RowIndex, this);
            crd.ToUpdateList += UpdatePersonAchievement;
            crd.Show();
        }
        private void UpdatePersonAchievement()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = context.PersonAchievement.Where(x => x.PersonId == GuidId).Select(x => new { x.Id, x.AchievementType.Name, x.AchievementType.Mark }).ToArray();
                dgvIndividualAchievements.DataSource = Converter.ConvertToDataTable(src);
                dgvIndividualAchievements.DataBindingComplete += ((e, sender) =>
                {
                    dgvIndividualAchievements.Columns["Id"].Visible = false;
                    dgvIndividualAchievements.Columns["Name"].HeaderText = "Достижение";
                    dgvIndividualAchievements.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dgvIndividualAchievements.Columns["Mark"].HeaderText = "Баллы";
                    dgvIndividualAchievements.Columns["Mark"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                });
            }
        }
        #endregion

        private void btnSendToOnline_Click(object sender, EventArgs e)
        {
            if (!MainClass.IsPasha())
            {
                WinFormsServ.Error("Только для администраторов. Извините.");
                return;
            }

            if (!GuidId.HasValue)
            {
                WinFormsServ.Error("Нет Id");
                return;
            }

            string msg = "Переместить абитуриента обратно в онлайн вместе со всеми его заявлениями?";
            if (MessageBox.Show(msg, "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    LoadFromInet loadClass = new LoadFromInet();
                    loadClass.SendPersonBackToOnline(GuidId.Value);
                    loadClass.CloseDB();
                    MessageBox.Show("Done");
                    this.Close();
                    MainClass.DataRefresh();
                }
                catch (Exception ex)
                {
                    WinFormsServ.Error("Ошибка при отправке абитуриента обратно в онлайн", ex);
                }
            }
        }
        private void btnOnlineEducLoad_Click(object sender, EventArgs e)
        {
            if (!personBarc.HasValue)
            {
                WinFormsServ.Error("Нет баркода для загрузки!");
                return;
            }

            if (lstEducationInfo != null)
            {
                WinFormsServ.Error("Документы об образовании уже есть!");
                return;
            }

            try
            {
                LoadFromInet load = new LoadFromInet();
                var docs = load.GetPersonEducationDocumentsByBarcode(personBarc.Value);
                foreach (var ED in docs)
                {
                    ED.PersonId = GuidId.Value;
                    PersonDataProvider.SaveEducationDocument(ED, true);
                }

                UpdateEducationData();

                MessageBox.Show("Done!");
            }
            catch (Exception ex)
            {
                WinFormsServ.Error(ex);
            }
        }
        private void btnOtherPassports_Click(object sender, EventArgs e)
        {
            if (!GuidId.HasValue)
                return;

            var crd = new OtherPersonPassportList(GuidId.Value);
            crd.Show();
        }

        private async void UpdateNoticies()
        {
            if (!GuidId.HasValue)
                return;

            using (PriemEntities context = new PriemEntities())
            {
                var Notes = await Task.Run(() =>
                {
                    return context.PersonNoticies.Where(x => x.PersonId == GuidId).Select(x => new { x.DateCreated, x.CreateAuthor, x.NoticeText }).ToList();
                });
                
                tbNotes.Text = "";

                if (Notes.Count > 0)
                {
                    tbNotes.Text = Notes.Select(x => "[" + x.DateCreated.ToShortDateString() + " " + x.DateCreated.ToShortTimeString() + "] "
                        + MainClass.GetADUserName(x.CreateAuthor) + " (" + MainClass.GetFacultyForAccount(x.CreateAuthor) + "):\r\n"
                        + x.NoticeText + "\r\n\r\n").Aggregate((x, tail) => x + tail);
                }
            }
        }
        private void btnAddNotice_Click(object sender, EventArgs e)
        {
            if (!GuidId.HasValue)
                return;

            if (string.IsNullOrEmpty(tbAddNotice.Text.Trim()))
            {
                UpdateNoticies();
                return;
            }

            using (PriemEntities context = new PriemEntities())
            {
                context.PersonNoticies_insert(GuidId, tbAddNotice.Text.Trim());
            }

            UpdateNoticies();
        }

        private void UpdateVedGrid()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src =
                    from VedHist in context.ExamsVedHistory
                    join Ved in context.ExamsVed on VedHist.ExamsVedId equals Ved.Id
                    join Fac in context.SP_Faculty on Ved.FacultyId equals Fac.Id
                    join Ex in context.Exam on Ved.ExamId equals Ex.Id
                    join ExName in context.ExamName on Ex.ExamNameId equals ExName.Id
                    where VedHist.PersonId == GuidId
                    select new
                    {
                        Ved.Id,
                        Fac = Fac.Name,
                        ExName.Name,
                        Ved.Number
                    };

                dgvVedList.DataSource = Converter.ConvertToDataTable(src.ToArray());
                dgvVedList.Columns["Id"].Visible = false;
                dgvVedList.Columns["Fac"].HeaderText = "Подразделение";
                dgvVedList.Columns["Name"].HeaderText = "Экзамен";
                dgvVedList.Columns["Number"].HeaderText = "Номер ведомости";
            }
        }

        private void btnCertificateAdd_Click(object sender, EventArgs e)
        {
            if (GuidId.HasValue)
                new CardLangCertificate(null, GuidId.Value, FillLanguageCertificates).Show();
        }

        private void btnCertificateDelete_Click(object sender, EventArgs e)
        {
            if (dgvCertificates.CurrentCell != null)
                if (dgvCertificates.CurrentCell.RowIndex > 0)
                {
                    if (MessageBox.Show("Удалить?", "Подтвердите удаление", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        using (PriemEntities context = new PriemEntities())
                        {
                            int Certid = int.Parse(dgvCertificates.CurrentRow.Cells["Id"].Value.ToString());
                            context.PersonLanguageCertificates.RemoveRange(context.PersonLanguageCertificates.Where(x => x.Id == Certid));
                            context.SaveChanges();
                        }
                        FillLanguageCertificates();
                    }
                }
        }

        private void dgvCertificates_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCertificates.CurrentCell != null)
                if (dgvCertificates.CurrentCell.RowIndex > 0)
                {
                    int Certid = int.Parse(dgvCertificates.CurrentRow.Cells["Id"].Value.ToString());
                    new CardLangCertificate(Certid, GuidId.Value, FillLanguageCertificates).Show();
                }
        }

        private void btnDocsList_Click(object sender, EventArgs e)
        { 
            if (!GuidId.HasValue)
            {
                WinFormsServ.Error("Сохраните сперва карточку!");
                return;
            } 
            new CardDocumentList(GuidId.Value).Show();
        }

        #region AdditionalInfo
 
        private void FillPersonParents()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var AddInfo = context.Person_AdditionalInfo.Where(x => x.PersonId == GuidId).FirstOrDefault();
                tbParent_Surname.Text =AddInfo.Parent_Surname;
                tbParent_Name.Text = AddInfo.Parent_Name;
                tbParent_SecondName.Text = AddInfo.Parent_SecondName;
                tbParent_Phone.Text = AddInfo.Parent_Phone;
                tbParent_Email.Text = AddInfo.Parent_Email;
                tbParent_WorkPlace.Text =AddInfo.Parent_Work;
                tbParent_WorkPosition.Text = AddInfo.Parent_WorkPosition;

                tbParent2_Surname.Text = AddInfo.Parent2_Surname;
                tbParent2_Name.Text = AddInfo.Parent2_Name;
                tbParent2_SecondName.Text = AddInfo.Parent2_SecondName;
                tbParent2_Phone.Text = AddInfo.Parent2_Phone;
                tbParent2_Email.Text = AddInfo.Parent2_Email;
                tbParent2_WorkPlace.Text = AddInfo.Parent2_Work;
                tbParent2_WorkPosition.Text = AddInfo.Parent2_WorkPosition;
            }
        }
        
        private void FillSportQulification()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var Sport = context.PersonSportQualification.Where(x => x.PersonId == GuidId).FirstOrDefault();
                if (Sport != null)
                {
                    tbSportLevel.Text = Sport.SportQualificationLevel;
                    tbSportQualification.Text = Sport.OtherSportQualification;
                    ComboServ.SetComboId(cbSportQualification, Sport.SportQualificationId);
                }
            }
        }
        
        int rowPersonScienceWork = -1;
        Guid? PersonScienceWorkId = null;
        private void dgvPersonScienceWork_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvPersonScienceWork.CurrentRow != null)
            {
                if (dgvPersonScienceWork.CurrentRow.Index != rowPersonScienceWork)
                {
                    rowPersonScienceWork = dgvPersonScienceWork.CurrentRow.Index;
                    PersonScienceWorkId = Guid.Parse(dgvPersonScienceWork.CurrentRow.Cells["id"].Value.ToString());

                    using (PriemEntities context = new PriemEntities())
                    {
                        var ScienceWork = context.PersonScienceWork.Where(x => x.Id == PersonScienceWorkId).FirstOrDefault();
                        tbScienceWork.Text = ScienceWork.WorkInfo;
                        tbScienceWorkYear.Text = ScienceWork.WorkYear;
                        ComboServ.SetComboId(cbScienceWorkType,  ScienceWork.WorkTypeId);
                        btnSaveScienceWork.Text = "Изменить";
                        ScienceWork_IsModified = false;
                        tbScienceWork.Enabled = tbScienceWorkYear.Enabled = cbScienceWorkType.Enabled = false;
                    }
                }
            }
        }

        bool ScienceWork_IsModified = false;
        private void btnSaveScienceWork_Click(object sender, EventArgs e)
        {
            if (!ScienceWork_IsModified)
            {
                btnSaveScienceWork.Text = PersonScienceWorkId.HasValue ? "Сохранить" : "Добавить";
                ScienceWork_IsModified = true;
                tbScienceWork.Enabled = tbScienceWorkYear.Enabled = cbScienceWorkType.Enabled = true;
            }
            else
            {
                btnSaveScienceWork.Text = "Изменить";
                ScienceWork_IsModified = false;
                tbScienceWork.Enabled = tbScienceWorkYear.Enabled = cbScienceWorkType.Enabled = false;
                if (GuidId.HasValue)
                    using (PriemEntities context = new PriemEntities())
                    {
                        bool bIsNew = false;
                        var ScW = context.PersonScienceWork.Where(x => x.Id == PersonScienceWorkId).FirstOrDefault();
                        if (ScW == null)
                        {
                            PersonScienceWorkId = Guid.NewGuid();
                            ScW = new PersonScienceWork();
                            ScW.PersonId = GuidId.Value;
                            ScW.Id = PersonScienceWorkId.Value;
                            bIsNew = true;
                        }
                        ScW.WorkTypeId = ComboServ.GetComboIdInt(cbScienceWorkType);
                        ScW.WorkInfo = tbScienceWork.Text.Trim();
                        ScW.WorkYear = tbScienceWorkYear.Text.Trim();
                        if (bIsNew)
                            context.PersonScienceWork.Add(ScW);
                        context.SaveChanges();
                    }
                else
                {
                    WinFormsServ.Error("Необходимо сохранить карточку");
                }
            }
        }

        bool Work_IsModified = false;
        private void btnWorkSave_Click(object sender, EventArgs e)
        {
            if (!Work_IsModified)
            {
                btnWorkSave.Text = PersonWorkId.HasValue ? "Сохранить" : "Добавить";
                Work_IsModified = true;
                tbWorkPlace.Enabled = tbWorkStag.Enabled = tbWorkProfession.Enabled = tbWorkSpecification.Enabled = true;
            }
            else
            {
                btnWorkSave.Text = "Изменить";
                Work_IsModified = false;
                tbWorkPlace.Enabled = tbWorkStag.Enabled = tbWorkProfession.Enabled = tbWorkSpecification.Enabled = false;

                if (GuidId.HasValue)
                    using (PriemEntities context = new PriemEntities())
                    {
                        bool bIsNew = false;
                        var ScW = context.PersonWork.Where(x => x.Id == PersonWorkId).FirstOrDefault();
                        if (ScW == null)
                        {
                            PersonWorkId = Guid.NewGuid();
                            ScW = new PersonWork();
                            ScW.PersonId = GuidId.Value;
                            ScW.Id = PersonWorkId.Value;
                            bIsNew = true;
                        }
                        ScW.WorkPlace = tbWorkPlace.Text.Trim();
                        ScW.WorkProfession = tbWorkProfession.Text.Trim();
                        ScW.Stage = tbWorkStag.Text.Trim();
                        ScW.WorkSpecifications = tbWorkSpecification.Text.Trim();
                        if (bIsNew)
                            context.PersonWork.Add(ScW);
                        context.SaveChanges();
                    }
                else
                {
                    WinFormsServ.Error("Необходимо сохранить карточку");
                }
            }
        }
       
        int rowPersonWork = -1;
        Guid? PersonWorkId = null;
        private void dgvPersonWork_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvPersonWork.CurrentRow != null)
            {
                if (dgvPersonWork.CurrentRow.Index != rowPersonWork)
                {
                    rowPersonWork = dgvPersonWork.CurrentRow.Index;
                    PersonWorkId = Guid.Parse(dgvPersonWork.CurrentRow.Cells["id"].Value.ToString());

                    using (PriemEntities context = new PriemEntities())
                    {
                        var Work = context.PersonWork.Where(x => x.Id == PersonWorkId).FirstOrDefault();
                        tbWorkPlace.Text = Work.WorkPlace;
                        tbWorkProfession.Text = Work.WorkProfession;
                        tbWorkStag.Text = Work.Stage;
                        tbWorkSpecification.Text = Work.WorkSpecifications;
                        btnWorkSave.Text = "Изменить";
                        Work_IsModified = false;
                        tbWorkPlace.Enabled = tbWorkStag.Enabled = tbWorkProfession.Enabled = tbWorkSpecification.Enabled = false;
                    }
                }
            }
        }

        private void SavePersonParents()
        {
            if (GuidId.HasValue)
            using (PriemEntities context = new PriemEntities())
            {
                var AddInfo = context.Person_AdditionalInfo.Where(x => x.PersonId == GuidId.Value).FirstOrDefault();
                if (AddInfo!=null)
                {
                    AddInfo.Parent_Surname = tbParent_Surname.Text.Trim();
                    AddInfo.Parent_Name = tbParent_Name.Text.Trim();
                    AddInfo.Parent_SecondName = tbParent_SecondName.Text.Trim();
                    AddInfo.Parent_Phone = tbParent_Phone.Text.Trim();
                    AddInfo.Parent_Email = tbParent_Email.Text.Trim();
                    AddInfo.Parent_Work = tbParent_WorkPlace.Text.Trim();
                    AddInfo.Parent_WorkPosition = tbParent_WorkPosition.Text.Trim();

                    AddInfo.Parent2_Surname = tbParent2_Surname.Text.Trim();
                    AddInfo.Parent2_Name = tbParent2_Name.Text.Trim();
                    AddInfo.Parent2_SecondName = tbParent2_SecondName.Text.Trim();
                    AddInfo.Parent2_Phone = tbParent2_Phone.Text.Trim();
                    AddInfo.Parent2_Email = tbParent2_Email.Text.Trim();
                    AddInfo.Parent2_Work = tbParent2_WorkPlace.Text.Trim();
                    AddInfo.Parent2_WorkPosition = tbParent2_WorkPosition.Text.Trim();

                    AddInfo.ReturnDocumentTypeId = ReturnDocumentTypeId;
                    AddInfo.ExtPossId = ExtPossId;

                    context.SaveChanges();
                }
            }
        }

        private void SaveSportQualification()
        {
            if (GuidId.HasValue)
                using (PriemEntities context = new PriemEntities())
                {
                    bool isNew = false;
                    var Sport = context.PersonSportQualification.Where(x => x.PersonId == GuidId).FirstOrDefault();
                    if (Sport == null)
                    {
                        Sport = new PersonSportQualification();
                        Sport.PersonId = GuidId.Value;
                        isNew = true;
                    }
                    Sport.SportQualificationId = ComboServ.GetComboIdInt(cbSportQualification);
                    Sport.SportQualificationLevel = tbSportLevel.Text.Trim();
                    Sport.OtherSportQualification = tbSportQualification.Text.Trim();

                    if (isNew)
                        context.PersonSportQualification.Add(Sport);
                    context.SaveChanges();
                }
        }
        private void SaveEgeManualExams()
        {
            if (GuidId.HasValue)
                using (PriemEntities context = new PriemEntities())
                {
                    bool? PassExamInSpbu = chbPassExamInSpbu.Checked; 
                    var AddInfo = context.Person_AdditionalInfo.Where(x => x.PersonId == GuidId).FirstOrDefault();
                    if (AddInfo != null)
                    {
                        AddInfo.PassExamInSpbu = PassExamInSpbu;
                        AddInfo.CategoryId = _CategoryId;
                    }
                    context.SaveChanges();
                    foreach (DataGridViewRow rw in dgvEgeManualExam.Rows)
                    {
                        int ExamId = (int)rw.Cells["ExamId"].Value;
                        if (context.PersonManualExams.Where(x=>x.PersonId == GuidId && ExamId == ExamId).Select(x=>x).Count() == 0)
                            context.PersonManualExams.Add(new PersonManualExams()
                            {
                                PersonId = GuidId,
                                ExamId = ExamId,
                            });
                    }
                    context.SaveChanges();
                }
        }
        
        private void cbSportQulification_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboServ.GetComboIdInt(cbSportQualification) == 44)
            {
                tbSportLevel.Text = "";
                tbSportLevel.ReadOnly = true;
                tbSportQualification.ReadOnly = false;
            }
            else if (ComboServ.GetComboIdInt(cbSportQualification) == 0)
            {
                tbSportLevel.Text = tbSportQualification.Text = "";
                tbSportQualification.ReadOnly = true;
                tbSportLevel.ReadOnly = true;
            }
            else
            {
                tbSportQualification.Text = "";
                tbSportQualification.ReadOnly = true;
                tbSportLevel.ReadOnly = false;
            }
        }
        #endregion

        private void chbExtPoss_CheckedChanged(object sender, EventArgs e)
        {
            cbExtPoss.Visible = chbExtPoss.Checked;
            //cbExtPossType.Enabled = chbExtPoss.Checked;
        }

        int? _CategoryId;
        Dictionary<RadioButton, int> RadioButtonCategory;
        private void FillCategoryForManualExam()
        {
            RadioButtonCategory = new Dictionary<RadioButton, int>();
            using (PriemEntities context = new PriemEntities())
            {
                var PassExamInSpbu = context.Person_AdditionalInfo.Where(x => x.PersonId == GuidId)
                        .Select(x => new { x.PassExamInSpbu, x.CategoryId }).FirstOrDefault();

                _CategoryId = PassExamInSpbu.CategoryId;
                chbPassExamInSpbu.Checked = PassExamInSpbu.PassExamInSpbu ?? false;

                var Cat = context.PersonCategoryForManualExams.ToList();
                Panel p = this.pnCategoryForManualExam;
                int Height = 3;
                const int max_char = 45;
                foreach (var cat in Cat)
                {
                    RadioButton b = new RadioButton();
                    b.AutoSize = true;
                    string oldname = cat.Name;

                    int len = 0;
                    int strcnt = 1;
                    string name = "";
                    while (oldname != "")
                    {
                        if (oldname.IndexOf(' ') > 0)
                        {
                            if ((len + oldname.IndexOf(' ')) > max_char)
                            {
                                name += '\n'; len = 0; strcnt++;
                            }
                            len += oldname.IndexOf(' ');
                            name += oldname.Substring(0, oldname.IndexOf(' ') + 1);
                            oldname = oldname.Substring(oldname.IndexOf(' ') + 1);
                        }
                        else
                        {
                            if ((len + oldname.Length) > max_char)
                            {
                                name += '\n'; strcnt++;
                            }
                            name += oldname;
                            oldname = "";
                        }
                    }
                    b.Text = name;
                    if (_CategoryId == cat.Id)
                    {
                        b.Checked = true;
                        
                    }
                    b.CheckAlign = ContentAlignment.TopLeft;
                    RadioButtonCategory.Add(b, cat.Id);
                    b.CheckedChanged += SetCategory;
                    b.Location = new Point(4, Height);
                    Height += 14 * strcnt + 5;

                    p.Controls.Add(b);
                }

                var exams = context.PersonManualExams.Where(x => x.PersonId == GuidId)
                    .Select(x => new
                {
                    ExamId = x.ExamId,
                    Предмет = x.EgeExamName.Name,
                }).ToList();
                dgvEgeManualExam.DataSource = exams;
                if (dgvEgeManualExam.Columns.Contains("ExamId"))
                    dgvEgeManualExam.Columns["ExamId"].Visible = false;

            }
        }
        private void SetCategory(object sender, EventArgs e)
        {
            _CategoryId = RadioButtonCategory[(RadioButton)sender];
        }
    }
}