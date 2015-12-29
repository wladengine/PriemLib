using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq;

using BDClassLib;
using EducServLib;

namespace PriemLib
{
    public partial class Migrator : Form
    {
        private DBPriem _bdc;
        private OleDbClass _odc;
        private string _emptyBase = string.Format(@"{0}\Templates\EMPTYAbiturientDB.mdb", Application.StartupPath);
        private string _emptyBaseFor = string.Format(@"{0}\Templates\EMPTYAbiturientDB_For.mdb", Application.StartupPath);
        private string _metroBase = string.Format(@"{0}\Templates\MetroDB.mdb", Application.StartupPath);

        private long _NewId = 1000001;
        private SortedList<string, long> _slIds;
        private SortedList<string, string> slProf = null;
        private SortedList<string, string> slSpec = null;
        ArrayList _alQueries;
       
        //конструктор
        public Migrator()
        {
            InitializeComponent();
            InitItems();
        }

        //дополнительная инициализация
        private void InitItems()
        {
            this.CenterToParent();
            this.MdiParent = MainClass.mainform;

            _bdc = MainClass.Bdc;

            using (PriemEntities context = new PriemEntities())
            {
                var src = context.StudyLevelGroup.Select(x => new { x.Id, x.Name }).ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).ToList();

                ComboServ.FillCombo(cbStudyLevelGroup, src, false, false);
            }

            btnStart.Enabled = false;
            btnMetro.Enabled = false;
        }

        //старт
        private void btnStart_Click(object sender, EventArgs e)
        {
            string newfile = folderBrowser.SelectedPath + "/AbiturientDB.mdb";

            FileInfo fi = new FileInfo(IsFor ? _emptyBaseFor : _emptyBase);
            fi.CopyTo(newfile, true);

            _odc = new OleDbClass();
            _odc.OpenDatabase(newfile);
                        
            _alQueries = new ArrayList();
            PrepareRegion();
            MigrateProfSpez();
            MigrateOrders();
            _odc.ExecuteWithTrasaction(_alQueries);
            MigrateAbits();
            //_odc.ExecuteWithTrasaction(_alQueries);
            _odc.CloseDataBase();
            MessageBox.Show("Готово!");
        }

        public string FacultyId
        {
            get { return ComboServ.GetComboId(cbFaculty); }            
        }
        public int? StudyLevelGroupId
        {
            get { return ComboServ.GetComboIdInt(cbStudyLevelGroup); }
        }
        public bool IsFor
        {
            get { return chbIsFor.Checked; }
        }

        Dictionary<string, string> _dRegion;
        private void PrepareRegion()
        {
            _dRegion = new Dictionary<string, string>();

            if (IsFor)
            {
                DataSet ds = _bdc.GetDataSet("SELECT * FROM ed.ForeignCountry");

                foreach (DataRow row in ds.Tables[0].Rows)
                    _dRegion.Add(row["Name"].ToString(), row["Id"].ToString());
            }
            else
            {
                DataSet ds = _bdc.GetDataSet("SELECT * FROM ed.Region");

                foreach (DataRow row in ds.Tables[0].Rows)
                    _dRegion.Add(row["Name"].ToString(), row["Id"].ToString());
            }
        }

