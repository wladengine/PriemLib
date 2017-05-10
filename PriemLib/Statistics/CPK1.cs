using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EducServLib;
using WordOut;
using BaseFormsLib;
using RtfWriter;

namespace PriemLib
{
    public partial class CPK1 : BaseForm
    {
        private class StatRow
        {
            public int Num { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public int PlanB { get; set; }
            public int PlanP { get; set; }
            public int SumB { get; set; }
            public int SumP { get; set; }
            public string ConcB { get; set; }
            public string ConcP { get; set; }
        }
        private int StudyLevelGroupId
        {
            get { return ComboServ.GetComboIdInt(cbStudyLevelGroup) ?? 0; }
        }

        public CPK1()
        {
            InitializeComponent();
            this.MdiParent = MainClass.mainform;
            //iStudyLevelGroupId = MainClass.lstStudyLevelGroupId.First();

            InitGrid();
            LoadGroups();
            //дальше автоматом подгрузятся остальные комбы и сформируется грид
        }

        private void LoadGroups()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lst =
                    (from ent in context.extEntry
                     where MainClass.lstStudyLevelGroupId.Contains(ent.StudyLevelGroupId)
                     select new
                     {
                         ent.StudyLevelGroupId,
                         ent.StudyLevelGroupName,
                     }).Distinct()
                     .ToList()
                     .Select(x => new KeyValuePair<string, string>(x.StudyLevelGroupId.ToString(), x.StudyLevelGroupName))
                     .ToList();

                ComboServ.FillCombo(cbStudyLevelGroup, lst, false, false);
            }
        }

        private void LoadFaculties()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var facs =
                    (from ent in context.qEntry
                     join sl in context.StudyLevel
                     on ent.StudyLevelId equals sl.Id
                     where MainClass.lstStudyLevelGroupId.Contains(sl.LevelGroupId)
                     select new
                     {
                         ent.StudyBasisId,
                         ent.StudyFormId,
                         Id = ent.FacultyId,
                         Name = ent.FacultyName
                     });

                int? iStudyFormId = GetStudyFormId();
                int? iStudyBasisId = GetStudyBasisId();

                if (iStudyFormId != null)
                    facs = facs.Where(x => x.StudyFormId == iStudyFormId.Value);
                //if (iStudyBasisId != null)
                //    facs = facs.Where(x => x.StudyBasisId == iStudyBasisId.Value);

