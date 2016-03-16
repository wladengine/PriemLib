using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


using BaseFormsLib;
using EducServLib;

namespace PriemLib
{
    public partial class CardExamTimeTableAbitList : Form
    {
        int? StudyLevelId
        {
            get { return ComboServ.GetComboIdInt(cbStudyLevel); }
            set { ComboServ.SetComboId(cbStudyLevel, value); }
        }
        public int? LicenseProgramId
        {
            get { return ComboServ.GetComboIdInt(cbLicenseProgram); }
            set { ComboServ.SetComboId(cbLicenseProgram, value); }
        }
        public int? ObrazProgramId
        {
            get { return ComboServ.GetComboIdInt(cbObrazProgram); }
            set { ComboServ.SetComboId(cbObrazProgram, value); }
        }
        public int? TimeTableId
        {
            get { return ComboServ.GetComboIdInt(cbExamTimeTable); }
            set { ComboServ.SetComboId(cbExamTimeTable, value); }
        }
        public string ExamenBlockId
        {
            get 
            {
                string tmp = ComboServ.GetComboId(cbExamenBlock);
                if (tmp == ComboServ.ALL_VALUE)
                    return null;
                else
                    return tmp;
            }
            set { ComboServ.SetComboId(cbExamenBlock, value); }
        }
        public string ExamenUnitId
        {
            get 
            { 
                string tmp = ComboServ.GetComboId(cbExamenUnit);
                if (tmp == ComboServ.ALL_VALUE)
                    return null;
                else
                    return tmp;
            }
            set { ComboServ.SetComboId(cbExamenUnit, value); }
        }
        public CardExamTimeTableAbitList()
        {
            InitializeComponent();
            this.MdiParent = MainClass.mainform;
            this.Text = "Расписание экзаменов";
            FillCard(); 
        }

        private void FillCard()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    List<KeyValuePair<string, string>> lst =
                        ((from ent in GetEntry(context)
                          orderby ent.StudyLevelId
                          select new
                          {
                              Id = ent.StudyLevelId,
                              Name = ent.StudyLevelName,
                          }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                    ComboServ.FillCombo(cbStudyLevel, lst, false, true);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы FillCard", exc);
            }
        }
        private void FillDataGrid()
        {
            string query = @"
SELECT  Application.Id
, Person.Surname as 'Фамилия'
, Person.Name as  'Имя'
, Person.SecondName as  'Отчество'
, ExamName.Name as 'Экзамен'
,Entry.StudyLevelName as 'Уровень'
, Entry.LicenseProgramName as 'Профиль'
, ExamTimetable.ExamDate as 'Дата экзамена'
, ExamTimetable.Address as 'Место проведения'
, ApplicationSelectedExam.RegistrationDate as 'Дата регистрации на экзамен'
  FROM dbo.Application
  join dbo.Person on Person.Id = Application.PersonId
  join dbo.ApplicationSelectedExam on Application.Id = ApplicationSelectedExam.ApplicationId
  join dbo.ExamTimetable on ExamTimetable.Id = ApplicationSelectedExam.ExamTimetableId
  join dbo.ExamInEntryBlockUnit on ApplicationSelectedExam.ExamInEntryBlockUnitId = ExamInEntryBlockUnit.Id
join dbo.ExamInEntryBlock on ExamInEntryBlockUnit.ExamInEntryBlockId = ExamInEntryBlock.Id
  join dbo.Exam on ExamInEntryBlockUnit.ExamId = Exam.id
  join dbo.ExamName on ExamName.Id = ExamNameId
  join dbo.Entry on Entry.Id = Application.EntryId

where 1=1  ";

            SortedList<string, object> dic = new SortedList<string, object>();
            if (TimeTableId.HasValue)
            {
                dic.Add("@TTid", TimeTableId);
                query += @" and Month(ExamTimetable.Examdate) = @TTid ";
            }
            if (!String.IsNullOrEmpty(ExamenUnitId))
            {
                dic.Add("@UnitId", ExamenUnitId);
                query += " and ExamName.Name = @UnitId ";
            }
            else if (!String.IsNullOrEmpty(ExamenBlockId))
            {
                dic.Add("@ExamInEntryBlock", ExamenBlockId);
                query += " and ExamInEntryBlock.Name = @ExamInEntryBlock ";
            }
            
            
            if (ObrazProgramId.HasValue)
            {
                dic.Add("@ObrazProgramId", ObrazProgramId);
                query += @" and Entry.ObrazProgramId = @ObrazProgramId";
            } else if (LicenseProgramId.HasValue)
            {
                dic.Add("@LicenseProgramId", LicenseProgramId);
                query += @" and Entry.LicenseProgramId = @LicenseProgramId";
            } else if (StudyLevelId.HasValue)
            {
                dic.Add("@StudyLevelId", StudyLevelId);
                query += @" and Entry.StudyLevelId = @StudyLevelId";
            }
            LoadFromInet load = new LoadFromInet();

            DataTable tbl = load.BDCInet.GetDataSet(query, dic).Tables[0];

            dgv.DataSource = tbl;
            if (dgv.Columns.Contains("Id"))
                dgv.Columns["Id"].Visible = false;
            lblCount.Text = dgv.Rows.Count.ToString();
        }

