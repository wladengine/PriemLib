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

namespace PriemLib 
{
    public partial class CardLangCertificate : Form
    {
        long? _Id;
        Guid PersonId;
        UpdateHandler _hdl;

        public List<KeyValuePair<int, bool>> cbBooltype;

        public CardLangCertificate(long? id, Guid persId, UpdateHandler h)
        {
            InitializeComponent();
            _Id = id;
            _hdl = h;
            PersonId = persId;
            this.MdiParent = MainClass.mainform;
            FillCard();
        }
        public void FillCard()
        {
            cbType.SelectedIndexChanged -= new EventHandler(cbType_SelectedIndexChanged);
            ComboServ.FillCombo(cbType, HelpClass.GetComboListByTable("ed.LanguageCertificatesType"), false, false);
            cbType.SelectedIndexChanged += new EventHandler(cbType_SelectedIndexChanged);

            using (PriemEntities context = new PriemEntities())
            {
                cbBooltype = (from x in context.LanguageCertificatesType
                             select new
                            {
                                x.Id,
                                x.BoolType
                            }).ToList().Select(x => new KeyValuePair<int, bool>(x.Id, x.BoolType)).ToList();

                btnAdd.Text = "Добавить";
                if (!_Id.HasValue)
                    return;

                btnAdd.Text = "Сохранить";
                var Lang = (from x in context.PersonLanguageCertificates
                            where x.Id == _Id
                            select x).FirstOrDefault();
                ComboServ.SetComboId(cbType, Lang.LanguageCertificateTypeId);
                tbNumber.Text = Lang.Number;
                tbResult.Text = cbBooltype.Where(x=>x.Key == Lang.LanguageCertificateTypeId).
                    Select(x=>x.Value).First() ? "сдан" : Lang.ResultValue.ToString();
                tbResult.Enabled = !cbBooltype.Where(x => x.Key == Lang.LanguageCertificateTypeId).
                    Select(x => x.Value).First();
            }
        }
        private bool Check()
        {
            double tmp;
            bool isBool = cbBooltype.Where(x => x.Key == ComboServ.GetComboIdInt(cbType)).Select(x => x.Value).First();
            if (!isBool && !double.TryParse(tbResult.Text, out tmp))
            {
                MessageBox.Show("некорректное значение");
                return false;
            }
            return true;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!Check())
                return;

            using (PriemEntities context = new PriemEntities())
            {
                bool isNew = false;
                PersonLanguageCertificates lang = context.PersonLanguageCertificates.Where(x => x.Id == _Id).FirstOrDefault();
                if (lang == null)
                {
                    isNew = true;
                    lang = new PersonLanguageCertificates();
                    lang.PersonId = PersonId;
                }

                lang.LanguageCertificateTypeId = ComboServ.GetComboIdInt(cbType).Value;
                lang.Number = tbNumber.Text.Trim();
                bool isBool = cbBooltype.Where(x => x.Key == ComboServ.GetComboIdInt(cbType)).Select(x => x.Value).First();


                if (!isBool)
                {
                    double tmp;
                    if (!double.TryParse(tbResult.Text, out tmp))
                        tmp = 0;
                    lang.ResultValue = tmp;
                }

                if (isNew)
                    context.PersonLanguageCertificates.Add(lang);

                context.SaveChanges();
                if (_hdl != null)
                    _hdl();
                this.Close();
            }
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isBool = cbBooltype.Where(x => x.Key == ComboServ.GetComboIdInt(cbType)).Select(x => x.Value).First();
            tbResult.Enabled = !isBool;
        }
    }
}
