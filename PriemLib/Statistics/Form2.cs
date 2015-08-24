using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WordOut;

namespace PriemLib
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.MdiParent = MainClass.mainform;
            FillForm();
        }

        private void FillForm()
        {
            using (PriemEntities context = new PriemEntities())
            {
                string query = "SELECT SUM(KCP) AS CNT FROM ed.qEntry WHERE StudyLevelGroupId=1 AND StudyFormId='1' AND StudyBasisId='1' AND IsCrimea = 0 AND IsForeign = 0";
                int iKCP_1k = context.extEntry.Where(x => MainClass.lstStudyLevelGroupId.Contains(x.StudyLevelGroupId) && x.StudyBasisId == 1 && x.StudyFormId == 1)
                    .Select(x => x.KCP ?? 0).DefaultIfEmpty(0).Sum();
                tbKCP_1kurs.Text = iKCP_1k.ToString();

                //Подано заявлений на 1 курс, всего
                var q_AllAbits = context.Abiturient
                    .Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId) && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
                tbCnt_Abit_1K_B.Text = q_AllAbits.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Abit_1K_P.Text = q_AllAbits.Where(x => x.StudyBasisId == 2).Count().ToString();

                //Подано заявлений на 1 курс, СПб
                var q_AllAbits_SPb = context.Abiturient.Where(x => x.Entry.StudyFormId == 1 && MainClass.lstStudyLevelGroupId.Contains(x.Entry.StudyLevel.LevelGroupId)
                    && x.Person.Person_Contacts.RegionId == 1 && !x.Entry.IsForeign)
                    .Select(x => new { x.Entry.StudyBasisId, x.PersonId }).Distinct();
                tbCnt_Abit_1K_B_SPB.Text = q_AllAbits_SPb.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Abit_1K_P_SPB.Text = q_AllAbits_SPb.Where(x => x.StudyBasisId == 2).Count().ToString();

                //зачислено на 1 курс, всего
                var EntView = context.extEntryView
                    .Where(x => x.StudyFormId == 1 && x.StudyLevelGroupId == 1 && !x.IsForeign)
                    .Select(x => new { x.StudyBasisId, x.AbiturientId }).Distinct();
                int iCntStud1K_B = EntView.Where(x => x.StudyBasisId == 1).Count();
                int iCntStud1K_P = EntView.Where(x => x.StudyBasisId == 2).Count();

                tbCnt_Stud_1K_All.Text = (iCntStud1K_B + iCntStud1K_P).ToString();
                tbCnt_Stud_1K_B.Text = iCntStud1K_B.ToString();
                tbCnt_Stud_1K_P.Text = iCntStud1K_P.ToString();

                //зачислено на 1 курс, СПб
                var EntView_SPB = (from Ab in context.Abiturient
                                   join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                                   join Ent in context.Entry on Ab.EntryId equals Ent.Id
                                   join Pers in context.Person on Ab.PersonId equals Pers.Id
                                   where Ab.Entry.StudyFormId == 1
                                   && Ab.Entry.StudyLevel.LevelGroupId == 1
                                   && Pers.Person_Contacts.RegionId == 1
                                   && !Ent.IsForeign
                                   select new
                                   {
                                       Ent.StudyBasisId,
                                       Ab.PersonId
                                   }).Distinct();
                int iCntStud1K_B_SPB = EntView_SPB.Where(x => x.StudyBasisId == 1).Count();
                int iCntStud1K_P_SPB = EntView_SPB.Where(x => x.StudyBasisId == 2).Count();

                tbCnt_Stud_1K_B_SPB.Text = iCntStud1K_B_SPB.ToString();
                tbCnt_Stud_1K_P_SPB.Text = iCntStud1K_P_SPB.ToString();
                tbCnt_Stud_1K_All_SPB.Text = (iCntStud1K_B_SPB + iCntStud1K_P_SPB).ToString();

                //зачислено на 1 курс мужчин, всего
                var Male = (from Ab in context.Abiturient
                            join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                            join Ent in context.Entry on Ab.EntryId equals Ent.Id
                            join Pers in context.Person on Ab.PersonId equals Pers.Id
                            where Ab.Entry.StudyFormId == 1
                            && Ab.Entry.StudyLevel.LevelGroupId == 1
                            && Pers.Sex && !Ent.IsForeign
                            select new
                            {
                                Ent.StudyBasisId,
                                Ab.PersonId
                            }).Distinct();
                tbCnt_Stud_1K_Male_B.Text = Male.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_Male_P.Text = Male.Where(x => x.StudyBasisId == 2).Count().ToString();

                //зачислено на 1 курс мужчин, СПб
                var Male_SPB = (from Ab in context.Abiturient
                                join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                                join Ent in context.Entry on Ab.EntryId equals Ent.Id
                                join Pers in context.Person on Ab.PersonId equals Pers.Id
                                where Ab.Entry.StudyFormId == 1
                                && Ab.Entry.StudyLevel.LevelGroupId == 1
                                && Pers.Person_Contacts.RegionId == 1
                                && !Ent.IsForeign 
                                && Pers.Sex
                                select new
                                {
                                    Ent.StudyBasisId,
                                    Ab.PersonId
                                }).Distinct();
                tbCnt_Stud_1K_Male_B_SPB.Text = Male_SPB.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_Male_P_SPB.Text = Male_SPB.Where(x => x.StudyBasisId == 2).Count().ToString();

                //имеющих среднее (полное) общее образование, всего
                var School = (from Ab in context.Abiturient
                              join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                              join Ent in context.Entry on Ab.EntryId equals Ent.Id
                              join Pers in context.Person on Ab.PersonId equals Pers.Id
                              join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                              where Ab.Entry.StudyFormId == 1
                              && Ab.Entry.StudyLevel.LevelGroupId == 1
                              && PersEduc.SchoolTypeId == 1
                              && !Ent.IsForeign 
                              select new
                              {
                                  Ent.StudyBasisId,
                                  Ab.PersonId
                              }).Distinct();
                int iCntStud1K_School_B_Priem = School.Where(x => x.StudyBasisId == 1).Count();
                int iCntStud1K_School_P_Priem = School.Where(x => x.StudyBasisId == 2).Count();
                tbCnt_Stud_1K_School_B.Text = iCntStud1K_School_B_Priem.ToString();
                tbCnt_Stud_1K_School_P.Text = iCntStud1K_School_P_Priem.ToString();

                //имеющих среднее (полное) общее образование, СПб
                var School_SPB = (from Ab in context.Abiturient
                                  join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                                  join Ent in context.Entry on Ab.EntryId equals Ent.Id
                                  join Pers in context.Person on Ab.PersonId equals Pers.Id
                                  join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                                  where Ab.Entry.StudyFormId == 1
                                  && Ab.Entry.StudyLevel.LevelGroupId == 1
                                  && PersEduc.SchoolTypeId == 1
                                  && !Ent.IsForeign
                                  && Pers.Person_Contacts.RegionId == 1
                                  select new
                                  {
                                      Ent.StudyBasisId,
                                      Ab.PersonId
                                  }).Distinct();

                tbCnt_Stud_1K_School_B_SPB.Text = School_SPB.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_School_P_SPB.Text = School_SPB.Where(x => x.StudyBasisId == 2).Count().ToString();

                //имеющих среднее и высшее профессиональное образование, всего
                var Prof = (from Ab in context.Abiturient
                            join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                            join Ent in context.Entry on Ab.EntryId equals Ent.Id
                            join Pers in context.Person on Ab.PersonId equals Pers.Id
                            join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                            where Ab.Entry.StudyFormId == 1
                            && Ab.Entry.StudyLevel.LevelGroupId == 1
                            && PersEduc.SchoolTypeId != 1 && PersEduc.SchoolTypeId != 3
                            && !Ent.IsForeign
                            select new
                            {
                                Ent.StudyBasisId,
                                Ab.PersonId
                            }).Distinct();
                int iCntStud1K_Prof_B_Priem = Prof.Where(x => x.StudyBasisId == 1).Count();
                int iCntStud1K_Prof_P_Priem = Prof.Where(x => x.StudyBasisId == 2).Count();
                tbCnt_Stud_1K_Prof_B.Text = iCntStud1K_Prof_B_Priem.ToString();
                tbCnt_Stud_1K_Prof_P.Text = iCntStud1K_Prof_P_Priem.ToString();

                //имеющих среднее и высшее профессиональное образование, СПб
                var Prof_SPB = (from Ab in context.Abiturient
                                join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                                join Ent in context.Entry on Ab.EntryId equals Ent.Id
                                join Pers in context.Person on Ab.PersonId equals Pers.Id
                                join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                                where Ab.Entry.StudyFormId == 1
                                && Ab.Entry.StudyLevel.LevelGroupId == 1
                                && PersEduc.SchoolTypeId != 1 && PersEduc.SchoolTypeId != 3
                                && !Ent.IsForeign
                                && Pers.Person_Contacts.RegionId == 1
                                select new
                                {
                                    Ent.StudyBasisId,
                                    Ab.PersonId
                                }).Distinct();
                tbCnt_Stud_1K_Prof_B_SPB.Text = Prof_SPB.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_Prof_P_SPB.Text = Prof_SPB.Where(x => x.StudyBasisId == 2).Count().ToString();

                //имеющих НПО, всего
                var NPOs = (from Ab in context.Abiturient
                            join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                            join Ent in context.Entry on Ab.EntryId equals Ent.Id
                            join Pers in context.Person on Ab.PersonId equals Pers.Id
                            join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                            where Ab.Entry.StudyFormId == 1
                            && Ab.Entry.StudyLevel.LevelGroupId == 1
                            && PersEduc.SchoolTypeId == 3
                            && !Ent.IsForeign
                            select new
                            {
                                Ent.StudyBasisId,
                                Ab.PersonId
                            }).Distinct();
                tbCnt_Stud_1K_NPO_B.Text = NPOs.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_NPO_P.Text = NPOs.Where(x => x.StudyBasisId == 2).Count().ToString();

                //имеющих НПО, СПб
                var NPO_SPB = (from Ab in context.Abiturient
                               join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                               join Ent in context.Entry on Ab.EntryId equals Ent.Id
                               join Pers in context.Person on Ab.PersonId equals Pers.Id
                               join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                               where Ab.Entry.StudyFormId == 1
                               && Ab.Entry.StudyLevel.LevelGroupId == 1
                               && PersEduc.SchoolTypeId == 3
                               && !Ent.IsForeign
                               && Pers.Person_Contacts.RegionId == 1
                               select new
                               {
                                   Ent.StudyBasisId,
                                   Ab.PersonId
                               }).Distinct();
                tbCnt_Stud_1K_NPO_B_SPB.Text = NPO_SPB.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_NPO_P_SPB.Text = NPO_SPB.Where(x => x.StudyBasisId == 2).Count().ToString();

                //зачисленных в/к, всего
                var VK = (from Ab in context.Abiturient
                          join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                          join Ent in context.Entry on Ab.EntryId equals Ent.Id
                          join Pers in context.Person on Ab.PersonId equals Pers.Id
                          join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                          where Ab.Entry.StudyFormId == 1
                          && Ab.Entry.StudyLevel.LevelGroupId == 1
                          && !Ent.IsForeign
                          && extEntView.IsQuota
                          select new
                          {
                              Ent.StudyBasisId,
                              Ab.PersonId
                          }).Distinct();
                tbCnt_Stud_1K_VK_B.Text = VK.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_VK_P.Text = VK.Where(x => x.StudyBasisId == 2).Count().ToString();

                //зачисленных в/к, СПб
                var VK_SPB = (from Ab in context.Abiturient
                              join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                              join Ent in context.Entry on Ab.EntryId equals Ent.Id
                              join Pers in context.Person on Ab.PersonId equals Pers.Id
                              join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                              where Ab.Entry.StudyFormId == 1
                              && Ab.Entry.StudyLevel.LevelGroupId == 1
                              && !Ent.IsForeign
                              && Pers.Person_Contacts.RegionId == 1
                              && extEntView.IsQuota
                              select new
                              {
                                  Ent.StudyBasisId,
                                  Ab.PersonId
                              }).Distinct();
                tbCnt_Stud_1K_VK_B_SPB.Text = VK_SPB.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_VK_P_SPB.Text = VK_SPB.Where(x => x.StudyBasisId == 2).Count().ToString();

                //зачисленных олимпиадников, всего
                var Olymp = (from Ab in context.Abiturient
                             join Ol in context.Olympiads on Ab.Id equals Ol.AbiturientId
                             join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                             join Ent in context.Entry on Ab.EntryId equals Ent.Id
                             join Pers in context.Person on Ab.PersonId equals Pers.Id
                             join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                             where Ab.Entry.StudyFormId == 1
                             && Ab.Entry.StudyLevel.LevelGroupId == 1
                             && !Ent.IsForeign
                             && Ol.OlympValueId > 4 && (Ol.OlympTypeId == 3 || Ol.OlympTypeId == 4)
                             select new
                             {
                                 Ent.StudyBasisId,
                                 Ab.PersonId
                             }).Distinct();
                tbCnt_Stud_1K_Olymp_B.Text = Olymp.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_Olymp_P.Text = Olymp.Where(x => x.StudyBasisId == 2).Count().ToString();

                //зачисленных олимпиадников, СПб
                var Olymp_SPB = (from Ab in context.Abiturient
                                 join Ol in context.Olympiads on Ab.Id equals Ol.AbiturientId
                                 join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                                 join Ent in context.Entry on Ab.EntryId equals Ent.Id
                                 join Pers in context.Person on Ab.PersonId equals Pers.Id
                                 join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                                 where Ab.Entry.StudyFormId == 1
                                 && Ab.Entry.StudyLevel.LevelGroupId == 1
                                 && !Ent.IsForeign
                                 && Ol.OlympValueId > 4 && (Ol.OlympTypeId == 3 || Ol.OlympTypeId == 4)
                                 && Pers.Person_Contacts.RegionId == 1
                                 select new
                                 {
                                     Ent.StudyBasisId,
                                     Ab.PersonId
                                 }).Distinct();
                tbCnt_Stud_1K_Olymp_B_SPB.Text = Olymp_SPB.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_Olymp_P_SPB.Text = Olymp_SPB.Where(x => x.StudyBasisId == 2).Count().ToString();

                //иностранцев
                var Foreigners = (from Ab in context.Abiturient
                                  join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                                  join Ent in context.Entry on Ab.EntryId equals Ent.Id
                                  join Pers in context.Person on Ab.PersonId equals Pers.Id
                                  join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                                  where Ab.Entry.StudyFormId == 1
                                  && Ab.Entry.StudyLevel.LevelGroupId == 1
                                  && !Ent.IsForeign
                                  && Pers.Person_Contacts.CountryId != 1
                                  select new
                                  {
                                      Ent.StudyBasisId,
                                      Ab.PersonId
                                  }).Distinct();
                int iForeigners1K_B = Foreigners.Where(x => x.StudyBasisId == 1).Count();
                int iForeigners1K_P = Foreigners.Where(x => x.StudyBasisId == 2).Count();

                tbCnt_Stud_1K_Foreign_B.Text = iForeigners1K_B.ToString();
                tbCnt_Stud_1K_Foreign_P.Text = iForeigners1K_P.ToString();

                //CCCP
                var USSR = (from Ab in context.Abiturient
                            join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                            join Ent in context.Entry on Ab.EntryId equals Ent.Id
                            join Pers in context.Person on Ab.PersonId equals Pers.Id
                            join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                            where Ab.Entry.StudyFormId == 1
                            && Ab.Entry.StudyLevel.LevelGroupId == 1
                            && !Ent.IsForeign
                            && Pers.Person_Contacts.CountryId != 1 && Pers.Person_Contacts.CountryId != 6
                            select new
                            {
                                Ent.StudyBasisId,
                                Ab.PersonId
                            }).Distinct();
                int iUSSR1K_B = USSR.Where(x => x.StudyBasisId == 1).Count();
                int iUSSR1K_P = USSR.Where(x => x.StudyBasisId == 2).Count();

                tbCnt_Stud_1K_USSR_B.Text = iUSSR1K_B.ToString();
                tbCnt_Stud_1K_USSR_P.Text = iUSSR1K_P.ToString();

                var balls =
                    (from mrk in context.Mark
                     join ExInEnt in context.ExamInEntry on mrk.ExamInEntryId equals ExInEnt.Id
                     join extEv in context.extEntryView on mrk.AbiturientId equals extEv.AbiturientId
                     where extEv.StudyLevelGroupId == 1 && extEv.StudyFormId == 1
                     select new { mrk.Value, extEv.StudyBasisId }).ToList();

                //Средний балл ЕГЭ
                tbAVG_Ege_B.Text = balls.Where(x => x.StudyBasisId == 1).Select(x => x.Value).DefaultIfEmpty(0m).Average().ToString();
                tbAVG_Ege_P.Text = balls.Where(x => x.StudyBasisId == 2).Select(x => x.Value).DefaultIfEmpty(0m).Average().ToString();

                //КЦ Магистратура
                query = "SELECT SUM(KCP) AS CNT FROM ed.qEntry WHERE StudyLevelGroupId=2 AND StudyFormId='1' AND StudyBasisId='1' AND IsCrimea = 0 AND IsForeign = 0";
                int iKCP_Mag = (int)MainClass.Bdc.GetValue(query);
                tbKCP_Mag.Text = iKCP_Mag.ToString();

                //Зачислено бюджет магистратура
                query = @"SELECT COUNT(extAbit.Id) 
FROM ed.extAbit 
INNER JOIN ed.extEntryView ON extEntryView.AbiturientId=extAbit.Id 
WHERE extAbit.StudyLevelGroupId=2 AND extAbit.StudyFormId=1 AND extAbit.StudyBasisId=1";
                int iCNT_Mag = (int)MainClass.Bdc.GetValue(query);
                tbCnt_Stud_MAG_All.Text = iCNT_Mag.ToString();

                //Зачислено бюджет магистратура СПб
                query = @"SELECT COUNT(extAbit.Id) 
FROM ed.extAbit 
INNER JOIN ed.extEntryView ON extEntryView.AbiturientId=extAbit.Id
INNER JOIN ed.extPerson ON extPerson.Id=extAbit.PersonId
WHERE extAbit.StudyLevelGroupId=2 AND extAbit.StudyFormId=1 AND extAbit.StudyBasisId=1 AND extPerson.RegionId = 1";
                int iCNT_Mag_SPB = (int)MainClass.Bdc.GetValue(query);
                tbCnt_Stud_MAG_All_SPB.Text = iCNT_Mag_SPB.ToString();
            }
        }

        private void btnWord_Click(object sender, EventArgs e)
        {
            try
            {
                WordDoc doc = new WordDoc(MainClass.dirTemplates + "\\Form2.dot", true);
                doc.SetFields("KCP_1KURS", tbKCP_1kurs.Text);

                doc.SetFields("CNT_ABIT_1K_B", tbCnt_Abit_1K_B.Text);
                doc.SetFields("CNT_ABIT_1K_P", tbCnt_Abit_1K_P.Text);
                doc.SetFields("CNT_ABIT_1K_B_SPB", tbCnt_Abit_1K_B_SPB.Text);
                doc.SetFields("CNT_ABIT_1K_P_SPB", tbCnt_Abit_1K_P_SPB.Text);

                doc.SetFields("CNT_STUD_1K_ALL", tbCnt_Stud_1K_All.Text);
                doc.SetFields("CNT_STUD_1K_ALL_SPB", tbCnt_Stud_1K_All_SPB.Text);

                doc.SetFields("CNT_STUD_1K_B", tbCnt_Stud_1K_B.Text);
                doc.SetFields("CNT_STUD_1K_P", tbCnt_Stud_1K_P.Text);

                doc.SetFields("CNT_STUD_1K_B_SPB", tbCnt_Stud_1K_B_SPB.Text);
                doc.SetFields("CNT_STUD_1K_P_SPB", tbCnt_Stud_1K_P_SPB.Text);

                doc.SetFields("CNT_STUD_1K_MALE_B", tbCnt_Stud_1K_Male_B.Text);
                doc.SetFields("CNT_STUD_1K_MALE_P", tbCnt_Stud_1K_Male_P.Text);
                doc.SetFields("CNT_STUD_1K_MALE_B_SPB", tbCnt_Stud_1K_Male_B_SPB.Text);
                doc.SetFields("CNT_STUD_1K_MALE_P_SPB", tbCnt_Stud_1K_Male_P_SPB.Text);

                doc.SetFields("CNT_STUD_1K_SCHOOL_B", tbCnt_Stud_1K_School_B.Text);
                doc.SetFields("CNT_STUD_1K_SCHOOL_P", tbCnt_Stud_1K_School_P.Text);
                doc.SetFields("CNT_STUD_1K_SCHOOL_B_SPB", tbCnt_Stud_1K_School_B_SPB.Text);
                doc.SetFields("CNT_STUD_1K_SCHOOL_P_SPB", tbCnt_Stud_1K_School_P_SPB.Text);

                doc.SetFields("CNT_STUD_1K_PROF_B", tbCnt_Stud_1K_Prof_B.Text);
                doc.SetFields("CNT_STUD_1K_PROF_P", tbCnt_Stud_1K_Prof_P.Text);
                doc.SetFields("CNT_STUD_1K_PROF_B_SPB", tbCnt_Stud_1K_Prof_B_SPB.Text);
                doc.SetFields("CNT_STUD_1K_PROF_P_SPB", tbCnt_Stud_1K_Prof_P_SPB.Text);

                doc.SetFields("CNT_STUD_1K_NPO_B", tbCnt_Stud_1K_NPO_B.Text);
                doc.SetFields("CNT_STUD_1K_NPO_P", tbCnt_Stud_1K_NPO_P.Text);
                doc.SetFields("CNT_STUD_1K_NPO_B_SPB", tbCnt_Stud_1K_NPO_B_SPB.Text);
                doc.SetFields("CNT_STUD_1K_NPO_P_SPB", tbCnt_Stud_1K_NPO_P_SPB.Text);

                doc.SetFields("CNT_STUD_1K_VK_B", tbCnt_Stud_1K_VK_B.Text);
                doc.SetFields("CNT_STUD_1K_VK_P", tbCnt_Stud_1K_VK_P.Text);
                doc.SetFields("CNT_STUD_1K_VK_B_SPB", tbCnt_Stud_1K_VK_B_SPB.Text);
                doc.SetFields("CNT_STUD_1K_VK_P_SPB", tbCnt_Stud_1K_VK_P_SPB.Text);

                doc.SetFields("CNT_STUD_1K_OLYMP_B", tbCnt_Stud_1K_Olymp_B.Text);
                doc.SetFields("CNT_STUD_1K_OLYMP_P", tbCnt_Stud_1K_Olymp_P.Text);
                doc.SetFields("CNT_STUD_1K_OLYMP_B_SPB", tbCnt_Stud_1K_Olymp_B_SPB.Text);
                doc.SetFields("CNT_STUD_1K_OLYMP_P_SPB", tbCnt_Stud_1K_Olymp_P_SPB.Text);

                doc.SetFields("CNT_STUD_1K_FOREIGN_B", tbCnt_Stud_1K_Foreign_B.Text);
                doc.SetFields("CNT_STUD_1K_FOREIGN_P", tbCnt_Stud_1K_Foreign_P.Text);

                doc.SetFields("CNT_STUD_1K_USSR_B", tbCnt_Stud_1K_USSR_B.Text);
                doc.SetFields("CNT_STUD_1K_USSR_P", tbCnt_Stud_1K_USSR_P.Text);

                doc.SetFields("KCP_MAG", tbKCP_Mag.Text);
                doc.SetFields("CNT_STUD_MAG", tbCnt_Stud_MAG_All.Text);
                doc.SetFields("CNT_STUD_MAG_SPB", tbCnt_Stud_MAG_All_SPB.Text);
            }
            catch
            {

            }
        }
    }
}
