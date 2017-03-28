﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EducServLib;
using BaseFormsLib;

namespace PriemLib
{
    public partial class OlympBookList : BookList
    {
        public OlympBookList()
        {
            InitializeComponent();
            
            Dgv = dgvOlymps;
            _tableName = "ed.OlympBook";
            _title = "Список олимпиад";

            InitControls();
        }

        //дополнительная инициализация контролов
        protected override void ExtraInit()
        {
            base.ExtraInit();

            using (PriemEntities context = new PriemEntities())
            {
                List<KeyValuePair<string, string>> lstYears = new List<KeyValuePair<string, string>>();
                for (int i = MainClass.iPriemYear; i > MainClass.iPriemYear - 5; i--)
                    lstYears.Add(new KeyValuePair<string, string>(i.ToString(), i.ToString()));

                ComboServ.FillCombo(cbOlympYear, lstYears, false, false);

                ComboServ.FillCombo(cbOlympType, HelpClass.GetComboListByTable("ed.OlympType", "ORDER BY Id "), false, false);
                cbOlympType.SelectedIndexChanged += new EventHandler(cbOlympType_SelectedIndexChanged);
                cbOlympYear.SelectedIndexChanged += new EventHandler(cbOlympType_SelectedIndexChanged);
            }

            this.Width = 1174;
        }

        void cbOlympType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }

        protected override void OpenCard(string id, BaseFormEx formOwner, int? index)
        {
            new CardOlympBook(id).Show();
        }

        public override void InitHandlers()
        {
            MainClass.AddHandler(UpdateDataGrid);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            MainClass.RemoveHandler(UpdateDataGrid);
        }

        protected override void GetSource()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var query = from ob in context.extOlympBook
                            orderby ob.Number, ob.OlympNameName, ob.OlympLevelId
                            select new
                            {
                                ob.Id,
                                ob.OlympTypeId,
                                ob.Number,
                                ob.OlympTypeName,
                                ob.OlympNameName,
                                ob.OlympProfileName,
                                ob.OlympSubjectName,
                                ob.OlympLevelName,
                                ob.OlympYear
                            };

                int? olTypeId = ComboServ.GetComboIdInt(cbOlympType);
                if (olTypeId != null)
                    query = query.Where(c => c.OlympTypeId == olTypeId);

                int? olTypeYear = ComboServ.GetComboIdInt(cbOlympYear);
                if (olTypeYear != null)
                    query = query.Where(c => c.OlympYear == olTypeYear);

                try
                {
                    DataTable tblSource = Converter.ConvertToDataTable(query.ToArray());
                    Dgv.DataSource = tblSource;
                    SetVisibleColumnsAndNameColumns();
                }
                catch (Exception ex)
                {
                    WinFormsServ.Error(ex);
                }
            }
        }

        protected override void SetVisibleColumnsAndNameColumns()
        {
            Dgv.AutoGenerateColumns = false;

            foreach (DataGridViewColumn col in Dgv.Columns)
            {
                col.Visible = false;
            }

            SetVisibleColumnsAndNameColumnsOrdered("OlympTypeName", "Тип", 0);
            SetVisibleColumnsAndNameColumnsOrdered("OlympNameName", "Название", 1);

            SetVisibleColumnsAndNameColumnsOrdered("OlympProfileName", "Профиль", 2);
            SetVisibleColumnsAndNameColumnsOrdered("OlympSubjectName", "Предмет", 3);
            SetVisibleColumnsAndNameColumnsOrdered("OlympLevelName", "Уровень", 4);
            SetVisibleColumnsAndNameColumnsOrdered("OlympYear", "Год", 5);

            Dgv.Columns["OlympTypeName"].Width = 192;
            Dgv.Columns["OlympNameName"].Width = 590;
            Dgv.Columns["OlympSubjectName"].Width = 191;
            Dgv.Columns["OlympLevelName"].Width = 100;
        }

        protected override void DeleteSelectedRows(string sId)
        {
            if (!MainClass.IsEntryChanger())
                return;

            using (PriemEntities context = new PriemEntities())
            {
                Guid gId = new Guid(sId);
                context.OlympBook_Delete(gId);
            }
        }
    }
}
