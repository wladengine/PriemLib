﻿using System;
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
                for (int SLGr = 1; SLGr <= 4; SLGr++)
                {
                    //взять максимум номера, если еще ничего не назначено
                    string num =
                        (from ab in context.qAbitAll
                         where ab.StudyLevelGroupId == SLGr && !ab.IsForeign
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
                    if (Pref == 4)
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
    }
}
