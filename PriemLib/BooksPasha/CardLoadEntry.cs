using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using EducServLib;
using BDClassLib;
using System.Transactions;

namespace PriemLib
{
    public partial class CardLoadEntry : Form
    {
        private DBPriem _bdcEduc;
        private SQLClass _bdcPriemOnline;

        public CardLoadEntry()
        {
            InitializeComponent();
            InitDB();
        }

        private void InitDB()
        {
            _bdcEduc = new DBPriem();
            _bdcPriemOnline = new SQLClass();
            try
            {
                _bdcEduc.OpenDatabase(DBConstants.CS_STUDYPLAN);
                _bdcPriemOnline.OpenDatabase(DBConstants.CS_PriemONLINE_ReadWrite);
            }
            catch (Exception exc)
            {
                WinFormsServ.Error(exc);
            }
        }

        private void btnLoadAll_Click(object sender, EventArgs e)
        {
            if (!MainClass.IsEntryChanger())
                return;

            DataSet dsEntry = _bdcEduc.GetDataSet("SELECT * FROM ed.extCurrentEntry");
            DataTable dt = dsEntry.Tables[0];

            using (PriemEntities context = new PriemEntities())
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Guid entryId = (Guid)dr["Id"];

                    List<int> lst = new List<int>() { 16/*, 17*/, 18 };//только 1 курс и магистратура - СПО не трогаем

                    int cntEnt = (from ent in context.Entry
                                  where ent.Id == entryId
                                  && lst.Contains(ent.StudyLevelId)
                                  select ent).Count();

                    int iObrazProgramId = (int)dr["ObrazProgramId"];
                    Guid gStudyPlanId = (Guid)dr["StudyPlanId"];
                    
                    if (cntEnt == 0)
                    {
                        Entry item = new Entry();
                        item.Id = entryId;
                        item.FacultyId = (int)dr["FacultyId"];
                        item.LicenseProgramId = (int)dr["LicenseProgramId"];
                        item.ObrazProgramId = iObrazProgramId;
                        item.ProfileId = dr.Field<int?>("ProfileId") ?? 0;
                        //item.ProfileName = dr["ProfileName"].ToString();
                        item.StudyBasisId = (int)dr["StudyBasisId"];
                        item.StudyFormId = (int)dr["StudyFormId"];
                        item.StudyLevelId = (int)dr["StudyLevelId"];
                        item.StudyPlanId = gStudyPlanId;
                        item.StudyPlanNumber = dr["StudyPlanNumber"].ToString();
                        item.ProgramModeShortName = dr["ProgramModeShortName"].ToString();
                        item.IsSecond = (bool)dr["IsSecond"];
                        item.KCP = dr.Field<int?>("KCP");

                        context.Entry_Insert(entryId, (int)dr["FacultyId"], (int)dr["LicenseProgramId"],
                                (int)dr["ObrazProgramId"], dr.Field<int?>("ProfileId") ?? 0, (int)dr["StudyBasisId"],
                                (int)dr["StudyFormId"], (int)dr["StudyLevelId"], (Guid)dr["StudyPlanId"], dr["StudyPlanNumber"].ToString(),
                                dr["ProgramModeShortName"].ToString(), (bool)dr["IsSecond"], (bool)dr["IsReduced"], (bool)dr["IsParallel"], dr.Field<int?>("KCP"), null, false, false);
                    }

                    ////inner profiles
                    //DataSet dsProf = _bdcEduc.GetDataSet("SELECT ProfileId FROM ed.extStudyPlanProfiles WHERE StudyPlanId=@SP", new SortedList<string,object>() { {"@SP", gStudyPlanId}});
                    //DataTable tbl = dsProf.Tables[0];
                    ////если план многопрофильный
                    //if (tbl.Rows.Count > 1)
                    //{
                    //    foreach (DataRow row in tbl.Rows)
                    //    {
                    //        int iProfileId = row.Field<int?>("ProfileId") ?? 0;
                    //        if (context.InnerEntryInEntry.Where(x => x.EntryId == entryId && x.ObrazProgramId == iObrazProgramId && x.ProfileId == iProfileId).Count() == 0)
                    //        {
                    //            InnerEntryInEntry innEntry = new InnerEntryInEntry();
                    //            innEntry.Id = Guid.NewGuid();
                    //            innEntry.ObrazProgramId = iObrazProgramId;
                    //            innEntry.ProfileId = iProfileId;
                    //            innEntry.EntryId = entryId;

                    //            context.InnerEntryInEntry.Add(innEntry);
                    //            context.SaveChanges();
                    //        }
                    //    }
                    //}
                }

