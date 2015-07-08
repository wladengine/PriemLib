using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

using EducServLib;
using BDClassLib;
using WordOut;
using BaseFormsLib;
using System.Threading.Tasks;

namespace PriemLib
{
    public partial class ListAbit : FormFilter
    {
        private DataRefreshHandler _drh;
        private DBPriem _bdc;
        private BackgroundWorker bw;

        private bool sorted = false;
        private int index = 0;
        private ListSortDirection order;
        private string sortedColumn;

        public string FacultyId
        {
            get
            {
                return ComboServ.GetComboId(cbFaculty);
            }
        }

        //конструктор
        public ListAbit(Form own)
        {
            InitializeComponent();

            InitControls();
            InitHandlers();
            UpdateDataGrid();
            SetSort();
            lblCount.Text = dgvAbitList.RowCount.ToString();
            tbNumber.Focus();
        }

        //конструктор
        public ListAbit(string sFaculty, string where)
        {
            InitializeComponent();
            InitControls();
            cbFaculty.SelectedItem = sFaculty;            
            InitHandlers();
            List<string> tableList = new List<string>();
            tableList.Add("Competition");
            UpdateDataGrid(where, tableList);
            SetSort();
            lblCount.Text = dgvAbitList.RowCount.ToString();
            tbNumber.Focus();
        }

