using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Entity.Core.Objects;
using System.Transactions;
using System.Linq;
using EducServLib;

namespace PriemLib
{
    public class ChangeCompCelProtocol : ProtocolCard
    {
        //конструктор 
        public ChangeCompCelProtocol(ProtocolList owner, int iStudyLevelGroupId, int iFacultyId, int iStudyBasisId, int iStudyFormId)
            : this(owner, iStudyLevelGroupId, iFacultyId, iStudyBasisId, iStudyFormId, null)
        {
        }

        //конструктор 
        public ChangeCompCelProtocol(ProtocolList owner, int iStudyLevelGroupId, int iFacultyId, int iStudyBasisId, int iStudyFormId, Guid? ProtocolId)
            : base(owner, iStudyLevelGroupId, iFacultyId, iStudyBasisId, iStudyFormId, ProtocolId)
        {
            _type = ProtocolTypes.ChangeCompCelProtocol;
            base.sQuery = this.sQuery;
        }

        //дополнительная инициализация
        protected override void InitControls()
        {
            sQuery = @"SELECT DISTINCT qAbiturient.Sum, extPerson.EducDocument, qAbiturient.Id AS Id, qAbiturient.BAckDoc AS backdoc, 
             (qAbiturient.BAckDoc | qAbiturient.NotEnabled) AS Red, qAbiturient.RegNum AS Рег_Номер, 
             extPerson.FIO AS ФИО, 
             extPerson.EducDocument AS Документ_об_образовании, 
             extPerson.PassportSeries + ' №' + extPerson.PassportNumber AS Паспорт, 
             qAbiturient.ObrazProgramNameEx + ' ' + (CASE WHEN qAbiturient.ProfileId IS NULL THEN '' ELSE qAbiturient.ProfileName END) AS Направление, 
             Competition.NAme AS Конкурс, qAbiturient.BackDoc 
             FROM ed.qAbiturient INNER JOIN ed.extPerson ON qAbiturient.PersonId = extPerson.Id                
             LEFT JOIN ed.Competition ON Competition.Id = qAbiturient.CompetitionId
             INNER JOIN ed.qProtocolHistory ON qProtocolHistory.AbiturientId = qAbiturient.Id";

            base.InitControls();

            this.Text = "Протокол об изменении типа конкурса целевикам";
            this.chbEnable.Text = "Изменить всем выбранным слева абитуриентам тип конкурса с целевого на дополнительный/общий";

            this.chbFilter.Text = "Отфильтровать абитуриентов с проверенными данными";
            this.chbFilter.Visible = false;
        }

        protected override void InitAndFillGrids()
        {
            base.InitAndFillGrids();

            string sFilter = string.Empty;
            sFilter = " AND qAbiturient.CompetitionId = 6 ";

            FillGrid(dgvRight, sQuery, GetWhereClause("ed.qAbiturient") + sFilter, sOrderby);

            //заполнили левый
            if (_id != null)
            {
                sFilter = string.Format(" WHERE qAbiturient.Id IN (SELECT AbiturientId FROM ed.qProtocolHistory WHERE ProtocolId = '{0}')", _id.ToString());
                FillGrid(dgvLeft, sQuery, sFilter, sOrderby);
            }
            else //новый
            {
                InitGrid(dgvLeft);
            }
        }

        //подготовка нужного грида
        protected override void InitGrid(DataGridView dgv)
        {
            base.InitGrid(dgv);
        }

        protected override bool Save()
        {
            base.Save();
            try
            {
                ArrayList alQueries = new ArrayList();

                using (PriemEntities context = new PriemEntities())
                {
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        try
                        {
                            //меняет тип конкурса
                            foreach (DataGridViewRow r in dgvLeft.Rows)
                            {
                                SortedList slVals = new SortedList();
                                Guid? abitId = new Guid(r.Cells["Id"].Value.ToString());
                                                                

                                int? compNew = (from ab in context.qAbiturient
                                         where ab.Id == abitId
                                         select ab.OtherCompetitionId).FirstOrDefault();
                                if (compNew == null)
                                    compNew = 4;
                                if (compNew < 1 || compNew > 7)
                                    compNew = 4;

                                context.Abiturient_UpdateCompetititon(compNew, null, false, abitId);
                            }

                            transaction.Complete();
                        }
                        catch (Exception exc)
                        {
                            WinFormsServ.Error("Ошибка при сохранении данных: ", exc);
                        }
                    }
                }               
                return true;
            }

            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка при изменении типа конкурса: ", ex);
                return false;
            }
        }
    }
}
