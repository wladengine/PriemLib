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

        int? ExamId { get; set; }
        Guid? UnitId
        {
            get { return ComboServ.GetComboIdGuid(cbUnitList); }
            set {
                ComboServ.SetComboId(cbUnitList, value);
            }
        }
        int? TimetableId; 

        public CardExamTimeTable(List<ExamenBlockUnit> l)
        {
            InitializeComponent();
            this.MdiParent = MainClass.mainform;
            this.Text = "Расписание экзаменов";
            lstUnit = l;
            
            
            cbUnitList.DisplayMember = "Value";
            cbUnitList.ValueMember = "Key";
            cbSubject.DisplayMember = "Value";
            cbSubject.ValueMember = "Key";

            FillCard();
        }

        private void FillCard()
        {
            cbUnitList.DataSource = (from lst in lstUnit
                                       select new KeyValuePair<string, string>(
                                           lst.UnitId.ToString(),
                                           lst.ExamUnitName
                                       )).ToList();
            

            cbSubject.DataSource = (from lst in lstUnit
                                       select new KeyValuePair<string, string>(
                                           lst.UnitId.ToString(),
                                           lst.ExamUnitName
                                       )).ToList(); 
            lbExamTimeTableRestriction.DisplayMember = "Value";
            lbExamTimeTableRestriction.ValueMember = "Key";
            ExamId = lstUnit.Where(x => x.UnitId == UnitId).Select(x => x.ExamId).First();
        }
        private void FillBaseExamTimeTable()
        {
            if (!UnitId.HasValue)
                ComboServ.FillCombo(cbBaseExamTimeTable, new List<KeyValuePair<string, string>>(), true, false);
            else
            {
                string query = @" select 
  Id, (convert(nvarchar(50), ExamDate, 6) +' (' +Address+')') as Name
  from dbo.ExamBaseTimetable
  where ExamBaseTimetable.ExamId = @ExamId
order by ExamDate";
                DataTable tbl = MainClass.BdcOnlineReadWrite.GetDataSet(query, new SortedList<string, object>() { { "@ExamId", ExamId.ToString() } }).Tables[0];
                List<KeyValuePair<string, string>> lst = (from DataRow t in tbl.Rows
                                                          select new KeyValuePair<string, string>(t.Field<int>("Id").ToString(), t.Field<string>("Name"))).ToList();

                ComboServ.FillCombo(cbBaseExamTimeTable, lst, true, false);
            }
        }
      /*  private void FillRestriction()
        {
            if (!UnitId.HasValue)
            {
                lbExamTimeTableRestriction.DataSource = new List<KeyValuePair<string, string>>();
            }
            else
            {
                string query = @"with t as (
  select 
	ExamBaseTimetable.Id,  ExamBaseTimetable.ExamDate , ExamName.Name , ExamTimeTable.Address, Entry.StudyLevelId 
  from dbo.ExamBaseTimetable 
  join dbo.ExamInEntryBlockUnit on ExamInEntryBlockUnit.Id = @UnitId
  join dbo.Exam on ExamBaseTimetable.ExamId = Exam.Id
  join dbo.ExamName on ExamName.Id = Exam.ExamNameId
  join dbo.ExamInEntryBlock on ExamInEntryBlock.Id = ExamInEntryBlockUnit.ExamInEntryBlockId
  join dbo.Entry on ExamInEntryBlock.EntryId = Entry.Id
  )
   select 
	t.Id
, (convert(nvarchar(50), t.ExamDate, 6) + ' '+t.Name +'  (' +t.Address+')') as Name 
, (case when Exists (select * from dbo.ExamTimeTableOneDayRestriction where (ExamTimetableId1 = t.Id and ExamTimetableId2 = @Id) 
	or (ExamTimetableId2 = t.Id and ExamTimetableId1 = @Id)) then '1' else '0' end) as Selected
	from t
  where 
 ((CONVERT(date, t.ExamDate)  = (select CONVERT(date, t.ExamDate) from t where t.Id =  @Id )  and t.StudyLevelId = (select StudyLevelId from t where t.Id = @Id))
or
(Exists (select * from dbo.ExamTimeTableOneDayRestriction where (ExamTimetableId1 = t.Id and ExamTimetableId2 = @Id) 
	or (ExamTimetableId2 = t.Id and ExamTimetableId1 = @Id)))
)
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
        }*/
        private void FillDataGrid()
        {
            string query = @"select 
uTT.Id
, ExamDate as 'Дата экзамена'
, Address as 'Место проведения'
, DateOfClose as 'Дата окончания регистрации'
from dbo.ExamInEntryBlockUnitTimetable uTT
join dbo.ExamBasetimetable bTT on bTT.Id = uTT.ExamBaseTimeTableId
where uTT.ExamInEntryBlockUnitId = @Id ";
            DataTable tbl = MainClass.BdcOnlineReadWrite.GetDataSet(query, new SortedList<string, object>() { { "@Id", UnitId.ToString() } }).Tables[0];

            dgv.DataSource = tbl;
            if (dgv.Columns.Contains("Id"))
                dgv.Columns["Id"].Visible = false;
        }
        private void FillDataGridBaseTimetable()
        {
            string query = @"select 
bTT.Id
, ExamDate as 'Дата экзамена'
, Address as 'Место проведения'
, DateOfClose as 'Дата окончания регистрации'
from dbo.ExamBasetimetable bTT  
where bTT.ExamId = @Id 
and bTT.ExamDate > '01-01-" + MainClass.sPriemYear + @"'";
            DataTable tbl = MainClass.BdcOnlineReadWrite.GetDataSet(query, new SortedList<string, object>() { { "@Id", ExamId.ToString() } }).Tables[0];

            dgvExamBaseTimetable.DataSource = tbl;
            if (dgvExamBaseTimetable.Columns.Contains("Id"))
                dgvExamBaseTimetable.Columns["Id"].Visible = false;
            FillBaseExamTimeTable(); 
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

            from dbo.ExamBaseTimeTable 
            where Id = @Id ";
                DataTable tbl = MainClass.BdcOnlineReadWrite.GetDataSet(query, new SortedList<string, object>() { { "@Id", TimetableId.ToString() } }).Tables[0];
                if (tbl.Rows.Count > 0)
                {
                    tbExamAddress.Text = tbl.Rows[0].Field<string>("Address");
                    dtpDateOfClose.Value = tbl.Rows[0].Field<DateTime>("DateOfClose");
                    dtpExamDate.Value = tbl.Rows[0].Field<DateTime>("ExamDate");
                    ComboServ.SetComboId(cbBaseExamTimeTable, -1);

                    btnSave.Text = "Обновить";

                    //FillRestriction();
                }
                else
                {
                    WinFormsServ.Error("Не удалось получить данные с расписания ID=" + TimetableId);
                }
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            SortedList<string, object> Dictionary = new SortedList<string, object>();
            if (TimetableId.HasValue)
                Dictionary.Add( "@Id", TimetableId);
            Dictionary.Add("@ExamDate", dtpExamDate.Value);
            Dictionary.Add("@Address", tbExamAddress.Text);
            Dictionary.Add("@DateOfClose", dtpDateOfClose.Value);
            

            if (TimetableId.HasValue)
            {
                string query = @"update dbo.ExamBaseTimeTable
                set ExamDate=@ExamDate, 
                Address=@Address, 
                DateOfClose= @DateOfClose
                where Id = @Id ";
                MainClass.BdcOnlineReadWrite.ExecuteQuery(query, Dictionary);

//                string Ids = "";
//                foreach (var x in lbExamTimeTableRestriction.Items)
//                {
//                    KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)x;
//                    Ids += kvp.Key + ",";
//                }
//                Ids = Ids.Length > 0 ? Ids.Substring(0, Ids.Length - 1) : Ids;
//                if (!String.IsNullOrEmpty(Ids))
//                {
//                    query = @"delete from dbo.ExamTimeTableOneDayRestriction
//                where (ExamTimetableId1 = @Id and ExamTimetableId2 in (" + Ids + @")) or (ExamTimetableId1 in (" + Ids + ") and ExamTimetableId1= @Id  )";
//                    MainClass.BdcOnlineReadWrite.ExecuteQuery(query, Dictionary);
//                }
//                foreach (var x in lbExamTimeTableRestriction.SelectedItems)
//                {
//                    KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)x;
//                    int kvpId = int.Parse(kvp.Key);
//                    query = @"  
//    IF NOT EXISTS 
//    (   SELECT  1
//        FROM    dbo.ExamTimeTableOneDayRestriction 
//        WHERE   (ExamTimetableId1 = @Id1 and ExamTimetableId2 = @Id) 
//	or (ExamTimetableId2 = @Id1 and ExamTimetableId1 = @Id) 
//    )
//    BEGIN
//        INSERT dbo.ExamTimeTableOneDayRestriction  (ExamTimetableId1, ExamTimetableId2) 
//        VALUES (@Id1, @Id) 
//    END;";
//                    MainClass.BdcOnlineReadWrite.ExecuteQuery(query, new SortedList<string, object>() { { "@Id1", kvpId }, { "@Id", TimetableId } });
//                }
            }
            else
            {
                string query = @"insert into  dbo.ExamBaseTimeTable
                (ExamDate, Address, DateOfClose, ExamId) 
        VALUES (@ExamDate, @Address, @DateOfClose, @ExamId)";

                Dictionary.Add("@ExamId", ExamId);
                MainClass.BdcOnlineReadWrite.ExecuteQuery(query, Dictionary);
            }
            FillDataGrid();
            FillDataGridBaseTimetable();
        }

        private void cbUnitList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboServ.SetComboId(cbSubject, UnitId);
            FillDataGrid();
            FillDataGridBaseTimetable();
            
            ExamId = lstUnit.Where(x => x.UnitId == UnitId).Select(x => x.ExamId).First();

            string query = @"select 
            bTT.Id
            , ExamDate as 'Дата экзамена'
            , Address as 'Место проведения'
            , DateOfClose as 'Дата окончания регистрации'
            from dbo.ExamBasetimetable bTT  
            where bTT.ExamId = @Id 
            and bTT.ExamDate > '01-01-"+MainClass.sPriemYear+@"'
            and bTT.Id not in (select ExamBaseTimetableID from dbo.ExamInEntryBlockUnitTimetable where ExamInEntryBlockUnitId = @UnitId)
