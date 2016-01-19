using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EducServLib;

namespace PriemLib
{
    public partial class Filters : Form
    {
        private FilterType _currentType;
        private FilterItem _currentItem;
        private int _current;

        private DBPriem _bdc;
        private FormFilter _owner;

        private bool flag;

        //конструтор
        public Filters(FormFilter la, DBPriem bdc, List<iFilter> list)
        {
            InitializeComponent();

            _bdc = bdc;
            _owner = la;

            AbstractFilterProvider p;
            switch (MainClass.dbType)
            {
                case PriemType.Priem: { p = new FilterProvider_1K(); break; }
                case PriemType.PriemMag: { p = new FilterProvider_Mag(); break; }
                case PriemType.PriemSPO: { p = new FilterProvider_SPO(); break; }
                case PriemType.PriemAspirant: { p = new FilterProvider_Asp(); break; }
                case PriemType.PriemAG: { p = new FilterProvider_AG(); break; }
                default: { p = new FilterProvider_1K(); break; }
            }

            foreach (FilterItem fi in p.GetFilterList(_bdc))
                AddItem(fi);

            if (list != null)
            {
                foreach (iFilter obj in list)
                {
                    flag = true;
                    AddInList(obj);
                }
                lbFilters.SelectedIndex = 0;
            }

            cmbFilters.SelectedIndex = 0;
        }

        private void AddItem(FilterItem fi)
        {
            cmbFilters.Items.Add(fi);
        }

        private void ShowBool()
        {
            fBool.Visible = true;
            fFromTo.Visible = false;
            fMult.Visible = false;
            fDateFromTo.Visible = false;
            fText.Visible = false;

            fBool.Clear();
        }

        private void ShowText()
        {
            fBool.Visible = false;
            fFromTo.Visible = false;
            fMult.Visible = false;
            fDateFromTo.Visible = false;
            fText.Visible = true;

            fText.Text = string.Empty;
        }

        private void ShowFromTo()
        {
            fBool.Visible = false;
            fFromTo.Visible = true;
            fMult.Visible = false;
            fDateFromTo.Visible = false;
            fText.Visible = false;

            fFromTo.Clear();
            fFromTo.Location = new Point(43, 118);
        }

        private void ShowMult()
        {
            fBool.Visible = false;
            fFromTo.Visible = false;
            fMult.Visible = true;
            fDateFromTo.Visible = false;
            fText.Visible = false;

            fMult.ClearLists();
        }

        private void ShowMultFromTo()
        {
            fBool.Visible = false;
            fFromTo.Visible = true;
            fMult.Visible = true;
            fDateFromTo.Visible = false;
            fText.Visible = false;

            fMult.ClearLists();
            fFromTo.Clear();
            fFromTo.Location = new Point(43, 242);
        }

        private void ShowDateFromTo()
        {
            fBool.Visible = false;
            fFromTo.Visible = false;
            fMult.Visible = false;
            fDateFromTo.Visible = true;
            fText.Visible = false;

            fDateFromTo.Clear();
        }

        //реакция на смену в комбике
        private void cmbFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterItem fi = cmbFilters.SelectedItem as FilterItem;
            FilterType ft = fi.Type;

            _current = cmbFilters.SelectedIndex;
            _currentItem = fi;
            _currentType = ft;

            switch (ft)
            {
                case FilterType.Bool:
                    ShowBool();
                    break;
                case FilterType.FromTo:
                    ShowFromTo();
                    break;
                case FilterType.Multi:
                    ShowMult();
                    FillMulti();
                    break;
                case FilterType.DateFromTo:
                    ShowDateFromTo();
                    break;
                case FilterType.Text:
                    ShowText();
                    break;
                case FilterType.MultiFromTo:
                    ShowMultFromTo();
                    FillMulti();
                    break;
            }
        }

        //заполняет мультифильтр-юзерконтрол
        private void FillMulti()
        {
            try
            {
                DataSet ds = _bdc.GetDataSet(_currentItem.Query);

                //если ничего нет - выходим
                if (ds.Tables[0].Rows.Count == 0)
                    return;

                List<ListItem> list = new List<ListItem>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                    list.Add(new ListItem(dr["Id"].ToString(), dr["Name"].ToString()));

                fMult.ClearLists();
                fMult.FillList(list, true);
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка при заполнении фильтра:", ex);
            }
        }

