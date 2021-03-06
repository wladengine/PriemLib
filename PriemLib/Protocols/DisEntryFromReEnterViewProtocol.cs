﻿using EducServLib;
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
    public partial class DisEntryFromReEnterViewProtocol : ProtocolCard
    {
        public DisEntryFromReEnterViewProtocol(ProtocolList owner, int iStudyLevelGroupId, int sFac, int sSection, int sForm, int? sProf, bool isSec, bool isReduced, bool isParal, bool isList)
            : this(owner, iStudyLevelGroupId, sFac, sSection, sForm, sProf, isSec, isReduced, isParal, isList, null)
        {
        }

        public DisEntryFromReEnterViewProtocol(ProtocolList owner, int iStudyLevelGroupId, int sFac, int sSection, int sForm, int? sProf, bool isSec, bool isReduced, bool isParal, bool isList, Guid? sProtocol)
            : base(owner, iStudyLevelGroupId, sFac, sSection, sForm, sProf, isSec, isReduced, isParal, isList, sProtocol)
        {
            _type = ProtocolTypes.DisEntryFromReEnterView;
        }

        //дополнительная инициализация
        protected override void InitControls()
        {
            sQuery = string.Format("SELECT DISTINCT ed.extAbitMarksSum.TotalSum as Sum, ed.extPerson.EducDocument, ed.extAbit.Id as Id, ed.extAbit.BAckDoc as backdoc, " +
            " 'false' as Red, ed.extAbit.RegNum as Рег_Номер, " +
            " ed.extPerson.FIO as ФИО, " +
            " ed.extPerson.EducDocument as Документ_об_образовании, " +
            " ed.extPerson.PassportSeries + ' №' + ed.extPerson.PassportNumber as Паспорт, " +
            " ed.extAbit.ObrazProgramName + ' ' +(Case when ed.extAbit.ProfileName IS NULL then '' else ed.extAbit.ProfileName end) as Направление, " +
            " ed.Competition.NAme as Конкурс, ed.extAbit.BackDoc " +
            " FROM ed.extAbit INNER JOIN ed.extPErson ON ed.extAbit.PErsonId = ed.extPerson.Id " +
            " INNER JOIN ed.extEnableProtocol ON ed.extAbit.Id=ed.extEnableProtocol.AbiturientId " +
            " INNER JOIN ed.extEntryView ON ed.extAbit.Id=ed.extEntryView.AbiturientId " +
            " INNER JOIN ed.extAbitMarksSum ON ed.extAbit.Id=ed.extAbitMarksSum.Id " +
            " LEFT JOIN ed.Competition ON ed.Competition.Id = ed.extAbit.CompetitionId ");

            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            lst.Add(new KeyValuePair<string, string>("1", "Зачисленные ранее"));
            ComboServ.FillCombo(cbHeaders, lst, false, false);

            //string q = string.Format("SELECT DISTINCT CONVERT(varchar(100), Id) AS Id, Number as Name FROM ed.extEntryView WHERE Excluded=0 AND IsOld=0 AND FacultyId ={0} AND StudyFormId = {1} AND StudyBasisId = {2} AND LicenseProgramId = {3} AND IsSecond = {4} AND IsReduced = {5} AND IsParallel = {6} AND IsListener = {7}", _facultyId, _studyFormId, _studyBasisId, _licenseProgramId, QueryServ.StringParseFromBool(_isSecond), QueryServ.StringParseFromBool(_isReduced), QueryServ.StringParseFromBool(_isParallel), QueryServ.StringParseFromBool(_isListener));
            //ComboServ.FillCombo(cbHeaders, HelpClass.GetComboListByQuery(q), false, false);

            cbHeaders.Visible = true;
            lblHeaderText.Text = "Из представления к зачислению №";
            chbInostr.Visible = true;

            cbHeaders.SelectedIndexChanged += new EventHandler(cbHeaders_SelectedIndexChanged);
            chbInostr.CheckedChanged += new System.EventHandler(UpdateGrids);

            base.InitControls();

            this.Text = "Приказ об исключении ";
            this.chbEnable.Text = "Добавить всех выбранных слева абитуриентов в приказ об исключении";

            this.chbFilter.Visible = false;
        }

        void cbHeaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateGrids();
        }

        public string HeaderId
        {
            get { return ComboServ.GetComboId(cbHeaders); }
            set { ComboServ.SetComboId(cbHeaders, value); }
        }

        protected override void InitAndFillGrids()
        {
            base.InitAndFillGrids();

            UpdateRight();
            string sFilter = string.Empty;

            //заполнили левый
            if (_id != null)
            {
                FillGrid(dgvLeft, sQuery, sFilter, sOrderby);
            }
            else //новый
            {
                InitGrid(dgvLeft);
            }
        }

        private void UpdateGrids(object sender, EventArgs e)
        {
            UpdateGrids();
        }

        void UpdateGrids()
        {
            UpdateRight();
            InitGrid(dgvLeft);
        }

        void UpdateRight()
        {
            if (HeaderId == null)
            {
                dgvRight.DataSource = null;
                return;
            }

            string sFilter = @" AND extAbit.PersonId IN 
 (
	SELECT Abiturient.PersonId FROM ed._FirstWaveGreen 
	INNER JOIN ed.Abiturient ON Abiturient.Id = _FirstWaveGreen.AbiturientId
	WHERE NOT EXISTS (SELECT * FROM ed.hlpAbitToGo WHERE hlpAbitToGo.PersonId = Abiturient.PersonId AND hlpAbitToGo.IsGo = 1 AND hlpAbitToGo.Priority < Abiturient.Priority)
	UNION 
	SELECT Abiturient.PersonId FROM ed._FirstWaveYellow 
	INNER JOIN ed.Abiturient ON Abiturient.Id = _FirstWaveYellow.AbiturientId
	WHERE NOT EXISTS (SELECT * FROM ed.hlpAbitToGo WHERE hlpAbitToGo.PersonId = Abiturient.PersonId AND hlpAbitToGo.IsGo = 1 AND hlpAbitToGo.Priority < Abiturient.Priority)
	UNION 
	SELECT Abiturient.PersonId FROM ed._FirstWaveLast
	INNER JOIN ed.Abiturient ON Abiturient.Id = _FirstWaveLast.AbiturientId
	WHERE NOT EXISTS (SELECT * FROM ed.hlpAbitToGo WHERE hlpAbitToGo.PersonId = Abiturient.PersonId AND hlpAbitToGo.IsGo = 1 AND hlpAbitToGo.Priority < Abiturient.Priority)
)
AND extAbit.Id NOT IN (SELECT AbiturientId FROM ed.extDisEntryFromReEnterView)
";

            FillGrid(dgvRight, sQuery, GetWhereClause("ed.extAbit") + sFilter, sOrderby);
        }

        //подготовка нужного грида
        protected override void InitGrid(DataGridView dgv)
        {
            base.InitGrid(dgv);

            dgv.Columns["Pasport"].Visible = false;
            dgv.Columns["Attestat"].Visible = false;
        }
    }
}
