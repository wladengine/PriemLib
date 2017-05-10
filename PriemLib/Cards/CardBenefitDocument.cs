using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EducServLib;
using BaseFormsLib;
using System.Data.Entity.Core.Objects;

namespace PriemLib
{
    public partial class CardBenefitDocument : BookCard
    {
        public int BenefitDocumentTypeId
        {
            get { return ComboServ.GetComboIdInt(cbBenefitDocumentType).Value; }
            set { ComboServ.SetComboId(cbBenefitDocumentType, value); }
        }
        public int BenefitDocumentId
        {
            get { return ComboServ.GetComboIdInt(cbBenefitDocument).Value; }
            set { ComboServ.SetComboId(cbBenefitDocument, value); }
        }
        public string Series
        {
            get { return tbSeries.Text.Trim(); }
            set { tbSeries.Text = value;}
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

        private Guid _PersonId;
        private int _BenefitDocumentTypeId;

        public CardBenefitDocument(string id, Guid personId, int iBenefitDocumentTypeId)
        {
            InitializeComponent();
            _Id = id;
            _PersonId = personId;
            _BenefitDocumentTypeId = iBenefitDocumentTypeId;

            InitControls();
            _tableName = "ed.PersonBenefitDocument";
            _title = "Льгота";
        }

        protected override void ExtraInit()
        {
            base.ExtraInit();
            ComboServ.FillCombo(cbBenefitDocumentType, HelpClass.GetComboListByTable("ed.BenefitDocumentType WHERE Id > 1"), false, false);
            BenefitDocumentTypeId = _BenefitDocumentTypeId;
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
                var Person = context.Person.Where(x => x.Id == _PersonId).FirstOrDefault();
                lblFIO.Text = Person.Surname + " " + Person.Name + " " + Person.SecondName;

                if (_Id == null)
                    return;

                var value = context.PersonBenefitDocument.Where(x => x.Id == GuidId).FirstOrDefault();
                if (value == null)
                    WinFormsServ.Error("Не удалось загрузить из PersonBenefitDocument");

                BenefitDocumentTypeId = value.BenefitDocumentTypeId;
                BenefitDocumentId = value.BenefitDocumentId;
                Series = value.Series;
                Number = value.Number;
                Date = value.Date;
                Author = value.Author;
                HasOriginals = value.HasOriginals;
            }
        }

        protected override void InsertRec(PriemEntities context, ObjectParameter idParam)
        {
            context.PersonBenefitDocument_insert(_PersonId, BenefitDocumentTypeId, BenefitDocumentId, Series, Number, Date, Author, HasOriginals, null, idParam);
            _Id = idParam.Value.ToString();
        }
        protected override void UpdateRec(PriemEntities context, Guid id)
        {
            context.PersonBenefitDocument_update(BenefitDocumentTypeId, BenefitDocumentId, Series, Number, Date, Author, HasOriginals, null, GuidId);
        }

        private void cbBenefitDocumentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = context.BenefitDocument.Where(x => x.BenefitDocumentTypeId == BenefitDocumentTypeId)
                    .Select(x => new { x.Id, x.Name })
                    .ToList()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name))
                    .ToList();

                ComboServ.FillCombo(cbBenefitDocument, src, false, false);
            }
        }
    }
}
