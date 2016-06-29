using EducServLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

namespace PriemLib
{
    public partial class CardOlympResultToAdditionalMark_Master : Form
    {
        private Guid? _EntryId;
        private int? ExamId { get { return ComboServ.GetComboIdInt(cbExam); } }
        private int? OlympTypeId { get { return ComboServ.GetComboIdInt(cbOlympType); } }
        private int? OlympProfileId { get { return ComboServ.GetComboIdInt(cbOlympProfile); } }
        private int? OlympSubjectId { get { return ComboServ.GetComboIdInt(cbOlympSubject); } }
        private int AdditionalMark
        { 
            get 
            { 
                int iVal = 0;
                int.TryParse(tbAdditionalMark.Text, out iVal);
                return iVal;
            } 
        }
        private int MinEge
        {
            get
            {
                int iVal = 0;
                int.TryParse(tbMinEge.Text, out iVal);
                return iVal;
            }
        }
        public event Action OnSave;

        public CardOlympResultToAdditionalMark_Master(Guid EntryId)
        {
            InitializeComponent();
            this.MdiParent = MainClass.mainform;
            _EntryId = EntryId;
            FillExam();

            var lst = HelpClass.GetComboListByTable("ed.OlympType");
            ComboServ.FillCombo(cbOlympType, lst, false, true);
            FillProfiles();
            FillOlympSubjects();
        }

        private void cbOlympType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillProfiles();
        }

        private void FillExam()
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<KeyValuePair<string, string>> lst =
                    ((from ob in context.extOlympBook
                      join exToOl in context.OlympSubjectToExam on ob.OlympSubjectId equals exToOl.OlympSubjectId
                      join ex in context.Exam on exToOl.ExamId equals ex.Id
                      join exInEnt in context.extExamInEntry on ex.Id equals exInEnt.ExamId
                      where exInEnt.EntryId == _EntryId
                      select new
                      {
                          Id = ex.Id,
                          Name = ex.ExamName.Name,
                          exInEnt.OrderNumber
                      }).Distinct()).ToList().OrderBy(x => x.OrderNumber).Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                cbExam.Enabled = true;
                ComboServ.FillCombo(cbExam, lst, false, false);
                cbExam.SelectedIndex = 0;
            }
        }
        private void FillProfiles()
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<KeyValuePair<string, string>> lst =
                    ((from ob in context.extOlympBook
                      where OlympTypeId.HasValue ?  ob.OlympTypeId == OlympTypeId : true
                      select new
                      {
                          Id = ob.OlympProfileId,
                          Name = ob.OlympProfileName,
                      }).Distinct()).ToList().OrderBy(x => x.Name).Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                ComboServ.FillCombo(cbOlympProfile, lst, false, true);
            }
        }
        private void FillOlympSubjects()
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<KeyValuePair<string, string>> lst =
                    ((from ob in context.extOlympBook
                      select new
                      {
                          Id = ob.OlympSubjectId,
                          Name = ob.OlympSubjectName,
                      }).Distinct()).ToList()
                      .OrderBy(x => x.Name)
                      .Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                cbOlympSubject.Enabled = true;
                ComboServ.FillCombo(cbOlympSubject, lst, false, true);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            List<int> lstOlympLevels = new List<int>();
            if (OlympTypeId != 1 && OlympTypeId != 2)
            {
                if (chbOlympLevel_1.Checked)
                    lstOlympLevels.Add(1);
                if (chbOlympLevel_2.Checked)
                    lstOlympLevels.Add(2);
                if (chbOlympLevel_3.Checked)
                    lstOlympLevels.Add(3);
            }
            else
            {
                lstOlympLevels.Add(0);
            }

            List<int> lstOlympVals = new List<int>();
            if (chbOlympValue_1.Checked)
                lstOlympVals.Add(6);
            if (chbOlympValue_2.Checked)
                lstOlympVals.Add(5);
            if (chbOlympValue_3.Checked)
                lstOlympVals.Add(7);

            using (PriemEntities context = new PriemEntities())
            {
                var Ent = context.Entry.Where(x => x.Id == _EntryId).FirstOrDefault();
                if (Ent == null)
                    return;

                var lstEntry = context.Entry.Where(x => x.LicenseProgramId == Ent.LicenseProgramId && x.ObrazProgramId == Ent.ObrazProgramId
                    && x.ProfileId == Ent.ProfileId && x.IsCrimea == Ent.IsCrimea && x.IsForeign == Ent.IsForeign).Select(x => x.Id).ToList();

                var lst = context.OlympResultToAdditionalMark.Where(x => lstEntry.Contains(x.EntryId))
                    .Select(x => new { x.OlympLevelId, x.OlympValueId, x.ExamId, x.EntryId });
                using (TransactionScope tran = new TransactionScope())
                {
                    foreach (int iOlLevel in lstOlympLevels)
                    {
                        foreach (int iOlValue in lstOlympVals)
                        {
                            foreach (Guid EntryId in lstEntry)
                            {
                                int iCnt = lst.Where(x => x.EntryId == EntryId && x.ExamId == ExamId && x.OlympLevelId == iOlLevel && x.OlympValueId == iOlValue).Count();
                                if (iCnt == 0)
                                {
                                    var idParam = new System.Data.Entity.Core.Objects.ObjectParameter("id", typeof(int));
                                    context.OlympResultToAdditionalMark_Insert(EntryId, OlympTypeId, iOlLevel, iOlValue, ExamId, OlympProfileId, OlympSubjectId, AdditionalMark, MinEge, idParam);
                                }
                            }
                        }
                    }

                    tran.Complete();
                }
            }

            MessageBox.Show("OK");

            if (OnSave != null)
                OnSave();
            this.Close();
        }
    }
}
