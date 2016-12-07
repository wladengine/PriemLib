using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;
using System.Linq;

using EducServLib;

namespace PriemLib
{
    public static class ExportClass
    {
        public static void SetStudyNumbers()
        {
            if (!MainClass.IsPasha())
                return;
            
            using (PriemEntities context = new PriemEntities())
            {
                // префиксы
                // 1 - бакалавриат
                // 2 - магистратура
                // 3 - СПО
                // 4 - иностранцы
                // 5 - аспиранты
                for (int SLGr = 1; SLGr <= 5; SLGr++)
                {
                    //взять максимум номера, если еще ничего не назначено
                    int SLG = SLGr;
                    if (SLG == 5) //ORD -> ASP == 4
                        SLG = 4;

                    string num =
                        (from ab in context.qAbitAll
                         where ab.StudyLevelGroupId == SLG && !ab.IsForeign
                         select ab.StudyNumber).Max();

                    string numFor =
                        (from ab in context.qAbitAll
                         where ab.IsForeign
                         select ab.StudyNumber).Max();

                    var abits =
                        (from ab in context.extAbit
                         join ev in context.extEntryView
                         on ab.Id equals ev.AbiturientId
                         where ab.StudyLevelGroupId == SLGr && (ab.StudyNumber == null || ab.StudyNumber.Length == 0)
                         && !ab.IsForeign
                         orderby ab.FacultyId, ab.FIO
                         select ab.Id).ToList();

                    var foreignAbits =
                        (from ab in context.extAbit
                         join ev in context.extEntryView
                         on ab.Id equals ev.AbiturientId
                         where ab.StudyLevelGroupId == SLGr && (ab.StudyNumber == null || ab.StudyNumber.Length == 0)
                         && ab.IsForeign
                         orderby ab.FacultyId, ab.FIO
                         select ab.Id).ToList();

                    List<Guid> lstAbits = abits.Except(foreignAbits).ToList();

                    int stNum = 0;
                    if (num != null && num.Length != 0)
                        stNum = int.Parse(num.Substring(3));

                    int stNumFor = 0;
                    if (numFor != null && numFor.Length != 0)
                        stNumFor = int.Parse(numFor.Substring(3));

                    stNum++;
                    stNumFor++;

                    int Pref = SLGr;
                    if (Pref == 4) // ASP&ORD = 5
                        Pref = 5;

                    foreach (Guid abitId in lstAbits)
                    {
                        string sNum = "0000" + stNum.ToString();
                        sNum = sNum.Substring(sNum.Length - 4, 4);
                        sNum = (MainClass.iPriemYear % 100).ToString() + Pref + sNum;

                        context.Abiturient_UpdateStudyNumber(sNum, abitId);
                        stNum++;
                    }

                    foreach (Guid abitId in foreignAbits)
                    {
                        string sNum = "0000" + stNumFor.ToString();
                        sNum = sNum.Substring(sNum.Length - 4, 4);
                        sNum = (MainClass.iPriemYear % 100).ToString() + "4" + sNum;

                        context.Abiturient_UpdateStudyNumber(sNum, abitId);
                        stNumFor++;
                    }
                }

                MessageBox.Show("Done");
            }
        }

