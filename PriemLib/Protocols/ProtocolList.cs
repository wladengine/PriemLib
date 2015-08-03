using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

//using BDClassLib;
using EducServLib;
using BaseFormsLib;

namespace PriemLib
{
    public partial class ProtocolList : BaseFormEx
    {
        protected ProtocolTypes _protocolType;
        protected string sQuery;
        protected string sWhere;
        protected string sOrderby;

        private DateTime _protocolDate;
        private string _protocolName;
        private string _protocolReason;
        
        protected DataRefreshHandler drh = null;
        protected ProtocolRefreshHandler prh = null;

        //конструктор
        public ProtocolList(ProtocolTypes protocolType)
        {
            this.CenterToParent();
            this.MdiParent = MainClass.mainform;
            this._protocolType = protocolType;

            this.sQuery = @"SELECT DISTINCT 
Abiturient.Id as Id, 
Abiturient.RegNum,
Abiturient.BAckDoc, 
Abiturient.RegNum as Рег_Номер,
extPerson.FIO as ФИО, 
extPerson.EducDocument as Документ_об_образовании, 
extEntry.ObrazProgramNameEx + ' ' + 
	(Case when extEntry.ProfileId = 0 then '' 
	else extEntry.ProfileName end) as Направление,
ed.Competition.Name as Конкурс, 
0 as Black,
(CASE WHEN Abiturient.BackDoc > 0 THEN 'Забрал док.' 
	ELSE (CASE WHEN Abiturient.NotEnabled > 0 THEN 'Не допущен'
		ELSE (CASE WHEN ProtocolHistory.Excluded>0 THEN 'Исключен из протокола' 
			ELSE '' 
			END) 
	END)
 END) as Примечания, 
(Abiturient.BAckDoc | Abiturient.NotEnabled | ProtocolHistory.Excluded) as Red, 
Abiturient.Sum as Сумма_баллов, 
(Case WHEN extPerson.HostelAbit > 0 then 'да' else 'нет' END) as Общежитие 
FROM ed.Abiturient 
INNER JOIN ed.extPerson ON ed.extPerson.Id= Abiturient.PersonId 
INNER JOIN ed.extEntry ON extEntry.Id = Abiturient.EntryId
LEFT JOIN ed.Competition ON ed.Competition.Id = Abiturient.CompetitionId
LEFT JOIN ed.ProtocolHistory ON ProtocolHistory.AbiturientId = Abiturient.Id
LEFT JOIN ed.Protocol ON Protocol.Id =  ProtocolHistory.ProtocolId ";

            this.sWhere = string.Format(" WHERE 1=1  AND Protocol.ProtocolTypeId = {0}", TypeToInt(_protocolType));
            this.sOrderby = " ORDER BY Abiturient.RegNum, Abiturient.BAckDoc ";

            InitializeComponent();

            InitControls();
        }

        //дополнительная инициализация контролов
        protected virtual void InitControls()
        {
            InitFocusHandlers();
            drh = new DataRefreshHandler(UpdateDataGrid);
            prh = new ProtocolRefreshHandler(UpdateProtocolList);
            prh += new ProtocolRefreshHandler(SelectLastProtocol);
            MainClass.AddHandler(drh);
            MainClass.AddProtocolHandler(prh);

            Dgv = dgvProtocols;

            cbPrint.Items.Clear();

            InitGrid();
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    FillComboFaculty();
                    //ComboServ.FillCombo(cbFaculty, HelpClass.GetComboListByTable("ed.qFaculty", "ORDER BY Acronym"), false, false);
                    ComboServ.FillCombo(cbStudyBasis, HelpClass.GetComboListByTable("ed.StudyBasis", "ORDER BY Name"), false, false);

                    FillComboStudyLevelGroup();
                    FillComboStudyForm();
                    UpdateProtocolList();
                  
                    cbFaculty.SelectedIndexChanged += new EventHandler(cbFaculty_SelectedIndexChanged);
                    cbStudyBasis.SelectedIndexChanged += new EventHandler(cbStudyBasis_SelectedIndexChanged);
                    cbStudyForm.SelectedIndexChanged += new EventHandler(cbStudyForm_SelectedIndexChanged);
                    cbProtocolNum.SelectedIndexChanged += new EventHandler(cbProtocolNum_SelectedIndexChanged);
                    cbStudyLevelGroup.SelectedIndexChanged += cbStudyLevelGroup_SelectedIndexChanged;
                }
                
                if (MainClass.RightsSov_SovMain_FacMain())
                    btnCreate.Visible = true;
                
                // закрываем для создания новых кроме огр набора
                //if (MainClass.HasAddRightsForPriem(FacultyId.Value, null, null, null, StudyFormId.Value, StudyBasisId.Value))
                //    btnCreate.Enabled = true;

