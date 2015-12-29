using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EducServLib;
using System.Xml;

namespace PriemLib
{
    public partial class StatFormGSGU : Form
    {
        public StatFormGSGU()
        {
            InitializeComponent();
            this.MdiParent = MainClass.mainform;
            FillCombos();
        }

        private int? StudyLevelId
        {
            get { return ComboServ.GetComboIdInt(cbStudyLevel); }
        }

        private void FillCombos()
        {
            using (PriemEntities context = new PriemEntities())
            {
                var src = context.StudyLevel.Where(x => x.LevelGroupId == 1 || x.LevelGroupId == 2).Select(x => new { x.Id, x.Name }).ToList().Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name)).ToList();

                ComboServ.FillCombo(cbStudyLevel, src, false, true);
            }
        }

        private void btnStartImport_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            var declaration = doc.CreateXmlDeclaration("1.0", "utf-8", "");
            doc.AppendChild(declaration);
            var rootNode = doc.AppendChild(doc.CreateNode(XmlNodeType.Element, "root", ""));
            XmlAttribute attr = doc.CreateAttribute("id");
            attr.Value = "2609";
            rootNode.Attributes.Append(attr);
            using (PriemEntities context = new PriemEntities())
            {
                int rowNum = 0;
                var ListLP = context.Entry
                    .Where(x => x.KCP.HasValue && x.KCP > 0 && x.StudyLevel.LevelGroupId != 3
                        && (StudyLevelId.HasValue ? x.StudyLevel.Id == StudyLevelId : true) && !x.IsForeign && !x.IsCrimea)
                    .Select(x => new { x.StudyFormId, x.StudyBasisId, x.LicenseProgramId, x.SP_LicenseProgram.GSGUCode, x.SP_LicenseProgram.Name, x.SP_LicenseProgram.Code, x.StudyLevel.LevelGroupId })
                    .Distinct().ToList();
                ProgressForm pf = new ProgressForm();
                pf.SetProgressText("Загрузка данных...");
                pf.MaxPrBarValue = ListLP.Count;
                pf.Show();
                try
                {
                    foreach (var LP in ListLP)
                    {
                        pf.PerformStep();
                        if (string.IsNullOrEmpty(LP.GSGUCode))
                        {
                            MessageBox.Show(LP.Code + " " + LP.Name + " - не указан код ГЗГУ");
                            continue;
                        }
                        rowNum++;

                        //номер строки
                        var rwNode = rootNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "lines", ""));
                        attr = doc.CreateAttribute("id");
                        attr.Value = rowNum.ToString();
                        rwNode.Attributes.Append(attr);

                        //ID организации или филиала, предоставляющего данные
                        var node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "oo", ""));
                        node.InnerText = "2609";

                        //ID специальности (по справочнику №2)
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "spec", ""));
                        node.InnerText = LP.GSGUCode;

                        //ID формы обучения (по справочнику №3)
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "fo", ""));
                        node.InnerText = LP.StudyFormId.ToString();

                        //ID формы финансирования (по справочнику №4)
                        string ff = "1";
                        if (LP.StudyBasisId == 2)
                            ff = "4";
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "ff", ""));
                        node.InnerText = ff;

                        //Срок окончания приёма заявлений
                        DateTime dtLast = context.Entry.Where(x => x.LicenseProgramId == LP.LicenseProgramId && x.StudyFormId == LP.StudyFormId && x.StudyBasisId == LP.StudyBasisId)
                            .Select(x => x.DateOfClose).Max();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "dat_opz", ""));
                        node.InnerText = dtLast.ToString("yyyy-MM-dd HH:mm:ss");

                        if (LP.LevelGroupId == 1 && LP.StudyBasisId == 1)
                            dtLast = new DateTime(2015, 8, 7, 23, 59, 59);
                        else if (LP.LevelGroupId == 1 && LP.StudyBasisId == 2)
                            dtLast = new DateTime(2015, 8, 21, 23, 59, 59);
                        else if (LP.LevelGroupId == 2 && LP.StudyFormId == 1)
                            dtLast = new DateTime(2015, 8, 31, 23, 59, 59);
                        else if (LP.LevelGroupId == 2 && LP.StudyFormId == 2)
                            dtLast = new DateTime(2015, 9, 28, 23, 59, 59);
                        else if (LP.LevelGroupId == 4)
                            dtLast = new DateTime(2015, 8, 11, 23, 59, 59);
                        else if (LP.LevelGroupId == 5)
                            dtLast = new DateTime(2015, 8, 21, 23, 59, 59);

                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "dat_ppz", ""));
                        node.InnerText = dtLast.ToString("yyyy-MM-dd HH:mm:ss");

                        //КЦП общие (без учёта Крыма)
                        int KCP = context.Entry.Where(x => x.LicenseProgramId == LP.LicenseProgramId && x.StudyFormId == LP.StudyFormId && x.StudyBasisId == LP.StudyBasisId && !x.IsCrimea && !x.IsForeign)
                            .Select(x => x.KCP).ToList().Select(x => x ?? 0).Sum();

                        //зачислено на крымские места
                        int enteredCrimea = context.extEntryView
                            .Where(x => x.LicenseProgramId == LP.LicenseProgramId && x.StudyFormId == LP.StudyFormId && x.StudyBasisId == LP.StudyBasisId && x.IsCrimea && !x.IsForeign)
                            .Count();

                        int KCPQuota = context.Entry.Where(x => x.LicenseProgramId == LP.LicenseProgramId && x.StudyFormId == LP.StudyFormId && x.StudyBasisId == LP.StudyBasisId && !x.IsCrimea && !x.IsForeign)
                            .Select(x => x.KCPQuota).ToList().Select(x => x ?? 0).Sum();

                        int KCPCel = context.Entry.Where(x => x.LicenseProgramId == LP.LicenseProgramId && x.StudyFormId == LP.StudyFormId && x.StudyBasisId == LP.StudyBasisId && !x.IsCrimea && !x.IsForeign)
                            .Select(x => x.KCPCel).ToList().Select(x => x ?? 0).Sum();
                        //Всего мест для приёма граждан
                        //1.4 Всего
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_4", ""));
                        node.InnerText = (KCP - enteredCrimea).ToString();
                        // 1.5 из них квотники
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_5", ""));
                        node.InnerText = KCPQuota.ToString();
                        // 1.6 из них целевики
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_6", ""));
                        node.InnerText = KCPCel.ToString();

                        //КЦП выделенные (Крым)
                        KCP = context.Entry.Where(x => x.LicenseProgramId == LP.LicenseProgramId && x.StudyFormId == LP.StudyFormId && x.StudyBasisId == LP.StudyBasisId && x.IsCrimea && !x.IsForeign)
                            .Select(x => x.KCP).ToList().Select(x => x ?? 0).Sum();

                        KCPQuota = context.Entry.Where(x => x.LicenseProgramId == LP.LicenseProgramId && x.StudyFormId == LP.StudyFormId && x.StudyBasisId == LP.StudyBasisId && x.IsCrimea && !x.IsForeign)
                            .Select(x => x.KCPQuota).ToList().Select(x => x ?? 0).Sum();

                        KCPCel = context.Entry.Where(x => x.LicenseProgramId == LP.LicenseProgramId && x.StudyFormId == LP.StudyFormId && x.StudyBasisId == LP.StudyBasisId && x.IsCrimea && !x.IsForeign)
                            .Select(x => x.KCPCel).ToList().Select(x => x ?? 0).Sum();
                        //Всего мест для приёма граждан
                        //1.7 Всего
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_7", ""));
                        node.InnerText = enteredCrimea.ToString();
                        // 1.8 из них квотники
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_8", ""));
                        node.InnerText = KCPQuota.ToString();
                        // 1.9 из них целевики
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_9", ""));
                        node.InnerText = KCPCel.ToString();

                        //Количество поданных заявлений
                        var AbitList = context.Abiturient
                            .Where(x => x.Entry.LicenseProgramId == LP.LicenseProgramId && x.Entry.StudyFormId == LP.StudyFormId && x.Entry.StudyBasisId == LP.StudyBasisId && !x.Entry.IsForeign)
                            .Select(x => new { x.Id, x.CompetitionId, x.Entry.IsCrimea, x.DocInsertDate }).ToList();

                        //количество поданных заявлений
                        //на выделенные места (Крым)
                        //для организаций, расположенных за пределами Крыма и Севастополя (на 14.07.2015)
                        //1.10 Всего
                        int CountAbit = AbitList.Where(x => x.DocInsertDate < new DateTime(2015, 7, 15)).Where(x => x.IsCrimea).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_10", ""));
                        node.InnerText = CountAbit.ToString();
                        //1.11 из них квотники
                        int CountAbit_VK = AbitList.Where(x => x.DocInsertDate < new DateTime(2015, 7, 15))
                            .Where(x => x.IsCrimea).Where(x => (x.CompetitionId == 2 || x.CompetitionId == 7)).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_11", ""));
                        node.InnerText = CountAbit_VK.ToString();
                        //1.12 из них целевики
                        int CountAbit_Cel = AbitList.Where(x => x.DocInsertDate < new DateTime(2015, 7, 15))
                            .Where(x => x.IsCrimea).Where(x => (x.CompetitionId == 6)).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_12", ""));
                        node.InnerText = CountAbit_Cel.ToString();
                        //для организаций, расположенных в пределах Крыма и Севастополя (на 09.08.2015)
                        //1.13 всего
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_13", ""));
                        node.InnerText = "0";
                        //1.14 квотники
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_14", ""));
                        node.InnerText = "0";
                        //1.15 целевики
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_15", ""));
                        node.InnerText = "0";

                        //на общие места (не Крым) на 24.07.2015
                        //1.16 всего
                        CountAbit = AbitList.Where(x => x.DocInsertDate < new DateTime(2015, 7, 25))
                            .Where(x => !x.IsCrimea).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_16", ""));
                        node.InnerText = CountAbit.ToString();
                        //1.17 квотники
                        CountAbit_VK = AbitList.Where(x => x.DocInsertDate < new DateTime(2015, 7, 25))
                            .Where(x => !x.IsCrimea).Where(x => (x.CompetitionId == 2 || x.CompetitionId == 7)).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_17", ""));
                        node.InnerText = CountAbit_VK.ToString();
                        //1.18 целевики
                        CountAbit_Cel = AbitList.Where(x => x.DocInsertDate < new DateTime(2015, 7, 25))
                            .Where(x => !x.IsCrimea).Where(x => (x.CompetitionId == 6)).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_18", ""));
                        node.InnerText = CountAbit_Cel.ToString();

                        //1.19 всего после 24.07.2015 для организаций, расположенных за пределами Крыма и Севастополя
                        int CountAbit_After2507 = AbitList.Where(x => x.DocInsertDate >= new DateTime(2015, 7, 25)).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_19", ""));
                        node.InnerText = CountAbit_After2507.ToString();
                        //1.20 всего после 09.08.2015 для организаций, расположенных в пределах Крыма и Севастополя
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_20", ""));
                        node.InnerText = "0";

                        //Раздел "Зачисленные"
                        //на общие места
                        var ExV = 
                            (from EV in context.extEntryView 
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId && EV.StudyBasisId == LP.StudyBasisId && !EV.IsCrimea
                             select new { Ab.Id, Ab.CompetitionId, EV.Date, EV.IsBE, EV.IsQuota, EV.IsCel, Ab.Entry.StudyLevel.LevelGroupId }).ToList();

                        DateTime dtZeroWaveFirst = new DateTime(2015, 7, 29);
                        DateTime dtZeroWaveLast = new DateTime(2015, 7, 30);

                        DateTime dtFirstWaveFirst = new DateTime(2015, 8, 3);
                        DateTime dtFirstWaveLast = new DateTime(2015, 8, 4);

                        DateTime dtSecondWaveFirst = new DateTime(2015, 8, 6);
                        DateTime dtSecondWaveLast = new DateTime(2015, 8, 8);

                        //1.21 зачисленных абитуриентов 30.07 - б/э
                        int cnt_BE = ExV.Where(x => x.Date <= dtZeroWaveLast && x.Date >= dtZeroWaveFirst && x.IsBE).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_21", ""));
                        node.InnerText = cnt_BE.ToString();

                        //1.22 зачисленных абитуриентов 30.07 - в/к
                        int cnt_VK = ExV.Where(x => x.Date <= dtZeroWaveLast && x.Date >= dtZeroWaveFirst && x.IsQuota).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_22", ""));
                        node.InnerText = cnt_VK.ToString();

                        //1.23 зачисленных абитуриентов 30.07 - цел
                        int cnt_Сel = ExV.Where(x => x.Date <= dtZeroWaveLast && x.Date >= dtZeroWaveFirst && x.IsCel).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_23", ""));
                        node.InnerText = cnt_Сel.ToString();

                        //1.24 зачисленных абитуриентов 1 волна
                        int cnt_FW = ExV.Where(x => x.Date <= dtFirstWaveLast && x.Date >= dtFirstWaveFirst).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_24", ""));
                        node.InnerText = cnt_FW.ToString();

                        //1.25 зачисленных абитуриентов 2 волна
                        int cnt_SecondWave = ExV.Where(x => x.Date <= dtSecondWaveLast && x.Date >= dtSecondWaveFirst).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_25", ""));
                        node.InnerText = cnt_SecondWave.ToString();

                        //1.26 зачисленных абитуриентов после 2 волны
                        int cnt_AfterSecondWave = ExV.Where(x => x.Date > dtSecondWaveLast).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_26", ""));
                        node.InnerText = cnt_AfterSecondWave.ToString();

                        //на выделенные места (Крым) для организаций, расположенных за пределами Крыма и Севастополя
                        var ExV_Crimea =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId && EV.StudyBasisId == LP.StudyBasisId && EV.IsCrimea
                             select new { Ab.Id, Ab.CompetitionId, EV.Date, EV.IsBE, EV.IsQuota, EV.IsCel, Ab.Entry.StudyLevel.LevelGroupId }).ToList();

                        DateTime dtZeroWaveFirst_Crimea = new DateTime(2015, 7, 16);
                        DateTime dtZeroWaveLast_Crimea = new DateTime(2015, 7, 17);

                        DateTime dtFirstWaveFirst_Crimea = new DateTime(2015, 7, 21);
                        DateTime dtFirstWaveLast_Crimea = new DateTime(2015, 7, 23);

                        DateTime dtSecondWaveFirst_Crimea = new DateTime(2015, 7, 25);
                        DateTime dtSecondWaveLast_Crimea = new DateTime(2015, 7, 28);

                        //1.27 зачисленных абитуриентов 17.07 - б/э
                        cnt_BE = ExV_Crimea.Where(x => x.Date <= dtZeroWaveLast_Crimea && x.Date >= dtZeroWaveFirst_Crimea && x.IsBE).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_27", ""));
                        node.InnerText = cnt_BE.ToString();

                        //1.28 зачисленных абитуриентов 17.07 - в/к
                        cnt_VK = ExV_Crimea.Where(x => x.Date <= dtZeroWaveLast_Crimea && x.Date >= dtZeroWaveFirst_Crimea && x.IsQuota).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_28", ""));
                        node.InnerText = cnt_VK.ToString();

                        //1.29 зачисленных абитуриентов 17.07 - цел
                        cnt_Сel = ExV_Crimea.Where(x => x.Date <= dtZeroWaveLast_Crimea && x.Date >= dtZeroWaveFirst_Crimea && x.IsCel).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_29", ""));
                        node.InnerText = cnt_Сel.ToString();

                        //1.30 зачисленных абитуриентов 1 волна
                        cnt_FW = ExV_Crimea.Where(x => x.Date <= dtFirstWaveLast_Crimea && x.Date >= dtFirstWaveFirst_Crimea).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_30", ""));
                        node.InnerText = cnt_FW.ToString();

                        //1.31 зачисленных абитуриентов 2 волна
                        cnt_SecondWave = ExV_Crimea.Where(x => x.Date >= dtSecondWaveFirst_Crimea).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_31", ""));
                        node.InnerText = cnt_SecondWave.ToString();

                        //на выделенные места (Крым) для организаций, расположенных в пределах Крыма и Севастополя
                        //1.32 
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_32", ""));
                        node.InnerText = "0";
                        //1.33 зачисленных абитуриентов 13.08 - б/э
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_33", ""));
                        node.InnerText = "0";
                        //1.34 зачисленных абитуриентов 13.08 - в/к
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_34", ""));
                        node.InnerText = "0";
                        //1.35 зачисленных абитуриентов 13.08 - цел
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_35", ""));
                        node.InnerText = "0";
                        //1.36 зачисленных абитуриентов 1 волна
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_36", ""));
                        node.InnerText = "0";
                        //1.37 зачисленных абитуриентов 2 волна
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_37", ""));
                        node.InnerText = "0";

                        //Количество зачисленных по категориям
                        //1.38 всего
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_38", ""));
                        node.InnerText = ExV.Count().ToString();
                        //без вступительных испытаний
                        //1.39 (чемпионы Олимпийских игр (всегда = 0 для СПбГУ))
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_39", ""));
                        node.InnerText = "0";
                        //1.40 Победители и призёры закл. этапа Всеросса, международных олимпиад
                        int cntOlympVseross =
                            (from exEV in context.extEntryView
                             join mrk in context.Olympiads on exEV.AbiturientId equals mrk.AbiturientId
                             where mrk.OlympTypeId <= 2 //всеросс и международные
                             && exEV.LicenseProgramId == LP.LicenseProgramId
                             && exEV.StudyFormId == LP.StudyFormId
                             && exEV.StudyBasisId == LP.StudyBasisId
                             && !exEV.IsCrimea && !exEV.IsCel && !exEV.IsQuota && exEV.IsBE
                             select exEV.AbiturientId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_40", ""));
                        node.InnerText = cntOlympVseross.ToString();

                        //1.41 Победители и призёры закл. этапа Всеукр олимпиад
                        int cntOlympVseukr =
                            (from exEV in context.extEntryView
                             join mrk in context.Olympiads on exEV.AbiturientId equals mrk.AbiturientId
                             where mrk.OlympTypeId == 8 //Всеукр
                             && exEV.LicenseProgramId == LP.LicenseProgramId
                             && exEV.StudyFormId == LP.StudyFormId
                             && exEV.StudyBasisId == LP.StudyBasisId
                             && !exEV.IsCrimea && !exEV.IsCel && !exEV.IsQuota && exEV.IsBE
                             select exEV.AbiturientId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_41", ""));
                        node.InnerText = cntOlympVseukr.ToString();

                        //1.42 Победители и призёры прочих олимпиад б/э
                        int cntBEComp = ExV.Where(x => !x.IsCel && !x.IsQuota && x.IsBE).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_42", ""));
                        node.InnerText = (cntBEComp - cntOlympVseross - cntOlympVseukr).ToString();

                        //в пределах установленных квот
                        int cntQuotaComp = ExV.Where(x => !x.IsCel && x.IsQuota && !x.IsBE).Count();
                        int cntCelComp = ExV.Where(x => x.IsCel && !x.IsQuota && !x.IsBE).Count();

                        //1.43 в/к
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_43", ""));
                        node.InnerText = cntQuotaComp.ToString();
                        
                        //1.44 цел
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_44", ""));
                        node.InnerText = cntCelComp.ToString();

                        //по общему конкурсу
                        int cntCommonComp = ExV.Where(x => !x.IsCel && !x.IsQuota && !x.IsBE).Count();
                        var cntOlympLike100Balls =
                            (from exEV in context.extEntryView
                             join mrk in context.Mark on exEV.AbiturientId equals mrk.AbiturientId
                             join Ol in context.Olympiads on mrk.OlympiadId equals Ol.Id
                             where mrk.IsFromOlymp
                             && exEV.LicenseProgramId == LP.LicenseProgramId
                             && exEV.StudyFormId == LP.StudyFormId
                             && exEV.StudyBasisId == LP.StudyBasisId
                             && !exEV.IsCrimea && !exEV.IsCel && !exEV.IsQuota && !exEV.IsBE
                             select new { exEV.AbiturientId, Ol.OlympTypeId }).ToList();

                        //1.45 всего зачисленных абитуриентов по общему конкурсу (без учёта получивших 100 баллов за олимпиаду)
                        int cnt_common_no_100_balls = cntCommonComp - cntOlympLike100Balls.Count;
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_45", ""));
                        node.InnerText = cnt_common_no_100_balls.ToString();

                        //1.46 всероссы-международники, получившие 100 баллов за олимпиаду
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_46", ""));
                        node.InnerText = cntOlympLike100Balls.Where(x => x.OlympTypeId == 1 || x.OlympTypeId == 2).Count().ToString();
                        //1.47 из них (1.46) не по профилю олимпиады
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_47", ""));
                        node.InnerText = "0";

                        //1.48 всеукры, получившие 100 баллов за олимпиаду
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_48", ""));
                        node.InnerText = cntOlympLike100Balls.Where(x => x.OlympTypeId == 8).Count().ToString();
                        //1.49 из них (1.48) не по профилю олимпиады
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_49", ""));
                        node.InnerText = "0";

                        //1.50 прочие олимпиады, получившие 100 баллов за олимпиаду
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_50", ""));
                        node.InnerText = cntOlympLike100Balls.Where(x => x.OlympTypeId == 3 || x.OlympTypeId == 4).Count().ToString();
                        //1.51 из них (1.50) не по профилю олимпиады
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_51", ""));
                        node.InnerText = "0";
                        //1.52 чемпионы и призёры Олимпийских игр (для общ конкурса, всегда 0)
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_52", ""));
                        node.InnerText = "0";

                        //1.53 на выделенные места (Крым)
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_53", ""));
                        node.InnerText = ExV_Crimea.Count().ToString();
                        //1.54 имеющих преимущ право (общ-преим, дог-преим) на 15.08.2015
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_54", ""));
                        node.InnerText = ExV.Where(x => x.CompetitionId == 9 || x.CompetitionId == 5).Where(x => x.Date <= new DateTime(2015, 8, 15)).Count().ToString();
                        //1.55 имеющих преимущ право (общ-преим, дог-преим) на 1.11.2015
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_55", ""));
                        node.InnerText = ExV.Where(x => x.CompetitionId == 9 || x.CompetitionId == 5).Where(x => x.Date <= new DateTime(2015, 11, 1)).Count().ToString();

                        List<int> AspOrd = new List<int>() { 4, 5 };
                        //раздел только для аспирантуры и ординатуры
                        //Количество мест для приёма граждан
                        KCP = context.Entry
                            .Where(x => x.LicenseProgramId == LP.LicenseProgramId && x.StudyFormId == LP.StudyFormId && x.StudyBasisId == LP.StudyBasisId && !x.IsCrimea && !x.IsForeign 
                                && AspOrd.Contains(x.StudyLevel.LevelGroupId))
                            .Select(x => x.KCP).ToList().Select(x => x ?? 0).Sum();
                        KCPCel = context.Entry
                            .Where(x => x.LicenseProgramId == LP.LicenseProgramId && x.StudyFormId == LP.StudyFormId && x.StudyBasisId == LP.StudyBasisId && !x.IsCrimea && !x.IsForeign 
                                && AspOrd.Contains(x.StudyLevel.LevelGroupId))
                            .Select(x => x.KCPCel).ToList().Select(x => x ?? 0).Sum();
                        //1.56 число мест
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_56", ""));
                        node.InnerText = KCP.ToString();
                        //1.57 в т.ч. целевики
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_57", ""));
                        node.InnerText = KCPCel.ToString();
                        //КЦП выделенные (Крым)
                        KCP = context.Entry
                            .Where(x => x.LicenseProgramId == LP.LicenseProgramId && x.StudyFormId == LP.StudyFormId && x.StudyBasisId == LP.StudyBasisId && x.IsCrimea && !x.IsForeign 
                                && AspOrd.Contains(x.StudyLevel.LevelGroupId))
                            .Select(x => x.KCP).ToList().Select(x => x ?? 0).Sum();
                        //1.58 число мест
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_58", ""));
                        node.InnerText = KCP.ToString();

                        //Количество поданых заявлений
                        var CntAbits = context.Abiturient.Where(x => AspOrd.Contains(x.Entry.StudyLevel.LevelGroupId)
                            && x.Entry.LicenseProgramId == LP.LicenseProgramId && x.Entry.StudyFormId == LP.StudyFormId && x.Entry.StudyBasisId == LP.StudyBasisId && !x.Entry.IsForeign)
                            .Select(x => new { x.Entry.IsCrimea, x.PersonId, x.CompetitionId }).Distinct().ToList();
                        //1.59 всего
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_59", ""));
                        node.InnerText = CntAbits.Count.ToString();
                        //1.60 целевиков
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_60", ""));
                        node.InnerText = CntAbits.Where(x => x.CompetitionId == 6).Count().ToString();
                        //1.61 Крым
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_61", ""));
                        node.InnerText = CntAbits.Where(x => x.IsCrimea).Count().ToString();

                        //Количество зачисленных
                        var CntAbitsEntered =
                            (from Ab in context.Abiturient
                             join extEntryV in context.extEntryView on Ab.Id equals extEntryV.AbiturientId
                             where AspOrd.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                             && Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             select new
                             {
                                 Ab.Entry.IsCrimea,
                                 Ab.PersonId,
                                 Ab.CompetitionId
                             }).Distinct().ToList();
                        //1.62 всего
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_62", ""));
                        node.InnerText = CntAbitsEntered.Count.ToString();
                        //1.63 целевиков
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_63", ""));
                        node.InnerText = CntAbitsEntered.Where(x => x.CompetitionId == 6).Count().ToString();
                        //1.64 Крым
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_64", ""));
                        node.InnerText = CntAbitsEntered.Where(x => x.IsCrimea).Count().ToString();

                        //1.65 проходной балл зачисленных (по сумме всех ВИ)
                        var ProhBall =
                            (from Ab in context.Abiturient
                             join extEntryV in context.extEntryView on Ab.Id equals extEntryV.AbiturientId
                             join Mrk in context.extAbitMarksSum on Ab.Id equals Mrk.Id
                             where AspOrd.Contains(Ab.Entry.StudyLevel.LevelGroupId)
                             && Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             select Mrk.TotalSum)
                             .ToList()
                             .Where(c => c.HasValue)
                             .DefaultIfEmpty(0m)
                             .Min();

                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p1_65", ""));
                        node.InnerText = ProhBall.ToString();

                        //часть 2. Сведения о среднем балле зачисленных (бак-спец)
                        //2.0 всего зачисленных
                        int Cnt1K = ExV.Where(x => x.LevelGroupId == 1).Count() + ExV_Crimea.Where(x => x.LevelGroupId == 1).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_0", ""));
                        node.InnerText = Cnt1K.ToString();

                        //2.1 зачисленных б/э
                        Cnt1K = ExV.Where(x => x.LevelGroupId == 1 && x.IsBE).Count() + ExV_Crimea.Where(x => x.LevelGroupId == 1 && x.IsBE).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_1", ""));
                        node.InnerText = Cnt1K.ToString();

                        //2.2 зачисленных в/к, поступающих по ЕГЭ
                        Cnt1K = ExV.Where(x => x.LevelGroupId == 1 && x.IsQuota).Count() + ExV_Crimea.Where(x => x.LevelGroupId == 1 && x.IsQuota).Count();
                        int Cnt1K_NoEge =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.Mark on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId && !Mrk.IsFromEge && !Mrk.IsFromOlymp
                             && EV.IsQuota
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_2", ""));
                        node.InnerText = (Cnt1K - Cnt1K_NoEge).ToString();

                        //2.3 по результатам ВИ, проводимых самостоятельно
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_3", ""));
                        node.InnerText = Cnt1K_NoEge.ToString();

                        //2.4 целевиков по ЕГЭ
                        Cnt1K = ExV.Where(x => x.LevelGroupId == 1 && x.IsCel).Count() + ExV_Crimea.Where(x => x.LevelGroupId == 1 && x.IsCel).Count();
                        Cnt1K_NoEge =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.Mark on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId && !Mrk.IsFromEge && !Mrk.IsFromOlymp
                             && EV.IsCel
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_4", ""));
                        node.InnerText = (Cnt1K - Cnt1K_NoEge).ToString();

                        //2.5 целевиков без ЕГЭ
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_5", ""));
                        node.InnerText = Cnt1K_NoEge.ToString();

                        //по общему конкурсу
                        //2.6 всего
                        Cnt1K = ExV.Where(x => x.LevelGroupId == 1 && !x.IsCel && !x.IsBE && !x.IsQuota).Count() 
                            + ExV_Crimea.Where(x => x.LevelGroupId == 1 && !x.IsCel && !x.IsBE && !x.IsQuota).Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_6", ""));
                        node.InnerText = Cnt1K.ToString();

                        int Cnt1K_NoEgeNoOlymp =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.Mark on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId && !Mrk.IsFromEge && !Mrk.IsFromOlymp
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota && !Mrk.ExamInEntryBlockUnit.Exam.IsAdditional
                             select Ab.PersonId).Distinct().Count();

                        int Cnt1K_NoEgeOlymp =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.Mark on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId && !Mrk.IsFromEge && Mrk.IsFromOlymp
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota && !Mrk.ExamInEntryBlockUnit.Exam.IsAdditional
                             select Ab.PersonId).Distinct().Count();

                        int Cnt1K_Tvor =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.Mark on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId && !Mrk.IsFromEge && !Mrk.IsFromOlymp
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota && Mrk.ExamInEntryBlockUnit.Exam.IsAdditional
                             select Ab.PersonId).Distinct().Count();

                        //2.7 по результатам ЕГЭ (нет олимпиад, нет самостоятельных ВИ)
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_7", ""));
                        node.InnerText = (Cnt1K - Cnt1K_NoEgeNoOlymp - Cnt1K_NoEgeOlymp).ToString();
                        //2.8 победители и призёры олимпиад
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_8", ""));
                        node.InnerText = Cnt1K_NoEgeOlymp.ToString();
                        //2.9 по результатам ВИ, проводимых самостоятельно
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_9", ""));
                        node.InnerText = Cnt1K_NoEgeNoOlymp.ToString();
                        //2.10 по результатам ВИ, проводимых самостоятельно
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_10", ""));
                        node.InnerText = Cnt1K_Tvor.ToString();

                        //Средний балл ЕГЭ зачисленных
                        //2.11 для в/к без учёта доп
                        var AvgBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.Mark on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && EV.IsQuota && !Mrk.ExamInEntryBlockUnit.Exam.IsAdditional
                             && !Ab.Entry.IsCrimea
                             select Mrk.Value).DefaultIfEmpty(0m).Average();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_11", ""));
                        node.InnerText = AvgBall.ToString();

                        //2.12 для цел без учёта доп
                        AvgBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.Mark on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && EV.IsCel && !EV.IsBE && !EV.IsQuota && !Mrk.ExamInEntryBlockUnit.Exam.IsAdditional
                             && !Ab.Entry.IsCrimea
                             select Mrk.Value).DefaultIfEmpty(0m).Average();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_12", ""));
                        node.InnerText = AvgBall.ToString();

                        //2.13 для общ (без учёта доп и олимпиад)
                        AvgBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.Mark on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId && !Mrk.IsFromOlymp
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota && !Mrk.ExamInEntryBlockUnit.Exam.IsAdditional
                             && !Ab.Entry.IsCrimea
                             select Mrk.Value).DefaultIfEmpty(0m).Average();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_13", ""));
                        node.InnerText = AvgBall.ToString();

                        //2.14 для общ (без учёта доп но с олимпиадами)
                        AvgBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.Mark on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota && !Mrk.ExamInEntryBlockUnit.Exam.IsAdditional
                             && !Ab.Entry.IsCrimea
                             select Mrk.Value).DefaultIfEmpty(0m).Average();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_14", ""));
                        node.InnerText = AvgBall.ToString();

                        //2.15 для общ (с учётом доп и ьез олимпиад)
                        AvgBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.Mark on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId && !Mrk.IsFromOlymp
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota 
                             && !Ab.Entry.IsCrimea
                             select Mrk.Value).DefaultIfEmpty(0m).Average();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_15", ""));
                        node.InnerText = AvgBall.ToString();

                        //2.16 для общ (без учёта доп но с олимпиадами)
                        AvgBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.Mark on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             select Mrk.Value).DefaultIfEmpty(0m).Average();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_16", ""));
                        node.InnerText = AvgBall.ToString();

                        //2.17 проходной балл, приведённый к 100-балльной шкале
                        AvgBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.extAbitMarksSum on Ab.Id equals Mrk.Id
                             join MrkAdd in context.extAbitAdditionalMarksSum on Ab.Id equals MrkAdd.AbiturientId into MrkAdd2
                             from MrkAdd in MrkAdd2.DefaultIfEmpty()
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             select new { Mrk.Id, Mrk.TotalSum, Mrk.TotalCount, MrkAdd.AdditionalMarksSum })
                             .ToList()
                             .Where(x => x.TotalCount != 0 && x.TotalCount.HasValue)
                             .Select(x => new { x.Id, TotalMark = (x.TotalSum ?? 0m) + ((decimal?)x.AdditionalMarksSum ?? 0m), x.TotalCount})
                             .Select(x => x.TotalMark / x.TotalCount.Value)
                             .DefaultIfEmpty(0m)
                             .Min();

                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_17", ""));
                        node.InnerText = AvgBall.ToString();

                        //Сведения об учёте индивидуальных достижений
                        //Количество начисляемых баллов
                        var Balls = context.AchievementType.Select(x => new { x.Id, x.Mark }).ToList();
                        //2.18 Олимпиады, первенства мира и Европы, ГТО и т.п.
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_18", ""));
                        node.InnerText = Balls.Where(x => x.Id <= 8).Select(x => x.Mark).DefaultIfEmpty(0).Max().ToString();
                        //2.19 Аттестат с отличием
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_19", ""));
                        node.InnerText = Balls.Where(x => x.Id == 9).Select(x => x.Mark).DefaultIfEmpty(0).Max().ToString();
                        //2.20 волонтёр
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_20", ""));
                        node.InnerText = Balls.Where(x => x.Id == 10).Select(x => x.Mark).DefaultIfEmpty(0).Max().ToString();
                        //2.21 олимпиады и др.конкурсы
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_21", ""));
                        node.InnerText = 4.ToString();
                        //2.22 итоговое сочинение
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_22", ""));
                        node.InnerText = Balls.Where(x => x.Id == 12).Select(x => x.Mark).DefaultIfEmpty(0).Max().ToString();
                        
                        //средний начисленный балл
                        //2.23 Олимпиады, первенства мира и Европы, ГТО и т.п.
                        double AvgAddBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.extAbitAllAdditionalAchievements on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             && Mrk.AchievementTypeId <= 8
                             select Mrk.Mark)
                             .ToList()
                             .Select(x => x ?? 0)
                             .DefaultIfEmpty(0)
                             .Average();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_23", ""));
                        node.InnerText = AvgBall.ToString();
                        //2.24 Аттестат с отличием
                        AvgAddBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.extAbitAllAdditionalAchievements on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             && Mrk.AchievementTypeId == 9
                             select Mrk.Mark)
                             .ToList()
                             .Select(x => x ?? 0)
                             .DefaultIfEmpty(0)
                             .Average();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_24", ""));
                        node.InnerText = AvgBall.ToString();
                        //2.25 волонтёр
                        AvgAddBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.extAbitAllAdditionalAchievements on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             && Mrk.AchievementTypeId == 10
                             select Mrk.Mark)
                             .ToList()
                             .Select(x => x ?? 0)
                             .DefaultIfEmpty(0)
                             .Average();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_25", ""));
                        node.InnerText = AvgBall.ToString();

                        //2.26 олимпиады и др.конкурсы
                        AvgAddBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.extAbitAllAdditionalAchievements on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             && Mrk.AchievementTypeId == 11
                             select Mrk.Mark)
                             .ToList()
                             .Select(x => x ?? 0)
                             .DefaultIfEmpty(0)
                             .Average();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_26", ""));
                        node.InnerText = AvgBall.ToString();

                        //2.27 итоговое сочинение
                        AvgAddBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.extAbitAllAdditionalAchievements on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             && Mrk.AchievementTypeId == 12
                             select Mrk.Mark)
                             .ToList()
                             .Select(x => x ?? 0)
                             .DefaultIfEmpty(0)
                             .Average();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_27", ""));
                        node.InnerText = AvgBall.ToString();

                        //количество лиц, поступивших с учётом результатов начисленных баллов
                        //2.28 Олимпиады, первенства мира и Европы, ГТО и т.п.
                        int CntAddBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.extAbitAllAdditionalAchievements on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             && Mrk.AchievementTypeId <= 8
                             select Mrk.Mark)
                             .ToList()
                             .Where(x => x.HasValue && x.Value > 0)
                             .Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_28", ""));
                        node.InnerText = AvgBall.ToString();
                        //2.29 Аттестат с отличием
                        CntAddBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.extAbitAllAdditionalAchievements on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             && Mrk.AchievementTypeId == 9
                             select Mrk.Mark)
                             .ToList()
                             .Where(x => x.HasValue && x.Value > 0)
                             .Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_29", ""));
                        node.InnerText = CntAddBall.ToString();
                        //2.30 волонтёр
                        CntAddBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.extAbitAllAdditionalAchievements on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             && Mrk.AchievementTypeId == 10
                             select Mrk.Mark)
                             .ToList()
                             .Where(x => x.HasValue && x.Value > 0)
                             .Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_30", ""));
                        node.InnerText = CntAddBall.ToString();

                        //2.31 олимпиады и др.конкурсы
                        CntAddBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.extAbitAllAdditionalAchievements on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             && Mrk.AchievementTypeId == 11
                             select Mrk.Mark)
                             .ToList()
                             .Where(x => x.HasValue && x.Value > 0)
                             .Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_31", ""));
                        node.InnerText = CntAddBall.ToString();

                        //2.32 итоговое сочинение
                        CntAddBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.extAbitAllAdditionalAchievements on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             && Mrk.AchievementTypeId == 12
                             select Mrk.Mark)
                             .ToList()
                             .Where(x => x.HasValue && x.Value > 0)
                             .Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p2_32", ""));
                        node.InnerText = CntAddBall.ToString();


                        //Раздел 3. Сведения о приёме лиц из числа отдельных категорий граждан
                        //количество поданных заявлений
                        //от инвалидов и лиц с огр. возможностями
                        //3.1 с нарушениями зрения
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_1", ""));
                        node.InnerText = "0";
                        //3.2 с нарушениями слуха и речи
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_2", ""));
                        node.InnerText = "0";
                        //3.3 с нарушениями опорно-двиг аппарата
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_3", ""));
                        node.InnerText = "0";
                        //3.4 с соматическими нарушениями
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_4", ""));
                        node.InnerText = "0";
                        //3.5 с не указанными нарушениями
                        int CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && (((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1)
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_5", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //3.6 детей-сирот
                        CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_6", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //3.7 на конкурсы б/э
                        CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && (Ab.CompetitionId == 1 || Ab.CompetitionId == 8)
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1)
                             || (Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_7", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //3.8 на конкурсы в/к
                        CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && (Ab.CompetitionId == 2 || Ab.CompetitionId == 7)
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1)
                             || (Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_8", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //3.9 на конкурсы цел
                        CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && (Ab.CompetitionId == 6)
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1)
                             || (Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_9", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //количество зачисленных
                        //из числа инвалидов и огр.возм. здоровья
                        //3.10 всего
                        CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_10", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //3.11 б/э
                        CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && (Ab.CompetitionId == 1 || Ab.CompetitionId == 8)
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_11", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //3.12 в/к
                        CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && (Ab.CompetitionId == 2 || Ab.CompetitionId == 7)
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_12", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //3.13 цел
                        CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && (Ab.CompetitionId == 6)
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_13", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //3.14 общ
                        CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && (Ab.CompetitionId == 3 || Ab.CompetitionId == 4 || Ab.CompetitionId == 5)
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_14", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //3.15 с наруш. зрения
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_15", ""));
                        node.InnerText = "0";
                        //3.16 с наруш слуха и речи
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_16", ""));
                        node.InnerText = "0";
                        //3.17 с наруш опорно-двиг аппарата
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_17", ""));
                        node.InnerText = "0";
                        //3.18 с соматич нарушениями
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_18", ""));
                        node.InnerText = "0";
                        //3.19 нарушения не указаны
                        CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_19", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //из числа детей-сирот
                        //3.20 всего
                        CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_20", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //3.21 б/э
                        CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && (Ab.CompetitionId == 1 || Ab.CompetitionId == 8)
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_21", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //3.22 в/к
                        CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && (Ab.CompetitionId == 2 || Ab.CompetitionId == 7)
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_22", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //3.23 цел
                        CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && (Ab.CompetitionId == 6)
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_23", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //3.24 общ
                        CntAbitsInvalid =
                            (from Ab in context.Abiturient
                             join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                             where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                             && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                             && (Ab.CompetitionId == 3 || Ab.CompetitionId == 4 || Ab.CompetitionId == 5)
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                             select Ab.PersonId).Distinct().Count();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_24", ""));
                        node.InnerText = CntAbitsInvalid.ToString();

                        //Средний балл зачисленных
                        //инвалидов и лиц с огр.возм.здоровья
                        //3.25 в/к
                        AvgBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.Mark on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1))
                             select Mrk.Value).DefaultIfEmpty(0m).Average();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_25", ""));
                        node.InnerText = AvgBall.ToString();
                        //3.26 общ
                        AvgBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.Mark on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1))
                             select Mrk.Value).DefaultIfEmpty(0m).Average();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_26", ""));
                        node.InnerText = AvgBall.ToString();
                        //детей-сирот
                        //3.27 в/к
                        AvgBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.Mark on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                             select Mrk.Value).DefaultIfEmpty(0m).Average();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_27", ""));
                        node.InnerText = AvgBall.ToString();
                        //3.28 общ
                        AvgBall =
                            (from EV in context.extEntryView
                             join Ab in context.Abiturient on EV.AbiturientId equals Ab.Id
                             join Mrk in context.Mark on Ab.Id equals Mrk.AbiturientId
                             where EV.LicenseProgramId == LP.LicenseProgramId && EV.StudyFormId == LP.StudyFormId
                             && EV.StudyBasisId == LP.StudyBasisId
                             && !EV.IsCel && !EV.IsBE && !EV.IsQuota
                             && !Ab.Entry.IsCrimea
                             && ((Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                             || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                             select Mrk.Value).DefaultIfEmpty(0m).Average();
                        node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p3_28", ""));
                        node.InnerText = AvgBall.ToString();


                        //Раздел 4. Сведения об успеваемости лиц из числа отдельных категорий граждан
                        for (int sem = 1; sem <= 2; sem++)
                        {
                            //количество студентов, зачисленных из числа:
                            //инвалиды и лица с огр.возм.здоровья
                            //4.2 всего
                            CntAbitsInvalid = 0;
                            if (sem == 1)
                            {
                                CntAbitsInvalid =
                                    (from Ab in context.Abiturient
                                     join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                                     where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                                     && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                                     && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                                     || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1))
                                     select Ab.PersonId).Distinct().Count();
                            }
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_2_" + sem, ""));
                            node.InnerText = CntAbitsInvalid.ToString();

                            //4.3 б/э
                            CntAbitsInvalid = 0;
                            if (sem == 1)
                            {
                                CntAbitsInvalid =
                                    (from Ab in context.Abiturient
                                     join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                                     where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                                     && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                                     && (Ab.CompetitionId == 1 || Ab.CompetitionId == 8)
                                     && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                                     || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1))
                                     select Ab.PersonId).Distinct().Count();
                            }
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_3_" + sem, ""));
                            node.InnerText = CntAbitsInvalid.ToString();

                            //4.4 в/к
                            CntAbitsInvalid = 0;
                            if (sem == 1)
                            {
                                CntAbitsInvalid =
                                    (from Ab in context.Abiturient
                                     join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                                     where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                                     && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                                     && (Ab.CompetitionId == 2 || Ab.CompetitionId == 7)
                                     && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                                     || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1))
                                     select Ab.PersonId).Distinct().Count();
                            }
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_4_" + sem, ""));
                            node.InnerText = CntAbitsInvalid.ToString();

                            //4.5 цел
                            CntAbitsInvalid = 0;
                            if (sem == 1)
                            {
                                CntAbitsInvalid =
                                    (from Ab in context.Abiturient
                                     join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                                     where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                                     && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                                     && (Ab.CompetitionId == 6)
                                     && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                                     || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1))
                                     select Ab.PersonId).Distinct().Count();
                            }
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_5_" + sem, ""));
                            node.InnerText = CntAbitsInvalid.ToString();

                            //4.6 общ
                            CntAbitsInvalid = 0;
                            if (sem == 1)
                            {
                                CntAbitsInvalid =
                                    (from Ab in context.Abiturient
                                     join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                                     where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                                     && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                                     && !ex.IsCel && !ex.IsBE && !ex.IsQuota
                                     && ((Ab.Person.Person_AdditionalInfo.Privileges & 32) == 1
                                     || ((Ab.Person.Person_AdditionalInfo.Privileges & 512) == 1))
                                     select Ab.PersonId).Distinct().Count();
                            }
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_6_" + sem, ""));
                            node.InnerText = CntAbitsInvalid.ToString();


                            //дети-сироты
                            //4.7 всего
                            CntAbitsInvalid = 0;
                            if (sem == 1)
                            {
                                CntAbitsInvalid =
                                    (from Ab in context.Abiturient
                                     join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                                     where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                                     && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                                     && ((Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                                     || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                                     select Ab.PersonId).Distinct().Count();
                            }
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_7_" + sem, ""));
                            node.InnerText = CntAbitsInvalid.ToString();

                            //4.8 б/э
                            CntAbitsInvalid = 0;
                            if (sem == 1)
                            {
                                CntAbitsInvalid =
                                    (from Ab in context.Abiturient
                                     join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                                     where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                                     && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                                     && (Ab.CompetitionId == 1 || Ab.CompetitionId == 8)
                                     && ((Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                                     || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                                     select Ab.PersonId).Distinct().Count();
                            }
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_8_" + sem, ""));
                            node.InnerText = CntAbitsInvalid.ToString();

                            //4.9 в/к
                            CntAbitsInvalid = 0;
                            if (sem == 1)
                            {
                                CntAbitsInvalid =
                                    (from Ab in context.Abiturient
                                     join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                                     where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                                     && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                                     && (Ab.CompetitionId == 2 || Ab.CompetitionId == 7)
                                     && ((Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                                     || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                                     select Ab.PersonId).Distinct().Count();
                            }
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_9_" + sem, ""));
                            node.InnerText = CntAbitsInvalid.ToString();

                            //4.10 цел
                            CntAbitsInvalid = 0;
                            if (sem == 1)
                            {
                                CntAbitsInvalid =
                                    (from Ab in context.Abiturient
                                     join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                                     where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                                     && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                                     && (Ab.CompetitionId == 6)
                                     && ((Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                                     || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                                     select Ab.PersonId).Distinct().Count();
                            }
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_10_" + sem, ""));
                            node.InnerText = CntAbitsInvalid.ToString();

                            //4.11 общ
                            CntAbitsInvalid = 0;
                            if (sem == 1)
                            {
                                CntAbitsInvalid =
                                    (from Ab in context.Abiturient
                                     join ex in context.extEntryView on Ab.Id equals ex.AbiturientId
                                     where Ab.Entry.LicenseProgramId == LP.LicenseProgramId && Ab.Entry.StudyFormId == LP.StudyFormId
                                     && Ab.Entry.StudyBasisId == LP.StudyBasisId && !Ab.Entry.IsForeign
                                     && !ex.IsCel && !ex.IsBE && !ex.IsQuota
                                     && ((Ab.Person.Person_AdditionalInfo.Privileges & 1) == 1
                                     || ((Ab.Person.Person_AdditionalInfo.Privileges & 256) == 1))
                                     select Ab.PersonId).Distinct().Count();
                            }
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_11_" + sem, ""));
                            node.InnerText = CntAbitsInvalid.ToString();

                            //средний балл по итогам пром.аттестации студентов
                            //инвалиды и лица с огр.возм.здоровья
                            //4.12 общий
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_12_" + sem, ""));
                            node.InnerText = "0";
                            //4.13 зачисленных в/к
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_13_" + sem, ""));
                            node.InnerText = "0";
                            //4.14 зачисленных общ
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_14_" + sem, ""));
                            node.InnerText = "0";

                            //дети-сироты
                            //4.15 общий
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_15_" + sem, ""));
                            node.InnerText = "0";
                            //4.16 зачисленных в/к
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_16_" + sem, ""));
                            node.InnerText = "0";
                            //4.17 зачисленных общ
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_17_" + sem, ""));
                            node.InnerText = "0";

                            //иных категорий обучающихся
                            //4.18
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_18_" + sem, ""));
                            node.InnerText = "0";

                            //количество студентов, отчисленных по результатам пром.атт
                            //инвалиды и лица с огр.возм.здоровья
                            //4.19 всего
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_19_" + sem, ""));
                            node.InnerText = "0";
                            //4.20 зачисленных в/к
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_20_" + sem, ""));
                            node.InnerText = "0";
                            //4.21 зачисленных общ
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_21_" + sem, ""));
                            node.InnerText = "0";

                            //дети-сироты
                            //4.22 всего
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_22_" + sem, ""));
                            node.InnerText = "0";
                            //4.23 зачисленных в/к
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_23_" + sem, ""));
                            node.InnerText = "0";
                            //4.24 зачисленных общ
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_24_" + sem, ""));
                            node.InnerText = "0";

                            //4.25 % отчисления от количества зачисленных в отчётном году из числа всех категорий студентов (в среднем по университету)
                            node = rwNode.AppendChild(doc.CreateNode(XmlNodeType.Element, "p4_25_" + sem, ""));
                            node.InnerText = "0";
                        }
                    }

                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "XML files|*.xml";
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        XmlWriter w = XmlWriter.Create(sfd.FileName, new XmlWriterSettings() { NewLineHandling = NewLineHandling.Entitize, NewLineChars = "" });
                        doc.Save(w);
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    WinFormsServ.Error(ex);
                }
                finally
                {
                    pf.Close();
                }
                

                //retString = declaration.OuterXml + doc.InnerXml;
            }
        }
    }
}
