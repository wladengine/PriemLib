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
    public partial class CardBenefintDocument_Select : Form
    {
        public event Action ToUpdateList;
        private Guid _personId;
        public CardBenefintDocument_Select(Guid PersonId)
        {
            InitializeComponent();

            _personId = PersonId;

            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            lst.Add(new KeyValuePair<string, string>("Olympiad", "Олимпиада"));
            lst.Add(new KeyValuePair<string, string>("DisabilityDocument", "Справка об установлении инвалидности"));
            lst.Add(new KeyValuePair<string, string>("BenefitDocument2", "Заключение психолого-медико-педагогической комиссии"));
            lst.Add(new KeyValuePair<string, string>("BenefitDocument3", "Заключение об отсутствии противопоказаний для обучения"));
            lst.Add(new KeyValuePair<string, string>("BenefitDocument5", "Документ, подтверждающий принадлежность к детям-сиротам и детям, оставшимся без попечения родителей"));
            lst.Add(new KeyValuePair<string, string>("BenefitDocument6", "Диплом в области спорта"));
            lst.Add(new KeyValuePair<string, string>("BenefitDocument7", "Документ, подтверждающий принадлежность к соотечественникам"));
            lst.Add(new KeyValuePair<string, string>("BenefitDocument8", "Документ, подтверждающий принадлежность к ветеранам боевых действий"));
            lst.Add(new KeyValuePair<string, string>("BenefitDocument9", "Документы для ординатуры"));

            ComboServ.FillCombo(cbBenefitDocumentType, lst, false, false);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            string sType = ComboServ.GetComboId(cbBenefitDocumentType);
            BookCard crd;
            switch (sType)
            {
                case "Olympiad": { crd = new OlympCard(_personId); break; }
                case "DisabilityDocument": { crd = new CardDisabilityDocument(null, _personId); break; }
                case "BenefitDocument2": { crd = new CardBenefitDocument(null, _personId, 2); break; }
                case "BenefitDocument3": { crd = new CardBenefitDocument(null, _personId, 3); break; }
                case "BenefitDocument5": { crd = new CardBenefitDocument(null, _personId, 5); break; }
                case "BenefitDocument6": { crd = new CardBenefitDocument(null, _personId, 6); break; }
                case "BenefitDocument7": { crd = new CardBenefitDocument(null, _personId, 7); break; }
                case "BenefitDocument8": { crd = new CardBenefitDocument(null, _personId, 8); break; }
                case "BenefitDocument9": { crd = new CardBenefitDocument(null, _personId, 9); break; }
                default: { crd = new OlympCard(_personId); break; }
            }

            crd.Show();
            crd.ToUpdateList += ToUpdateList;

            this.Close();
        }
    }
}
