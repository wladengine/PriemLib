﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EducServLib;
using BaseFormsLib;

namespace PriemLib
{
    public partial class AbitFacultyIntersectionPersons : BaseFormEx
    {
        public int FacultyId
        {
            get
            {
                return ComboServ.GetComboIdInt(cbFaculty).Value;
            }
            set
            {
                ComboServ.SetComboId(cbFaculty, value);
            }
        }
        public int? LicenseProgramId
        {
            get
            {
                if (cbLicenseProgram.Text == ComboServ.ALL_VALUE)
                    return null;
                    
                return ComboServ.GetComboIdInt(cbLicenseProgram);
            }
            set
            {
                ComboServ.SetComboId(cbLicenseProgram, value);
            }
        }
        public int? ObrazProgramId
        {
            get
            {
                if (cbObrazProgram.Text == ComboServ.ALL_VALUE)
                    return null;

                return ComboServ.GetComboIdInt(cbObrazProgram);
            }
            set
            {
                ComboServ.SetComboId(cbObrazProgram, value);
            }
        }
        public int OtherFacultyId
        {
            get
            {
                return ComboServ.GetComboIdInt(cbOtherFaculty).Value;
            }
            set
            {
                ComboServ.SetComboId(cbOtherFaculty, value);
            }
        }
        public int? OtherLicenseProgramId
        {
            get
            {
                if (cbOtherLicenseProgram.Text == ComboServ.ALL_VALUE)
                    return null;

                return ComboServ.GetComboIdInt(cbOtherLicenseProgram);
            }
            set
            {
                ComboServ.SetComboId(cbOtherLicenseProgram, value);
            }
        }
        public int? OtherObrazProgramId
        {
            get
            {
                if (cbOtherObrazProgram.Text == ComboServ.ALL_VALUE)
                    return null;

                return ComboServ.GetComboIdInt(cbOtherObrazProgram);
            }
            set
            {
                ComboServ.SetComboId(cbOtherObrazProgram, value);
            }
        }
        public int StudyFormId
        {
            get
            {
                return ComboServ.GetComboIdInt(cbStudyForm).Value;
            }
            set
            {
                ComboServ.SetComboId(cbStudyForm, value);
            }
        }
        public int OtherStudyFormId
        {
            get
            {
                return ComboServ.GetComboIdInt(cbOtherStudyForm).Value;
            }
            set
            {
                ComboServ.SetComboId(cbOtherStudyForm, value);
            }
        }
        public int StudyBasisId
        {
            get
            {
                return ComboServ.GetComboIdInt(cbStudyBasis).Value;
            }
            set
            {
                ComboServ.SetComboId(cbStudyBasis, value);
            }
        }
        public int OtherStudyBasisId
        {
            get
            {
                return ComboServ.GetComboIdInt(cbOtherStudyBasis).Value;
            }
            set
            {
                ComboServ.SetComboId(cbOtherStudyBasis, value);
            }
        }

        private DataTable tblBaseSource;

        public AbitFacultyIntersectionPersons(int iFacultyId, int iOtherFacultyId)
        {
            InitializeComponent();
            Dgv = dgv;
            this.MdiParent = MainClass.mainform;

            FillCombosFaculties();
            FillComboFaculty();
            FacultyId = iFacultyId;
            OtherFacultyId = iOtherFacultyId;
            
            FillComboLicenseProgram();
            FillComboObrazProgram();
            FillComboStudyForm();
            FillComboStudyBasis();
            FillComboOtherLicenseProgram();
            FillComboOtherObrazProgram();
            FillComboOtherStudyForm();
            FillComboOtherStudyBasis();

            FillGrid();
            InitHandlers();
        }

