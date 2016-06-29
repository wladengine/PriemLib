using EducServLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

namespace PriemLib
{
    public class OlympVserossImporter
    {
        class EntOlympVserossToImport
        {
            public string LicenseProgramCode { get; set; }
            public string ObrazProgramName { get; set; }
            public string OlympProfileName { get; set; }
        }
        public static void ImportDataFromXML(string fileName)
        {
            List<EntOlympVserossToImport> lstToImport = new List<EntOlympVserossToImport>();
            DataTable tbl = PrintClass.GetDataTableFromExcel2007_New(fileName);

            foreach (DataRow rw in tbl.Rows)
                lstToImport.Add(new EntOlympVserossToImport() { LicenseProgramCode = rw.Field<string>(2), ObrazProgramName = rw.Field<string>(1), OlympProfileName = rw.Field<string>(0) });

            using (PriemEntities context = new PriemEntities())
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    foreach (var ol in lstToImport)
                    {
                        int iOlympProfileId = context.OlympProfile.Where(x => x.Name == ol.OlympProfileName).Select(x => x.Id).DefaultIfEmpty(0).First();
                        if (iOlympProfileId == 0)
                            WinFormsServ.Error("Не найдено соответствие профилю с ProfileID=" + ol.OlympProfileName);

                        int iLicenseProgramId = context.SP_LicenseProgram.Where(x => x.Code == ol.LicenseProgramCode).Select(x => x.Id).DefaultIfEmpty(0).First();
                        if (iLicenseProgramId == 0)
                            WinFormsServ.Error("Не найдено соответствие направлению с LicenseProgramCode=" + ol.LicenseProgramCode);

                        int iObrazProgramId = context.SP_ObrazProgram.Where(x => x.Name == ol.ObrazProgramName).Select(x => x.Id).DefaultIfEmpty(0).First();
                        if (iObrazProgramId == 0)
                            WinFormsServ.Error("Не найдено соответствие программе " + ol.ObrazProgramName);

                        var lstEntry = context.Entry
                            .Where(x => x.LicenseProgramId == iLicenseProgramId && x.ObrazProgramId == iObrazProgramId && !x.IsForeign && !x.IsCrimea)
                            .Select(x => x.Id)
                            .ToList();

                        var lstBenefits = context.OlympResultToCommonBenefit.Where(x => x.OlympTypeId == 1 || x.OlympTypeId == 2)
                            .Select(x => new { x.EntryId, x.OlympTypeId, x.OlympProfileId, x.OlympValueId }).ToList();


                        foreach (Guid EntryId in lstEntry)
                        {
                            for (int iOlympTypeId = 1; iOlympTypeId < 3; iOlympTypeId++)
                            {
                                foreach (int iOlympValueId in new List<int>() { 5, 6, 7 })
                                {
                                    int cnt = lstBenefits
                                        .Where(x => x.EntryId == EntryId && x.OlympTypeId == iOlympTypeId && x.OlympProfileId == iOlympProfileId && x.OlympValueId == iOlympValueId)
                                        .Count();
                                    if (cnt == 0)
                                    {
                                        ObjectParameter idParam = new ObjectParameter("id", typeof(Guid));
                                        context.OlympResultToCommonBenefit_Insert(EntryId, iOlympTypeId, 0, iOlympValueId, null, iOlympProfileId, null, null, idParam);
                                    }
                                }
                            }
                        }


                    }
                    context.SaveChanges();

                    tran.Complete();
                    MessageBox.Show("OK");
                }
            }
        }
    }
}