                if (MainClass.IsOwner())
                    btnMake.Visible = true;               

            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка при инициализации формы протоколов: ", ex);
            }
        }

        void cbStudyLevelGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateProtocolList();
        }

        void cbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillComboStudyLevelGroup();
            FillComboStudyForm();
            UpdateProtocolList();
        }      
        void cbStudyForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateProtocolList();
        }
        void cbStudyBasis_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateProtocolList();
        }
        void cbProtocolNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetProtocolInfo();
            UpdateDataGrid();
        }

        private void FillComboStudyForm()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lst = ((from ent in MainClass.GetEntry(context)
                            where ent.FacultyId == FacultyId                           
                            select new
                            {
                                ent.StudyFormId,
                                ent.StudyFormName
                            }).Distinct()).ToList().Select(x => new KeyValuePair<string, string>(x.StudyFormId.ToString(), x.StudyFormName)).ToList();

                ComboServ.FillCombo(cbStudyForm, lst, false, false);
            }
        }
        private void FillComboStudyLevelGroup()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lst = ((from ent in MainClass.GetEntry(context)
                            select new
                            {
                                ent.StudyLevelGroupId,
                                ent.StudyLevelGroupName
                            }).Distinct()).ToList().Select(x => new KeyValuePair<string, string>(x.StudyLevelGroupId.ToString(), x.StudyLevelGroupName)).ToList();

                ComboServ.FillCombo(cbStudyLevelGroup, lst, false, false);
            }
        }
        private void FillComboFaculty()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lst = ((from ent in MainClass.GetEntry(context)
                            select new
                            {
                                ent.FacultyId,
                                ent.FacultyName,
                                ent.FacultyAcr
                            }).Distinct()).ToList().OrderBy(x => x.FacultyAcr).Select(x => new KeyValuePair<string, string>(x.FacultyId.ToString(), x.FacultyName)).ToList();

                ComboServ.FillCombo(cbFaculty, lst, false, false);
            }
        }

        //обновление списка протоколов
        private int _currentProtocolIndex = 0;
        public void UpdateProtocolList(object sender, EventArgs e)
        {
            try
            {
                _currentProtocolIndex = cbProtocolNum.SelectedIndex;

                using (PriemEntities context = new PriemEntities())
                {
                    int protType = TypeToInt(_protocolType);                    
                    var protocols = (from p in context.qProtocol
                                     where !p.IsOld && p.ProtocolTypeId == protType
                                     && p.FacultyId == FacultyId
                                     && p.StudyFormId == StudyFormId
                                     && p.StudyBasisId == StudyBasisId
                                     && p.StudyLevelGroupId == StudyLevelGroupId
                                     orderby p.Number
                                     select new
                                     {
                                         p.Id,
                                         p.Number
                                     }).ToList().Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Number)).ToList();

                    ComboServ.FillCombo(cbProtocolNum, protocols, true, false);
                } 
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка при обновлении списка протоколов: ", ex);
            }
        }

        //данные протокола
        protected void GetProtocolInfo()
        {
            try
            {
                if (ProtocolNumId == null)
                    return;

                using (PriemEntities context = new PriemEntities())
                {
                    var info = context.qProtocol.Where(x => x.Id == ProtocolNumId).
                        Select(x => new { x.Date, x.Number, x.Reason }).FirstOrDefault();

                    ProtocolDate = info.Date;//??? 
                    ProtocolName = info.Number;
                    ProtocolReason = info.Reason;
                }                
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка при взятии данных протокола: ", ex);
            }
        }

        //подготовка грида
        virtual protected void InitGrid()
        {
            dgvProtocols.Columns.Add("Id", "Id");
            dgvProtocols.Columns.Add("Red", "Red");
            dgvProtocols.Columns.Add("Black", "Black");
            dgvProtocols.Columns.Add("Num", "Рег_номер");
            dgvProtocols.Columns.Add("FIO", "ФИО");
            dgvProtocols.Columns.Add("Sum", "Сумма баллов");
            dgvProtocols.Columns.Add("Attestat", "Документ_об_образовании");
            dgvProtocols.Columns.Add("Spec", "Направление");
            dgvProtocols.Columns.Add("Konkurs", "Конкурс");
            dgvProtocols.Columns.Add("Notes", "Примечания");

            dgvProtocols.Columns["Id"].Visible = false;
            dgvProtocols.Columns["Red"].Visible = false;
            dgvProtocols.Columns["Black"].Visible = false;
        }

        //обновление грида
        public virtual void UpdateDataGrid()
        {
            ////закрываем для создания новых кроме огр набора
            //if (MainClass.HasAddRightsForPriem(FacultyId.Value, null, null, null, StudyFormId.Value, StudyBasisId.Value))
            //    btnCreate.Enabled = true;
            //else
            //    btnCreate.Enabled = false;

            //скрыли/показали кнопку, если надо           
            if (ProtocolNumId == null)
            {
                btnMake.Enabled = false;
                dgvProtocols.Rows.Clear();
                dgvProtocols.Update();
                return;
            }
            else
            {
                btnMake.Enabled = true;
            }

            //подчистили строки
            dgvProtocols.Rows.Clear();

            //обработали номер            
            string sFilters = string.Format(" AND ProtocolHistory.ProtocolId = '{0}'", ProtocolNumId.ToString());

            //заполнили грид
            try
            {
                DataSet ds = MainClass.Bdc.GetDataSet(sQuery + sWhere + sFilters + sOrderby);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataGridViewRow r = new DataGridViewRow();
                    r.CreateCells(dgvProtocols, dr["Id"], dr["Red"], dr["Black"], dr["Рег_номер"], dr["ФИО"], dr["Сумма_баллов"], dr["Документ_об_образовании"], dr["Направление"], dr["Конкурс"], ((int)dr["Black"] != 0 ? "двойные оценки" : dr["Примечания"]));
                    if (bool.Parse(dr["Red"].ToString()))
                    {
                        r.Cells[3].Style.ForeColor = Color.Red;
                        r.Cells[4].Style.ForeColor = Color.Red;
                    }

                    dgvProtocols.Rows.Add(r);
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error(ex);
            }
        }

        //сбор фильтров 
        protected string GetFilterString(string sTable)
        {
            string s = string.Empty;

            //обработали форму обучения             
            if (StudyFormId != null)
                s += string.Format(" AND {0}.StudyFormId = {1}", sTable, StudyFormId);

            //обработали основу обучения           
            if (StudyBasisId != null)
                s += string.Format(" AND {0}.StudyBasisId = {1}", sTable, StudyBasisId);

            //обработали факультет           
            if (FacultyId != null)
                s += string.Format(" AND {0}.FacultyId = {1}", sTable, FacultyId);

            if (StudyLevelGroupId != null)
                s += string.Format(" AND {0}.StudyLevelGroupId = {1}", sTable, StudyLevelGroupId);

            return s;
        }

        //закрытие
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //печать
        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintProtocol();
        }

        protected virtual void PrintProtocol()
        {
            return;
        }

        //изменение - только для супер
        private void btnMake_Click(object sender, EventArgs e)
        {
            OpenExistingCard();
        }

        protected virtual void OpenExistingCard()
        {
            if (MainClass.IsOwner())
            {
                ProtocolCard p = new ProtocolCard(this, StudyLevelGroupId.Value, FacultyId.Value, StudyBasisId.Value, StudyFormId.Value, ProtocolNumId);
                p.Show();
            }
        }

        //карточка
        private void btnCard_Click(object sender, EventArgs e)
        {
            OpenCard();
        }

        //открытие карточки
        private void OpenCard()
        {
            if (dgvProtocols.CurrentCell != null && dgvProtocols.CurrentCell.RowIndex > -1)
            {
                string abId = dgvProtocols.Rows[dgvProtocols.CurrentCell.RowIndex].Cells["Id"].Value.ToString();
                if (abId != "")
                {
                    MainClass.OpenCardAbit(abId, this, dgvProtocols.CurrentCell.RowIndex);
                }
            }
        }

        //создать новый протокол
        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (!MainClass.RightsSov_SovMain_FacMain())
                return;

            if (StudyFormId != null)
                MainClass.OpenNewProtocol(this, StudyLevelGroupId.Value, FacultyId.Value, StudyFormId.Value, StudyBasisId.Value, _protocolType);
        }

        //открытие карточки по двойному клику
        private void dgvProtocols_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                OpenCard();
        }

        //на закрытие карточки 
        private void ProtocolList_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainClass.RemoveHandler(drh);
            MainClass.RemoveProtocolHandler(prh);
        }

        //выбрать протокол в списке
        public void SelectProtocol(string name)
        {
            cbProtocolNum.SelectedItem = name;
        }

        public void SelectLastProtocol()
        {
            cbProtocolNum.SelectedIndex = _currentProtocolIndex == 0 ? cbProtocolNum.Items.Count - 1 : _currentProtocolIndex;
        }

        public static int TypeToInt(ProtocolTypes type)
        {
            switch (type)
            {
                case ProtocolTypes.EnableProtocol: { return 1; }
                case ProtocolTypes.DisEnableProtocol: { return 2; }
                case ProtocolTypes.ChangeCompCelProtocol: { return 3; }
                case ProtocolTypes.EntryView: { return 4; }
                case ProtocolTypes.DisEntryView: { return 5; }
                case ProtocolTypes.ChangeCompBEProtocol: { return 6; }
                case ProtocolTypes.DisEntryFromReEnterView: { return 7; }
            }
            return 0;
        }       

        private void UpdateProtocolList()
        {
            UpdateProtocolList(null, null);
        }        
    }
}
