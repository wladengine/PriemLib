using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PriemLib
{
    public class EnableProtocol : ProtocolCard
    {
        //конструктор         
        public EnableProtocol(ProtocolList owner, int iStudyLevelGroupId, int iFacultyId, int iStudyBasisId, int iStudyFormId)
            : base(owner, iStudyLevelGroupId, iFacultyId, iStudyBasisId, iStudyFormId)
        {
            _type = ProtocolTypes.EnableProtocol;
        }

        public EnableProtocol(ProtocolList owner, int iStudyLevelGroupId, int iFacultyId, int iStudyBasisId, int iStudyFormId, Guid gId)
            : base(owner, iStudyLevelGroupId, iFacultyId, iStudyBasisId, iStudyFormId, gId)
        {
            _type = ProtocolTypes.EnableProtocol;
        }

        public EnableProtocol() : base()
        { 
            //InitializeComponent(); 
        }

        //дополнительная инициализация
        protected override void InitControls()
        {
            //инвалиды (extPerson.Privileges & 32 > 0) имеют право пересдать ЕГЭ по запоротому предмету (где получили меньше минимума)
            sQuery = @"SELECT DISTINCT qAbiturient.Sum, extPerson.EducDocument, qAbiturient.Id as Id, qAbiturient.BAckDoc as backdoc, 
(qAbiturient.BAckDoc | qAbiturient.NotEnabled | case when (NOT hlpMinEgeAbiturient.Id IS NULL AND extPerson.Privileges & 32 = 0) then 'true' else 'false' end) as Red, 
qAbiturient.RegNum as Рег_Номер, 
extPerson.FIO as ФИО, 
extPerson.EducDocument as Документ_об_образовании, 
extPerson.PassportSeries + ' №' + extPerson.PassportNumber as Паспорт, 
qAbiturient.ObrazProgramNameEx + ' ' + (Case when qAbiturient.ProfileId IS NULL then '' else qAbiturient.ProfileName end) as Направление, 
Competition.Name as Конкурс, qAbiturient.BackDoc 
FROM ed.qAbiturient 
INNER JOIN ed.extPerson ON qAbiturient.PersonId = extPerson.Id   
LEFT JOIN ed.hlpMinEgeAbiturient ON hlpMinEgeAbiturient.Id = qAbiturient.Id
LEFT JOIN ed.Competition ON Competition.Id = qAbiturient.CompetitionId";

            base.InitControls();

            this.Text = "Протокол о допуске";
            this.chbEnable.Text = "Добавить всех выбранных слева абитуриентов в протокол о допуске";

            this.chbFilter.Text = "Отфильтровать абитуриентов с проверенными данными";
            this.chbFilter.Visible = true;
        }

        protected override void InitAndFillGrids()
        {
            base.InitAndFillGrids();

            string sFilter = string.Format(@" AND qAbiturient.BackDoc = 0 AND qAbiturient.NotEnabled = 0 
AND qAbiturient.Id NOT IN 
(
    SELECT AbiturientId 
    FROM ed.extEnableProtocol 
    WHERE Excluded=0 AND FacultyId = {0} AND StudyFormId = {1} AND StudyBasisId = {2}
)", _facultyId.ToString(), _studyFormId.ToString(), _studyBasisId.ToString());

            if (chbFilter.Checked)
                sFilter += " AND qAbiturient.Checked > 0";

            //сперва общий конкурс (не общ-преим), т.к. чернобыльцы негодуют - льготы есть, а в протокол не попасть
            FillGrid(dgvRight, sQuery, GetWhereClause("ed.qAbiturient") + sFilter + " AND qAbiturient.CompetitionId NOT IN (1,2,7,8) ", sOrderby);

            //заполнили левый
            if (_id != null)
            {
                FillGrid(dgvLeft, sQuery, string.Format(" WHERE qAbiturient.Id IN (SELECT AbiturientId FROM ed.qProtocolHistory WHERE ProtocolId = '{0}')", _id.ToString()), sOrderby);
            }
            else //новый
            {
                InitGrid(dgvLeft);
            }

            // заполнение льготников, проверенных советниками
            string query = sQuery + GetWhereClause("ed.qAbiturient") + sFilter + " AND (qAbiturient.CompetitionId IN (1,8) OR (extPerson.Privileges>0 AND qAbiturient.CompetitionId IN (2,7))) AND qAbiturient.Checked > 0 ";

            DataSet ds = MainClass.Bdc.GetDataSet(query);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DataGridViewRow r = new DataGridViewRow();
                r.CreateCells(dgvLeft, false, dr["Id"].ToString(), dr["Red"].ToString(), true, dr["backdoc"].ToString(), dr["Рег_номер"].ToString(), dr["ФИО"].ToString(), dr["Sum"].ToString(), dr["Документ_об_образовании"].ToString(), dr["Направление"].ToString(), dr["Конкурс"].ToString(), dr["Паспорт"].ToString());
                if (bool.Parse(dr["Red"].ToString()))
                {
                    r.Cells[5].Style.ForeColor = Color.Red;
                    r.Cells[6].Style.ForeColor = Color.Red;
                }

                r.Cells[5].Style.BackColor = Color.Green;
                r.Cells[6].Style.BackColor = Color.Green;

                dgvLeft.Rows.Add(r);
            }

            //блокируем редактирование кроме первого столбца
            for (int i = 1; i < dgvLeft.ColumnCount; i++)
                dgvLeft.Columns[i].ReadOnly = true;

            dgvLeft.Update();
        }

        //подготовка нужного грида
        protected override void InitGrid(DataGridView dgv)
        {
            base.InitGrid(dgv);
        }
    }
}