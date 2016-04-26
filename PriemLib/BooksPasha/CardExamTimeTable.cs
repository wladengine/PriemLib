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
            lbExamTimeTableRestriction.DisplayMember = "Value";
            lbExamTimeTableRestriction.ValueMember = "Key";
        }
        private void FillBaseExamTimeTable()
        {
            if (!UnitId.HasValue)
                ComboServ.FillCombo(cbBaseExamTimeTable, new List<KeyValuePair<string, string>>(), true, false);
            else
            {
                string query = @" select 
  Id, (convert(nvarchar(50), ExamDate, 6) +' (' +Address+')') as Name
  from dbo.ExamTimetable
  where ExamInEntryBlockUnitId = @UnitId ";
                DataTable tbl = MainClass.BdcOnlineReadWrite.GetDataSet(query, new SortedList<string, object>() { { "@UnitId", UnitId.ToString() } }).Tables[0];
                List<KeyValuePair<string, string>> lst = (from DataRow t in tbl.Rows
                                                          select new KeyValuePair<string, string>(t.Field<int>("Id").ToString(), t.Field<string>("Name"))).ToList();

                ComboServ.FillCombo(cbBaseExamTimeTable, lst, true, false);
            }
        }
        private void FillRestriction()
        {
            if (!UnitId.HasValue)
            {
                lbExamTimeTableRestriction.DataSource = new List<KeyValuePair<string, string>>();
            }
            else
            {
                string query = @"with t as (
  select 
	ExamTimetable.Id,  ExamDate , ExamName.Name , ExamTimeTable.Address, Entry.StudyLevelId 
  from dbo.ExamTimetable 
  join dbo.ExamInEntryBlockUnit on ExamInEntryBlockUnit.Id = ExamTimetable.ExamInEntryBlockUnitId
  join dbo.Exam on ExamInEntryBlockUnit.ExamId = Exam.Id
  join dbo.ExamName on ExamName.Id = Exam.ExamNameId
  join dbo.ExamInEntryBlock on ExamInEntryBlock.Id = ExamInEntryBlockUnit.ExamInEntryBlockId
  join dbo.Entry on ExamInEntryBlock.EntryId = Entry.Id
  )
   select 
	t.Id, (convert(nvarchar(50), t.ExamDate, 6) + ' '+t.Name +'  (' +t.Address+')') as Name 
, (case when Exists (select * from dbo.ExamTimeTableOneDayRestriction where (ExamTimetableId1 = t.Id and ExamTimetableId2 = 26) 
	or (ExamTimetableId2 = t.Id and ExamTimetableId1 = 26)) then '1' else '0' end) as Selected
	from t
  where 
  CONVERT(date, t.ExamDate)  = (select CONVERT(date, t.ExamDate) from t where t.Id =  @Id )
  and t.StudyLevelId = (select StudyLevelId from t where t.Id = @Id)
and t.Id <> @Id;";
                DataTable tbl = MainClass.BdcOnlineReadWrite.GetDataSet(query, new SortedList<string, object>() { { "@Id", TimetableId.ToString() } }).Tables[0];
                var Lst = (from DataRow t in tbl.Rows
                           select new {
                               Id = t.Field<int>("Id").ToString(),
                               Name =  t.Field<string>("Name"),
                               Selected =  t.Field<string>("Selected")=="1",
                           }).ToList();
                List<KeyValuePair<string, string>> lst = (from t in Lst
                                                          select new KeyValuePair<string, string>(t.Id, t.Name)).ToList();
                lbExamTimeTableRestriction.DataSource = lst;
                lbExamTimeTableRestriction.ClearSelected();
                for (int i = 0; i < lbExamTimeTableRestriction.Items.Count; i++ )
                {
                    var x = lbExamTimeTableRestriction.Items[i];
                    KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)x;
                    bool Selected = Lst.Where(t => t.Id == kvp.Key).Select(t => t.Selected).First();
                    if (Selected)
                        lbExamTimeTableRestriction.SelectedItems.Add(x);
                }
            }
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
            , BaseExamTimeTableId
            from dbo.ExamTimeTable 
            where Id = @Id ";
                DataTable tbl = MainClass.BdcOnlineReadWrite.GetDataSet(query, new SortedList<string, object>() { { "@Id", TimetableId.ToString() } }).Tables[0];
                tbExamAddress.Text = tbl.Rows[0].Field<string>("Address");
                dtpDateOfClose.Value = tbl.Rows[0].Field<DateTime>("DateOfClose");
                dtpExamDate.Value = tbl.Rows[0].Field<DateTime>("ExamDate");
                ComboServ.SetComboId(cbBaseExamTimeTable, tbl.Rows[0].Field<int?>("BaseExamTimeTableId"));

                btnSave.Text = "Обновить";
                FillRestriction();
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            int? BaseExamTimeTableId = ComboServ.GetComboIdInt(cbBaseExamTimeTable);
            SortedList<string, object> Dictionary = new SortedList<string, object>();
            if (TimetableId.HasValue)
                Dictionary.Add( "@Id", TimetableId);
            Dictionary.Add("@ExamDate", dtpExamDate.Value);
            Dictionary.Add("@Address", tbExamAddress.Text);
            Dictionary.Add("@DateOfClose", dtpDateOfClose.Value);
            if (BaseExamTimeTableId.HasValue)
                Dictionary.Add("@BaseExamTimeTableId", BaseExamTimeTableId.Value);
            else
                Dictionary.Add("@BaseExamTimeTableId", DBNull.Value);

            if (TimetableId.HasValue)
            {
                string query = @"update dbo.ExamTimeTable
                set ExamDate=@ExamDate, 
                Address=@Address, 
                DateOfClose= @DateOfClose,
                BaseExamTimeTableId = @BaseExamTimeTableId
                where Id = @Id ";
                MainClass.BdcOnlineReadWrite.ExecuteQuery(query, Dictionary);

                string Ids = "";
                foreach (var x in lbExamTimeTableRestriction.Items)
                {
                    KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)x;
                    Ids += kvp.Key + ",";
                }
                Ids = Ids.Length > 0 ? Ids.Substring(0, Ids.Length - 1) : Ids;
                if (!String.IsNullOrEmpty(Ids))
                {
                    query = @"delete from dbo.ExamTimeTableOneDayRestriction
                where (ExamTimetableId1 = @Id and ExamTimetableId1 in (" + Ids + @")) or (ExamTimetableId1 = @Id and ExamTimetableId1 in (" + Ids + "))";
                    MainClass.BdcOnlineReadWrite.ExecuteQuery(query, Dictionary);
                }
                foreach (var x in lbExamTimeTableRestriction.SelectedItems)
                {
                    KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)x;
                    int kvpId = int.Parse(kvp.Key);
                    query = @"  
    IF NOT EXISTS 
    (   SELECT  1
        FROM    dbo.ExamTimeTableOneDayRestriction 
        WHERE   (ExamTimetableId1 = @Id1 and ExamTimetableId2 = @Id) 
	or (ExamTimetableId2 = @Id1 and ExamTimetableId1 = @Id) 
    )
    BEGIN
        INSERT dbo.ExamTimeTableOneDayRestriction  (ExamTimetableId1, ExamTimetableId2) 
        VALUES (@Id1, @Id) 
    END;";
                    MainClass.BdcOnlineReadWrite.ExecuteQuery(query, new SortedList<string, object>() { { "@Id1", kvpId }, { "@Id", TimetableId } });
                }
            }
            else
            {
                string query = @"insert into  dbo.ExamTimeTable
                (ExamDate, Address, DateOfClose, ExamInEntryBlockUnitId, BaseExamTimeTableId) VALUES (@ExamDate, @Address, @DateOfClose, @ExamInEntryBlockUnitId, @BaseExamTimeTableId)";

                Dictionary.Add("@ExamInEntryBlockUnitId", UnitId);
                MainClass.BdcOnlineReadWrite.ExecuteQuery(query, Dictionary);
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
            FillNew();
        }
        public void FillNew()
        {
            btnSave.Text = "Добавить";
            TimetableId = null;
            tbExamAddress.Text = "";
            dtpExamDate.Value = DateTime.Now.AddMonths(1);
            dtpDateOfClose.Value = DateTime.Now.AddMonths(1);
            FillBaseExamTimeTable();
            FillRestriction();
        }

        private void CardExamTimeTable_Load(object sender, EventArgs e)
        {

        }

        private void CardExamTimeTable_Shown(object sender, EventArgs e)
        {
            dgv.ClearSelection();
            FillNew();
        }

    }
}
