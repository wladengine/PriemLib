using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

using WordOut;
using EducServLib;
using BDClassLib;
using BaseFormsLib;

namespace PriemLib
{
    public partial class ExamResults : BookList
    {       
        private SortedList examsId;
        private bool muchExams;

        public ExamResults()
        {
            InitializeComponent();
            examsId = new SortedList();
            Dgv = dgvMarks;
            _tableName = "ed.qMark";
            _title = "���������� ���������";

            InitControls();             
        }

        //�������������� ������������� ���������
        protected override void ExtraInit()
        {
            base.ExtraInit();         
            this.Width = 840;

            muchExams = false;

            btnRemove.Visible = btnAdd.Visible = false;

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    ComboServ.FillCombo(cbFaculty, HelpClass.GetComboListByTable("ed.qFaculty", "ORDER BY Acronym"), false, false);
                    ComboServ.FillCombo(cbStudyBasis, HelpClass.GetComboListByTable("ed.StudyBasis", "ORDER BY Name"), false, true);

                    cbStudyBasis.SelectedIndex = 0;
                    FillStudyForm();
                    FillLicenseProgram();
                    FillObrazProgram();
                    FillProfile();
                    
                    ComboServ.FillCombo(cbCompetition, HelpClass.GetComboListByTable("ed.Competition", "ORDER BY Name"), false, true);
                    FillExams();

                    chbEGE.Checked = false;
                    chbOlymps.Checked = false;

                    if (MainClass.dbType == PriemType.PriemMag)
                    {
                        chbEGE.Visible = false;
                        chbOlymps.Visible = false;
                    }
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("������ ��� ������������� ����� " + exc.Message);
            }           
        }

        #region Handlers

