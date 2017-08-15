using EducServLib;
using Novacode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
                int iKCP_1k = context.extEntry.Where(x =>x.StudyLevelGroupId == 1 && x.StudyBasisId == 1 && x.StudyFormId == 1 && !x.IsForeign)
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
                    .Select(x => new { x.Entry.StudyBasisId, x.Id }).Distinct();
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
                                       Ab.Id
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
                                Ab.Id
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
                                    Ab.Id
                                }).Distinct();
                tbCnt_Stud_1K_Male_B_SPB.Text = Male_SPB.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_Male_P_SPB.Text = Male_SPB.Where(x => x.StudyBasisId == 2).Count().ToString();

                //имеющих среднее (полное) общее образование, всего
                var School = (from Ab in context.Abiturient
                              join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                              join Ent in context.Entry on Ab.EntryId equals Ent.Id
                              join Pers in context.Person on Ab.PersonId equals Pers.Id
                              join PersEduc in context.extPerson_EducationInfo_Current on Pers.Id equals PersEduc.PersonId
                              where Ab.Entry.StudyFormId == 1
                              && Ab.Entry.StudyLevel.LevelGroupId == 1
                              && PersEduc.SchoolTypeId == 1
                              && !Ent.IsForeign 
                              select new
                              {
                                  Ent.StudyBasisId,
                                  Ab.Id
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
                                  join PersEduc in context.extPerson_EducationInfo_Current on Pers.Id equals PersEduc.PersonId
                                  where Ab.Entry.StudyFormId == 1
                                  && Ab.Entry.StudyLevel.LevelGroupId == 1
                                  && PersEduc.SchoolTypeId == 1
                                  && !Ent.IsForeign
                                  && Pers.Person_Contacts.RegionId == 1
                                  select new
                                  {
                                      Ent.StudyBasisId,
                                      Ab.Id
                                  }).Distinct();

                tbCnt_Stud_1K_School_B_SPB.Text = School_SPB.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_School_P_SPB.Text = School_SPB.Where(x => x.StudyBasisId == 2).Count().ToString();

                //имеющих среднее профессиональное образование, всего
                var Prof = (from Ab in context.Abiturient
                            join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                            join Ent in context.Entry on Ab.EntryId equals Ent.Id
                            join Pers in context.Person on Ab.PersonId equals Pers.Id
                            join PersEduc in context.extPerson_EducationInfo_Current on Pers.Id equals PersEduc.PersonId
                            where Ab.Entry.StudyFormId == 1
                            && Ab.Entry.StudyLevel.LevelGroupId == 1
                            && PersEduc.SchoolTypeId != 1 && PersEduc.SchoolTypeId != 4
                            && !Ent.IsForeign
                            select new
                            {
                                Ent.StudyBasisId,
                                Ab.Id
                            }).Distinct();
                int iCntStud1K_Prof_B_Priem = Prof.Where(x => x.StudyBasisId == 1).Count();
                int iCntStud1K_Prof_P_Priem = Prof.Where(x => x.StudyBasisId == 2).Count();
                tbCnt_Stud_1K_Prof_B.Text = iCntStud1K_Prof_B_Priem.ToString();
                tbCnt_Stud_1K_Prof_P.Text = iCntStud1K_Prof_P_Priem.ToString();

                //имеющих среднее профессиональное образование, СПб
                var Prof_SPB = (from Ab in context.Abiturient
                                join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                                join Ent in context.Entry on Ab.EntryId equals Ent.Id
                                join Pers in context.Person on Ab.PersonId equals Pers.Id
                                join PersEduc in context.extPerson_EducationInfo_Current on Pers.Id equals PersEduc.PersonId
                                where Ab.Entry.StudyFormId == 1
                                && Ab.Entry.StudyLevel.LevelGroupId == 1
                                && PersEduc.SchoolTypeId != 1 && PersEduc.SchoolTypeId != 4
                                && !Ent.IsForeign
                                && Pers.Person_Contacts.RegionId == 1
                                select new
                                {
                                    Ent.StudyBasisId,
                                    Ab.Id
                                }).Distinct();
                tbCnt_Stud_1K_Prof_B_SPB.Text = Prof_SPB.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_Prof_P_SPB.Text = Prof_SPB.Where(x => x.StudyBasisId == 2).Count().ToString();

                //имеющих высшее профессиональное образование, всего
                var NPOs = (from Ab in context.Abiturient
                            join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                            join Ent in context.Entry on Ab.EntryId equals Ent.Id
                            join Pers in context.Person on Ab.PersonId equals Pers.Id
                            join PersEduc in context.extPerson_EducationInfo_Current on Pers.Id equals PersEduc.PersonId
                            where Ab.Entry.StudyFormId == 1
                            && Ab.Entry.StudyLevel.LevelGroupId == 1
                            && PersEduc.SchoolTypeId == 4
                            && !Ent.IsForeign
                            select new
                            {
                                Ent.StudyBasisId,
                                Ab.Id
                            }).Distinct();
                tbCnt_Stud_1K_NPO_B.Text = NPOs.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_NPO_P.Text = NPOs.Where(x => x.StudyBasisId == 2).Count().ToString();

                //имеющих высшее профессиональное образование, СПб
                var NPO_SPB = (from Ab in context.Abiturient
                               join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                               join Ent in context.Entry on Ab.EntryId equals Ent.Id
                               join Pers in context.Person on Ab.PersonId equals Pers.Id
                               join PersEduc in context.extPerson_EducationInfo_Current on Pers.Id equals PersEduc.PersonId
                               where Ab.Entry.StudyFormId == 1
                               && Ab.Entry.StudyLevel.LevelGroupId == 1
                               && PersEduc.SchoolTypeId == 4
                               && !Ent.IsForeign
                               && Pers.Person_Contacts.RegionId == 1
                               select new
                               {
                                   Ent.StudyBasisId,
                                   Ab.Id
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
                          && Ent.IsForeign == false
                          && extEntView.IsQuota == true
                          select new
                          {
                              Ent.StudyBasisId,
                              Ab.Id
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
                              && Pers.Person_Contacts.RegionId == 1
                              && Ent.IsForeign == false
                              && extEntView.IsQuota == true
                              select new
                              {
                                  Ent.StudyBasisId,
                                  Ab.Id
                              }).Distinct();
                tbCnt_Stud_1K_VK_B_SPB.Text = VK_SPB.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_VK_P_SPB.Text = VK_SPB.Where(x => x.StudyBasisId == 2).Count().ToString();

                //зачисленных олимпиадников, всего
                var Olymp = (from Ab in context.Abiturient
                             join Ol in context.Olympiads on Ab.OlympiadId equals Ol.Id
                             join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                             join Ent in context.Entry on Ab.EntryId equals Ent.Id
                             join Pers in context.Person on Ab.PersonId equals Pers.Id
                             join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                             where Ab.Entry.StudyFormId == 1
                             && Ab.Entry.StudyLevel.LevelGroupId == 1
                             && !Ent.IsForeign
                             && Ol.OlympValueId > 4 && (Ol.OlympTypeId == 3 || Ol.OlympTypeId == 4)
                             && extEntView.IsBE == true
                             select new
                             {
                                 Ent.StudyBasisId,
                                 Ab.Id
                             }).Distinct();
                tbCnt_Stud_1K_Olymp_B.Text = Olymp.Where(x => x.StudyBasisId == 1).Count().ToString();
                tbCnt_Stud_1K_Olymp_P.Text = Olymp.Where(x => x.StudyBasisId == 2).Count().ToString();

                //зачисленных олимпиадников, СПб
                var Olymp_SPB = (from Ab in context.Abiturient
                                 join Ol in context.Olympiads on Ab.OlympiadId equals Ol.Id
                                 join extEntView in context.extEntryView on Ab.Id equals extEntView.AbiturientId
                                 join Ent in context.Entry on Ab.EntryId equals Ent.Id
                                 join Pers in context.Person on Ab.PersonId equals Pers.Id
                                 join PersEduc in context.Person_EducationInfo on Pers.Id equals PersEduc.PersonId
                                 where Ab.Entry.StudyFormId == 1
                                 && Ab.Entry.StudyLevel.LevelGroupId == 1
                                 && !Ent.IsForeign
                                 && Ol.OlympValueId > 4 && (Ol.OlympTypeId == 3 || Ol.OlympTypeId == 4)
                                 && Pers.Person_Contacts.RegionId == 1
                                 && extEntView.IsBE == true
                                 select new
                                 {
                                     Ent.StudyBasisId,
                                     Ab.Id
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
                                      Ab.Id
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
                                Ab.Id
                            }).Distinct();
                int iUSSR1K_B = USSR.Where(x => x.StudyBasisId == 1).Count();
                int iUSSR1K_P = USSR.Where(x => x.StudyBasisId == 2).Count();

                tbCnt_Stud_1K_USSR_B.Text = iUSSR1K_B.ToString();
                tbCnt_Stud_1K_USSR_P.Text = iUSSR1K_P.ToString();

                var balls =
                    (from mrk in context.Mark
                     join ExInEnt in context.ExamInEntryBlockUnit on mrk.ExamInEntryBlockUnitId equals ExInEnt.Id
                     join extEv in context.extEntryView on mrk.AbiturientId equals extEv.AbiturientId
                     where extEv.StudyLevelGroupId == 1 && extEv.StudyFormId == 1
                     select new { mrk.Value, extEv.StudyBasisId }).ToList();

                //Средний балл ЕГЭ
                tbAVG_Ege_B.Text = Math.Round(balls.Where(x => x.StudyBasisId == 1).Select(x => x.Value).DefaultIfEmpty(0m).Average(), 3).ToString();
                tbAVG_Ege_P.Text = Math.Round(balls.Where(x => x.StudyBasisId == 2).Select(x => x.Value).DefaultIfEmpty(0m).Average(), 3).ToString();

                //КЦ Магистратура
                string query = "SELECT SUM(KCP) AS CNT FROM ed.qEntry WHERE StudyLevelGroupId=2 AND StudyFormId='1' AND StudyBasisId='1' AND IsCrimea = 0 AND IsForeign = 0";
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
                string sFileName = Path.Combine(MainClass.dirTemplates, "Form2.docx");
                if (!File.Exists(sFileName))
                {
                    WinFormsServ.Error("Файл шаблона не найден");
                    return;
                }

                using (FileStream fs = new FileStream(sFileName, FileMode.Open, FileAccess.Read))
                using (DocX doc = DocX.Load(fs))
                {
                    doc.ReplaceText("&KCP 1KURS&", tbKCP_1kurs.Text);

                    doc.ReplaceText("&CNT ABIT 1K B&", tbCnt_Abit_1K_B.Text);
                    doc.ReplaceText("&CNT ABIT 1K P&", tbCnt_Abit_1K_P.Text);
                    doc.ReplaceText("&CNT ABIT 1K B SPB&", tbCnt_Abit_1K_B_SPB.Text);
                    doc.ReplaceText("&CNT ABIT 1K P SPB&", tbCnt_Abit_1K_P_SPB.Text);

                    doc.ReplaceText("&CNT STUD 1K ALL&", tbCnt_Stud_1K_All.Text);
                    doc.ReplaceText("&CNT STUD 1K ALL SPB&", tbCnt_Stud_1K_All_SPB.Text);

                    doc.ReplaceText("&CNT STUD 1K B&", tbCnt_Stud_1K_B.Text);
                    doc.ReplaceText("&CNT STUD 1K P&", tbCnt_Stud_1K_P.Text);
                    doc.ReplaceText("&CNT STUD 1K B SPB&", tbCnt_Stud_1K_B_SPB.Text);
                    doc.ReplaceText("&CNT STUD 1K P SPB&", tbCnt_Stud_1K_P_SPB.Text);

                    doc.ReplaceText("&CNT STUD 1K MALE B&", tbCnt_Stud_1K_Male_B.Text);
                    doc.ReplaceText("&CNT STUD 1K MALE P&", tbCnt_Stud_1K_Male_P.Text);
                    doc.ReplaceText("&CNT STUD 1K MALE B SPB&", tbCnt_Stud_1K_Male_B_SPB.Text);
                    doc.ReplaceText("&CNT STUD 1K MALE P SPB&", tbCnt_Stud_1K_Male_P_SPB.Text);

                    doc.ReplaceText("&CNT STUD 1K SCHOOL B&", tbCnt_Stud_1K_School_B.Text);
                    doc.ReplaceText("&CNT STUD 1K SCHOOL P&", tbCnt_Stud_1K_School_P.Text);
                    doc.ReplaceText("&CNT STUD 1K SCHOOL B SPB&", tbCnt_Stud_1K_School_B_SPB.Text);
                    doc.ReplaceText("&CNT STUD 1K SCHOOL P SPB&", tbCnt_Stud_1K_School_P_SPB.Text);

                    doc.ReplaceText("&CNT STUD 1K PROF B&", tbCnt_Stud_1K_Prof_B.Text);
                    doc.ReplaceText("&CNT STUD 1K PROF P&", tbCnt_Stud_1K_Prof_P.Text);
                    doc.ReplaceText("&CNT STUD 1K PROF B SPB&", tbCnt_Stud_1K_Prof_B_SPB.Text);
                    doc.ReplaceText("&CNT STUD 1K PROF P SPB&", tbCnt_Stud_1K_Prof_P_SPB.Text);

                    doc.ReplaceText("&CNT STUD 1K NPO B&", tbCnt_Stud_1K_NPO_B.Text);
                    doc.ReplaceText("&CNT STUD 1K NPO P&", tbCnt_Stud_1K_NPO_P.Text);
                    doc.ReplaceText("&CNT STUD 1K NPO B SPB&", tbCnt_Stud_1K_NPO_B_SPB.Text);
                    doc.ReplaceText("&CNT STUD 1K NPO P SPB&", tbCnt_Stud_1K_NPO_P_SPB.Text);

                    doc.ReplaceText("&CNT STUD 1K VK B&", tbCnt_Stud_1K_VK_B.Text);
                    doc.ReplaceText("&CNT STUD 1K VK P&", tbCnt_Stud_1K_VK_P.Text);
                    doc.ReplaceText("&CNT STUD 1K VK B SPB&", tbCnt_Stud_1K_VK_B_SPB.Text);
                    doc.ReplaceText("&CNT STUD 1K VK P SPB&", tbCnt_Stud_1K_VK_P_SPB.Text);

                    doc.ReplaceText("&CNT STUD 1K OLYMP B&", tbCnt_Stud_1K_Olymp_B.Text);
                    doc.ReplaceText("&CNT STUD 1K OLYMP P&", tbCnt_Stud_1K_Olymp_P.Text);
                    doc.ReplaceText("&CNT STUD 1K OLYMP B SPB&", tbCnt_Stud_1K_Olymp_B_SPB.Text);
                    doc.ReplaceText("&CNT STUD 1K OLYMP P SPB&", tbCnt_Stud_1K_Olymp_P_SPB.Text);

                    doc.ReplaceText("&CNT STUD 1K FOREIGN B&", tbCnt_Stud_1K_Foreign_B.Text);
                    doc.ReplaceText("&CNT STUD 1K FOREIGN P&", tbCnt_Stud_1K_Foreign_P.Text);

                    doc.ReplaceText("&CNT STUD 1K USSR B&", tbCnt_Stud_1K_USSR_B.Text);
                    doc.ReplaceText("&CNT STUD 1K USSR P&", tbCnt_Stud_1K_USSR_P.Text);

                    doc.ReplaceText("&AVG EGE B&", tbAVG_Ege_B.Text);
                    doc.ReplaceText("&AVG EGE P&", tbAVG_Ege_P.Text);

                    doc.ReplaceText("&KCP MAG&", tbKCP_Mag.Text);
                    doc.ReplaceText("&CNT STUD MAG&", tbCnt_Stud_MAG_All.Text);
                    doc.ReplaceText("&CNT STUD MAG SPB&", tbCnt_Stud_MAG_All_SPB.Text);

                    string outFileName = Path.Combine(MainClass.saveTempFolder, "Form2" + Guid.NewGuid() + ".docx");
                    doc.SaveAs(outFileName);

                    Process p = new Process();
                    p.StartInfo.FileName = outFileName;
                    p.StartInfo.Verb = "Open";
                    p.Start();
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error(ex);
            }
        }
    }
}