        //дополнительная инициализация контролов
        private void InitControls()
        {
            try
            {
                InitFocusHandlers();

                this.CenterToScreen();
                this.MdiParent = MainClass.mainform;
                _bdc = MainClass.Bdc;
                _drh = new DataRefreshHandler(UpdateDataGrid);
                MainClass.AddHandler(_drh);

                bw = new BackgroundWorker();
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += bw_DoWork;
                bw.RunWorkerCompleted += bw_RunWorkerCompleted;

                Dgv = dgvAbitList;

                lblCount.Text = dgvAbitList.RowCount.ToString();
                if (!MainClass.IsOwner())
                    btnRemove.Visible = false;

                using (PriemEntities context = new PriemEntities())
                {
                    ComboServ.FillCombo(cbFaculty, HelpClass.GetComboListByTable("ed.qFaculty", "ORDER BY Acronym"), false, true);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы ", exc);
            }
        }
        
        private void SetSort()
        {
            if (dgvAbitList.Columns.Contains("ФИО"))
                dgvAbitList.Sort(dgvAbitList.Columns["ФИО"], ListSortDirection.Ascending);
        }

        //инициализация обработчиков мегакомбов
        private void InitHandlers()
        {
            cbFaculty.SelectedIndexChanged +=new EventHandler(cbFaculty_SelectedIndexChanged); 
        }

        void cbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {           
            UpdateDataGrid();
        }

        //установить фильтры
        public void SetFilters(string sFaculty, string sGridQuery)
        {
            cbFaculty.SelectedItem = sFaculty;            

            HelpClass.FillDataGrid(dgvAbitList, _bdc, sGridQuery, "");
            lblCount.Text = dgvAbitList.RowCount.ToString();
        }

        //открытие карточки для конкретного человека
        private void OpenAbitCard()
        {
            if (dgvAbitList.CurrentCell != null && dgvAbitList.CurrentCell.RowIndex > -1)
            {
                string abId = dgvAbitList.Rows[dgvAbitList.CurrentCell.RowIndex].Cells["Id"].Value.ToString();
                if (abId != "")
                    MainClass.OpenCardAbit(abId, this, dgvAbitList.CurrentRow.Index);
            }
        }

        //кнопка карточка -> открыть карточку
        private void btnCard_Click(object sender, EventArgs e)
        {
            OpenAbitCard();
        }

        //кнопка удалить
        private void btnRemove_Click(object sender, EventArgs e)
        {
        //    if (_bdc.RightsPashaOlia())
        //    {
        //        if (MessageBox.Show("Удалить записи?", "Удаление", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //        {
        //            foreach (DataGridViewRow dgvr in dgvAbitList.SelectedRows)
        //            {
        //                string sId = dgvr.Cells["Id"].Value.ToString();
        //                try
        //                {
        //                    string query;
        //                    query = string.Format("DELETE FROM Abiturient WHERE Id = '{0}'", sId);
        //                    _bdc.ExecuteQuery(query);
        //                }
        //                catch (Exception ex)
        //                {
        //                    WinFormsServ.Error("Ошибка удаления данных", ex);
        //                    goto Next;
        //                }
        //            Next: ;
        //            }
        //            MainClass.DataRefresh();
        //        }
        //    }
        }

        //обновление грида
        public override void UpdateDataGrid()
        {
            string sFilters = GetFilterString();
            string sOrderBy = GetOrderByString();
            DoUpdate(sFilters, sOrderBy, null);
        }

        private void UpdateDataGrid(string where, List<string> tableList)
        {
            DoUpdate(where, string.Empty, tableList);
        }

        private void DoUpdate(dynamic data)
        {
            DoUpdate(data.sFilters, data.sOrderBy, null);
        }
        private void DoUpdate(string filters, string orderby, List<string> tableList)
        {
            List<string> lTables = tableList == null ? new List<string>() : tableList;

            if (_list != null)
                foreach (iFilter i in _list)
                    if (i is Filter)
                    {
                        string sTable = (i as Filter).GetFilterItem().Table;
                        if (!lTables.Contains(sTable))
                            lTables.Add(sTable);
                    }

            if (!MainClass._config.ColumnListAbit.Contains("Рег_номер"))
                MainClass._config.ColumnListAbit.Add("Рег_номер");
            if (!MainClass._config.ColumnListAbit.Contains("ФИО"))
                MainClass._config.ColumnListAbit.Add("ФИО");


            sortedColumn = string.Empty;
            order = ListSortDirection.Ascending;
            index = dgvAbitList.CurrentRow == null ? -1 : dgvAbitList.CurrentRow.Index;

            if (dgvAbitList.SortOrder != SortOrder.None)
            {
                sorted = true;
                sortedColumn = dgvAbitList.SortedColumn.Name;
                order = dgvAbitList.SortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending;
            }

            //stop previous
            if (bw.IsBusy)
            {
                bw.CancelAsync();
                gbWait.Visible = false;
                return;
            }

            DBPriem __bdc = new DBPriem();
            __bdc.OpenDatabase(MainClass.connString);

            bw.RunWorkerAsync(new { dgv = this.dgvAbitList, bdc = __bdc, query = MainClass.qBuilder.GetQuery(MainClass._config.ColumnListAbit, lTables, null), filters = filters, orderby = orderby });

            cbFaculty.Enabled = false;
            btnCard.Enabled = false;
            btnClose.Enabled = false;
            btnColumns.Enabled = false;
            btnExcel.Enabled = false;
            btnFile.Enabled = false;
            btnFilters.Enabled = false;
            btnGroup.Enabled = false;
            btnPrint.Enabled = false;
            btnRemove.Enabled = false;
            //btnUpdate.Enabled = false;
            btnUpdate.Text = "Отмена";
            gbWait.Visible = true;
        }

        async void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker _bw = sender as BackgroundWorker;
            //e.Result = HelpClass.GetDataView(((dynamic)e.Argument).dgv, ((dynamic)e.Argument).bdc, ((dynamic)e.Argument).query, ((dynamic)e.Argument).filters, ((dynamic)e.Argument).orderby);
            Task<DataView> task = HelpClass.GetDataViewAsync((DataGridView)((dynamic)e.Argument).dgv, (BDClass)((dynamic)e.Argument).bdc, 
                (string)((dynamic)e.Argument).query, (string)((dynamic)e.Argument).filters, (string)((dynamic)e.Argument).orderby, false);

            while (!task.IsCompleted && !task.IsFaulted)
            {
                System.Threading.Thread.Sleep(25);

                if (_bw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }

            try
            {
                e.Result = await task;
            }
            catch (Exception ex)
            {
                WinFormsServ.Error(ex);
            }
        }
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cbFaculty.Enabled = true;

            if (!e.Cancelled)
            {
                HelpClass.FillDataGrid(this.Dgv, (DataView)e.Result);

                if (_groupList == null)
                {
                    if (dgvAbitList.Rows.Count > 0)
                    {
                        if (sorted && dgvAbitList.Columns.Contains(sortedColumn))
                            dgvAbitList.Sort(dgvAbitList.Columns[sortedColumn], order);
                        if (index >= 0 && index <= dgvAbitList.Rows.Count)
                            dgvAbitList.CurrentCell = dgvAbitList[1, index];
                    }
                }
                else
                    _groupList = null;

                lblCount.Text = dgvAbitList.RowCount.ToString();
                btnCard.Enabled = (dgvAbitList.RowCount != 0);
            }

            cbFaculty.Enabled = true;
            btnCard.Enabled = true;
            btnClose.Enabled = true;
            btnColumns.Enabled = true;
            btnExcel.Enabled = true;
            btnFile.Enabled = true;
            btnFilters.Enabled = true;
            btnGroup.Enabled = true;
            btnPrint.Enabled = true;
            btnRemove.Enabled = true;
            //btnUpdate.Enabled = true;
            btnUpdate.Text = "Обновить";
            gbWait.Visible = false;
        }