        //стрелка вверх
        private void btnUp_Click(object sender, EventArgs e)
        {
            flag = true;
            if (lbFilters.Items.Count == 0)
                return;

            int i = lbFilters.SelectedIndex;
            if (i == 0)
                return;

            object obj = lbFilters.Items[i];
            lbFilters.Items.RemoveAt(i);
            lbFilters.Items.Insert(i - 1, obj);
            lbFilters.SetSelected(i - 1, true);
            flag = false;
        }

        //стрелка вниз
        private void btnDown_Click(object sender, EventArgs e)
        {
            flag = true;
            if (lbFilters.Items.Count == 0)
                return;

            int i = lbFilters.SelectedIndex;
            if (i == lbFilters.Items.Count - 1)
                return;

            object obj = lbFilters.Items[i];
            lbFilters.Items.RemoveAt(i);
            lbFilters.Items.Insert(i + 1, obj);
            lbFilters.SetSelected(i + 1, true);
        }

        //закрытие с передачей результата
        private void btnExit_Click(object sender, EventArgs e)
        {
            List<iFilter> list = new List<iFilter>();
            int cnt = CheckFilters(list);

            if (cnt < 0)
                return;
            else if (cnt == 0)
                _owner.FilterList = null;
            else
            {
                _owner.FilterList = list;
            }
            _owner.UpdateDataGrid();

            this.Close();
        }

        //проверка и подсчет
        private int CheckFilters(List<iFilter> list)
        {
            int filtCount = 0, leftBrackCount = 0, rightBrackCount = 0;

            foreach (iFilter obj in lbFilters.Items)
            {
                list.Add(obj);

                if (obj is Filter)
                    filtCount++;
                else if (obj is LeftBracket)
                    leftBrackCount++;
                else if (obj is RightBracket)
                    rightBrackCount++;
            }

            if (leftBrackCount != rightBrackCount)
            {
                WinFormsServ.Error("Неправильная расстановка скобок");
                return -1;
            }
            return filtCount;
        }

