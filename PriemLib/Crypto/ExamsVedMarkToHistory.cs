using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BDClassLib;
using EducServLib;
using BaseFormsLib;

namespace PriemLib
{
    public partial class ExamsVedMarkToHistory : Form
    {
        public int? StudyLevelGroupId
        {
            get { return ComboServ.GetComboIdInt(cbStudyLevelGroup); }
            set { ComboServ.SetComboId(cbStudyLevelGroup, value); }
        }
        public int? FacultyId
        {
            get { return ComboServ.GetComboIdInt(cbFaculty); }
            set { ComboServ.SetComboId(cbFaculty, value); }
        }
        public int? StudyBasisId
        {
            get { return ComboServ.GetComboIdInt(cbStudyBasis); }
            set { ComboServ.SetComboId(cbStudyBasis, value); }
        }
        public int? ExamId
        {
            get { return ComboServ.GetComboIdInt(cbExam); }
            set { ComboServ.SetComboId(cbExam, value); }
        }
        public List<Guid> gExamsVed;

        public ExamsVedMarkToHistory()
        {
            InitializeComponent();
            SelectedMarkTypes = new List<int>();
            this.MdiParent = MainClass.mainform;
            FillFaculty();
        }
        protected virtual List<KeyValuePair<string, string>> GetSourceStudyLevelGroup()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = MainClass.GetEntry(context)
                    .Select(x => new { x.StudyLevelGroupId, x.StudyLevelGroupName })
                    .Distinct()
                    .ToList()
                    .Select(x => new KeyValuePair<string, string>(x.StudyLevelGroupId.ToString(), x.StudyLevelGroupName))
                    .ToList();

