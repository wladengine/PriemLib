﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Data;

using EducServLib;
using BDClassLib;

namespace PriemLib
{
    public class DocsClass
    {
        private DBPriem _bdcInet;
        private string _personId;
        private string _abitId;
        private string _commitId;
        private bool _bShowAllFiles;

        public DocsClass(int personBarcode, int? abitCommitBarcode, bool bShowAllFiles)
        {
            _bdcInet = new DBPriem();
            _bShowAllFiles = bShowAllFiles;

            try
            {
                _bdcInet.OpenDatabase(MainClass.connStringOnline);
            }
            catch (Exception exc)
            {
                WinFormsServ.Error(exc);
            }

            _personId = _bdcInet.GetStringValue("SELECT Person.Id FROM Person WHERE Person.Barcode = " + personBarcode);

            if (abitCommitBarcode == null)
                _abitId = null;
            else
            {
                _abitId = _bdcInet.GetStringValue("SELECT qAbiturient.Id FROM qAbiturient WHERE qAbiturient.CommitNumber = " + abitCommitBarcode);
                _commitId = _bdcInet.GetStringValue("SELECT qAbiturient.CommitId FROM qAbiturient WHERE qAbiturient.CommitNumber = " + abitCommitBarcode);
            }
        }

        public DBPriem BDCInet
        {
            get { return _bdcInet; }
        }

        public void CloseDB()
        {
            _bdcInet.CloseDataBase();
        }

        public void OpenFile(List<KeyValuePair<string, string>> lstFiles)
        {
            try
            {
                foreach (KeyValuePair<string, string> file in lstFiles)
                {
                    byte[] bt = _bdcInet.ReadFile(string.Format("SELECT FileData FROM extAbitFiles_All WHERE Id = '{0}'", file.Key));

                    string filename = file.Value.Replace(@"\", "-").Replace(@":", "-");

                    StreamWriter sw = new StreamWriter(MainClass.saveTempFolder + filename);
                    BinaryWriter bw = new BinaryWriter(sw.BaseStream);
                    bw.Write(bt);
                    bw.Flush();
                    bw.Close();
                    Process.Start(MainClass.saveTempFolder + filename);
                }
            }
            catch (System.Exception exc)
            {
                WinFormsServ.Error("Ошибка открытия файла: ", exc);
            }
        }

        public List<KeyValuePair<string, string>> UpdateFiles()
        {
            try
            {
                if (_personId == null)
                    return null;

                List<KeyValuePair<string, string>> lstFiles = new List<KeyValuePair<string, string>>();

                string query = string.Format("SELECT Id, FileName + ' (' + convert(nvarchar, extAbitFiles.LoadDate, 104) + ' ' + convert(nvarchar, extAbitFiles.LoadDate, 108) + ')' + FileExtention AS FileName, IsDeleted FROM {3} AS extAbitFiles WHERE extAbitFiles.PersonId = '{0}' {1} {2}", _personId,
                    !string.IsNullOrEmpty(_abitId) ? " AND (extAbitFiles.ApplicationId = '" + _abitId + "' OR extAbitFiles.ApplicationId IS NULL)" : "",
                    !string.IsNullOrEmpty(_commitId) ? " AND (extAbitFiles.CommitId = '" + _commitId + "' OR extAbitFiles.CommitId IS NULL)" : "",
                    _bShowAllFiles ? "extAbitFiles_All" : "extAbitFiles");

                DataSet ds = _bdcInet.GetDataSet(query + " ORDER BY extAbitFiles.LoadDate DESC");
                foreach (DataRow dRow in ds.Tables[0].Rows)
                {
                    lstFiles.Add(new KeyValuePair<string, string>(dRow["Id"].ToString(), dRow["FileName"].ToString()));
                }

                return lstFiles;
            }
            catch (System.Exception exc)
            {
                WinFormsServ.Error("Ошибка обновления данных о приложениях: ", exc);
                return null;
            }
        }
        public List<KeyValuePair<string, string>> UpdateFiles(string sFilters)
        {
            try
            {
                if (_personId == null)
                    return null;

                List<KeyValuePair<string, string>> lstFiles = new List<KeyValuePair<string, string>>();

                string query = string.Format("SELECT Id, FileName + ' (' + convert(nvarchar, extAbitFiles.LoadDate, 104) + ' ' + convert(nvarchar, extAbitFiles.LoadDate, 108) + ')' + FileExtention AS FileName, IsDeleted FROM {4} AS extAbitFiles WHERE extAbitFiles.PersonId = '{0}' {1} {2} {3}", _personId,
                    !string.IsNullOrEmpty(_abitId) ? " AND (extAbitFiles.ApplicationId = '" + _abitId + "' OR extAbitFiles.ApplicationId IS NULL)" : "",
                    !string.IsNullOrEmpty(_commitId) ? " AND (extAbitFiles.CommitId = '" + _commitId + "' OR extAbitFiles.CommitId IS NULL)" : "",
                    !string.IsNullOrEmpty(sFilters) ? sFilters : "",
                    _bShowAllFiles ? "extAbitFileNames_All" : "extAbitFileNames");
                DataSet ds = _bdcInet.GetDataSet(query + " ORDER BY extAbitFiles.LoadDate DESC");
                foreach (DataRow dRow in ds.Tables[0].Rows)
                {
                    lstFiles.Add(new KeyValuePair<string, string>(dRow["Id"].ToString(), dRow["FileName"].ToString()));
                }

                return lstFiles;
            }
            catch (System.Exception exc)
            {
                WinFormsServ.Error("Ошибка обновления данных о приложениях: ", exc);
                return null;
            }
        }

        public DataTable UpdateFilesTable(string sFilters = "")
        {
            try
            {
                if (_personId == null)
                    return null;
                DataTable tbl = new DataTable();


                string query = string.Format("SELECT Id, FileName + ' (' + convert(nvarchar, extAbitFiles.LoadDate, 104) + ' ' + convert(nvarchar, extAbitFiles.LoadDate, 108) + ')' + FileExtention AS FileName, Comment, FileTypeName, FileExtention, IsDeleted  FROM {3} extAbitFiles WHERE extAbitFiles.PersonId = '{0}' {1} {2}", _personId,
                    !string.IsNullOrEmpty(_abitId) ? " AND (extAbitFiles.ApplicationId = '" + _abitId + "' OR extAbitFiles.ApplicationId IS NULL)" : "",
                    !string.IsNullOrEmpty(_commitId) ? " AND (extAbitFiles.CommitId = '" + _commitId + "' OR extAbitFiles.CommitId IS NULL)" : "",
                    _bShowAllFiles ? "extAbitFileNames_All" : "extAbitFileNames");

                DataSet ds = _bdcInet.GetDataSet(query + (sFilters ?? "") + " ORDER BY extAbitFiles.LoadDate DESC");

                if (ds.Tables[0] != null)
                    tbl = ds.Tables[0];

                return tbl;
            }
            catch (System.Exception exc)
            {
                WinFormsServ.Error("Ошибка обновления данных о приложениях: ", exc);
                return null;
            }
        }
    }
}