        //удаление
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lbFilters.Items.Count != 0)
            {
                flag = true;
                lbFilters.Items.RemoveAt(lbFilters.SelectedIndex);

                if (lbFilters.Items.Count == 0)
                    btnChange.Enabled = false;
                else
                    lbFilters.SelectedIndex = 0;

                cmbFilters.SelectedIndex = 0;
            }
        }

        //сохранение нового
        private void btnAdd_Click(object sender, EventArgs e)
        {
            Filter f = Save();
            if (f != null)
            {
                AddInList(f);
                btnChange.Enabled = true;
            }
        }

        //сохранение
        private Filter Save()
        {
            switch (_currentType)
            {
                case FilterType.Bool:
                    return AddBool();
                case FilterType.FromTo:
                    return AddFromTo();
                case FilterType.DateFromTo:
                    return AddDateFromTo();
                case FilterType.Multi:
                    return AddMult();
                case FilterType.Text:
                    return AddText();
                case FilterType.MultiFromTo:
                    return AddMultFromTo();
            }
            return null;
        }


        //добавление в список созданных фильтров
        private void AddInList(object filt)
        {
            int i = lbFilters.SelectedIndex;

            lbFilters.Items.Insert(i + 1, filt);
            lbFilters.SelectedItem = filt;
        }

        //добавление нового
        private BoolFilter AddBool()
        {
            bool val = fBool.Value;

            BoolFilter bf = new BoolFilter(_currentItem, val, _current);

            return bf;
        }

        private TextFilter AddText()
        {
            string s = fText.Text.Trim();
            if (s.Length == 0)
            {
                epError.SetError(fText, "Пустое значение");
                return null;
            }
            else
                epError.Clear();

            TextFilter tf = new TextFilter(_currentItem, s, _current);
            return tf;
        }

        //добавление нового
        private FromToFilter AddFromTo()
        {
            if (!fFromTo.CheckValues())
                return null;

            FromToFilter ftf = new FromToFilter(_currentItem, fFromTo.FromValue, fFromTo.ToValue, _current);

            return ftf;
        }

        //добавление нового
        private DateFromToFilter AddDateFromTo()
        {
            if (!fDateFromTo.CheckValues())
                return null;

            DateFromToFilter dftf = new DateFromToFilter(_currentItem, fDateFromTo.FromValue, fDateFromTo.ToValue, _current);

            return dftf;
        }

        private MultiFromToFilter AddMultFromTo()
        {
            if (_currentItem.Name == "Экзамены")
                return AddExams();

            return null;
        }

        //добавление нового
        private MultiSelectFilter AddMult()
        {
            if (fMult.YesCount == 0)
            {
                WinFormsServ.Error("Не выбраны значения!");
                return null;
            }

            MultiSelectFilter msf = new MultiSelectFilter(_currentItem, fMult.GetSelectedItems(), _current);

            return msf;
        }

        private MultiFromToFilter AddExams()
        {
            if (fMult.YesCount == 0)
            {
                WinFormsServ.Error("Не выбраны значения!");
                return null;
            }
            if (!fFromTo.CheckValues())
                return null;

            FromToFilter ftf = new FromToFilter(new FilterItem("Оценка", FilterType.FromTo, "temp.markvalue", null), fFromTo.FromValue, fFromTo.ToValue, _current);
            MultiSelectFilter msf = new MultiSelectFilter(new FilterItem("Экзамены", FilterType.Multi, "qMark.ExamInprogramId", null), fMult.GetSelectedItems(), _current);

            return new MultiFromToFilter(_currentItem, msf, ftf, _current);
        }

        //изменение текущего
        private void btnChange_Click(object sender, EventArgs e)
        {
            if (lbFilters.SelectedIndex < 0)
            {
                return;
            }

            flag = true;

            Filter filter = Save();
            if (filter != null)
                lbFilters.Items[lbFilters.SelectedIndex] = filter;
        }

        //реакция на тычок по созданному фильтру
        private void lbFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbFilters.SelectedIndex < 0)
            {
                btnChange.Enabled = false;
                return;
            }
            else
                btnChange.Enabled = true;

            //если жмут по стрелкам, ничего не происходит
            if (flag)
            {
                flag = false;
                return;
            }

            object obj = lbFilters.SelectedItem;

            if (obj is Bracket)
                btnUp.Enabled = btnDown.Enabled = false;
            else
                btnUp.Enabled = btnDown.Enabled = true;

            if (!(obj is Filter))
                return;

            Filter filter = obj as Filter;

            //проставили в комбик нужный фильтр
            cmbFilters.SelectedIndex = filter.NumInList;

            if (filter is BoolFilter)
            {
                BoolFilter bf = filter as BoolFilter;
                fBool.Value = bf.Value;
            }
            else if (filter is DateFromToFilter)
            {
                DateFromToFilter dftf = filter as DateFromToFilter;

                fDateFromTo.FromValue = dftf.FromValue;
                fDateFromTo.ToValue = dftf.ToValue;
            }
            else if (filter is FromToFilter)
            {
                FromToFilter ftf = filter as FromToFilter;

                fFromTo.FromValue = ftf.FromValue;
                fFromTo.ToValue = ftf.ToValue;
            }
            else if (filter is MultiSelectFilter)
            {
                MultiSelectFilter msf = filter as MultiSelectFilter;

                FillMulti();
                fMult.FillList(msf.List, false);

                foreach (ListItem li in msf.List)
                    fMult.RemoveAtRight(li.Name);
            }
            else if (filter is TextFilter)
            {
                TextFilter tf = filter as TextFilter;

                fText.Text = tf.Value;
            }
            else if (filter is MultiFromToFilter)
            {
                MultiSelectFilter msf = (filter as MultiFromToFilter).MultiSelectFilter;

                FillMulti();
                fMult.FillList(msf.List, false);

                foreach (ListItem li in msf.List)
                    fMult.RemoveAtRight(li.Name);

                FromToFilter ftf = (filter as MultiFromToFilter).FromtoFilter;

                fFromTo.FromValue = ftf.FromValue;
                fFromTo.ToValue = ftf.ToValue;
            }
        }

        //вставка или
        private void btnOr_Click(object sender, EventArgs e)
        {
            AddInList(new Or());
        }

        //вставка скобок
        private void btnBrackets_Click(object sender, EventArgs e)
        {
            AddInList(new LeftBracket());
            AddInList(new RightBracket());
        }

        //очистка
        private void btnClear_Click(object sender, EventArgs e)
        {
            lbFilters.Items.Clear();
        }
    }
}