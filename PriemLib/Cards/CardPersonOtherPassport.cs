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

namespace PriemLib
{
    public partial class CardPersonOtherPassport : BookCardInt
    {
        private int? PassportTypeId
        {
            get { return ComboServ.GetComboIdInt(cbPassportType); }
            set { ComboServ.SetComboId(cbPassportType, value); }
        }

        private string PassportSeries
        {
            get { return tbSeries.Text.Trim(); }
            set { tbSeries.Text = value; }
        }
        private string PassportNumber
        {
            get { return tbNumber.Text.Trim(); }
            set { tbNumber.Text = value; }
        }
        private DateTime PassportDate
        {
            get { return dtpDate.Value.Date; }
            set { dtpDate.Value = value; }
        }

        private string Surname
        {
            get { return tbSurname.Text.Trim(); }
            set { tbSurname.Text = value; }
        }
        private string PersonName
        {
            get { return tbName.Text.Trim(); }
            set { tbName.Text = value; }
        }
        private string SecondName
        {
            get { return tbSecondName.Text.Trim(); }
            set { tbSecondName.Text = value; }
        }
        private Guid PersonId;

        public CardPersonOtherPassport(string id) : base(id)
        {
            InitializeComponent();

            InitControls();
        }
        public CardPersonOtherPassport(Guid _personId)
        {
            InitializeComponent();

            InitControls();
            PersonId = _personId;
        }

        protected override void ExtraInit()
        {
            base.ExtraInit();

            _tableName = "ed.PersonOtherPassport";
            _title = "Ранее выданный паспорт";
            

            using (PriemEntities context = new PriemEntities())
            {
                var src = context.PassportType
                    .Select(x => new { x.Id, x.Name })
                    .ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name))
                    .ToList();

                ComboServ.FillCombo(cbPassportType, src, false, false);
            }
        }
        protected override void FillCard()
        {
            if (string.IsNullOrEmpty(_Id))
                return;

            int IntId = 0;
            if (!int.TryParse(_Id, out IntId))
                return;

            using (PriemEntities context = new PriemEntities())
            {
                var ent = context.PersonOtherPassport.Where(x => x.Id == IntId).FirstOrDefault();
                if (ent == null)
                {
                    WinFormsServ.Error("Не удалось получить данные ранее выданного паспорта!");
                    return;
                }

                PassportTypeId = ent.PassportTypeId;
                PassportSeries = ent.PassportSeries;
                PassportNumber = ent.PassportNumber;
                PassportDate = ent.PassportDate ?? DateTime.Now;

                Surname = ent.Surname;
                PersonName = ent.Name;
                SecondName = ent.SecondName;
            }
        }

        protected override void InsertRec(PriemEntities context, System.Data.Entity.Core.Objects.ObjectParameter idParam)
        {
            context.PersonOtherPassport_insert(PersonId, PassportTypeId, PassportSeries, PassportNumber, Surname, PersonName, SecondName, PassportDate, idParam);
        }
        protected override void UpdateRec(PriemEntities context, int id)
        {
            context.PersonOtherPassport_update(PersonId, PassportTypeId, PassportSeries, PassportNumber, Surname, PersonName, SecondName, PassportDate, id);
        }
    }
}
