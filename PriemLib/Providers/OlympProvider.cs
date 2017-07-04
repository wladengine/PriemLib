using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriemLib
{
    public static class OlympProvider
    {
        public static Guid GetOlympBookId(int OlympYear, int OlympTypeId, int OlympNameId, int OlympProfileId, int OlympSubjectId)
        {
            using (PriemEntities context = new PriemEntities())
            {
                var olympList = context.OlympBook.Where(x => x.OlympYear == OlympYear
                    && x.OlympTypeId == OlympTypeId
                    && x.OlympNameId == OlympNameId
                    && x.OlympProfileId == OlympProfileId
                    && x.OlympSubjectId == OlympSubjectId).Select(x => x.Id).ToList();

                if (olympList.Count > 1)
                    throw new Exception("Не найдено однозначного соответствия олимпиады из справочника!");
                if (olympList.Count == 0)
                    throw new Exception("Не найдено олимпиады из справочника!");

                return olympList[0];
            }
        }
    }
}
