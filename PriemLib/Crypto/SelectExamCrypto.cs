using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

using EducServLib;
using BDClassLib;
using BaseFormsLib;

namespace PriemLib
{
    public partial class SelectExamCrypto : BaseForm
    {
        private DBPriem bdc;
        protected int iStudyLevelGroupId;
        protected int? iFacultyId;
        protected int? iStudyBasisId;
        private ExamsVedList owner;
        protected bool isAdditional;
        protected DateTime ExamDate
        {
            get { return dtDateExam.Value;  }
            set { dtDateExam.Value = value; }
        }
        public int? ExamId
        {
            get { return ComboServ.GetComboIdInt(cbExam); }
            set { ComboServ.SetComboId(cbExam, value); }
        }

        public SelectExamCrypto(ExamsVedList owner, int studyLevelGroupId, int? facId, int? basisId, DateTime passDate, int? examId)
        {
            InitializeComponent();            
            this.owner = owner;
            this.iStudyLevelGroupId = studyLevelGroupId;
            this.iFacultyId = facId;
            this.iStudyBasisId = basisId;
            this.isAdditional = true;            

            InitControls(); 
            cbExam.Enabled = false;
            dtDateExam.Enabled = false;

            dtDateExam.Value = passDate;            
            this.ExamId = examId;
        }

        public SelectExamCrypto(ExamsVedList owner, int studyLevelId, int? facId, int? basisId)
        {
            InitializeComponent();
            this.owner = owner;
            this.iFacultyId = facId;
            this.iStudyBasisId = basisId;
            this.isAdditional = false;
            this.iStudyLevelGroupId = studyLevelId;

            InitControls();
        }  

        //дополнительная инициализация контролов
        private void InitControls()
        {
            this.CenterToParent();
            InitFocusHandlers();
            this.MdiParent = MainClass.mainform;
            bdc = MainClass.Bdc; 
           
            FillExams();
        }

        private void FillExams()
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<KeyValuePair<string, string>> lst = GetSourceExam();
                ComboServ.FillCombo(cbExam, lst, false, false);                 
            }          
        }

        protected virtual List<KeyValuePair<string, string>> GetSourceExam()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var ent = Exams.GetExamsWithFilters(context, MainClass.lstStudyLevelGroupId, iFacultyId, null, null, null, null, iStudyBasisId, null, null, null);
                List<KeyValuePair<string, string>> lst = ent.ToList().Select(u => new KeyValuePair<string, string>(u.ExamId.ToString(), u.ExamName)).Distinct().ToList();

                return lst;
            }
        }

        protected bool CheckFields()
        {
            if (!ExamId.HasValue)
            {
                WinFormsServ.Error("Не выбран экзамен!");
                return false ;
            }
            if (!iFacultyId.HasValue)
            {
                WinFormsServ.Error("Не выбрано подразделение!");
                return false;
            }

            if (!isAdditional)
            {
                using (PriemEntities context = new PriemEntities())
                {
                    int cnt = (from ev in context.extExamsVed
                               where ev.ExamId == ExamId && ev.Date == dtDateExam.Value.Date
                               && ev.FacultyId == iFacultyId
                               && (iStudyBasisId == null ? ev.StudyBasisId == null : ev.StudyBasisId == iStudyBasisId)
                               && !ev.IsAddVed
                               select ev).Count();

                    if (cnt > 0)
                    {
                        WinFormsServ.Error(string.Format("{0}едомость на этот экзамен на эту дату {1} уже существует! ", isAdditional ? "Дополнительная в" : "В", iStudyBasisId == null ? "" : "на эту основу обучения"));
                        return false;
                    }
                }
            }
            return true;
        }
        protected virtual void btnOk_Click(object sender, EventArgs e)
        {
            if (!CheckFields())
                return;

            ExamsVedCard frm = new ExamsVedCard(owner, iStudyLevelGroupId, iFacultyId.Value, ExamId.Value, ExamDate, iStudyBasisId, isAdditional);
            frm.Show();
            this.Close();
        }                     
    }
}