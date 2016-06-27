using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PriemLib
{
    public static class EnabledAndDisenabledApplicationToOnlineUpdater
    {
        public static List<int> GetEnabledApplicationBarcodeListInWorkDB()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = (from App in context.Abiturient
                        join enabl in context.extEnableProtocol on App.Id equals enabl.AbiturientId
                        where App.Barcode != null
                        select App.Barcode.Value);
                return src.ToList();

            }
        }

        public static List<int> GetEnabledApplicationBarcodeListInOnlineDB()
        {
            string query = "SELECT Barcode FROM [ApplicationAddedToProtocol]";
            DataTable tbl = MainClass.BdcOnlineReadWrite.GetDataSet(query).Tables[0];

            return tbl.Rows.Cast<DataRow>()
                .Select(x => x.Field<int>(0)).ToList();
        }

        public static void SyncronizeBases()
        {
            List<int> lstInWorkDB = GetEnabledApplicationBarcodeListInWorkDB();
            List<int> lstInOnlineDB = GetEnabledApplicationBarcodeListInOnlineDB();

            List<int> lstToAdd = lstInWorkDB.Except(lstInOnlineDB).ToList();

            foreach (int brc in lstToAdd)
            {
                string query = "INSERT INTO [ApplicationAddedToProtocol] (Barcode) VALUES (@Barcode)";
                MainClass.BdcOnlineReadWrite.ExecuteQuery(query, new SortedList<string, object>() { { "@Barcode", brc } });
            }

            List<int> lstToDelete = lstInOnlineDB.Except(lstInWorkDB).ToList();

            MessageBox.Show("Добавлено пропущенных записей: " + lstToAdd.Count + "\nУдалённых записей: " + lstToDelete.Count);
        }
    }
}
