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
    public partial class CardBenefintDocument_Select : Form
    {
        public CardBenefintDocument_Select()
        {
            InitializeComponent();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            lst.Add(new KeyValuePair<string, string>("Olympiad", "Олимпиада"));
            lst.Add(new KeyValuePair<string, string>("DisabilityDocument", ""));
            lst.Add(new KeyValuePair<string, string>("", ""));
            lst.Add(new KeyValuePair<string, string>("", ""));
            lst.Add(new KeyValuePair<string, string>("", ""));
            lst.Add(new KeyValuePair<string, string>("", ""));
            lst.Add(new KeyValuePair<string, string>("", ""));
            lst.Add(new KeyValuePair<string, string>("", ""));
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {

        }
    }
}
