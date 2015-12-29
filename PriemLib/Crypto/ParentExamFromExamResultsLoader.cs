using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BaseFormsLib;
using EducServLib;

namespace PriemLib
{
    public partial class ParentExamFromExamResultsLoader : BaseFormEx
    {
        public ParentExamFromExamResultsLoader()
        {
            InitializeComponent();

            InitControls();
        }

        private void InitControls()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lst = (from extEx in context.extExamInEntry
                           join ex in context.ExamInEntryBlock on extEx.Id equals ex.ParentExamInEntryBlockId
                           select new
                           {
                               extEx.Id,
                               extEx.ExamName
                           }).Distinct().ToList()
                          .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.ExamName))
                          .ToList();
                ComboServ.FillCombo(cbExamInEntry, lst, false, false);
            }
        }
    }
}