        #region Search
        //поиск по номеру
        private void tbNumber_TextChanged(object sender, EventArgs e)
        {
            try
            {
                WinFormsServ.Search(this.dgvAbitList, "Рег_номер", tbNumber.Text);
            }
            catch
            {
            }
        }

        //поиск по ид. номеру
        private void tbPersonNumber_TextChanged(object sender, EventArgs e)
        {
            try
            {
                WinFormsServ.Search(this.dgvAbitList, "Ид_номер", tbPersonNumber.Text);
            }
            catch
            {
            }
        }

        //поиск по фио
        private void tbFIO_TextChanged(object sender, EventArgs e)
        {
            try
            {
                WinFormsServ.Search(this.dgvAbitList, "ФИО", tbFIO.Text);
            }
            catch
            {
                try
                {
                    WinFormsServ.Search(this.dgvAbitList, "Фамилия", tbFIO.Text);
                }
                catch
                {
                }
            }
        }
        #endregion

        //строит строку ордер-бай
        private string GetOrderByString()
        {
            string res = string.Empty;

            if (_groupList != null)
            {
                res += " ORDER BY ";
                res += Util.BuildStringWithCollection(_groupList, true);
                res += ", Рег_номер";
            }
            else
                res = "";

            return res;
        }

        private string GetFilterString()
        {
            string s = " WHERE 1=1 ";
            if (MainClass.dbType != PriemType.PriemForeigners)
                s += " AND qAbiturient.IsForeign <> 1";
            else
                s += " AND qAbiturient.IsForeign = 1";

            s += MainClass.GetStLevelFilter("ed.qAbiturient");           
            
            //обработали факультет
            
            if (!string.IsNullOrEmpty(FacultyId))
                s += " AND ed.qAbiturient.FacultyId = " + FacultyId;

            if (_list != null)
                s += StrangeAboutFilters.BuildFilterWithCollection(_list);

            return s;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ListAbit_Activated(object sender, EventArgs e)
        {
            HelpClass.FillDataGridAsync(dgvAbitList, _bdc, MainClass.qBuilder.GetQuery(MainClass._config.ColumnListAbit, null), "");
            lblCount.Text = dgvAbitList.RowCount.ToString();
        }
        private void ListAbit_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainClass.RemoveHandler(_drh);
            bw.CancelAsync();
        }

        private void dgvAbitList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                OpenAbitCard();
        }

