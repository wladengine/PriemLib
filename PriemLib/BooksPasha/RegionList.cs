using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EducServLib;
using System.Transactions;

namespace PriemLib.BooksPasha
{
    public partial class RegionList : BookList
    {
        public RegionList()
        {
            InitializeComponent();
        }

        protected override void GetSource()
        {
            using (PriemEntities context = new PriemEntities())
            {
                Dgv.DataSource = 
                    Converter.ConvertToDataTable(context.Region.Select(x => new { x.Id, x.Name, x.RegionNumber }).ToArray());
            }
        }

        protected override void SetVisibleColumnsAndNameColumns()
        {
            base.SetVisibleColumnsAndNameColumns("RegionNumber", "Номер");
        }

        protected override void OpenCard(string itemId)
        {
        }

        protected override void DeleteSelectedRows(string sId)
        {
            if (!MainClass.IsPasha() || !MainClass.IsOwner())
                return;

            int iId = 0;
            if (!int.TryParse(sId, out iId))
            {
                WinFormsServ.Error("Некорректный идентификатор");
                return;
            }

            using (TransactionScope tran = new TransactionScope())
            using (PriemEntities context = new PriemEntities())
            {
                var Ent = context.Region.Where(x => x.Id == iId).FirstOrDefault();
                if (Ent == null)
                {
                    WinFormsServ.Error("Не найдена запись в базе");
                    return;
                }
                context.Region.DeleteObject(Ent);
                context.SaveChanges();

                string query = "DELETE FROM Region WHERE Id=@Id AND Id>112";//112 start value
                SortedList<string, object> sl = new SortedList<string, object>();
                sl.AddVal("@Id", iId);
                MainClass.BdcOnlineReadWrite.ExecuteQuery(query, sl);

                tran.Complete();
            }

            UpdateDataGrid();
        }
    }
}