                var lKVP = facs.OrderBy(x => x.Id).ToList().Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).Distinct().ToList();

                ComboServ.FillCombo(cbFaculty, lKVP, false, true);
            }
        }
        private void LoadStudyForms()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var sforms = (from ent in context.qEntry
                              join sl in context.StudyLevel
                              on ent.StudyLevelId equals sl.Id
                              where MainClass.lstStudyLevelGroupId.Contains(sl.LevelGroupId)
                              select new
                              {
                                  ent.StudyBasisId,
                                  ent.FacultyId,
                                  Id = ent.StudyFormId,
                                  Name = ent.StudyFormName
                              });

                int? iFacultyId = GetFacultyId();
                int? iStudyBasisId = GetStudyBasisId();
                if (iFacultyId != null)
                    sforms = sforms.Where(x => x.FacultyId == iFacultyId.Value);
                //if (iStudyBasisId != null)
                //    sforms = sforms.Where(x => x.StudyBasisId == iStudyBasisId.Value);

                var lKVP = sforms.ToList().Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).Distinct().OrderBy(x => x.Value).ToList();

                ComboServ.FillCombo(cbStudyForm, lKVP, false, false);
            }
        }
        private void LoadStudyBasis()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var sb = (from ent in context.qEntry
                              
                          join sl in context.StudyLevel
                          on ent.StudyLevelId equals sl.Id
                          where MainClass.lstStudyLevelGroupId.Contains(sl.LevelGroupId)
                          select new
                          {
                              ent.FacultyId,
                              ent.StudyFormId,
                              Id = ent.StudyBasisId,
                              Name = ent.StudyBasisName
                          });//.Distinct().ToList().Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).ToList();

                int? iFacultyId = GetFacultyId();
                int? iStudyFormId = GetStudyFormId();
                if (iFacultyId != null)
                    sb = sb.Where(x => x.FacultyId == iFacultyId.Value);
                if (iStudyFormId != null)
                    sb = sb.Where(x => x.StudyFormId == iStudyFormId.Value);

                var lKVP = sb.ToList().Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).OrderBy(x => x.Value).Distinct().ToList();

                ComboServ.FillCombo(cbStudyBasis, lKVP, false, false);
            }
        }

        private int? GetFacultyId()
        {
            if (ComboServ.IsNullOrEmpty(cbFaculty) || cbFaculty.Text == ComboServ.DISPLAY_ALL_VALUE)
                return null;

            return ComboServ.GetComboIdInt(cbFaculty);
        }
        private int? GetStudyFormId()
        {
            if (ComboServ.IsNullOrEmpty(cbStudyForm) || cbStudyForm.Text == ComboServ.DISPLAY_ALL_VALUE)
                return null;

            return ComboServ.GetComboIdInt(cbStudyForm);
        }
        private int? GetStudyBasisId()
        {
            if (ComboServ.IsNullOrEmpty(cbStudyBasis) || cbStudyBasis.Text == ComboServ.DISPLAY_ALL_VALUE)
                return null;

            return ComboServ.GetComboIdInt(cbStudyBasis);
        }

        private int GetWatchCount()
        {
            int? iFacultyId = GetFacultyId();
            int? iStudyFormId = GetStudyFormId();
            int? iStudyBasisId = GetStudyBasisId();

            string query = "SELECT COUNT(Id) FROM ed.qEntry WHERE StudyLevelGroupId=@SLGId ";
            SortedList<string, object> dic = new SortedList<string, object>();
            dic.Add("@SLGId", StudyLevelGroupId);
            if (iFacultyId != null)
            {
                query += " AND FacultyId=@FacultyId ";
                dic.Add("@FacultyId", iFacultyId.Value);
            }
            if (iStudyFormId != null)
            {
                query += " AND StudyFormId=@StudyFormId ";
                dic.Add("@StudyFormId", iStudyFormId.Value);
            }
            if (iStudyBasisId != null)
            {
                query += " AND StudyBasisId=@StudyBasisId ";
                dic.Add("@StudyBasisId", iStudyBasisId.Value);
            }

            return (int)MainClass.Bdc.GetValue(query, dic);
        }

        private void cbStudyLevelGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFaculties();
        }
        private void cbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStudyForms();
        }
        private void cbStudyForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStudyBasis();
            FillGrid();
        }

        private void FillGrid()
        {
            int num_pp = 0;
            int? iFacultyId = GetFacultyId();
            int? iStudyFormId = GetStudyFormId();
            int? iStudyBasisId = GetStudyBasisId();
            
            dgvData.Rows.Clear();

            int watch_count = GetWatchCount();
            Watch wc = new Watch(watch_count + watch_count/10); //+10%
            wc.Show();

            int iTotalPlanBudzh = 0;
            int iTotalPlanPlatn = 0;
            int iTotalBudzh = 0;
            int iTotalPlatn = 0;

            using (PriemEntities context = new PriemEntities())
            {
                var q_lp = from ent in context.qEntry
                           where ent.StudyLevelGroupId == StudyLevelGroupId
                         select new
                         {
                             ent.FacultyId,
                             ent.StudyFormId,
                             ent.StudyBasisId,
                             ent.LicenseProgramId,
                             ent.LicenseProgramCode,
                             ent.LicenseProgramName,
                         };
                if (iFacultyId != null)
                    q_lp = q_lp.Where(x => x.FacultyId == iFacultyId);
                if (iStudyFormId != null)
                    q_lp = q_lp.Where(x => x.StudyFormId == iStudyFormId);
                //if (iStudyBasisId != null)
                //    q_lp = q_lp.Where(x => x.StudyBasisId == iStudyBasisId);

                string ForeignCrimeaFilter = "";
                if (rbMainPriem.Checked)
                    ForeignCrimeaFilter = " AND IsForeign=0 AND IsCrimea=0 ";
                if (rbForeigners.Checked)
                    ForeignCrimeaFilter = " AND IsForeign=1 AND IsCrimea=0 ";

                var LicensePrograms = q_lp.Select(x => new { x.LicenseProgramId, x.LicenseProgramCode, x.LicenseProgramName }).Distinct().OrderBy(x => x.LicenseProgramCode);
                foreach (var lProgram in LicensePrograms)
                {
                    string query = string.Format(@"
                        SELECT StudyBasisId, COUNT(Id) AS CNT FROM ed.extAbit WHERE LicenseProgramId=@LicenseProgramId AND (convert(date, DocInsertDate)<=@Date)
                        AND StudyLevelGroupId=@SLGId {0} {1} " + 
                        (iStudyFormId.HasValue ? "AND StudyFormId=@StudyFormId" : "") + 
                        @" AND extAbit.Id NOT IN (SELECT Id FROM ed.qAbiturientForeignApplicationsOnly)
                        GROUP BY StudyBasisId", iFacultyId != null ? "AND FacultyId=@FacultyId" : "", ForeignCrimeaFilter);
                    SortedList<string, object> sl = new SortedList<string, object>();
                    sl.Add("@LicenseProgramId", lProgram.LicenseProgramId);
                    sl.Add("@Date", dtpDate.Value.Date );
                    sl.Add("@SLGId", StudyLevelGroupId );
                    sl.Add("@FacultyId", iFacultyId ?? 0 );
                    if (iStudyFormId.HasValue)
                        sl.Add("@StudyFormId", iStudyFormId);

                    DataTable tbl = MainClass.Bdc.GetDataSet(query, sl).Tables[0];

                    int sum_b = (from DataRow x in tbl.Rows where x.Field<int>("StudyBasisId") == 1 select x.Field<int>("CNT")).DefaultIfEmpty(0).First();
                    int sum_p = (from DataRow x in tbl.Rows where x.Field<int>("StudyBasisId") == 2 select x.Field<int>("CNT")).DefaultIfEmpty(0).First();

                    int? kcp_b = 0;
                    int? kcp_p = 0;
                    
                    query = string.Format(@"SELECT StudyBasisId, SUM(KCP) AS KCP FROM ed.qEntry WHERE LicenseProgramId=@LicenseProgramId 
                            AND StudyLevelGroupId=@SLGId {0} {1} "+ 
                        (iStudyFormId.HasValue ? "AND StudyFormId=@StudyFormId" : "") + 
                        @" GROUP BY StudyBasisId", iFacultyId != null ? "AND FacultyId=@FacultyId" : "", ForeignCrimeaFilter);
                    tbl = MainClass.Bdc.GetDataSet(query, sl).Tables[0];

                    kcp_b = (from DataRow x in tbl.Rows where x.Field<int>("StudyBasisId") == 1 select x.Field<int?>("KCP")).DefaultIfEmpty(0).First();
                    kcp_p = (from DataRow x in tbl.Rows where x.Field<int>("StudyBasisId") == 2 select x.Field<int?>("KCP")).DefaultIfEmpty(0).First();

                    float conc_b = kcp_b == null ? 0f : (sum_b == 0 ? 0f : (float)sum_b / (float)kcp_b.Value);
                    float conc_p = kcp_p == null ? 0f : (sum_p == 0 ? 0f : (float)sum_p / (float)kcp_p.Value);
                    AddRow(new StatRow
                    {
                        Num = ++num_pp,
                        Name = lProgram.LicenseProgramName,
                        Code = lProgram.LicenseProgramCode,
                        PlanB = kcp_b ?? 0,
                        PlanP = kcp_p ?? 0,
                        SumB = sum_b,
                        SumP = sum_p,
                        ConcB = kcp_b == 0 ? "!КЦ" : Math.Round(conc_b, 2).ToString(),
                        ConcP = kcp_p == 0 ? "!КЦ" : Math.Round(conc_p, 2).ToString()
                    });

                    iTotalBudzh += sum_b;
                    iTotalPlatn += sum_p;
                    iTotalPlanBudzh += kcp_b ?? 0;
                    iTotalPlanPlatn += kcp_p ?? 0;

                    var q_op = (from ent in context.qEntry
                                where ent.LicenseProgramId == lProgram.LicenseProgramId
                                && ent.StudyLevelGroupId == StudyLevelGroupId
                                && ent.IsForeign == rbForeigners.Checked
                                select new
                                {
                                    ent.FacultyId,
                                    ent.StudyFormId,
                                    ent.StudyBasisId,
                                    ent.ObrazProgramId,
                                    ent.ObrazProgramCrypt,
                                    ent.ObrazProgramName
                                });
                    if (iFacultyId != null)
                        q_op = q_op.Where(x => x.FacultyId == iFacultyId);
                    if (iStudyFormId != null)
                        q_op = q_op.Where(x => x.StudyFormId == iStudyFormId);
                    //if (iStudyBasisId != null)
                    //    q_op = q_op.Where(x => x.StudyBasisId == iStudyBasisId);

                    var ObrazPrograms = q_op.Select(x => new { x.ObrazProgramId, x.ObrazProgramName, x.ObrazProgramCrypt }).Distinct().OrderBy(x => x.ObrazProgramCrypt);
                    foreach (var oProgram in ObrazPrograms)
                    {
                        if (sl.ContainsKey("@ObrazProgramId"))
                            sl["@ObrazProgramId"] = oProgram.ObrazProgramId;
                        else
                            sl.Add("@ObrazProgramId", oProgram.ObrazProgramId);

                        query = string.Format(@"SELECT StudyBasisId, COUNT(Id) AS CNT FROM ed.extAbit 
                        WHERE LicenseProgramId=@LicenseProgramId AND ObrazProgramId=@ObrazProgramId AND (convert(date, DocInsertDate)<=@Date)
                        AND StudyLevelGroupId=@SLGId {0} {1} " +
                        (iStudyFormId.HasValue ? "AND StudyFormId=@StudyFormId" : "") +
                        @" AND extAbit.Id NOT IN (SELECT Id FROM ed.qAbiturientForeignApplicationsOnly)
                        GROUP BY StudyBasisId", iFacultyId != null ? "AND FacultyId=@FacultyId" : "", ForeignCrimeaFilter);
                        tbl = MainClass.Bdc.GetDataSet(query, sl).Tables[0];

                        sum_b = (from DataRow x in tbl.Rows where x.Field<int>("StudyBasisId") == 1 select x.Field<int>("CNT")).DefaultIfEmpty(0).First();
                        sum_p = (from DataRow x in tbl.Rows where x.Field<int>("StudyBasisId") == 2 select x.Field<int>("CNT")).DefaultIfEmpty(0).First();
                        
                        query = string.Format(@"SELECT StudyBasisId, SUM(KCP) AS KCP FROM ed.qEntry WHERE LicenseProgramId=@LicenseProgramId 
                        AND ObrazProgramId=@ObrazProgramId AND StudyLevelGroupId=@SLGId {0} {1} " +
                        (iStudyFormId.HasValue ? "AND StudyFormId=@StudyFormId" : "") +
                        @" GROUP BY StudyBasisId", iFacultyId != null ? "AND FacultyId=@FacultyId" : "", ForeignCrimeaFilter);
                        tbl = MainClass.Bdc.GetDataSet(query, sl).Tables[0];

                        kcp_b = (from DataRow x in tbl.Rows where x.Field<int>("StudyBasisId") == 1 select x.Field<int?>("KCP")).DefaultIfEmpty(0).First();
                        kcp_p = (from DataRow x in tbl.Rows where x.Field<int>("StudyBasisId") == 2 select x.Field<int?>("KCP")).DefaultIfEmpty(0).First();

                        conc_b = kcp_b == null ? 0f : (sum_b == 0 ? 0f : (float)sum_b / (float)kcp_b.Value);
                        conc_p = kcp_p == null ? 0f : (sum_p == 0 ? 0f : (float)sum_p / (float)kcp_p.Value);
                        AddRow(new StatRow
                        {
                            Num = ++num_pp,
                            Name = "        " + oProgram.ObrazProgramName,
                            Code = "",
                            PlanB = kcp_b ?? 0,
                            PlanP = kcp_p ?? 0,
                            SumB = sum_b,
                            SumP = sum_p,
                            ConcB = kcp_b == 0 ? "!КЦ" : Math.Round(conc_b, 2).ToString(),
                            ConcP = kcp_p == 0 ? "!КЦ" : Math.Round(conc_p, 2).ToString()
                        });

                        var q_prof = (from ent in context.qEntry
                                    where ent.LicenseProgramId == lProgram.LicenseProgramId
                                    && ent.ObrazProgramId == oProgram.ObrazProgramId
                                    && ent.StudyLevelGroupId == StudyLevelGroupId
                                    && ent.ProfileId != null
                                    && ent.IsForeign == rbForeigners.Checked
                                    select new
                                    {
                                        ent.FacultyId,
                                        ent.StudyFormId,
                                        ent.StudyBasisId,
                                        ent.ProfileId,
                                        ent.ProfileName,
                                    });
                        if (iFacultyId != null)
                            q_prof = q_prof.Where(x => x.FacultyId == iFacultyId);
                        if (iStudyFormId != null)
                            q_prof = q_prof.Where(x => x.StudyFormId == iStudyFormId);
                        //if (iStudyBasisId != null)
                        //    q_prof = q_prof.Where(x => x.StudyBasisId == iStudyBasisId);

                        var Profiles = q_prof.Select(x => new { x.ProfileId, x.ProfileName }).Distinct().OrderBy(x => x.ProfileName);
                        if (Profiles.Count() == 1 && Profiles.First().ProfileId == 0)
                            continue;
                        foreach (var prof in Profiles)
                        {
                            if (sl.ContainsKey("@ProfileId"))
                                sl["@ProfileId"] = prof.ProfileId;
                            else
                                sl.Add("@ProfileId", prof.ProfileId);

                            query = string.Format(@"SELECT StudyBasisId, COUNT(Id) AS CNT FROM ed.extAbit 
                        WHERE LicenseProgramId=@LicenseProgramId AND ObrazProgramId=@ObrazProgramId AND ProfileId=@ProfileId AND (convert(date, DocInsertDate)<=@Date) 
                        AND StudyLevelGroupId=@SLGId {0} {1} " +
                        (iStudyFormId.HasValue ? "AND StudyFormId=@StudyFormId" : "") +
                        @" AND extAbit.Id NOT IN (SELECT Id FROM ed.qAbiturientForeignApplicationsOnly)
                        GROUP BY StudyBasisId", iFacultyId != null ? "AND FacultyId=@FacultyId" : "", ForeignCrimeaFilter);
                            tbl = MainClass.Bdc.GetDataSet(query, sl).Tables[0];

                            sum_b = (from DataRow x in tbl.Rows where x.Field<int>("StudyBasisId") == 1 select x.Field<int>("CNT")).DefaultIfEmpty(0).First();
                            sum_p = (from DataRow x in tbl.Rows where x.Field<int>("StudyBasisId") == 2 select x.Field<int>("CNT")).DefaultIfEmpty(0).First();
                            
                            query = string.Format(@"SELECT StudyBasisId, SUM(KCP) AS KCP FROM ed.qEntry WHERE LicenseProgramId=@LicenseProgramId 
                        AND ObrazProgramId=@ObrazProgramId AND ProfileId=@ProfileId 
                        AND StudyLevelGroupId=@SLGId {0} {1} " +
                        (iStudyFormId.HasValue ? "AND StudyFormId=@StudyFormId" : "") +
                        @" GROUP BY StudyBasisId", iFacultyId != null ? "AND FacultyId=@FacultyId" : "", ForeignCrimeaFilter);
                            tbl = MainClass.Bdc.GetDataSet(query, sl).Tables[0];

                            kcp_b = (from DataRow x in tbl.Rows where x.Field<int>("StudyBasisId") == 1 select x.Field<int?>("KCP")).DefaultIfEmpty(0).First();
                            kcp_p = (from DataRow x in tbl.Rows where x.Field<int>("StudyBasisId") == 2 select x.Field<int?>("KCP")).DefaultIfEmpty(0).First();

                            conc_b = kcp_b == null ? 0f : (sum_b == 0 ? 0f : (float)sum_b / (float)kcp_b.Value);
                            conc_p = kcp_p == null ? 0f : (sum_p == 0 ? 0f : (float)sum_p / (float)kcp_p.Value);
                            AddRow(new StatRow
                            {
                                Num = ++num_pp,
                                Name = "                " + prof.ProfileName,
                                Code = "",
                                PlanB = kcp_b ?? 0,
                                PlanP = kcp_p ?? 0,
                                SumB = sum_b,
                                SumP = sum_p,
                                ConcB = kcp_b == 0 ? "!КЦ" : Math.Round(conc_b, 2).ToString(),
                                ConcP = kcp_p == 0 ? "!КЦ" : Math.Round(conc_p, 2).ToString()
                            });

                            wc.PerformStep();
                        }
                        wc.PerformStep();
                    }
                    wc.PerformStep();
                }
                AddRow(new StatRow()
                {
                    Name = "",
                    Code = "Всего",
                    PlanB = iTotalPlanBudzh,
                    PlanP = iTotalPlanPlatn,
                    SumB = iTotalBudzh,
                    SumP = iTotalPlatn,
                    ConcB = "-",
                    ConcP = "-"
                });
                wc.Close();
            }
        }

        private void AddRow(StatRow data)
        {
            dgvData.Rows.Add(data.Num, data.Name, data.Code, data.PlanB, data.PlanP, data.SumB, data.SumP, data.ConcB, data.ConcP);
        }

        private void InitGrid()
        {
            dgvData.Columns.Add("Num", "№ п/п");
            dgvData.Columns.Add("Name", "Направление");
            dgvData.Columns.Add("Code", "Шифр");
            dgvData.Columns.Add("PlanB", "План г/б");
            dgvData.Columns.Add("PlanP", "План дог");
            dgvData.Columns.Add("SumB", "Подано г/б");
            dgvData.Columns.Add("SumP", "Подано дог");
            dgvData.Columns.Add("ConcB", "Конкурс г/б");
            dgvData.Columns.Add("ConcP", "Конкурс дог");

            dgvData.Columns["Num"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvData.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvData.Columns["Code"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvData.Columns["PlanB"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvData.Columns["PlanP"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvData.Columns["SumB"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvData.Columns["SumP"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvData.Columns["ConcB"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvData.Columns["ConcP"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
        }

        private void btnWord_Click(object sender, EventArgs e)
        {
            try
            {
                WordDoc wd = new WordDoc(string.Format(@"{0}\CPKForm1.dot", MainClass.dirTemplates));
                TableDoc td = wd.Tables[0];

                for (int i = 1; i < dgvData.RowCount; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        td[j, i + 1] = dgvData.Rows[i - 1].Cells[j].Value.ToString();
                    }
                    td.AddRow(1);
                }

                //td.DeleteLastRow();

                string sFac = cbFaculty.Text.ToLower();
                if (sFac.CompareTo("все") == 0)
                    sFac = "всех факультетов ";
                else
                {
                    if (ComboServ.GetComboId(cbFaculty) == "10")
                        sFac = "медицинского колледжа ";
                    else if (ComboServ.GetComboId(cbFaculty) == "3")
                        sFac = "высшей школы менеджмента ";
                    else
                        sFac = sFac.Replace("кий", "кого ").Replace("ый", "ого ") + " факультета ";
                }

                string sForm = cbStudyForm.Text.ToLower();
                if (sForm.CompareTo("все") == 0)
                    sForm = " всех форм обучения ";
                else
                    sForm = sForm.Replace("ая", "ой").Replace("яя", "ей") + " формы обучения ";
                wd.Fields["Faculty"].Text = sFac;
                wd.Fields["Section"].Text = sForm;
                wd.Fields["Date"].Text = "на " + dtpDate.Value.ToShortDateString();
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
        //private void CPK_Rtf()
        //{
        //    RtfDocument doc = new RtfDocument(PaperSize.A4, PaperOrientation.Landscape, Lcid.Russian);

        //    string sFac = cbFaculty.Text.ToLower();
        //    if (sFac.CompareTo("все") == 0)
        //        sFac = "всех факультетов ";
        //    else
        //    {
        //        if (ComboServ.GetComboId(cbFaculty) == "10")
        //            sFac = "медицинского колледжа ";
        //        else if (ComboServ.GetComboId(cbFaculty) == "3")
        //            sFac = "высшей школы менеджмента ";
        //        else
        //            sFac = sFac.Replace("кий", "кого ").Replace("ый", "ого ").Replace("акультет", "акультета");
        //    }

        //    string sForm = cbStudyForm.Text.ToLower();
        //    if (sForm.CompareTo("все") == 0)
        //        sForm = " всех форм обучения ";
        //    else
        //        sForm = sForm.Replace("ая", "ой").Replace("яя", "ей") + " формы обучения ";

        //    doc.addParagraph().Text = 
        //        string.Format("Ежедневная информация (форма ЦПК)\r\n{0}\r\n{1}", sFac + " " + sForm, "на " + DateTime.Now.ToShortDateString());

        //    RtfTable tbl = doc.addTable(2 + dgvData.Rows.Count, 1 + dgvData.ColumnCount);
            
        //    //делаем заголовки
        //    //
        //    tbl.merge(0, 0, 2, 1);
        //    tbl.FillCell(0, 0, "", FontStyleFlag.Normal);
            
        //    tbl.merge(0, 1, 2, 1);
        //    tbl.FillCell(0, 1, "Направление, специальность", FontStyleFlag.Normal);
            
        //    tbl.merge(0, 2, 2, 1);
        //    tbl.FillCell(0, 0, "Шифр", FontStyleFlag.Normal);

        //    tbl.merge(0, 3, 1, 2);
        //    tbl.FillCell(0, 0, "План приема", FontStyleFlag.Normal);

        //    tbl.merge(0, 5, 1, 2);
        //    tbl.FillCell(0, 0, "Количество поданных заявлений", FontStyleFlag.Normal);

        //    tbl.merge(0, 7, 1, 2);
        //    tbl.FillCell(0, 0, "Конкурс", FontStyleFlag.Normal);

        //    tbl.FillCell(1, 3, "бюджет", FontStyleFlag.Normal);
        //    tbl.FillCell(1, 4, "догов.", FontStyleFlag.Normal);
        //    tbl.FillCell(1, 5, "бюджет", FontStyleFlag.Normal);
        //    tbl.FillCell(1, 6, "догов.", FontStyleFlag.Normal);
        //    tbl.FillCell(1, 7, "бюджет", FontStyleFlag.Normal);
        //    tbl.FillCell(1, 8, "догов.", FontStyleFlag.Normal);

        //    int iRow = 3;
        //    RtfParagraph p;
        //    foreach (DataGridViewRow row in dgvData.Rows)
        //    {
        //        string sProgramName = row.Cells[1].Value.ToString();
        //        if (!sProgramName.StartsWith("   ", StringComparison.OrdinalIgnoreCase))
        //        {

        //        }

        //        p = tbl.cell(iRow, 0).addParagraph();
        //        p.Text = (iRow - 2).ToString();
        //    }

        //    doc.render();
        //}
        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            FillGrid();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void rbMainPriem_CheckedChanged(object sender, EventArgs e)
        {
            //для предотвращения двойного обновления грида
            if (rbMainPriem.Checked)
                FillGrid();
        }
        private void rbForeigners_CheckedChanged(object sender, EventArgs e)
        {
            //для предотвращения двойного обновления грида
            if (rbForeigners.Checked)
                FillGrid();
        }
    }
}
