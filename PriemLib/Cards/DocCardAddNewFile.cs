using EducServLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PriemLib
{
    public partial class DocCardAddNewFile : Form
    {
        private int? FileTypeId
        {
            get { return ComboServ.GetComboIdInt(cbFileType); }
        }
        private string Comment
        {
            get { return tbComment.Text.Trim(); }
        }
        private string Type;
        private int PersonBarcode;
        private DBPriem _bdcInet;

        public event Action ToUpdateList;

        public DocCardAddNewFile(string _type, int _personBarcode)
        {
            InitializeComponent();
            Type = _type;
            PersonBarcode = _personBarcode;
            this.Text = "Добавление нового файла";
            this.MdiParent = MainClass.mainform;

            _bdcInet = new DBPriem();
            try
            {
                _bdcInet.OpenDatabase(MainClass.connStringOnline);
            }
            catch (Exception exc)
            {
                WinFormsServ.Error(exc);
            }

            UpdateFileTypeCombo();
        }

        private void UpdateFileTypeCombo()
        {
            string query = "SELECT Id, Name FROM PersonFileType ";
            if (Type == "PersonFile")
                query = string.Format(query, "PersonFileType");
            else
                query = string.Format(query, "FileType");

            DataTable tbl = _bdcInet.GetDataSet(query).Tables[0];
            var src = 
                (from DataRow rw in tbl.Rows
                 select new KeyValuePair<string, string>(rw["Id"].ToString(), rw["Name"].ToString())
                ).ToList();

            ComboServ.FillCombo(cbFileType, src, false, false);
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbFilePath.Text = ofd.FileName;
            }
        }

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            string FileName = tbFilePath.Text.Trim();

            if (!FileTypeId.HasValue)
                WinFormsServ.Error("Не выбран тип файла!");
            if (string.IsNullOrEmpty(tbFilePath.Text.Trim()))
                WinFormsServ.Error("Не выбран файл!");

            if (File.Exists(FileName))
            {
                byte[] data = File.ReadAllBytes(FileName);
                string fNameShort = FileName.Substring(FileName.LastIndexOf('\\') + 1);
                string fNameExt = fNameShort.Substring(fNameShort.LastIndexOf('.'));

                Guid PersonId = (Guid)_bdcInet.GetValue("SELECT Id FROM Person WHERE Barcode = " + PersonBarcode.ToString());

                SortedList<string, object> sl = new SortedList<string, object>();
                sl.Add("@PersonId", PersonId);
                sl.Add("@FileName", fNameShort);
                sl.Add("@FileExtention", fNameExt);
                sl.Add("@FileSize", data.Length);
                sl.Add("@FileData", data);
                sl.Add("@Comment", Comment);
                sl.Add("@LoadDate", DateTime.Now);
                sl.Add("@PersonFileTypeId", FileTypeId);

                if (Type == "PersonFile")
                {
                    sl.Add("@FileId", Guid.NewGuid());
                    _bdcInet.ExecuteQuery(@"INSERT INTO 
FileStorage (Id, FileData) 
VALUES (@FileId, @FileData)", sl);
                    _bdcInet.ExecuteQuery(@"INSERT INTO 
PersonFile (Id, PersonId, FileName, FileExtention, FileSize, Comment, LoadDate, PersonFileTypeId) 
VALUES (@FileId, @PersonId, @FileName, @FileExtention, @FileSize, @Comment, @LoadDate, @PersonFileTypeId)", sl);
                }

                if (ToUpdateList != null)
                    ToUpdateList();
                
                this.Close();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _bdcInet.CloseDataBase();
            base.OnClosing(e);
        }
    }
}