        private IEnumerable<qEntry> GetEntry(PriemEntities context)
        {
            IEnumerable<qEntry> entry = MainClass.GetEntry(context);

            entry = entry.Where(c => c.IsReduced == false);
            entry = entry.Where(c => c.IsParallel == false);
            entry = entry.Where(c => c.IsSecond == false);
            entry = entry.Where(c => c.IsForeign == false);
            entry = entry.Where(c => c.IsCrimea == false);

            return entry;
        }

        private void FillLicenseProgram()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    List<KeyValuePair<string, string>> lst =
                        ((from ent in GetEntry(context)
                          where (StudyLevelId.HasValue )? (ent.StudyLevelId == StudyLevelId) : (true)
                          orderby ent.LicenseProgramName
                          select new
                          {
                              Id = ent.LicenseProgramId,
                              Name = ent.LicenseProgramName,
                              Code = ent.LicenseProgramCode
                          }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name + ' ' + u.Code)).ToList();
                    ComboServ.FillCombo(cbLicenseProgram, lst, false, true);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы FillLicenseProgram", exc);
            }
        }
        private void FillObrazProgram()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    List<KeyValuePair<string, string>> lst =
                        ((from ent in GetEntry(context)
                          where (LicenseProgramId.HasValue ? ent.LicenseProgramId == LicenseProgramId : true )
                          && (StudyLevelId.HasValue ? ent.StudyLevelId == StudyLevelId : true)
                          orderby ent.ObrazProgramName
                          select new
                          {
                              Id = ent.ObrazProgramId,
                              Name = ent.ObrazProgramName,
                              Crypt = ent.ObrazProgramCrypt
                          }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name + ' ' + u.Crypt)).ToList();

                    ComboServ.FillCombo(cbObrazProgram, lst, false, true);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы FillObrazProgram", exc);
            }
        }
        private void FillExamenBlock()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    var lst =
                        (from ent in GetEntry(context)
                         where (LicenseProgramId.HasValue ? (ent.LicenseProgramId == LicenseProgramId) : true)
                          && (ObrazProgramId.HasValue ? ent.ObrazProgramId == ObrazProgramId : true)
                          && (StudyLevelId.HasValue ? ent.StudyLevelId == StudyLevelId : true)
                          orderby ent.ProfileName
                          select ent.Id).Distinct().ToList();
                    //if (lst.Count() == 1) 
                    //{
                    //    var blocks = (from bl in context.ExamInEntryBlock
                    //                  where lst.Contains(bl.EntryId) && bl.ParentExamInEntryBlockId == null 
                    //                  select new
                    //                  {
                    //                      bl.Id, bl.Name
                    //                  }).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();
                    //    ComboServ.FillCombo(cbExamenBlock, blocks, false, true);
                    //} 
                    //else 
                        if (lst.Count() > 0)
                    {
                        var blocks = (from bl in context.ExamInEntryBlock
                                      where lst.Contains(bl.EntryId) && bl.ParentExamInEntryBlockId == null
                                      select new
                                      {
                                          bl.Name
                                      }).Distinct().ToList().Select(u => new KeyValuePair<string, string>(u.Name.ToString(), u.Name)).ToList();
                        ComboServ.FillCombo(cbExamenBlock, blocks, false, true); 
                    }
                    else
                    {
                        MessageBox.Show("Entry не найден","");
                    }
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы FillExamenBlock", exc);
            }
        }
        private void FillExamenUnit()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    var blocks = (from un in context.ExamInEntryBlockUnit
                                  join bl in context.ExamInEntryBlock on un.ExamInEntryBlockId equals bl.Id
                                  join ent in context.Entry on bl.EntryId equals ent.Id
                                  join ex in context.Exam on un.ExamId equals ex.Id
                                  join exn in context.ExamName on ex.ExamNameId equals exn.Id
                                  where bl.ParentExamInEntryBlockId == null
                                  && (!String.IsNullOrEmpty(ExamenBlockId) ?  bl.Name.ToLower() == ExamenBlockId.ToLower() : true)
                                  && (LicenseProgramId.HasValue ? ent.LicenseProgramId == LicenseProgramId : true)
                                  && MainClass.lstStudyLevelGroupId.Contains(ent.StudyLevel.LevelGroupId)
                                  && (StudyLevelId.HasValue ? ent.StudyLevelId == StudyLevelId : true)
                                      select new
                                      {
                                          exn.Name
                                      }).Distinct().ToList().Select(u => new KeyValuePair<string, string>(u.Name, u.Name)).ToList();
                    
                    ComboServ.FillCombo(cbExamenUnit, blocks, false, true);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы FillExamenUnit", exc);
            }
        }
        private void FillExamTimeTable()
        {
            try
            {
                string query = @"select distinct
 MONTH(ExamDate) as [Key]
, DATENAME(month, ExamDate) +', ' + Address as [Value]
from dbo.ExamTimeTable 
join dbo.ExamInEntryBlockUnit on ExamInEntryBlockUnit.Id  = ExamTimeTable.ExamInEntryBlockUnitId
join dbo.Exam on Exam.Id = ExamInEntryBlockUnit.ExamId
join dbo.ExamName on ExamName.Id = Exam.ExamNameId
" + (!String.IsNullOrEmpty(ExamenUnitId) ? @"
where ExamName.Name = @Id " : "");

                LoadFromInet load = new LoadFromInet();
                SortedList<string, object> dic = new SortedList<string, object>();
                if (!String.IsNullOrEmpty(ExamenUnitId))
                    dic.Add("@Id", ExamenUnitId);
                DataTable tbl = load.BDCInet.GetDataSet(query, dic).Tables[0];
                List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
                foreach (DataRow t in tbl.Rows)
                {
                    lst.Add(new KeyValuePair<string, string>
                        (t.Field<int>("Key").ToString(), t.Field<string>("Value")));
                }
                ComboServ.FillCombo(cbExamTimeTable, lst, false, true);
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы FillExamenUnit", exc);
            }
        }

        private void cbStudyLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillLicenseProgram();
        }
        private void cbLicenseProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillObrazProgram();
        }
        private void cbObrazProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillExamenBlock();
        }
        private void cbExamenUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillExamTimeTable();
        }
        private void cbExamenBlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillExamenUnit();
        }

        private void btnFillGrid_Click(object sender, EventArgs e)
        {
            FillDataGrid();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0)
                return;
            try
            {
                PrintClass.PrintAllToExcel(dgv, false, "Экзамен");
            }
            catch (Exception ex)
            {
                WinFormsServ.Error(ex);
            }
        }
    }
}
