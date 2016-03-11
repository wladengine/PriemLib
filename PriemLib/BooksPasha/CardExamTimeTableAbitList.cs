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
        public Guid? ExamenBlockId
        {
            get { return ComboServ.GetComboIdGuid(cbExamenBlock); }
            set { ComboServ.SetComboId(cbExamenBlock, value); }
        }
        public Guid? ExamenUnitId
        {
            get { return ComboServ.GetComboIdGuid(cbExamenUnit); }
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

                    ComboServ.FillCombo(cbStudyLevel, lst, false, false);
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
, ExamTimetable.ExamDate as 'Дата экзамена'
, ExamTimetable.Address as 'Место проведения'
, ApplicationSelectedExam.RegistrationDate as 'Дата регистрации на экзамен'
  FROM dbo.Application
  join dbo.Person on Person.Id = Application.PersonId
  join dbo.ApplicationSelectedExam on Application.Id = ApplicationSelectedExam.ApplicationId
  join dbo.ExamTimetable on ExamTimetable.Id = ApplicationSelectedExam.ExamTimetableId
where " +
     (   TimeTableId.HasValue ? @" ExamTimetable.Id = @TTid " : " ApplicationSelectedExam.ExamInEntryBlockUnitId = @UnitId ");

            SortedList<string, object> dic = new SortedList<string, object>();
            if (TimeTableId.HasValue)
                dic.Add("@TTid", TimeTableId);
            dic.Add("@UnitId", ExamenUnitId);

            LoadFromInet load = new LoadFromInet();

            DataTable tbl = load.BDCInet.GetDataSet(query, dic).Tables[0];

            dgv.DataSource = tbl;
            if (dgv.Columns.Contains("Id"))
                dgv.Columns["Id"].Visible = false;
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
                          where ent.StudyLevelId == StudyLevelId
                          orderby ent.LicenseProgramName
                          select new
                          {
                              Id = ent.LicenseProgramId,
                              Name = ent.LicenseProgramName,
                              Code = ent.LicenseProgramCode
                          }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name + ' ' + u.Code)).ToList();

                    ComboServ.FillCombo(cbLicenseProgram, lst, false, false);
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
                          where ent.LicenseProgramId == LicenseProgramId
                          orderby ent.ObrazProgramName
                          select new
                          {
                              Id = ent.ObrazProgramId,
                              Name = ent.ObrazProgramName,
                              Crypt = ent.ObrazProgramCrypt
                          }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name + ' ' + u.Crypt)).ToList();

                    ComboServ.FillCombo(cbObrazProgram, lst, false, false);
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
                          where ent.LicenseProgramId == LicenseProgramId
                          && ent.ObrazProgramId == ObrazProgramId
                          && ent.StudyLevelId == StudyLevelId
                          orderby ent.ProfileName
                          select ent.Id).Distinct().ToList();

                    if (lst.Count() == 1)
                    {
                        var blocks = (from bl in context.ExamInEntryBlock
                                      where bl.EntryId == lst.FirstOrDefault()
                                      select new
                                      {
                                          bl.Id, bl.Name
                                      }).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();
                        ComboServ.FillCombo(cbExamenBlock, blocks, false, false);
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
                    var blocks = (from bl in context.ExamInEntryBlockUnit
                                  join ex in context.Exam on bl.ExamId equals ex.Id
                                  join exn in context.ExamName on ex.ExamNameId equals exn.Id
                                      where bl.ExamInEntryBlockId == ExamenBlockId
                                      select new
                                      {
                                          bl.Id,
                                          exn.Name
                                      }).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();
                    ComboServ.FillCombo(cbExamenUnit, blocks, false, false);
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
                string query = @"select 
Id
, ExamDate 
, Address 
from dbo.ExamTimeTable 
where ExamInEntryBlockUnitId = @Id ";
                DataTable tbl = MainClass.BdcOnlineReadWrite.GetDataSet(query, new SortedList<string, object>() { { "@Id", ExamenUnitId } }).Tables[0];
                List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
                foreach (DataRow t in tbl.Rows)
                {
                    lst.Add(new KeyValuePair<string, string>(t.Field<int>("Id").ToString(), t.Field<DateTime>("ExamDate").ToShortDateString() + " в " +t.Field<DateTime>("ExamDate").ToShortTimeString() + ", " + t.Field<string>("Address")));
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
                DataTable tbl = ((DataTable)dgv.DataSource).Copy();
                foreach (DataGridViewColumn c in dgv.Columns)
                    if (!c.Visible)
                    {
                        if (tbl.Columns.Contains(c.HeaderText))
                            tbl.Columns.Remove(c.HeaderText);
                    }
                tbl = tbl.DefaultView.ToTable(true);

                string filenameDate = "Список абитуриентов на экзамен ";
                string filename = MainClass.saveTempFolder + filenameDate + ".xls";
                int fileindex = 1;
                while (File.Exists(filename))
                {
                    filename = MainClass.saveTempFolder + filenameDate + "(" + fileindex + ")" + ".xls";
                    fileindex++;
                }
                StringBuilder sb = new StringBuilder();
                string delimiter = ";";
                string[] outputColNames = (from DataColumn col in tbl.Columns select col.ColumnName).ToArray();

                sb.AppendLine(string.Join(delimiter, outputColNames));

                foreach (DataRow rw in tbl.Rows)
                {
                    List<string> outp = new List<string>();
                    foreach (var x in rw.ItemArray)
                    {
                        if (x is DateTime)
                        {
                            DateTime xc = DateTime.Parse(x.ToString());
                            outp.Add(xc.ToShortDateString() + " "+xc.ToShortTimeString());
                        }
                        else
                        {
                            long p;
                            if (long.TryParse(x.ToString(), out p))
                                outp.Add("=\"" + x.ToString() + "\"");
                            else
                                outp.Add(x.ToString());
                        }
                    }

                    sb.AppendLine(string.Join(delimiter, outp.ToArray()));
                }
                File.WriteAllText(filename, sb.ToString(), Encoding.UTF8);
                System.Diagnostics.Process.Start(filename);
            }
            catch
            {
            }
        }
    }
}
