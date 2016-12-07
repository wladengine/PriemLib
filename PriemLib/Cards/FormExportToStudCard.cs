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
    public partial class FormExportToStudCard : Form
    {
        public FormExportToStudCard()
        {
            InitializeComponent();
        }

        private void btnStartExport_Click(object sender, EventArgs e)
        {
            ExportClass.ExportInNewStudent(dtpMinOrderDate.Checked ? (DateTime?)dtpMinOrderDate.Value.Date : null);
        }
    }
}
