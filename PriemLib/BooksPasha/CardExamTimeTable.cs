using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BaseFormsLib;
using EducServLib;

namespace PriemLib
{
    public partial class CardExamTimeTable : Form
    {
        List<ExamenBlockUnit> lstUnit;
        Guid? UnitId
        {
            get { return ComboServ.GetComboIdGuid(cbUnitList); }
            set { ComboServ.SetComboId(cbUnitList, value); }
        }
        int? TimetableId; 

        public CardExamTimeTable(List<ExamenBlockUnit> l)
        {
            InitializeComponent();
            this.MdiParent = MainClass.mainform;
            this.Text = "Расписание экзаменов";
            lstUnit = l;
            FillCard(); 
        }

        private void FillCard()
        {
            cbUnitList.DataSource = (from lst in lstUnit
                                       select new KeyValuePair<string, string>(
                                           lst.UnitId.ToString(),
                                           lst.ExamUnitName
                                       )).ToList();
            cbUnitList.DisplayMember = "Value" ;
            cbUnitList.ValueMember = "Key";
        }
        private void FillDataGrid()
        {
            string query = @"select 
Id
, ExamDate as 'Дата экзамена'
, Address as 'Место проведения'
, DateOfClose as 'Дата окончания регистрации'
from dbo.ExamTimeTable 
where ExamInEntryBlockUnitId = @Id ";
            DataTable tbl = MainClass.BdcOnlineReadWrite.GetDataSet(query, new SortedList<string, object>() { { "@Id", UnitId.ToString() } }).Tables[0];

            dgv.DataSource = tbl;
            if (dgv.Columns.Contains("Id"))
                dgv.Columns["Id"].Visible = false;
        }

        private void FillTimeTable()
        {
            if (TimetableId != null)
            {
                string query = @"select 
            Id
            , ExamDate  
            , Address  
            , DateOfClose 
            from dbo.ExamTimeTable 
            where Id = @Id ";
                DataTable tbl = MainClass.BdcOnlineReadWrite.GetDataSet(query, new SortedList<string, object>() { { "@Id", TimetableId.ToString() } }).Tables[0];
                tbExamAddress.Text = tbl.Rows[0].Field<string>("Address");
                dtpDateOfClose.Value = tbl.Rows[0].Field<DateTime>("DateOfClose");
                dtpExamDate.Value = tbl.Rows[0].Field<DateTime>("ExamDate");
                btnSave.Text = "Обновить";
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (TimetableId.HasValue)
            {
                string query = @"update dbo.ExamTimeTable
                set ExamDate=@ExamDate, 
                Address=@Address, 
                DateOfClose= @DateOfClose
                where Id = @Id ";
                MainClass.BdcOnlineReadWrite.ExecuteQuery(query, new SortedList<string, object>() { { "@Id", TimetableId }, { "@ExamDate", dtpExamDate.Value }, { "@Address", tbExamAddress.Text }, { "@DateOfClose", dtpDateOfClose.Value } });
            }
            else
            {
                string query = @"insert into  dbo.ExamTimeTable
                (ExamDate, Address, DateOfClose, ExamInEntryBlockUnitId) VALUES (@ExamDate, @Address, @DateOfClose, @ExamInEntryBlockUnitId)";
                MainClass.BdcOnlineReadWrite.ExecuteQuery(query, new SortedList<string, object>() { { "@ExamDate", dtpExamDate.Value }, { "@Address", tbExamAddress.Text }, { "@DateOfClose", dtpDateOfClose.Value }, { "@ExamInEntryBlockUnitId", UnitId} });
            }
            FillDataGrid();
        }

        private void cbUnitList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataGrid();
        }

        private void dgv_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentCell != null)
                TimetableId = int.Parse(dgv.CurrentRow.Cells["Id"].Value.ToString());

            FillTimeTable();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            btnSave.Text = "Добавить";
            TimetableId = null;
            tbExamAddress.Text = "";
            dtpExamDate.Value = DateTime.Now.AddMonths(1);
            dtpDateOfClose.Value = DateTime.Now.AddMonths(1);
        }

    }
}
