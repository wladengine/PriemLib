using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Transactions;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Entity.Core.Objects;

using BDClassLib;
using EducServLib;
using WordOut;

namespace PriemLib
{
    public class EntryViewProtocol : ProtocolCard
    {
        Dictionary<int?, List<string>> lstSelected;

        public EntryViewProtocol(ProtocolList owner, int iStudyLevelGroup, int sFac, int sSection, int sForm, int? sProf, bool isSec, bool isReduced, bool isParal, bool isList, bool isCel)
            : this(owner, iStudyLevelGroup, sFac, sSection, sForm, sProf, isSec, isReduced, isParal, isList, isCel, null)
        {
        }

        //конструктор 
        public EntryViewProtocol(ProtocolList owner, int iStudyLevelGroup, int sFac, int sSection, int sForm, int? sProf, bool isSec, bool isReduced, bool isParal, bool isList, bool isCel, Guid? sProtocol)
            : base(owner, iStudyLevelGroup, sFac, sSection, sForm, sProf, isSec, isReduced, isParal, isList, isCel, sProtocol)
        {
            _type = ProtocolTypes.EntryView;
        }

        //дополнительная инициализация
        protected override void  InitControls()
        {
            using (PriemEntities context = new PriemEntities())
            {
               string ehQuery = string.Empty; 
                
                if (_isCel)
                   ehQuery = "SELECT CONVERT(varchar(100), Id) AS Id, Acronym as Name FROM ed.EntryHeader WHERE Id IN (7) ORDER BY Id";
                else
                {
                    if(MainClass.dbType == PriemType.PriemMag)
                        ehQuery = "SELECT CONVERT(varchar(100), Id) AS Id, Acronym as Name FROM ed.EntryHeader WHERE Id IN (8,10, 11, 12) ORDER BY Id";                        
                    else
                        ehQuery = "SELECT CONVERT(varchar(100), Id) AS Id, Acronym as Name FROM ed.EntryHeader WHERE Id IN (8, 9, 10, 11, 12) ORDER BY Id";
                }

                ComboServ.FillCombo(cbHeaders, HelpClass.GetComboListByQuery(ehQuery), false, false);
                
                cbHeaders.Visible = true;
                cbHeaders.SelectedIndexChanged += new EventHandler(cbHeaders_SelectedIndexChanged);

                lstSelected = new Dictionary<int?, List<string>>();
                foreach(KeyValuePair<string,string> val in cbHeaders.Items)
                { 
                    lstSelected.Add(int.Parse(val.Key), new List<string>());
                }
                
                base.InitControls();

                if (!MainClass.bEntryViewCreateEnabled)
                {
                    MessageBox.Show("Создание представлений к зачислению в настоящий момент запрещено администратором");
                    btnOk.Enabled = false;
                }

                this.Text = "Представление о зачислении";
                this.chbEnable.Text = "Добавить всех выбранных слева абитуриентов в представление о зачислении";

                this.chbFilter.Visible = false;

                sQuery = "SELECT DISTINCT extAbitMarksSum.TotalSum as Sum, extPerson.EducDocument, qAbiturient.Id as Id, qAbiturient.BAckDoc as backdoc, " +
                    " 'false' as Red, qAbiturient.RegNum as Рег_Номер, " +
                    " extPerson.FIO as ФИО, " +
                    " extPerson.EducDocument as Документ_об_образовании, " +
                    " extPerson.PassportSeries + ' №' + extPerson.PassportNumber as Паспорт, " +
                    " LicenseProgramCode + ' ' + LicenseProgramName + ' ' +(Case when NOT ed.qAbiturient.ProfileId IS NULL then ProfileName else ObrazProgramName end) as Направление, " +
                    " Competition.NAme as Конкурс, ed.qAbiturient.BackDoc, _FirstWave.SortNum " +
                    " FROM ed.qAbiturient INNER JOIN ed.extPerson ON ed.qAbiturient.PErsonId =  ed.extPerson.Id " +
                    " INNER JOIN ed.extEnableProtocol ON ed.qAbiturient.Id=ed.extEnableProtocol.AbiturientId " +
                    " INNER JOIN ed._FirstWave AS _FirstWave ON ed.qAbiturient.Id= _FirstWave.AbiturientId " +
                    " LeFT JOIN ed._FirstWaveGreen ON ed.qAbiturient.Id= _FirstWaveGreen.AbiturientId " +
                    " LEFT JOIN ed.extAbitMarksSum ON ed.qAbiturient.Id=ed.extAbitMarksSum.Id " +
                    " LEFT JOIN ed.Competition ON ed.Competition.Id = ed.qAbiturient.CompetitionId ";
            }
        }

