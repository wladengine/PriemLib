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
    public partial class OtherPersonPassportList : Form
    {
        private Guid PersonId;

        public OtherPersonPassportList(Guid _personId)
        {
            InitializeComponent();
            PersonId = _personId;

            this.MdiParent = MainClass.mainform;

            UpdateGrid();
        }

        private void UpdateGrid()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = context.PersonOtherPassport.Where(x => x.PersonId == PersonId)
                    .Select(x => new { x.Id, PassportType = x.PassportType.Name, x.PassportSeries, x.PassportNumber, x.Surname, x.Name, x.SecondName })
                    .ToArray();

                dgvPassportsList.DataSource = Converter.ConvertToDataTable(src);
                dgvPassportsList.Columns["Id"].Visible = false;
                dgvPassportsList.Columns["PassportType"].HeaderText = "Тип";
                dgvPassportsList.Columns["PassportSeries"].HeaderText = "Серия";
                dgvPassportsList.Columns["PassportNumber"].HeaderText = "Номер";
                dgvPassportsList.Columns["Surname"].HeaderText = "Фамилия";
                dgvPassportsList.Columns["Name"].HeaderText = "Имя";
                dgvPassportsList.Columns["SecondName"].HeaderText = "Отчество";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var crd = new CardPersonOtherPassport(PersonId);
            crd.ToUpdateList += UpdateGrid;
            crd.Show();
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (dgvPassportsList.SelectedCells.Count == 0)
                return;

            int rwInd = dgvPassportsList.SelectedCells[0].RowIndex;
            int id = (int)dgvPassportsList["Id", rwInd].Value;

            var crd = new CardPersonOtherPassport(id.ToString());
            crd.ToUpdateList += UpdateGrid;
            crd.Show();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvPassportsList.SelectedCells.Count == 0)
                return;

            int rwInd = dgvPassportsList.SelectedCells[0].RowIndex;
            int id = (int)dgvPassportsList["Id", rwInd].Value;

            using (PriemEntities context = new PriemEntities())
            {
                context.PersonOtherPassport_delete(id);
            }

            UpdateGrid();
        }
    }
}
