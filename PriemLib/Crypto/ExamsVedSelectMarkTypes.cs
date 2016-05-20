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
    public partial class ExamsVedSelectMarkTypes : Form
    {
        Guid _ExamsVedId;

        public ExamsVedSelectMarkTypes(Guid Id)
        {
            InitializeComponent();
            _ExamsVedId = Id;
            this.MdiParent = MainClass.mainform;
            FillCard();
        }

        public void FillCard()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var lst = (from x in context.ExamsVedMarkType
                           select new
                             {
                                 x.Id,
                                 x.Name
                             }).ToList();
                var e_lst = (from x in context.ExamsVedSelectedMarkType
                             where x.ExamsVedId == _ExamsVedId
                             select x.MarkTypeId).ToList();
                foreach (var l in lst)
                    chblst.Items.Add(new KeyValuePair<string, string>(l.Id.ToString(), l.Name), e_lst.Contains(l.Id));
                chblst.DisplayMember = "Value";
                chblst.ValueMember = "Id";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                int cnt = (from x in context.ExamsVedHistory
                           join m in context.ExamsVedHistoryMark on x.Id equals m.ExamsVedHistoryId
                           where (m.MarkValue > 0 || m.AppealMarkValue > 0)
                           && x.ExamsVedId == _ExamsVedId
                           select m).Count();
                if (cnt > 0)
                {
                    MessageBox.Show("В ведомости уже внесены оценки. Изменение параметров проверки невозможно", "Ошибка");
                }
                else
                {
                    List<int> NewSelectedMarksType = new List<int>();
                    foreach (var x in chblst.CheckedItems)
                    {
                        int newId = int.Parse(((KeyValuePair<string, string>)x).Key);
                        NewSelectedMarksType.Add(newId);
                    }
                    List<int> OldSelectedMarksType = (from x in context.ExamsVedSelectedMarkType
                                                      where x.ExamsVedId == _ExamsVedId
                                                      select x.MarkTypeId).ToList();
                    //старое удалить
                    List<int> typeid = OldSelectedMarksType.Where(x => !NewSelectedMarksType.Contains(x)).Select(x => x).ToList();
                    if (typeid.Count > 0)
                    {
                        var marks = (from x in context.ExamsVedHistory
                                            join m in context.ExamsVedHistoryMark on x.Id equals m.ExamsVedHistoryId
                                            where x.ExamsVedId == _ExamsVedId && typeid.Contains(m.ExamsVedMarkTypeId.Value)
                                            select m).ToList();
                        context.ExamsVedHistoryMark.RemoveRange(marks);
                        context.ExamsVedSelectedMarkType.RemoveRange(context.ExamsVedSelectedMarkType.Where(x => x.ExamsVedId == _ExamsVedId && typeid.Contains(x.MarkTypeId)).ToList());
                    }
                    // новое добавить
                    typeid = NewSelectedMarksType.Where(x => !OldSelectedMarksType.Contains(x)).Select(x => x).ToList();
                    foreach (int type in typeid)
                    {
                        context.ExamsVedSelectedMarkType.Add(new ExamsVedSelectedMarkType() { 
                         ExamsVedId = _ExamsVedId, MarkTypeId = type});
                        List<Guid> marks = (from x in context.ExamsVedHistory
                                            where x.ExamsVedId == _ExamsVedId 
                                            select x.Id).ToList();
                        foreach (Guid id in marks)
                        context.ExamsVedHistoryMark.Add(new ExamsVedHistoryMark()
                            {
                                 Id = Guid.NewGuid(),
                                 ExamsVedHistoryId = id,
                                 ExamsVedMarkTypeId = type,
                            });
                    }
                    context.SaveChanges();
                }
            }
            this.Close();
        }
    }
}
