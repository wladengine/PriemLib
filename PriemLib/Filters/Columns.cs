using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using EducServLib;
using BDClassLib;

namespace PriemLib
{
    public partial class Columns : Form
    {
        private FormFilter own;
        private DBPriem _bdc;
        private string _fac;
        private SortedList<string,string> _columnList;

        //конструктор
        public Columns(FormFilter owner, string facId)
        {
            InitializeComponent();
            this.CenterToParent();
            own = owner;
            _fac = facId;
            _bdc = MainClass.Bdc;
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

            _columnList = p.GetColumns(_bdc, _fac);

            foreach (DataGridViewColumn col in own.Dgv.Columns)
                if (col.Visible)
                    try
                    {
                        lbYes.Items.Add(_columnList[col.Name]);
                    }
                    catch { }


            foreach(string li in _columnList.Values)
            {
                try
                {
                    if (!lbYes.Items.Contains(li))
                        lbNo.Items.Add(li);
                }
                catch
                {
                }
            }
        }

        //кнопка ОК
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (own is ListAbit)
            {                
                MainClass._config.ClearColumnListAbit();

                foreach (string li in lbYes.Items)
                {
                    MainClass._config.AddColumnNameAbit(_columnList.Keys[_columnList.IndexOfValue(li)]);
                }
            }
            else if (own is ListPersonFilter)
            {
                MainClass._config.ClearColumnListPerson();

                foreach (string li in lbYes.Items)
                {
                    MainClass._config.AddColumnNamePerson(_columnList.Keys[_columnList.IndexOfValue(li)]);
                }
            }

            own.UpdateDataGrid();
            this.Close();
        }

        //функции переноса строк
        private void btnLeft_Click(object sender, EventArgs e)
        {
            WinFormsServ.MoveRows(lbNo, lbYes, false);
        }

        //
        private void btnRight_Click(object sender, EventArgs e)
        {
            WinFormsServ.MoveRows(lbYes, lbNo, false);
        }

        //
        private void btnLeftAll_Click(object sender, EventArgs e)
        {
            WinFormsServ.MoveRows(lbNo, lbYes, true);
        }

        //
        private void btnRightAll_Click(object sender, EventArgs e)
        {
            WinFormsServ.MoveRows(lbYes, lbNo, true);
        }

        //стрелка вверх
        private void btnUp_Click(object sender, EventArgs e)
        {
            if (lbYes.Items.Count == 0)
                return;

            int i = lbYes.SelectedIndex;
            if (i == 0)
                return;

            object obj = lbYes.Items[i];
            lbYes.Items.RemoveAt(i);
            lbYes.Items.Insert(i - 1, obj);
            lbYes.SetSelected(i - 1, true);
        }

        //стрелка вниз
        private void btnDown_Click(object sender, EventArgs e)
        {
            if (lbYes.Items.Count == 0)
                return;

            int i = lbYes.SelectedIndex;
            if (i == lbYes.Items.Count - 1)
                return;

            object obj = lbYes.Items[i];
            lbYes.Items.RemoveAt(i);
            lbYes.Items.Insert(i + 1, obj);
            lbYes.SetSelected(i + 1, true);
        }
    }
}