        public static void ExportVTB()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV files|*.csv";

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                using (StreamWriter writer = new StreamWriter(sfd.OpenFile(), Encoding.GetEncoding(1251)))
                using (PriemEntities context = new PriemEntities())
                {
                    string[] headers = new string[] { 
                "Фамилия","Имя","Отчество","Пол","Дата рождения","Место рождения","Гражданство","Код документа"
                ,"Серия паспорта","Номер паспорта","Когда выдан паспорт","Кем выдан паспорт","Код подразделения"
                ,"Адрес регистрации","Индекс","Район","Город","Улица","Дом","Корпус","Квартира","Телефон по месту работы"
                ,"Контактный телефон","Рабочий телефон","Должность","Кодовое слово","Основной доход","Тип карты","Дата приема на работу"};


                    writer.WriteLine(string.Join(";", headers));

                    string query = @"select 
ed.person.surname, ed.person.name, ed.person.secondname,
case when ed.person.sex>0 then 'М' else 'Ж' end as sex,

CAST(
(
STR( DAY( ed.person.Birthdate ) ) + '/' +
STR( MONTH( ed.person.Birthdate ) ) + '/' +
STR( YEAR( ed.person.Birthdate ) )
)
AS DATETIME
) as birthdate,
ed.person.birthplace,
nation.name as nationality,
ed.passporttype.name as passporttype,
case when passporttypeid=1 then substring(ed.person.passportseries,1,2)+ ' ' + substring(ed.person.passportseries,3,2) else ed.person.passportseries end as passportseries, 
ed.person.passportnumber, ed.person.passportauthor, ed.person.passportcode,
CAST(
(
STR( DAY( ed.person.passportDate ) ) + '/' +
STR( MONTH( ed.person.passportDate ) ) + '/' +
STR( YEAR( ed.person.passportDate ) )
)
AS DATETIME
) as passportwhen,


ed.person.code,
ed.region.name as region,
ed.person.city,
ed.person.street,
ed.person.house,
ed.person.korpus,
ed.person.flat,

ed.person.codereal,
ed.person.cityreal,
ed.person.streetreal,
ed.person.housereal,
ed.person.korpusreal,
ed.person.flatreal,

ed.person.phone,
ed.person.mobiles
from
ed.extentryview 
inner join ed.extAbit on ed.extabit.id=ed.extentryview.abiturientid
inner join ed.person on ed.person.id=ed.extabit.personid
inner join ed.country as nation on nation.id=ed.person.nationalityid
inner join ed.passporttype on ed.passporttype.id=ed.person.passporttypeid
left join ed.region on ed.region.id=ed.person.regionid
where ed.extentryview.studyformid=1 and ed.extentryview.studybasisid=1 and ed.extabit.studylevelgroupid IN (" + Util.BuildStringWithCollection(MainClass.lstStudyLevelGroupId) + ")";

                    var AbitList =
                        from Abit in context.Abiturient
                        join extEV in context.extEntryView on Abit.Id equals extEV.AbiturientId
                        where MainClass.lstStudyLevelGroupId.Contains(Abit.Entry.StudyLevel.LevelGroupId)
                        && Abit.Entry.StudyFormId == 1 && Abit.Entry.StudyBasisId == 1
                        select new
                        {
                            Abit.Person.Surname,
                            Abit.Person.Name,
                            Abit.Person.SecondName,
                            Abit.Person.Sex,
                            Abit.Person.BirthDate,
                            Abit.Person.BirthPlace,
                            Nationality = Abit.Person.Nationality.Name,
                            PassportType = Abit.Person.PassportType.Name,
                            Abit.Person.PassportTypeId,
                            Abit.Person.PassportSeries,
                            Abit.Person.PassportNumber,
                            Abit.Person.PassportAuthor,
                            Abit.Person.PassportCode,
                            Abit.Person.PassportDate,
                            Abit.Person.Person_Contacts.Code,
                            Region = Abit.Person.Person_Contacts.Region.Name,
                            Abit.Person.Person_Contacts.City,
                            Abit.Person.Person_Contacts.Street,
                            Abit.Person.Person_Contacts.House,
                            Abit.Person.Person_Contacts.Korpus,
                            Abit.Person.Person_Contacts.Flat,
                            Abit.Person.Person_Contacts.CodeReal,
                            Abit.Person.Person_Contacts.CityReal,
                            Abit.Person.Person_Contacts.StreetReal,
                            Abit.Person.Person_Contacts.HouseReal,
                            Abit.Person.Person_Contacts.FlatReal,
                            Abit.Person.Person_Contacts.Phone,
                            Abit.Person.Person_Contacts.Mobiles
                        };

                    //DataSet ds = MainClass.Bdc.GetDataSet(query);
                    foreach (/*DataRow row in ds.Tables[0].Rows*/ var Abit in AbitList)
                    {
                        List<string> list = new List<string>();

                        //list.Add(row["surname"].ToString());
                        //list.Add(row["name"].ToString());
                        //list.Add(row["secondname"].ToString());
                        //list.Add(row["sex"].ToString());
                        //list.Add(DateTime.Parse(row["birthdate"].ToString()).ToString("dd/MM/yyyy"));

                        //list.Add(row["birthplace"].ToString());
                        //list.Add(row["nationality"].ToString());
                        //list.Add(row["passporttype"].ToString());
                        //list.Add(row["passportseries"].ToString());
                        //list.Add(row["passportnumber"].ToString());

                        //list.Add(DateTime.Parse(row["passportwhen"].ToString()).ToString("dd/MM/yyyy"));
                        //list.Add(row["passportauthor"].ToString());
                        //list.Add(row["passportcode"].ToString());
                        //list.Add("по паспорту");
                        //list.Add(row["code"].ToString());

                        //list.Add(row["region"].ToString());
                        //list.Add(row["city"].ToString());
                        //list.Add(row["street"].ToString());
                        //list.Add(row["house"].ToString());
                        //list.Add(row["korpus"].ToString());

                        //list.Add(row["flat"].ToString());
                        //list.Add("");
                        //list.Add(row["phone"].ToString() + ", " + row["mobiles"].ToString().Replace(";", ","));
                        //list.Add("");
                        //list.Add("студент");

                        //list.Add("");
                        //list.Add("0");
                        //list.Add("visaelectron");
                        //list.Add("01/09/2012");

                        list.Add(Abit.Surname);
                        list.Add(Abit.Name);
                        list.Add(Abit.SecondName);
                        list.Add(Abit.Sex ? "М" : "Ж");
                        list.Add(Abit.BirthDate.ToString("dd/MM/yyyy"));

                        list.Add(Abit.BirthPlace);
                        list.Add(Abit.Nationality);
                        list.Add(Abit.PassportType);
                        list.Add(Abit.PassportTypeId == 1 ? Abit.PassportSeries.Replace(" ", "").Substring(0, 2) + " " + Abit.PassportSeries.Replace(" ", "").Substring(2) : Abit.PassportSeries);
                        list.Add(Abit.PassportNumber);

                        list.Add(Abit.PassportDate.Value.ToString("dd/MM/yyyy"));
                        list.Add(Abit.PassportAuthor);
                        list.Add(Abit.PassportCode);
                        list.Add("по паспорту");

                        list.Add(Abit.Code);
                        list.Add(Abit.Region);
                        list.Add(Abit.City);
                        list.Add(Abit.Street);
                        list.Add(Abit.House);
                        list.Add(Abit.Korpus);

                        list.Add(Abit.Flat);
                        list.Add("");
                        list.Add(Abit.Phone + ", " + Abit.Mobiles.Replace(";", ","));
                        list.Add("");
                        list.Add("студент");

                        list.Add("");
                        list.Add("0");
                        list.Add("visaelectron");
                        list.Add("01/09/2013");

                        writer.WriteLine(string.Join(";", list.ToArray()));
                    }
                }

