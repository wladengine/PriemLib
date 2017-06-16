using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PriemLib
{
    public static class EntryProvider
    {
        public static Guid InnerEntryInEntry_Insert(Guid EntryId, int ObrazProgramId, int ProfileId, int KCP, int? EgeExamNameId)
        {
            try
            {
                Guid Id = Guid.NewGuid();

                using (TransactionScope tran = new TransactionScope())
                using (PriemEntities context = new PriemEntities())
                {
                    string query = "INSERT INTO InnerEntryInEntry (Id, ObrazProgramId, ProfileId, EntryId) VALUES (@Id, @ObrazProgramId, @ProfileId, @EntryId)";

                    context.InnerEntryInEntry.Add(new InnerEntryInEntry() { Id = Id, ObrazProgramId = ObrazProgramId, ProfileId = ProfileId, KCP = KCP, EntryId = EntryId });
                    context.SaveChanges();

                    SortedList<string, object> slParams = new SortedList<string, object>();
                    slParams.Add("@Id", Id);
                    slParams.Add("@ObrazProgramId", ObrazProgramId);
                    slParams.Add("@EntryId", EntryId);
                    slParams.Add("@ProfileId", ProfileId);
                    MainClass.BdcOnlineReadWrite.ExecuteQuery(query, slParams);

                    tran.Complete();

                    return Id;
                }
            }
            catch
            {
                throw;
            }
        }

        public static void InnerEntryInEntry_Update(Guid Id, Guid EntryId, int ObrazProgramId, int ProfileId, int KCP, int? EgeExamNameId)
        {
            using (TransactionScope tran = new TransactionScope())
            using (PriemEntities context = new PriemEntities())
            {
                string query = "SELECT COUNT(*) FROM InnerEntryInEntry WHERE Id=@Id";
                int iCnt = (int)MainClass.BdcOnlineReadWrite.GetValue(query, new SortedList<string, object>() { { "@Id", Id } });
                if (iCnt == 0)
                    query = "INSERT INTO InnerEntryInEntry (Id, ObrazProgramId, ProfileId, EntryId) VALUES (@Id, @ObrazProgramId, @ProfileId, @EntryId)";
                else
                    query = "UPDATE InnerEntryInEntry SET ObrazProgramId=@ObrazProgramId, ProfileId=@ProfileId, EntryId=@EntryId WHERE Id=@Id";

                var Ent = context.InnerEntryInEntry.Where(x => x.Id == Id).FirstOrDefault();
                if (Ent == null)
                    throw new Exception("Не найдена запись в таблице InnerEntryInEntry!");

                Ent.ObrazProgramId = ObrazProgramId;
                Ent.ProfileId = ProfileId;
                Ent.KCP = KCP;
                Ent.EgeExamNameId = EgeExamNameId;

                context.SaveChanges();

                SortedList<string, object> slParams = new SortedList<string, object>();
                slParams.Add("@Id", Id);
                slParams.Add("@ObrazProgramId", ObrazProgramId);
                slParams.Add("@EntryId", EntryId);
                slParams.Add("@ProfileId", ProfileId);
                MainClass.BdcOnlineReadWrite.ExecuteQuery(query, slParams);

                tran.Complete();
            }
        }
    }
}
