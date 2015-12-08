using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PriemLib
{
    public partial class CardAppManualExams : Form
    {
        ShortCompetition Comp;

        public CardAppManualExams(ShortCompetition _c)
        {
            InitializeComponent();
            Comp = _c;

            this.MdiParent = MainClass.mainform;

            FillGrid();
        }
        public void FillGrid()
        {
            List<ExamenBlock> lst = Comp.lstExamInEntryBlock;

            DataTable tbl = new DataTable();
            DataColumn clm = new DataColumn();
            clm.ColumnName = "BlockId";
            tbl.Columns.Add(clm);

            clm = new DataColumn();
            clm.ColumnName = "BlockName";
            tbl.Columns.Add(clm);

            clm = new DataColumn();
            clm.ColumnName = "UnitName";
            tbl.Columns.Add(clm);

            foreach (ExamenBlock bl in lst)
            {
                DataRow rw = tbl.NewRow();
                rw.SetField("BlockId", bl.BlockId);
                rw.SetField("BlockName", bl.BlockName);
                rw.SetField("UnitName", bl.SelectedUnitName);
                tbl.Rows.Add(rw);
            }
            dgv.DataSource = tbl;
            dgv.Columns["BlockId"].Visible = false;
            dgv.Columns["BlockName"].HeaderText = "Название блока";
            dgv.Columns["UnitName"].HeaderText = "Выбранный экзамен";
        }
    }
}