        public int? FacultyId
        {
            get { return ComboServ.GetComboIdInt(cbFaculty); }
            set { ComboServ.SetComboId(cbFaculty, value); }
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
        public int? ProfileId
        {
            get
            {
                return ComboServ.GetComboIdInt(cbProfile);
                //string prId = ComboServ.GetComboId(cbProfile);
                //if (string.IsNullOrEmpty(prId))
                //    return null;
                //else
                //    return new Guid(prId);
            }
            set
            {
                if (value == null)
                    ComboServ.SetComboId(cbProfile, (string)null);
                else
                    ComboServ.SetComboId(cbProfile, value.ToString());
            }
        }
        public int? StudyBasisId
        {
            get { return ComboServ.GetComboIdInt(cbStudyBasis); }
            set { ComboServ.SetComboId(cbStudyBasis, value); }
        }
        public int? StudyFormId
        {
            get { return ComboServ.GetComboIdInt(cbStudyForm); }
            set { ComboServ.SetComboId(cbStudyForm, value); }
        }
        public int? CompetitionId
        {
            get { return ComboServ.GetComboIdInt(cbCompetition); }
            set { ComboServ.SetComboId(cbCompetition, value); }
        }
        public int? ExamId
        {
            get { return ComboServ.GetComboIdInt(cbExam); }
            set { ComboServ.SetComboId(cbExam, value); }
        }

        private void FillStudyForm()
        {            
            using (PriemEntities context = new PriemEntities())
            {
                var ent = MainClass.GetEntry(context).Where(c => c.FacultyId == FacultyId);

                List<KeyValuePair<string, string>> lst = ent.ToList().Select(u => new KeyValuePair<string, string>(u.StudyFormId.ToString(), u.StudyFormName)).Distinct().ToList();

                ComboServ.FillCombo(cbStudyForm, lst, false, true);
                cbStudyForm.SelectedIndex = 0;
            }
        }
        private void FillLicenseProgram()
        {            
            using (PriemEntities context = new PriemEntities())
            {
                var ent = MainClass.GetEntry(context).Where(c => c.FacultyId == FacultyId);

                if(StudyFormId != null)
                    ent = ent.Where(c => c.StudyFormId == StudyFormId);

                List<KeyValuePair<string, string>> lst = ent.ToList().Select(u => new KeyValuePair<string, string>(u.LicenseProgramId.ToString(), u.LicenseProgramName)).Distinct().ToList();

                ComboServ.FillCombo(cbLicenseProgram, lst, false, true);
                cbLicenseProgram.SelectedIndex = 0;
            }
        }
        private void FillObrazProgram()
        {          
            using (PriemEntities context = new PriemEntities())
            {
                var ent = MainClass.GetEntry(context).Where(c => c.FacultyId == FacultyId);

                if (StudyFormId != null)
                    ent = ent.Where(c => c.StudyFormId == StudyFormId);
                if (LicenseProgramId != null)
                    ent = ent.Where(c => c.LicenseProgramId == LicenseProgramId);

                List<KeyValuePair<string, string>> lst = ent.ToList().Select(u => new KeyValuePair<string, string>(u.ObrazProgramId.ToString(), u.ObrazProgramName + ' ' + u.ObrazProgramCrypt)).Distinct().ToList();

                ComboServ.FillCombo(cbObrazProgram, lst, false, true);
            }
        }
        private void FillProfile()
        {           
            using (PriemEntities context = new PriemEntities())
            {
                if (ObrazProgramId == null)
                {
                    ComboServ.FillCombo(cbProfile, new List<KeyValuePair<string, string>>(), false, false);
                    cbProfile.Enabled = false;
                    return;
                }

                var ent = MainClass.GetEntry(context).Where(c => c.FacultyId == FacultyId).Where(c => c.ProfileId != null);

                if (StudyFormId != null)
                    ent = ent.Where(c => c.StudyFormId == StudyFormId);
                if (LicenseProgramId != null)
                    ent = ent.Where(c => c.LicenseProgramId == LicenseProgramId);
                if (ObrazProgramId != null)
                    ent = ent.Where(c => c.ObrazProgramId == ObrazProgramId);

                List<KeyValuePair<string, string>> lst = ent.ToList().Select(u => new KeyValuePair<string, string>(u.ProfileId.ToString(), u.ProfileName)).Distinct().ToList();

                if (lst.Count() > 0)
                {
                    ComboServ.FillCombo(cbProfile, lst, false, true);
                    cbProfile.Enabled = true;
                }
                else
                {
                    ComboServ.FillCombo(cbProfile, new List<KeyValuePair<string, string>>(), false, false);
                    cbProfile.Enabled = false;
                }
            }
        }
        private void FillExams()
        {            
            using (PriemEntities context = new PriemEntities())
            {
                var ent = Exams.GetExamsWithFilters(context, FacultyId, LicenseProgramId, ObrazProgramId, ProfileId, StudyFormId, StudyBasisId, null, null, null);
                List<KeyValuePair<string, string>> lst = ent.ToList().Select(u => new KeyValuePair<string, string>(u.ExamId.ToString(), u.ExamName)).Distinct().ToList();
                ComboServ.FillCombo(cbExam, lst, false, true);
            }            
        }

        //������������� ������������ ����������
        public override void InitHandlers()
        {
            cbFaculty.SelectedIndexChanged += new EventHandler(cbFaculty_SelectedIndexChanged);
            cbLicenseProgram.SelectedIndexChanged += new EventHandler(cbLicenseProgram_SelectedIndexChanged);
            cbObrazProgram.SelectedIndexChanged += new EventHandler(cbObrazProgram_SelectedIndexChanged);
            cbProfile.SelectedIndexChanged += new EventHandler(cbProfile_SelectedIndexChanged);
            cbStudyForm.SelectedIndexChanged += new EventHandler(cbStudyForm_SelectedIndexChanged);
            cbStudyBasis.SelectedIndexChanged += new EventHandler(cbStudyBasis_SelectedIndexChanged);
            cbCompetition.SelectedIndexChanged += new EventHandler(cbCompetition_SelectedIndexChanged);
            cbExam.SelectedIndexChanged += new EventHandler(cbExam_SelectedIndexChanged);

            chbEGE.CheckStateChanged += new System.EventHandler(this.chbEGE_CheckStateChanged);
            chbOlymps.CheckStateChanged += new System.EventHandler(this.chbOlymps_CheckStateChanged);
        }

        void cbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvMarks.Rows.Count > 0)
            {
                cbStudyForm.SelectedIndexChanged -= new EventHandler(cbStudyForm_SelectedIndexChanged);
                cbLicenseProgram.SelectedIndexChanged -= new EventHandler(cbLicenseProgram_SelectedIndexChanged);
                cbObrazProgram.SelectedIndexChanged -= new EventHandler(cbObrazProgram_SelectedIndexChanged);
                cbProfile.SelectedIndexChanged -= new EventHandler(cbProfile_SelectedIndexChanged);
                cbStudyForm.SelectedIndexChanged -= new EventHandler(cbStudyForm_SelectedIndexChanged);
                cbStudyBasis.SelectedIndexChanged -= new EventHandler(cbStudyBasis_SelectedIndexChanged);
                cbCompetition.SelectedIndexChanged -= new EventHandler(cbCompetition_SelectedIndexChanged);
            }

