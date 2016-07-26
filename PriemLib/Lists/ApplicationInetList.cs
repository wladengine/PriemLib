using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EducServLib;
using BDClassLib;
using BaseFormsLib;
using System.Threading.Tasks;
using System.Threading;

namespace PriemLib
{
    public partial class ApplicationInetList : BookList
    {
        protected DBPriem _bdcInet;       
        private LoadFromInet loadClass;
        private BackgroundWorker bw;
        private CancellationTokenSource _cancel = new CancellationTokenSource();

        //конструктор
        public ApplicationInetList()
        {            
            InitializeComponent();
            Dgv = dgvAbiturients;

            _title = "Список online заявлений для загрузки";            
                        
            InitControls();            
        }

        #region Fields
        public DBPriem BdcInet
        {
            get { return _bdcInet; }
        }
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
        public Guid? ProfileId
        {
            get
            {
                string prId = ComboServ.GetComboId(cbProfile);
                if (string.IsNullOrEmpty(prId))
                    return null;
                else
                    return new Guid(prId);
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
        #endregion

        protected override void ExtraInit()
        {
            base.ExtraInit();

            btnCard.Visible = btnAdd.Visible = btnRemove.Visible = false;
            
            loadClass = new LoadFromInet();
            _bdcInet = loadClass.BDCInet;

            if (MainClass.IsReadOnly())
                btnLoad.Enabled = false;

            btnSetInvisible.Visible = MainClass.IsPasha();

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    ComboServ.FillCombo(cbFaculty, HelpClass.GetComboListByTable("ed.qFaculty", "ORDER BY Acronym"), false, true);
                    ComboServ.FillCombo(cbStudyBasis, HelpClass.GetComboListByTable("ed.StudyBasis", "ORDER BY Name"), false, true);

                    cbStudyBasis.SelectedIndex = 0;
                    FillLicenseProgram();
                    FillObrazProgram();
                    FillProfile();

                    bw = new BackgroundWorker();
                    bw.WorkerSupportsCancellation = true;
                    bw.DoWork += bw_DoWork;
                    bw.RunWorkerCompleted += bw_RunWorkerCompleted;

                    if (MainClass.IsPasha())
                        gbUpdateImport.Visible = true;
                    else
                        gbUpdateImport.Visible = false;

                    chbSelectAll.Checked = false;

                    tbAbitBarcode.Focus();
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы ", exc);
            } 
        }

        private void FillLicenseProgram()
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<KeyValuePair<string, string>> lst =
                    ((from ent in MainClass.GetEntry(context)
                      where ent.FacultyId == FacultyId
                      select new
                      {
                          Id = ent.LicenseProgramId,
                          Name = ent.LicenseProgramName
                      }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                ComboServ.FillCombo(cbLicenseProgram, lst, false, true);
                cbLicenseProgram.SelectedIndex = 0;
            }
        }
        private void FillObrazProgram()
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<KeyValuePair<string, string>> lst =
                    ((from ent in MainClass.GetEntry(context)
                      where ent.FacultyId == FacultyId
                      && (LicenseProgramId.HasValue ? ent.LicenseProgramId == LicenseProgramId : true)
                      select new
                      {
                          Id = ent.ObrazProgramId,
                          Name = ent.ObrazProgramName,
                          Crypt = ent.ObrazProgramCrypt
                      }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name + ' ' + u.Crypt)).ToList();

                ComboServ.FillCombo(cbObrazProgram, lst, false, true);
            }
        }
        private void FillProfile()
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<KeyValuePair<string, string>> lst =
                    ((from ent in MainClass.GetEntry(context)
                      where ent.FacultyId == FacultyId
                      && (LicenseProgramId.HasValue ? ent.LicenseProgramId == LicenseProgramId : true)
                      && (ObrazProgramId.HasValue ? ent.ObrazProgramId == ObrazProgramId : true)
                      select new
                      {
                          Id = ent.ProfileId,
                          Name = ent.ProfileName
                      }).Distinct()).ToList().Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name)).ToList();

                if (lst.Count() > 0)
                {
                    ComboServ.FillCombo(cbProfile, lst, false, true);
                    cbProfile.Enabled = true;
                }
                else
                {
                    ComboServ.FillCombo(cbProfile, new List<KeyValuePair<string, string>>(), true, false);
                    cbProfile.Enabled = false;
                }
            }
        }

        #region Handlers
        //инициализация обработчиков мегакомбов
        public override void InitHandlers()
        {
            cbFaculty.SelectedIndexChanged += new EventHandler(cbFaculty_SelectedIndexChanged);
            cbLicenseProgram.SelectedIndexChanged += new EventHandler(cbLicenseProgram_SelectedIndexChanged);
            cbObrazProgram.SelectedIndexChanged += new EventHandler(cbObrazProgram_SelectedIndexChanged);
            cbProfile.SelectedIndexChanged += new EventHandler(cbProfile_SelectedIndexChanged);
            cbStudyBasis.SelectedIndexChanged += new EventHandler(cbStudyBasis_SelectedIndexChanged);            
        }
        void cbStudyBasis_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }
        void cbProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        } 
        void cbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillLicenseProgram();
            //UpdateDataGrid();
        }
        void cbLicenseProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillObrazProgram();
            //UpdateDataGrid();
        }
        void cbObrazProgram_SelectedIndexChanged(object sender, EventArgs e)
        {            
            FillProfile();
            UpdateDataGrid();
        }
        #endregion

        //строим запрос фильтров
        private string GetFilterString()
        {
            string s = string.Format("AND qAbiturient.StudyLevelGroupId IN ({0})", Util.BuildStringWithCollection(MainClass.lstStudyLevelGroupId));

            //ограничение
            using (PriemEntities context = new PriemEntities())
            {
                List<int> lstFacs = context.qFaculty.Select(x => x.Id).ToList();
                if (lstFacs.Count > 0)
                    s += "AND qAbiturient.FacultyId IN (" + lstFacs.Select(x => x.ToString()).Aggregate((x, tail) => x + "," + tail) + ") ";
            }

            //обработали основу обучения 
            if (StudyBasisId != null)
                s += string.Format(" AND qAbiturient.StudyBasisId = {0}", StudyBasisId);

            //обработали факультет            
            if (FacultyId != null)
                s += string.Format(" AND qAbiturient.FacultyId = {0}", FacultyId);         

            //обработали Направление
            if (LicenseProgramId != null)
                s += string.Format(" AND qAbiturient.LicenseProgramId = {0}", LicenseProgramId); 

            //обработали Образ программу
            if (ObrazProgramId != null)
                s += string.Format(" AND qAbiturient.ObrazProgramId = {0}", ObrazProgramId); 

            //обработали специализацию 
            if (ProfileId != null)
                s += string.Format(" AND qAbiturient.ProfileId = '{0}'", ProfileId); 

            return s;
        }       

        //обновление грида
        protected override void GetSource()
        {
            if (bw.IsBusy)
                return;

            var __bdcInet = new LoadFromInet().BDCInet;

            bw.RunWorkerAsync(new { dgv = this.dgvAbiturients, _bdc = __bdcInet, filters = GetFilterString() });

            SetControlsEnableStatus(false);
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cbFaculty.Enabled = true;

            if (!e.Cancelled)
            {
                HelpClass.FillDataGrid(this.Dgv, (DataView)e.Result);

                lblCount.Text = "Всего: " + dgvAbiturients.RowCount.ToString();
                btnCard.Enabled = (dgvAbiturients.RowCount != 0);

                DataGridViewButtonColumn col = new DataGridViewButtonColumn();

                dgvAbiturients.Columns["ФИО"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
                btnLoad.Enabled = !(dgvAbiturients.RowCount == 0);
            }

            lblCount.Text = "Всего: " + dgvAbiturients.RowCount.ToString();
            SetControlsEnableStatus(true);
        }

        private void SetControlsEnableStatus(bool status)
        {
            cbFaculty.Enabled = status;
            cbLicenseProgram.Enabled = status;
            cbObrazProgram.Enabled = status;
            cbProfile.Enabled = status;
            cbStudyBasis.Enabled = status;
            tbAbitBarcode.Enabled = status;
            tbSearch.Enabled = status;

            btnCard.Enabled = status;
            btnClose.Enabled = status;
            btnRemove.Enabled = status;
            //btnUpdate.Enabled = status;
            gbWait.Visible = !status;
        }

        async void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker _bw = sender as BackgroundWorker;

            _orderBy = " ORDER BY [Приложены файлы], [Дата обновления файлов] DESC, ФИО";
            if (MainClass.dbType == PriemType.PriemAG)
            {
                _sQuery = @"SELECT DISTINCT qAbiturient.CommitId AS Id, extPerson.Surname + ' ' + extPerson.Name + ' ' + extPerson.SecondName as ФИО, 
                       extPerson.BirthDate AS Дата_рождения, qAbiturient.CommitNumber AS Barcode, 
                       (Case When EXISTS(SELECT extAbitFileNames.Id FROM extAbitFileNames WHERE extAbitFileNames.PersonId = extPerson.Id) then 'Да' else 'Нет' end) AS [Приложены файлы],
                       (CASE WHEN EXISTS(SELECT * FROM qAbitFiles_OnlyEssayMotivLetter q WHERE q.PersonId=qAbiturient.PersonId AND FileTypeId=2) THEN 'Да' ELSE 'Нет' END) AS [Мотивац письмо],
                       (SELECT Max(extAbitFileNames.LoadDate) FROM extAbitFileNames WHERE extAbitFileNames.PersonId = extPerson.Id AND (extAbitFileNames.CommitId = qAbiturient.CommitId OR extAbitFileNames.CommitId IS NULL)) AS [Дата обновления файлов]
                       FROM qAbiturient INNER JOIN extPerson ON qAbiturient.PersonId = extPerson.Id
                       WHERE qAbiturient.IsImported = 0 AND SemesterId = 1 AND Enabled = 1 AND IsCommited = 1 AND IsVisible = 1 AND qAbiturient.IsForeign = 0";
            }
            else { 
            _sQuery = @"SELECT DISTINCT qAbiturient.CommitId AS Id, extPerson.Surname + ' ' + extPerson.Name + ' ' + extPerson.SecondName as ФИО, 
                       extPerson.BirthDate AS Дата_рождения, qAbiturient.CommitNumber AS Barcode, 
                       (Case When EXISTS(SELECT extAbitFileNames_ALL.Id FROM extAbitFileNames_ALL WHERE extAbitFileNames_ALL.PersonId = extPerson.Id) then 'Да' else 'Нет' end) AS [Приложены файлы],
                       (SELECT Max(extAbitFileNames_ALL.LoadDate) FROM extAbitFileNames_ALL WHERE extAbitFileNames_ALL.PersonId = extPerson.Id AND (extAbitFileNames_ALL.CommitId = qAbiturient.CommitId OR extAbitFileNames_ALL.CommitId IS NULL)) AS [Дата обновления файлов],
                       (CASE WHEN EXISTS(SELECT * FROM qAbitFiles_OnlyEssayMotivLetter q WHERE q.PersonId=qAbiturient.PersonId AND FileTypeId=2) THEN 'Да' ELSE 'Нет' END) AS [Мотивац письмо],
                       (CASE WHEN EXISTS(SELECT * FROM qAbitFiles_OnlyEssayMotivLetter q WHERE q.PersonId=qAbiturient.PersonId AND FileTypeId=3) THEN 'Да' ELSE 'Нет' END) AS [Эссе],
                       (CASE WHEN EXISTS(SELECT * FROM qAbitFiles_OnlyPhilosophy q WHERE q.PersonId=qAbiturient.PersonId) THEN 'Да' ELSE 'Нет' END) AS [Философия]
                       FROM qAbiturient INNER JOIN extPerson ON qAbiturient.PersonId = extPerson.Id
                       WHERE qAbiturient.IsImported = 0 AND SemesterId = 1 AND Enabled = 1 AND IsCommited = 1 AND IsVisible = 1 AND qAbiturient.IsForeign = 0";
}
            Task<DataView> task = HelpClass.GetDataViewAsync((DataGridView)((dynamic)e.Argument).dgv, (BDClass)((dynamic)e.Argument)._bdc, _sQuery, (string)((dynamic)e.Argument).filters, _orderBy, false, _cancel.Token);

            while (!task.IsCompleted)
            {
                if (_bw.CancellationPending)
                {
                    e.Cancel = true;
                    //_cancel.Cancel();
                    return;
                }

                System.Threading.Thread.Sleep(25);
            }

            e.Result = await task;
        }

        //поле поиска
        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            WinFormsServ.Search(this.dgvAbiturients, "ФИО", tbSearch.Text);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {   
                string barcText = tbAbitBarcode.Text.Trim();

                if (barcText.Length > 0)
                {
                    if (barcText.Length == 7)
                    {
                        if (barcText.StartsWith("1"))
                        {
                            WinFormsServ.Error("Выбран человек, подавший заявления на первый курс");
                            return;
                        }
                        barcText = barcText.Substring(1);
                    }

                    int code;
                    if (!int.TryParse(barcText, out code))
                    {
                        WinFormsServ.Error("Не распознан баркод!");
                        return;
                    }
                    if (code == 0)
                    {
                        WinFormsServ.Error("Не распознан баркод!");
                        return;
                    }

                    if (code >= 100000 && code < 199999)
                    {
                        WinFormsServ.Error("Указан баркод поступающего в бакалавриат!");
                        return;
                    }

                    if (code > 99999)
                        code = code % 100000;

                    string query = "SELECT COUNT(*) FROM [Abiturient] WHERE ApplicationCommitNumber=@Number";
                    int cnt = (int)BdcInet.GetValue(query, new SortedList<string, object>() { { "@Number", code } });

                    if (cnt == 0)
                    {
                        MessageBox.Show("Данное заявление было удалено в личном кабинете");
                        UpdateDataGrid();
                        return;
                    }

                    query = "SELECT IsPrinted FROM [Abiturient] WHERE ApplicationCommitNumber=@Number";
                    bool bIsPrinted = (bool?)BdcInet.GetValue(query, new SortedList<string, object>() { { "@Number", code } }) ?? false;

                    if (!bIsPrinted)
                    {
                        var dr = MessageBox.Show(@"Данное заявление не было распечатано в личном кабинете. 
Это означает, что абитуриент всё ещё может изменить список конкурсов и их приоритетность. 
Если с момента подачи прошло много времени, рекомендуется связаться с абитуриентом.
Желаете открыть карточку перегрузки?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (dr == System.Windows.Forms.DialogResult.No)
                            return;
                    }

                    using (PriemEntities context = new PriemEntities())
                    {
                        var EntryIds_workBase = (from ab in context.Abiturient
                                        where ab.CommitNumber == code
                                        select ab.EntryId).Distinct().ToList();

                        query = "SELECT EntryId FROM [Abiturient] WHERE ApplicationCommitNumber=@Number";
                        DataTable tblEntryIds_onlineBase = BdcInet.GetDataSet(query, new SortedList<string, object>() { { "@Number", code } }).Tables[0];
                        var EntryIds_onlineBase = (from DataRow rw in tblEntryIds_onlineBase.Rows
                                                   select rw.Field<Guid>("EntryId")).ToList();

                        cnt = EntryIds_onlineBase.Except(EntryIds_workBase).Count();
                            //(from ab in context.Abiturient
                            // where ab.CommitNumber == code
                            // select ab).Count();

                        if (cnt == 0)
                        {
                            WinFormsServ.Error("Запись уже добавлена!");
                            return;
                        }

                        for (int i = 0; i < dgvAbiturients.Rows.Count; i++)
                        {
                            if (dgvAbiturients.Rows[i].Cells["Barcode"].Value.ToString() == code.ToString())
                            {
                                dgvAbiturients.CurrentCell = dgvAbiturients[1, i];
                                break;
                            }
                        }

                        tbAbitBarcode.Text = string.Empty;

                        CardFromInet crd = new CardFromInet(null, code, false);
                        crd.ToUpdateList += new UpdateListHandler(UpdateDataGrid);
                        crd.Show();
                    }
                }
            }            
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка при заполнении формы ", ex);
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy)
            {
                bw.CancelAsync();
                System.Threading.Thread.Sleep(25);
            }
            else
                UpdateDataGrid();
        }

        protected override void OnClosed()
        {
            if (bw.IsBusy)
            {
                bw.CancelAsync();
                System.Threading.Thread.Sleep(25);
            }
            base.OnClosed();
            //loadClass.CloseDB();
        }

        private void dgvAbiturients_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgvAbiturients.Columns["ФИО"].Index && dgvAbiturients["Приложены файлы", e.RowIndex].Value.ToString() == "Да")
            {
                dgvAbiturients["ФИО", e.RowIndex].Style.BackColor = Color.LightGreen;
            }
        }

        private void btnUpdateImport_Click(object sender, EventArgs e)
        {
            if (!MainClass.IsPasha())
                return;            

            if (MessageBox.Show("Обновить IsImported = true?", "Обновление", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (DataGridViewRow dgvr in dgvAbiturients.SelectedRows)
                {
                    string sId = dgvr.Cells["Id"].Value.ToString();
                    try
                    {
                        string query; 
                        query = string.Format("UPDATE [Application] SET IsImported = 1 WHERE Id = '{0}'",  sId);                        

                        _bdcInet.ExecuteQuery(query);
                    }
                    catch (Exception ex)
                    {
                        WinFormsServ.Error("Ошибка обновления данных", ex);
                        goto Next;
                    }
                Next: ;
                }
                UpdateDataGrid();
            }
        }

        private void chbSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            dgvAbiturients.MultiSelect = chbSelectAll.Checked;
            btnLoad.Enabled = !chbSelectAll.Checked;
        }

        protected override void Dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Dgv.CurrentCell != null && Dgv.CurrentCell.RowIndex > -1)            
                tbAbitBarcode.Text = Dgv.Rows[Dgv.CurrentCell.RowIndex].Cells["Barcode"].Value.ToString();

            btnLoad_Click(null, null);
        }

        private void btnPrintToExcel_Click(object sender, EventArgs e)
        {
            PrintClass.PrintAllToExcel(this.dgvAbiturients);
        }

        private void btnSetInvisible_Click(object sender, EventArgs e)
        {
            if (Dgv.CurrentCell != null && Dgv.CurrentCell.RowIndex > -1)
            {
                string sId = Dgv.Rows[Dgv.CurrentCell.RowIndex].Cells["Barcode"].Value.ToString();

                string query = "UPDATE ApplicationCommit SET IsVisible = 0 WHERE [IntNumber] = '" + sId + "'";
                _bdcInet.ExecuteQuery(query);
                MessageBox.Show("OK");
                UpdateDataGrid();
            }
        }       
    }
}