        //путь сохранения
        private void btnFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                tbFolder.Text = folderBrowser.SelectedPath;
                btnStart.Enabled = true;
                if(MainClass.IsPasha())
                    btnMetro.Enabled = true;
            }
        }
        
        //миграция
        private void MigrateProfSpez()
        {
            slProf = new SortedList<string, string>();
            slSpec = new SortedList<string, string>();

            string query = @"SELECT DISTINCT LicenseProgramId, FacultyId, LicenseProgramCode, LicenseProgramName  
                       FROM ed.qEntry WHERE qEntry.StudyLevelGroupId = " + StudyLevelGroupId + ((string.IsNullOrEmpty(FacultyId) ? "" : " AND ed.qEntry.FacultyId = " + FacultyId));

            DataSet dsProf = _bdc.GetDataSet(query);

            int newProfId = 1;
            foreach (DataRow rowProf in dsProf.Tables[0].Rows)
            {
                string queryIns = string.Format("INSERT INTO Profession (Id, Name, Code, FacultyId) VALUES ({0}, '{1}', '{2}', {3})", newProfId, rowProf["LicenseProgramName"].ToString(), rowProf["LicenseProgramCode"].ToString(), rowProf["FacultyId"].ToString());
                _odc.ExecuteQuery(queryIns);

                string key = rowProf["FacultyId"].ToString() + "_" + rowProf["LicenseProgramId"].ToString();
                string value = newProfId.ToString();
                slProf.Add(key, value);

                newProfId++;
            }

            query = @"SELECT DISTINCT LicenseProgramId, FacultyId, ProfileId, ProfileName 
                       FROM ed.qEntry WHERE NOT ProfileId IS NULL AND qEntry.StudyLevelGroupId = " + StudyLevelGroupId + ((string.IsNullOrEmpty(FacultyId) ? "" : " AND ed.qEntry.FacultyId = " + FacultyId));

            DataSet dsSpec = _bdc.GetDataSet(query);

            int newSpecId = 1;
            foreach (DataRow rowSpec in dsSpec.Tables[0].Rows)
            {               
                string profId = slProf[rowSpec["FacultyId"].ToString() + "_" + rowSpec["LicenseProgramId"].ToString()];

                string queryIns = string.Format("INSERT INTO Specialization (Id, Name, ProfessionId) VALUES ({0}, '{1}', {2})", newSpecId, rowSpec["ProfileName"].ToString(), profId);
                _odc.ExecuteQuery(queryIns);

                string key = string.Format("{0}_{1}_{2}", rowSpec["FacultyId"], rowSpec["LicenseProgramId"], rowSpec["ProfileId"]);
                string value = newSpecId.ToString();
                slSpec.Add(key, value);

                newSpecId++;
            }
        }
        private void MigrateOrders()
        {
            NewWatch wc = new NewWatch(100);
            wc.Show();
            wc.SetText("Загрузка приказов...");
            wc.PerformStep();

            string query = @"SELECT ed.OrderNumbers.*, ed.Protocol.FacultyId, ed.Protocol.StudyFormId, ed.Protocol.StudyBasisId FROM ed.OrderNumbers 
                             INNER JOIN ed.Protocol On ed.OrderNumbers.ProtocolId = ed.Protocol.Id WHERE Protocol.StudyLevelGroupId = " + StudyLevelGroupId + " " + GetFilter("ed.Protocol");
            string queryAbits;
            _slIds = new SortedList<string, long>();
            DataSet ds = _bdc.GetDataSet(query);

            wc.SetMax(ds.Tables[0].Rows.Count);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                wc.PerformStep();
                string s = string.Empty;
                if (dr["OrderNum"].ToString().Length != 0)
                {
                    s = string.Format("INSERT INTO Protocol (Id,FacultyId, SectionId, StudyFormId, Name, FromDate) VALUES ({0},{1},{2},{3},'{4}','{5}')", 
                            _NewId, 
                            dr["FacultyId"].ToString(), 
                            dr["StudyFormId"].ToString(), 
                            dr["StudyBasisId"].ToString(), 
                            dr["OrderNum"].ToString(), 
                            dr["OrderDate"].ToString());
                    _alQueries.Add(s);

                    queryAbits = string.Format(@"SELECT ed.extEntryView.AbiturientId FROM ed.extEntryView INNER JOIN ed.qAbiturient ON ed.extEntryView.AbiturientId = ed.qAbiturient.Id 
INNER JOIN ed.Person ON ed.qAbiturient.PersonId = ed.Person.Id WHERE ed.extEntryView.Id = '{0}' AND ed.Person.NationalityId=1  ", dr["ProtocolId"].ToString());
                    foreach (DataRow drr in _bdc.GetDataSet(queryAbits).Tables[0].Rows)
                    {
                        _slIds.Add(drr["AbiturientId"].ToString(), _NewId);
                    }

                    _NewId++;
                }
                if (dr["OrderNumFor"].ToString().Length != 0)
                {
                    s = string.Format("INSERT INTO Protocol (Id,FacultyId, SectionId, StudyFormId, Name, FromDate) VALUES ({0},{1},{2},{3},'{4}','{5}')", 
                            _NewId, 
                            dr["FacultyId"].ToString(), 
                            dr["StudyFormId"].ToString(), 
                            dr["StudyBasisId"].ToString(), 
                            dr["OrderNumFor"].ToString(), 
                            dr["OrderDateFor"].ToString());
                    _alQueries.Add(s);

                    queryAbits = string.Format(@"SELECT extEntryView.AbiturientId FROM ed.extEntryView INNER JOIN ed.qAbiturient ON extEntryView.AbiturientId = qAbiturient.Id 
INNER JOIN ed.Person ON qAbiturient.PersonId = Person.Id WHERE extEntryView.Id = '{0}' AND Person.NationalityId <> 1 ", dr["ProtocolId"].ToString());
                    foreach (DataRow drr in _bdc.GetDataSet(queryAbits).Tables[0].Rows)
                    {
                        _slIds.Add(drr["AbiturientId"].ToString(), _NewId);
                    }
                    _NewId++;          
                }
            }

            wc.Close();
        }
        private void MigrateAbits()
        {
            NewWatch wc = new NewWatch(100);
            wc.Show();
            wc.SetText("Загрузка данных...");
            wc.PerformStep();
            using (PriemEntities context = new PriemEntities())
            {
                int iFacultyId = 0;
                int.TryParse(FacultyId, out iFacultyId);

                var abitList =
                    (from Ab in context.Abiturient
                     join Pers in context.extPerson on Ab.PersonId equals Pers.Id
                     join PersEduc in context.extPerson_EducationInfo_Current on Pers.Id equals PersEduc.PersonId
                     join Ent in context.extEntry on Ab.EntryId equals Ent.Id
                     join extEV in context.extEntryView on Ab.Id equals extEV.AbiturientId

                     join forNat in context.ForeignCountry on Pers.ForeignNationalityId equals forNat.Id into forNat2
                     from forNat in forNat2.DefaultIfEmpty()

                     where Ab.Entry.StudyLevel.LevelGroupId == StudyLevelGroupId &&
                     (iFacultyId == 0 ? true : Ab.Entry.FacultyId == iFacultyId)
                     && Ent.IsForeign == IsFor
                     //&& Ab.Entry.StudyFormId == 2
                     //&& extEV.Date > new DateTime(2013, 9, 18)
                     select new
                     {
                         Ab.Id,
                         Ab.RegNum,
                         Pers.Surname,
                         Pers.Name,
                         Pers.SecondName,
                         Pers.BirthDate,
                         Pers.BirthPlace,
                         Pers.Sex,
                         Pers.PassportTypeId,
                         Pers.PassportSeries,
                         Pers.PassportNumber,
                         Pers.PassportAuthor,
                         Pers.PassportDate,
                         Pers.NationalityId,
                         Pers.RegionId,
                         Pers.Phone,
                         Pers.Mobiles,
                         Ab.LanguageId,
                         Pers.Privileges,
                         Pers.Code,
                         Pers.City,
                         Pers.Street,
                         Pers.House,
                         Pers.Korpus,
                         Pers.Flat,
                         Pers.CodeReal,
                         Pers.CityReal,
                         Pers.StreetReal,
                         Pers.HouseReal,
                         Pers.KorpusReal,
                         Pers.FlatReal,
                         PersEduc.SchoolTypeId,
                         PersEduc.SchoolCity,
                         PersEduc.SchoolName,
                         PersEduc.SchoolNum,
                         PersEduc.SchoolExitYear,
                         PersEduc.AttestatSeries,
                         PersEduc.AttestatNum,
                         PersEduc.DiplomSeries,
                         PersEduc.DiplomNum,
                         PersEduc.IsExcellent,
                         PersEduc.HEExitYear,
                         Nation = Pers.NationalityName,
                         ForNation = forNat.Name,
                         Ent.FacultyId,
                         Ent.StudyFormId,
                         Ent.StudyBasisId,
                         Ent.LicenseProgramId,
                         Ent.ProfileId,
                         Ab.CompetitionId,
                         Ab.StudyNumber,
                         ObrazProgramName = Ent.ObrazProgramName,
                         ObrazProgramCrypt = Ent.ObrazProgramCrypt,
                         Ab.DocDate,
                         Ab.IsListener,
                         Ent.StudyPlanNumber,
                         ListenerTypeId = Ent.IsSecond ? 1 : (Ent.IsReduced ? 2 : (Ent.IsParallel ? 3 : 0)),
                         Pers.HostelEduc,
                         Pers.HasOriginals,
                         Pers.SNILS,
                     }).ToList();

                if (abitList.Count == 0)
                    return;

                wc.SetMax(abitList.Count);
                wc.SetText("Импорт данных...");

                var notApplicableList = abitList.Select(x => x.Id).Except(_slIds.Keys.Select(x => Guid.Parse(x))).ToList();

                if (notApplicableList.Count() > 0 && !IsFor)
                {
                    WinFormsServ.Error(string.Format("Not found orders for {0} Ids", notApplicableList.Count()));
                    wc.Close();
                    return;
                }

                foreach (var Abit in abitList)
                {
                    string zc = Abit.Code.Replace(" ", "");
                    if (zc.Length > 10)
                        zc = zc.Substring(0, 10);

                    string pa = Abit.PassportAuthor;
                    if (pa.Length > 250)
                        pa = pa.Substring(0, 250);

                    string a = (Abit.Code ?? "") + ", " + (Abit.City ?? "") + ", " + (Abit.Street ?? "") + ", д." + (Abit.House ?? "") + ", " 
                        + ((Abit.Korpus ?? "").Length > 0 ? " к." + (Abit.Korpus ?? "") + ", " : "") + "кв." + (Abit.Flat ?? "");
                    if (a.Length > 250)
                        a = a.Substring(0, 250);

                    string la = (Abit.CodeReal ?? "") + ", " + (Abit.CityReal ?? "") + ", " + (Abit.StreetReal ?? "") + ", д." + (Abit.HouseReal ?? "") + ", " 
                        + ((Abit.KorpusReal ?? "").Length > 0 ? " к." + (Abit.KorpusReal ?? "") + ", " : "") + "кв." + (Abit.FlatReal ?? "");
                    if ((Abit.CodeReal ?? "").Length == 0 && (Abit.CityReal ?? "").Length == 0 && (Abit.StreetReal ?? "").Length == 0 
                        && (Abit.HouseReal ?? "").Length == 0 && (Abit.KorpusReal ?? "").Length == 0 && (Abit.FlatReal ?? "").Length == 0)
                        la = "";
                    if (la.Length > 250)
                        la = la.Substring(0, 250);

                    string ph = (Abit.Phone ?? "") + ((Abit.Mobiles ?? "").Length == 0 ? "" : "; " + (Abit.Mobiles ?? ""));
                    if (ph.Length > 100)
                        ph = ph.Substring(0, 100);

                    string profId = slProf[Abit.FacultyId + "_" + Abit.LicenseProgramId];

                    string specId;
                    if (Abit.ProfileId == 0)
                        specId = "0";
                    else
                        specId = slSpec[Abit.FacultyId + "_" + Abit.LicenseProgramId + "_" + Abit.ProfileId.ToString()];

                    string educSeries = StudyLevelGroupId == 1 ? Abit.AttestatSeries ?? "" : Abit.DiplomSeries ?? "";
                    string educNum = StudyLevelGroupId == 1 ? Abit.AttestatNum ?? "" : Abit.DiplomNum ?? "";

                    if (educNum.Length > 15)
                        educNum = educNum.Substring(0, 15);

                    string educYear = StudyLevelGroupId == 1 ? Abit.SchoolExitYear.ToString() : (Abit.HEExitYear.HasValue ? Abit.HEExitYear.ToString() : "");
                    if (string.IsNullOrEmpty(educYear))
                        educYear = "0";

                    int iEducYear = int.Parse(educYear);

                    if (IsFor && !_slIds.ContainsKey(Abit.Id.ToString()))
                        continue;

                    long abId = _slIds[Abit.Id.ToString()];
                    string regionId = string.Empty;

                    if (IsFor)
                    {
                        if (_dRegion.ContainsKey(Abit.ForNation))
                            regionId = _dRegion[Abit.ForNation];
                        else
                            regionId = _dRegion[Abit.Nation];
                    }
                    else
                        regionId = _dRegion[Abit.Nation];

                    int iRegionId = 0;
                    int.TryParse(regionId, out iRegionId);

                    string AbitSchoolName = (Abit.SchoolName ?? "").Replace("'", "");
                    if (AbitSchoolName.Length > 200)
                        AbitSchoolName = AbitSchoolName.Substring(0, 200);

                    string s = string.Format(
                        "INSERT INTO Abiturient (" +
                        "[FileNum], [Name], [Patronymic], [Surname], " +
                        "[Privileges], [IsExcellent], [ListenerTypeId], [IsActualListener], " +
                        "[Hostel], [FacultyId], [ProfessionId], [SpecializationId], " +
                        "[StudyFormId], [SectionId], [CompetitionId], " +
                        "[DocDate], [CitizenId], [RegionId], [LanguageId], " +
                        "[AttestatSeries], [AttestatRegion], [AttestatNum], [AttestatCopy], " +
                        "[SchoolName], [SchoolCity], [SchoolNum], [SchoolTypeId], [ExitYear], " +
                        "[Phone], [ZipCode], [Adress], [LifeAddress], " +
                        "[BirthDate], [Sex], " +
                        "[PasswordTypeId], [PaswSeries], [PaswNumber], [PaswDate], [PaswAuthor], " +
                        "[StudyNumber], [EntryOrderId], [EduProgName], [EduProgKod], [StudyPlanNumber], [SNILS]) " +
                        "VALUES (" +
                        "@FileNum, @Name, @Patronymic, @Surname, " +
                        "@Privileges, @IsExcellent, @ListenerTypeId, @IsActualListener, " +
                        "@Hostel, @FacultyId, @ProfessionId, @SpecializationId, " +
                        "@StudyFormId, @SectionId, @CompetitionId, " +
                        "@DocDate, @CitizenId, @RegionId, @LanguageId, " +
                        "@AttestatSeries, @AttestatRegion, @AttestatNum, @AttestatCopy, " +
                        "@SchoolName, @SchoolCity, @SchoolNum, @SchoolTypeId, @ExitYear, " +
                        "@Phone, @ZipCode, @Adress, @LifeAddress, " +
                        "@BirthDate, @Sex, " +
                        "@PasswordTypeId, @PaswSeries, @PaswNumber, @PaswDate, @PaswAuthor, " +
                        "@StudyNumber, @EntryOrderId, @EduProgName, @EduProgKod, @StudyPlanNumber, @SNILS)"
                        //"'{0}','{1}','{2}','{3}'," +
                        //"'{4}','{5}','{6}','{7}', " +
                        //"'{8}','{9}','{10}','{11}'," +
                        //"'{12}','{13}','{14}'," +
                        //"'{15}','{16}','{17}','{18}'," +
                        //"'{19}','{20}','{21}','{22}'," +
                        //"'{23}','{24}','{25}','{26}','{27}'," +
                        //"'{28}','{29}','{30}','{31}'," +
                        //"'{32}','{33}'," +
                        //"'{34}','{35}','{36}','{37}','{38}'," +
                        //"'{39}','{40}', '{41}','{42}', '{43}', '{44}')",
                        //Abit.RegNum ?? "", Abit.Name, Abit.SecondName, Abit.Surname,
                        //Abit.Privileges.ToString(), QueryServ.QueryForBool(Abit.IsExcellent.ToString()), Abit.ListenerTypeId.ToString(), QueryServ.QueryForBool(Abit.IsListener.ToString()),
                        //QueryServ.QueryForBool(Abit.HostelEduc.ToString()), Abit.FacultyId.ToString(), profId, specId,
                        //Abit.StudyBasisId, Abit.StudyFormId, Abit.CompetitionId,
                        //Abit.DocDate.ToString(), regionId, IsFor ? regionId : Abit.RegionId.ToString(),
                        //Abit.LanguageId.ToString(),
                        //educSeries, "", educNum, QueryServ.QueryForBool(Abit.HasOriginals.ToString()),
                        //AbitSchoolName ?? "", Abit.SchoolCity ?? "", Abit.SchoolNum ?? "", Abit.SchoolTypeId.ToString(),
                        //(string.IsNullOrEmpty(educYear) ? DateTime.Now.Year.ToString() : educYear),
                        //ph, zc, a, la,
                        //Abit.BirthDate.ToString(), QueryServ.QueryForBool(Abit.Sex.ToString()),
                        //Abit.PassportTypeId.ToString(), Abit.PassportSeries ?? "", Abit.PassportNumber ?? "", Abit.PassportDate.ToString(), pa,
                        //Abit.StudyNumber ?? "", abId,
                        //((Abit.ObrazProgramName ?? "").Length > 128 ? (Abit.ObrazProgramName ?? "").Substring(0, 128) : Abit.ObrazProgramName ?? ""), 
                        //Abit.ObrazProgramCrypt ?? "", (Abit.StudyPlanNumber ?? ""), Abit.SNILS ?? ""
                        );

                    long iRegNum = int.Parse(Abit.RegNum ?? "0");

                    long iProfId = int.Parse(profId ?? "0");
                    long iSpecId = int.Parse(specId ?? "0");

                    List<KeyValuePair<string, object>> slParams = new List<KeyValuePair<string, object>>();
                    slParams.Add(new KeyValuePair<string, object>("@FileNum", iRegNum));
                    slParams.Add(new KeyValuePair<string, object>("@Name", Abit.Name));
                    slParams.Add(new KeyValuePair<string, object>("@Patronymic", Abit.SecondName));
                    slParams.Add(new KeyValuePair<string, object>("@Surname", Abit.Surname));
                    slParams.Add(new KeyValuePair<string, object>("@Privileges", (long)Abit.Privileges));
                    slParams.Add(new KeyValuePair<string, object>("@IsExcellent", Abit.IsExcellent));
                    slParams.Add(new KeyValuePair<string, object>("@ListenerTypeId", (long)Abit.ListenerTypeId));
                    slParams.Add(new KeyValuePair<string, object>("@IsActualListener", Abit.IsListener));
                    slParams.Add(new KeyValuePair<string, object>("@Hostel", Abit.HostelEduc));
                    slParams.Add(new KeyValuePair<string, object>("@FacultyId", (long)Abit.FacultyId));
                    slParams.Add(new KeyValuePair<string, object>("@ProfessionId", iProfId));
                    slParams.Add(new KeyValuePair<string, object>("@SpecializationId", iSpecId));
                    slParams.Add(new KeyValuePair<string, object>("@StudyFormId", (long)Abit.StudyBasisId));
                    slParams.Add(new KeyValuePair<string, object>("@SectionId", (long)Abit.StudyFormId));
                    slParams.Add(new KeyValuePair<string, object>("@CompetitionId", (long)Abit.CompetitionId));
                    slParams.Add(new KeyValuePair<string, object>("@DocDate", Abit.DocDate.Date));
                    slParams.Add(new KeyValuePair<string, object>("@CitizenId", (long)iRegionId));
                    slParams.Add(new KeyValuePair<string, object>("@RegionId", IsFor ? (long)iRegionId : (long)Abit.RegionId));
                    slParams.Add(new KeyValuePair<string, object>("@LanguageId", (long)Abit.LanguageId));
                    slParams.Add(new KeyValuePair<string, object>("@AttestatSeries", educSeries.Length > 10 ? educSeries.Substring(educSeries.Length - 10, 10) : educSeries));
                    slParams.Add(new KeyValuePair<string, object>("@AttestatRegion", ""));
                    slParams.Add(new KeyValuePair<string, object>("@AttestatNum", educNum.Length > 15 ? educNum.Substring(educNum.Length - 15, 15) : educNum));
                    slParams.Add(new KeyValuePair<string, object>("@AttestatCopy", Abit.HasOriginals));
                    slParams.Add(new KeyValuePair<string, object>("@SchoolName", AbitSchoolName ?? ""));
                    slParams.Add(new KeyValuePair<string, object>("@SchoolCity", Abit.SchoolCity ?? ""));
                    slParams.Add(new KeyValuePair<string, object>("@SchoolNum", Abit.SchoolNum ?? ""));
                    slParams.Add(new KeyValuePair<string, object>("@SchoolTypeId", (long)Abit.SchoolTypeId));
                    slParams.Add(new KeyValuePair<string, object>("@ExitYear", iEducYear == 0 ? (long)DateTime.Now.Year : (long)iEducYear));
                    slParams.Add(new KeyValuePair<string, object>("@Phone", ph));
                    slParams.Add(new KeyValuePair<string, object>("@ZipCode", zc));
                    slParams.Add(new KeyValuePair<string, object>("@Adress", a));
                    slParams.Add(new KeyValuePair<string, object>("@LifeAddress", la));
                    slParams.Add(new KeyValuePair<string, object>("@BirthDate", Abit.BirthDate.Date));
                    slParams.Add(new KeyValuePair<string, object>("@Sex", Abit.Sex));
                    slParams.Add(new KeyValuePair<string, object>("@PasswordTypeId", (long)Abit.PassportTypeId));
                    slParams.Add(new KeyValuePair<string, object>("@PaswSeries", Abit.PassportSeries ?? ""));
                    slParams.Add(new KeyValuePair<string, object>("@PaswNumber", Abit.PassportNumber ?? ""));
                    slParams.Add(new KeyValuePair<string, object>("@PaswDate", (Abit.PassportDate ?? new DateTime(1997, 1, 1)).Date));
                    slParams.Add(new KeyValuePair<string, object>("@PaswAuthor", pa));
                    slParams.Add(new KeyValuePair<string, object>("@StudyNumber", Abit.StudyNumber ?? ""));
                    slParams.Add(new KeyValuePair<string, object>("@EntryOrderId", abId));
                    slParams.Add(new KeyValuePair<string, object>("@EduProgName", ((Abit.ObrazProgramName ?? "").Length > 128 ? (Abit.ObrazProgramName ?? "").Substring(0, 128) : Abit.ObrazProgramName ?? "")));
                    slParams.Add(new KeyValuePair<string, object>("@EduProgKod", Abit.ObrazProgramCrypt ?? ""));
                    slParams.Add(new KeyValuePair<string, object>("@StudyPlanNumber", (Abit.StudyPlanNumber ?? "")));
                    slParams.Add(new KeyValuePair<string, object>("@SNILS", Abit.SNILS ?? ""));

                    //_odc.ExecuteQuery(s, slParams);

                    _NewId++;
                    wc.PerformStep();
                }
            }
            wc.Close();
        }

        //фильтр по факультету
        private string GetFilter(string table)
        {
            string res = string.Empty;

            if (!string.IsNullOrEmpty(FacultyId))
                res += string.Format(" AND {0}.FacultyId = {1} ", table, FacultyId);
            
            if (chbIsFor.Checked)
                res += string.Format(" AND {0}.IsForeign = 1 ", table);

            res += string.Format(" AND {0}.StudyLevelGroupId = {1} ", table, StudyLevelGroupId);

            return res;
        }

        private void btnMetro_Click(object sender, EventArgs e)
        {
            string newfile = folderBrowser.SelectedPath + "/MetroDB.mdb";

            FileInfo fi = new FileInfo(_metroBase);
            fi.CopyTo(newfile, true);

            _alQueries = new ArrayList();

            _odc = new OleDbClass();
            _odc.OpenDatabase(newfile);

            string query = 
                string.Format(
                    "SELECT DISTINCT ed.extAbit.Id, ed.Person.Name, ed.Person.SecondName, ed.Person.Surname, " +
                    "ed.Person.BirthDate, ed.extAbit.StudyNumber, ed.extAbit.StudyLevelId, " +
                    "ed.Person.PassportTypeId, case when ed.Person.PassportTypeId=1 then 'Р' when ed.Person.PassportTypeId=3 then 'З' else '' end as PassportType, " +
                    "ed.Person.PassportSeries, ed.Person.PassportNumber, EEE.DateFinishEduc, " +
                    "ed.extEntryView.Id AS EntryProtId " +
                    "FROM ed.extAbit INNER JOIN ed.Person ON ed.extAbit.PersonId = ed.Person.Id " +
                    "INNER JOIN ed.extEntryView ON ed.extEntryView.AbiturientId = ed.extAbit.Id " +
                    "INNER JOIN ed.Entry AS EEE ON EEE.Id = extAbit.EntryId " +
                    "WHERE ed.extAbit.StudyFormId = 1 {0} {1}", 
                    GetFilter("extAbit"),
                    ""
                    //" AND extEntryView.Date >= '26.08.2015' "
                );

            DataSet ds = _bdc.GetDataSet(query);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string ser = dr["PassportSeries"].ToString();
                string ser1 = string.Empty, ser2 = string.Empty;
                string pType = dr["PassportTypeId"].ToString();

                int num = 0;
                string temp = ser.Replace(" ", "").Replace("-", "");
                if (pType == "1" && int.TryParse(temp, out num) && temp.Length == 4)
                {
                    ser = ser.Replace(" ", "").Replace("-", "");
                    ser1 = ser.Substring(0, 2);
                    ser2 = ser.Substring(2, 2);
                }
                else
                    ser1 = ser;

                string dateEnd;
                string course;

                string stLevel = dr["StudyLevelId"].ToString();
                DateTime? dtFinish = dr.Field<DateTime?>("DateFinishEduc");
                
                if (stLevel == "16")
                {
                    course = "1";
                    if (dtFinish.HasValue)
                        dateEnd = dtFinish.Value.ToString("dd.MM.yyyy");
                    else
                        dateEnd = "31.08." + (MainClass.iPriemYear + 4).ToString();
                }
                else if (stLevel == "17")
                {
                    course = "5";
                    if (dtFinish.HasValue)
                        dateEnd = dtFinish.Value.ToString("dd.MM.yyyy");
                    else
                        dateEnd = "31.08." + (MainClass.iPriemYear + 2).ToString();
                }
                else if (stLevel == "18")
                {
                    course = "1";
                    if (dtFinish.HasValue)
                        dateEnd = dtFinish.Value.ToString("dd.MM.yyyy");
                    else
                        dateEnd = "31.08." + (MainClass.iPriemYear + 5).ToString();
                }
                else
                {
                    course = "1";
                    if (dtFinish.HasValue)
                        dateEnd = dtFinish.Value.ToString("dd.MM.yyyy");
                    else
                        dateEnd = "31.08." + (MainClass.iPriemYear + 3).ToString();//aspirant
                }

                string datebirth = ((DateTime)dr["BirthDate"]).ToString("dd.MM.yyyy");
                string OrgCode = "21";
                if (FacultyId == "17")
                    OrgCode = "197";
                if (FacultyId == "29")
                    OrgCode = "105";

                string s = string.Format(
                    "INSERT INTO sList ([DOC_KIND], [DOC_SN], [DOC_S]," +
                    "[DOC_NUM],[SDOCUM],[NAME_F],[NAME_I], [NAME_O]," +
                    "[ORGCODE], [DATEEND], [BIRTHDAY], [COURSE])" +
                    "VALUES ('{0}','{1}','{2}'," +
                    "'{3}','{4}','{5}','{6}','{7}'," +
                    "'" + OrgCode + "','{8}','{9}','{10}')",
                    dr["PassportType"].ToString(), ser1, ser2,
                    dr["PassportNumber"].ToString(), dr["StudyNumber"].ToString(), dr["Surname"].ToString(), dr["Name"].ToString(), dr["SecondName"].ToString(),
                    dateEnd, datebirth, course);

                _alQueries.Add(s);
            }

            _odc.ExecuteWithTrasaction(_alQueries);
            _odc.CloseDataBase();
            MessageBox.Show("Done!");
        }

        private void cbStudyLevelGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = context.extEntry.Where(x => x.StudyLevelGroupId == StudyLevelGroupId).Select(x => new { x.FacultyId, x.FacultyName }).Distinct().ToList().OrderBy(x => x.FacultyName)
                    .Select(x => new KeyValuePair<string, string>(x.FacultyId.ToString(), x.FacultyName)).ToList();
                ComboServ.FillCombo(cbFaculty, src, false, true);
            }
        }
    }
}