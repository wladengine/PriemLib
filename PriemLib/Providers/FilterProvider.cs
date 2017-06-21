using EducServLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace PriemLib
{
    public abstract class AbstractFilterProvider
    {
        public abstract SortedList<string, string> GetColumns(DBPriem _bdc, string _fac);
        protected abstract SortedList<string, string> GetBaseColumns(DBPriem _bdc, string _fac);
        protected abstract SortedList<string, string> GetOtherColumns(DBPriem _bdc, string _fac);

        public abstract List<FilterItem> GetFilterList(DBPriem _bdc);
        protected abstract List<FilterItem> GetBaseFilterList(DBPriem _bdc);
        protected abstract List<FilterItem> GetOtherFilterList(DBPriem _bdc);

        public abstract List<ListItem> GetGroups(DBPriem _bdc);
        protected abstract List<ListItem> GetBaseGroups(DBPriem _bdc);
        protected abstract List<ListItem> GetOtherGroups(DBPriem _bdc);
    }

    public abstract class AbstractFilterProvider<T>: AbstractFilterProvider where T: Common_Classes.PriemTypeClass
    {
        public override SortedList<string, string> GetColumns(DBPriem _bdc, string _fac)
        {
            SortedList<string, string> list = GetBaseColumns(_bdc, _fac);
            foreach (var kvp in GetOtherColumns(_bdc, _fac))
                list.Add(kvp.Key, kvp.Value);

            return list;
        }
        protected override SortedList<string, string> GetBaseColumns(DBPriem _bdc, string _fac)
        {
            SortedList<string, string> list = new SortedList<string, string>();
            list.Add("Рег_номер", "Рег. номер");
            list.Add("Ид_номер", "Идент. номер");
            list.Add("Фамилия", "Фамилия");
            list.Add("Имя", "Имя");
            list.Add("Отчество", "Отчество");
            list.Add("ФИО", "ФИО");
            list.Add("Дата_рождения", "Дата рождения");
            list.Add("Место_рождения", "Место рождения");
            list.Add("Тип_паспорта", "Тип паспорта");
            list.Add("Серия_паспорта", "Серия паспорта");
            list.Add("Номер_паспорта", "Номер паспорта");
            list.Add("СНИЛС", "СНИЛС");
            list.Add("Кем_выдан_паспорт", "Кем выдан паспорт");
            list.Add("Дата_выдачи_паспорта", "Дата выдачи паспорта");
            list.Add("Код_подразделения_паспорта", "Код подразделения (паспорт)");
            list.Add("Личный_код_паспорт", "Личный код (паспорт)");
            list.Add("Пол_мужской", "Пол");
            list.Add("Телефон", "Телефон");
            list.Add("Email", "E-mail");
            list.Add("Адрес_регистрации", "Адрес регистрации");
            list.Add("Адрес_проживания", "Адрес проживания");
            list.Add("Страна", "Страна");
            list.Add("Гражданство", "Гражданство");
            list.Add("Регион", "Регион");
            list.Add("Предоставлять_общежитие_поступление", "Предоставлять общежитие на время поступления");
            list.Add("Выдано_направление_на_поселение", "Выдано направление на поселение");
            list.Add("Факультет_выдавший_направление", "Факультет, выдавший направление");
            list.Add("Ин_язык", "Ин. язык испытания");
            list.Add("Страна_получ_пред_образ", "Страна получения пред.образования");
            list.Add("Название_уч_заведения", "Название уч. заведения");
            list.Add("Предоставлять_общежитие_обучение", "Предоставлять общежитие на время обучения");
            list.Add("Протокол_о_допуске", "Протокол о допуске");
            list.Add("Представление", "Представление к зачислению");
            list.Add("Коэффициент_полупрохода", "Рейтинговый коэффициент");
            list.Add("Сумма_баллов", "Сумма баллов");
            list.Add("Номер_зачетки", "Номер зачетной книжки");
            list.Add("Целевик_тип", "Тип целевика");
            list.Add("Средний_балл_сессии", "Средний балл сессии");
            list.Add("Статус_студента", "Статус студента");
            list.Add("Приоритет", "Приоритет");
            list.Add("Доп_контакты", "Доп. контакты");
            list.Add("Данные_о_родителях", "Данные о родителях");

            list.Add("Образовательная_программа_зачисления", "Образовательная программа зачисления");
            list.Add("Профиль_зачисления", "Профиль зачисления");

            list.Add("Слушатель", "Слушатель");
            list.Add("Оплатил", "Оплатил");
            list.Add("Номер_договора", "Договор (номер)");
            list.Add("Дата_договора", "Договор (дата)");
            list.Add("Заказчик_договора", "Договор (заказчик)");


            list.Add("Забрал_док", "Забрал док.");
            list.Add("Заявление_просмотрено", "Заявление просмотрено");
            list.Add("Данные_проверены", "Данные проверены");
            list.Add("Дата_возврата_док", "Дата возврата док.");
            list.Add("Дата_подачи_док", "Дата подачи док.");
            list.Add("Поданы_подлинник_атт", "Подан подлинник документа об образовании");
            list.Add("Подал_подлинники", "Подал подлинники на заявление");
            list.Add("Подал_подлинники_в_университет", "Подал подлинники в университет");

            list.Add("Факультет", "Факультет");
            list.Add("Направление", "Направление");
            list.Add("Образ_программа", "Образовательная программа");
            list.Add("Код_направления", "Код направления");
            list.Add("Форма_обучения", "Форма обучения");
            list.Add("Основа_обучения", "Основа обучения");
            list.Add("Тип_конкурса", "Тип конкурса");
            list.Add("Доп_тип_конкурса", "Доп. тип конкурса");

            list.Add("Крым", "Крым");
            list.Add("сирота", "сирота");
            list.Add("чернобылец", "чернобылец");
            list.Add("военнослужащий", "военнослужащий");
            list.Add("полусирота", "полусирота");
            list.Add("инвалид", "инвалид");
            list.Add("уч.боев.", "уч.боев.");
            list.Add("стажник", "стажник");
            list.Add("реб.-сирота", "реб.-сирота");
            list.Add("огр.возможности", "огр.возможности");

            list.Add("Первый_предмет_ВИ", "Первый предмет ВИ");
            list.Add("Второй_предмет_ВИ", "Второй предмет ВИ");
            list.Add("Третий_предмет_ВИ", "Третий предмет ВИ");
            list.Add("Четвертый_предмет_ВИ", "Четвертый предмет ВИ");
            list.Add("Пятый_предмет_ВИ", "Пятый предмет ВИ");

            list.Add("Балл_Первый_предмет_ВИ", "Балл Первый предмет ВИ");
            list.Add("Балл_Второй_предмет_ВИ", "Балл Второй предмет ВИ");
            list.Add("Балл_Третий_предмет_ВИ", "Балл Третий предмет ВИ");
            list.Add("Балл_Четвертый_предмет_ВИ", "Балл Четвертый предмет ВИ");
            list.Add("Балл_Пятый_предмет_ВИ", "Балл Пятый предмет ВИ");

            list.Add("Англ_с_нуля", "Желает изучать англ с нуля");
            list.Add("Англ_оценка", "Итог. оценка по англ");

            list.Add("Номер_приказа_о_зачислении", "Номер приказа о зачислении");
            list.Add("Дата_приказа_о_зачислении", "Дата приказа о зачислении");
            list.Add("Номер_приказа_о_зачислении_иностр", "Номер приказа о зачислении (иностр)");
            list.Add("Дата_приказа_о_зачислении_иностр", "Дата приказа о зачислении (иностр)");

            return list;
        }
        protected override abstract SortedList<string, string> GetOtherColumns(DBPriem _bdc, string _fac);

        public override List<FilterItem> GetFilterList(DBPriem _bdc)
        {
            List<FilterItem> lst = GetBaseFilterList(_bdc);
            lst.AddRange(GetOtherFilterList(_bdc));

            return lst;
        }
        protected override List<FilterItem> GetBaseFilterList(DBPriem _bdc)
        {
            List<FilterItem> lst = new List<FilterItem>();

            //абитуриент
            lst.Add(new FilterItem("Приоритет", FilterType.FromTo, "ed.qAbiturient.Priority", "ed.qAbiturient"));
            lst.Add(new FilterItem("Ид_номер", FilterType.FromTo, "ed.extPerson.PersonNum", "ed.extPerson"));
            lst.Add(new FilterItem("Рег_номер", FilterType.FromTo, "ed.qAbiturient.RegNum", "ed.qAbiturient"));
            lst.Add(new FilterItem("Номер зачетки", FilterType.FromTo, "ed.qAbiturient.StudyNumber", "ed.qAbiturient"));
            lst.Add(new FilterItem("Фамилия", FilterType.Text, "ed.extPerson.Surname", "ed.extPerson"));
            lst.Add(new FilterItem("Имя", FilterType.Text, "ed.extPerson.Name", "ed.extPerson"));
            lst.Add(new FilterItem("Отчество", FilterType.Text, "ed.extPerson.SecondName", "ed.extPerson"));
            lst.Add(new FilterItem("Дата Рождения", FilterType.DateFromTo, "ed.extPerson.BirthDate", "ed.extPerson"));
            lst.Add(new FilterItem("Номер паспорта", FilterType.FromTo, "ed.extPerson.PassportNumber", "ed.extPerson"));
            lst.Add(new FilterItem("Серия паспорта", FilterType.FromTo, "ed.extPerson.PassportSeries", "ed.extPerson"));
            lst.Add(new FilterItem("Дата выдачи паспорта", FilterType.DateFromTo, "ed.extPerson.PassportDate", "ed.extPerson"));
            lst.Add(new FilterItem("Пол мужской?", FilterType.Bool, "ed.extPerson.Sex", "ed.extPerson"));
            lst.Add(new FilterItem("Страна", FilterType.Multi, "ed.extPerson.CountryId", "ed.extPerson", " SELECT ed.Country.Id, ed.Country.Name FROM ed.Country ORDER BY Name "));
            lst.Add(new FilterItem("Гражданство", FilterType.Multi, "ed.extPerson.NationalityId", "ed.extPerson", " SELECT ed.Country.Id, ed.Country.Name FROM ed.Country ORDER BY Name "));
            lst.Add(new FilterItem("Регион", FilterType.Multi, "ed.extPerson.RegionId", "ed.extPerson", " SELECT Id, Name FROM ed.Region "));
            lst.Add(new FilterItem("Телефон", FilterType.Text, "ed.extPerson.Phone", "ed.extPerson"));
            lst.Add(new FilterItem("Предоставлять общежитие на время поступления", FilterType.Bool, "ed.extPerson.HostelAbit", "ed.extPerson"));
            lst.Add(new FilterItem("Выдано направление на поселение", FilterType.Bool, "ed.extPerson.HasAssignToHostel", "ed.extPerson"));
            lst.Add(new FilterItem("Выдан экзаменационный пропуск", FilterType.Bool, "ed.extPerson.HasExamPass", "ed.extPerson"));
            lst.Add(new FilterItem("Факультет, выдавший направление", FilterType.Multi, "ed.extPerson.HostelFacultyId", "ed.extPerson", " SELECT Id, Name FROM ed.SP_Faculty "));
            lst.Add(new FilterItem("Желает изучать англ с нуля", FilterType.Bool, "ed.extPerson.StartEnglish", "ed.extPerson"));
            lst.Add(new FilterItem("Сдает ЕГЭ в СПбГУ", FilterType.Bool, "ed.extPerson.EgeInSpbgu", "ed.extPerson"));
            lst.Add(new FilterItem("Факультет", FilterType.Multi, "ed.qAbiturient.FacultyId", "ed.qAbiturient", " SELECT Id, Name FROM ed.qFaculty "));
            lst.Add(new FilterItem("Форма обучения", FilterType.Multi, "ed.qAbiturient.StudyFormId", "ed.qAbiturient", "SELECT Id, Name FROM ed.StudyForm "));
            lst.Add(new FilterItem("Основа обучения", FilterType.Multi, "ed.qAbiturient.StudyBasisId", "ed.qAbiturient", " SELECT Id, Name FROM ed.StudyBasis "));
            lst.Add(new FilterItem("Предоставлять общежитие на время обучения", FilterType.Bool, "ed.extPerson.HostelEduc", "ed.extPerson"));
            lst.Add(new FilterItem("Тип конкурса", FilterType.Multi, "ed.qAbiturient.CompetitionId", "ed.qAbiturient", "SELECT Id, Name FROM ed.Competition ORDER BY Name"));
            lst.Add(new FilterItem("Слушатель", FilterType.Bool, "ed.qAbiturient.IsListener", "ed.qAbiturient"));
            lst.Add(new FilterItem("Оплатил", FilterType.Bool, "ed.qAbiturient.IsPaid", "ed.qAbiturient"));
            lst.Add(new FilterItem("Заявление просмотрено", FilterType.Bool, "ed.qAbiturient.IsViewed", "ed.qAbiturient"));
            lst.Add(new FilterItem("Забрал документы", FilterType.Bool, "ed.qAbiturient.BackDoc", "ed.qAbiturient"));
            lst.Add(new FilterItem("Данные проверены", FilterType.Bool, "ed.qAbiturient.Checked", "ed.qAbiturient"));
            lst.Add(new FilterItem("Не допущен", FilterType.Bool, "ed.qAbiturient.NotEnabled", "ed.qAbiturient"));
            lst.Add(new FilterItem("Дата возврата документов", FilterType.DateFromTo, "ed.qAbiturient.BackDocDate", "ed.qAbiturient"));
            lst.Add(new FilterItem("Дата подачи документов", FilterType.DateFromTo, "ed.qAbiturient.DocDate", "ed.qAbiturient"));
            lst.Add(new FilterItem("Сумма баллов", FilterType.FromTo, "ed.extAbitMarksSum.TotalSum", "ed.extAbitMarksSum"));
            lst.Add(new FilterItem("Ин. язык испытания", FilterType.Multi, "ed.qAbiturient.LanguageId", "ed.qAbiturient", "SELECT Id, Name FROM ed.Language ORDER BY Name"));

            lst.Add(new FilterItem("Инд.достижения", FilterType.Multi, "ed.PersonAchievement.AchievementTypeId", "ed.PersonAchievement", "SELECT Id, Name FROM ed.AchievementType ORDER BY Name"));

            //lst.Add(new FilterItem("Средний балл сессии", FilterType.FromTo, "ed.qAbiturient.SessionAVG", "ed.qAbiturient"));
            //lst.Add(new FilterItem("Статус студента", FilterType.FromTo, "ed.qAbiturient.StudentStatus", "ed.qAbiturient"));
            lst.Add(new FilterItem("Договор об оплате", FilterType.Bool, " EXISTS (SELECT Top(1) ed.PaidData.Id FROM ed.PaidData WHERE ed.PaidData.AbiturientId = ed.qAbiturient.Id)", "ed.qAbiturient"));

            //экзамены 
            DataSet dsExams = _bdc.GetDataSet("SELECT DISTINCT ed.extExamInEntry.ExamId AS Id, ed.extExamInEntry.ExamName AS Name FROM ed.extExamInEntry INNER JOIN ed.qEntry ON qEntry.Id = extExamInEntry.EntryId WHERE qEntry.StudyLevelGroupId IN (" + Util.BuildStringWithCollection(MainClass.lstStudyLevelGroupId) + ")");

            foreach (DataRow dr in dsExams.Tables[0].Rows)
                lst.Add(new FilterItem("Экзамен " + dr["Name"].ToString(), FilterType.FromTo, string.Format("(select Min(ed.Mark.Value) FROM ed.Mark INNER JOIN ed.extExamInEntry ON ed.Mark.ExamInEntryBlockUnitId =ed.extExamInEntry.Id WHERE Mark.AbiturientId = ed.qAbiturient.Id AND ed.extExamInEntry.ExamId={0})", dr["Id"]), "ed.qAbiturient"));

            lst.Add(new FilterItem("Номер протокола о допуске", FilterType.FromTo, "ed.extEnableProtocol.Number", "ed.extEnableProtocol"));
            lst.Add(new FilterItem("Номер представления к зачислению", FilterType.FromTo, "extEntryView.Number", "extEntryView"));
            lst.Add(new FilterItem("Рейтинговый коэффициент", FilterType.FromTo, "ed.qAbiturient.Coefficient", "ed.qAbiturient"));
            lst.Add(new FilterItem("Подал подлинники для зачисления", FilterType.Bool, "ed.extPerson.HasOriginals", "ed.extPerson"));
            lst.Add(new FilterItem("Подал подлинники на заявление", FilterType.Bool, "ed.qAbiturient.HasOriginals", "ed.qAbiturient"));
            lst.Add(new FilterItem("Зачислен в СПбГУ (человек)", FilterType.Bool, " EXISTS (SELECT Top(1) extEntryView.Id FROM ed.extEntryView WHERE extEntryView.PersonId = extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Зачислен в СПбГУ (заявление)", FilterType.Bool, " EXISTS (SELECT Top(1) extEntryView.Id FROM ed.extEntryView WHERE extEntryView.AbiturientId = qAbiturient.Id)", "ed.qAbiturient"));
            lst.Add(new FilterItem("Отчислен из СПбГУ", FilterType.Bool, " EXISTS (SELECT Top(1) ed.extProtocol.Id FROM ed.extProtocol WHERE ProtocolTypeId = 4 AND IsOld = 0 AND Excluded = 1 AND ed.extProtocol.AbiturientId = ed.qAbiturient.Id)", "ed.qAbiturient"));
            lst.Add(new FilterItem("Есть в представлении к отчислению", FilterType.Bool, " EXISTS (SELECT Top(1) ed.extDisEntryView.Id FROM ed.extDisEntryView WHERE ed.extDisEntryView.AbiturientId = ed.qAbiturient.Id)", "ed.qAbiturient"));
            lst.Add(new FilterItem("Есть в протоколе о допуске", FilterType.Bool, " EXISTS (SELECT Top(1) ed.extEnableProtocol.Id FROM ed.extEnableProtocol WHERE ed.extEnableProtocol.AbiturientId = ed.qAbiturient.Id)", "ed.qAbiturient"));
            lst.Add(new FilterItem("Есть в представлении к зачислению", FilterType.Bool, " EXISTS (SELECT Top(1) ed.extEntryView.Id FROM ed.extEntryView WHERE ed.extEntryView.AbiturientId = ed.qAbiturient.Id)", "ed.qAbiturient"));

            lst.Add(new FilterItem("Направление", FilterType.Multi, "ed.qAbiturient.LicenseProgramId", "ed.qAbiturient", " SELECT DISTINCT ed.qEntry.LicenseProgramId AS Id, ed.qEntry.LicenseProgramCode + ' ' + ed.qEntry.LicenseProgramName AS Name FROM ed.qEntry WHERE qEntry.StudyLevelGroupId IN (" + Util.BuildStringWithCollection(MainClass.lstStudyLevelGroupId) + ")"));
            lst.Add(new FilterItem("Образовательная программа", FilterType.Multi, "ed.qAbiturient.ObrazProgramId", "ed.qAbiturient", " SELECT DISTINCT ed.qEntry.ObrazProgramId AS Id, '[' + ed.qEntry.ObrazProgramCrypt + '] ' + ed.qEntry.ObrazProgramName AS Name FROM ed.qEntry WHERE qEntry.StudyLevelGroupId IN (" + Util.BuildStringWithCollection(MainClass.lstStudyLevelGroupId) + ")"));

            //льготы
            lst.Add(new FilterItem("Военнослужащий", FilterType.Bool, "ed.extPerson.Privileges & 4 ", "ed.extPerson"));
            lst.Add(new FilterItem("Сирота", FilterType.Bool, "ed.extPerson.Privileges & 1 ", "ed.extPerson"));
            lst.Add(new FilterItem("Чернобылец", FilterType.Bool, "ed.extPerson.Privileges & 2 ", "ed.extPerson"));
            lst.Add(new FilterItem("Полусирота", FilterType.Bool, "ed.extPerson.Privileges & 16 ", "ed.extPerson"));
            lst.Add(new FilterItem("Инвалид", FilterType.Bool, "ed.extPerson.Privileges & 32 ", "ed.extPerson"));
            lst.Add(new FilterItem("Уч. боев.", FilterType.Bool, "ed.extPerson.Privileges & 64 ", "ed.extPerson"));
            lst.Add(new FilterItem("Стажник", FilterType.Bool, "ed.extPerson.Privileges & 128 ", "ed.extPerson"));
            lst.Add(new FilterItem("Реб.-сирота", FilterType.Bool, "ed.extPerson.Privileges & 256 ", "ed.extPerson"));
            lst.Add(new FilterItem("Огр. возможности", FilterType.Bool, "ed.extPerson.Privileges & 512 ", "ed.extPerson"));

            //Заявления
            lst.Add(new FilterItem("Крым", FilterType.Bool, "ed.qAbiturient.IsCrimea", "ed.qAbiturient"));

            lst.Add(new FilterItem("Только одно заявление на университет", FilterType.Bool, " (SELECT Count(Id) FROM ed.qAbiturient WHERE PersonId = qAbiturient.PersonId) = 1 ", "ed.qAbiturient"));
            lst.Add(new FilterItem("Только одно заявление на ваш факультет", FilterType.Bool, " (SELECT Count(Id) FROM ed.qAbiturient AS ab WHERE ab.PersonId = qAbiturient.PersonId AND ab.FacultyId = ed.qAbiturient.FacultyId) = 1 ", "ed.qAbiturient"));
            lst.Add(new FilterItem("Только дневное на ваш факультет", FilterType.Bool, " ( NOT EXISTS (SELECT * FROM ed.qAbiturient ab WHERE ab.PersonId = qAbiturient.PersonId AND ab.StudyFormId <> 1)) ", "ed.qAbiturient"));
            lst.Add(new FilterItem("Только вечернее на ваш факультет", FilterType.Bool, " ( NOT EXISTS (SELECT * FROM ed.qAbiturient ab WHERE ab.PersonId = qAbiturient.PersonId AND ab.StudyFormId <> 2)) ", "ed.qAbiturient"));
            lst.Add(new FilterItem("Только заочное на ваш факультет", FilterType.Bool, " ( NOT EXISTS (SELECT * FROM ed.qAbiturient ab WHERE ab.PersonId = qAbiturient.PersonId AND ab.StudyFormId <> 3)) ", "ed.qAbiturient"));
            lst.Add(new FilterItem("Только бюджет на ваш факультет", FilterType.Bool, " ( NOT EXISTS (SELECT * FROM ed.qAbiturient ab WHERE ab.PersonId = qAbiturient.PersonId AND ab.StudyBasisId <> 1)) ", "ed.qAbiturient"));
            lst.Add(new FilterItem("Только платно на ваш факультет", FilterType.Bool, " ( NOT EXISTS (SELECT * FROM ed.qAbiturient ab WHERE ab.PersonId = qAbiturient.PersonId AND ab.StudyBasisId <> 2)) ", "ed.qAbiturient"));

            return lst;
        }
        protected override abstract List<FilterItem> GetOtherFilterList(DBPriem _bdc);

        public override List<ListItem> GetGroups(DBPriem _bdc)
        {
            List<ListItem> li = GetBaseGroups(_bdc);
            li.AddRange(GetOtherGroups(_bdc));

            return li;
        }
        protected override List<ListItem> GetBaseGroups(DBPriem _bdc)
        {
            List<ListItem> list = new List<ListItem>();

            list.Add(new ListItem("ФИО", "Фамилия Имя Отчество"));
            list.Add(new ListItem("Направление", "Направление"));
            list.Add(new ListItem("Образ_программа", "Образовательная программа"));
            list.Add(new ListItem("Тип_паспорта", "Тип документа"));
            list.Add(new ListItem("Забрал_док", "Забрал документы"));
            list.Add(new ListItem("Пол_мужской", "Пол"));
            list.Add(new ListItem("Медалист", "Медалист"));
            list.Add(new ListItem("Год_выпуска", "Год выпуска"));
            list.Add(new ListItem("Ин_язык", "Иностранный язык"));
            list.Add(new ListItem("Профиль", "Профиль"));
            list.Add(new ListItem("Форма_обучения", "Форма обучения"));
            list.Add(new ListItem("Основа_обучения", "Основа обучения"));
            list.Add(new ListItem("Тип_конкурса", "Тип конкурса"));
            list.Add(new ListItem("второе_высшее", "Второе высшее"));
            list.Add(new ListItem("Страна", "Страна"));
            list.Add(new ListItem("Гражданство", "Гражданство"));
            list.Add(new ListItem("Оплатил", "Оплатил"));
            list.Add(new ListItem("Слушатель", "Слушатель"));
            list.Add(new ListItem("Предоставлять_общежитие_поступление", "Предоставлять общежитие на время поступления"));
            list.Add(new ListItem("Регион", "Регион"));
            list.Add(new ListItem("Тип_уч_заведения", "Тип учебного заведения"));

            return list;
        }
        protected override abstract List<ListItem> GetOtherGroups(DBPriem _bdc);
    }

    public class FilterProvider_1K : AbstractFilterProvider<Common_Classes.PriemType1K>
    {
        protected override SortedList<string, string> GetOtherColumns(DBPriem _bdc, string _fac)
        {
            SortedList<string, string> list = new SortedList<string, string>();

            list.Add("Программы_для_ВО", "Программа для лиц с ВО");
            list.Add("Программы_сокр", "Сокращенная программа");
            list.Add("Программы_парал", "Параллельная программа");

            //индивидуальные достижения
            list.Add("ИНД_Аттестат", "ИД: Аттестат");
            list.Add("ИНД_ГТО", "ИД: ГТО");
            list.Add("ИНД_СПО", "ИД: СПО с отличием");
            list.Add("ИНД_ПобРег", "ИД: ПобРег");
            list.Add("ИНД_ПризРег", "ИД: ПризРег");
            list.Add("ИНД_ОлСПбГУ", "ИД: ОлСПбГУ");
            list.Add("ИНД_ПрочРСОШ", "ИД: ПрочРСОШ");
            list.Add("ИНД_СУММ", "ИД: Сумма баллов");

            list.Add("Город_уч_заведения", "Город уч.заведения");
            list.Add("Тип_уч_заведения", "Тип уч.заведения");
            list.Add("Медалист", "Медалист");
            list.Add("Номер_школы", "Номер школы");
            list.Add("Серия_атт", "Серия аттестата");
            list.Add("Номер_атт", "Номер аттестата");

            list.Add("Серия_диплома", "Серия диплома");
            list.Add("Номер_диплома", "Номер диплома");

            list.Add("Год_выпуска", "Год выпуска");

            list.Add("Средний_балл_атт", "Средний балл аттестата");
            list.Add("Статус_ФБС", "Статус ФБС");
            list.Add("Поданы_подлинники_ЕГЭ", "Поданы подлинники ЕГЭ");

            list.Add("Профиль", "Профиль");

            list.Add("Свидетельство_ЕГЭ_2012", "Свидетельство ЕГЭ 2012 года");
            list.Add("Свидетельство_ЕГЭ_2013", "Свидетельство ЕГЭ 2013 года");
            list.Add("Загружено_Свид-во_ЕГЭ_2012", "Загружено свид-во ЕГЭ 2012 года");
            list.Add("Загружено_Свид-во_ЕГЭ_2013", "Загружено свид-во ЕГЭ 2013 года");
            list.Add("Загружено_Свид-во_ЕГЭ_2014", "Загружено свид-во ЕГЭ 2014 года");
            list.Add("Загружено_Свид-во_ЕГЭ_2015", "Загружено свид-во ЕГЭ 2015 года");
            list.Add("Загружено_Свид-во_ЕГЭ_2016", "Загружено свид-во ЕГЭ 2016 года");
            list.Add("Загружено_Свид-во_ЕГЭ_2017", "Загружено свид-во ЕГЭ 2017 года");

            list.Add("ЕГЭ_англ.яз", "ЕГЭ англ.яз");
            list.Add("ЕГЭ_русск.язык", "ЕГЭ русск.язык");
            list.Add("ЕГЭ_математика", "ЕГЭ математика");
            list.Add("ЕГЭ_физика", "ЕГЭ физика");
            list.Add("ЕГЭ_химия", "ЕГЭ химия");
            list.Add("ЕГЭ_биология", "ЕГЭ биология");
            list.Add("ЕГЭ_история", "ЕГЭ история");
            list.Add("ЕГЭ_география", "ЕГЭ география");
            list.Add("ЕГЭ_немец.язык", "ЕГЭ немец.язык");
            list.Add("ЕГЭ_франц.язык", "ЕГЭ франц.язык");
            list.Add("ЕГЭ_обществознание", "ЕГЭ обществознание");
            list.Add("ЕГЭ_литература", "ЕГЭ литература");
            list.Add("ЕГЭ_испан.язык", "ЕГЭ испан.язык");
            list.Add("ЕГЭ_информатика", "ЕГЭ информатика");
            list.Add("ЕГЭ_Сочинение", "ЕГЭ (Сочинение)");

            list.Add("Аттестат_алгебра", "Aттестат Алгебра");
            list.Add("Аттестат_англ_язык", "Aттестат Англ. язык");
            list.Add("Аттестат_астрономия", "Aттестат Астрономия");
            list.Add("Аттестат_биология", "Aттестат Биология");
            list.Add("Аттестат_вселенная_чел", "Aттестат Вселенная человека");
            list.Add("Аттестат_вс_история", "Aттестат Всеобщая история");
            list.Add("Аттестат_география", "Aттестат География");
            list.Add("Аттестат_геометрия", "Aттестат Геометрия");
            list.Add("Аттестат_информатика", "Aттестат Информатика");
            list.Add("Аттестат_история_Спб", "Aттестат История и культура Санкт-Петербурга");
            list.Add("Аттестат_ист_России", "Aттестат История России");
            list.Add("Аттестат_литература", "Aттестат Литература");
            list.Add("Аттестат_мировая_худ_культура", "Aттестат Мировая художественная культура");
            list.Add("Аттестат_обществознание", "Aттестат Обществознание");
            list.Add("Аттестат_ОБЖ", "Aттестат ОБЖ");
            list.Add("Аттестат_русск_язык", "Aттестат Русский язык");
            list.Add("Аттестат_технология", "Aттестат Технология");
            list.Add("Аттестат_физика", "Aттестат Физика");
            list.Add("Аттестат_физ_культура", "Aттестат Физическая культура");
            list.Add("Аттестат_химия", "Aттестат Химия");
            list.Add("Аттестат_экология", "Aттестат Экология");
            list.Add("Аттестат_немецкий_язык", "Aттестат Немецкий язык");
            list.Add("Аттестат_испанский_язык", "Aттестат Испанский язык");
            list.Add("Аттестат_французский_язык", "Aттестат Французский язык");
            list.Add("Аттестат_итальянский_язык", "Aттестат Итальянский язык");

            // олимпиады
            //list.Add("Всероссийкая", "Всероссийкая олимпиада");
            //list.Add("Международная", "Международная олимпиада");
            //list.Add("Региональная", "Региональная олимпиада");
            //list.Add("Межвузовская", "Межвузовская олимпиада");
            //list.Add("СПбГУ", "СПбГУ олимпиада");
            //list.Add("Школьников", "Олимпиада школьников");  

            list.Add("Степень_диплома", "Степень диплома");
            list.Add("Уровень_поступления", "Уровень_поступления");

            return list;
        }
        protected override List<FilterItem> GetOtherFilterList(DBPriem _bdc)
        {
            List<FilterItem> lst = new List<FilterItem>();

            lst.Add(new FilterItem("Программы для лиц с ВО", FilterType.Bool, "ed.qAbiturient.IsSecond", "ed.qAbiturient"));
            lst.Add(new FilterItem("Сокращенные программы", FilterType.Bool, "ed.qAbiturient.IsReduced", "ed.qAbiturient"));
            lst.Add(new FilterItem("параллельные программы", FilterType.Bool, "ed.qAbiturient.IsParallel", "ed.qAbiturient"));

            lst.Add(new FilterItem("Медалист", FilterType.Bool, "ed.extPerson_EducationInfo_Current.IsExcellent", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Подан подлинник аттестата", FilterType.Bool, "ed.qAbiturient.HasOriginals", "ed.qAbiturient"));

            lst.Add(new FilterItem("Номер аттестата", FilterType.FromTo, "ed.extPerson_EducationInfo_Current.AttestatNum", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Серия аттестата", FilterType.FromTo, "ed.extPerson_EducationInfo_Current.AttestatSeries", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Средний балл аттестата", FilterType.FromTo, "ed.extPerson_EducationInfo_Current.SchoolAVG", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Введен средний балл аттестата", FilterType.Bool, "(NOT ed.extPerson_EducationInfo_Current.SchoolAVG IS NULL AND Len(ed.extPerson_EducationInfo_Current.SchoolAVG) > 0)", "ed.extPerson_EducationInfo_Current"));

            lst.Add(new FilterItem("Город учебного заведения", FilterType.Text, "ed.extPerson_EducationInfo_Current.SchoolCity", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Тип учебного заведения", FilterType.Multi, "ed.extPerson_EducationInfo_Current.SchoolTypeId", "ed.extPerson_EducationInfo_Current", "SELECT Id, Name FROM SchoolType ORDER BY Name"));
            lst.Add(new FilterItem("Название учебного заведения", FilterType.Text, "ed.extPerson_EducationInfo_Current.SchoolName", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Номер учебного заведения", FilterType.FromTo, "ed.extPerson_EducationInfo_Current.SchoolNum", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Год окончания учебного заведения", FilterType.FromTo, "ed.extPerson_EducationInfo_Current.SchoolExitYear", "ed.extPerson_EducationInfo_Current"));

            lst.Add(new FilterItem("Уровень", FilterType.Multi, "ed.qAbiturient.StudyLevelId", "ed.qAbiturient", " SELECT DISTINCT Id, Name AS Name FROM ed.StudyLevel "));
            lst.Add(new FilterItem("Профиль", FilterType.Multi, "ed.qAbiturient.ProfileId", "ed.qAbiturient", " SELECT DISTINCT ed.qProfile.Id, ed.qProfile.Name AS Name FROM ed.qProfile "));

            lst.Add(new FilterItem("Статус ФБС", FilterType.Multi, "(SELECT FBSStatusId FROM ed.extFBSStatus WHERE ed.extFBSStatus.PersonId = ed.extPerson.Id)", "ed.extPerson", "SELECT Id, Name FROM ed.FBSStatus WHERE Id <> 3"));

            //олимпиады
            lst.Add(new FilterItem("Международная олимпиада", FilterType.Bool, " EXISTS (SELECT * FROM ed.Olympiads WHERE Olympiads.PersonId = qAbiturient.PersonId AND Olympiads.OlympTypeId=1 ) ", "ed.qAbiturient"));
            lst.Add(new FilterItem("Всероссийская олимпиада", FilterType.Bool, " EXISTS (SELECT * FROM ed.Olympiads WHERE Olympiads.PersonId = qAbiturient.PersonId AND Olympiads.OlympTypeId=2 ) ", "ed.qAbiturient"));
            lst.Add(new FilterItem("Олимпиада СПбГУ", FilterType.Bool, " EXISTS (SELECT * FROM ed.Olympiads WHERE Olympiads.AbiturientId = qAbiturient.PersonId AND Olympiads.OlympTypeId=3 ) ", "ed.qAbiturient"));
            lst.Add(new FilterItem("Другие олимпиады школьников", FilterType.Bool, " EXISTS (SELECT * FROM ed.Olympiads WHERE Olympiads.PersonId = qAbiturient.PersonId AND Olympiads.OlympTypeId=4 ) ", "ed.qAbiturient"));

            lst.Add(new FilterItem("Степень диплома олимпиады", FilterType.Multi, "(SELECT MAX(OlympValueId) FROM ed.Olympiads WHERE Olympiads.PersonId = qAbiturient.PersonId)", "ed.qAbiturient", " SELECT Id, Name FROM ed.OlympValue "));

            //ЕГЭ
            lst.Add(new FilterItem("Номер свидетельства ЕГЭ 2011 года", FilterType.FromTo, " (SELECT Top(1) ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2011)", "ed.extPerson"));
            lst.Add(new FilterItem("Номер свидетельства ЕГЭ 2012 года", FilterType.FromTo, " (SELECT Top(1) ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2012)", "ed.extPerson"));
            lst.Add(new FilterItem("Номер свидетельства ЕГЭ 2013 года", FilterType.FromTo, " (SELECT Top(1) ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2012)", "ed.extPerson"));
            lst.Add(new FilterItem("Загружено cвид-во ЕГЭ 2012 года", FilterType.Bool, "EXISTS (SELECT Top(1) ed.EgeCertificate.IsImported FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND Year=2012 AND IsImported > 0)", "ed.extPerson"));
            lst.Add(new FilterItem("Загружено cвид-во ЕГЭ 2013 года", FilterType.Bool, "EXISTS (SELECT Top(1) ed.EgeCertificate.IsImported FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND Year=2013 AND IsImported > 0)", "ed.extPerson"));
            lst.Add(new FilterItem("Загружено cвид-во ЕГЭ 2014 года", FilterType.Bool, "EXISTS (SELECT Top(1) ed.EgeCertificate.IsImported FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND Year=2014 AND IsImported > 0)", "ed.extPerson"));
            lst.Add(new FilterItem("Загружено cвид-во ЕГЭ 2015 года", FilterType.Bool, "EXISTS (SELECT Top(1) ed.EgeCertificate.IsImported FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND Year=2015 AND IsImported > 0)", "ed.extPerson"));
            lst.Add(new FilterItem("Загружено cвид-во ЕГЭ 2016 года", FilterType.Bool, "EXISTS (SELECT Top(1) ed.EgeCertificate.IsImported FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND Year=2016 AND IsImported > 0)", "ed.extPerson"));
            lst.Add(new FilterItem("Загружено cвид-во ЕГЭ 2017 года", FilterType.Bool, "EXISTS (SELECT Top(1) ed.EgeCertificate.IsImported FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND Year=2017 AND IsImported > 0)", "ed.extPerson"));
            lst.Add(new FilterItem("Сдавал ЕГЭ", FilterType.Bool, " EXISTS (SELECT ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));

            lst.Add(new FilterItem("Есть свидетельство ЕГЭ 2012 года", FilterType.Bool, " EXISTS (SELECT ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2012)", "ed.extPerson"));
            lst.Add(new FilterItem("Есть свидетельство ЕГЭ 2013 года", FilterType.Bool, " EXISTS (SELECT ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2013)", "ed.extPerson"));
            lst.Add(new FilterItem("Есть свидетельство ЕГЭ 2014 года", FilterType.Bool, " EXISTS (SELECT ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2014)", "ed.extPerson"));
            lst.Add(new FilterItem("Есть свидетельство ЕГЭ 2015 года", FilterType.Bool, " EXISTS (SELECT ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2015)", "ed.extPerson"));
            lst.Add(new FilterItem("Есть свидетельство ЕГЭ 2016 года", FilterType.Bool, " EXISTS (SELECT ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2016)", "ed.extPerson"));
            lst.Add(new FilterItem("Есть свидетельство ЕГЭ 2017 года", FilterType.Bool, " EXISTS (SELECT ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2017)", "ed.extPerson"));

            //инд достижения
            lst.Add(new FilterItem("Инд.достижения: Аттестат с отличием", FilterType.Bool, " EXISTS (SELECT * FROM ed.PersonAchievement PA WHERE PA.PersonId = extPerson.Id AND PA.AchievementTypeId = 9)", "ed.extPerson"));
            lst.Add(new FilterItem("Инд.достижения: ГТО", FilterType.Bool, " EXISTS (SELECT * FROM ed.PersonAchievement PA WHERE PA.PersonId = extPerson.Id AND PA.AchievementTypeId = 8)", "ed.extPerson"));
            lst.Add(new FilterItem("Инд.достижения: Победитель рег. этапа Всеросс", FilterType.Bool, " EXISTS (SELECT * FROM ed.Olympiads OL WHERE OL.PersonId = qAbiturient.PersonId AND OL.OlympTypeId = 7 AND OL.OlympValueId = 6)", "ed.qAbiturient"));
            lst.Add(new FilterItem("Инд.достижения: Призёр рег. этапа Всеросс", FilterType.Bool, " EXISTS (SELECT * FROM ed.Olympiads OL WHERE OL.PersonId = qAbiturient.PersonId AND OL.OlympTypeId = 7 AND OL.OlympValueId IN (5, 7))", "ed.qAbiturient"));
            lst.Add(new FilterItem("Инд.достижения: Прочие конкурсы", FilterType.Bool, " EXISTS (SELECT * FROM ed.PersonAchievement PA WHERE PA.PersonId = extPerson.Id AND PA.AchievementTypeId = 11)", "ed.extPerson"));
            lst.Add(new FilterItem("Инд.достижения: Диплом СПО с отличием", FilterType.Bool, " EXISTS (SELECT * FROM ed.PersonAchievement PA WHERE PA.PersonId = extPerson.Id AND PA.AchievementTypeId = 16)", "ed.extPerson"));
            lst.Add(new FilterItem("Инд.достижения: Сумма баллов", FilterType.FromTo, " (SELECT Top(1) [AdditionalMarksSum] FROM ed.extAbitAdditionalMarksSum WHERE extAbitAdditionalMarksSum.AbiturientId = qAbiturient.Id)", "ed.qAbiturient"));

            lst.Add(new FilterItem("Апелляция", FilterType.Bool, " EXISTS (SELECT * FROM ed.EgeMark LEFT JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id WHERE ed.EgeMark.IsAppeal>0 AND ed.EgeCertificate.PersonId = ed.extPerson.Id) ", "ed.extPerson"));
            lst.Add(new FilterItem("Из олимпиад", FilterType.Bool, " EXISTS (SELECT * FROM ed.EgeMark LEFT JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id WHERE ed.EgeMark.IsFromOlymp>0 AND ed.EgeCertificate.PersonId = ed.extPerson.Id) ", "ed.extPerson"));
            lst.Add(new FilterItem("ЕГЭ Английский язык", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Английский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("ЕГЭ Русский язык", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Русский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("ЕГЭ Математика", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Математика' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("ЕГЭ Физика", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Физика' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("ЕГЭ Химия", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Химия' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("ЕГЭ Биология", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Биология' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("ЕГЭ История", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='История' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("ЕГЭ География", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='География' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("ЕГЭ Немецкий язык", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Немецкий язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("ЕГЭ Французский язык", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Французский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("ЕГЭ Обществознание", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Обществознание' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("ЕГЭ Литература", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Литература' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("ЕГЭ Испанский язык", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Испанский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("ЕГЭ Информатика и ИКТ", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Информатика и ИКТ' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));

            lst.Add(new FilterItem("Сдавал ЕГЭ Английский язык", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Английский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Сдавал ЕГЭ Русский язык", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Русский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Сдавал ЕГЭ Математика", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Математика' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Сдавал ЕГЭ Физика", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Физика' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Сдавал ЕГЭ Химия", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Химия' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Сдавал ЕГЭ Биология", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Биология' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Сдавал ЕГЭ История", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='История' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Сдавал ЕГЭ География", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='География' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Сдавал ЕГЭ Немецкий язык", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Немецкий язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Сдавал ЕГЭ Французский язык", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Французский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Сдавал ЕГЭ Обществознание", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Обществознание' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Сдавал ЕГЭ Литература", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Литература' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Сдавал ЕГЭ Испанский язык", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Испанский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Сдавал ЕГЭ Информатика и ИКТ", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Информатика и ИКТ' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Сдавал Сочинение", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id) INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.Id = 15 AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));

            lst.Add(new FilterItem("Сдавал ЕГЭ в СПбГУ", FilterType.Bool, " EXISTS (SELECT * FROM ed.qMark INNER JOIN ed.extExamInEntry ON qMark.ExamInEntryBlockUnitId = ed.extExamInEntry.Id WHERE ed.qMark.IsFromEge = 0 AND ed.qMark.IsFromOlymp = 0 AND ed.extExamInEntry.IsAdditional = 0 AND ed.qMark.AbiturientId = ed.qAbiturient.Id)", "ed.qAbiturient"));

            lst.Add(new FilterItem("Квалификация (пред. обр.)", FilterType.Multi, "ed.extPerson_EducationInfo_Current.HEQualification", "ed.extPerson_EducationInfo_Current", " SELECT DISTINCT HEQualification AS Id, HEQualification AS Name FROM ed.extPerson_EducationInfo_Current "));
            return lst;
        }
        protected override List<ListItem> GetOtherGroups(DBPriem _bdc)
        {
            return new List<ListItem>();
        }
    }

    public class FilterProvider_Mag : AbstractFilterProvider<Common_Classes.PriemTypeMag>
    {
        protected override SortedList<string, string> GetOtherColumns(DBPriem _bdc, string _fac)
        {
            SortedList<string, string> list = new SortedList<string, string>();

            list.Add("Медалист", "Красный диплом");
            list.Add("Профиль", "Профиль");
            list.Add("Направление_подготовки", "Базовое_направление(специальность)");
            list.Add("Год_выпуска", "Год выпуска");
            list.Add("Средний_балл_атт", "Средний балл");

            list.Add("Номер_диплома", "Номер диплома");
            list.Add("Серия_диплома", "Серия диплома");

            list.Add("Квалификация", "Квалификация");
            list.Add("Место_предыдущего_образования_маг", "Место предыдущего образования (для маг)");

            return list;
        }
        protected override List<FilterItem> GetOtherFilterList(DBPriem _bdc)
        {
            List<FilterItem> lst = new List<FilterItem>();
            lst.Add(new FilterItem("Диплом с отличием", FilterType.Bool, "ed.extPerson_EducationInfo_Current.IsExcellent", "ed.extPerson_EducationInfo_Current"));

            lst.Add(new FilterItem("Средний балл диплома", FilterType.FromTo, "ed.extPerson_EducationInfo_Current.SchoolAVG", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Введен средний балл диплома", FilterType.Bool, "(NOT ed.extPerson_EducationInfo_Current.SchoolAVG IS NULL AND Len(ed.extPerson_EducationInfo_Current.SchoolAVG) > 0)", "ed.extPerson_EducationInfo_Current"));

            lst.Add(new FilterItem("Город учебного заведения", FilterType.Text, "ed.extPerson_EducationInfo_Current.SchoolCity", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Название учебного заведения", FilterType.Text, "ed.extPerson_EducationInfo_Current.SchoolName", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Год окончания учебного заведения", FilterType.FromTo, "ed.extPerson_EducationInfo_Current.SchoolExitYear", "ed.extPerson_EducationInfo_Current"));

            lst.Add(new FilterItem("Квалификация (пред. обр.)", FilterType.Multi, "ed.extPerson_EducationInfo_Current.HEQualification", "ed.extPerson_EducationInfo_Current", " SELECT DISTINCT HEQualification AS Id, HEQualification AS Name FROM ed.extPerson_EducationInfo_Current "));
            lst.Add(new FilterItem("Уровень", FilterType.Multi, "ed.qAbiturient.StudyLevelId", "ed.qAbiturient", " SELECT DISTINCT Id, Name AS Name FROM ed.StudyLevel "));
            lst.Add(new FilterItem("Профиль", FilterType.Multi, "ed.qAbiturient.ProfileId", "ed.qAbiturient", " SELECT DISTINCT ed.qProfile.Id, ed.qProfile.Name AS Name FROM ed.qProfile "));

            //олимпиады
            lst.Add(new FilterItem("Студенческая олимпиада", FilterType.Bool, " EXISTS (SELECT * FROM ed.Olympiads WHERE Olympiads.PersonId = qAbiturient.PersonId AND Olympiads.OlympTypeId=5 ) ", "ed.qAbiturient"));
            lst.Add(new FilterItem("Прочие инт. достижения", FilterType.Bool, " EXISTS (SELECT * FROM ed.Olympiads WHERE Olympiads.PersonId = qAbiturient.PersonId AND Olympiads.OlympTypeId=6 ) ", "ed.qAbiturient"));
            lst.Add(new FilterItem("Степень диплома", FilterType.Multi, "(SELECT MAX(OlympValueId) FROM ed.Olympiads WHERE Olympiads.PersonId = qAbiturient.PersonId)", "ed.qAbiturient", " SELECT Id, Name FROM ed.OlympValue "));

            return lst;
        }
        protected override List<ListItem> GetOtherGroups(DBPriem _bdc)
        {
            return new List<ListItem>();
        }
    }
    
    public class FilterProvider_SPO : AbstractFilterProvider<Common_Classes.PriemTypeSPO>
    {
        protected override SortedList<string, string> GetOtherColumns(DBPriem _bdc, string _fac)
        {
            SortedList<string, string> list = new SortedList<string, string>();

            list.Add("Программы_для_ВО", "Программа для лиц с ВО");
            list.Add("Программы_сокр", "Сокращенная программа");
            list.Add("Программы_парал", "Параллельная программа");

            list.Add("Город_уч_заведения", "Город уч.заведения");
            list.Add("Тип_уч_заведения", "Тип уч.заведения");
            list.Add("Медалист", "Медалист");
            list.Add("Номер_школы", "Номер школы");
            list.Add("Серия_атт", "Серия аттестата");
            list.Add("Номер_атт", "Номер аттестата");

            list.Add("Серия_диплома", "Серия диплома");
            list.Add("Номер_диплома", "Номер диплома");

            list.Add("Год_выпуска", "Год выпуска");

            list.Add("Средний_балл_атт", "Средний балл аттестата");
            list.Add("Статус_ФБС", "Статус ФБС");
            list.Add("Поданы_подлинники_ЕГЭ", "Поданы подлинники ЕГЭ");

            list.Add("Профиль", "Профиль");

            list.Add("Свидетельство_ЕГЭ_2011", "Свидетельство ЕГЭ 2011 года");
            list.Add("Свидетельство_ЕГЭ_2012", "Свидетельство ЕГЭ 2012 года");
            list.Add("Загружено_Свид-во_ЕГЭ_2012", "Загружено свид-во ЕГЭ 2012 года");

            list.Add("ЕГЭ_англ.яз", "ЕГЭ англ.яз");
            list.Add("ЕГЭ_русск.язык", "ЕГЭ русск.язык");
            list.Add("ЕГЭ_математика", "ЕГЭ математика");
            list.Add("ЕГЭ_физика", "ЕГЭ физика");
            list.Add("ЕГЭ_химия", "ЕГЭ химия");
            list.Add("ЕГЭ_биология", "ЕГЭ биология");
            list.Add("ЕГЭ_история", "ЕГЭ история");
            list.Add("ЕГЭ_география", "ЕГЭ география");
            list.Add("ЕГЭ_немец.язык", "ЕГЭ немец.язык");
            list.Add("ЕГЭ_франц.язык", "ЕГЭ франц.язык");
            list.Add("ЕГЭ_обществознание", "ЕГЭ обществознание");
            list.Add("ЕГЭ_литература", "ЕГЭ литература");
            list.Add("ЕГЭ_испан.язык", "ЕГЭ испан.язык");
            list.Add("ЕГЭ_информатика", "ЕГЭ информатика");

            list.Add("Аттестат_алгебра", "Aттестат Алгебра");
            list.Add("Аттестат_англ_язык", "Aттестат Англ. язык");
            list.Add("Аттестат_астрономия", "Aттестат Астрономия");
            list.Add("Аттестат_биология", "Aттестат Биология");
            list.Add("Аттестат_вселенная_чел", "Aттестат Вселенная человека");
            list.Add("Аттестат_вс_история", "Aттестат Всеобщая история");
            list.Add("Аттестат_география", "Aттестат География");
            list.Add("Аттестат_геометрия", "Aттестат Геометрия");
            list.Add("Аттестат_информатика", "Aттестат Информатика");
            list.Add("Аттестат_история_Спб", "Aттестат История и культура Санкт-Петербурга");
            list.Add("Аттестат_ист_России", "Aттестат История России");
            list.Add("Аттестат_литература", "Aттестат Литература");
            list.Add("Аттестат_мировая_худ_культура", "Aттестат Мировая художественная культура");
            list.Add("Аттестат_обществознание", "Aттестат Обществознание");
            list.Add("Аттестат_ОБЖ", "Aттестат ОБЖ");
            list.Add("Аттестат_русск_язык", "Aттестат Русский язык");
            list.Add("Аттестат_технология", "Aттестат Технология");
            list.Add("Аттестат_физика", "Aттестат Физика");
            list.Add("Аттестат_физ_культура", "Aттестат Физическая культура");
            list.Add("Аттестат_химия", "Aттестат Химия");
            list.Add("Аттестат_экология", "Aттестат Экология");
            list.Add("Аттестат_немецкий_язык", "Aттестат Немецкий язык");
            list.Add("Аттестат_испанский_язык", "Aттестат Испанский язык");
            list.Add("Аттестат_французский_язык", "Aттестат Французский язык");
            list.Add("Аттестат_итальянский_язык", "Aттестат Итальянский язык");

            // олимпиады
            //list.Add("Всероссийкая", "Всероссийкая олимпиада");
            //list.Add("Международная", "Международная олимпиада");
            //list.Add("Региональная", "Региональная олимпиада");
            //list.Add("Межвузовская", "Межвузовская олимпиада");
            //list.Add("СПбГУ", "СПбГУ олимпиада");
            //list.Add("Школьников", "Олимпиада школьников");  

            list.Add("Степень_диплома", "Степень диплома");

            return list;
        }
        protected override List<FilterItem> GetOtherFilterList(DBPriem _bdc)
        {
            List<FilterItem> lst = new List<FilterItem>();

            return lst;
        }
        protected override List<ListItem> GetOtherGroups(DBPriem _bdc)
        {
            return new List<ListItem>();
        }
    }

    public class FilterProvider_Asp : AbstractFilterProvider<Common_Classes.PriemTypeAspirant>
    {
        protected override SortedList<string, string> GetOtherColumns(DBPriem _bdc, string _fac)
        {
            SortedList<string, string> list = new SortedList<string, string>();

            list.Add("Медалист", "Красный диплом");
            list.Add("Профиль", "Профиль");
            list.Add("Направление_подготовки", "Базовое_направление(специальность)");
            list.Add("Год_выпуска", "Год выпуска");

            list.Add("Номер_диплома", "Номер диплома");
            list.Add("Серия_диплома", "Серия диплома");

            list.Add("Квалификация", "Квалификация");
            list.Add("Место_предыдущего_образования_маг", "Место предыдущего образования (для маг)");

            return list;
        }
        protected override List<FilterItem> GetOtherFilterList(DBPriem _bdc)
        {
            List<FilterItem> lst = new List<FilterItem>();

            lst.Add(new FilterItem("Номер диплома", FilterType.FromTo, "ed.extPerson_EducationInfo_Current.DiplomNum", "ed.extPerson_EducationInfo_Current"));

            lst.Add(new FilterItem("Название учебного заведения", FilterType.Text, "ed.extPerson_EducationInfo_Current.HightEducation", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Год окончания учебного заведения", FilterType.FromTo, "ed.extPerson_EducationInfo_Current.HEExitYear", "ed.extPerson_EducationInfo_Current"));

            lst.Add(new FilterItem("Профиль", FilterType.Multi, "ed.qAbiturient.ProfileId", "ed.qAbiturient", " SELECT DISTINCT ed.qProfile.Id, ed.qProfile.Name AS Name FROM ed.qProfile "));

            lst.Add(new FilterItem("Красный диплом", FilterType.Bool, "ed.extPerson_EducationInfo_Current.IsExcellent", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Подан подлинник диплома", FilterType.Bool, "ed.qAbiturient.HasOriginals", "ed.qAbiturient"));

            lst.Add(new FilterItem("Квалификация (пред. обр.)", FilterType.Multi, "ed.extPerson_EducationInfo_Current.HEQualification", "ed.extPerson_EducationInfo_Current", " SELECT DISTINCT HEQualification AS Id, HEQualification AS Name FROM ed.extPerson_EducationInfo_Current "));

            return lst;
        }
        protected override List<ListItem> GetOtherGroups(DBPriem _bdc)
        {
            return new List<ListItem>();
        }
    }

    public class FilterProvider_AG : AbstractFilterProvider<Common_Classes.PriemType1K>
    {
        protected override SortedList<string, string> GetOtherColumns(DBPriem _bdc, string _fac)
        {
            SortedList<string, string> list = new SortedList<string, string>();

            //list.Add("Программы_для_ВО", "Программа для лиц с ВО");
            //list.Add("Программы_сокр", "Сокращенная программа");
            //list.Add("Программы_парал", "Параллельная программа");

            //индивидуальные достижения
            list.Add("ИНД_Аттестат", "ИНД_Аттестат");
            list.Add("ИНД_Волонтёр", "ИНД_Волонтёр");
            list.Add("ИНД_ПобРег", "ИНД_ПобРег");
            list.Add("ИНД_ПризРег", "ИНД_ПризРег");

            list.Add("Город_уч_заведения", "Город уч.заведения");
            list.Add("Тип_уч_заведения", "Тип уч.заведения");
            list.Add("Медалист", "Медалист");
            list.Add("Номер_школы", "Номер школы");
            //list.Add("Серия_атт", "Серия аттестата");
            //list.Add("Номер_атт", "Номер аттестата");

            //list.Add("Серия_диплома", "Серия диплома");
            //list.Add("Номер_диплома", "Номер диплома");

            //list.Add("Год_выпуска", "Год выпуска");

            //list.Add("Средний_балл_атт", "Средний балл аттестата");
            //list.Add("Статус_ФБС", "Статус ФБС");
            //list.Add("Поданы_подлинники_ЕГЭ", "Поданы подлинники ЕГЭ");

            //list.Add("Профиль", "Профиль");

            //list.Add("Свидетельство_ЕГЭ_2012", "Свидетельство ЕГЭ 2012 года");
            //list.Add("Свидетельство_ЕГЭ_2013", "Свидетельство ЕГЭ 2013 года");
            //list.Add("Загружено_Свид-во_ЕГЭ_2012", "Загружено свид-во ЕГЭ 2012 года");
            //list.Add("Загружено_Свид-во_ЕГЭ_2013", "Загружено свид-во ЕГЭ 2013 года");
            //list.Add("Загружено_Свид-во_ЕГЭ_2014", "Загружено свид-во ЕГЭ 2014 года");
            //list.Add("Загружено_Свид-во_ЕГЭ_2015", "Загружено свид-во ЕГЭ 2015 года");

            //list.Add("ЕГЭ_англ.яз", "ЕГЭ англ.яз");
            //list.Add("ЕГЭ_русск.язык", "ЕГЭ русск.язык");
            //list.Add("ЕГЭ_математика", "ЕГЭ математика");
            //list.Add("ЕГЭ_физика", "ЕГЭ физика");
            //list.Add("ЕГЭ_химия", "ЕГЭ химия");
            //list.Add("ЕГЭ_биология", "ЕГЭ биология");
            //list.Add("ЕГЭ_история", "ЕГЭ история");
            //list.Add("ЕГЭ_география", "ЕГЭ география");
            //list.Add("ЕГЭ_немец.язык", "ЕГЭ немец.язык");
            //list.Add("ЕГЭ_франц.язык", "ЕГЭ франц.язык");
            //list.Add("ЕГЭ_обществознание", "ЕГЭ обществознание");
            //list.Add("ЕГЭ_литература", "ЕГЭ литература");
            //list.Add("ЕГЭ_испан.язык", "ЕГЭ испан.язык");
            //list.Add("ЕГЭ_информатика", "ЕГЭ информатика");
            //list.Add("ЕГЭ_Сочинение", "ЕГЭ (Сочинение)");

            //list.Add("Аттестат_алгебра", "Aттестат Алгебра");
            //list.Add("Аттестат_англ_язык", "Aттестат Англ. язык");
            //list.Add("Аттестат_астрономия", "Aттестат Астрономия");
            //list.Add("Аттестат_биология", "Aттестат Биология");
            //list.Add("Аттестат_вселенная_чел", "Aттестат Вселенная человека");
            //list.Add("Аттестат_вс_история", "Aттестат Всеобщая история");
            //list.Add("Аттестат_география", "Aттестат География");
            //list.Add("Аттестат_геометрия", "Aттестат Геометрия");
            //list.Add("Аттестат_информатика", "Aттестат Информатика");
            //list.Add("Аттестат_история_Спб", "Aттестат История и культура Санкт-Петербурга");
            //list.Add("Аттестат_ист_России", "Aттестат История России");
            //list.Add("Аттестат_литература", "Aттестат Литература");
            //list.Add("Аттестат_мировая_худ_культура", "Aттестат Мировая художественная культура");
            //list.Add("Аттестат_обществознание", "Aттестат Обществознание");
            //list.Add("Аттестат_ОБЖ", "Aттестат ОБЖ");
            //list.Add("Аттестат_русск_язык", "Aттестат Русский язык");
            //list.Add("Аттестат_технология", "Aттестат Технология");
            //list.Add("Аттестат_физика", "Aттестат Физика");
            //list.Add("Аттестат_физ_культура", "Aттестат Физическая культура");
            //list.Add("Аттестат_химия", "Aттестат Химия");
            //list.Add("Аттестат_экология", "Aттестат Экология");
            //list.Add("Аттестат_немецкий_язык", "Aттестат Немецкий язык");
            //list.Add("Аттестат_испанский_язык", "Aттестат Испанский язык");
            //list.Add("Аттестат_французский_язык", "Aттестат Французский язык");
            //list.Add("Аттестат_итальянский_язык", "Aттестат Итальянский язык");

            // олимпиады
            //list.Add("Всероссийкая", "Всероссийкая олимпиада");
            //list.Add("Международная", "Международная олимпиада");
            //list.Add("Региональная", "Региональная олимпиада");
            //list.Add("Межвузовская", "Межвузовская олимпиада");
            //list.Add("СПбГУ", "СПбГУ олимпиада");
            //list.Add("Школьников", "Олимпиада школьников");  

            //list.Add("Степень_диплома", "Степень диплома");
            //list.Add("Уровень_поступления", "Уровень_поступления");

            DataTable tbl = _bdc.GetDataSet(string.Format(@"
with t as (
SELECT distinct block.id
  FROM ed.ExamInEntryBlock block
  join ed.ExamInEntryBlockUnit unit on block.Id = unit.ExamInEntryBlockId
  join ed.extEntry on block.EntryId = extEntry.Id
  where extEntry.StudyLevelGroupId in ({0})
 Group by block.Id
 Having COUNT(unit.Id) > 1 
 )  
 select distinct Exam.Id, ExamName.Name
 from t
  join ed.ExamInEntryBlockUnit unit on t.Id = unit.ExamInEntryBlockId
  join ed.Exam on Exam.Id = unit.ExamId
  join ed.ExamName on ExamName.Id = Exam.ExamNameId ", Util.BuildStringWithCollection(MainClass.lstStudyLevelGroupId))).Tables[0];

            foreach (DataRow rw in tbl.Rows)
            {
                list.Add("Экзамен_по_выбору_" + rw.Field<string>("Name").Replace(" ", "_"), "Экзамен по выбору - " +rw.Field<string>("Name"));
            }
            
            return list;
        }
        protected override List<FilterItem> GetOtherFilterList(DBPriem _bdc)
        {
            List<FilterItem> lst = new List<FilterItem>();

            lst.Add(new FilterItem("Количество поданных заявлений", FilterType.FromTo, 
                @" 
(SELECT Count(Id) FROM  ed.Abiturient 
WHERE ed.Abiturient.BackDoc = 0 AND ed.Abiturient.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            lst.Add(new FilterItem("Экзамен по выбору", FilterType.Multi, @" ed.ExamInEntryBlockUnit.ExamId ", "ed.AbiturientSelectedExam", "SELECT Exam.Id, ExamName.Name FROM ed.Exam join ed.ExamName on Exam.ExamNameId = ExamName.Id ORDER BY Name"));
 
            //lst.Add(new FilterItem("Программы для лиц с ВО", FilterType.Bool, "ed.qAbiturient.IsSecond", "ed.qAbiturient"));
            //lst.Add(new FilterItem("Сокращенные программы", FilterType.Bool, "ed.qAbiturient.IsReduced", "ed.qAbiturient"));
            //lst.Add(new FilterItem("параллельные программы", FilterType.Bool, "ed.qAbiturient.IsParallel", "ed.qAbiturient"));

            lst.Add(new FilterItem("Медалист", FilterType.Bool, "ed.extPerson_EducationInfo_Current.IsExcellent", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Подан подлинник аттестата", FilterType.Bool, "ed.qAbiturient.HasOriginals", "ed.qAbiturient"));

            //lst.Add(new FilterItem("Номер аттестата", FilterType.FromTo, "ed.extPerson_EducationInfo_Current.AttestatNum", "ed.extPerson_EducationInfo_Current"));
            //lst.Add(new FilterItem("Серия аттестата", FilterType.FromTo, "ed.extPerson_EducationInfo_Current.AttestatSeries", "ed.extPerson_EducationInfo_Current"));
            //lst.Add(new FilterItem("Средний балл аттестата", FilterType.FromTo, "ed.extPerson_EducationInfo_Current.SchoolAVG", "ed.extPerson_EducationInfo_Current"));
            //lst.Add(new FilterItem("Введен средний балл аттестата", FilterType.Bool, "(NOT ed.extPerson_EducationInfo_Current.SchoolAVG IS NULL AND Len(ed.extPerson_EducationInfo_Current.SchoolAVG) > 0)", "ed.extPerson_EducationInfo_Current"));

            lst.Add(new FilterItem("Город учебного заведения", FilterType.Text, "ed.extPerson_EducationInfo_Current.SchoolCity", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Тип учебного заведения", FilterType.Multi, "ed.extPerson_EducationInfo_Current.SchoolTypeId", "ed.extPerson_EducationInfo_Current", "SELECT Id, Name FROM SchoolType ORDER BY Name"));
            lst.Add(new FilterItem("Название учебного заведения", FilterType.Text, "ed.extPerson_EducationInfo_Current.SchoolName", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Номер учебного заведения", FilterType.FromTo, "ed.extPerson_EducationInfo_Current.SchoolNum", "ed.extPerson_EducationInfo_Current"));
            lst.Add(new FilterItem("Год окончания учебного заведения", FilterType.FromTo, "ed.extPerson_EducationInfo_Current.SchoolExitYear", "ed.extPerson_EducationInfo_Current"));

            //lst.Add(new FilterItem("Уровень", FilterType.Multi, "ed.qAbiturient.StudyLevelId", "ed.qAbiturient", " SELECT DISTINCT Id, Name AS Name FROM ed.StudyLevel "));
            //lst.Add(new FilterItem("Профиль", FilterType.Multi, "ed.qAbiturient.ProfileId", "ed.qAbiturient", " SELECT DISTINCT ed.qProfile.Id, ed.qProfile.Name AS Name FROM ed.qProfile "));

            //lst.Add(new FilterItem("Статус ФБС", FilterType.Multi, "(SELECT FBSStatusId FROM ed.extFBSStatus WHERE ed.extFBSStatus.PersonId = ed.extPerson.Id)", "ed.extPerson", "SELECT Id, Name FROM ed.FBSStatus WHERE Id <> 3"));

            //олимпиады
            lst.Add(new FilterItem("Международная олимпиада", FilterType.Bool, " EXISTS (SELECT * FROM ed.Olympiads WHERE Olympiads.PersonId = qAbiturient.PersonId AND Olympiads.OlympTypeId=1 ) ", "ed.qAbiturient"));
            lst.Add(new FilterItem("Всероссийская олимпиада", FilterType.Bool, " EXISTS (SELECT * FROM ed.Olympiads WHERE Olympiads.PersonId = qAbiturient.PersonId AND Olympiads.OlympTypeId=2 ) ", "ed.qAbiturient"));
            lst.Add(new FilterItem("Олимпиада СПбГУ", FilterType.Bool, " EXISTS (SELECT * FROM ed.Olympiads WHERE Olympiads.PersonId = qAbiturient.PersonId AND Olympiads.OlympTypeId=3 ) ", "ed.qAbiturient"));
            lst.Add(new FilterItem("Другие олимпиады школьников", FilterType.Bool, " EXISTS (SELECT * FROM ed.Olympiads WHERE Olympiads.PersonId = qAbiturient.PersonId AND Olympiads.OlympTypeId=4 ) ", "ed.qAbiturient"));

            lst.Add(new FilterItem("Степень диплома олимпиады", FilterType.Multi, "(SELECT MAX(OlympValueId) FROM ed.Olympiads WHERE ed.Olympiads.AbiturientId = ed.qAbiturient.Id)", "ed.qAbiturient", " SELECT Id, Name FROM ed.OlympValue "));

            //ЕГЭ
            //lst.Add(new FilterItem("Номер свидетельства ЕГЭ 2011 года", FilterType.FromTo, " (SELECT Top(1) ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2011)", "ed.extPerson"));
            //lst.Add(new FilterItem("Номер свидетельства ЕГЭ 2012 года", FilterType.FromTo, " (SELECT Top(1) ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2012)", "ed.extPerson"));
            //lst.Add(new FilterItem("Номер свидетельства ЕГЭ 2013 года", FilterType.FromTo, " (SELECT Top(1) ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2012)", "ed.extPerson"));
            //lst.Add(new FilterItem("Загружено cвид-во ЕГЭ 2012 года", FilterType.Bool, "EXISTS (SELECT Top(1) ed.EgeCertificate.IsImported FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND Year=2012 AND IsImported > 0)", "ed.extPerson"));
            //lst.Add(new FilterItem("Загружено cвид-во ЕГЭ 2013 года", FilterType.Bool, "EXISTS (SELECT Top(1) ed.EgeCertificate.IsImported FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND Year=2013 AND IsImported > 0)", "ed.extPerson"));
            //lst.Add(new FilterItem("Загружено cвид-во ЕГЭ 2014 года", FilterType.Bool, "EXISTS (SELECT Top(1) ed.EgeCertificate.IsImported FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND Year=2014 AND IsImported > 0)", "ed.extPerson"));
            //lst.Add(new FilterItem("Загружено cвид-во ЕГЭ 2015 года", FilterType.Bool, "EXISTS (SELECT Top(1) ed.EgeCertificate.IsImported FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND Year=2015 AND IsImported > 0)", "ed.extPerson"));
            //lst.Add(new FilterItem("Сдавал ЕГЭ", FilterType.Bool, " EXISTS (SELECT ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));

            //lst.Add(new FilterItem("Есть свидетельство ЕГЭ 2012 года", FilterType.Bool, " EXISTS (SELECT ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2012)", "ed.extPerson"));
            //lst.Add(new FilterItem("Есть свидетельство ЕГЭ 2013 года", FilterType.Bool, " EXISTS (SELECT ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2013)", "ed.extPerson"));
            //lst.Add(new FilterItem("Есть свидетельство ЕГЭ 2014 года", FilterType.Bool, " EXISTS (SELECT ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2014)", "ed.extPerson"));
            //lst.Add(new FilterItem("Есть свидетельство ЕГЭ 2015 года", FilterType.Bool, " EXISTS (SELECT ed.EgeCertificate.Number FROM ed.EgeCertificate WHERE ed.EgeCertificate.PersonId = ed.extPerson.Id AND ed.EgeCertificate.Year = 2015)", "ed.extPerson"));

            //инд достижения
            lst.Add(new FilterItem("Инд.достижения: Аттестат с отличием", FilterType.Bool, " EXISTS (SELECT * FROM ed.PersonAchievement PA WHERE PA.PersonId = extPerson.Id AND PA.AchievementTypeId = 9)", "ed.extPerson"));
            lst.Add(new FilterItem("Инд.достижения: Волонтёр", FilterType.Bool, " EXISTS (SELECT * FROM ed.PersonAchievement PA WHERE PA.PersonId = extPerson.Id AND PA.AchievementTypeId = 10)", "ed.extPerson"));
            lst.Add(new FilterItem("Инд.достижения: Победитель рег. этапа Всеросс", FilterType.Bool, " EXISTS (SELECT * FROM ed.Olympiads OL WHERE OL.PersonId = qAbiturient.PersonId AND OL.OlympTypeId = 7 AND OL.OlympValueId = 6)", "ed.qAbiturient"));
            lst.Add(new FilterItem("Инд.достижения: Призёр рег. этапа Всеросс", FilterType.Bool, " EXISTS (SELECT * FROM ed.Olympiads OL WHERE OL.PersonId = qAbiturient.PersonId AND OL.OlympTypeId = 7 AND OL.OlympValueId IN (5, 7))", "ed.qAbiturient"));
            lst.Add(new FilterItem("Инд.достижения: Прочие конкурсы", FilterType.Bool, " EXISTS (SELECT * FROM ed.PersonAchievement PA WHERE PA.PersonId = extPerson.Id AND PA.AchievementTypeId = 11)", "ed.extPerson"));
            lst.Add(new FilterItem("Инд.достижения: Сочинение (оценка \"зачёт\")", FilterType.Bool, " EXISTS (SELECT * FROM ed.PersonAchievement PA WHERE PA.PersonId = extPerson.Id AND PA.AchievementTypeId = 12)", "ed.extPerson"));

            //lst.Add(new FilterItem("Апелляция", FilterType.Bool, " EXISTS (SELECT * FROM ed.EgeMark LEFT JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id WHERE ed.EgeMark.IsAppeal>0 AND ed.EgeCertificate.PersonId = ed.extPerson.Id) ", "ed.extPerson"));
            //lst.Add(new FilterItem("Из олимпиад", FilterType.Bool, " EXISTS (SELECT * FROM ed.EgeMark LEFT JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id WHERE ed.EgeMark.IsFromOlymp>0 AND ed.EgeCertificate.PersonId = ed.extPerson.Id) ", "ed.extPerson"));
            //lst.Add(new FilterItem("ЕГЭ Английский язык", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Английский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("ЕГЭ Русский язык", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Русский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("ЕГЭ Математика", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Математика' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("ЕГЭ Физика", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Физика' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("ЕГЭ Химия", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Химия' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("ЕГЭ Биология", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Биология' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("ЕГЭ История", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='История' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("ЕГЭ География", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='География' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("ЕГЭ Немецкий язык", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Немецкий язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("ЕГЭ Французский язык", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Французский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("ЕГЭ Обществознание", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Обществознание' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("ЕГЭ Литература", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Литература' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("ЕГЭ Испанский язык", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Испанский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("ЕГЭ Информатика и ИКТ", FilterType.FromTo, " (SELECT Max(ed.EgeMark.Value) FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Информатика и ИКТ' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));

            //lst.Add(new FilterItem("Сдавал ЕГЭ Английский язык", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Английский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("Сдавал ЕГЭ Русский язык", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Русский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("Сдавал ЕГЭ Математика", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Математика' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("Сдавал ЕГЭ Физика", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Физика' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("Сдавал ЕГЭ Химия", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Химия' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("Сдавал ЕГЭ Биология", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Биология' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("Сдавал ЕГЭ История", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='История' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("Сдавал ЕГЭ География", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='География' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("Сдавал ЕГЭ Немецкий язык", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Немецкий язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("Сдавал ЕГЭ Французский язык", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Французский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("Сдавал ЕГЭ Обществознание", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Обществознание' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("Сдавал ЕГЭ Литература", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Литература' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("Сдавал ЕГЭ Испанский язык", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Испанский язык' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("Сдавал ЕГЭ Информатика и ИКТ", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id)INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.NAme='Информатика и ИКТ' AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));
            //lst.Add(new FilterItem("Сдавал Сочинение", FilterType.Bool, " EXISTS (SELECT * FROM (ed.EgeMark INNER JOIN ed.EgeCertificate ON ed.EgeMark.EgeCertificateId = ed.EgeCertificate.Id) INNER JOIN ed.EgeExamName ON ed.EgeMark.EgeExamNameId = ed.EgeExamName.Id WHERE ed.EgeExamName.Id = 15 AND ed.EgeCertificate.PersonId = ed.extPerson.Id)", "ed.extPerson"));

            //lst.Add(new FilterItem("Сдавал ЕГЭ в СПбГУ", FilterType.Bool, " EXISTS (SELECT * FROM ed.qMark INNER JOIN ed.extExamInEntry ON qMark.ExamInEntryBlockUnitId = ed.extExamInEntry.Id WHERE ed.qMark.IsFromEge = 0 AND ed.qMark.IsFromOlymp = 0 AND ed.extExamInEntry.IsAdditional = 0 AND ed.qMark.AbiturientId = ed.qAbiturient.Id)", "ed.qAbiturient"));

            return lst;
        }
        protected override List<ListItem> GetOtherGroups(DBPriem _bdc)
        {
            return new List<ListItem>();
        }
    }

}
