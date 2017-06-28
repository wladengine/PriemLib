﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Entity.Core.Objects;

using BaseFormsLib;
using EducServLib;
using System.Transactions;

namespace PriemLib
{
    public partial class EntryList : BookList
    {
        public EntryList()
        {
            InitializeComponent();
            Dgv = dgvEntry;            
            InitControls();            
        }

        protected override void ExtraInit()
        {
            base.ExtraInit();

            _tableName = "ed.Entry";
            _title = "Конкурс";

            //Dgv.Size = new Size(1031, 270);
            //Dgv.Location = new Point(12, 172);
            //lblCount.Location = new Point(405, 448);            

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    List<KeyValuePair<string, string>> lst = 
                        (from f in context.qFaculty
                         orderby f.Acronym
                         select new { f.Id, f.Name })
                         .ToList()
                         .OrderBy(x => x.Name)
                         .Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name))
                         .ToList();
                    ComboServ.FillCombo(cbFaculty, lst, false, true);

                    lst = (from f in context.SP_AggregateGroup
                           orderby f.AcronymEng
                           select new { f.Id, f.Name })
                           .ToList()
                           .Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name))
                           .ToList();
                    ComboServ.FillCombo(cbAggregateGroup, lst, false, true);

                    lst = (from f in context.StudyLevel
                           orderby f.Acronym
                           select new { f.Id, f.Name })
                           .ToList()
                           .Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name))
                           .ToList();
                    ComboServ.FillCombo(cbStudyLevel, lst, false, true);

                    lst = (from f in context.StudyBasis
                           orderby f.Acronym
                           select new { f.Id, f.Name })
                           .ToList()
                           .Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name))
                           .ToList();
                    ComboServ.FillCombo(cbStudyBasis, lst, false, true);

                    lst = (from f in context.StudyForm
                           orderby f.Acronym
                           select new { f.Id, f.Name })
                           .ToList()
                           .Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name))
                           .ToList();
                    ComboServ.FillCombo(cbStudyForm, lst, false, true);

                    lst = ComboServ.GetBoolFilter();
                    ComboServ.FillCombo(cbIsReduced, lst, false, true);
                    lst = ComboServ.GetBoolFilter();
                    ComboServ.FillCombo(cbIsParallel, lst, false, true);
                    lst = ComboServ.GetBoolFilter();
                    ComboServ.FillCombo(cbIsSecond, lst, false, true);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы ", exc);
            }

            InitHandlers();

            //btnAdd.Visible = btnRemove.Visible = false;
        }

        public override void  InitHandlers()
        {            
            cbFaculty.SelectedIndexChanged += new EventHandler(UpdateDataGrid);           
            cbStudyBasis.SelectedIndexChanged += new EventHandler(UpdateDataGrid);
            cbStudyForm.SelectedIndexChanged += new EventHandler(UpdateDataGrid);
            cbStudyLevel.SelectedIndexChanged += new EventHandler(UpdateDataGrid);
            cbIsSecond.SelectedIndexChanged += new EventHandler(UpdateDataGrid);
            cbIsReduced.SelectedIndexChanged += new EventHandler(UpdateDataGrid); 
            cbIsParallel.SelectedIndexChanged += new EventHandler(UpdateDataGrid);
            cbAggregateGroup.SelectedIndexChanged += new EventHandler(UpdateDataGrid);
        }

        void chbIsSecond_CheckedChanged(object sender, EventArgs e)
        {
            _orderBy = null;
            UpdateDataGrid();
        }

        private void UpdateDataGrid(object sender, EventArgs e)
        {
            _orderBy = null;
            UpdateDataGrid();
        }

        private void GetFilters(ref IQueryable<qEntry> query)
        {
            using (PriemEntities context = new PriemEntities())
            {
                // Факультеты
                if (cbFaculty.SelectedValue.ToString() != ComboServ.ALL_VALUE)
                {
                    int? h = ComboServ.GetComboIdInt(cbFaculty);
                    if (h != null) 
                        query = query.Where(c => c.FacultyId == h);
                }
               
                // основа обучения
                if (cbStudyBasis.SelectedValue.ToString() != ComboServ.ALL_VALUE)
                {
                    int? h = ComboServ.GetComboIdInt(cbStudyBasis);
                    if (h != null)
                        query = query.Where(c => c.StudyBasisId == h);
                }

                // форма обучения
                if (cbStudyForm.SelectedValue.ToString() != ComboServ.ALL_VALUE)
                {
                    int? h = ComboServ.GetComboIdInt(cbStudyForm);
                    if (h != null)
                        query = query.Where(c => c.StudyFormId == h);
                }                

                // подвид программы
                if (cbStudyLevel.SelectedValue.ToString() != ComboServ.ALL_VALUE)
                {
                    int? h = ComboServ.GetComboIdInt(cbStudyLevel);
                    if (h != null)
                        query = query.Where(c => c.StudyLevelId == h);
                }

                // укрупнённая группа
                if (cbAggregateGroup.SelectedValue.ToString() != ComboServ.ALL_VALUE)
                {
                    int? h = ComboServ.GetComboIdInt(cbAggregateGroup);
                    if (h != null)
                        query = query.Where(c => c.AggregateGroupId == h);
                }

                // ускоренная
                if (cbIsReduced.SelectedValue.ToString() != ComboServ.ALL_VALUE)
                {
                    bool h = QueryServ.ToBoolValue(ComboServ.GetComboIdInt(cbIsReduced));
                    query = query.Where(c => c.IsReduced == h);
                }

                // параллельная
                if (cbIsParallel.SelectedValue.ToString() != ComboServ.ALL_VALUE)
                {
                    bool h = QueryServ.ToBoolValue(ComboServ.GetComboIdInt(cbIsParallel));
                    query = query.Where(c => c.IsParallel == h);
                }

                // для лиц с во
                if (cbIsSecond.SelectedValue.ToString() != ComboServ.ALL_VALUE)
                {
                    bool h = QueryServ.ToBoolValue(ComboServ.GetComboIdInt(cbIsSecond));
                    query = query.Where(c => c.IsSecond == h);
                }

                //иностранцы    
                query = query.Where(c => c.IsForeign == chbIsForeign.Checked);

                //даты начала/окончания
                if (dtpDateStartFrom.Checked)
                {
                    query = query.Where(x => x.DateOfStart >= dtpDateStartFrom.Value);
                }
                if (dtpDateStartTo.Checked)
                {
                    query = query.Where(x => x.DateOfStart <= dtpDateStartTo.Value);
                }
                if (dtpDateFinishFrom.Checked)
                {
                    query = query.Where(x => x.DateOfClose >= dtpDateFinishFrom.Value);
                }
                if (dtpDateFinishTo.Checked)
                {
                    query = query.Where(x => x.DateOfClose <= dtpDateFinishTo.Value);
                }
            }
        }

        protected override void GetSource()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    IQueryable<qEntry> query;

                    if (string.IsNullOrEmpty(_orderBy))
                        query = context.qEntry.OrderBy(c => c.StudyLevelName).ThenBy(c => c.FacultyName).ThenBy(c => c.ObrazProgramName).ThenBy(c => c.ProfileName).ThenBy(c => c.StudyFormId).ThenBy(c => c.StudyBasisId);
                    else
                        query = context.qEntry;

                    GetFilters(ref query);
                    Dgv.DataSource = Converter.ConvertToDataTable(query.ToArray());

                    SetVisibleColumnsAndNameColumns();
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error(exc);
            }
        }

        protected override void SetVisibleColumnsAndNameColumns()
        {
            foreach (DataGridViewColumn col in Dgv.Columns)
            {
                col.Visible = false;
            }

            Dgv.AutoGenerateColumns = false;
            
            SetVisibleColumnsAndNameColumnsOrdered("StudyLevelName", "Уровень программы", 0);            
            SetVisibleColumnsAndNameColumnsOrdered("FacultyName", "Факультет", 1);
            SetVisibleColumnsAndNameColumnsOrdered("LicenseProgramCode", "Код направления", 2);
            SetVisibleColumnsAndNameColumnsOrdered("LicenseProgramName", "Направление", 3);
            SetVisibleColumnsAndNameColumnsOrdered("ObrazProgramName", "Образовательная программа", 4);
            SetVisibleColumnsAndNameColumnsOrdered("ObrazProgramCrypt", "Шифр программы", 5);
            SetVisibleColumnsAndNameColumnsOrdered("ProfileName", "Профиль", 6);            
            SetVisibleColumnsAndNameColumnsOrdered("StudyFormName", "Форма обучения", 7);
            SetVisibleColumnsAndNameColumnsOrdered("StudyBasisName", "Основа обучения", 8);
            SetVisibleColumnsAndNameColumnsOrdered("StudyPlanNumber", "Номер плана", 9);            
            SetVisibleColumnsAndNameColumnsOrdered("SecondType", "Реализуется как ВВ", 10);
            SetVisibleColumnsAndNameColumnsOrdered("KCP", "Контрольные цифры приема", 11);
            SetVisibleColumnsAndNameColumnsOrdered("KCPCel", "КЦП целевики", 12);
        }

        protected override void OpenCard(string itemId)
        {
            int? iSLId = ComboServ.GetComboIdInt(cbStudyLevel);
            if (string.IsNullOrEmpty(itemId) && iSLId.HasValue)
            {
                CardEntry crd = new CardEntry(iSLId.Value);
                crd.ToUpdateList += UpdateDataGrid;
                crd.Show();
            }
            else
            {
                CardEntry crd = new CardEntry(itemId);
                crd.ToUpdateList += UpdateDataGrid;
                crd.Show();
            }
        }

        protected override void Delete(string tableName, string id)
        {
            return;
        }

        private void tbPlanNumSearch_TextChanged(object sender, EventArgs e)
        {
            WinFormsServ.SearchInsideValue(this.Dgv, "ObrazProgramName", tbPlanNumSearch.Text);
        }

        private void btnLoadEntry_Click(object sender, EventArgs e)
        {
            new CardLoadEntry().Show();
        }

        private void btnToExcel_Click(object sender, EventArgs e)
        {
            EducServLib.PrintClass.PrintAllToExcel(Dgv);
        }

        protected override void DeleteSelectedRows(string sId)
        {
            if (!MainClass.IsPasha() || !MainClass.IsOwner())
                return;

            Guid gId = Guid.Empty;
            if (!Guid.TryParse(sId, out gId))
            {
                WinFormsServ.Error("Некорректный идентификатор");
                return;
            }

            using (TransactionScope tran = new TransactionScope())
            using (PriemEntities context = new PriemEntities())
            {
                var Ent = context.Entry.Where(x => x.Id == gId).FirstOrDefault();
                if (Ent == null)
                {
                    WinFormsServ.Error("Не найдена запись в базе");
                    return;
                }
                context.Entry.Remove(Ent);
                context.SaveChanges();

                string query = "DELETE FROM _Entry WHERE Id=@Id";
                SortedList<string, object> sl = new SortedList<string, object>();
                sl.AddVal("@Id", gId);
                MainClass.BdcOnlineReadWrite.ExecuteQuery(query, sl);

                tran.Complete();
            }

            UpdateDataGrid();
        }

        private void chbIsForeign_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }
        private void chbIsCrimea_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }
        private void dtpDateStartFrom_ValueChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }
        private void dtpDateStartTo_ValueChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }
        private void dtpDateFinishFrom_ValueChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }
        private void dtpDateFinishTo_ValueChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }
    }
}
