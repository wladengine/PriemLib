using EducServLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PriemLib.Cards
{
    public partial class CardChangeOriginalsPlace : Form
    {
        private Guid AbiturientId;
        private event UpdateListHandler OnUpdated;
        public CardChangeOriginalsPlace(Guid _AbiturientId)
        {
            InitializeComponent();
            AbiturientId = _AbiturientId;

            FillCombo();
        }

        private void FillCombo()
        {
            using (PriemEntities context = new PriemEntities())
            {
                List<KeyValuePair<string, string>> lst =
                    (from Ab in context.Abiturient
                     join OthAb in context.Abiturient on Ab.PersonId equals OthAb.PersonId
                     join ent in context.qEntry on OthAb.EntryId equals ent.Id
                     where Ab.Id == AbiturientId && OthAb.Id != AbiturientId
                     && !OthAb.HasOriginals && !OthAb.BackDoc && !OthAb.NotEnabled
                     select new
                     {
                         OthAb.Id,
                         OthAb.Priority,
                         ent.FacultyAcr,
                         ent.LicenseProgramName,
                         ent.ObrazProgramName
                     }).ToList()
                     .OrderBy(x => x.Priority)
                     .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), "Приор: " + x.Priority + "; [" + x.FacultyAcr + "] " + x.LicenseProgramName + "/" + x.ObrazProgramName))
                     .ToList();

                ComboServ.FillCombo(cbAbiturient, lst, true, false);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Guid? DestAbiturientId = ComboServ.GetComboIdGuid(cbAbiturient);

            if (DestAbiturientId.HasValue)
                ApplicationDataProvider.ChangeHasOriginalsDestination(AbiturientId, DestAbiturientId.Value);
            else
                WinFormsServ.Error("Не указан конкурс!");
        }
    }
}