        void cbHeaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateGrids();
        }

        public int? HeaderId
        {
            get { return ComboServ.GetComboIdInt(cbHeaders); }
            set { ComboServ.SetComboId(cbHeaders, value); }
        }

        string GetTotalFilter()
        {
            return GetTotalFilter(true);
        }

        string GetTotalFilter(bool header)
        {
            string sFilter = string.Empty;

            //обработали Направление 
            if (_licenseProgramId != null)
                sFilter += " AND extEntry.LicenseProgramId = " + _licenseProgramId;

            sFilter += " AND extEntry.IsSecond = " + QueryServ.StringParseFromBool(_isSecond);
            sFilter += " AND extEntry.IsReduced = " + QueryServ.StringParseFromBool(_isReduced);
            sFilter += " AND extEntry.IsParallel = " + QueryServ.StringParseFromBool(_isParallel);

            //обработали слушатель
            sFilter += " AND Abiturient.IsListener = " + QueryServ.StringParseFromBool(_isListener);

            sFilter += " AND Abiturient.BackDoc = 0 ";

            sFilter += " AND Abiturient.Id NOT IN (SELECT AbiturientId FROM ed.extEntryView) ";

            if (_studyBasisId == 1)
                sFilter += string.Format(" AND Abiturient.PersonId NOT IN (SELECT PersonId FROM ed.extEntryView WHERE StudyLevelGroupId = {0} AND StudyBasisId = 1)", _studyLevelGroupId);

//            sFilter += @"AND ((ed.qAbiturient.IsListener = 0 AND ed.qAbiturient.IsSecond = 0 AND ed.qAbiturient.IsReduced = 0 AND ed.qAbiturient.IsParallel = 0 
//AND EXISTS (SELECT * FROM ed.Abiturient AB WHERE AB.HasOriginals > 0 AND AB.PersonId = qAbiturient.PersonId)) 
//OR ed.qAbiturient.IsListener = 1 OR ed.qAbiturient.IsSecond = 1 OR ed.qAbiturient.IsReduced = 1 OR ed.qAbiturient.IsParallel = 1 OR ed.qAbiturient.IsPaid = 1)";
            sFilter += @"AND 
(
	(
		Abiturient.IsListener = 0 
		AND extEntry.IsSecond = 0 
		AND extEntry.IsReduced = 0 
		AND extEntry.IsParallel = 0 
		AND EXISTS 
		(
			SELECT * 
            FROM ed.Abiturient AB 
			WHERE AB.HasOriginals > 0 
            AND AB.PersonId = Abiturient.PersonId
            AND AB.BackDoc = 0
		)
	) 
	OR Abiturient.IsListener = 1 
	OR extEntry.IsSecond = 1 
	OR extEntry.IsReduced = 1 
	OR extEntry.IsParallel = 1 
	OR Abiturient.IsPaid = 1
)";
      
            if (_studyBasisId == 2)
            {
                sFilter += " AND Abiturient.IsPaid > 0 ";
                sFilter += " AND EXISTS (SELECT TOP(1) PaidData.Id FROM ed.PaidData WHERE ed.PaidData.AbiturientId = Abiturient.Id) ";
            }

            if (header)
            {
                switch (HeaderId)
                {
                    case 1:
                        sFilter += " AND extEntry.IsCrimea = 0 AND Abiturient.CompetitionId=1 ";
                        sFilter += " AND Abiturient.Id IN (SELECT AbiturientId FROM ed.Olympiads WHERE OlympTypeId=1) ";
                        break;
                    case 2:
                        sFilter += " AND extEntry.IsCrimea = 0 AND Abiturient.CompetitionId=1 ";
                        sFilter += " AND Abiturient.Id IN (SELECT AbiturientId FROM ed.Olympiads WHERE OlympValueId=6 AND OlympTypeId=2) ";
                        break;
                    case 3:
                        sFilter += " AND extEntry.IsCrimea = 0 "; 
                        sFilter += " AND Abiturient.Id IN (SELECT AbiturientId FROM ed.Olympiads WHERE OlympValueId=5 AND OlympTypeId=2) ";
                        break;
                    case 4:
                        sFilter += " AND extEntry.IsCrimea = 0 "; 
                        sFilter += " AND Abiturient.Id IN (SELECT AbiturientId FROM ed.Olympiads WHERE OlympValueId=6 AND OlympTypeId IN (3,4)) ";
                        break;
                    case 5:
                        sFilter += " AND extEntry.IsCrimea = 0 ";
                        sFilter += " AND Abiturient.Id IN (SELECT AbiturientId FROM ed.Olympiads WHERE OlympValueId=5 AND OlympTypeId IN (3,4)) ";
                        break;
                    case 6:
                        sFilter += " AND extEntry.IsCrimea = 0 AND Abiturient.CompetitionId = 2 ";
                        break;
                    case 7:
                        sFilter += " AND extEntry.IsCrimea = 0 AND Abiturient.CompetitionId = 6";
                        break;
                    case 8:
                        sFilter += " AND extEntry.IsCrimea = 0 AND Abiturient.CompetitionId NOT IN (1,2,6,7,8,11,12) ";
                        break;
                    case 9:
                        sFilter += " AND extEntry.IsCrimea = 0 AND Abiturient.CompetitionId IN (1,8) ";
                        break;
                    case 10:
                        sFilter += " AND extEntry.IsCrimea = 0 AND Abiturient.CompetitionId IN (2,7) ";
                        break;
                    case 11:
                        sFilter += " AND extEntry.IsCrimea = 1 AND Abiturient.CompetitionId NOT IN (1,8)  ";
                        break;
                    case 12:
                        sFilter += " AND extEntry.IsCrimea = 1 AND Abiturient.CompetitionId IN (1,8) ";
                        break;
                }
            }
            return GetWhereClause("extEntry") + sFilter;
        }
                