        private void FillCombosFaculties()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = (from x in context.SP_Faculty
                           select new
                           {
                               x.Id,
                               x.Name
                           }).ToList().Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).ToList();

                
                ComboServ.FillCombo(cbOtherFaculty, src, false, false);
            }
        }
        private void FillComboFaculty()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var bind = (from x in context.qEntry
                            where MainClass.lstStudyLevelGroupId.Contains(x.StudyLevelGroupId)
                            orderby x.FacultyId
                            select new
                            {
                                x.FacultyId,
                                x.FacultyName
                            }).Distinct().ToList().Select(x => new KeyValuePair<string, string>(x.FacultyId.ToString(), x.FacultyName)).ToList();

                ComboServ.FillCombo(cbFaculty, bind, false, false);
            }
        }
        private void FillComboLicenseProgram()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = (from x in context.Entry
                           join y in context.StudyLevel
                           on x.StudyLevelId equals y.Id
                           where MainClass.lstStudyLevelGroupId.Contains(y.LevelGroupId) && x.FacultyId == FacultyId
                           orderby x.SP_LicenseProgram.Code
                           select new
                           {
                               x.LicenseProgramId,
                               LicenseProgramCode = x.SP_LicenseProgram.Code,
                               LicenseProgramName = x.SP_LicenseProgram.Name
                           }).Distinct().ToList()
                           .Select(x => new KeyValuePair<string, string>(x.LicenseProgramId.ToString(), x.LicenseProgramCode + " " + x.LicenseProgramName)).ToList();

                ComboServ.FillCombo(cbLicenseProgram, src, false, true);
            }
        }
        private void FillComboObrazProgram()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = (from x in context.extEntry
                           where MainClass.lstStudyLevelGroupId.Contains(x.StudyLevelGroupId) && x.FacultyId == FacultyId
                           orderby x.ObrazProgramNumber
                           select new
                           {
                               x.LicenseProgramId,
                               x.ObrazProgramId,
                               x.ObrazProgramName,
                               x.ObrazProgramCrypt
                           }).Distinct();
                if (LicenseProgramId.HasValue)
                    src = src.Where(x => x.LicenseProgramId == LicenseProgramId);
                var bind = src.Distinct().ToList()
                           .Select(x => new KeyValuePair<string, string>(x.ObrazProgramId.ToString(), x.ObrazProgramCrypt + " " + x.ObrazProgramName)).ToList();

                ComboServ.FillCombo(cbObrazProgram, bind, false, true);
            }
        }
        private void FillComboStudyForm()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = (from x in context.Entry
                           join y in context.StudyLevel
                           on x.StudyLevelId equals y.Id
                           join sf in context.StudyForm
                           on x.StudyFormId equals sf.Id
                           where MainClass.lstStudyLevelGroupId.Contains(y.LevelGroupId) && x.FacultyId == FacultyId
                           orderby sf.Id
                           select new
                           {
                               x.LicenseProgramId,
                               x.ObrazProgramId,
                               sf.Id,
                               sf.Name
                           }).Distinct();
                if (LicenseProgramId.HasValue)
                    src = src.Where(x => x.LicenseProgramId == LicenseProgramId);
                if (ObrazProgramId.HasValue)
                    src = src.Where(x => x.ObrazProgramId == ObrazProgramId);
                var bind = src.Distinct().ToList()
                           .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).OrderBy(x => x.Key).Distinct().ToList();

                ComboServ.FillCombo(cbStudyForm, bind, false, false);
            }
        }
        private void FillComboStudyBasis()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = (from x in context.Entry
                           join y in context.StudyLevel
                           on x.StudyLevelId equals y.Id
                           join sb in context.StudyBasis
                           on x.StudyBasisId equals sb.Id
                           where MainClass.lstStudyLevelGroupId.Contains(y.LevelGroupId) && x.FacultyId == FacultyId && x.StudyFormId == StudyFormId
                           orderby sb.Id
                           select new
                           {
                               x.LicenseProgramId,
                               x.ObrazProgramId,
                               sb.Id,
                               sb.Name
                           }).Distinct();
                if (LicenseProgramId.HasValue)
                    src = src.Where(x => x.LicenseProgramId == LicenseProgramId);
                if (ObrazProgramId.HasValue)
                    src = src.Where(x => x.ObrazProgramId == ObrazProgramId);
                var bind = src.Distinct().ToList()
                           .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).Distinct().ToList();

                ComboServ.FillCombo(cbStudyBasis, bind, false, false);
            }
        }
        private void FillComboOtherLicenseProgram()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = (from x in context.Entry
                           join y in context.StudyLevel
                           on x.StudyLevelId equals y.Id
                           where MainClass.lstStudyLevelGroupId.Contains(y.LevelGroupId) && x.FacultyId == OtherFacultyId
                           orderby x.SP_LicenseProgram.Code
                           select new
                           {
                               x.LicenseProgramId,
                               LicenseProgramCode = x.SP_LicenseProgram.Code,
                               LicenseProgramName = x.SP_LicenseProgram.Name
                           }).Distinct().ToList()
                           .Select(x => new KeyValuePair<string, string>(x.LicenseProgramId.ToString(), x.LicenseProgramCode + " " + x.LicenseProgramName)).ToList();

                ComboServ.FillCombo(cbOtherLicenseProgram, src, false, true);
            }
        }
        private void FillComboOtherObrazProgram()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = (from x in context.extEntry
                           where MainClass.lstStudyLevelGroupId.Contains(x.StudyLevelGroupId) && x.FacultyId == OtherFacultyId
                           orderby x.ObrazProgramNumber
                           select new
                           {
                               x.LicenseProgramId,
                               x.ObrazProgramId,
                               x.ObrazProgramName,
                               x.ObrazProgramCrypt
                           }).Distinct();
                if (OtherLicenseProgramId.HasValue)
                    src = src.Where(x => x.LicenseProgramId == OtherLicenseProgramId);
                var bind = src.ToList()
                           .Select(x => new KeyValuePair<string, string>(x.ObrazProgramId.ToString(), x.ObrazProgramCrypt + " " + x.ObrazProgramName)).ToList();

                ComboServ.FillCombo(cbOtherObrazProgram, bind, false, true);
            }
        }
        private void FillComboOtherStudyForm()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = (from x in context.Entry
                           join y in context.StudyLevel
                           on x.StudyLevelId equals y.Id
                           join sf in context.StudyForm
                           on x.StudyFormId equals sf.Id
                           where MainClass.lstStudyLevelGroupId.Contains(y.LevelGroupId) && x.FacultyId == OtherFacultyId
                           orderby sf.Id
                           select new
                           {
                               x.LicenseProgramId,
                               x.ObrazProgramId,
                               sf.Id,
                               sf.Name
                           }).Distinct();
                if (OtherLicenseProgramId.HasValue)
                    src = src.Where(x => x.LicenseProgramId == OtherLicenseProgramId);
                if (OtherObrazProgramId.HasValue)
                    src = src.Where(x => x.ObrazProgramId == OtherObrazProgramId);
                var bind = src.Distinct().ToList()
                           .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).OrderBy(x => x.Key).Distinct().ToList();

                ComboServ.FillCombo(cbOtherStudyForm, bind, false, false);
            }
        }
        private void FillComboOtherStudyBasis()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = (from x in context.Entry
                           join y in context.StudyLevel
                           on x.StudyLevelId equals y.Id
                           join sb in context.StudyBasis
                           on x.StudyBasisId equals sb.Id
                           where MainClass.lstStudyLevelGroupId.Contains(y.LevelGroupId) && x.FacultyId == OtherFacultyId && x.StudyFormId == OtherStudyFormId
                           orderby sb.Id
                           select new
                           {
                               x.LicenseProgramId,
                               x.ObrazProgramId,
                               sb.Id,
                               sb.Name
                           }).Distinct();
                if (OtherLicenseProgramId.HasValue)
                    src = src.Where(x => x.LicenseProgramId == OtherLicenseProgramId);
                if (OtherObrazProgramId.HasValue)
                    src = src.Where(x => x.ObrazProgramId == OtherObrazProgramId);
                var bind = src.Distinct().ToList()
                           .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).Distinct().ToList();

                ComboServ.FillCombo(cbOtherStudyBasis, bind, false, false);
            }
        }

        private void InitHandlers()
        {
            cbFaculty.SelectedIndexChanged += new EventHandler(cbFaculty_SelectedIndexChanged);
            cbLicenseProgram.SelectedIndexChanged += new EventHandler(cbLicenseProgram_SelectedIndexChanged);
            cbObrazProgram.SelectedIndexChanged += new EventHandler(cbObrazProgram_SelectedIndexChanged);
            cbStudyForm.SelectedIndexChanged += new EventHandler(cbStudyForm_SelectedIndexChanged);
            cbStudyBasis.SelectedIndexChanged += new EventHandler(cbStudyBasis_SelectedIndexChanged);
            
            cbOtherFaculty.SelectedIndexChanged += new EventHandler(cbOtherFaculty_SelectedIndexChanged);
            cbOtherLicenseProgram.SelectedIndexChanged += new EventHandler(cbOtherLiceseProgram_SelectedIndexChanged);
            cbOtherObrazProgram.SelectedIndexChanged += new EventHandler(cbOtherObrazProgram_SelectedIndexChanged);
            cbOtherStudyForm.SelectedIndexChanged += new EventHandler(cbOtherStudyForm_SelectedIndexChanged);
            cbOtherStudyBasis.SelectedIndexChanged += new EventHandler(cbOtherStudyBasis_SelectedIndexChanged);
        }

        void cbOtherFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillComboOtherLicenseProgram();
        }
        void cbOtherLiceseProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillComboOtherObrazProgram();
        }
        void cbOtherObrazProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillComboStudyForm();
        }
        void cbStudyForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillComboStudyBasis();
        }
        void cbStudyBasis_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillGrid();
        }

        void cbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillComboLicenseProgram();
        }
        void cbLicenseProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillComboObrazProgram();
        }
        void cbObrazProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillComboOtherStudyForm();
        }
        void cbOtherStudyForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillComboOtherStudyBasis();
        }
        void cbOtherStudyBasis_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillGrid();
        }

        private void tbFIO_TextChanged(object sender, EventArgs e)
        {
            //WinFormsServ.Search(dgv, "ФИО", tbFIO.Text);
            NewWatch wc = new NewWatch(1);
            wc.Show();
            wc.SetText("Обработка, подождите...");
            GridSearch();
            wc.PerformStep();
            wc.Close();
        }

        private void GridSearch()
        {
            DataTable dtNew = new DataTable();
            foreach (DataColumn col in tblBaseSource.Columns)
                dtNew.Columns.Add(col.ColumnName, col.DataType);

            if (tbFIO.Text.Length == 0)
            {
                dgv.DataSource = tblBaseSource;
                GridColumnsSizeAndVisible();
                return;
            }

            foreach (DataRow rw in tblBaseSource.Rows)
            {
                DataRow newrow = dtNew.NewRow();
                if (rw.Field<string>("ФИО").StartsWith(tbFIO.Text, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (DataColumn col in tblBaseSource.Columns)
                        newrow[col.ColumnName] = rw[col.ColumnName];
                    dtNew.Rows.Add(newrow);
                }
            }

            dgv.DataSource = dtNew;
            GridColumnsSizeAndVisible();
        }

        private void FillGrid()
        {
            using (PriemEntities context = new PriemEntities())
            {
                NewWatch wc = new NewWatch(3);
                wc.Show();
                wc.SetText("Данные вашего факультета...");

                List<int> Competition_Vks = new List<int>() { 1, 2, 5, 7, 8 };
                var PersonsIdLst = (from qAb in context.qAbitAll
                                    where
                                       qAb.FacultyId == OtherFacultyId &&
                                       qAb.FacultyId != FacultyId &&
                                       !qAb.BackDoc &&
                                       MainClass.lstStudyLevelGroupId.Contains(qAb.StudyLevelGroupId)
                                    select qAb.PersonId).Distinct().ToList();

                var Persons = (from qAb in context.Abiturient
                               join extP in context.Person on qAb.PersonId equals extP.Id
                               join ent in context.Entry on qAb.EntryId equals ent.Id
                               where
                               PersonsIdLst.Contains(qAb.PersonId) &&
                               ent.FacultyId == FacultyId && 
                               !qAb.BackDoc &&
                               MainClass.lstStudyLevelGroupId.Contains(ent.StudyLevel.LevelGroupId) &&
                               ent.StudyFormId == StudyFormId &&
                               ent.StudyBasisId == StudyBasisId &&
                               qAb.CompetitionId != 6 &&
                               ((LicenseProgramId.HasValue) ? ent.LicenseProgramId == LicenseProgramId : true) &&
                               ((ObrazProgramId.HasValue) ? ent.ObrazProgramId == ObrazProgramId : true)
                               select new
                               {
                                   PersonId = extP.Id,
                                   qAb.Id,
                                   FIO = (extP.Surname ?? "") + " " + (extP.Name ?? "") + " " + (extP.SecondName ?? ""),
                                   ent.KCP,
                                   VK = Competition_Vks.Contains(qAb.CompetitionId),
                                   Originals = (qAb.HasOriginals),
                               }).Distinct().OrderBy(x => x.FIO).ToList();
                var AbitsIslst = Persons.Select(x => x.Id).Distinct().ToList();
                var PersonHlpStat = (from hlp in context.hlpStatRatingList
                                     where AbitsIslst.Contains(hlp.AbiturientId)
                                     select new
                                     {
                                         hlp.AbiturientId,
                                         hlp.SUM,
                                         hlp.Rank,  
                                     }).Distinct().ToList();

                #region oldcode
                //            string query = @"
                //                SELECT DISTINCT extPerson.Id, extPerson.FIO, hlpStatRatingList.SUM, hlpStatRatingList.Rank, Entry.KCP, 
                //                (case when q.CompetitionId IN (1,2,5,7,8) then 1 else 0 end) AS VKs,
                //                (CASE when Rank <= Entry.KCP then 1 else 0 end) AS GREEN, (case when q.HasOriginals='True' then 1 else 0 end) AS Orig
                //                FROM ed.qAbitAll as q
                //                INNER JOIN ed.extPerson ON extPerson.Id = q.PersonId
                //                INNER JOIN ed.Entry ON Entry.Id = q.EntryId
                //                LEFT JOIN ed.hlpStatRatingList ON hlpStatRatingList.AbiturientId=q.Id
                //                WHERE q.PersonId IN
                //                (
                //                    SELECT PersonId
                //                    FROM ed.qAbitAll
                //                    WHERE qAbitAll.FacultyId <> q.FacultyId
                //                    AND qAbitAll.FacultyId = @OtherFacultyId
                //                    AND StudyLevelGroupId = @StudyLevelGroupId
                //                    AND qAbitAll.BackDoc = 0
                //                )
                //                AND StudyLevelGroupId = @StudyLevelGroupId
                //                AND q.FacultyId=@FacultyId AND q.BackDoc = 0 AND q.StudyFormId=@StudyFormId AND q.StudyBasisId=@StudyBasisId AND q.CompetitionId<>6 ";

                //            SortedList<string, object> sl = new SortedList<string, object>();
                //            sl.Add("@StudyLevelGroupId", MainClass.lstStudyLevelGroupId.First());
                //            sl.Add("@FacultyId", FacultyId);
                //            sl.Add("@OtherFacultyId", OtherFacultyId);
                //            sl.Add("@StudyFormId", StudyFormId);
                //            sl.Add("@StudyBasisId", StudyBasisId);

                //            if (LicenseProgramId.HasValue)
                //            {
                //                query += " AND q.LicenseProgramId=@LicenseProgramId ";
                //                sl.Add("@LicenseProgramId", LicenseProgramId);
                //            }
                //            if (ObrazProgramId.HasValue)
                //            {
                //                query += " AND q.ObrazProgramId=@ObrazProgramId ";
                //                sl.Add("@ObrazProgramId", ObrazProgramId);
                //            }
                #endregion
                wc.PerformStep();
                #region oldcode
                //DataTable tbl = MainClass.Bdc.GetDataSet(query + " ORDER BY FIO ", sl).Tables[0];

                //var Persons = from DataRow rw in tbl.Rows
                //              select new
                //              {
                //                  PersonId = rw.Field<Guid>("Id"),
                //                  FIO = rw.Field<string>("FIO"),
                //                  SUM = rw.Field<decimal?>("SUM"),
                //                  Rank = rw.Field<long>("Rank"),
                //                  KCP = rw.Field<int?>("KCP") ?? 0,
                //                  Green = rw.Field<int>("GREEN") == 1 ? true : false,
                //                  Originals = rw.Field<int>("Orig") == 1 ? true : false,
                //                  VK = rw.Field<int>("VKs") == 1 ? true : false
                //              };
                #endregion

                var Abits = (from qAb in context.Abiturient
                             join ent in context.extEntry  on qAb.EntryId equals ent.Id
                             where
                             MainClass.lstStudyLevelGroupId.Contains(ent.StudyLevelGroupId) &&
                             ent.FacultyId == OtherFacultyId &&
                             ent.StudyFormId == OtherStudyFormId &&
                             ent.StudyBasisId == OtherStudyBasisId &&
                             ((OtherLicenseProgramId.HasValue) ? ent.LicenseProgramId == OtherLicenseProgramId : true) &&
                             ((OtherObrazProgramId.HasValue) ? ent.ObrazProgramId == OtherObrazProgramId : true) &&
                             !qAb.BackDoc &&
                             qAb.CompetitionId != 6 &&
                             !qAb.NotEnabled
                             select new
                             {
                                 qAb.Id,
                                 PersonId = qAb.PersonId,
                                 VK = Competition_Vks.Contains(qAb.CompetitionId),
                                 Originals = (qAb.HasOriginals),
                                 qAb.EntryId,
                                 ent.KCP,
                                 LicenseProgram = ent.LicenseProgramCode + " " + ent.LicenseProgramName,
                                 ObrazProgram = ent.ObrazProgramCrypt + " " + ent.ObrazProgramName,
                                 Profile = ent.ProfileName, 
                             }).Distinct().ToList();

                var AbIdlst = Abits.Select(x => x.Id).ToList();
                var OtherMarks = (from hlp in context.hlpStatRatingList
                                  where AbIdlst.Contains(hlp.AbiturientId)
                                  select new
                                  {
                                      hlp.AbiturientId,
                                      hlp.SUM,
                                      hlp.Rank,
                                  }).Distinct().ToList();
                #region oldcode
//                string query = @"
//                SELECT DISTINCT extPerson.Id AS PersonId, 
//                (case when qAbitAll.CompetitionId IN (1,2,5,7,8) then 1 else 0 end) AS VKs,
//                (case when qAbitAll.HasOriginals='True' then 1 else 0 end) AS Orig,
//                qAbitAll.LicenseProgramCode + ' ' + qAbitAll.LicenseProgramName AS LP,
//                qAbitAll.ObrazProgramCrypt + ' ' + qAbitAll.ObrazProgramName AS OP,
//                qAbitAll.ProfileName,
//                hlpStatRatingList.SUM, hlpStatRatingList.Rank, Entry.KCP, (CASE when Rank <= Entry.KCP then 1 else 0 end) AS GREEN
//                FROM ed.qAbitAll
//                INNER JOIN ed.Entry ON Entry.Id = qAbitAll.EntryId
//                INNER JOIN ed.extPerson ON extPerson.Id=qAbitAll.PersonId
//                LEFT JOIN ed.hlpStatRatingList ON hlpStatRatingList.AbiturientId=qAbitAll.Id
//                WHERE qAbitAll.FacultyId=@OtherFacultyId
//                AND StudyLevelGroupId=@StudyLevelGroupId AND qAbitAll.CompetitionId<>6 AND qAbitAll.NotEnabled<>'True'
//                AND qAbitAll.BackDoc=0 AND qAbitAll.StudyBasisId=@OtherStudyBasisId AND qAbitAll.StudyFormId=@OtherStudyFormId ";
               
//                sl.Add("@OtherStudyFormId", OtherStudyFormId);
//                sl.Add("@OtherStudyBasisId", OtherStudyBasisId);
            
                
                //if (OtherLicenseProgramId.HasValue)
                //{
                //    query += " AND qAbitAll.LicenseProgramId=@OtherLicenseProgramId ";
                //    sl.Add("@OtherLicenseProgramId", OtherLicenseProgramId);
                //}
                //if (OtherObrazProgramId.HasValue)
                //{
                //    query += " AND qAbitAll.ObrazProgramId=@OtherObrazProgramId ";
                //    sl.Add("@OtherObrazProgramId", OtherObrazProgramId);
                //}

                
                //tbl = MainClass.Bdc.GetDataSet(query, sl).Tables[0];

                //var OtherMarks = from DataRow rw in tbl.Rows
                //                 select new
                //                 {
                //                     PersonId = rw.Field<Guid>("PersonId"),
                //                     LicenseProgram = rw.Field<string>("LP"),
                //                     ObrazProgram = rw.Field<string>("OP"),
                //                     Profile = rw.Field<string>("ProfileName"),
                //                     SUM = rw.Field<decimal?>("SUM"),
                //                     Rank = rw.Field<long?>("Rank"),
                //                     KCP = rw.Field<int?>("KCP"),
                //                     Green = rw.Field<int>("GREEN") == 1 ? true : false,
                //                     Originals = rw.Field<int>("Orig") == 1 ? true : false,
                //                     VK = rw.Field<int>("VKs") == 1 ? true : false
                //                 };
                #endregion
                wc.PerformStep();
                wc.SetText("Данные сравниваемого факультета...");
                DataTable tblSource = new DataTable();

                tblSource.Columns.Add("Id");
                tblSource.Columns.Add("ФИО");
                tblSource.Columns.Add("Сумма баллов у нас");
                tblSource.Columns.Add("Рейтинг у нас", typeof(int));
                tblSource.Columns.Add("Наш проходной (КЦ)", typeof(decimal));
                tblSource.Columns.Add("Оригинал у нас");
                tblSource.Columns.Add("Направление");
                tblSource.Columns.Add("Образовательная программа");
                tblSource.Columns.Add("Профиль");
                tblSource.Columns.Add("Сумма баллов у них");
                tblSource.Columns.Add("Рейтинг у них", typeof(int));
                tblSource.Columns.Add("Их проходной (КЦ)", typeof(int));
                tblSource.Columns.Add("Оригинал у них");
                tblSource.Columns.Add("OurGREEN", typeof(bool));
                tblSource.Columns.Add("TheirGREEN", typeof(bool));
                tblSource.Columns.Add("OurVK", typeof(bool));
                tblSource.Columns.Add("TheirVK", typeof(bool));

                wc.SetText("Построение списка...");
                wc.SetMax(Persons.Count());
                foreach (var p in Persons)
                {
                    var om = Abits.Where(x => x.PersonId == p.PersonId);

                    foreach (var mrk in om)
                    {
                        DataRow row = tblSource.NewRow();
                        var _m_hlp = OtherMarks.Where(x => x.AbiturientId == mrk.Id).FirstOrDefault();
                        var _p_hlp = PersonHlpStat.Where(x => x.AbiturientId == p.Id).FirstOrDefault();
                        row["Id"] = p.PersonId;
                        row["ФИО"] = p.FIO;
                        if (_p_hlp != null)
                        {
                            if (_p_hlp.SUM.HasValue) row["Сумма баллов у нас"] = _p_hlp.SUM;  else  row["Сумма баллов у нас"] = null;
                            if (_p_hlp.Rank.HasValue) row["Рейтинг у нас"] = _p_hlp.Rank; else row["Рейтинг у нас"] = null; ;
                            if (_p_hlp.Rank.HasValue) row["OurGREEN"] = _p_hlp.Rank <= p.KCP; else row["OurGREEN"] = false;
                        }
                        else
                        {
                            row["OurGREEN"] = false;
                            row["Сумма баллов у нас"] = row["Рейтинг у нас"] = null;
                        }

                        row["OurVK"] = p.VK;
                        row["Наш проходной (КЦ)"] = p.KCP;
                        row["Оригинал у нас"] = p.Originals ? "Да" : "Нет";

                        row["Направление"] = mrk.LicenseProgram;
                        row["Образовательная программа"] = mrk.ObrazProgram;
                        row["Профиль"] = mrk.Profile;

                        if (_m_hlp != null)
                        {
                            if (_m_hlp.SUM.HasValue) row["Сумма баллов у них"] = _m_hlp.SUM; else row["Сумма баллов у них"] = null; ;
                            if (_m_hlp.Rank.HasValue) row["Рейтинг у них"] = _m_hlp.Rank; else row["Рейтинг у них"] = null; ;
                            if (_m_hlp.Rank.HasValue) row["TheirGREEN"] = _m_hlp.Rank <= mrk.KCP; else row["TheirGREEN"] = false;
                        }
                        else
                        {
                            row["Сумма баллов у них"] =   row["Рейтинг у них"] = null; 
                            row["TheirGREEN"] = false;
                        }
                        row["Их проходной (КЦ)"] = mrk.KCP;
                        row["Оригинал у них"] = mrk.Originals ? "Да" : "Нет";

                        
                        row["TheirVK"] = mrk.VK;
                         
                        tblSource.Rows.Add(row);
                    }

                    wc.PerformStep();
                }
                tblBaseSource = tblSource;
                wc.Close();

                int iMaxYellow = Persons.Select(x => x.KCP).DefaultIfEmpty(0).Max() ?? 0;
                int iMaxYellowOther = Abits.Select(x => x.KCP ?? 0).DefaultIfEmpty(0).Max();

                //int tbYell = 0;
                //int.TryParse(tbYellow.Text, out tbYell);
                //int tbYellOther = 0;
                //int.TryParse(tbYellowOther.Text, out tbYellOther);

                if (string.IsNullOrEmpty(tbYellow.Text))
                    tbYellow.Text = iMaxYellow.ToString();
                if (string.IsNullOrEmpty(tbYellowOther.Text))
                    tbYellowOther.Text = iMaxYellowOther.ToString();

                if (tbFIO.Text.Length > 0)
                {
                    GridSearch();
                    return;
                }

                dgv.DataSource = tblSource;
                GridColumnsSizeAndVisible();

                lblCount.Text = tblSource.Rows.Count.ToString();
            }
        }

        private void GridColumnsSizeAndVisible()
        {
            dgv.Columns["Id"].Visible = false;
            dgv.Columns["OurGREEN"].Visible = false;
            dgv.Columns["TheirGREEN"].Visible = false;
            dgv.Columns["OurVK"].Visible = false;
            dgv.Columns["TheirVK"].Visible = false;

            dgv.Columns["ФИО"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgv.Columns["Направление"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dgv.Columns["Образовательная программа"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgv.Columns["Сумма баллов у нас"].Width = 50;
            dgv.Columns["Сумма баллов у них"].Width = 50;
            dgv.Columns["Рейтинг у нас"].Width = 50;
            dgv.Columns["Рейтинг у них"].Width = 50;
            dgv.Columns["Оригинал у нас"].Width = 60;
            dgv.Columns["Оригинал у них"].Width = 60;

            dgv.Columns["Наш проходной (КЦ)"].Width = 65;
            dgv.Columns["Их проходной (КЦ)"].Width = 65;

            dgv.Sort(dgv.Columns["Рейтинг у нас"], ListSortDirection.Ascending);
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            //MainClass.OpenCardPerson(dgv.Rows[e.RowIndex].Cells["Id"].Value.ToString(), this, e.RowIndex);
            foreach (var x in MainClass.mainform.MdiChildren)
            {
                if (x is PersonStatRating)
                    x.Close();
            }

            Guid g;
            if (!Guid.TryParse(dgv.Rows[e.RowIndex].Cells["Id"].Value.ToString(), out g))
                return;

            new PersonStatRating(g).Show();
        }

        private void dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex != dgv.Columns["ФИО"].Index)
                return;

            int iYellow = 0;
            int.TryParse(tbYellow.Text, out iYellow);
            int iYellowOther = 0;
            int.TryParse(tbYellowOther.Text, out iYellowOther);

            if ((bool)dgv.Rows[e.RowIndex].Cells["OurGREEN"].Value)
                dgv.Rows[e.RowIndex].Cells["Рейтинг у нас"].Style.BackColor = Color.Green;
            else if (
                ((int?)dgv.Rows[e.RowIndex].Cells["Рейтинг у нас"].Value).HasValue 
                && iYellow > 0
                && (int)dgv.Rows[e.RowIndex].Cells["Рейтинг у нас"].Value <= iYellow)
                dgv.Rows[e.RowIndex].Cells["Рейтинг у нас"].Style.BackColor = Color.Orange;
            else
                dgv.Rows[e.RowIndex].Cells["Рейтинг у нас"].Style.BackColor = Color.White;

            if ((bool)dgv.Rows[e.RowIndex].Cells["TheirVK"].Value)
            {
                dgv.Rows[e.RowIndex].Cells["Сумма баллов у них"].Style.BackColor = Color.LightSkyBlue;
                dgv.Rows[e.RowIndex].Cells["Направление"].Style.BackColor = Color.LightSkyBlue;
                dgv.Rows[e.RowIndex].Cells["Образовательная программа"].Style.BackColor = Color.LightSkyBlue;
                dgv.Rows[e.RowIndex].Cells["Профиль"].Style.BackColor = Color.LightSkyBlue;
            }
            else
            {
                dgv.Rows[e.RowIndex].Cells["Сумма баллов у них"].Style.BackColor = Color.White;
                dgv.Rows[e.RowIndex].Cells["Направление"].Style.BackColor = Color.White;
                dgv.Rows[e.RowIndex].Cells["Образовательная программа"].Style.BackColor = Color.White;
                dgv.Rows[e.RowIndex].Cells["Профиль"].Style.BackColor = Color.White;
            }

            if ((bool)dgv.Rows[e.RowIndex].Cells["OurVK"].Value)
            {
                dgv.Rows[e.RowIndex].Cells["ФИО"].Style.BackColor = Color.LightSkyBlue;
                dgv.Rows[e.RowIndex].Cells["Сумма баллов у нас"].Style.BackColor = Color.LightSkyBlue;
            }
            else
            {
                dgv.Rows[e.RowIndex].Cells["ФИО"].Style.BackColor = Color.White;
                dgv.Rows[e.RowIndex].Cells["Сумма баллов у нас"].Style.BackColor = Color.White;
            }

            if ((bool)dgv.Rows[e.RowIndex].Cells["TheirGREEN"].Value)
                dgv.Rows[e.RowIndex].Cells["Рейтинг у них"].Style.BackColor = Color.Green;
            else if (
                ((int?)dgv.Rows[e.RowIndex].Cells["Рейтинг у них"].Value).HasValue
                && iYellowOther > 0
                && (int)dgv.Rows[e.RowIndex].Cells["Рейтинг у них"].Value <= iYellowOther)
                dgv.Rows[e.RowIndex].Cells["Рейтинг у них"].Style.BackColor = Color.Orange;
            else
                dgv.Rows[e.RowIndex].Cells["Рейтинг у них"].Style.BackColor = Color.White;

            if (dgv.Rows[e.RowIndex].Cells["Оригинал у нас"].Value.ToString() == "Да")
                dgv.Rows[e.RowIndex].Cells["Оригинал у нас"].Style.BackColor = Color.Gold;
            else
                dgv.Rows[e.RowIndex].Cells["Оригинал у нас"].Style.BackColor = Color.White;

            if (dgv.Rows[e.RowIndex].Cells["Оригинал у них"].Value.ToString() == "Да")
                dgv.Rows[e.RowIndex].Cells["Оригинал у них"].Style.BackColor = Color.Gold;
            else
                dgv.Rows[e.RowIndex].Cells["Оригинал у них"].Style.BackColor = Color.White;
        }

        private void tbYellow_TextChanged(object sender, EventArgs e)
        {
            dgv.Refresh();
        }
        private void tbYellowOther_TextChanged(object sender, EventArgs e)
        {
            dgv.Refresh();
        }
    }
}