                MessageBox.Show("Done!");
            }
            catch
            {
                WinFormsServ.Error("Ошибка при экспорте");
            }
            return;
        }

        public static void ExportSber()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV files|*.csv";

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                using (StreamWriter writer = new StreamWriter(sfd.OpenFile(), Encoding.GetEncoding(1251)))
                using (PriemEntities context = new PriemEntities())
                {
                    string[] headers = new string[] { 
                "Пол","ФИО","Паспорт","Дата выдачи", "Кем выдан", "Дата рождения","Место рождения",
                "Контрольное слово","Индекс","Адрес 1","Адрес 2","Адрес 3","Адрес 4","Телефон домашний","Телефон мобильный",
                "Телефон рабочий","Должность","Гражданство"};

                    writer.WriteLine(string.Join(";", headers));

                    string query = @"select 
ed.person.surname, ed.person.name, ed.person.secondname,
case when ed.person.sex>0 then 'М' else 'Ж' end as sex,
CAST(
(
STR( DAY( ed.person.Birthdate ) ) + '/' +
STR( MONTH( ed.person.Birthdate ) ) + '/' +
STR( YEAR( ed.person.Birthdate ) )
)
AS DATETIME
) as birthdate,
ed.person.birthplace,
nation.name as nationality,
ed.passporttype.name as passporttype,
case when passporttypeid=1 then substring(ed.person.passportseries,1,2)+ ' ' + substring(ed.person.passportseries,3,2) else ed.person.passportseries end as passportseries, 
ed.person.passportnumber, ed.person.passportauthor, ed.person.passportcode,
CAST(
(
STR( DAY( ed.person.passportDate ) ) + '/' +
STR( MONTH( ed.person.passportDate ) ) + '/' +
STR( YEAR( ed.person.passportDate ) )
)
AS DATETIME
) as passportwhen,
ed.person.code,
ed.region.name as region,
ed.person.city,
ed.person.street,
ed.person.house,
ed.person.korpus,
ed.person.flat,
ed.person.codereal,
ed.person.cityreal,
ed.person.streetreal,
ed.person.housereal,
ed.person.korpusreal,
ed.person.flatreal,
ed.person.phone,
ed.person.mobiles

from
ed.extentryview 
inner join ed.extAbit on ed.extabit.id=ed.extentryview.abiturientid
inner join ed.person on ed.person.id=ed.extabit.personid
inner join ed.country as nation on nation.id=ed.person.nationalityid
inner join ed.passporttype on ed.passporttype.id=ed.person.passporttypeid
left join ed.region on ed.region.id=ed.person.regionid
where ed.extentryview.studyformid=1 and ed.extentryview.studybasisid=1 and ed.extabit.studylevelgroupid IN (" + Util.BuildStringWithCollection(MainClass.lstStudyLevelGroupId) + ")";

                    var AbitList =
                        from Abit in context.Abiturient
                        join extEV in context.extEntryView on Abit.Id equals extEV.AbiturientId
                        where MainClass.lstStudyLevelGroupId.Contains(Abit.Entry.StudyLevel.LevelGroupId)
                        && Abit.Entry.StudyFormId == 1 && Abit.Entry.StudyBasisId == 1
                        select new
                        {
                            Abit.Person.Surname,
                            Abit.Person.Name,
                            Abit.Person.SecondName,
                            Abit.Person.Sex,
                            Abit.Person.BirthDate,
                            Abit.Person.BirthPlace,
                            Nationality = Abit.Person.Nationality.Name,
                            PassportType = Abit.Person.PassportType.Name,
                            Abit.Person.PassportTypeId,
                            Abit.Person.PassportSeries,
                            Abit.Person.PassportNumber,
                            Abit.Person.PassportAuthor,
                            Abit.Person.PassportCode,
                            Abit.Person.PassportDate,
                            Abit.Person.Person_Contacts.Code,
                            Region = Abit.Person.Person_Contacts.Region.Name,
                            Abit.Person.Person_Contacts.City,
                            Abit.Person.Person_Contacts.Street,
                            Abit.Person.Person_Contacts.House,
                            Abit.Person.Person_Contacts.Korpus,
                            Abit.Person.Person_Contacts.Flat,
                            Abit.Person.Person_Contacts.CodeReal,
                            Abit.Person.Person_Contacts.CityReal,
                            Abit.Person.Person_Contacts.StreetReal,
                            Abit.Person.Person_Contacts.HouseReal,
                            Abit.Person.Person_Contacts.FlatReal,
                            Abit.Person.Person_Contacts.Phone,
                            Abit.Person.Person_Contacts.Mobiles
                        };

                    //DataSet ds = MainClass.Bdc.GetDataSet(query);
                    foreach (/*DataRow row in ds.Tables[0].Rows*/ var Abit in AbitList)
                    {
                        List<string> list = new List<string>();

                        //list.Add(row["sex"].ToString());
                        //list.Add((row["surname"].ToString() + " " + row["name"].ToString() + " " + row["secondname"].ToString()).Trim());
                        //list.Add((row["passportseries"].ToString() + " " + row["passportnumber"].ToString()).Trim());
                        //list.Add(DateTime.Parse(row["passportwhen"].ToString()).ToString("dd.MM.yyyy"));
                        //list.Add(row["passportauthor"].ToString());

                        //list.Add(DateTime.Parse(row["birthdate"].ToString()).ToString("dd.MM.yyyy"));
                        //list.Add(row["birthplace"].ToString());
                        //list.Add("");
                        //list.Add(row["code"].ToString());
                        //list.Add(row["region"].ToString() + " " + row["city"].ToString());

                        //list.Add(row["street"].ToString() + ", " + row["house"].ToString());
                        //list.Add(row["korpus"].ToString());
                        //list.Add(row["flat"].ToString());
                        //list.Add(row["phone"].ToString());
                        //list.Add(row["mobiles"].ToString().Replace(";", ","));

                        //list.Add("");
                        //list.Add("студент");
                        //list.Add(row["nationality"].ToString());

                        list.Add(Abit.Sex ? "М" : "Ж");
                        list.Add((Abit.Surname + " " + Abit.Name + " " + Abit.SecondName).Trim());
                        list.Add((Abit.PassportSeries + " " + Abit.PassportNumber).Trim());
                        list.Add(Abit.PassportDate.Value.ToString("dd.MM.yyyy"));
                        list.Add(Abit.PassportAuthor);

                        list.Add(Abit.BirthDate.ToString("dd.MM.yyyy"));
                        list.Add(Abit.BirthPlace);
                        list.Add("");
                        list.Add(Abit.Code);
                        list.Add(Abit.Region + " " + Abit.City);

                        list.Add(Abit.Street + ", " + Abit.House);
                        list.Add(Abit.Korpus);
                        list.Add(Abit.Flat);
                        list.Add(Abit.Phone);
                        list.Add(Abit.Mobiles.Replace(";", ","));

                        list.Add("");
                        list.Add("студент");
                        list.Add(Abit.Nationality);

                        writer.WriteLine(string.Join(";", list.ToArray()));
                    }
                }
                MessageBox.Show("Done");
            }
            catch
            {
                WinFormsServ.Error("Ошибка при экспорте");
            }
            return;
        }

        public static void SetAvgBall()
        {
            if (!MainClass.IsPasha())
                return;

            MessageBox.Show("Метод не реализован!");
        }

        public static void SetPayData()
        {
            if (!MainClass.IsPasha())
                return;

            MessageBox.Show("Метод не реализован!");
        }

        public static void ExportInNewStudent(DateTime? dtMinOrderDate)
        {
            using (PriemEntities context = new PriemEntities())
            {
                var exportData =
                    (from Pers in context.extPerson
                     join PaspType in context.PassportType on Pers.PassportTypeId equals PaspType.Id
                     join Abit in context.Abiturient on Pers.Id equals Abit.PersonId
                     join Ent in context.extEntry on Abit.EntryId equals Ent.Id
                     join SL in context.StudyLevel on Ent.StudyLevelId equals SL.Id
                     join EV in context.extEntryView on Abit.Id equals EV.AbiturientId
                     join ForCountry in context.ForeignCountry on Pers.ForeignNationalityId equals ForCountry.Id
                     join Reg in context.Region on Pers.RegionId equals Reg.Id
                     join AD in context.ADUserData on Abit.Id equals AD.AbiturientId
                     join EDUC in context.extPerson_EducationInfo_Current on Pers.Id equals EDUC.PersonId
                     join InEnt in context.InnerEntryInEntry on Abit.InnerEntryInEntryId equals InEnt.Id into InEnt2
                     from InEnt in InEnt2.DefaultIfEmpty()
                     join OP in context.qObrazProgram on InEnt.ObrazProgramId equals OP.Id into OP2
                     from OP in OP2.DefaultIfEmpty()
                     join Prof in context.SP_Profile on InEnt.ProfileId equals Prof.Id into Prof2
                     from Prof in Prof2.DefaultIfEmpty()
                     where (Ent.StudyLevelGroupId == 1 || Ent.StudyLevelGroupId == 2)
                     && (dtMinOrderDate.HasValue ? (EV.OrderDate >= dtMinOrderDate.Value || EV.OrderDateFor >= dtMinOrderDate.Value) : true)
                     select new
                     {
                    //Персона:
                    //1. Id персоны
                    //2. Фамилия (string(150))
                    //3. Имя (string(150))
                    //4. Отчество (string(150))
                    //5. Старая фамилия (string(100)) – может быть null
                    //6. Без отчества (bool) – проставляется если у персоны нет отчества
                    //7. Пол (bool) - True = муж, False = же
                    //8. Место рождения (string(1024))
                    //9. Дата рождения (datetime)
                    //10. Гражданство (справочник) - Искать страну по коду ОКСМ
                    //11. Регион (справочник) – Искать по коду региона
                    //12. Адрес (string(1024))
                    //13. Логин (string(100)) - stХХХХХХ
                    //14. Email (string(100))
                    //15. Телефон (string(100))
                    //16. Снилс (string(100))
                         PersonId = Pers.Id,
                         Pers.Surname,
                         Pers.Name,
                         Pers.SecondName,
                         Pers.Sex,
                         Pers.BirthDate,
                         Pers.BirthPlace,
                         Pers.NationalityId,
                         ForCountry.OSKMCode,
                         Pers.RegionNumber,
                         Pers.Code,
                         Pers.City,
                         Pers.Street,
                         Pers.House,
                         Pers.Flat,
                         Pers.SNILS,
                         AD.Login,
                         Pers.Phone,
                         //Pers.Email, //заменяем Email из карточки на присвоенный Email
                         AD.Email,
                    //Паспорт:
                    //17. Тип документа (справочник)
                    //18. Серия (string(100))
                    //19. Номер (string(100))
                    //20. Кем выдан (string(255))
                    //21. Дата (datetime)
                         PassportTypeName = PaspType.SKName,
                         Pers.PassportSeries,
                         Pers.PassportNumber,
                         Pers.PassportAuthor,
                         Pers.PassportDate,
                    //Студент:
                    //22. Направление (справочник) - надо разобраться с новыми подразделениями
                    //23. Образовательная программа (справочник) - связывать по коду ОП
                    //24. Профиль (справочник) – может быть null
                    //25. Специальность (справочник) - лучше всего связывать данные по классификатору кодов направлений
                    //26. Специализация (справочник) - полный аналог профиля, только относится к специалитету
                    //27. Год поступления (справочник)
                    //28. Линия прибытия (справочник)
                    //29. Ступень обучения (справочник)
                    //30. Категория зачисления (справочник) - гослиния, контракт, межвуз, равный прием
                    //31. Дата начала обучения (datetime)
                    //32. Основа обучения (справочник) - (бюджет, межвуз, платно)
                    //33. Учебный план (справочник) – по коду учебного плана в формате *\*\*
                    //34. Номер зачетной книжки (string(100))
                    //35. Курс (справочник) – всегда первый
                    //36. Семестр (справочник) – всегда первый
                    //37. Форма обучения (справочник) - дневное, вечернее, заочное
                         FacultyName = Ent.FacultyDirectionName,
                         ObrazProgramCrypt = Abit.InnerEntryInEntryId == null ? Ent.ObrazProgramCrypt : OP.Crypt,
                         ProfileName = Abit.InnerEntryInEntryId == null ? Ent.ProfileName : Prof.Name,
                         Ent.LicenseProgramCode,
                         Ent.LicenseProgramName,
                         Ent.StudyLevelId,
                         StudyLevelName = SL.SKName,
                         Ent.IsForeign,
                         Ent.StudyBasisId,
                         Ent.DateStartEduc,
                         Ent.StudyPlanNumber,
                         Abit.StudyNumber,
                         Ent.StudyFormId,
                    //Приказ о зачислении:
                    //38. Тип документа (справочник) – всегда «Приказ о зачислении»
                    //39. Номер (string(100))
                    //40. Дата (datetime)
                        EV.OrderNum,
                        EV.OrderNumFor,
                        EV.OrderDate,
                        EV.OrderDateFor,
                    //Документ об образовании:
                    //41. Тип документа (справочник)
                    //42. Серия (string(100))
                    //43. Номер (string(100))
                    //44. Кем выдан (string(255))
                    //45. Дата (datetime)
                        EDUC.SchoolTypeId,
                        EDUC.SchoolName,
                        EDUC.AttestatSeries,
                        EDUC.AttestatNum,
                        EDUC.DiplomSeries,
                        EDUC.DiplomNum,
                        EDUC.SchoolExitYear
                     }).ToList();

                DataTable tbl = new DataTable();
                tbl.Columns.Add("ID");
                tbl.Columns.Add("Name");
                tbl.Columns.Add("Surname");
                tbl.Columns.Add("SecondName");
                tbl.Columns.Add("OldSurname");
                tbl.Columns.Add("NoSecondName", typeof(bool));
                tbl.Columns.Add("Sex", typeof(bool));
                tbl.Columns.Add("BirthPlace");
                tbl.Columns.Add("BirthDate", typeof(DateTime));
                tbl.Columns.Add("Nationality");
                tbl.Columns.Add("Region");
                tbl.Columns.Add("Address");
                tbl.Columns.Add("Login");
                tbl.Columns.Add("Emiail");
                tbl.Columns.Add("Phone");
                tbl.Columns.Add("SNILS");
                tbl.Columns.Add("DocumentType");
                tbl.Columns.Add("DocumentSeries");
                tbl.Columns.Add("DocumentNumber");
                tbl.Columns.Add("DocumentAuthor");
                tbl.Columns.Add("DocumentDate");
                tbl.Columns.Add("Faculty");
                tbl.Columns.Add("ObrazProgramNumber");
                tbl.Columns.Add("Profile");
                tbl.Columns.Add("DirectionCode");
                tbl.Columns.Add("SpecializationCode");
                tbl.Columns.Add("EntryYear");
                tbl.Columns.Add("EntryLine");
                tbl.Columns.Add("StudyLevel");
                tbl.Columns.Add("EntryCategory");
                tbl.Columns.Add("DateStartEduc");
                tbl.Columns.Add("StudyBasis");
                tbl.Columns.Add("StudyPlan");
                tbl.Columns.Add("StudyNumber");
                tbl.Columns.Add("Course");
                tbl.Columns.Add("Semester");
                tbl.Columns.Add("StudyForm");
                tbl.Columns.Add("EntryOrderDocumentType");
                tbl.Columns.Add("EntryOrderNumber");
                tbl.Columns.Add("EntryOrderDate");
                tbl.Columns.Add("EducationDocumentType");
                tbl.Columns.Add("EducationDocumentSeries");
                tbl.Columns.Add("EducationDocumentNumber");
                tbl.Columns.Add("EducationDocumentAuthor");
                tbl.Columns.Add("EducationDocumentDate", typeof(DateTime));

                foreach (var ent in exportData)
                {
                    DataRow rw = tbl.NewRow();

                    rw["ID"] = ent.PersonId;
                    rw["Surname"] = ent.Surname;
                    rw["Name"] = ent.Name;
                    rw["SecondName"] = ent.SecondName;
                    rw["OldSurname"] = "";
                    rw["NoSecondName"] = string.IsNullOrEmpty(ent.SecondName);
                    rw["Sex"] = ent.Sex;

                    rw["BirthPlace"] = ent.BirthPlace;
                    rw["BirthDate"] = ent.BirthDate;
                    rw["Nationality"] = ent.OSKMCode;
                    rw["Region"] = ent.RegionNumber;
                    rw["Address"] = ent.Code + " " + ent.City + " " + ent.Street + " " + ent.House + " " + ent.Flat;
                    rw["Login"] = ent.Login;
                    rw["Emiail"] = ent.Email;
                    rw["Phone"] = ent.Phone;
                    rw["SNILS"] = ent.SNILS;

                    rw["DocumentType"] = ent.PassportTypeName;
                    rw["DocumentSeries"] = ent.PassportSeries;
                    rw["DocumentNumber"] = ent.PassportNumber;
                    rw["DocumentAuthor"] = ent.PassportAuthor;
                    rw["DocumentDate"] = ent.PassportDate;

                    rw["Faculty"] = ent.FacultyName;
                    rw["ObrazProgramNumber"] = ent.ObrazProgramCrypt;
                    rw["Profile"] = ent.ProfileName == "нет" ? null : ent.ProfileName;
                    rw["DirectionCode"] = ent.LicenseProgramCode;
                    rw["SpecializationCode"] = ent.LicenseProgramCode;
                    rw["EntryYear"] = MainClass.iPriemYear;
                    //rw["EntryLine"] = ent.StudyBasisId == 2 ? "контракт" : (ent.IsForeign ? "гослиния" : "равный прием");
                    rw["EntryLine"] = ent.StudyBasisId == 2 ? "контракт" : (ent.IsForeign ? "гослиния" : "госбюджет");
                    rw["StudyLevel"] = ent.StudyLevelName;
                    rw["EntryCategory"] = ent.StudyBasisId == 2 ? "контракт" : (ent.IsForeign ? "гослиния" : "равный прием");
                    //rw["EntryCategory"] = ent.StudyBasisId == 2 ? "контракт" : (ent.IsForeign ? "гослиния" : "госбюджет");
                    rw["DateStartEduc"] = ent.DateStartEduc ?? new DateTime(MainClass.iPriemYear, 9, 1);
                    rw["StudyBasis"] = ent.StudyBasisId == 1 ? "бюджет" : "платно";
                    rw["StudyPlan"] = ent.StudyPlanNumber;
                    rw["StudyNumber"] = ent.StudyNumber;
                    rw["Course"] = "1";
                    rw["Semester"] = "1";
                    rw["StudyForm"] = (ent.StudyFormId == 1 ? "дневное" : (ent.StudyFormId == 3 ? "заочное" : "вечернее"));

                    rw["EntryOrderDocumentType"] = "приказ Первого проректора по учебной, внеучебной и учебно-методической работе";

                    string sOrderNum = ent.NationalityId == 1 ? ent.OrderNum : ent.OrderNumFor;
                    if (string.IsNullOrEmpty(sOrderNum))
                        continue;
                    rw["EntryOrderNumber"] = sOrderNum;

                    DateTime? dtOrderDate = ent.NationalityId == 1 ? ent.OrderDate : ent.OrderDateFor;
                    if (!dtOrderDate.HasValue)
                        continue;
                    rw["EntryOrderDate"] = dtOrderDate.Value;

                    rw["EducationDocumentType"] = ent.SchoolTypeId == 1 ? "Аттестат о среднем (полном) общем образовании" : "Диплом";
                    rw["EducationDocumentSeries"] = ent.SchoolTypeId == 1 ? ent.AttestatSeries : ent.DiplomSeries;
                    rw["EducationDocumentNumber"] = ent.SchoolTypeId == 1 ? ent.AttestatNum : ent.DiplomNum;
                    rw["EducationDocumentAuthor"] = ent.SchoolName;
                    rw["EducationDocumentDate"] = new DateTime(ent.SchoolExitYear == 0 ? DateTime.Now.Year : ent.SchoolExitYear, 6, 1);

                    tbl.Rows.Add(rw);
                }

                PrintClass.PrintAllToExcel2007(tbl, "Export");
            }
        }
    }
}