        //int GetTotalCount()
        //{            
        //    string query = string.Format("SELECT DISTINCT Count(qAbiturient.Id) " +
        //    " FROM qAbiturient INNER JOIN PErson ON ed.qAbiturientPErsonId = Person.Id " +
        //    " INNER JOIN extEnableProtocol ON ed.qAbiturientId=extEnableProtocol.AbiturientId " +
        //    " INNER JOIN _FirstWaveGreenBackup ON ed.qAbiturientId=_FirstWaveGreenBackup.AbiturientId " +
        //    " INNER JOIN extAbitMarksSum ON ed.qAbiturientId=extAbitMarksSum.Id " +
        //    " LEFT JOIN Specialization ON Specialization.Id = ed.qAbiturientSpecializationId LEFT JOIN StudyBasis ON StudyBasis.Id = ed.qAbiturientStudyBasisId " +
        //    " LEFT JOIN StudyForm ON StudyForm.Id = ed.qAbiturientStudyFormId LEFT JOIN Profession ON Profession.Id = ed.qAbiturientProfessionId " +
        //    " LEFT JOIN Competition ON Competition.Id = ed.qAbiturientCompetitionId ", MainClass.GetStringAbitNumber("qAbiturient"));

        //    return (int)MainClass.Bdc.GetValue(query + GetTotalFilter(false));
        //}

        protected override void InitAndFillGrids()
        {
            base.InitAndFillGrids();            
            /*
            FillGrid(dgvRight, sQuery, GetTotalFilter() , sOrderby);
                        
            //заполнили левый
            if (_id!=null)
            {
                string sFilter = string.Format(" WHERE ed.qAbiturientId IN (SELECT AbiturientId FROM qProtocolHistory WHERE ProtocolId = '{0}')", _id);
                FillGrid(dgvLeft, sQuery, sFilter, sOrderby);
            }
            else //новый
            {
                InitGrid(dgvLeft);
            }
            
            //блокируем редактирование кроме первого столбца
            for (int i = 1; i < dgvLeft.ColumnCount; i++)
                dgvLeft.Columns[i].ReadOnly = true;

            dgvLeft.Update();    */
            UpdateLeft();
            UpdateRight();
        }

