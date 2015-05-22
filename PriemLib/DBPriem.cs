using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

using BDClassLib;
using EducServLib;
using System.Threading.Tasks;

namespace PriemLib
{
    public partial class DBPriem : SQLClass
    {
        string ExamNameQuery = "SELECT Id, Name as 'Название', Acronym as 'Сокращение', NamePad as 'Дательный падеж' from ed.ExamName order by name";
        public DataTable GetExamName()
        {
            DataTable dataTable = null;
            using (SqlCommand comm = new SqlCommand(ExamNameQuery, _cn))
            {
                using (SqlDataAdapter adapt = new SqlDataAdapter(comm))
                {
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapt);

                    dataTable = new DataTable();

                    adapt.Fill(dataTable);
                    adapt.FillSchema(dataTable, SchemaType.Mapped);
                    dataTable.Columns["Id"].AutoIncrement = true;
                    dataTable.Columns["Id"].AutoIncrementSeed = 0;
                    dataTable.Columns["Id"].AutoIncrementStep = -1;
                }
            }

            return dataTable;
        }

        public void SetExamName(DataTable dataTable)
        {
            SetTable(dataTable, ExamNameQuery);

            return;
        }

        string EgeExamQuery = "SELECT Id, Name as 'Название', FBSnumber as 'Порядковый номер в ФБС', EgeMin as 'Минимальная планка' from ed.EgeExamName order by FBSnumber";
        public DataTable GetEgeExam()
        {
            DataTable dataTable = null;
            using (SqlCommand comm = new SqlCommand(EgeExamQuery, _cn))
            {
                using (SqlDataAdapter adapt = new SqlDataAdapter(comm))
                {
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapt);

                    dataTable = new DataTable();

                    adapt.Fill(dataTable);
                    adapt.FillSchema(dataTable, SchemaType.Mapped);
                    dataTable.Columns["Id"].AutoIncrement = true;
                    dataTable.Columns["Id"].AutoIncrementSeed = 0;
                    dataTable.Columns["Id"].AutoIncrementStep = -1;
                }
            }

            return dataTable;
        }

        public void SetEgeExam(DataTable dataTable)
        {
            SetTable(dataTable, EgeExamQuery);

            return;
        }

        public void SetTable(DataTable dataTable, string query)
        {
            using (SqlCommand comm = new SqlCommand(query, _cn))
            {
                using (SqlDataAdapter adapt = new SqlDataAdapter(comm))
                {
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapt);

                    adapt.Update(dataTable);
                }
            }

            return;
        }

        public DataTable GetMinEge()
        {
            DataTable dataTable = null;
            string MinEgeQuery =
                @"SELECT 
                ed.extExamInEntry.Id, ed.qEntry.FacultyAcr as 'Факультет',
                ed.qEntry.ObrazProgramCrypt as 'Код',
                ed.qEntry.LicenseProgramName as 'Направление',
                ed.qEntry.ObrazProgramName as 'Образовательная_программа', 
                ed.qEntry.ProfileName as 'Профиль',                 
                ed.qEntry.StudyFormName AS 'Форма обучения',
                ed.qEntry.StudyBasisName AS 'Основа обучения',
                ed.extExamInEntry.ExamName as 'Экзамен',
                ed.extExamInEntry.EgeMin AS 'Мин_планка'               
                FROM ed.extExamInEntry INNER JOIN ed.qEntry ON ed.extExamInEntry.EntryId=ed.qEntry.Id 
                ORDER BY ed.qEntry.FacultyAcr, ed.qEntry.ObrazProgramCrypt, ed.qEntry.ProfileName, ed.extExamInEntry.ExamName, ed.qEntry.StudyFormName, ed.qEntry.StudyBasisName ";

            using (SqlCommand comm = new SqlCommand(MinEgeQuery, _cn))
            {
                using (SqlDataAdapter adapt = new SqlDataAdapter(comm))
                {
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapt);

                    dataTable = new DataTable();

                    adapt.Fill(dataTable);
                    adapt.FillSchema(dataTable, SchemaType.Mapped);
                }
            }

            return dataTable;
        }

        public void SetMinEge(DataTable dataTable)
        {
            using (PriemEntities context = new PriemEntities())
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    context.ExamInEntry_UpdateMinEge(row.Field<int?>("Мин_планка"), row.Field<int>("Id"));
                }
            }
            return;
        }

        public async Task<DataTable> GetDataTableAsync(string query, SortedList<string, object> slParams)
        {
            return GetDataSet(query, slParams).Tables[0];
        }
    }

    //public class AsyncReader : System.ComponentModel.INotifyPropertyChanged
    //{
    //    private IEnumerable<dynamic> _readResult;
    //    public IEnumerable<dynamic> ReadResult
    //    {
    //        get
    //        {
    //            return _readResult;
    //        }
    //    }

    //    private DataTable _tbl;
    //    public DataTable Table
    //    {
    //        get
    //        {
    //            return _tbl;
    //        }
    //    }

    //    private IAsyncResult _asyncResult;

    //    private bool _cancelled;

    //    private object _sync;

    //    private SqlCommand _command;

    //    private List<string> _lstFields;

    //    public AsyncReader(List<string> lstFields)
    //    {
    //        this._cancelled = false;
    //        this._sync = new object();
    //        _lstFields = lstFields;
    //    }

    //    private void EndExecCommand(IAsyncResult result)
    //    {
    //        if (result.IsCompleted)
    //        {
    //            List<object> resList = new List<object>();
    //            int cancelCheckCount = 0;
    //            SqlDataReader reader = _command.EndExecuteReader(result);
    //            for (int i = 0; i < reader.FieldCount; i++)
 
    //            while (reader.Read())
    //            {
    //                cancelCheckCount++;
    //                if (cancelCheckCount / 100 == 0)
    //                {
    //                    lock (_sync)
    //                    {
    //                        if (_cancelled)
    //                        {
    //                            return;
    //                        }
    //                    }
    //                }
    //            }
    //            this._readResult = resList;
    //            if (PropertyChanged != null)
    //            {
    //                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("ReadResult"));
    //            }
    //        }
    //    }

    //    public void BeginRead(SqlCommand command)
    //    {
    //        _command = command;
    //        _asyncResult = _command.BeginExecuteReader(new AsyncCallback(EndExecCommand), null);
    //    }
    //    public void CancelRead()
    //    {
    //        lock (_sync)
    //        {
    //            _cancelled = true;
    //            _command.Cancel();
    //        }
    //    }

    //    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    //}
}
