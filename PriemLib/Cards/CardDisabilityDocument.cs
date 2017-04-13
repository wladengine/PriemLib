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
using System.Data.Entity.Core.Objects;

namespace PriemLib
{
    public partial class CardDisabilityDocument : BookCard
    {
        public int? DisabilityTypeId
        {
            get { return ComboServ.GetComboIdInt(cbDisabilityType); }
            set { ComboServ.SetComboId(cbDisabilityType, value); }
        }
        public string Series
        {
            get { return tbSeries.Text.Trim(); }
            set { tbSeries.Text = value; }
        }
        public string Number
        {
            get { return tbNumber.Text.Trim(); }
            set { tbNumber.Text = value; }
        }
        public string Author
        {
            get { return tbAuthor.Text.Trim(); }
            set { tbAuthor.Text = value; }
        }
        public DateTime? Date
        {
            get
            {
                if (!dtpDate.Checked)
                    return null;
                else
                    return dtpDate.Value;
            }
            set
            {
                dtpDate.Checked = value.HasValue;
                if (value.HasValue)
                    dtpDate.Value = value.Value;
            }
        }
        public bool HasOriginals
        {
            get { return chbHasOriginals.Checked; }
            set { chbHasOriginals.Checked = value; }
        }

        private Guid PersonId;
        public int? BenefitDocumentId;

        public CardDisabilityDocument(string id, Guid _personId)
        {
            InitializeComponent();
            _Id = id;
            PersonId = _personId;

            InitControls();
            _tableName = "ed.PersonBenefitDocument";
        }

        protected override void ExtraInit()
        {
            base.ExtraInit();
            ComboServ.FillCombo(cbDisabilityType, HelpClass.GetComboListByTable("ed.DisabilityType"), false, false);
            using (PriemEntities context = new PriemEntities())
            {
                BenefitDocumentId = context.BenefitDocument.Where(x => x.BenefitDocumentTypeId == 1).Select(x => x.Id).First();
            }
        }

        protected override void SetAllFieldsNotEnabled()
        {
            foreach (Control crl in this.Controls)
                crl.Enabled = false;
        }

        protected override void FillCard()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var Person = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                lblFIO.Text = Person.Surname + " " + Person.Name + " " + Person.SecondName;

                if (_Id == null)
                    return;

                var value = context.PersonBenefitDocument.Where(x => x.Id == GuidId).FirstOrDefault();
                if (value == null)
                    WinFormsServ.Error("Не удалось загрузить из PersonBenefitDocument");

                DisabilityTypeId = value.DisabilityTypeId;
                Series = value.Series;
                Number = value.Number;
                Date = value.Date;
                Author = value.Author;
                HasOriginals = value.HasOriginals;
            }
        }

        protected override void InsertRec(PriemEntities context, ObjectParameter idParam)
        {
            context.PersonBenefitDocument_insert(PersonId, 1, BenefitDocumentId, Series, Number, Date, Author, HasOriginals, DisabilityTypeId, idParam);
            _Id = idParam.Value.ToString();
        }
        protected override void UpdateRec(PriemEntities context, Guid id)
        {
            context.PersonBenefitDocument_update(1, BenefitDocumentId, Series, Number, Date, Author, HasOriginals, DisabilityTypeId, GuidId);
        }
    }
}