                return src;
            }
        }
        public void FillFaculty()
        {
            FillMarkType();
            ComboServ.FillCombo(cbStudyLevelGroup, GetSourceStudyLevelGroup(), false, false);
            ComboServ.FillCombo(cbFaculty, HelpClass.GetComboListByTable("ed.qFaculty", "ORDER BY Acronym"), false, true);
            ComboServ.FillCombo(cbStudyBasis, HelpClass.GetComboListByTable("ed.StudyBasis", "ORDER BY Name"), false, true);
            
        }
        public void FillMarkType()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lst = (from x in context.ExamsVedMarkType
                               select new {
                                   x.Id, x.Name
                               }).ToList().OrderBy(x=>x.Id).Select(u => new KeyValuePair<string, string>(
                          u.Id.ToString(), u.Name)).ToList();
                lbmarktype.DataSource = lst;
                lbmarktype.ValueMember = "Key";
                lbmarktype.DisplayMember = "Value";
                lbmarktype.ClearSelected();

                lbmarktype.SelectedIndices.Add(2);
                lbmarktype.SelectedIndices.Add(3);
                lbmarktype.SelectedIndices.Add(4);
                lbmarktype.SelectedIndices.Add(5);
            }
        }
        public void FillExam()
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<Guid> veds = (from x in context.ExamsVedSelectedMarkType
                                   where SelectedMarkTypes.Contains(x.MarkTypeId)
                                   select x.ExamsVedId).Distinct().ToList();

                List<KeyValuePair<string, string>> lst =
                    ((from ent in context.extExamsVed
                      
                      where MainClass.lstStudyLevelGroupId.Contains(ent.StudyLevelGroupId)
                      && (FacultyId.HasValue ?  ent.FacultyId == FacultyId : true)
                      && (StudyBasisId != null ? ent.StudyBasisId == StudyBasisId : true == true)
                      && (veds.Count > 0 ? veds.Contains(ent.Id) : true)
                      select new
                      {
                          Id = ent.ExamId,
                          Name = ent.ExamName,
                      }).Distinct()).ToList().OrderBy(x => x.Name)
                      .Select(u => new KeyValuePair<string, string>(
                          u.Id.ToString(), u.Name)).ToList();
                ComboServ.FillCombo(cbExam, lst, false, true);
            }
        }
        public void FillVed()
        {
            using (PriemEntities context = new PriemEntities())
            {

                List<Guid> veds = (from x in context.ExamsVedSelectedMarkType
                                   where SelectedMarkTypes.Contains(x.MarkTypeId)
                                   select x.ExamsVedId).Distinct().ToList();


                List<KeyValuePair<string, string>> lst =
                    ((from ent in context.extExamsVed
                      where MainClass.lstStudyLevelGroupId.Contains(ent.StudyLevelGroupId)
                       && (FacultyId.HasValue ? ent.FacultyId == FacultyId : true)
                      && (StudyBasisId != null ? ent.StudyBasisId == StudyBasisId : true == true)
                      && ((ExamId  != null) ? ent.ExamId == ExamId : true)
                      && (veds.Count > 0 ? veds.Contains(ent.Id) : true)
                      select new
                      {
                          ent.Id,
                          ent.Number,
                          ent.ExamName,
                          ent.Date,
                          StBasis = ent.StudyBasisId == null ? "" : ent.StudyBasisAcr,
                          AddVed = ent.IsAddVed ? " дополнительная" : "",
                          ent.AddCount
                      }).Distinct()).ToList().OrderBy(x => x.Date).ThenBy(x => x.ExamName).ThenBy(x => x.Number)
                      .Select(u => new KeyValuePair<string, string>(
                          u.Id.ToString(),
                          "[" + u.Number + "] " + u.ExamName + ' ' + u.Date.ToShortDateString() + ' ' + u.StBasis + u.AddVed +
                            (u.AddCount > 1 ? "(" + Convert.ToString(u.AddCount) + ")" : ""))).ToList();

                ComboServ.FillCombo(cbExamVed, lst, false, true);
            }
        }

        private void cbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillExam();
        }

        private void cbExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillVed();
        }

        private void cbExamVed_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                Guid? VedId =  ComboServ.GetComboIdGuid(cbExamVed);
                List<Guid> veds = (from x in context.ExamsVedSelectedMarkType
                                   where SelectedMarkTypes.Contains(x.MarkTypeId)
                                   select x.ExamsVedId).Distinct().ToList();
                gExamsVed =  
                    (from ent in context.extExamsVed
                      where MainClass.lstStudyLevelGroupId.Contains(ent.StudyLevelGroupId)
                       && (FacultyId.HasValue ? ent.FacultyId == FacultyId : true)
                      && (StudyBasisId != null ? ent.StudyBasisId == StudyBasisId : true == true)
                      && ((ExamId  != null) ? ent.ExamId == ExamId : true)
                      && (VedId.HasValue ? ent.Id == VedId.Value : true)
                      && (veds.Count > 0 ? veds.Contains(ent.Id) : true)
                      select ent.Id).Distinct().ToList();

                var Cnt = (from x in context.ExamsVedHistory 
                               where gExamsVed.Contains(x.ExamsVedId)
                               select x.Id).Count();

                lblPersonCount.Text = gExamsVed.Count().ToString() + " ведомостей // " + Cnt.ToString() + " записей в ведомостях";
            }
        }
        List<int> SelectedMarkTypes;
        private void lbmarktype_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedMarkTypes = new List<int>();
            foreach (KeyValuePair<string,string> x in lbmarktype.SelectedItems)
            {
                SelectedMarkTypes.Add(int.Parse(x.Key));
            }
            FillExam();
        }

        private void btnUpdateMarks_Click(object sender, EventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                foreach (Guid VedId in gExamsVed)
                {
                    // если MarkValue == null ???? 
                    //var Marks = (from x in context.ExamsVedHistoryMark
                    //             join h in context.ExamsVedHistory on x.ExamsVedHistoryId equals h.Id
                    //             where h.ExamsVedId == VedId && x.MarkValue != null
                    //             select x).ToList().GroupBy(x => x.ExamsVedHistoryId).Select(x => new
                    //             {
                    //                 ExamsVedHistoryId = x.Key,
                    //                 MarkSum = x.Sum(t => t.MarkValue ?? 0),
                    //             }).ToList();

                    //foreach (var Mark in Marks)
                    //{
                    //    ExamsVedHistory H = context.ExamsVedHistory.Where(x => x.Id == Mark.ExamsVedHistoryId).First();
                    //    H.Mark = Mark.MarkSum;
                    //    context.SaveChanges();
                    //}

                    var MarksRawData = (from x in context.ExamsVedHistoryMark
                                        join h in context.ExamsVedHistory on x.ExamsVedHistoryId equals h.Id
                                        where h.ExamsVedId == VedId && x.MarkValue != null
                                        select new
                                        {
                                            x.ExamsVedHistoryId,
                                            x.ExamsVedMarkTypeId,
                                            x.MarkValue,
                                        }).ToList();

                    var PersonList = MarksRawData.Select(x => x.ExamsVedHistoryId).Distinct().ToList();
                    var lstTypes = MarksRawData.Where(x => x.ExamsVedMarkTypeId != null).Select(x => x.ExamsVedMarkTypeId.Value).Distinct().OrderBy(x => x).ToList();
                    bool HasTwoMarks = lstTypes.Count == 2;
                    int iFirst = 0, iSecond = 0;
                    if (HasTwoMarks)
                    {
                        iFirst = lstTypes[0];
                        iSecond = lstTypes[1];
                    }
                    foreach (Guid HistId in PersonList)
                    {
                        ExamsVedHistory H = context.ExamsVedHistory.Where(x => x.Id == HistId).FirstOrDefault();
                        if (H != null)
                        {
                            if (HasTwoMarks)
                            {
                                var Mrks = MarksRawData.Where(x => x.ExamsVedHistoryId == HistId).ToList();
                                if (Mrks.Count > 0)
                                    H.Mark = Mrks.Where(x => x.ExamsVedMarkTypeId == iFirst).Select(x => x.MarkValue ?? 0).FirstOrDefault();
                                if (Mrks.Count > 1)
                                    H.OralMark = Mrks.Where(x => x.ExamsVedMarkTypeId == iSecond).Select(x => x.MarkValue ?? 0).FirstOrDefault();
                            }
                            else
                            {
                                H.Mark = MarksRawData.Where(x => x.ExamsVedHistoryId == HistId).Select(x => x.MarkValue ?? 0).Sum();
                            }
                            context.SaveChanges();
                        }
                    }
                }
            }

            MessageBox.Show("OK");
        }
    }
}