            FillStudyForm();
            FillLicenseProgram();
            FillObrazProgram();
            FillProfile();
            FillExams();

            cbStudyForm.SelectedIndexChanged += new EventHandler(cbStudyForm_SelectedIndexChanged);
            cbLicenseProgram.SelectedIndexChanged += new EventHandler(cbLicenseProgram_SelectedIndexChanged);
            cbObrazProgram.SelectedIndexChanged += new EventHandler(cbObrazProgram_SelectedIndexChanged);
            cbProfile.SelectedIndexChanged += new EventHandler(cbProfile_SelectedIndexChanged);
            cbStudyForm.SelectedIndexChanged += new EventHandler(cbStudyForm_SelectedIndexChanged);
            cbStudyBasis.SelectedIndexChanged += new EventHandler(cbStudyBasis_SelectedIndexChanged);
            cbCompetition.SelectedIndexChanged += new EventHandler(cbCompetition_SelectedIndexChanged);
        }
        void cbStudyForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillLicenseProgram();            
        }
        void cbLicenseProgram_SelectedIndexChanged(object sender, EventArgs e)
        {        
            FillObrazProgram();            
        }
        void cbObrazProgram_SelectedIndexChanged(object sender, EventArgs e)
        {           
            FillProfile();
            //if (cbProfile.Items.Count < 2)
            //    FillExams();
        }
        void cbProfile_SelectedIndexChanged(object sender, EventArgs e)
        {           
            FillExams();
        }
        void cbStudyBasis_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillExams();
        }
        void cbCompetition_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }
        void cbExam_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }

        #endregion

        public override void UpdateDataGrid()
        {
            FillGridMarks(GetAbitFilterString(), GetExamFilterString(), chbEGE.Checked, chbOlymps.Checked);          

            lblCount.Text = "�����: " + Dgv.Rows.Count.ToString();
            btnCard.Enabled = !(Dgv.RowCount == 0);
        }
        
        private void FillGridMarks(string abitFilters, string examFilters, bool isEge, bool isOlymp)
        {
            DataTable examTable = new DataTable();
            DataSet ds;
            string sQueryAbit;
            List<ListItem> egeList = new List<ListItem>();
            List<ListItem> olympList = new List<ListItem>();

            DataColumn clm;
            clm = new DataColumn();
            clm.ColumnName = "��_�����";
            examTable.Columns.Add(clm);

            clm = new DataColumn();
            clm.ColumnName = "���";
            examTable.Columns.Add(clm);

            clm = new DataColumn();
            clm.ColumnName = "���_�����";
            examTable.Columns.Add(clm); 
            
            clm = new DataColumn();
            clm.ColumnName = "�����������";
            examTable.Columns.Add(clm);            
            
            clm = new DataColumn();
            clm.ColumnName = "������ ��������";
            examTable.Columns.Add(clm);
            
            clm = new DataColumn();
            clm.ColumnName = "����� ��������";
            examTable.Columns.Add(clm);            

            clm = new DataColumn();
            clm.ColumnName = "Id";
            examTable.Columns.Add(clm);

            clm = new DataColumn();
            clm.ColumnName = "�������";
            examTable.Columns.Add(clm);

            clm = new DataColumn();
            clm.ColumnName = "����� ������";
            clm.DataType = typeof(int);
            examTable.Columns.Add(clm);

            IList lst = examsId.GetKeyList();

            string examsFields = string.Empty;

            if (examsId.Count > 50)
            {
                if(!muchExams) 
                    WinFormsServ.Error("������� ����� ���������, �������������� ���������!");
                
                muchExams = true;
                dgvMarks.DataSource = null;
                return;
            }

            muchExams = false;
            foreach (DictionaryEntry de in examsId)
            {
                clm = new DataColumn();
                clm.ColumnName = de.Value.ToString();                                              
                examTable.Columns.Add(clm);

                clm = new DataColumn();
                clm.ColumnName = de.Value.ToString()+"IsEge";                
                examTable.Columns.Add(clm);

                clm = new DataColumn();
                clm.ColumnName = de.Value.ToString() + "IsOlymp";
                examTable.Columns.Add(clm);
            }

            NewWatch wc = new NewWatch();
            wc.Show();
            wc.SetText("��������� ������ �� ������������...");
            sQueryAbit = string.Format(@"SELECT DISTINCT 
ed.extAbit.Id as Id, 
extPerson.PersonNum as ��_�����, 
ed.extAbit.RegNum as ���_�����, 
ed.extAbitMarksSum.TotalSum AS Sum, 
ed.extPerson.FIO as ���, 
ed.extAbit.ObrazProgramCrypt + ' ' +(Case when NOT ed.extAbit.ProfileId IS NULL then ed.extAbit.ProfileName else ed.extAbit.ObrazProgramName end) as Spec, 
ed.extAbit.StudyFormName AS StudyForm, 
ed.extAbit.StudyBasisName AS StudyBasis,
Competition.Name AS CompName
FROM ed.extAbit 
LEFT JOIN ed.extPerson ON ed.extAbit.PersonId = ed.extPerson.Id 
LEFT JOIN ed.Competition ON ed.extAbit.CompetitionId = ed.Competition.Id 
LEFT JOIN ed.extAbitMarksSum ON ed.extAbitMarksSum.Id = ed.extAbit.Id 
--LEFT JOIN ed.qMark ON qMark.AbiturientId = extAbit.Id
{0}
ORDER BY ���", abitFilters);
                        
            ds = _bdc.GetDataSet(sQueryAbit);

            var abit_data = from DataRow rw in ds.Tables[0].Rows
                            select new
                            {
                                Id = rw.Field<Guid>("Id"),
                                PersonNum = rw["��_�����"].ToString(),
                                RegNum = rw["���_�����"].ToString(),
                                Sum = rw.Field<int?>("Sum"),
                                FIO = rw.Field<string>("���"),
                                StudyForm = rw.Field<string>("StudyForm"),
                                StudyBasis = rw.Field<string>("StudyBasis"),
                                Spec = rw.Field<string>("Spec"),
                                Competition = rw.Field<string>("CompName"),
                            };

            List<Guid> ids = abit_data.Select(x => x.Id).Distinct().ToList();
            
            wc.SetText("��������� ������ �� ����...");

            string query = string.Format(@"SELECT AbiturientId, 
                qMark.ExamId,
                qMark.Value,
                case when qMark.IsFromEge IS NULL OR qMark.IsFromEge = 'False' then 0 else 1 end AS IsFromEge,
                case when qMark.IsFromOlymp IS NULL OR qMark.IsFromOlymp = 'False' then 0 else 1 end AS IsFromOlymp
                FROM ed.qMark
                INNER JOIN ed.extAbit ON extAbit.Id = qMark.AbiturientId
                {0}", abitFilters);
            ds = _bdc.GetDataSet(query);

            var marks = from DataRow rw in ds.Tables[0].Rows
                        select new
                        {
                            AbiturientId = rw.Field<Guid>("AbiturientId"),
                            ExamId = rw.Field<int?>("ExamId"),
                            Value = rw.Field<byte?>("Value"),
                            IsFromOlymp = rw.Field<int>("IsFromOlymp") == 1 ? true : false,
                            IsFromEge = rw.Field<int>("IsFromEge") == 1 ? true : false
                        };
            marks.GetEnumerator();
            wc.SetText("���������� ������...");
            
            List<string> lstIds = new List<string>();
            int iCntAbits = ids.Count();
            wc.SetMax(iCntAbits);
            foreach (Guid abitId in ids)
            {
                DataRow newRow;
                newRow = examTable.NewRow();
                var abit = abit_data.Where(x => x.Id == abitId).First();
                newRow["��_�����"] = abit.PersonNum.ToString();
                newRow["���"] = abit.FIO;
                newRow["���_�����"] = abit.RegNum.ToString();
                newRow["�����������"] = abit.Spec;
                newRow["������ ��������"] = abit.StudyBasis;
                newRow["����� ��������"] = abit.StudyForm;
                newRow["Id"] = abit.Id.ToString();
                newRow["�������"] = abit.Competition;
                newRow["����� ������"] = abit.Sum.HasValue ? abit.Sum.Value : 0;
                foreach (DictionaryEntry de in examsId)
                {
                    int iExamId = 0;
                    if (!int.TryParse(de.Key.ToString(), out iExamId))
                        continue;
                    var mark_data = marks.Where(x => x.AbiturientId == abitId && x.ExamId == iExamId);
                    int markSum = mark_data.Select(x => x.Value).DefaultIfEmpty((byte?)0).Sum(x => x.Value);
                    bool isFromEge = mark_data.Select(x => x.IsFromEge).DefaultIfEmpty(false).First();
                    bool isFromOlymp = mark_data.Select(x => x.IsFromOlymp).DefaultIfEmpty(false).First();
                    
                    newRow[de.Value.ToString()] = markSum == 0 ? "" : markSum.ToString();
                    newRow[de.Value.ToString() + "IsEge"] = isFromEge.ToString();
                    newRow[de.Value.ToString() + "IsOlymp"] = isFromOlymp.ToString();
                }

                examTable.Rows.Add(newRow);
                wc.PerformStep();
                
                lstIds.Add(string.Format("'{0}'", abitId.ToString()));
            }

            wc.Close();

            DataView dv = new DataView(examTable);
            dv.AllowNew = false;

            dgvMarks.DataSource = dv;
            dgvMarks.ReadOnly = true;

            dgvMarks.Columns["Id"].Visible = false;

            foreach (DictionaryEntry de in examsId)
            {                
                dgvMarks.Columns[de.Value.ToString() + "IsEge"].Visible=false;
                dgvMarks.Columns[de.Value.ToString() + "IsOlymp"].Visible=false;                
            }

            dgvMarks.Columns["��_�����"].Width = 65;
            dgvMarks.Columns["���_�����"].Width = 66;
            dgvMarks.Columns["���"].Width = 203;
            dgvMarks.Columns["����� ��������"].Width = 56;
            dgvMarks.Columns["������ ��������"].Width = 56;
            dgvMarks.Columns["�����������"].Width = 100;
            dgvMarks.Columns["�������"].Width = 54;
            dgvMarks.Columns["����� ������"].Width = 49;
           
            for (int i = 9; i < dgvMarks.Columns.Count; i = i + 1)
                dgvMarks.Columns[i].Width = 43;

            dgvMarks.Update(); 
        }        

        //������ ������ �������� ��� ������������
        private string GetAbitFilterString()
        {
            string s = " WHERE 1=1 ";

            s += " AND ed.extAbit.StudyLevelGroupId = " + MainClass.studyLevelGroupId;

            //���������� ����� ��������  
            if (StudyFormId != null)
                s += " AND ed.extAbit.StudyFormId = " + StudyFormId;

            //���������� ������ ��������  
            if (StudyBasisId != null)
                s += " AND ed.extAbit.StudyBasisId = " + StudyBasisId;   

            //���������� ���������
            if (FacultyId != null)
                s += " AND ed.extAbit.FacultyId = " + FacultyId;

            //���������� ��� ��������          
            if (CompetitionId != null)
                s += " AND ed.extAbit.CompetitionId = " + CompetitionId;
            
            //���������� �����������
            if (LicenseProgramId != null)
                s += " AND ed.extAbit.LicenseProgramId = " + LicenseProgramId;

            //���������� ����� ���������
            if (ObrazProgramId != null)
                s += " AND ed.extAbit.ObrazProgramId = " + ObrazProgramId;

            //���������� ������������� 
            if (ProfileId != null)
                s += string.Format(" AND ed.extAbit.ProfileId = '{0}'", ProfileId);
          
            return s;
        }

        // ������ ������ �������� ��� ���������
        private string GetExamFilterString()
        {
            string s = "";
            string defQuery = "SELECT DISTINCT ed.extExamInEntry.ExamId AS Id, ed.extExamInEntry.ExamName AS Name FROM ed.extExamInEntry WHERE 1=1";

            s += " AND ed.extExamInEntry.StudyLevelGroupId = " + MainClass.studyLevelGroupId;
            defQuery += " AND ed.extExamInEntry.StudyLevelGroupId = " + MainClass.studyLevelGroupId;

            if (ExamId != null)
            {
                s += " AND ed.extExamInEntry.ExamId = " + ExamId;
                defQuery += " AND ed.extExamInEntry.ExamId = " + ExamId;
            }

            //���������� ����� ��������  
            if (StudyFormId != null)
            {
                s += " AND ed.extExamInEntry.StudyFormId = " + StudyFormId;
                defQuery += " AND ed.extExamInEntry.StudyFormId = " + StudyFormId;
            }
            
            //���������� ������ ��������  
            if (StudyBasisId != null)
            {
                s += " AND ed.extExamInEntry.StudyBasisId = " + StudyBasisId;
                defQuery += " AND ed.extExamInEntry.StudyBasisId = " + StudyBasisId;
            }

            //���������� ���������
            if (FacultyId != null)
            {
                s += " AND ed.extExamInEntry.FacultyId = " + FacultyId;
                defQuery += " AND ed.extExamInEntry.FacultyId = " + FacultyId;
            }

            //���������� �����������
            if (LicenseProgramId != null)
            {
                s += " AND ed.extExamInEntry.LicenseProgramId = " + LicenseProgramId;
                defQuery += " AND ed.extExamInEntry.LicenseProgramId = " + LicenseProgramId;
            }

            //���������� ����� ���������             
            if (ObrazProgramId != null)            
            {
                s += " AND ed.extExamInEntry.ObrazProgramId = " + ObrazProgramId;
                defQuery += " AND ed.extExamInEntry.ObrazProgramId = " + ObrazProgramId;
            }

            //���������� �������������           
            if (ProfileId != null)
            {
                s += string.Format(" AND ed.extExamInEntry.ProfileId = '{0}'", ProfileId);
                defQuery += string.Format(" AND ed.extExamInEntry.ProfileId = '{0}'", ProfileId);              
            }
          
            //���������� ����
            if (dtDateExam.Checked)            
                s += " AND qMark.PassDate = " + _bdc.BuildData(dtDateExam.Value);           
            
            DataSet ds = _bdc.GetDataSet(defQuery);
            examsId.Clear();
            foreach (DataRow dsRow in ds.Tables[0].Rows)
            {
                string sName =dsRow["Name"].ToString();
                examsId.Add(dsRow["Id"].ToString(), sName);
            }

            return s;
        }
        
        protected override void OpenCard(string itemId, BaseFormEx formOwner, int? index)
        {
            MainClass.OpenCardAbit(itemId, this, dgvMarks.CurrentRow.Index);
        }       

        //����� �� ������
        private void tbNumber_TextChanged(object sender, EventArgs e)
        {
            WinFormsServ.Search(this.dgvMarks, "���_�����", tbNumber.Text);
        }
        //����� �� ���
        private void tbFIO_TextChanged(object sender, EventArgs e)
        {
            WinFormsServ.Search(this.dgvMarks, "���", tbFIO.Text);
        }
        
        private void chbEGE_CheckStateChanged(object sender, EventArgs e)
        {            
            dgvMarks.Refresh();
        }
        private void chbOlymps_CheckStateChanged(object sender, EventArgs e)
        {            
            dgvMarks.Refresh();
        }       
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                WordDoc wd = new WordDoc(string.Format(@"{0}\Marks.dot", MainClass.dirTemplates));

                //����������
                string sFac = cbFaculty.Text.ToLower();
                if (sFac.CompareTo("���") == 0)
                    sFac = "��� ���������� ";
                else
                {                   
                    if (FacultyId == 3)
                        sFac = "������ ����� ����������� ";
                    else
                        sFac = sFac + " ��������� ";
                }

                string sForm = cbStudyForm.Text.ToLower();
                if (sForm.CompareTo("���") == 0)
                    sForm = " ��� ����� �������� ";
                else
                    sForm = sForm + " ����� �������� ";

                string sCom = cbCompetition.Text.ToLower();
                if (sCom.CompareTo("���") == 0)
                    sCom = "��� ���� �������� ";
                else
                    sCom = "��� ��������: " + sCom;

                string sSpec = cbLicenseProgram.Text.ToLower();
                if (sSpec.CompareTo("���") == 0)
                    sSpec = " ��� �����������";
                else
                    sSpec = " �����������: " + sSpec;

                wd.Fields["Faculty"].Text = sFac;
                wd.Fields["Section"].Text = sForm;
                wd.Fields["Competition"].Text = sCom;
                wd.Fields["Profession"].Text = sSpec;

                int colCount = 0;
                foreach (DataGridViewColumn clm in dgvMarks.Columns)
                {
                    if (clm.Visible)
                        colCount++;
                }

                wd.AddNewTable(2, colCount);
                TableDoc td = wd.Tables[0];

                int i = 0;
                foreach (DataGridViewColumn clm in dgvMarks.Columns)
                {
                    if (clm.Visible)
                    {
                        td[i, 0] = clm.HeaderText;
                        i++;
                    }
                }

                i = 1;
                int j;
                foreach (DataGridViewRow dgvr in dgvMarks.Rows)
                {
                    j = 0;
                    foreach (DataGridViewColumn clm in dgvMarks.Columns)
                    {
                        if (clm.Visible)
                        {
                            td[j, i] = dgvr.Cells[clm.Index].Value.ToString();
                            j++;
                        }
                    }
                    i++;
                    td.AddRow(1);
                }
            }
            catch (WordException we)
            {
                WinFormsServ.Error(we.Message);
            }
            catch (Exception exc)
            {
                WinFormsServ.Error(exc.Message);
            }              
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {            
            MainClass.DataRefresh();
        }
        private void dtDateExam_ValueChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }
        private void dgvMarks_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                foreach (DictionaryEntry de in examsId)
                {
                    if (e.ColumnIndex == dgvMarks.Columns[de.Value.ToString()].Index)
                    {
                        if (chbEGE.Checked)
                        {
                            if (dgvMarks[de.Value.ToString() + "IsEge", e.RowIndex].Value.ToString() == "True")
                                dgvMarks[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.LightGreen;
                            else
                                dgvMarks[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                        }
                        else if (chbOlymps.Checked)
                        {
                            if (dgvMarks[de.Value.ToString() + "IsOlymp", e.RowIndex].Value.ToString() == "True")
                                dgvMarks[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.LightBlue;
                            else
                                dgvMarks[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                        }
                        else
                            dgvMarks[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                        break;
                    }

                }
            }
            catch
            {
            }
        }
        protected override void Dgv_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex <= 0)
                _orderBy = null;
            else
                _orderBy = "it." + Dgv.Columns[e.ColumnIndex].Name;
        }

        private void btnPrintToCSV_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            List<string> FieldsNotToUse = new List<string>() { "���_�����", "�����������", "������ ��������", "Id", "�������", "����� ��������" };
            
            StringBuilder sb = new StringBuilder();
            int i = 1;
            int j;
            string str = "";

            NewWatch wc = new NewWatch(dgvMarks.Rows.Count);
            wc.Show();

            i = 0;
            foreach (DataGridViewColumn clm in dgvMarks.Columns)
            {
                if (clm.Visible && !FieldsNotToUse.Contains(clm.Name))
                {
                    str += clm.HeaderText + ";";
                    i++;
                }
            }
            sb.AppendLine(str);
            List<string> lstVals = new List<string>();
            foreach (DataGridViewRow dgvr in dgvMarks.Rows)
            {
                str = "";
                j = 0;
                foreach (DataGridViewColumn clm in dgvMarks.Columns)
                {
                    if (clm.Visible && !FieldsNotToUse.Contains(clm.Name))
                    {
                        str += dgvr.Cells[clm.Index].Value.ToString() + ";";
                        j++;
                    }
                }
                i++;
                if (!lstVals.Contains(str))
                {
                    sb.AppendLine(str);
                    lstVals.Add(str);
                }
                wc.PerformStep();
            }
            wc.SetText("���������� ����� �� ����...");
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName, false, Encoding.GetEncoding(1251)))
            {
                sw.Write(sb.ToString());
                sw.Flush();
            }
            wc.Close();
            MessageBox.Show("Done");
        }
    }
}