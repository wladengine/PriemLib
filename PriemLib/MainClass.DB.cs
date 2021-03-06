﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data;

using BaseFormsLib;
using EducServLib;

namespace PriemLib
{
    public static partial class MainClass
    {
        public static bool GetIsOpen(string tableName, string itemId)
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    ObjectParameter entId = new ObjectParameter("result", typeof(bool));
                    context.Get_IsOpen(tableName, itemId, entId);

                    return Convert.ToBoolean(entId.Value);
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка в MainClass.GetIsOpen() ", ex);
                return true;
            }
        }

        public static string GetIsOpenHolder(string tableName, string itemId)
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    ObjectParameter entId = new ObjectParameter("result", typeof(string));
                    context.Get_OpenHolder(tableName, itemId, entId);

                    return entId.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка в MainClass.GetIsOpenHolder() ", ex);
                return "";
            }
        }

        public static string GetFacultyForAccount(string account)
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    List<int?> lstFacId = context.GetFacultyIdsByNameFunc(account).ToList();
                    string Facs = context.SP_Faculty.Where(x => lstFacId.Contains(x.Id))
                        .Select(x => x.Acronym).Distinct()
                        .ToList()
                        .OrderBy(x => x)
                        .DefaultIfEmpty("")
                        .Aggregate((x, tail) => x + ", " + tail);

                    return Facs;
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка в MainClass.GetFacultyForAccount() ", ex);
                return "";
            }
        }

        public static void SetIsOpen(string tableName, string itemId)
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    context.Set_IsOpen(tableName, itemId, true, userName);
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка в MainClass.SetIsOpen() ", ex);
            }
        }

        public static void DeleteIsOpen(string tableName, string itemId)
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    context.Set_IsOpen(tableName, itemId, false, null);
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка в MainClass.DeleteIsOpen() ", ex);
            }
        }

        public static void DeleteAllOpenByHolder()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    context.DeleteAllOpenByHolder(userName);
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка в MainClass.DeleteAllOpenByHolder() \n", ex);
            }
        }

        public static void Delete(string tableName, string itemId)
        {
            try
            {
                if (!IsReadOnly())
                {
                    using (PriemEntities context = new PriemEntities())
                    {
                        context.Delete(tableName, itemId);
                    }
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка в MainClass.Delete() ", ex);
            }
        }

        public static bool GetHasMotivationLetter(int? abitBarcode, int? persBarcode)
        {
            try
            {
                BDClassLib.SQLClass InetDB = new BDClassLib.SQLClass();
                InetDB.OpenDatabase(MainClass.connStringOnline);

                string query = "SELECT COUNT(*) FROM qAbitFiles_OnlyEssayMotivLetter WHERE (ApplicationBarcode=@ApplicationBarcode OR PersonBarcode=@PersonBarcode) AND FileTypeId=2";
                int cnt = (int)InetDB.GetValue(query, new SortedList<string, object>() 
                {
                    { "@ApplicationBarcode", QueryServ.ToNullDB(abitBarcode) }, 
                    { "@PersonBarcode", QueryServ.ToNullDB(persBarcode) } 
                });

                return (cnt > 0);
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка в MainClass.GetHasMotivationLetter() ", ex);
                return false;
            }
        }

        public static bool GetHasEssay(int? abitBarcode, int? persBarcode)
        {
            try
            {
                BDClassLib.SQLClass InetDB = new BDClassLib.SQLClass();
                InetDB.OpenDatabase(MainClass.connStringOnline);

                string query = "SELECT COUNT(*) FROM qAbitFiles_OnlyEssayMotivLetter WHERE (ApplicationBarcode=@ApplicationBarcode OR PersonBarcode=@PersonBarcode) AND FileTypeId=3";
                int cnt = (int)InetDB.GetValue(query, new SortedList<string, object>() 
                {
                    { "@ApplicationBarcode", QueryServ.ToNullDB(abitBarcode) }, 
                    { "@PersonBarcode", QueryServ.ToNullDB(persBarcode) }  
                });

                return (cnt > 0);
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка в MainClass.GetHasEssay() ", ex);
                return false;
            }
        }

        public static void AddVal(this SortedList<string, object> sl, string key, object val)
        {
            if (val != null)
                sl.Add(key, val);
            else
                sl.Add(key, DBNull.Value);
        }

        //инициализация постороителя запросов для списка с фильтрами
        public static void InitQueryBuilder()
        {
            try
            {
                qBuilder = new QueryBuilder("ed.qAbiturient");

                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.PersonNum", "Ид_номер"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "qAbiturient.RegNum", "Рег_номер"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.Surname", "Фамилия"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.Name", "Имя"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.SecondName", "Отчество"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.FIO", "ФИО"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.BirthDate", "Дата_рождения"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.BirthPlace", "Место_рождения"));

                //Person Passport
                qBuilder.AddQueryItem(new QueryItem("ed.PassportType", "PassportType.Name", "Тип_паспорта"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.PassportSeries", "Серия_паспорта"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.PassportNumber", "Номер_паспорта"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.PassportCode", "Код_подразделения_паспорта"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.PersonalCode", "Личный_код_паспорт"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.SNILS", "СНИЛС"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.PassportAuthor", "Кем_выдан_паспорт"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.PassportDate", "Дата_выдачи_паспорта"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", QueryBuilder.GetBoolField("ed.extPerson.HasOriginals"), "Подал_подлинники_в_университет"));


                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", QueryBuilder.GetBoolField("ed.extPerson.Sex"), "Пол_мужской"));

                //Person Contacts
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.Phone", "Телефон"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.Email", "Email"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.Code + ' ' + extPerson.City + ' ' + extPerson.Street + (Case when extPerson.House = '' then '' else ' д.' + extPerson.House end) + (Case when extPerson.Korpus = '' then '' else ' к.' + extPerson.Korpus end) + (Case when extPerson.Flat = '' then '' else ' кв.' + extPerson.Flat end)", "Адрес_регистрации"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.CodeReal + ' ' + extPerson.CityReal + ' ' + extPerson.StreetReal + (Case when extPerson.HouseReal = '' then '' else ' д.' + extPerson.HouseReal end) + (Case when extPerson.KorpusReal = '' then '' else ' к.' + extPerson.KorpusReal end) + (Case when extPerson.FlatReal = '' then '' else ' кв.' + extPerson.FlatReal end)", "Адрес_проживания"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", QueryBuilder.GetBoolField("extPerson.HostelAbit"), "Предоставлять_общежитие_поступление"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", QueryBuilder.GetBoolField("extPerson.HostelEduc"), "Предоставлять_общежитие_обучение"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", QueryBuilder.GetBoolField("extPerson.HasAssignToHostel"), "Выдано_направление_на_поселение"));
                qBuilder.AddQueryItem(new QueryItem("HostelFaculty", "HostelFaculty.Name", "Факультет_выдавший_направление"));
                qBuilder.AddQueryItem(new QueryItem("ed.Country", "ed.Country.Name", "Страна"));
                qBuilder.AddQueryItem(new QueryItem("Nationality", "Nationality.Name", "Гражданство"));
                qBuilder.AddQueryItem(new QueryItem("ed.Region", "ed.Region.Name", "Регион"));

                //Person Education Info
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson_EducationInfo_Current", QueryBuilder.GetBoolField("extPerson_EducationInfo_Current.IsExcellent"), "Медалист"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson_EducationInfo_Current", "extPerson_EducationInfo_Current.SchoolCity", "Город_уч_заведения"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson_EducationInfo_Current", "extPerson_EducationInfo_Current.SchoolNum", "Номер_школы"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson_EducationInfo_Current", "extPerson_EducationInfo_Current.AttestatSeries", "Серия_атт"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson_EducationInfo_Current", "extPerson_EducationInfo_Current.AttestatNum", "Номер_атт"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson_EducationInfo_Current", "extPerson_EducationInfo_Current.SchoolAVG", "Средний_балл_атт"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson_EducationInfo_Current", "extPerson_EducationInfo_Current.DiplomSeries", "Серия_диплома"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson_EducationInfo_Current", "extPerson_EducationInfo_Current.DiplomNum", "Номер_диплома"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson_EducationInfo_Current", "extPerson_EducationInfo_Current.HEQualification", "Квалификация"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson_EducationInfo_Current", "extPerson_EducationInfo_Current.HighEducation", "Место_предыдущего_образования"));

                if (MainClass.dbType == PriemType.PriemMag || MainClass.dbType == PriemType.PriemAspirant)
                {
                    qBuilder.AddQueryItem(new QueryItem("ed.extPerson_EducationInfo_Current", "extPerson_EducationInfo_Current.HighEducation", "Название_уч_заведения"));
                    qBuilder.AddQueryItem(new QueryItem("ed.extPerson_EducationInfo_Current", "extPerson_EducationInfo_Current.HEProfession", "Направление_подготовки"));
                    qBuilder.AddQueryItem(new QueryItem("ed.extPerson_EducationInfo_Current", "extPerson_EducationInfo_Current.HEExitYear", "Год_выпуска"));
                }
                else
                {
                    qBuilder.AddQueryItem(new QueryItem("ed.extPerson_EducationInfo_Current", "extPerson_EducationInfo_Current.SchoolName", "Название_уч_заведения"));
                    qBuilder.AddQueryItem(new QueryItem("ed.extPerson_EducationInfo_Current", "extPerson_EducationInfo_Current.SchoolExitYear", "Год_выпуска"));
                }
                qBuilder.AddQueryItem(new QueryItem("ed.SchoolType", "ed.SchoolType.Name", "Тип_уч_заведения"));
                qBuilder.AddQueryItem(new QueryItem("ed.CountryEduc", "CountryEduc.Name", "Страна_получ_пред_образ"));
                qBuilder.AddQueryItem(new QueryItem("ed.extFBSStatus", "extFBSStatus.FBSStatus", "Статус_ФБС"));

                //Person Additional Info
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.Mobiles", "Доп_контакты"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.PersonInfo", "Данные_о_родителях"));
                qBuilder.AddQueryItem(new QueryItem("ed.Language", "ed.Language.Name", "Ин_язык"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.StartEnglish", "Англ_с_нуля"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "extPerson.EnglishMark", "Англ_оценка"));

                //Abiturient Info 
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", "extEntry.FacultyName", "Факультет"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", "extEntry.LicenseProgramName", "Направление"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", "extEntry.LicenseProgramCode", "Код_направления"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", "extEntry.ObrazProgramName", "Образ_программа"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", "extEntry.ProfileName", "Профиль"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", "extEntry.StudyFormName", "Форма_обучения"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", "extEntry.StudyBasisName", "Основа_обучения"));

                qBuilder.AddQueryItem(new QueryItem("ed.Competition", "ed.Competition.Name", "Тип_конкурса"));
                qBuilder.AddQueryItem(new QueryItem("ed.OtherCompetition", "OtherCompetition.Name", "Доп_тип_конкурса"));
                qBuilder.AddQueryItem(new QueryItem("ed.CelCompetition", "ed.CelCompetition.Name", "Целевик_тип"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", QueryBuilder.GetBoolField("qAbiturient.IsListener"), "Слушатель"));
                //qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", QueryBuilder.GetBoolField("ed.qAbiturient.WithHE"), "Имеющий_ВВ"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", QueryBuilder.GetBoolField("extEntry.IsSecond"), "Программы_для_ВО"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", QueryBuilder.GetBoolField("extEntry.IsReduced"), "Программы_сокр"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", QueryBuilder.GetBoolField("extEntry.IsCrimea"), "Крым"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", QueryBuilder.GetBoolField("extEntry.IsParallel"), "Программы_парал"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", QueryBuilder.GetBoolField("qAbiturient.IsPaid"), "Оплатил"));

                qBuilder.AddQueryItem(new QueryItem("ed.PaidData", "PaidData.DogovorNum", "Номер_договора"));
                qBuilder.AddQueryItem(new QueryItem("ed.PaidData", "PaidData.DogovorDate", "Дата_договора"));
                qBuilder.AddQueryItem(new QueryItem("ed.PaidData", "PaidData.Customer", "Заказчик_договора"));

                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", QueryBuilder.GetBoolField("qAbiturient.BackDoc"), "Забрал_док"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", QueryBuilder.GetBoolField("qAbiturient.HasOriginals"), "Подал_подлинники"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", QueryBuilder.GetBoolField("qAbiturient.Checked"), "Данные_проверены"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", QueryBuilder.GetBoolField("qAbiturient.IsViewed"), "Заявление_просмотрено"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "qAbiturient.StudyNumber", "Номер_зачетки"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "qAbiturient.BackDocDate", "Дата_возврата_док"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "qAbiturient.DocDate", "Дата_подачи_док"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "qAbiturient.Coefficient", "Коэффициент_полупрохода"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "qAbiturient.Priority", "Приоритет"));
                //qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "qAbiturient.SessionAVG", "Средний_балл_сессии"));
                //qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "qAbiturient.StudentStatus", "Статус_студента"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", "extEntry.StudyLevelName", "Уровень_поступления"));
                qBuilder.AddQueryItem(new QueryItem("ed.extInnerEntryInEntry", "'[' + extInnerEntryInEntry.ObrazProgramCrypt + '] ' + extInnerEntryInEntry.ObrazProgramName", "Образовательная_программа_зачисления"));
                qBuilder.AddQueryItem(new QueryItem("ed.extInnerEntryInEntry", "extInnerEntryInEntry.ProfileName", "Профиль_зачисления"));

                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "(select MAX(extOlympiads.OlympValueName) AS Value FROM ed.extOlympiads WHERE extOlympiads.PersonId = qAbiturient.PersonId)", "Степень_диплома"));

                // ЕГЭ
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select Top(1) EgeCertificate.Number FROM ed.EgeCertificate WHERE EgeCertificate.PersonId = qAbiturient.PersonId AND Year=2012)", "Свидетельство_ЕГЭ_2012"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select Top(1) EgeCertificate.Number FROM ed.EgeCertificate WHERE EgeCertificate.PersonId = qAbiturient.PersonId AND Year=2013)", "Свидетельство_ЕГЭ_2013"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", QueryBuilder.GetBoolField("(select Top(1) EgeCertificate.IsImported FROM ed.EgeCertificate WHERE EgeCertificate.PersonId = qAbiturient.PersonId AND Year=2012 )"), "Загружено_Свид-во_ЕГЭ_2012"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", QueryBuilder.GetBoolField("(select Top(1) EgeCertificate.IsImported FROM ed.EgeCertificate WHERE EgeCertificate.PersonId = qAbiturient.PersonId AND Year=2013 )"), "Загружено_Свид-во_ЕГЭ_2013"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", QueryBuilder.GetBoolField("(select Top(1) EgeCertificate.IsImported FROM ed.EgeCertificate WHERE EgeCertificate.PersonId = qAbiturient.PersonId AND Year=2014 )"), "Загружено_Свид-во_ЕГЭ_2014"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", QueryBuilder.GetBoolField("(select Top(1) EgeCertificate.IsImported FROM ed.EgeCertificate WHERE EgeCertificate.PersonId = qAbiturient.PersonId AND Year=2015 )"), "Загружено_Свид-во_ЕГЭ_2015"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", QueryBuilder.GetBoolField("(select Top(1) EgeCertificate.IsImported FROM ed.EgeCertificate WHERE EgeCertificate.PersonId = qAbiturient.PersonId AND Year=2016 )"), "Загружено_Свид-во_ЕГЭ_2016"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", QueryBuilder.GetBoolField("(select Top(1) EgeCertificate.IsImported FROM ed.EgeCertificate WHERE EgeCertificate.PersonId = qAbiturient.PersonId AND Year=2017 )"), "Загружено_Свид-во_ЕГЭ_2017"));

                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", "extEntry.ExamName1", "Первый_предмет_ВИ"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", "extEntry.ExamName2", "Второй_предмет_ВИ"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", "extEntry.ExamName3", "Третий_предмет_ВИ"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", "extEntry.ExamName4", "Четвертый_предмет_ВИ"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntry", "extEntry.ExamName5", "Пятый_предмет_ВИ"));

                //загрузка оценок по-старому
                //qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "(SELECT TOP 1 Value FROM ed.Mark INNER JOIN ed.extExamInEntry ON extExamInEntry.Id = Mark.ExamInEntryBlockUnitId WHERE Mark.AbiturientId = qAbiturient.Id AND extExamInEntry.OrderNumber = 1)", "Балл_Первый_предмет_ВИ"));
                //qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "(SELECT TOP 1 Value FROM ed.Mark INNER JOIN ed.extExamInEntry ON extExamInEntry.Id = Mark.ExamInEntryBlockUnitId WHERE Mark.AbiturientId = qAbiturient.Id AND extExamInEntry.OrderNumber = 2)", "Балл_Второй_предмет_ВИ"));
                //qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "(SELECT TOP 1 Value FROM ed.Mark INNER JOIN ed.extExamInEntry ON extExamInEntry.Id = Mark.ExamInEntryBlockUnitId WHERE Mark.AbiturientId = qAbiturient.Id AND extExamInEntry.OrderNumber = 3)", "Балл_Третий_предмет_ВИ"));
                //qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "(SELECT TOP 1 Value FROM ed.Mark INNER JOIN ed.extExamInEntry ON extExamInEntry.Id = Mark.ExamInEntryBlockUnitId WHERE Mark.AbiturientId = qAbiturient.Id AND extExamInEntry.OrderNumber = 3)", "Балл_Четвертый_предмет_ВИ"));
                //qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "(SELECT TOP 1 Value FROM ed.Mark INNER JOIN ed.extExamInEntry ON extExamInEntry.Id = Mark.ExamInEntryBlockUnitId WHERE Mark.AbiturientId = qAbiturient.Id AND extExamInEntry.OrderNumber = 3)", "Балл_Пятый_предмет_ВИ"));

                qBuilder.AddQueryItem(new QueryItem("ed.Abiturient_FetchValues", "Abiturient_FetchValues.MarkOrderNumber1", "Балл_Первый_предмет_ВИ"));
                qBuilder.AddQueryItem(new QueryItem("ed.Abiturient_FetchValues", "Abiturient_FetchValues.MarkOrderNumber2", "Балл_Второй_предмет_ВИ"));
                qBuilder.AddQueryItem(new QueryItem("ed.Abiturient_FetchValues", "Abiturient_FetchValues.MarkOrderNumber3", "Балл_Третий_предмет_ВИ"));
                qBuilder.AddQueryItem(new QueryItem("ed.Abiturient_FetchValues", "Abiturient_FetchValues.MarkOrderNumber4", "Балл_Четвертый_предмет_ВИ"));
                qBuilder.AddQueryItem(new QueryItem("ed.Abiturient_FetchValues", "Abiturient_FetchValues.MarkOrderNumber5", "Балл_Пятый_предмет_ВИ"));

                qBuilder.AddQueryItem(new QueryItem("ed.Person_FetchValues", "Person_FetchValues.EGE_Rus", "ЕГЭ_русск.язык"));
                qBuilder.AddQueryItem(new QueryItem("ed.Person_FetchValues", "Person_FetchValues.EGE_Math", "ЕГЭ_математика"));
                qBuilder.AddQueryItem(new QueryItem("ed.Person_FetchValues", "Person_FetchValues.EGE_Fiz", "ЕГЭ_физика"));
                qBuilder.AddQueryItem(new QueryItem("ed.Person_FetchValues", "Person_FetchValues.EGE_Chem", "ЕГЭ_химия"));
                qBuilder.AddQueryItem(new QueryItem("ed.Person_FetchValues", "Person_FetchValues.EGE_Bio", "ЕГЭ_биология"));
                qBuilder.AddQueryItem(new QueryItem("ed.Person_FetchValues", "Person_FetchValues.EGE_Ist", "ЕГЭ_история"));
                qBuilder.AddQueryItem(new QueryItem("ed.Person_FetchValues", "Person_FetchValues.EGE_Geogr", "ЕГЭ_география"));
                qBuilder.AddQueryItem(new QueryItem("ed.Person_FetchValues", "Person_FetchValues.EGE_Eng", "ЕГЭ_англ.яз"));
                qBuilder.AddQueryItem(new QueryItem("ed.Person_FetchValues", "Person_FetchValues.EGE_Nem", "ЕГЭ_немец.язык"));
                qBuilder.AddQueryItem(new QueryItem("ed.Person_FetchValues", "Person_FetchValues.EGE_Fran", "ЕГЭ_франц.язык"));
                qBuilder.AddQueryItem(new QueryItem("ed.Person_FetchValues", "Person_FetchValues.EGE_Obsh", "ЕГЭ_обществознание"));
                qBuilder.AddQueryItem(new QueryItem("ed.Person_FetchValues", "Person_FetchValues.EGE_Lit", "ЕГЭ_литература"));
                qBuilder.AddQueryItem(new QueryItem("ed.Person_FetchValues", "Person_FetchValues.EGE_Esp", "ЕГЭ_испан.язык"));
                qBuilder.AddQueryItem(new QueryItem("ed.Person_FetchValues", "Person_FetchValues.EGE_Inform", "ЕГЭ_информатика"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MAX(EgeMark.value) as mark FROM ed.EgeMark INNER JOIN ed.EgeCertificate ON EgeMark.EgeCertificateId = EgeCertificate.Id WHERE EgeCertificate.PersonId = extPerson.Id AND EgeExamNameId=15)", "ЕГЭ_Сочинение"));

                //Олимпиады
                //qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when(SELECT count(*) FROM Olympiads WHERE OlympLevelId=1 and Olympiads.AbiturientId=ed.qAbiturient.id)>0 then 'Да' else 'Нет' end", "Международная"));
                //qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when(SELECT count(*) FROM Olympiads WHERE OlympLevelId=2 and Olympiads.AbiturientId=ed.qAbiturient.id)>0 then 'Да' else 'Нет' end", "Всероссийкая"));
                //qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when(SELECT count(*) FROM Olympiads WHERE OlympLevelId=4 and Olympiads.AbiturientId=ed.qAbiturient.id)>0 then 'Да' else 'Нет' end", "Межвузовская"));
                //qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when(SELECT count(*) FROM Olympiads WHERE OlympLevelId=5 and Olympiads.AbiturientId=ed.qAbiturient.id)>0 then 'Да' else 'Нет' end", "СПбГУ"));
                //qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when(SELECT count(*) FROM Olympiads WHERE OlympLevelId=3 and Olympiads.AbiturientId=ed.qAbiturient.id)>0 then 'Да' else 'Нет' end", "Региональная"));
                //qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when(SELECT count(*) FROM Olympiads WHERE OlympLevelId=9 and Olympiads.AbiturientId=ed.qAbiturient.id)>0 then 'Да' else 'Нет' end", "Школьников"));

                //Привилегии
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when (extPerson.Privileges & 1)>0 then 'Да' else 'Нет' end", "сирота"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when (extPerson.Privileges & 2)>0 then 'Да' else 'Нет' end", "чернобылец"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when (extPerson.Privileges & 4)>0 then 'Да' else 'Нет' end", "военнослужащий"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when (extPerson.Privileges & 16)>0 then 'Да' else 'Нет' end", "полусирота"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when (extPerson.Privileges & 32)>0 then 'Да' else 'Нет' end", "инвалид"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when (extPerson.Privileges & 64)>0 then 'Да' else 'Нет' end", "уч.боев."));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when (extPerson.Privileges & 128)>0 then 'Да' else 'Нет' end", "стажник"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when (extPerson.Privileges & 256)>0 then 'Да' else 'Нет' end", "реб.-сирота"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when (extPerson.Privileges & 512)>0 then 'Да' else 'Нет' end", "огр.возможности"));

                //инд достижения
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when EXISTS (SELECT * FROM ed.PersonAchievement PA WHERE PA.PersonId = extPerson.Id AND PA.AchievementTypeId = 9) then 'Да' else 'Нет' end", "ИНД_Аттестат"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when EXISTS (SELECT * FROM ed.PersonAchievement PA WHERE PA.PersonId = extPerson.Id AND PA.AchievementTypeId = 8) then 'Да' else 'Нет' end", "ИНД_ГТО"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when EXISTS (SELECT * FROM ed.PersonAchievement PA WHERE PA.PersonId = extPerson.Id AND PA.AchievementTypeId = 16) then 'Да' else 'Нет' end", "ИНД_СПО"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "(SELECT AddSumm.AdditionalMarksSum FROM ed.extAbitAdditionalMarksSum AddSumm WHERE AddSumm.AbiturientId = qAbiturient.Id)", "ИНД_СУММ"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "(SELECT AddSumm.AdditionalMarksSum FROM ed.extAbitAdditionalMarksSum_FAST AddSumm WHERE AddSumm.AbiturientId = qAbiturient.Id)", "ИД:сумма"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "case when EXISTS (SELECT * FROM ed.extAbitAllAdditionalAchievements OL WHERE OL.AbiturientId = qAbiturient.Id AND OL.AchievementTypeId = 11 AND OL.Mark = 4) then 'Да' else 'Нет' end", "ИНД_ПобРег"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "case when EXISTS (SELECT * FROM ed.extAbitAllAdditionalAchievements OL WHERE OL.AbiturientId = qAbiturient.Id AND OL.AchievementTypeId = 11 AND OL.Mark = 3) then 'Да' else 'Нет' end", "ИНД_ПризРег"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "case when EXISTS (SELECT * FROM ed.extAbitAllAdditionalAchievements OL WHERE OL.AbiturientId = qAbiturient.Id AND OL.AchievementTypeId = 11 AND OL.Mark = 2) then 'Да' else 'Нет' end", "ИНД_ОлСПбГУ"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "case when EXISTS (SELECT * FROM ed.extAbitAllAdditionalAchievements OL WHERE OL.AbiturientId = qAbiturient.Id AND OL.AchievementTypeId = 11 AND OL.Mark = 1) then 'Да' else 'Нет' end", "ИНД_ПрочРСОШ"));
                //qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "case when EXISTS (SELECT * FROM ed.PersonAchievement PA WHERE PA.PersonId = extPerson.Id AND PA.AchievementTypeId = 11) then 'Да' else 'Нет' end", "ИНД_Конкурсы"));

                //Протоколы
                qBuilder.AddQueryItem(new QueryItem("ed.extEnableProtocol", "ed.extEnableProtocol.Number", "Протокол_о_допуске"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntryView", "ed.extEntryView.Number", "Представление"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntryView", "ed.extEntryView.OrderNum", "Номер_приказа_о_зачислении"));
                qBuilder.AddQueryItem(new QueryItem("ed.extEntryView", "ed.extEntryView.OrderDate", "Дата_приказа_о_зачислении"));

                //qBuilder.AddQueryItem(new QueryItem("ed.extEntryView", "ed.extEntryView.OrderNumFor", "Номер_приказа_о_зачислении_иностр"));
                //qBuilder.AddQueryItem(new QueryItem("ed.extEntryView", "ed.extEntryView.OrderDateFor", "Дата_приказа_о_зачислении_иностр"));

                //Сумма баллов
                //qBuilder.AddQueryItem(new QueryItem("ed.extAbitMarksSum", "extAbitMarksSum.TotalSum", "Сумма_баллов"));
                qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", "qAbiturient.Sum", "Сумма_баллов"));

                //экзамены 
                //DataSet dsExams = _bdc.GetDataSet("SELECT DISTINCT ed.extExamInEntry.ExamId AS Id, ed.extExamInEntry.ExamName AS Name FROM ed.extExamInEntry");

                using (PriemEntities context = new PriemEntities())
                {
                    var dicExams = context.extExamInEntry.Select(x => new { x.ExamId, x.ExamName }).Distinct().ToList();
                    //from DataRow rwx in dsExams.Tables[0].Rows
                    //select new { Id = rwx["Id"], Name = rwx["Name"] };

                    foreach (var ex in dicExams)
                        qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", string.Format("(select Sum(Mark.Value) FROM ed.Mark INNER JOIN ed.extExamInEntry ON Mark.ExamInEntryBlockUnitId = extExamInEntry.Id WHERE AbiturientId = qAbiturient.Id AND extExamInEntry.ExamId={0})", ex.ExamId), ex.ExamName));
                }
                // Оценки из аттестата
                //qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select Min (AttMarks.value) as mark FROM ed.AttMarks WHERE ed.AttMarks.PersonId = ed.extPerson.Id AND AttSubjectId=	1)", "Аттестат_алгебра"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 1)", "Аттестат_алгебра"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 2)", "Аттестат_англ_язык"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 3)", "Аттестат_астрономия"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 4)", "Аттестат_биология"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 5)", "Аттестат_вселенная_чел"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 6)", "Аттестат_вс_история"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 7)", "Аттестат_география"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 8)", "Аттестат_геометрия"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 9)", "Аттестат_информатика"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 10)", "Аттестат_история_Спб"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 11)", "Аттестат_ист_России"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 12)", "Аттестат_литература"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 13)", "Аттестат_мировая_худ_культура"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 14)", "Аттестат_обществознание"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 15)", "Аттестат_ОБЖ"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 16)", "Аттестат_русск_язык"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 17)", "Аттестат_технология"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 18)", "Аттестат_физика"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 19)", "Аттестат_физ_культура"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 20)", "Аттестат_химия"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 21)", "Аттестат_экология"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 22)", "Аттестат_немецкий_язык"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 23)", "Аттестат_испанский_язык"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 24)", "Аттестат_французский_язык"));
                qBuilder.AddQueryItem(new QueryItem("ed.extPerson", "(select MIN(Value) FROM ed.AttMarks WHERE AttMarks.PersonId = extPerson.Id AND AttSubjectId = 25)", "Аттестат_итальянский_язык"));

                using (PriemEntities context = new PriemEntities())
                {
                    var lstBlocksToSelect = context.ExamInEntryBlock.Where(x => x.ExamInEntryBlockUnit.Count() > 1).Select(x => x.Id);

                    var lstExams =
                        (from ExBlockUnit in context.ExamInEntryBlockUnit
                         join Ex in context.Exam on ExBlockUnit.ExamId equals Ex.Id
                         join ExName in context.ExamName on Ex.ExamNameId equals ExName.Id
                         where lstBlocksToSelect.Contains(ExBlockUnit.ExamInEntryBlockId)
                         select new { Ex.Id, ExName.Name }).ToList().Distinct();
                    foreach (var Ex in lstExams)
                    {
                        qBuilder.AddQueryItem(new QueryItem("ed.qAbiturient", @"case when EXISTS (SELECT * 
FROM ed.AbiturientSelectedExam join ed.ExamInEntryBlockUnit unit on AbiturientSelectedExam.ExamInEntryBlockUnitId = unit.Id
WHERE ed.AbiturientSelectedExam.ApplicationId = qAbiturient.Id AND unit.ExamId = " + Ex.Id + " ) then 'Да' else 'Нет' end", "Экзамен_по_выбору_" + Ex.Name.Replace(" ", "_")));
                    }
                }

                //JOIN-ы
                qBuilder.AddTableJoint("ed.Person", " LEFT JOIN ed.Person ON ed.qAbiturient.PersonId = ed.Person.Id ");
                qBuilder.AddTableJoint("ed.extPerson_EducationInfo_Current", " LEFT JOIN ed.extPerson_EducationInfo_Current ON ed.extPerson_EducationInfo_Current.PersonId = ed.qAbiturient.PersonId ");
                qBuilder.AddTableJoint("ed.extPerson", " INNER JOIN ed.extPerson ON ed.qAbiturient.PersonId = ed.extPerson.Id ");
                qBuilder.AddTableJoint("ed.PersonAchievement", " LEFT JOIN ed.PersonAchievement ON ed.PersonAchievement.PersonId = ed.Person.Id ");

                //использование extAbit крайне нежелательно, т.к. провоцирует перегруз
                qBuilder.AddTableJoint("ed.extAbit", " INNER JOIN ed.extAbit ON ed.qAbiturient.Id = ed.extAbit.Id ");

                qBuilder.AddTableJoint("ed.PassportType", " LEFT JOIN ed.PassportType ON ed.PassportType.Id = ed.extPerson.PassportTypeId ");
                qBuilder.AddTableJoint("ed.Country", " LEFT JOIN ed.Country ON ed.extPerson.CountryId = ed.Country.Id ");
                if (dbType == PriemType.PriemForeigners)
                    qBuilder.AddTableJoint("Nationality", " LEFT JOIN ed.ForeignCountry AS Nationality ON ed.extPerson.ForeignNationalityId = Nationality.Id ");
                else
                    qBuilder.AddTableJoint("Nationality", " LEFT JOIN ed.Country AS Nationality ON ed.extPerson.NationalityId = Nationality.Id ");

                qBuilder.AddTableJoint("ed.PaidData", " LEFT JOIN ed.PaidData ON ed.qAbiturient.Id = ed.PaidData.AbiturientId ");

                qBuilder.AddTableJoint("ed.Region", " LEFT JOIN ed.Region ON ed.extPerson.RegionId = ed.Region.Id ");
                qBuilder.AddTableJoint("ed.Language", " LEFT JOIN ed.[Language] ON ed.qAbiturient.LanguageId = ed.[Language].Id ");
                qBuilder.AddTableJoint("ed.SchoolType", " LEFT JOIN ed.SchoolType ON ed.extPerson_EducationInfo_Current.SchoolTypeId = ed.SchoolType.Id ");
                qBuilder.AddTableJoint("ed.CountryEduc", " LEFT JOIN ed.Country AS CountryEduc ON ed.extPerson_EducationInfo_Current.CountryEducId = CountryEduc.Id ");
                qBuilder.AddTableJoint("HostelFaculty", " LEFT JOIN ed.SP_Faculty AS HostelFaculty ON ed.extPerson.HostelFacultyId = HostelFaculty.Id ");
                qBuilder.AddTableJoint("ed.extFBSStatus", " LEFT JOIN ed.extFBSStatus ON ed.extFBSStatus.PersonId = ed.extPerson.Id ");
                qBuilder.AddTableJoint("ed.Abiturient_FetchValues", " LEFT JOIN ed.Abiturient_FetchValues ON Abiturient_FetchValues.AbiturientId = qAbiturient.Id ");
                qBuilder.AddTableJoint("ed.Person_FetchValues", " LEFT JOIN ed.Person_FetchValues ON Person_FetchValues.Id = qAbiturient.PersonId ");

                qBuilder.AddTableJoint("ed.SP_Faculty", " LEFT JOIN ed.SP_Faculty ON SP_Faculty.Id = qAbiturient.FacultyId ");
                qBuilder.AddTableJoint("ed.SP_LicenseProgram", " LEFT JOIN ed.SP_LicenseProgram ON SP_LicenseProgram.Id = qAbiturient.LicenseProgramId ");
                qBuilder.AddTableJoint("ed.SP_ObrazProgram", " LEFT JOIN ed.SP_ObrazProgram ON SP_ObrazProgram.Id = qAbiturient.ObrazProgramId ");
                qBuilder.AddTableJoint("ed.SP_Profile", " LEFT JOIN ed.SP_Profile ON SP_Profile.Id = qAbiturient.ProfileId ");
                qBuilder.AddTableJoint("ed.StudyBasis", " LEFT JOIN ed.StudyBasis ON StudyBasis.Id = qAbiturient.StudyBasisId ");
                qBuilder.AddTableJoint("ed.StudyForm", " LEFT JOIN ed.StudyForm ON StudyForm.Id = qAbiturient.StudyFormId ");
                qBuilder.AddTableJoint("ed.StudyLevel", " LEFT JOIN ed.StudyLevel ON StudyLevel.Id = qAbiturient.StudyLevelId ");
                qBuilder.AddTableJoint("ed.Competition", " LEFT JOIN ed.Competition ON Competition.Id = qAbiturient.CompetitionId ");
                qBuilder.AddTableJoint("ed.OtherCompetition", " LEFT JOIN ed.Competition AS OtherCompetition ON qAbiturient.OtherCompetitionId = OtherCompetition.Id ");
                qBuilder.AddTableJoint("ed.CelCompetition", " LEFT JOIN ed.CelCompetition ON qAbiturient.CelCompetitionId = CelCompetition.Id ");

                qBuilder.AddTableJoint("ed.extInnerEntryInEntry", " LEFT JOIN ed.extInnerEntryInEntry ON extInnerEntryInEntry.Id = qAbiturient.InnerEntryInEntryId ");
                qBuilder.AddTableJoint("ed.extEnableProtocol", " LEFT JOIN ed.extEnableProtocol ON extEnableProtocol.AbiturientId = qAbiturient.Id ");
                qBuilder.AddTableJoint("ed.extEntryView", " LEFT JOIN ed.extEntryView ON extEntryView.AbiturientId = qAbiturient.Id ");
                qBuilder.AddTableJoint("ed.extEntry", " INNER JOIN ed.extEntry ON extEntry.Id = qAbiturient.EntryId ");
                //qBuilder.AddTableJoint("ed.extAbitMarksSum", " LEFT JOIN ed.extAbitMarksSum ON ed.extAbitMarksSum.Id = ed.qAbiturient.Id ");
                qBuilder.AddTableJoint("ed.AbiturientSelectedExam", " LEFT JOIN ed.AbiturientSelectedExam ON ed.AbiturientSelectedExam.ApplicationId = ed.qAbiturient.Id LEFT JOIN ed.ExamInEntryBlockUnit on ExamInEntryBlockUnit.Id = ExamInEntryBlockUnitId LEFT JOIN ed.Exam on Exam.Id = ExamInEntryBlockUnit.ExamId ");

            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка при инициализации построителя запросов", ex);
            }
        }
    }
}