        private void btnColumns_Click(object sender, EventArgs e)
        {
            Columns frm = new Columns(this, FacultyId);
            frm.ShowDialog();
        }                
        private void btnFilters_Click(object sender, EventArgs e)
        {
            Filters f = new Filters(this, _bdc, _list);
            f.ShowDialog();
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy)
                bw.CancelAsync();
            else
                MainClass.DataRefresh();
        }

        //группировки
        private void btnGroup_Click(object sender, EventArgs e)
        {
            Groups gr = new Groups(this);
            gr.ShowDialog();
        }

        #region Печать 
        
        private void btnPrint_Click(object sender, EventArgs e)
        {
            List<string> columnList = new List<string>();
            foreach (DataGridViewColumn column in dgvAbitList.Columns)
                if (column.Visible)
                    columnList.Add(column.Name);

            try
            {
                WordDoc wd = new WordDoc(string.Format(@"{0}\Templates\ListAbit.dot", Application.StartupPath));

                wd.AddNewTable(dgvAbitList.Rows.Count + 1, columnList.Count + 1);
                TableDoc td = wd.Tables[0];

                //переменные
                int? FacId = ComboServ.GetComboIdInt(cbFaculty);
                string sFac = cbFaculty.Text;
                if (cbFaculty.Text == ComboServ.DISPLAY_ALL_VALUE)
                    sFac = "Все факультеты";
                
                wd.Fields["Faculty"].Text = sFac;
                //wd.Fields["Section"].Text = sForm;

                int i = 0;

                td[0, 0] = "№ п/п";
                for (int j = 0; j < columnList.Count; j++)
                    td[j + 1, 0] = dgvAbitList.Columns[columnList[j]].HeaderText;

                // печать из грида
                foreach (DataGridViewRow dgvr in dgvAbitList.Rows)
                {
                    td[0, i + 1] = (i + 1).ToString();
                    for (int j = 0; j < columnList.Count; j++)
                        td[j + 1, i + 1] = dgvAbitList.Rows[i].Cells[columnList[j]].Value.ToString();

                    i++;
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка вывода в Word: \n", exc);
            }
        }

        //печать в текст
        private void btnFile_Click(object sender, EventArgs e)
        {
            if (sfdFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamWriter sw = new StreamWriter(sfdFile.OpenFile());
                    sw.WriteLine("№ \t Рег.ном. \t Фамилия Имя Отчество");
                    if (!_groupPrint)
                    {
                        int i = 1;
                        foreach (DataGridViewRow dgvr in dgvAbitList.Rows)
                            sw.WriteLine(string.Format("{0} \t {1} \t {2}", i++, dgvr.Cells["Рег_номер"].Value, dgvr.Cells["ФИО"].Value));
                    }
                    else
                    {
                        List<string> temp = null;
                        int i = 1;
                        foreach (DataGridViewRow dgvr in dgvAbitList.Rows)
                        {
                            List<string> ls = BuildStringWithRow(dgvr);

                            if (Util.ListCompare(ls, temp))
                                sw.WriteLine(string.Format("{0} \t {1} \t {2}", i++, dgvr.Cells["Рег_номер"].Value, dgvr.Cells["ФИО"].Value));
                            else
                            {
                                i = 1;
                                temp = ls;
                                sw.WriteLine("Группа");
                                for (int j = 0; j < _groupList.Count; j++)
                                {
                                    sw.WriteLine(string.Format("{0}: {1}", _groupList[j].Name, ls[j]));
                                }
                                sw.WriteLine(string.Format("{0} \t {1} \t {2}", i++, dgvr.Cells["Рег_номер"].Value, dgvr.Cells["ФИО"].Value));
                            }
                        }
                    }

                    sw.Close();
                }
                catch (Exception ex)
                {
                    WinFormsServ.Error("Ошибка при записи файла: ", ex);
                }

            }
        }

        private List<string> BuildStringWithRow(DataGridViewRow dgvr)
        {
            List<string> list = new List<string>();
            foreach (ListItem li in _groupList)
                list.Add(dgvr.Cells[li.Id].Value.ToString());

            return list;
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            PrintClass.PrintAllToExcel(Dgv);
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            //if (dgvAbitList.CurrentRow.Index >= 0)
            //{
            //    _bdc.ExecuteQuery(string.Format("INSERT INTO _FirstWave (AbiturientId, SortNum, Green) VALUES ('{0}',6666,0)",dgvAbitList.CurrentRow.Cells["Id"].Value.ToString()));
            //}

            //MessageBox.Show("Done! " + dgvAbitList.CurrentRow.Cells["ФИО"].Value.ToString());
        }
    }
}