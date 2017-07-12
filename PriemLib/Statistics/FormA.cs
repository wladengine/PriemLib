using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WordOut;
using EducServLib;
using BaseFormsLib;

namespace PriemLib
{
    public partial class FormA : BaseForm
    {
        private DateTime _One = new DateTime(MainClass.iPriemYear, 07, 6, 23, 59, 59);
        private DateTime _Two = new DateTime(MainClass.iPriemYear, 07, 17, 23, 59, 59);
        private DateTime _Three = new DateTime(MainClass.iPriemYear, 07, 27, 23, 59, 59);

        private DBPriem bdcInet = new DBPriem();
        NewWatch wc;

        public FormA()
        {
            InitializeComponent();
            this.MdiParent = MainClass.mainform;
            bdcInet.OpenDatabase(MainClass.connStringOnline);
            GetKC();
            try
            {
                wc = new NewWatch(DateTime.Now.Date > _One ? (DateTime.Now.Date > _Two ? 33 : 22) : 11);
                wc.Show();
                Fill_1();
                if (DateTime.Now.Date > _One)
                    Fill_2();
                if (DateTime.Now.Date > _Two)
                    Fill_3();
            }
            catch (Exception ex) { WinFormsServ.Error(ex); }
            wc.Close();
            wc = null;
            bdcInet.CloseDataBase();
        }

        private void GetKC()
        {
            string query = "SELECT SUM(KCP) AS KC FROM ed.qEntry WHERE StudyLevelGroupId=@SLGId AND StudyFormId='1' AND IsForeign=0 AND IsCrimea=0";
            MainClass.Bdc.GetValue(query, new SortedList<string, object>() { { "@SLGId", MainClass.lstStudyLevelGroupId.First() } });
            tbKC.Text = MainClass.Bdc.GetValue(query, new SortedList<string, object>() { { "@SLGId", MainClass.lstStudyLevelGroupId.First() } }).ToString();
        }