        //подготовка нужного грида
        protected override void InitGrid(DataGridView dgv)
        {
            base.InitGrid(dgv);

            dgv.Columns["Pasport"].Visible = false;
            dgv.Columns["Attestat"].Visible = false;

        }

        string GetSelectedIdList()
        {
            List<string> lstRez = new List<string>();

            foreach (List<string> lst in lstSelected.Values)
            {
                string temp = Util.BuildStringWithCollection(lst);
                if(temp.Length>0)
                    lstRez.Add(temp);
            }

            return Util.BuildStringWithCollection(lstRez);
        }

        void UpdateGrids()
        {
            UpdateLeft();
            UpdateRight();
        }

        void UpdateRight()
        {  
            string ids = GetSelectedIdList();

            string filt = string.IsNullOrEmpty(ids) ? "" : string.Format(" AND ed.qAbiturient.Id NOT IN ({0}) ", ids);
            dgvRight.Rows.Clear();
            
            DataTable dtAbits = new DataTable();

            string queryPrograms = 
                string.Format(@"SELECT DISTINCT ObrazProgramId, ProfileId, KCP AS Value, KCPCel AS ValueCel 
                                FROM ed.qEntry
                                WHERE StudyLevelGroupId = {7} AND FacultyId = {0} AND StudyFormId = {1} AND StudyBasisId = {2} 
                                {3} AND IsSecond = {4} AND IsReduced = {5} AND IsParallel = {6} 
                                AND IsForeign = {8} AND IsCrimea = {9}", 
                                _facultyId, 
                                _studyFormId, 
                                _studyBasisId, 
                                _licenseProgramId.HasValue ? string.Format(" AND LicenseProgramId = {0} ", _licenseProgramId) : "", 
                                QueryServ.StringParseFromBool(_isSecond), 
                                QueryServ.StringParseFromBool(_isReduced), 
                                QueryServ.StringParseFromBool(_isParallel), 
                                _studyLevelGroupId, 
                                QueryServ.StringParseFromBool(MainClass.dbType == PriemType.PriemForeigners), 
                                QueryServ.StringParseFromBool(HeaderId == 11 || HeaderId == 12));

            DataSet dsPrograms = MainClass.Bdc.GetDataSet(queryPrograms);

            //по каждой программе-профилю смотрим КЦ и оставшиеся места и зачисляем подавших подлинники
            foreach (DataRow dr in dsPrograms.Tables[0].Rows)
            {                    
                string obProg = dr["ObrazProgramId"].ToString();
                string spec = dr["ProfileId"].ToString();

                string enteredQuery = 
                    string.Format(@"SELECT Count(Abiturient.Id) FROM ed.Abiturient 
                        INNER JOIN ed.extEntry E ON E.Id = Abiturient.EntryId 
                        INNER JOIN ed.extEntryView ON Abiturient.Id = extEntryView.AbiturientId 
                        WHERE Excluded=0 AND E.StudyLevelGroupId = {9} AND (E.Id = Abiturient.EntryId OR E.ParentEntryId = Abiturient.EntryId)
                        AND E.FacultyId={0} AND E.StudyFormId={1} AND E.StudyBasisId={2} 
                        {3} AND E.IsSEcond = {4} AND E.IsReduced = {5} AND E.IsParallel = {6} AND E.ObrazProgramId={7} {8}", 
                        _facultyId, 
                        _studyFormId, 
                        _studyBasisId,
                        _licenseProgramId.HasValue ? string.Format(" AND E.LicenseProgramId = {0} ", _licenseProgramId) : "", 
                        QueryServ.StringParseFromBool(_isSecond), 
                        QueryServ.StringParseFromBool(_isReduced), 
                        QueryServ.StringParseFromBool(_isParallel),
                        obProg, 
                        string.IsNullOrEmpty(spec) ? " AND E.ProfileId IS NULL " : string.Format(" AND E.ProfileId='{0}'", spec), 
                        _studyLevelGroupId);

                if (_isCel)
                    enteredQuery += " AND Abiturient.CompetitionId = 6 ";

                if (HeaderId == 11 || HeaderId == 12)
                    enteredQuery += " AND E.IsCrimea = 1";

                enteredQuery += string.Format(" AND E.IsForeign = {0}", (MainClass.dbType == PriemType.PriemForeigners) ? "1" : "0");
                  
                int entered = 0;
                int kc = 0;
                    
                int.TryParse(MainClass.Bdc.GetStringValue(enteredQuery), out entered);
                if(_isCel)
                    int.TryParse(dr["ValueCel"].ToString(), out kc);
                else
                    int.TryParse(dr["Value"].ToString(), out kc);

                int kcRest = kc - entered;
                if (_studyLevelGroupId == 1)
                {
                    if (MainClass.bFirstWaveEnabled && !_isCel && HeaderId == 8)
                        kcRest = (int)(((float)kcRest * 0.8f) + 0.999f);
                }

                string sQueryBody = string.Format(@"SELECT DISTINCT TOP ({0}) extAbitMarksSum.TotalSum + ISNULL(extAbitAdditionalMarksSum.AdditionalMarksSum, 0) as Sum, 
Abiturient.Id as Id, Abiturient.BackDoc as backdoc,
'false' as Red, Abiturient.RegNum as Рег_Номер, extPerson.FIO as ФИО,
extPerson.EducDocument as Документ_об_образовании, 
extPerson.PassportSeries + ' №' + extPerson.PassportNumber as Паспорт, 
LicenseProgramCode + ' ' + LicenseProgramName + ' ' + (Case when ProfileId <> 0 then ProfileName else ObrazProgramName end) as Направление,
Competition.Name as Конкурс, _FirstWave.SortNum
FROM ed.Abiturient 
INNER JOIN ed.extEntry ON extEntry.Id = Abiturient.EntryId
INNER JOIN ed.extPerson ON Abiturient.PersonId = extPerson.Id 
INNER JOIN ed.extEnableProtocol ON Abiturient.Id=ed.extEnableProtocol.AbiturientId 
LEFT JOIN ed.extAbitAdditionalMarksSum ON extAbitAdditionalMarksSum.AbiturientId = Abiturient.Id
INNER JOIN ed._FirstWave AS _FirstWave ON Abiturient.Id = _FirstWave.AbiturientId  " +
(((_studyBasisId == 2 && MainClass.dbType == PriemType.Priem) || MainClass.dbType == PriemType.PriemAG) ? " INNER JOIN ed._FirstWaveGreen ON Abiturient.Id = _FirstWaveGreen.AbiturientId " : "") +
string.Format(@"LEFT JOIN ed.{0}extAbitMarksSum ON Abiturient.Id = extAbitMarksSum.Id 
LEFT JOIN ed.Competition ON Competition.Id = Abiturient.CompetitionId ", MainClass.dbType == PriemType.PriemAG ? "extAbitMarksSumAG AS " : ""), kcRest);

                string sQueryJoinFW = string.Empty;

                string sFilter = GetTotalFilter() + filt;
                sFilter += " AND extEntry.ObrazProgramId = " + obProg;
                sFilter += string.IsNullOrEmpty(spec) ? " AND extEntry.ProfileId = 0 " : " AND extEntry.ProfileId='" + spec + "'";

                if (_studyBasisId == 1 && MainClass.dbType != PriemType.PriemAG)
                {
                    sFilter += string.Format(@" AND
(
    Abiturient.Id IN
    ( 
	    SELECT Abiturient.Id FROM ed._FirstWaveGreen 
	    INNER JOIN ed.Abiturient ON Abiturient.Id = _FirstWaveGreen.AbiturientId
	    WHERE NOT EXISTS 
        (SELECT * FROM ed.hlpAbitToGo WHERE hlpAbitToGo.PersonId = Abiturient.PersonId AND hlpAbitToGo.IsGo = 1 
        AND hlpAbitToGo.Priority < Abiturient.Priority AND hlpAbitToGo.StudyLevelGroupId = {0})
	    UNION 
	    SELECT Abiturient.Id FROM ed._FirstWaveYellow 
	    INNER JOIN ed.Abiturient ON Abiturient.Id = _FirstWaveYellow.AbiturientId
	    WHERE NOT EXISTS 
        (SELECT * FROM ed.hlpAbitToGo WHERE hlpAbitToGo.PersonId = Abiturient.PersonId AND hlpAbitToGo.IsGo = 1 
        AND hlpAbitToGo.Priority < Abiturient.Priority AND hlpAbitToGo.StudyLevelGroupId = {0})
	    UNION 
	    SELECT Abiturient.Id FROM ed._FirstWaveLast
	    INNER JOIN ed.Abiturient ON Abiturient.Id = _FirstWaveLast.AbiturientId
	    WHERE NOT EXISTS 
        (SELECT * FROM ed.hlpAbitToGo WHERE hlpAbitToGo.PersonId = Abiturient.PersonId AND hlpAbitToGo.IsGo = 1 
        AND hlpAbitToGo.Priority < Abiturient.Priority AND hlpAbitToGo.StudyLevelGroupId = {0})
	)
    OR Abiturient.CompetitionId IN (1, 2, 7, 8)
)", _studyLevelGroupId);
                    sFilter += " AND Abiturient.HasEntryConfirm = 1 AND Abiturient.HasDisabledEntryConfirm = 0 ";
                }
                else
                    sFilter += string.Format(@" AND
(
    Abiturient.Id IN
    ( 
	    SELECT Abiturient.Id FROM ed._FirstWaveGreen 
	    INNER JOIN ed.Abiturient ON Abiturient.Id = _FirstWaveGreen.AbiturientId
	    WHERE NOT EXISTS 
        (SELECT * FROM ed.hlpAbitToGoGreen WHERE hlpAbitToGoGreen.PersonId = Abiturient.PersonId AND hlpAbitToGoGreen.IsGo = 1 
        AND hlpAbitToGoGreen.Priority < Abiturient.Priority AND hlpAbitToGoGreen.LevelGroupId = {0})
	    UNION 
	    SELECT Abiturient.Id FROM ed._FirstWaveYellow 
	    INNER JOIN ed.Abiturient ON Abiturient.Id = _FirstWaveYellow.AbiturientId
	    WHERE NOT EXISTS 
        (SELECT * FROM ed.hlpAbitToGoGreen WHERE hlpAbitToGoGreen.PersonId = Abiturient.PersonId AND hlpAbitToGoGreen.IsGo = 1 
        AND hlpAbitToGoGreen.Priority < Abiturient.Priority AND hlpAbitToGoGreen.LevelGroupId = {0})
	    UNION 
	    SELECT Abiturient.Id FROM ed._FirstWaveLast
	    INNER JOIN ed.Abiturient ON Abiturient.Id = _FirstWaveLast.AbiturientId
	    WHERE NOT EXISTS 
        (SELECT * FROM ed.hlpAbitToGoGreen WHERE hlpAbitToGoGreen.PersonId = Abiturient.PersonId AND hlpAbitToGoGreen.IsGo = 1 
        AND hlpAbitToGoGreen.Priority < Abiturient.Priority AND hlpAbitToGoGreen.LevelGroupId = {0})
	)
    OR Abiturient.CompetitionId IN (1, 2, 7, 8)
)", _studyLevelGroupId, kc);

                string orderBy = " ORDER BY SortNum";

                DataTable dtProg = MainClass.Bdc.GetDataSet(sQueryBody + sQueryJoinFW + sFilter + orderBy).Tables[0];

                dtAbits.Merge(dtProg);
            }

            FillGrid(dgvRight, dtAbits);
        }

        void UpdateLeft()
        { 
            string ids = Util.BuildStringWithCollection(lstSelected[HeaderId]);

            dgvLeft.Rows.Clear();
            if (ids.Length > 0)
                FillGrid(dgvLeft, sQuery, GetTotalFilter() + string.Format(" AND qAbiturient.Id IN ({0}) ", ids), sOrderby);
            else
                InitGrid(dgvLeft);
        }

        protected override void OnMoved()
        {
            base.OnMoved();

            List<string> curList = lstSelected[HeaderId];
            curList.Clear();
            foreach (DataGridViewRow row in dgvLeft.Rows)
                curList.Add("'" + row.Cells["Id"].Value.ToString() + "'");
        }

        //сохранение
        protected override bool Save()
        {
            //проверка данных
            if (!CheckData())
                return false;            
            /*
            int total = GetTotalCount();

            int selected = 0;

            foreach (List<string> lst in lstSelected)
                selected += lst.Count;

            if (selected != total)
            {
                MessageBox.Show("Выберите формулировку для всех абитуриентов, имеющих право на зачисления, из зеленый зоны волны!", "Внимание");
                return false;
            }
            */
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        Guid protocolId;

                        ObjectParameter paramId = new ObjectParameter("id", typeof(Guid));
                        int iProtocolTypeId = ProtocolList.TypeToInt(_type);

                        context.Protocol_InsertAll(_studyLevelGroupId,
                                  _facultyId, _licenseProgramId, _studyFormId, _studyBasisId, tbNum.Text, dtpDate.Value, iProtocolTypeId,
                                  string.Empty, !isNew, null, _isSecond, _isReduced, _isParallel, _isListener, _isForeign, paramId);

                        protocolId = (Guid)paramId.Value;                        

                        foreach (int? key in lstSelected.Keys)
                        //for (int i=0; i<lstSelected.Count; i++)
                        {
                            List<string> lst = lstSelected[key];
                            foreach (string str in lst)
                            {                              
                                Guid abId = new Guid(str.Trim(new char[] { '\'' }));
                                context.ProtocolHistory_Insert(abId, protocolId, false, null, key, paramId);                                
                            }
                        }
                        scope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка при сохранении протокола: ", ex);
            }

            return true;
        }

        protected override bool CheckData()
        {
            bool bRes = base.CheckData();

            using (PriemEntities context = new PriemEntities())
            {
                var bEntryViewCreateEnabled = "True".Equals(context.C_AppSettings.Find("bEntryViewCreateEnabled").ParamValue);
                if (!bEntryViewCreateEnabled)
                {
                    MessageBox.Show("Создание представлений к зачислению в настоящий момент запрещено администратором");
                    return false;
                }
            }

            return bRes;
        }

        //предварительный просмотр
        protected override void Preview()
        {            
            try
            {
                WordDoc wd = new WordDoc(string.Format(@"{0}\EmptyTemplate.dot", MainClass.dirTemplates));

                int counter = 0;

                int lstCount = -1;
                int lstTableCount = 0;
                //foreach (List<string> lst in lstSelected.Values)
                foreach (int? key in lstSelected.Keys)
                {
                    List<string> lst = lstSelected[key];

                    lstCount++;
                    if (lst.Count == 0)
                        continue;

                    string header = MainClass.Bdc.GetStringValue("SELECT Name FROM ed.EntryHeader WHERE id=" + key);
                    wd.AddParagraph(string.Format("\r\n {0}",header));
                    wd.AddNewTable(lst.Count+1,6);
                    TableDoc td = wd.Tables[lstTableCount];
                    ++lstTableCount;
                    //заполняем таблицу в шаблоне

                    int r = 0;
                    
                    td[0, r] = "№ п/п";
                    td[1, r] = "Рег.номер";
                    td[2, r] = "ФАМИЛИЯ, ИМЯ, ОТЧЕСТВО";
                    td[3, r] = "Сумма баллов";
                    td[4, r] = "Направление или Направление (профиль или Профиль)";
                    td[5, r] = "Вид конкурса";

                    DataSet ds = MainClass.Bdc.GetDataSet(sQuery + string.Format(" WHERE ed.qAbiturient.Id IN ({0})", Util.BuildStringWithCollection(lst)) + sOrderby);
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        ++r;
                        ++counter;
                        td[0, r] = counter.ToString();
                        td[1, r] = row["Рег_Номер"].ToString();
                        td[2, r] = row["ФИО"].ToString();
                        td[3, r] = row["Sum"].ToString();
                        td[4, r] = row["Направление"].ToString();
                        td[5, r] = row["Конкурс"].ToString();                                                
                    }
                }
            }
            catch (Exception ex)
            {
                WinFormsServ.Error("Ошибка при выводе в Word протокола о допуске: ", ex);
            }
        }
   }
}