";
            DataTable tbl = MainClass.BdcOnlineReadWrite.GetDataSet(query, new SortedList<string, object>() { { "@Id", ExamId.ToString() }, { "@UnitId", UnitId.ToString() } }).Tables[0];

            cbExamInEntryBlockUnitTimetable.DataSource = (from DataRow t in tbl.Rows
                                                          select new KeyValuePair<string, string>(t.Field<int>("Id").ToString(),
                                                              t.Field<DateTime>("Дата экзамена").ToShortDateString() + " в " +t.Field<DateTime>("Дата экзамена").ToShortTimeString() + ", " + t.Field<string>("Место проведения"))).ToList();
            cbExamInEntryBlockUnitTimetable.DisplayMember = "Value";
            cbExamInEntryBlockUnitTimetable.ValueMember = "Key";
        }

        private void dgvExamBaseTimetable_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvExamBaseTimetable.CurrentCell != null)
                TimetableId = int.Parse(dgvExamBaseTimetable.CurrentRow.Cells["Id"].Value.ToString());

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
            //FillRestriction();
        }

        private void CardExamTimeTable_Shown(object sender, EventArgs e)
        {
            dgv.ClearSelection();
            dgvExamBaseTimetable.ClearSelection();

            FillNew();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedCells.Count == 0)
                return;

            int rwInd = dgv.SelectedCells[0].RowIndex;
            string sId = dgv["Id", rwInd].Value.ToString();

            DeleteExamTimeTable(sId);
            FillTimeTable();
        }

        private void DeleteExamTimeTable(string sId)
        {
            try
            {
                string query = @"
DELETE FROM dbo.ExamTimeTable
where Id = @Id";

                int iID = 0;
                int.TryParse(sId, out iID);

                SortedList<string, object> Dictionary = new SortedList<string, object>();
                Dictionary.Add("@Id", iID);

                MainClass.BdcOnlineReadWrite.ExecuteQuery(query, Dictionary);
            }
            catch (Exception ex)
            {
                WinFormsServ.Error(ex);
            }

        }

        private void btnExaminEntryBlockUnitTTAdd_Click(object sender, EventArgs e)
        {
            SortedList<string, object> Dictionary = new SortedList<string, object>();
            Dictionary.Add("@ExamInEntryBlockUnitId", UnitId);
            Dictionary.Add("@ExamBaseTimetableId", ComboServ.GetComboIdInt(cbExamInEntryBlockUnitTimetable));

            string query = @"insert into  dbo.ExamInEntryBlockUnitTimetable
                (ExamInEntryBlockUnitId, ExamBaseTimetableId) 
        VALUES (@ExamInEntryBlockUnitId, @ExamBaseTimetableId)";
            
            MainClass.BdcOnlineReadWrite.ExecuteQuery(query, Dictionary);

            FillDataGrid();
        }
    }
}