        #region Fills
        private void Fill_1()
        {
            using (PriemEntities context = new PriemEntities())
            {
                DateTime _day = _One;
                wc.SetText("подано заявлений всего");
                //подано заявлений всего
                //база "Приём"
                string query = "SELECT StudyBasisId, SUM(CNT) AS CNT FROM ed.hlpStatistics WHERE Date<=@Date AND StudyLevelGroupId=@SLGId AND StudyFormId='1' GROUP BY StudyBasisId";
                DataTable tbl = MainClass.Bdc.GetDataSet(query,
                    new SortedList<string, object>() { { "@Date", _day }, { "@SLGId", MainClass.lstStudyLevelGroupId.First() } }).Tables[0];

                int iPriemCountB = context.hlpStatistics
                    .Where(x => x.StudyFormId == 1 && x.StudyBasisId == 1 && x.Date <= _day && MainClass.lstStudyLevelGroupId.Contains(x.StudyLevelGroupId))
                    .Select(x => x.CNT).Sum() ?? 0;
                int iPriemCountP = context.hlpStatistics
                    .Where(x => x.StudyFormId == 1 && x.StudyBasisId == 2 && x.Date <= _day && MainClass.lstStudyLevelGroupId.Contains(x.StudyLevelGroupId))
                    .Select(x => x.CNT).Sum() ?? 0;
                tb1_Budzh.Text = iPriemCountB.ToString();
                tb1_Platn.Text = iPriemCountP.ToString();
                tb1_FullCount.Text = (iPriemCountB + iPriemCountP).ToString();
                wc.PerformStep();

                wc.SetText("петербуржцев");
                //петербуржцев
                var spb = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.DocInsertDate <= _day && x.Person.Person_Contacts.RegionId == 1 && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                tb1_SpbBudzh.Text = spb.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb1_SpbPlatn.Text = spb.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                var mens = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.DocInsertDate <= _day && x.Person.Sex == true && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                wc.SetText("мужчин");
                //мужчин
                tb1_MaleBudzh.Text = mens.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb1_MalePlatn.Text = mens.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("Имеющих среднее(полное) среднее образование");
    //            //школьники
                var School =
                    (from Ab in context.Abiturient
                     join Ent in context.Entry on Ab.EntryId equals Ent.Id
                     join Pers in context.Person on Ab.PersonId equals Pers.Id
                     join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                     where Ab.Entry.StudyFormId == 1
                     && MainClass.lstStudyLevelGroupId.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                     && Ab.DocInsertDate <= _day
                     && PersEduc.SchoolTypeId == 1
                     && !Ent.IsForeign
                     select new
                     {
                         Ent.StudyBasisId,
                         Ab.Id
                     }).Distinct();
                tb1_SchoolBudzh.Text = School.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb1_SchoolPlatn.Text = School.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("Имеющих среднее профессиональное образование");
                //СПО
                var SPO_VPO = (from Ab in context.Abiturient
                               join Ent in context.Entry on Ab.EntryId equals Ent.Id
                               join Pers in context.Person on Ab.PersonId equals Pers.Id
                               join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                               where Ab.Entry.StudyFormId == 1
                               && MainClass.lstStudyLevelGroupId.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                               && Ab.DocInsertDate <= _day
                               && PersEduc.SchoolTypeId != 1 && PersEduc.SchoolTypeId != 3 && PersEduc.SchoolTypeId != 4
                               && !Ent.IsForeign
                               select new
                               {
                                   Ent.StudyBasisId,
                                   Ab.Id
                               }).Distinct();
                tb1_ProfBudzh.Text = SPO_VPO.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb1_ProfPlatn.Text = SPO_VPO.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("Имеющих высшее профессиональное образование");
                //ВПО
                var NPO = (from Ab in context.Abiturient
                           join Ent in context.Entry on Ab.EntryId equals Ent.Id
                           join Pers in context.Person on Ab.PersonId equals Pers.Id
                           join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                           where Ab.Entry.StudyFormId == 1
                           && MainClass.lstStudyLevelGroupId.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                           && Ab.DocInsertDate <= _day
                           && PersEduc.SchoolTypeId == 4
                           && !Ent.IsForeign
                           select new
                           {
                               Ent.StudyBasisId,
                               Ab.Id
                           }).Distinct();
                tb1_NPOBudzh.Text = NPO.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb1_NPOPlatn.Text = NPO.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("Победителей и призёров олимпиад");
                //Олимпиадники
                var Olymps =
                    (from Ol in context.Olympiads
                     join Ab in context.Abiturient on Ol.Id equals Ab.OlympiadId
                     where Ab.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                         && Ab.DocInsertDate <= _day && !Ab.Entry.IsForeign
                         && (Ol.OlympTypeId == 3 || Ol.OlympTypeId == 4)
                         && (Ol.OlympValueId == 5 || Ol.OlympValueId == 6)
                     select new
                     {
                         Ab.Entry.StudyBasisId,
                         Ab.Id
                     }).Distinct()
                     .Union(from Ol in context.Olympiads
                            join Mrk in context.Mark on Ol.Id equals Mrk.OlympiadId
                            join Ab in context.Abiturient on Mrk.AbiturientId equals Ab.Id
                            where Ab.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                                && Ab.DocInsertDate <= _day && !Ab.Entry.IsForeign
                                && (Ol.OlympTypeId == 3 || Ol.OlympTypeId == 4)
                                && (Ol.OlympValueId == 5 || Ol.OlympValueId == 6)
                            select new
                            {
                                Ab.Entry.StudyBasisId,
                                Ab.Id
                            }).ToList().Distinct();
                tb1_OlympBudzh.Text = Olymps.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb1_OlympPlatn.Text = Olymps.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("в/к");
                //в\к
                var VK = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.DocInsertDate <= _day && (x.CompetitionId == 2 || x.CompetitionId == 7) && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                tb1_VKBudzh.Text = VK.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb1_VKPlatn.Text = VK.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("иностранцев");
                //иностранцы
                var Foreigners = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.DocInsertDate <= _day && x.Person.NationalityId != 1 && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                int iPriemForeignersB = Foreigners.Where(x => x.StudyBasisId == 1).Count();
                int iPriemForeignersP = Foreigners.Where(x => x.StudyBasisId == 2).Count();
                tb1_ForeignBudzh.Text = iPriemForeignersB.ToString();
                tb1_ForeignPlatn.Text = iPriemForeignersP.ToString();
                wc.PerformStep();

                wc.SetText("Граждане б. СССР, кр. России");
                //USSR
                var USSR = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.DocInsertDate <= _day && x.Person.NationalityId != 1 && x.Person.NationalityId != 6 && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                tb1_USSRBudzh.Text = USSR.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb1_USSRPlatn.Text = USSR.Where(x => x.StudyBasisId == 2).Count().ToString();
            
                wc.PerformStep();

                wc.SetText("Макс. конкурс " + _One.ToShortDateString());
                //max concurs
                query = @"SELECT TOP 1
      t.[LicenseProgramCode] + ' ' + t.[LicenseProgramName] AS Name, convert(float,t.[_cnt]) / (convert(float,t.[_sum])) AS conk
      FROM 
    (
    SELECT     StudyLevelGroupId, LicenseProgramId, LicenseProgramCode, LicenseProgramName, StudyFormId, StudyBasisId, SUM(KCP) AS _sum,
               (SELECT COUNT(Id) AS Expr1
                FROM ed.qAbiturient
                WHERE (StudyLevelGroupId = ed.qEntry.StudyLevelGroupId) AND (LicenseProgramId = ed.qEntry.LicenseProgramId) AND 
                      (StudyFormId = ed.qEntry.StudyFormId) AND (StudyBasisId = ed.qEntry.StudyBasisId) AND convert(date, DocInsertDate)<=@Date AND qAbiturient.IsForeign = 0 AND qAbiturient.IsCrimea = 0) AS _cnt
    FROM         ed.qEntry
    WHERE IsForeign = 0 AND IsCrimea = 0
    GROUP BY StudyLevelGroupId, LicenseProgramId, LicenseProgramCode, LicenseProgramName, StudyFormId, StudyBasisId
    ) t
      WHERE t.[StudyLevelGroupId] = @SLGId AND t.StudyFormId = 1 AND t.StudyBasisId = 1 AND t._sum > 0
      ORDER BY conk DESC";
                tbl = MainClass.Bdc.GetDataSet(query,
                    new SortedList<string, object>() { { "@Date", _day }, { "@SLGId", MainClass.lstStudyLevelGroupId.First() } }).Tables[0];
                tb1_MaxConcurs.Text = Math.Round(tbl.Rows[0].Field<double>("conk"), 2).ToString() + " " + tbl.Rows[0].Field<string>("Name");
                wc.PerformStep();

                tb1_.Text = DateTime.Now < _day ? DateTime.Now.ToShortDateString() : _day.ToShortDateString();
            }
        }
        private void Fill_2()
        {
            DateTime _day = _Two;
            using (PriemEntities context = new PriemEntities())
            {
                wc.SetText("подано заявлений всего");
                //подано заявлений всего
                int iPriemCountB = context.hlpStatistics
                    .Where(x => x.StudyFormId == 1 && x.StudyBasisId == 1 && x.Date <= _day && MainClass.lstStudyLevelGroupId.Contains(x.StudyLevelGroupId))
                    .Select(x => x.CNT).Sum() ?? 0;
                int iPriemCountP = context.hlpStatistics
                    .Where(x => x.StudyFormId == 1 && x.StudyBasisId == 2 && x.Date <= _day && MainClass.lstStudyLevelGroupId.Contains(x.StudyLevelGroupId))
                    .Select(x => x.CNT).Sum() ?? 0;
                tb2_Budzh.Text = iPriemCountB.ToString();
                tb2_Platn.Text = iPriemCountP.ToString();
                tb2_FullCount.Text = (iPriemCountB + iPriemCountP).ToString();
                wc.PerformStep();

                var spb = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.DocInsertDate <= _day && x.Person.Person_Contacts.RegionId == 1 && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                tb2_SpbBudzh.Text = spb.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb2_SpbPlatn.Text = spb.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                var mens = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.DocInsertDate <= _day && x.Person.Sex == true && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                wc.SetText("мужчин");
                //мужчин
                tb2_MaleBudzh.Text = mens.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb2_MalePlatn.Text = mens.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("Имеющих среднее(полное) среднее образование");
                var School = (from Ab in context.Abiturient
                              join Ent in context.Entry on Ab.EntryId equals Ent.Id
                              join Pers in context.Person on Ab.PersonId equals Pers.Id
                              join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                              where Ab.Entry.StudyFormId == 1
                              && MainClass.lstStudyLevelGroupId.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                              && Ab.DocInsertDate <= _day
                              && PersEduc.SchoolTypeId == 1
                              && !Ent.IsForeign
                              select new
                              {
                                  Ent.StudyBasisId,
                                  Ab.Id
                              }).Distinct();
                tb2_SchoolBudzh.Text = School.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb2_SchoolPlatn.Text = School.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("Имеющих среднее профессиональное образование");
                var SPO_VPO = (from Ab in context.Abiturient
                               join Ent in context.Entry on Ab.EntryId equals Ent.Id
                               join Pers in context.Person on Ab.PersonId equals Pers.Id
                               join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                               where Ab.Entry.StudyFormId == 1
                               && MainClass.lstStudyLevelGroupId.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                               && Ab.DocInsertDate <= _day
                               && PersEduc.SchoolTypeId != 1 && PersEduc.SchoolTypeId != 3 && PersEduc.SchoolTypeId != 4
                               && !Ent.IsForeign
                               select new
                               {
                                   Ent.StudyBasisId,
                                   Ab.Id
                               }).Distinct();
                tb2_ProfBudzh.Text = SPO_VPO.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb2_ProfPlatn.Text = SPO_VPO.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("Имеющих высшее профессиональное образование");
                var NPO = (from Ab in context.Abiturient
                               join Ent in context.Entry on Ab.EntryId equals Ent.Id
                               join Pers in context.Person on Ab.PersonId equals Pers.Id
                               join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                               where Ab.Entry.StudyFormId == 1
                               && MainClass.lstStudyLevelGroupId.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                               && Ab.DocInsertDate <= _day
                               && PersEduc.SchoolTypeId == 4
                               && !Ent.IsForeign
                               select new
                               {
                                   Ent.StudyBasisId,
                                   Ab.Id
                               }).Distinct();
                tb2_NPOBudzh.Text = NPO.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb2_NPOPlatn.Text = NPO.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("Победителей и призёров олимпиад");
                //Олимпиадники
                var Olymps =
                    (from Ol in context.Olympiads
                     join Ab in context.Abiturient on Ol.Id equals Ab.OlympiadId
                     where Ab.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                         && Ab.DocInsertDate <= _day && !Ab.Entry.IsForeign
                         && (Ol.OlympTypeId == 3 || Ol.OlympTypeId == 4)
                         && (Ol.OlympValueId == 5 || Ol.OlympValueId == 6)
                     select new
                     {
                         Ab.Entry.StudyBasisId,
                         Ab.Id
                     }).Distinct()
                     .Union(from Ol in context.Olympiads
                            join Mrk in context.Mark on Ol.Id equals Mrk.OlympiadId
                            join Ab in context.Abiturient on Mrk.AbiturientId equals Ab.Id
                            where Ab.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                                && Ab.DocInsertDate <= _day && !Ab.Entry.IsForeign
                                && (Ol.OlympTypeId == 3 || Ol.OlympTypeId == 4)
                                && (Ol.OlympValueId == 5 || Ol.OlympValueId == 6)
                            select new
                            {
                                Ab.Entry.StudyBasisId,
                                Ab.Id
                            }).ToList().Distinct();
                tb2_OlympBudzh.Text = Olymps.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb2_OlympPlatn.Text = Olymps.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("в/к");
                //в\к
                var VK = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.DocInsertDate <= _day && (x.CompetitionId == 2 || x.CompetitionId == 7) && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                tb2_VKBudzh.Text = VK.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb2_VKPlatn.Text = VK.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("иностранцев");
                //иностранцы
                var Foreigners = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.DocInsertDate <= _day && x.Person.NationalityId != 1 && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                int iPriemForeignersB = Foreigners.Where(x => x.StudyBasisId == 1).Count();
                int iPriemForeignersP = Foreigners.Where(x => x.StudyBasisId == 2).Count();
                tb2_ForeignBudzh.Text = iPriemForeignersB.ToString();
                tb2_ForeignPlatn.Text = iPriemForeignersP.ToString();
                wc.PerformStep();

                wc.SetText("Граждане б. СССР, кр. России");
                //USSR
                var USSR = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.DocInsertDate <= _day && x.Person.NationalityId != 1 && x.Person.NationalityId != 6 && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                tb2_USSRBudzh.Text = USSR.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb2_USSRPlatn.Text = USSR.Where(x => x.StudyBasisId == 2).Count().ToString();

                wc.PerformStep();

                wc.SetText("Макс. конкурс " + _day.ToShortDateString());
                //max concurs
                string query = @"SELECT TOP 1
      t.[LicenseProgramCode] + ' ' + t.[LicenseProgramName] AS Name, convert(float,t.[_cnt]) / (convert(float,t.[_sum])) AS conk
      FROM 
    (
    SELECT     StudyLevelGroupId, LicenseProgramId, LicenseProgramCode, LicenseProgramName, StudyFormId, StudyBasisId, SUM(KCP) AS _sum,
               (SELECT COUNT(Id) AS Expr1
                FROM ed.qAbiturient
                WHERE (StudyLevelGroupId = ed.qEntry.StudyLevelGroupId) AND (LicenseProgramId = ed.qEntry.LicenseProgramId) AND 
                      (StudyFormId = ed.qEntry.StudyFormId) AND (StudyBasisId = ed.qEntry.StudyBasisId) AND convert(date, DocInsertDate)<=@Date AND qAbiturient.IsForeign = 0 AND qAbiturient.IsCrimea = 0) AS _cnt
    FROM         ed.qEntry
    WHERE IsForeign = 0 AND IsCrimea = 0
    GROUP BY StudyLevelGroupId, LicenseProgramId, LicenseProgramCode, LicenseProgramName, StudyFormId, StudyBasisId
    ) t
      WHERE t.[StudyLevelGroupId] = @SLGId AND t.StudyFormId = 1 AND t.StudyBasisId = 1 AND t._sum > 0
      ORDER BY conk DESC";
                DataTable tbl = MainClass.Bdc.GetDataSet(query,
                    new SortedList<string, object>() { { "@Date", _day }, { "@SLGId", MainClass.lstStudyLevelGroupId.First() } }).Tables[0];
                tb2_MaxConcurs.Text = Math.Round(tbl.Rows[0].Field<double>("conk"), 2).ToString() + " " + tbl.Rows[0].Field<string>("Name");
                wc.PerformStep();

                tb2_.Text = DateTime.Now < _day ? DateTime.Now.ToShortDateString() : _day.ToShortDateString();
            }
        }
        private void Fill_3()
        {
            DateTime _day = _Three;

            using (PriemEntities context = new PriemEntities())
            {
                wc.SetText("подано заявлений всего");
                //подано заявлений всего
                int iPriemCountB = context.hlpStatistics
                    .Where(x => x.StudyFormId == 1 && x.StudyBasisId == 1 && x.Date <= _day && MainClass.lstStudyLevelGroupId.Contains(x.StudyLevelGroupId))
                    .Select(x => x.CNT).Sum() ?? 0;
                int iPriemCountP = context.hlpStatistics
                    .Where(x => x.StudyFormId == 1 && x.StudyBasisId == 2 && x.Date <= _day && MainClass.lstStudyLevelGroupId.Contains(x.StudyLevelGroupId))
                    .Select(x => x.CNT).Sum() ?? 0;
                tb3_Budzh.Text = iPriemCountB.ToString();
                tb3_Platn.Text = iPriemCountP.ToString();
                tb3_FullCount.Text = (iPriemCountB + iPriemCountP).ToString();
                wc.PerformStep();

                wc.SetText("петербуржцев");
                //петербуржцев
                var spb = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.DocInsertDate <= _day && x.Person.Person_Contacts.RegionId == 1 && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                tb3_SpbBudzh.Text = spb.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb3_SpbPlatn.Text = spb.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                var mens = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.DocInsertDate <= _day && x.Person.Sex == true && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                wc.SetText("мужчин");
                //мужчин
                tb3_MaleBudzh.Text = mens.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb3_MalePlatn.Text = mens.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("Имеющих среднее(полное) среднее образование");
                var School = (from Ab in context.Abiturient
                              join Ent in context.Entry on Ab.EntryId equals Ent.Id
                              join Pers in context.Person on Ab.PersonId equals Pers.Id
                              join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                              where Ab.Entry.StudyFormId == 1
                              && MainClass.lstStudyLevelGroupId.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                              && Ab.DocInsertDate <= _day
                              && PersEduc.SchoolTypeId == 1
                              && !Ent.IsForeign
                              select new
                              {
                                  Ent.StudyBasisId,
                                  Ab.Id
                              }).Distinct();
                tb3_SchoolBudzh.Text = School.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb3_SchoolPlatn.Text = School.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("Имеющих среднее профессиональное образование");
                var SPO_VPO = (from Ab in context.Abiturient
                               join Ent in context.Entry on Ab.EntryId equals Ent.Id
                               join Pers in context.Person on Ab.PersonId equals Pers.Id
                               join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                               where Ab.Entry.StudyFormId == 1
                               && MainClass.lstStudyLevelGroupId.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                               && Ab.DocInsertDate <= _day
                               && PersEduc.SchoolTypeId != 1 && PersEduc.SchoolTypeId != 3 && PersEduc.SchoolTypeId != 4
                               && !Ent.IsForeign
                               select new
                               {
                                   Ent.StudyBasisId,
                                   Ab.Id
                               }).Distinct();
                tb3_ProfBudzh.Text = SPO_VPO.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb3_ProfPlatn.Text = SPO_VPO.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("Имеющих высшее профессиональное образование");
                var NPO = (from Ab in context.Abiturient
                           join Ent in context.Entry on Ab.EntryId equals Ent.Id
                           join Pers in context.Person on Ab.PersonId equals Pers.Id
                           join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                           where Ab.Entry.StudyFormId == 1
                           && MainClass.lstStudyLevelGroupId.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                           && Ab.DocInsertDate <= _day
                           && PersEduc.SchoolTypeId == 4
                           && !Ent.IsForeign
                           select new
                           {
                               Ent.StudyBasisId,
                               Ab.Id
                           }).Distinct();
                tb3_NPOBudzh.Text = NPO.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb3_NPOPlatn.Text = NPO.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("Победителей и призёров олимпиад");
                //Олимпиадники
                var Olymps =
                    (from Ol in context.Olympiads
                     join Ab in context.Abiturient on Ol.Id equals Ab.OlympiadId
                     where Ab.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                         && Ab.DocInsertDate <= _day && !Ab.Entry.IsForeign
                         && (Ol.OlympTypeId == 3 || Ol.OlympTypeId == 4)
                         && (Ol.OlympValueId == 5 || Ol.OlympValueId == 6)
                     select new
                     {
                         Ab.Entry.StudyBasisId,
                         Ab.Id
                     }).Distinct()
                     .Union(from Ol in context.Olympiads
                            join Mrk in context.Mark on Ol.Id equals Mrk.OlympiadId
                            join Ab in context.Abiturient on Mrk.AbiturientId equals Ab.Id
                            where Ab.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                                && Ab.DocInsertDate <= _day && !Ab.Entry.IsForeign
                                && (Ol.OlympTypeId == 3 || Ol.OlympTypeId == 4)
                                && (Ol.OlympValueId == 5 || Ol.OlympValueId == 6)
                            select new
                            {
                                Ab.Entry.StudyBasisId,
                                Ab.Id
                            }).ToList().Distinct();
                tb3_OlympBudzh.Text = Olymps.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb3_OlympPlatn.Text = Olymps.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("в/к");
                //в\к
                var VK = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.DocInsertDate <= _day && (x.CompetitionId == 2 || x.CompetitionId == 7) && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                tb3_VKBudzh.Text = VK.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb3_VKPlatn.Text = VK.Where(x => x.StudyBasisId == 2).Count().ToString();
                wc.PerformStep();

                wc.SetText("иностранцев");
                //иностранцы
                var Foreigners = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.DocInsertDate <= _day && x.Person.NationalityId != 1 && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                int iPriemForeignersB = Foreigners.Where(x => x.StudyBasisId == 1).Count();
                int iPriemForeignersP = Foreigners.Where(x => x.StudyBasisId == 2).Count();
                tb3_ForeignBudzh.Text = iPriemForeignersB.ToString();
                tb3_ForeignPlatn.Text = iPriemForeignersP.ToString();
                wc.PerformStep();

                wc.SetText("Граждане б. СССР, кр. России");
                //USSR
                var USSR = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.DocInsertDate <= _day && x.Person.NationalityId != 1 && x.Person.NationalityId != 6 && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                tb3_USSRBudzh.Text = USSR.Where(x => x.StudyBasisId == 1).Count().ToString();
                tb3_USSRPlatn.Text = USSR.Where(x => x.StudyBasisId == 2).Count().ToString();

                wc.PerformStep();

                wc.SetText("Макс. конкурс " + _day.ToShortDateString());
                //max concurs
                string query = @"SELECT TOP 1
      t.[LicenseProgramCode] + ' ' + t.[LicenseProgramName] AS Name, convert(float,t.[_cnt]) / (convert(float,t.[_sum])) AS conk
      FROM 
    (
    SELECT     StudyLevelGroupId, LicenseProgramId, LicenseProgramCode, LicenseProgramName, StudyFormId, StudyBasisId, SUM(KCP) AS _sum,
               (SELECT COUNT(Id) AS Expr1
                FROM ed.qAbiturient
                WHERE (StudyLevelGroupId = ed.qEntry.StudyLevelGroupId) AND (LicenseProgramId = ed.qEntry.LicenseProgramId) AND 
                      (StudyFormId = ed.qEntry.StudyFormId) AND (StudyBasisId = ed.qEntry.StudyBasisId) AND convert(date, DocInsertDate)<=@Date AND qAbiturient.IsForeign = 0 AND qAbiturient.IsCrimea = 0) AS _cnt
    FROM         ed.qEntry
    WHERE IsForeign = 0 AND IsCrimea = 0
    GROUP BY StudyLevelGroupId, LicenseProgramId, LicenseProgramCode, LicenseProgramName, StudyFormId, StudyBasisId
    ) t
      WHERE t.[StudyLevelGroupId] = @SLGId AND t.StudyFormId = 1 AND t.StudyBasisId = 1 AND t._sum > 0
      ORDER BY conk DESC";
                DataTable tbl = MainClass.Bdc.GetDataSet(query,
                    new SortedList<string, object>() { { "@Date", _day }, { "@SLGId", MainClass.lstStudyLevelGroupId.First() } }).Tables[0];
                tb3_MaxConcurs.Text = Math.Round(tbl.Rows[0].Field<double>("conk"), 2).ToString() + " " + tbl.Rows[0].Field<string>("Name");
                wc.PerformStep();

                tb3_.Text = DateTime.Now < _day ? DateTime.Now.ToShortDateString() : _day.ToShortDateString();
            }
        }
        #endregion

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            bdcInet.OpenDatabase(MainClass.connStringOnline);
            wc = new NewWatch(11);
            wc.Show();
            try
            {
                if (DateTime.Now < _One)
                    Fill_1();
                else if (DateTime.Now.Date > _One && DateTime.Now < _Two)
                    Fill_2();
                else if (DateTime.Now.Date > _Two && DateTime.Now < _Three)
                    Fill_3();
                wc.Close();
            }
            catch { }
            wc.Close();
            wc = null;
            bdcInet.CloseDataBase();
        }
        private void btnWord_Click(object sender, EventArgs e)
        {
            try
            {
                //MessageBox.Show("NotRealizedYet");
                WordDoc doc = new WordDoc(MainClass.dirTemplates + "\\FormA.dot", true);

                doc.SetFields("VUZNAME", "");
                doc.SetFields("KCP", tbKC.Text);

                //First part
                doc.SetFields("CNT1", tb1_FullCount.Text);
                doc.SetFields("CNT1_B", tb1_Budzh.Text);
                doc.SetFields("CNT1_P", tb1_Platn.Text);
                doc.SetFields("CNT1_SPB_B", tb1_SpbBudzh.Text);
                doc.SetFields("CNT1_SPB_P", tb1_SpbPlatn.Text);
                doc.SetFields("CNT1_MALE_B", tb1_MaleBudzh.Text);
                doc.SetFields("CNT1_MALE_P", tb1_MalePlatn.Text);
                doc.SetFields("CNT1_SCHOOL_B", tb1_SchoolBudzh.Text);
                doc.SetFields("CNT1_SCHOOL_P", tb1_SchoolPlatn.Text);
                doc.SetFields("CNT1_PROF_B", tb1_ProfBudzh.Text);
                doc.SetFields("CNT1_PROF_P", tb1_ProfPlatn.Text);
                doc.SetFields("CNT1_NPO_B", tb1_NPOBudzh.Text);
                doc.SetFields("CNT1_NPO_P", tb1_NPOPlatn.Text);
                doc.SetFields("CNT1_OLYMP_B", tb1_OlympBudzh.Text);
                doc.SetFields("CNT1_OLYMP_P", tb1_OlympPlatn.Text);
                doc.SetFields("CNT1_VK_B", tb1_VKBudzh.Text);
                doc.SetFields("CNT1_VK_P", tb1_VKPlatn.Text);
                doc.SetFields("CNT1_FOREIGN_B", tb1_ForeignBudzh.Text);
                doc.SetFields("CNT1_FOREIGN_P", tb1_ForeignPlatn.Text);
                doc.SetFields("CNT1_USSR_B", tb1_USSRBudzh.Text);
                doc.SetFields("CNT1_USSR_P", tb1_USSRPlatn.Text);
                doc.SetFields("CNT1_MAX_CONCURS", tb1_MaxConcurs.Text);

                //Second part
                doc.SetFields("CNT2", tb2_FullCount.Text);
                doc.SetFields("CNT2_B", tb2_Budzh.Text);
                doc.SetFields("CNT2_P", tb2_Platn.Text);
                doc.SetFields("CNT2_SPB_B", tb2_SpbBudzh.Text);
                doc.SetFields("CNT2_SPB_P", tb2_SpbPlatn.Text);
                doc.SetFields("CNT2_MALE_B", tb2_MaleBudzh.Text);
                doc.SetFields("CNT2_MALE_P", tb2_MalePlatn.Text);
                doc.SetFields("CNT2_SCHOOL_B", tb2_SchoolBudzh.Text);
                doc.SetFields("CNT2_SCHOOL_P", tb2_SchoolPlatn.Text);
                doc.SetFields("CNT2_PROF_B", tb2_ProfBudzh.Text);
                doc.SetFields("CNT2_PROF_P", tb2_ProfPlatn.Text);
                doc.SetFields("CNT2_NPO_B", tb2_NPOBudzh.Text);
                doc.SetFields("CNT2_NPO_P", tb2_NPOPlatn.Text);
                doc.SetFields("CNT2_OLYMP_B", tb2_OlympBudzh.Text);
                doc.SetFields("CNT2_OLYMP_P", tb2_OlympPlatn.Text);
                doc.SetFields("CNT2_VK_B", tb2_VKBudzh.Text);
                doc.SetFields("CNT2_VK_P", tb2_VKPlatn.Text);
                doc.SetFields("CNT2_FOREIGN_B", tb2_ForeignBudzh.Text);
                doc.SetFields("CNT2_FOREIGN_P", tb2_ForeignPlatn.Text);
                doc.SetFields("CNT2_USSR_B", tb2_USSRBudzh.Text);
                doc.SetFields("CNT2_USSR_P", tb2_USSRPlatn.Text);
                doc.SetFields("CNT2_MAX_CONCURS", tb2_MaxConcurs.Text);

                //Third part
                doc.SetFields("CNT3", tb3_FullCount.Text);
                doc.SetFields("CNT3_B", tb3_Budzh.Text);
                doc.SetFields("CNT3_P", tb3_Platn.Text);
                doc.SetFields("CNT3_SPB_B", tb3_SpbBudzh.Text);
                doc.SetFields("CNT3_SPB_P", tb3_SpbPlatn.Text);
                doc.SetFields("CNT3_MALE_B", tb3_MaleBudzh.Text);
                doc.SetFields("CNT3_MALE_P", tb3_MalePlatn.Text);
                doc.SetFields("CNT3_SCHOOL_B", tb3_SchoolBudzh.Text);
                doc.SetFields("CNT3_SCHOOL_P", tb3_SchoolPlatn.Text);
                doc.SetFields("CNT3_PROF_B", tb3_ProfBudzh.Text);
                doc.SetFields("CNT3_PROF_P", tb3_ProfPlatn.Text);
                doc.SetFields("CNT3_NPO_B", tb3_NPOBudzh.Text);
                doc.SetFields("CNT3_NPO_P", tb3_NPOPlatn.Text);
                doc.SetFields("CNT3_OLYMP_B", tb3_OlympBudzh.Text);
                doc.SetFields("CNT3_OLYMP_P", tb3_OlympPlatn.Text);
                doc.SetFields("CNT3_VK_B", tb3_VKBudzh.Text);
                doc.SetFields("CNT3_VK_P", tb3_VKPlatn.Text);
                doc.SetFields("CNT3_FOREIGN_B", tb3_ForeignBudzh.Text);
                doc.SetFields("CNT3_FOREIGN_P", tb3_ForeignPlatn.Text);
                doc.SetFields("CNT3_USSR_B", tb3_USSRBudzh.Text);
                doc.SetFields("CNT3_USSR_P", tb3_USSRPlatn.Text);
                doc.SetFields("CNT3_MAX_CONCURS", tb3_MaxConcurs.Text);
            }
            catch (WordException we)
            {
                WinFormsServ.Error(we);
            }
            catch (Exception exc)
            {
                WinFormsServ.Error(exc);
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
