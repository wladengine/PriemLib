using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using EducServLib;

namespace PriemLib
{
    public partial class CardSelectEgeExamName : Form
    {
        public event Action<int> EgeExamNameSelected;
        public CardSelectEgeExamName()
        {
            InitializeComponent();
            FillCombo();
        }

        private void FillCombo()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = context.EgeExamName.Select(x => new { x.Id, x.Name })
                    .ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name))
                    .ToList();

                ComboServ.FillCombo(cbEgeExamName, src, false, false);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int? EgeExamNameId = ComboServ.GetComboIdInt(cbEgeExamName);
            if (EgeExamNameId.HasValue && EgeExamNameSelected != null)
                EgeExamNameSelected(EgeExamNameId.Value);

            this.Close();
        }
    }
}