                MessageBox.Show("Выполнено");
            }
        }   

        private void CardLoadEntry_FormClosing(object sender, FormClosingEventArgs e)
        {
            _bdcEduc.CloseDataBase();
        }

        private void btnCheckUpdates_Click(object sender, EventArgs e)
        {
            if (!MainClass.IsEntryChanger())
                return;
            
            List<string> missingInOurs = new List<string>();
            List<string> invalidOurs = new List<string>();
            List<string> extraOurs = new List<string>();
                        
            DataSet ds = _bdcEduc.GetDataSet(string.Format("SELECT * FROM ed.extCurrentEntry"));
            string diskDriveLetter = System.Environment.UserName == "o.belenog" ? "O" : "D";
            using (StreamWriter sw = new StreamWriter(string.Format("{0}:\\result.txt", diskDriveLetter)))
            {
                List<string> lstOld = new List<string>();
                
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string entryId = dr["Id"].ToString();

                    lstOld.Add(string.Format("'{0}'", entryId));

                    DataSet dsOur = MainClass.Bdc.GetDataSet(string.Format("SELECT * FROM ed.extEntry WHERE Id = '{0}'", entryId));
                    if (dsOur.Tables[0].Rows.Count == 0)
                    {
                        missingInOurs.Add(entryId);
                        continue;
                    }

                    DataRow drOur = dsOur.Tables[0].Rows[0];

                    if (!dr["FacultyId"].Equals(drOur["FacultyId"]))
                        invalidOurs.Add(entryId + ": FacultyId Old - " + drOur["FacultyId"].ToString() + "; New - " + dr["FacultyId"].ToString());

                    if (!dr["LicenseProgramId"].Equals(drOur["LicenseProgramId"]))
                        invalidOurs.Add(entryId + ": LicenseProgramId Old - " + drOur["LicenseProgramId"].ToString() + "; New - " + dr["LicenseProgramId"].ToString());

                    if (!dr["LicenseProgramName"].Equals(drOur["LicenseProgramName"]))
                        invalidOurs.Add(entryId + ": LicenseProgramName Old - " + drOur["LicenseProgramName"].ToString() + "; New - " + dr["LicenseProgramName"].ToString());

                    if (!dr["LicenseProgramCode"].Equals(drOur["LicenseProgramCode"]))
                        invalidOurs.Add(entryId + ": LicenseProgramCode Old - " + drOur["LicenseProgramCode"].ToString() + "; New - " + dr["LicenseProgramCode"].ToString());

                    if (!dr["ObrazProgramId"].Equals(drOur["ObrazProgramId"]))
                        invalidOurs.Add(entryId + ": ObrazProgramId Old - " + drOur["ObrazProgramId"].ToString() + "; New - " + dr["ObrazProgramId"].ToString());

                    if (!dr["ObrazProgramName"].Equals(drOur["ObrazProgramName"]))
                        invalidOurs.Add(entryId + ": ObrazProgramName Old - " + drOur["ObrazProgramName"].ToString() + "; New - " + dr["ObrazProgramName"].ToString());

                    if (!dr["ObrazProgramNumber"].Equals(drOur["ObrazProgramNumber"]))
                        invalidOurs.Add(entryId + ": ObrazProgramNumber Old - " + drOur["ObrazProgramNumber"].ToString() + "; New - " + dr["FacultyId"].ToString());

                    if (!dr["ObrazProgramCrypt"].Equals(drOur["ObrazProgramCrypt"]))
                        invalidOurs.Add(entryId + ": ObrazProgramCrypt Old - " + drOur["ObrazProgramCrypt"].ToString() + "; New - " + dr["ObrazProgramCrypt"].ToString());

                    if (!dr["ProfileId"].Equals(drOur["ProfileId"]))
                        invalidOurs.Add(entryId + ": ProfileId Old - " + drOur["ProfileId"].ToString() + "; New - " + dr["ProfileId"].ToString());

                    if (!dr["ProfileName"].Equals(drOur["ProfileName"]))
                        invalidOurs.Add(entryId + ": ProfileName Old - " + drOur["ProfileName"].ToString() + "; New - " + dr["ProfileName"].ToString());

                    if (!dr["StudyBasisId"].Equals(drOur["StudyBasisId"]))
                        invalidOurs.Add(entryId + ": StudyBasisId Old - " + drOur["StudyBasisId"].ToString() + "; New - " + dr["StudyBasisId"].ToString());

                    if (!dr["StudyFormId"].Equals(drOur["StudyFormId"]))
                        invalidOurs.Add(entryId + ": StudyFormId Old - " + drOur["StudyFormId"].ToString() + "; New - " + dr["StudyFormId"].ToString());

                    if (!dr["StudyLevelId"].Equals(drOur["StudyLevelId"]))
                        invalidOurs.Add(entryId + ": StudyLevelId Old - " + drOur["StudyLevelId"].ToString() + "; New - " + dr["StudyLevelId"].ToString());

                    if (!dr["StudyPlanId"].Equals(drOur["StudyPlanId"]))
                        invalidOurs.Add(entryId + ": StudyPlanId Old - " + drOur["StudyPlanId"].ToString() + "; New - " + dr["StudyPlanId"].ToString());

                    if (!dr["StudyPlanNumber"].Equals(drOur["StudyPlanNumber"]))
                        invalidOurs.Add(entryId + ": StudyPlanNumber Old - " + drOur["StudyPlanNumber"].ToString() + "; New - " + dr["StudyPlanNumber"].ToString());

                    if (!dr["ProgramModeShortName"].Equals(drOur["ProgramModeShortName"]))
                        invalidOurs.Add(entryId + ": ProgramModeShortName Old - " + drOur["ProgramModeShortName"].ToString() + "; New - " + dr["ProgramModeShortName"].ToString());

                    if (!dr["IsSecond"].Equals(drOur["IsSecond"]))
                        invalidOurs.Add(entryId + ": IsSecond Old - " + drOur["IsSecond"].ToString() + "; New - " + dr["IsSecond"].ToString());

                    if (!dr["IsReduced"].Equals(drOur["IsReduced"]))
                        invalidOurs.Add(entryId + ": IsReduced Old - " + drOur["IsReduced"].ToString() + "; New - " + dr["IsReduced"].ToString());

                    if (!dr["IsParallel"].Equals(drOur["IsParallel"]))
                        invalidOurs.Add(entryId + ": IsParallel Old - " + drOur["IsParallel"].ToString() + "; New - " + dr["IsParallel"].ToString());

                    if (!dr["KCP"].Equals(drOur["KCP"]))
                        invalidOurs.Add(entryId + ": KCP Old - " + drOur["KCP"].ToString() + "; New - " + dr["KCP"].ToString());
                }

                DataSet dsExtra = MainClass.Bdc.GetDataSet(string.Format("SELECT ed.Entry.Id FROM ed.Entry WHERE Id NOT IN ({0})", Util.BuildStringWithCollection(lstOld)));
                foreach (DataRow dr in dsExtra.Tables[0].Rows)
                {
                    extraOurs.Add(dr["Id"].ToString());
                }
                
                sw.WriteLine("Лишние нас:");
                sw.WriteLine("");
                foreach (string pl in extraOurs)
                {
                    sw.WriteLine(pl);
                }

                sw.WriteLine("Отсутсвуют у нас:");
                sw.WriteLine("");
                foreach (string pl in missingInOurs)
                {
                    sw.WriteLine(pl);
                }

                sw.WriteLine("");
                sw.WriteLine("Другие значение у нас:");
                sw.WriteLine("");

                foreach (string pl in invalidOurs)
                {
                    sw.WriteLine(pl);
                }

                MessageBox.Show("Выполнено");

                Process pr = new Process();
                pr.StartInfo.Verb = "Open";
                pr.StartInfo.FileName = string.Format("{0}:\\result.txt", diskDriveLetter);
                pr.Start();
            }            
        }

        private void btnLoadUpdates_Click(object sender, EventArgs e)
        {
            if (!MainClass.IsEntryChanger())
                return;

            DataSet ds = _bdcEduc.GetDataSet(string.Format(@"SELECT Id, FacultyId, LicenseProgramId, ObrazProgramId, 
                      ProfileId, StudyBasisId, StudyFormId, ProgramModeId, KCP, StudyPlanId, 
                      StudyPlanNumber, PlanYearId, PlanYear, StudyLevelId, IsSecond, IsSecondPrint, IsParallel, IsReduced, IsElectronic, IsDistance, QualificationId, 
                      AggregateGroupId, EducationPeriodId FROM ed.extCurrentEntry"));

            using (PriemEntities context = new PriemEntities())
            using (System.Transactions.TransactionScope tran = new System.Transactions.TransactionScope())
            {
                try
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Guid entryId = (Guid)dr["Id"];

                        int cntEnt = (from ent in context.Entry
                                      where ent.Id == entryId
                                      select ent).Count();

                        if (cntEnt == 0)
                        {
                            Entry item = new Entry();
                            item.Id = entryId;
                            item.FacultyId = (int)dr["FacultyId"];
                            item.LicenseProgramId = (int)dr["LicenseProgramId"];
                            item.ObrazProgramId = (int)dr["ObrazProgramId"];
                            item.ProfileId = dr.Field<int?>("ProfileId") ?? 0;
                            item.StudyBasisId = (int)dr["StudyBasisId"];
                            item.StudyFormId = (int)dr["StudyFormId"];
                            item.StudyLevelId = (int)dr["StudyLevelId"];
                            item.StudyPlanId = (Guid)dr["StudyPlanId"];
                            item.StudyPlanNumber = dr["StudyPlanNumber"].ToString();
                            item.ProgramModeShortName = dr["ProgramModeShortName"].ToString();
                            item.IsSecond = (bool)dr["IsSecond"];
                            item.KCP = dr.Field<int?>("KCP");

                            context.Entry_Insert(entryId, (int)dr["FacultyId"], (int)dr["LicenseProgramId"], (int)dr["ObrazProgramId"], 
                                dr.Field<int?>("ProfileId") ?? 0, (int)dr["StudyBasisId"],
                                    (int)dr["StudyFormId"], (int)dr["StudyLevelId"], (Guid)dr["StudyPlanId"], dr["StudyPlanNumber"].ToString(),
                                    dr["ProgramModeShortName"].ToString(), (bool)dr["IsSecond"], (bool)dr["IsReduced"], (bool)dr["IsParallel"], dr.Field<int?>("KCP"), null, false, false);
                        }

                        string query = "SELECT COUNT(*) FROM Entry WHERE Id=@Id";
                        SortedList<string, object> slParams = new SortedList<string, object>();
                        slParams.Add("@Id", entryId);
                        cntEnt = int.Parse(MainClass.BdcOnlineReadWrite.GetStringValue(query, slParams));

                        if (cntEnt == 0)
                        {
                            query = @"INSERT INTO Entry (Id, SemesterId, StudyPlanId, StudyPlanNumber, FacultyId, LicenseProgramId, ObrazProgramId, StudyBasisId, StudyFormId, 
StudyLevelId, ProfileId, IsSecond, IsReduced, IsParallel, IsExpress, IsElectronic, IsDistance, 
CampaignYear, QualificationId, AggregateGroupId, ProgramModeId, EducationPeriodId, 
DateOfClose, DateOfStart, IsUsedForPriem) VALUES
(@Id, 1, @StudyPlanId, @StudyPlanNumber, @FacultyId, @LicenseProgramId, @ObrazProgramId, @StudyBasisId, @StudyFormId, 
@StudyLevelId, @ProfileId, @IsSecond, @IsReduced, @IsParallel, @IsExpress, @IsElectronic, @IsDistance, 
@CampaignYear, @QualificationId,  @AggregateGroupId, @ProgramModeId,  @EducationPeriodId, 
@DateOfClose, @DateOfStart, @IsUsedForPriem)";

                            slParams.Clear();
                            slParams.Add("@Id", entryId);
                            slParams.Add("@StudyPlanId", dr.Field<Guid>("StudyPlanId"));
                            slParams.Add("@StudyPlanNumber", dr.Field<string>("StudyPlanNumber"));
                            slParams.Add("@FacultyId", dr.Field<int>("FacultyId"));
                            slParams.Add("@FacultyName", dr.Field<string>("FacultyName"));
                            slParams.Add("@LicenseProgramId", dr.Field<int>("LicenseProgramId"));
                            slParams.Add("@ObrazProgramId", dr.Field<int>("ObrazProgramId"));
                            slParams.Add("@StudyBasisId", dr.Field<int>("StudyBasisId"));
                            slParams.Add("@StudyFormId", dr.Field<int>("StudyFormId"));
                            slParams.Add("@StudyLevelId", dr.Field<int>("StudyLevelId"));
                            slParams.Add("@ProfileId", dr.Field<int?>("ProfileId") ?? 0);
                            slParams.Add("@IsSecond", dr.Field<bool>("IsSecond"));
                            slParams.Add("@IsReduced", dr.Field<bool>("IsReduced"));
                            slParams.Add("@IsParallel", dr.Field<bool>("IsParallel"));
                            slParams.Add("@IsExpress", dr.Field<bool>("IsParallel"));
                            slParams.Add("@IsElectronic", dr.Field<bool>("IsElectronic"));
                            slParams.Add("@IsDistance", dr.Field<bool>("IsDistance"));
                            slParams.Add("@CampaignYear", MainClass.iPriemYear);
                            slParams.Add("@QualificationId", dr.Field<int>("QualificationId"));
                            slParams.Add("@AggregateGroupId", dr.Field<int>("AggregateGroupId"));
                            slParams.Add("@ProgramModeId", dr.Field<int>("ProgramModeId"));
                            slParams.Add("@EducationPeriodId", dr.Field<int>("EducationPeriodId"));
                            slParams.Add("@DateOfClose", new DateTime(MainClass.iPriemYear, 7, 20));
                            slParams.Add("@DateOfStart", dr.Field<int>("StudyLevelId") == 17 ? new DateTime(MainClass.iPriemYear, 6, 20) : new DateTime(MainClass.iPriemYear, 3, 1));
                            slParams.Add("@IsUsedForPriem", true);

                            context.Entry_Insert(entryId, (int)dr["FacultyId"], (int)dr["LicenseProgramId"], (int)dr["ObrazProgramId"], dr.Field<int?>("ProfileId") ?? 0, 
                                (int)dr["StudyBasisId"], (int)dr["StudyFormId"], (int)dr["StudyLevelId"], (Guid)dr["StudyPlanId"], dr["StudyPlanNumber"].ToString(),
                                    dr["ProgramModeShortName"].ToString(), (bool)dr["IsSecond"], (bool)dr["IsReduced"], (bool)dr["IsParallel"], dr.Field<int?>("KCP"), null, false, false);

                            MainClass.BdcOnlineReadWrite.ExecuteQuery(query, slParams);
                        }
                    }

                    tran.Complete();
                    MessageBox.Show("Выполнено");
                }
                catch (Exception ex)
                {
                    WinFormsServ.Error(ex);
                }
            }
        }

        private void btnUpdateKCP_Click(object sender, EventArgs e)
        {
            if (!MainClass.IsEntryChanger())
                return;

            DataSet ds = _bdcEduc.GetDataSet(string.Format("SELECT Id, KCP FROM ed.extCurrentEntry"));
            using (PriemEntities context = new PriemEntities())
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Guid entryId = (Guid)dr["Id"];

                    int cntEnt = (from ent in context.Entry
                                  where ent.Id == entryId
                                  select ent).Count();
                   
                   if (cntEnt == 0)
                       continue;

                    Entry entry =  (from ent in context.Entry
                                    where ent.Id == entryId
                                    select ent).FirstOrDefault();

                    int? kcpSP;
                    
                    if (dr["KCP"].ToString() == string.Empty)
                        kcpSP = 0;
                    else
                        kcpSP = (int?)dr["KCP"];

                    if (kcpSP != entry.KCP)
                        context.Entry_UpdateKC(entryId, kcpSP, kcpSP / 10);
                    
                }
                MessageBox.Show("Выполнено");
            }            
        }

        private void btnOnlineLoadUpdate_Click(object sender, EventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                var EntryList = context.Entry.ToList();
                var InnerEntiesData = context.InnerEntryInEntry.ToList();
                ProgressForm pf = new ProgressForm();
                pf.Show();
                pf.MaxPrBarValue = EntryList.Count;
                pf.SetProgressText("Загрузка списка конкурсов...");
                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {
                        foreach (var Entr in EntryList)
                        {
                            pf.PerformStep();

                            string query = "SELECT COUNT(*) FROM [Entry] WHERE Id=@Id";
                            SortedList<string, object> slParams = new SortedList<string, object>();
                            slParams.Add("@Id", Entr.Id);
                            int cnt = (int)_bdcPriemOnline.GetValue(query, slParams);
                            if (cnt == 0)
                            {
                                query = @"INSERT INTO Entry
(
       [Id]
      ,[SemesterId]
      ,[FacultyId]
      ,[LicenseProgramId]
      ,[ObrazProgramId]
      ,[StudyBasisId]
      ,[StudyFormId]
      ,[StudyLevelId]
      ,[ProfileId]
      ,[IsSecond]
      ,[IsReduced]
      ,[IsParallel]
      ,[CampaignYear]
      ,[DateOfClose]
      ,[DateOfStart]
      ,[IsUsedForPriem]
)
VALUES
(
       @Id
      ,@SemesterId
      ,@FacultyId
      ,@LicenseProgramId
      ,@ObrazProgramId
      ,@StudyBasisId
      ,@StudyFormId
      ,@StudyLevelId
      ,@ProfileId
      ,@IsSecond
      ,@IsReduced
      ,@IsParallel
      ,@CampaignYear
      ,@DateOfClose
      ,@DateOfStart
      ,@IsUsedForPriem
)";
                            }
                            else
                            {
                                query = @"UPDATE Entry
SET 
      [SemesterId]=@SemesterId
      ,[FacultyId]=@FacultyId
      ,[LicenseProgramId]=@LicenseProgramId
      ,[ObrazProgramId]=@ObrazProgramId
      ,[StudyBasisId]=@StudyBasisId
      ,[StudyFormId]=@StudyFormId
      ,[StudyLevelId]=@StudyLevelId
      ,[ProfileId]=@ProfileId
      ,[IsSecond]=@IsSecond
      ,[IsReduced]=@IsReduced
      ,[IsParallel]=@IsParallel
      ,[CampaignYear]=@CampaignYear
      ,[DateOfClose]=@DateOfClose
      ,[DateOfStart]=@DateOfStart
      ,[IsUsedForPriem]=@IsUsedForPriem
WHERE
      [Id]=@Id";
                            }
                            slParams.Clear();
                            slParams.Add("@IsUsedForPriem", true);
                            slParams.Add("@DateOfStart", Entr.DateOfStart);
                            slParams.Add("@DateOfClose", Entr.DateOfClose);
                            slParams.Add("@CampaignYear", MainClass.iPriemYear);
                            slParams.Add("@IsDistance", false);
                            slParams.Add("@IsElectronic", false);
                            slParams.Add("@IsExpress", false);
                            slParams.Add("@IsParallel", Entr.IsParallel);
                            slParams.Add("@IsReduced", Entr.IsReduced);
                            slParams.Add("@IsSecond", Entr.IsClosed);
                            slParams.AddVal("@ProfileId", Entr.ProfileId);

                            slParams.Add("@StudyLevelId", Entr.StudyLevelId);
                            slParams.Add("@StudyFormId", Entr.StudyFormId);
                            slParams.Add("@StudyBasisId", Entr.StudyBasisId);
                            slParams.Add("@ObrazProgramId", Entr.ObrazProgramId);
                            slParams.Add("@LicenseProgramId", Entr.LicenseProgramId);
                            slParams.Add("@FacultyId", Entr.FacultyId);
                            slParams.Add("@SemesterId", 1);
                            slParams.Add("@Id", Entr.Id);
                            _bdcPriemOnline.ExecuteQuery(query, slParams);

                            var lstInnerEntries = InnerEntiesData.Where(x => x.EntryId == Entr.Id).Select(x => new { x.Id, x.EntryId, x.ObrazProgramId, x.ProfileId }).ToList();
                            if (lstInnerEntries.Count > 1)
                            {
                                foreach (var inEnt in lstInnerEntries)
                                {
                                    query = "SELECT COUNT(*) FROM InnerEntryInEntry WHERE Id=@Id";
                                    slParams.Clear();
                                    slParams.Add("@Id", inEnt.Id);
                                    cnt = (int)_bdcPriemOnline.GetValue(query, slParams);

                                    if (cnt == 0)
                                        query = "INSERT INTO InnerEntryInEntry (Id, EntryId, ObrazProgramId, ProfileId) VALUES (@Id, @EntryId, @ObrazProgramId, @ProfileId)";
                                    else
                                        query = "UPDATE InnerEntryInEntry SET EntryId=@EntryId, ObrazProgramId=@ObrazProgramId, ProfileId=@ProfileId WHERE Id=@Id";
                                    
                                    slParams.Add("@EntryId", inEnt.EntryId);
                                    slParams.Add("@ObrazProgramId", inEnt.ObrazProgramId);
                                    slParams.Add("@ProfileId", inEnt.ProfileId);
                                    _bdcPriemOnline.ExecuteQuery(query, slParams);
                                }
                            }
                        }

                        tran.Complete();
                    }
                }
                catch (Exception ex)
                {
                    WinFormsServ.Error(ex);
                }
                finally
                {
                    pf.Close();
                }
            }

            MessageBox.Show("Done!");
        }

        private void btnUpdateBaseDics_Click(object sender, EventArgs e)
        {
            UpdateBaseDics_WorkBase();
            UpdateBaseDics_OnlineBase();
        }
        private void UpdateBaseDics_WorkBase()
        {
            ProgressForm pf = new ProgressForm();
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    pf.Show();

                    pf.SetProgressText("Загрузка SP_Faculty...");
                    string query = @"SELECT [Id]
      ,[Name]
      ,[NameEng]
      FROM ed.SP_Faculty";
                    DataTable tbl = _bdcEduc.GetDataSet(query).Tables[0];
                    List<SP_Faculty> facList_Educ =
                        (from DataRow rw in tbl.Rows
                         select new SP_Faculty()
                         {
                             Id = rw.Field<int>("Id"),
                             Name = rw.Field<string>("Name"),
                             NameEng = rw.Field<string>("NameEng"),
                         }).ToList();

                    List<SP_Faculty> facList_Priem = context.SP_Faculty.ToList();

                    //check only ID
                    List<int> lst = facList_Educ.Select(x => x.Id).Except(facList_Priem.Select(x => x.Id)).ToList();
                    if (lst.Count > 0)
                    {
                        int i = 1;
                        pf.PrBarValue = 0;
                        pf.MaxPrBarValue = lst.Count;
                        foreach (int lId in lst)
                        {
                            pf.SetProgressText("Загрузка SP_Faculty..." + i++ + "/" + lst.Count);
                            var Fac = facList_Educ.Where(x => x.Id == lId).First();
                            context.SP_Faculty.Add(Fac);
                            context.SaveChanges();
                            pf.PerformStep();
                        }
                    }

                    pf.SetProgressText("Загрузка SP_LicenseProgram...");
                    query = @"SELECT [Id]
      ,[Name]
      ,[NameEng]
      ,[Code]
      ,[StudyLevelId]
      ,[ProgramTypeId]
      ,[PositionNum]
      ,[NormativePeriod]
      ,[QualificationId]
      ,[AggregateGroupId]
      FROM ed.SP_LicenseProgram";
                    tbl = _bdcEduc.GetDataSet(query).Tables[0];
                    List<SP_LicenseProgram> lpList_Educ =
                        (from DataRow rw in tbl.Rows
                         select new SP_LicenseProgram()
                         {
                             Id = rw.Field<int>("Id"),
                             Name = rw.Field<string>("Name"),
                             NameEng = rw.Field<string>("NameEng"),
                             Code = rw.Field<string>("Code"),
                             StudyLevelId = rw.Field<int>("StudyLevelId"),
                             ProgramTypeId = rw.Field<int>("ProgramTypeId"),
                             PositionNum = rw.Field<string>("PositionNum"),
                             NormativePeriod = rw.Field<string>("NormativePeriod"),
                             QualificationId = rw.Field<int?>("QualificationId"),
                             AggregateGroupId = rw.Field<int>("AggregateGroupId"),
                         }).ToList();

                    var lstSL = context.StudyLevel.Select(x => x.Id);
                    lpList_Educ = lpList_Educ.Where(x => lstSL.Contains(x.StudyLevelId)).ToList();

                    List<SP_LicenseProgram> lpList_Priem = context.SP_LicenseProgram.ToList();

                    //check only ID
                    lst = lpList_Educ.Select(x => x.Id).Except(lpList_Priem.Select(x => x.Id)).ToList();
                    if (lst.Count > 0)
                    {
                        int i = 1;
                        pf.PrBarValue = 0;
                        pf.MaxPrBarValue = lst.Count;
                        foreach (int lId in lst)
                        {
                            pf.SetProgressText("Загрузка SP_LicenseProgram..." + i++ + "/" + lst.Count);
                            var LP = lpList_Educ.Where(x => x.Id == lId).First();
                            LP.IsOpen = false;
                            LP.Holder = "";
                            context.SP_LicenseProgram.Add(LP);
                            context.SaveChanges();
                            pf.PerformStep();
                        }
                    }

                    pf.SetProgressText("Загрузка SP_ObrazProgram...");
                    query = @"SELECT [Id]
      ,[Name]
      ,[NameEng]
      ,[Number]
      ,[LicenseProgramId]
      ,[FacultyId]
      ,[ProgramModeId]
      ,[IsExpress]
      FROM ed.SP_ObrazProgram";

                    tbl = _bdcEduc.GetDataSet(query).Tables[0];
                    List<SP_ObrazProgram> opList_Educ =
                        (from DataRow rw in tbl.Rows
                         select new SP_ObrazProgram()
                         {
                             Id = rw.Field<int>("Id"),
                             Name = rw.Field<string>("Name"),
                             NameEng = rw.Field<string>("NameEng"),
                             Number = rw.Field<string>("Number"),
                             LicenseProgramId = rw.Field<int>("LicenseProgramId"),
                             FacultyId = rw.Field<int>("FacultyId"),
                             ProgramModeId = rw.Field<int>("ProgramModeId"),
                             IsExpress = rw.Field<bool>("IsExpress")
                         }).ToList();
                    var lstLP = context.SP_LicenseProgram.Select(x => x.Id);

                    opList_Educ = opList_Educ.Where(x => lstLP.Contains(x.LicenseProgramId)).ToList();

                    List<SP_ObrazProgram> opList_Priem = context.SP_ObrazProgram.ToList();

                    //check only ID
                    lst = opList_Educ.Select(x => x.Id).Except(opList_Priem.Select(x => x.Id)).ToList();
                    if (lst.Count > 0)
                    {
                        pf.PrBarValue = 0;
                        pf.MaxPrBarValue = lst.Count;
                        int i = 1;
                        foreach (int lId in lst)
                        {
                            pf.SetProgressText("Загрузка SP_ObrazProgram..." + i++ + "/" + lst.Count);
                            var OP = opList_Educ.Where(x => x.Id == lId).First();
                            OP.IsOpen = false;
                            OP.Holder = "";
                            context.SP_ObrazProgram.Add(OP);
                            context.SaveChanges();
                            pf.PerformStep();
                        }
                    }

                    pf.SetProgressText("Загрузка SP_Profile...");
                    query = @"SELECT [Id]
      ,[Name]
      ,[NameEng]
      ,[Acronym]
      ,[AcronymEng]
      FROM ed.[SP_Profile]";
                    tbl = _bdcEduc.GetDataSet(query).Tables[0];
                    List<SP_Profile> profList_Educ =
                        (from DataRow rw in tbl.Rows
                         select new SP_Profile()
                         {
                             Id = rw.Field<int>("Id"),
                             Name = rw.Field<string>("Name"),
                             NameEng = rw.Field<string>("NameEng"),
                             Acronym = rw.Field<string>("Acronym"),
                             AcronymEng = rw.Field<string>("AcronymEng")
                         }).ToList();

                    List<SP_Profile> profList_Priem = context.SP_Profile.ToList();

                    //check only ID
                    lst = profList_Educ.Select(x => x.Id).Except(profList_Priem.Select(x => x.Id)).ToList();
                    if (lst.Count > 0)
                    {
                        pf.PrBarValue = 0;
                        pf.MaxPrBarValue = lst.Count;
                        int i = 1;
                        foreach (int lId in lst)
                        {
                            pf.SetProgressText("Загрузка SP_Profile..." + i++ + "/" + lst.Count);
                            var Prof = profList_Educ.Where(x => x.Id == lId).First();
                            Prof.IsOpen = false;
                            Prof.Holder = "";
                            context.SP_Profile.Add(Prof);
                            context.SaveChanges();
                            pf.PerformStep();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error(ex);
            }
            finally
            {
                pf.Close();
            }
        }
        private void UpdateBaseDics_OnlineBase()
        {
            ProgressForm pf = new ProgressForm();
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    pf.Show();

                    pf.SetProgressText("Загрузка SP_Faculty...");
                    string query = @"SELECT [Id] FROM SP_Faculty";
                    DataTable tbl = _bdcPriemOnline.GetDataSet(query).Tables[0];
                    var facList_Online =
                        (from DataRow rw in tbl.Rows
                         select rw.Field<int>("Id")).ToList();

                    List<SP_Faculty> facList_Priem = context.SP_Faculty.ToList();

                    //check only ID
                    List<int> lst = facList_Priem.Select(x => x.Id).Except(facList_Online).ToList();
                    if (lst.Count > 0)
                    {
                        int i = 1;
                        pf.PrBarValue = 0;
                        pf.MaxPrBarValue = lst.Count;
                        foreach (int lId in lst)
                        {
                            pf.SetProgressText("Загрузка SP_Faculty..." + i++ + "/" + lst.Count);
                            var Fac = facList_Priem.Where(x => x.Id == lId).First();
                            query = "INSERT INTO SP_Faculty (Id, Name, NameEng) VALUES (@Id, @Name, @NameEng)";
                            SortedList<string, object> slParams = new SortedList<string, object>();
                            slParams.Add("@Id", Fac.Id);
                            slParams.AddVal("@Name", Fac.Name);
                            slParams.AddVal("@NameEng", Fac.NameEng);
                            _bdcPriemOnline.ExecuteQuery(query, slParams);
                            pf.PerformStep();
                        }
                    }

                    pf.SetProgressText("Загрузка SP_LicenseProgram...");
                    query = @"SELECT [Id] FROM SP_LicenseProgram";
                    tbl = _bdcPriemOnline.GetDataSet(query).Tables[0];
                    var lpList_Online =
                        (from DataRow rw in tbl.Rows
                         select rw.Field<int>("Id")).ToList();

                    List<SP_LicenseProgram> lpList_Priem = context.SP_LicenseProgram.ToList();

                    //check only ID
                    lst = lpList_Priem.Select(x => x.Id).Except(lpList_Online).ToList();
                    if (lst.Count > 0)
                    {
                        int i = 1;
                        pf.PrBarValue = 0;
                        pf.MaxPrBarValue = lst.Count;
                        foreach (int lId in lst)
                        {
                            pf.SetProgressText("Загрузка SP_LicenseProgram..." + i++ + "/" + lst.Count);
                            var LP = lpList_Priem.Where(x => x.Id == lId).First();
                            query = @"INSERT INTO SP_LicenseProgram (Id, Name, NameEng, Code, StudyLevelId, ProgramTypeId, PositionNum, NormativePeriod, QualificationId, AggregateGroupId) 
                            VALUES (@Id, @Name, @NameEng, @Code, @StudyLevelId, @ProgramTypeId, @PositionNum, @NormativePeriod, @QualificationId, @AggregateGroupId)";
                            SortedList<string, object> slParams = new SortedList<string, object>();
                            slParams.Add("@Id", LP.Id);
                            slParams.AddVal("@Name", LP.Name);
                            slParams.AddVal("@NameEng", LP.NameEng);
                            slParams.AddVal("@Code", LP.Code);
                            slParams.AddVal("@StudyLevelId", LP.StudyLevelId);
                            slParams.AddVal("@ProgramTypeId", LP.ProgramTypeId);
                            slParams.AddVal("@PositionNum", LP.PositionNum);
                            slParams.AddVal("@NormativePeriod", LP.NormativePeriod);
                            slParams.AddVal("@QualificationId", LP.QualificationId);
                            slParams.AddVal("@AggregateGroupId", LP.AggregateGroupId);
                            _bdcPriemOnline.ExecuteQuery(query, slParams);
                            pf.PerformStep();
                        }
                    }

                    pf.SetProgressText("Загрузка SP_ObrazProgram...");
                    query = @"SELECT [Id] FROM SP_ObrazProgram";
                    tbl = _bdcPriemOnline.GetDataSet(query).Tables[0];
                    var opList_Online =
                        (from DataRow rw in tbl.Rows
                         select rw.Field<int>("Id")).ToList();

                    List<SP_ObrazProgram> opList_Priem = context.SP_ObrazProgram.ToList();

                    //check only ID
                    lst = opList_Priem.Select(x => x.Id).Except(opList_Online).ToList();
                    if (lst.Count > 0)
                    {
                        pf.PrBarValue = 0;
                        pf.MaxPrBarValue = lst.Count;
                        int i = 1;
                        foreach (int lId in lst)
                        {
                            pf.SetProgressText("Загрузка SP_ObrazProgram..." + i++ + "/" + lst.Count);
                            var OP = opList_Priem.Where(x => x.Id == lId).First();
                            query = "INSERT INTO SP_ObrazProgram (Id, Name, NameEng, Number, LicenseProgramId, FacultyId, ProgramModeId, IsExpress) VALUES (@Id, @Name, @NameEng, @Number, @LicenseProgramId, @FacultyId, @ProgramModeId, @IsExpress)";
                            SortedList<string, object> slParams = new SortedList<string, object>();
                            slParams.Add("@Id", OP.Id);
                            slParams.AddVal("@Name", OP.Name);
                            slParams.AddVal("@NameEng", OP.NameEng);
                            slParams.AddVal("@Number", OP.Number);
                            slParams.AddVal("@LicenseProgramId", OP.LicenseProgramId);
                            slParams.AddVal("@FacultyId", OP.FacultyId);
                            slParams.AddVal("@ProgramModeId", OP.ProgramModeId);
                            slParams.AddVal("@IsExpress", OP.IsExpress);
                            _bdcPriemOnline.ExecuteQuery(query, slParams);
                            pf.PerformStep();
                        }
                    }

                    pf.SetProgressText("Загрузка SP_Profile...");
                    query = @"SELECT [Id] FROM [SP_Profile]";
                    tbl = _bdcPriemOnline.GetDataSet(query).Tables[0];
                    var profList_Online =
                        (from DataRow rw in tbl.Rows
                         select rw.Field<int>("Id")
                         ).ToList();

                    List<SP_Profile> profList_Priem = context.SP_Profile.ToList();

                    //check only ID
                    lst = profList_Priem.Select(x => x.Id).Except(profList_Online).ToList();
                    if (lst.Count > 0)
                    {
                        pf.PrBarValue = 0;
                        pf.MaxPrBarValue = lst.Count;
                        int i = 1;
                        foreach (int lId in lst)
                        {
                            pf.SetProgressText("Загрузка SP_Profile..." + i++ + "/" + lst.Count);
                            var Prof = profList_Priem.Where(x => x.Id == lId).First();
                            query = "INSERT INTO SP_Profile (Id, Name, NameEng, Acronym, AcronymEng) VALUES (@Id, @Name, @NameEng, @Acronym, @AcronymEng)";
                            SortedList<string, object> slParams = new SortedList<string, object>();
                            slParams.Add("@Id", Prof.Id);
                            slParams.AddVal("@Name", Prof.Name);
                            slParams.AddVal("@NameEng", Prof.NameEng);
                            slParams.AddVal("@Acronym", Prof.Acronym);
                            slParams.AddVal("@AcronymEng", Prof.AcronymEng);
                            _bdcPriemOnline.ExecuteQuery(query, slParams);
                            pf.PerformStep();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error(ex);
            }
            finally
            {
                pf.Close();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _bdcPriemOnline.CloseDataBase();
            _bdcEduc.CloseDataBase();
        }

        private void btnCopyToCrimea_Click(object sender, EventArgs e)
        {
            ProgressForm pf = new ProgressForm();
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    pf.Show();
                    
                    var lstEntry = context.Entry.Where(x => !x.IsCrimea && !x.IsForeign)
                        .Select(x => new
                        {
                            x.Id,
                            x.LicenseProgramId,
                            x.ObrazProgramId,
                            x.ProfileId,
                            x.KCP,
                            x.IsSecond,
                            x.IsReduced,
                            x.IsParallel,
                            x.StudyLevelId,
                            x.StudyPlanId,
                            x.StudyPlanNumber,
                            x.StudyBasisId,
                            x.StudyFormId,
                            x.FacultyId,
                            x.DateOfStart,
                            x.DateOfClose
                        }).ToList();
                    pf.MaxPrBarValue = lstEntry.Count;
                    foreach (var Ent in lstEntry)
                    {
                        pf.PerformStep();
                        int cnt = context.Entry
                            .Where(x => x.LicenseProgramId == Ent.LicenseProgramId
                                && x.ObrazProgramId == Ent.ObrazProgramId
                                && x.ProfileId == Ent.ProfileId
                                && x.IsSecond == Ent.IsSecond
                                && x.IsReduced == Ent.IsReduced
                                && x.IsParallel == Ent.IsParallel
                                && x.StudyLevelId == Ent.StudyLevelId
                                && x.StudyBasisId == Ent.StudyBasisId
                                && x.StudyFormId == Ent.StudyFormId
                                && x.FacultyId == Ent.FacultyId
                                && x.IsCrimea && !x.IsForeign
                            ).Count();

                        if (cnt == 0)
                        {
                            var lstInnerEntriesInEntry = context.InnerEntryInEntry.Where(x => x.EntryId == Ent.Id)
                                .Select(x => new { x.EgeExamNameId, x.EgeMin, x.KCP, x.ObrazProgramId, x.ParentInnerEntryInEntryId, x.ProfileId })
                                .ToList();

                            using (TransactionScope tran = new TransactionScope())
                            {
                                Guid gId = Guid.NewGuid();
                                var CrEnt = new Entry();
                                CrEnt.Id = gId;
                                CrEnt.LicenseProgramId = Ent.LicenseProgramId;
                                CrEnt.ObrazProgramId = Ent.ObrazProgramId;
                                CrEnt.ProfileId = Ent.ProfileId;
                                CrEnt.IsSecond = Ent.IsSecond;
                                CrEnt.IsReduced = Ent.IsReduced;
                                CrEnt.IsParallel = Ent.IsParallel;
                                CrEnt.StudyLevelId = Ent.StudyLevelId;
                                CrEnt.StudyBasisId = Ent.StudyBasisId;
                                CrEnt.StudyFormId = Ent.StudyFormId;
                                CrEnt.FacultyId = Ent.FacultyId;
                                CrEnt.IsCrimea = true;
                                CrEnt.IsForeign = false;
                                CrEnt.StudyPlanId = Ent.StudyPlanId;
                                CrEnt.StudyPlanNumber = Ent.StudyPlanNumber;

                                CrEnt.DateOfStart = Ent.DateOfStart;
                                CrEnt.DateOfClose = Ent.DateOfClose;

                                context.Entry.Add(CrEnt);
                                context.SaveChanges();

                                string query = @"INSERT INTO _Entry 
    ( 
     Id,
     LicenseProgramId,
     ObrazProgramId,
     ProfileId,
     IsSecond,
     IsReduced,
     IsParallel,
     StudyLevelId,
     StudyPlanId,
     StudyPlanNumber,
     StudyBasisId,
     StudyFormId,
     FacultyId,
     DateOfStart,
     DateOfClose,
     IsCrimea,
     IsForeign,
     SemesterId,
     CampaignYear
    )
    VALUES
    (
     @Id,
     @LicenseProgramId,
     @ObrazProgramId,
     @ProfileId,
     @IsSecond,
     @IsReduced,
     @IsParallel,
     @StudyLevelId,
     @StudyPlanId,
     @StudyPlanNumber,
     @StudyBasisId,
     @StudyFormId,
     @FacultyId,
     @DateOfStart,
     @DateOfClose,
     1,
     0,
     1,
     @CampaignYear
    )";
                                SortedList<string, object> slParams = new SortedList<string, object>();
                                slParams.Add("@Id", gId);
                                slParams.Add("@LicenseProgramId", Ent.LicenseProgramId);
                                slParams.Add("@ObrazProgramId", Ent.ObrazProgramId);
                                slParams.Add("@ProfileId", Ent.ProfileId);
                                slParams.Add("@IsSecond", Ent.IsSecond);
                                slParams.Add("@IsReduced", Ent.IsReduced);
                                slParams.Add("@IsParallel", Ent.IsParallel);
                                slParams.Add("@StudyLevelId", Ent.StudyLevelId);
                                slParams.AddVal("@StudyPlanId", Ent.StudyPlanId);
                                slParams.AddVal("@StudyPlanNumber", Ent.StudyPlanNumber);
                                slParams.Add("@StudyBasisId", Ent.StudyBasisId);
                                slParams.Add("@StudyFormId", Ent.StudyFormId);
                                slParams.Add("@FacultyId", Ent.FacultyId);
                                slParams.Add("@DateOfStart", Ent.DateOfStart);
                                slParams.Add("@DateOfClose", Ent.DateOfClose);
                                slParams.Add("@CampaignYear", MainClass.iPriemYear);

                                _bdcPriemOnline.ExecuteQuery(query, slParams);


                                foreach (var InEnt in lstInnerEntriesInEntry)
                                {
                                    Guid IEIE_Id = Guid.NewGuid();
                                    InnerEntryInEntry IEIE = new InnerEntryInEntry();
                                    IEIE.Id = IEIE_Id;
                                    IEIE.EgeExamNameId = InEnt.EgeExamNameId;
                                    IEIE.EgeMin = InEnt.EgeMin;
                                    IEIE.EntryId = gId;
                                    IEIE.KCP = InEnt.KCP;
                                    IEIE.ObrazProgramId = InEnt.ObrazProgramId;
                                    IEIE.ParentInnerEntryInEntryId = InEnt.ParentInnerEntryInEntryId;
                                    IEIE.ProfileId = InEnt.ProfileId;

                                    context.InnerEntryInEntry.Add(IEIE);
                                    context.SaveChanges();

                                    query = @"
INSERT INTO InnerEntryInEntry
(
     Id
    ,EntryId
    ,ObrazProgramId
    ,ProfileId
)
VALUES
(
     @Id
    ,@EntryId
    ,@ObrazProgramId
    ,@ProfileId
)";
                                    slParams.Clear();
                                    slParams.Add("@Id", IEIE_Id);
                                    slParams.Add("@EntryId", gId);
                                    slParams.Add("@ObrazProgramId", InEnt.ObrazProgramId);
                                    slParams.Add("@ProfileId", InEnt.ProfileId);

                                    _bdcPriemOnline.ExecuteQuery(query, slParams);
                                }

                                tran.Complete();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error(ex);
            }
            finally
            {
                pf.Close();
            }
        }
        private void btnCopyToForeign_Click(object sender, EventArgs e)
        {
            ProgressForm pf = new ProgressForm();
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    pf.Show();

                    var lstEntry = context.Entry.Where(x => !x.IsCrimea && !x.IsForeign)
                        .Select(x => new
                        {
                            x.Id,
                            x.LicenseProgramId,
                            x.ObrazProgramId,
                            x.ProfileId,
                            x.KCP,
                            x.IsSecond,
                            x.IsReduced,
                            x.IsParallel,
                            x.StudyLevelId,
                            x.StudyPlanId,
                            x.StudyPlanNumber,
                            x.StudyBasisId,
                            x.StudyFormId,
                            x.FacultyId,
                            x.DateOfStart,
                            x.DateOfClose
                        }).ToList();
                    pf.MaxPrBarValue = lstEntry.Count;
                    foreach (var Ent in lstEntry)
                    {
                        pf.PerformStep();
                        int cnt = context.Entry
                            .Where(x => x.LicenseProgramId == Ent.LicenseProgramId
                                && x.ObrazProgramId == Ent.ObrazProgramId
                                && x.ProfileId == Ent.ProfileId
                                && x.IsSecond == Ent.IsSecond
                                && x.IsReduced == Ent.IsReduced
                                && x.IsParallel == Ent.IsParallel
                                && x.StudyLevelId == Ent.StudyLevelId
                                && x.StudyBasisId == Ent.StudyBasisId
                                && x.StudyFormId == Ent.StudyFormId
                                && x.FacultyId == Ent.FacultyId
                                && !x.IsCrimea && x.IsForeign
                            ).Count();

                        if (cnt == 0)
                        {
                            //var lstInnerEntriesInEntry = context.InnerEntryInEntry.Where(x => x.EntryId == Ent.Id)
                            //    .Select(x => new { x.EgeExamNameId, x.EgeMin, x.KCP, x.ObrazProgramId, x.ParentInnerEntryInEntryId, x.ProfileId })
                            //    .ToList();

                            using (TransactionScope tran = new TransactionScope())
                            {
                                Guid gId = Guid.NewGuid();
                                var CrEnt = new Entry();
                                CrEnt.Id = gId;
                                CrEnt.LicenseProgramId = Ent.LicenseProgramId;
                                CrEnt.ObrazProgramId = Ent.ObrazProgramId;
                                CrEnt.ProfileId = Ent.ProfileId;
                                CrEnt.IsSecond = Ent.IsSecond;
                                CrEnt.IsReduced = Ent.IsReduced;
                                CrEnt.IsParallel = Ent.IsParallel;
                                CrEnt.StudyLevelId = Ent.StudyLevelId;
                                CrEnt.StudyBasisId = Ent.StudyBasisId;
                                CrEnt.StudyFormId = Ent.StudyFormId;
                                CrEnt.FacultyId = Ent.FacultyId;
                                CrEnt.IsCrimea = false;
                                CrEnt.IsForeign = true;
                                CrEnt.StudyPlanId = Ent.StudyPlanId;
                                CrEnt.StudyPlanNumber = Ent.StudyPlanNumber;

                                CrEnt.DateOfStart = Ent.DateOfStart;
                                CrEnt.DateOfClose = Ent.DateOfClose;

                                context.Entry.Add(CrEnt);
                                context.SaveChanges();

                                string query = @"INSERT INTO _Entry 
    ( 
     Id,
     LicenseProgramId,
     ObrazProgramId,
     ProfileId,
     IsSecond,
     IsReduced,
     IsParallel,
     StudyLevelId,
     StudyPlanId,
     StudyPlanNumber,
     StudyBasisId,
     StudyFormId,
     FacultyId,
     DateOfStart,
     DateOfClose,
     IsCrimea,
     IsForeign,
     SemesterId,
     CampaignYear
    )
    VALUES
    (
     @Id,
     @LicenseProgramId,
     @ObrazProgramId,
     @ProfileId,
     @IsSecond,
     @IsReduced,
     @IsParallel,
     @StudyLevelId,
     @StudyPlanId,
     @StudyPlanNumber,
     @StudyBasisId,
     @StudyFormId,
     @FacultyId,
     @DateOfStart,
     @DateOfClose,
     0,
     1,
     1,
     @CampaignYear
    )";
                                SortedList<string, object> slParams = new SortedList<string, object>();
                                slParams.Add("@Id", gId);
                                slParams.Add("@LicenseProgramId", Ent.LicenseProgramId);
                                slParams.Add("@ObrazProgramId", Ent.ObrazProgramId);
                                slParams.Add("@ProfileId", Ent.ProfileId);
                                slParams.Add("@IsSecond", Ent.IsSecond);
                                slParams.Add("@IsReduced", Ent.IsReduced);
                                slParams.Add("@IsParallel", Ent.IsParallel);
                                slParams.Add("@StudyLevelId", Ent.StudyLevelId);
                                slParams.AddVal("@StudyPlanId", Ent.StudyPlanId);
                                slParams.AddVal("@StudyPlanNumber", Ent.StudyPlanNumber);
                                slParams.Add("@StudyBasisId", Ent.StudyBasisId);
                                slParams.Add("@StudyFormId", Ent.StudyFormId);
                                slParams.Add("@FacultyId", Ent.FacultyId);
                                slParams.Add("@DateOfStart", Ent.DateOfStart);
                                slParams.Add("@DateOfClose", Ent.DateOfClose);
                                slParams.Add("@CampaignYear", MainClass.iPriemYear);

                                _bdcPriemOnline.ExecuteQuery(query, slParams);

                                tran.Complete();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error(ex);
            }
            finally
            {
                pf.Close();
            }
        }

        private void btnLoadUpdateOlympiads_Click(object sender, EventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lstOlympName = context.OlympName.Select(x => new { x.Id, x.Name }).ToList();
                foreach (var Ent in lstOlympName)
                {
                    string query = "SELECT COUNT(*) FROM OlympName WHERE Id=@Id";
                    SortedList<string, object> slParams = new SortedList<string, object>() { { "@Id", Ent.Id }, { "@Name", Ent.Name } };
                    int cnt = (int)MainClass.BdcOnlineReadWrite.GetValue(query, slParams);
                    if (cnt == 0)
                        MainClass.BdcOnlineReadWrite.ExecuteQuery("INSERT INTO OlympName (Id, Name) VALUES (@Id, @Name)", slParams);
                    else
                        MainClass.BdcOnlineReadWrite.ExecuteQuery("UPDATE OlympName SET Name=@Name WHERE Id=@Id", slParams);
                }

                var lstOlympSubject = context.OlympSubject.Select(x => new { x.Id, x.Name }).ToList();
                foreach (var Ent in lstOlympSubject)
                {
                    string query = "SELECT COUNT(*) FROM OlympSubject WHERE Id=@Id";
                    SortedList<string, object> slParams = new SortedList<string, object>() { { "@Id", Ent.Id }, { "@Name", Ent.Name } };
                    int cnt = (int)MainClass.BdcOnlineReadWrite.GetValue(query, slParams);
                    if (cnt == 0)
                        MainClass.BdcOnlineReadWrite.ExecuteQuery("INSERT INTO OlympSubject (Id, Name) VALUES (@Id, @Name)", slParams);
                    else
                        MainClass.BdcOnlineReadWrite.ExecuteQuery("UPDATE OlympSubject SET Name=@Name WHERE Id=@Id", slParams);
                }

                var lstOlympProfile = context.OlympProfile.Select(x => new { x.Id, x.Name }).ToList();
                foreach (var Ent in lstOlympProfile)
                {
                    string query = "SELECT COUNT(*) FROM OlympProfile WHERE Id=@Id";
                    SortedList<string, object> slParams = new SortedList<string, object>() { { "@Id", Ent.Id }, { "@Name", Ent.Name } };
                    int cnt = (int)MainClass.BdcOnlineReadWrite.GetValue(query, slParams);
                    if (cnt == 0)
                        MainClass.BdcOnlineReadWrite.ExecuteQuery("INSERT INTO OlympProfile (Id, Name) VALUES (@Id, @Name)", slParams);
                    else
                        MainClass.BdcOnlineReadWrite.ExecuteQuery("UPDATE OlympProfile SET Name=@Name WHERE Id=@Id", slParams);
                }

                var lstOlympBook = context.OlympBook.Select(x => new { x.Id, x.OlympLevelId, x.OlympNameId, x.OlympProfileId, x.OlympSubjectId, x.OlympTypeId, x.OlympYear }).ToList();
                foreach (var Ent in lstOlympBook)
                {
                    string query = "SELECT COUNT(*) FROM OlympBook WHERE Id=@Id";
                    SortedList<string, object> slParams = new SortedList<string, object>() { 
                        { "@Id", Ent.Id }, 
                        { "@OlympLevelId", Ent.OlympLevelId },
                        { "@OlympNameId", Ent.OlympNameId },
                        { "@OlympProfileId", Ent.OlympProfileId },
                        { "@OlympSubjectId", Ent.OlympSubjectId },
                        { "@OlympTypeId", Ent.OlympTypeId },
                        { "@OlympYear", Ent.OlympYear }
                    };
                    int cnt = (int)MainClass.BdcOnlineReadWrite.GetValue(query, slParams);
                    if (cnt == 0)
                        MainClass.BdcOnlineReadWrite.ExecuteQuery(@"INSERT INTO OlympBook (Id, OlympLevelId, OlympNameId, OlympProfileId, OlympSubjectId, OlympTypeId, OlympYear) 
VALUES (@Id, @OlympLevelId, @OlympNameId, @OlympProfileId, @OlympSubjectId, @OlympTypeId, @OlympYear)", slParams);
                    else
                        MainClass.BdcOnlineReadWrite.ExecuteQuery(@"UPDATE OlympBook 
SET OlympLevelId=@OlympLevelId,
    OlympNameId=@OlympNameId,
    OlympProfileId=@OlympProfileId,
    OlympSubjectId=@OlympSubjectId,
    OlympTypeId=@OlympTypeId,
    OlympYear=@OlympYear
WHERE Id=@Id", slParams);
                }
            }
        }

        private void btnLoadUpdateExams_Click(object sender, EventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lstExamNames = context.ExamName.Select(x => new { x.Id, x.Name }).ToList();
                foreach (var ExName in lstExamNames)
                {
                    string query = "SELECT COUNT(*) FROM ExamName WHERE Id=@Id";
                    SortedList<string, object> slParams = new SortedList<string, object>() { { "@Id", ExName.Id }, { "@Name", ExName.Name } };
                    int cnt = (int)MainClass.BdcOnlineReadWrite.GetValue(query, slParams);
                    if (cnt == 0)
                        MainClass.BdcOnlineReadWrite.ExecuteQuery("INSERT INTO ExamName (Id, Name) VALUES (@Id, @Name)", slParams);
                    else
                        MainClass.BdcOnlineReadWrite.ExecuteQuery("UPDATE ExamName SET Name=@Name WHERE Id=@Id", slParams);
                }

                var lstExams = context.Exam.Select(x => new { x.Id, x.ExamNameId, x.IsAdditional, x.IsPortfolioAnonymPart, x.IsPortfolioCommonPart }).ToList();
                foreach (var Exam in lstExams)
                {
                    string query = "SELECT COUNT(*) FROM Exam WHERE Id=@Id";
                    SortedList<string, object> slParams = new SortedList<string, object>() {
                        { "@Id", Exam.Id },
                        { "@ExamNameId", Exam.ExamNameId },
                        { "@IsPortfolio", Exam.IsPortfolioAnonymPart },
                        { "@IsPortfolioCommonPart", Exam.IsPortfolioCommonPart },
                        { "@IsAdditional", Exam.IsAdditional }
                    };
                    int cnt = (int)MainClass.BdcOnlineReadWrite.GetValue(query, slParams);
                    if (cnt == 0)
                        MainClass.BdcOnlineReadWrite.ExecuteQuery("INSERT INTO Exam (Id, ExamNameId, IsPortfolio) VALUES (@Id, @ExamNameId, @IsPortfolio)", slParams);
                    else
                        MainClass.BdcOnlineReadWrite.ExecuteQuery("UPDATE Exam SET ExamNameId=@ExamNameId, IsPortfolio=@IsPortfolio WHERE Id=@Id", slParams);
                }
            }
        }
    }
}
