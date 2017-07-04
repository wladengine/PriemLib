using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Entity.Core.Objects;
using System.Transactions;

using EducServLib;

namespace PriemLib
{
    public partial class CardExamInEntryBlock : BookCard
    {
        #region Fields
        protected Guid? gId
        {
            get
            {
                if (_Id == null)
                    return null;
                else
                    return Guid.Parse(_Id);
            }
        }
        protected bool IsProfil
        {
            get { return chbIsProfil.Checked; }
            set { chbIsProfil.Checked = value; }
        }
        protected bool IsNotInTotalSum
        {
            get { return chbIsNotInTotalSum.Checked; }
            set { chbIsNotInTotalSum.Checked = value; }
        }

        protected string BlockName
        {
            get { return tbBlockName.Text.Trim(); }
            set { tbBlockName.Text = value.Trim(); }
        }
        
        protected byte? OrderNumber
        {
            get
            {
                byte j;
                if (byte.TryParse(tbOrderNumber.Text.Trim(), out j))
                    return j;
                else
                    return null;
            }
            set { tbOrderNumber.Text = value.ToString(); }
        }

        protected Guid? ParentExamInEntryId
        {
            get { return ComboServ.GetComboIdGuid(cbParentExamInEntry); }
            set { ComboServ.SetComboId(cbParentExamInEntry, value); }
        }
        protected List<ExamenBlockUnit> lstUnit;
        protected Action<ExamenBlockUnit> _hndl;

        protected int? OlympTypeId
        {
            get { return ComboServ.GetComboIdInt(cbOlympType); }
            set { ComboServ.SetComboId(cbOlympType, value); }
        }
        protected int? OlympLevelId
        {
            get { return ComboServ.GetComboIdInt(cbOlympLevel); }
            set { ComboServ.SetComboId(cbOlympLevel, value); }
        }
        protected int? OlympYear
        {
            get { return ComboServ.GetComboIdInt(cbOlympYear); }
            set { ComboServ.SetComboId(cbOlympYear, value); }
        }
        #endregion

        private bool _isForModified;
        private Guid? _entryId;
        private DataTable _tblSource;

        public CardExamInEntryBlock(Guid? entryId, string id, bool isForModified)
            : base(id)
        {
            InitializeComponent();
            _entryId = entryId;
            _isForModified = isForModified;
            InitControls();
        }

        protected override void ExtraInit()
        {
            base.ExtraInit();
            _title = "Экзамен";
            _tableName = "ed.[ExamInEntryBlock]";

            try
            {
                if (_Id != null)
                    chbToAllStudyBasis.Visible = false;
                chbToAllStudyBasis.Checked = true;

                using (PriemEntities context = new PriemEntities())
                {
                    lbExams.DisplayMember = "Value";
                    lbExams.ValueMember = "Key";

                    List<KeyValuePair<string, string>> lst =
                        ((from bl in context.ExamInEntryBlock 
                          join unit in context.ExamInEntryBlockUnit on bl.Id equals unit.ExamInEntryBlockId
                          join f in context.Exam on unit.ExamId equals f.Id
                          join en in context.ExamName on f.ExamNameId equals en.Id
                          where bl.Id == gId
                          select new
                          {
                              Id = unit.Id,
                              Name = en.Name,
                              unit.EgeMin,
                          }).Distinct()).ToList()
                          .Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name + (u.EgeMin.HasValue? " ("+u.EgeMin.Value.ToString()+")":"")))
                          .OrderBy(x => x.Value)
                          .ToList(); 

                    if (lst.Count == 0)
                    {
                        this.Shown += CardExamInEntryBlock_Shown;
                    }
                    else
                        foreach (var x in lst)
                            lbExams.Items.Add(x);

                    lst = ((from ex in context.ExamInEntryBlock
                            where ex.EntryId == _entryId
                            select new
                            {
                                Id = ex.Id,
                                Name = ex.Name,
                                Ord = ex.OrderNumber
                            }).Distinct()).ToList()
                          .OrderBy(x => x.Ord)
                          .ThenBy(x => x.Name)
                          .Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name))
                          .ToList();
                    ComboServ.FillCombo(cbParentExamInEntry, lst, true, false);

                    lst = context.OlympLevel.Select(x => new { x.Id, x.Name })
                        .ToList()
                        .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name))
                        .ToList();
                    ComboServ.FillCombo(cbOlympLevel, lst, false, true);

                    lst = context.OlympType.Select(x => new { x.Id, x.Name })
                        .ToList()
                        .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name))
                        .ToList();
                    ComboServ.FillCombo(cbOlympType, lst, false, true);

                    lst = context.OlympBook.Select(x => new { Id = x.OlympYear, Name = x.OlympYear })
                        .Distinct()
                        .ToList()
                        .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name.ToString()))
                        .ToList();
                    ComboServ.FillCombo(cbOlympYear, lst, false, true);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы ", exc);
            }
        }
        protected override void InitHandlers()
        {
            cbOlympType.SelectedIndexChanged += (a, b) => { UpdateGridOlymps(); };
            cbOlympLevel.SelectedIndexChanged += (a, b) => { UpdateGridOlymps(); };
            cbOlympYear.SelectedIndexChanged += (a, b) => { UpdateGridOlymps(); };
            tbOlympName.TextChanged += (a, b) => { ApplyFiltersToGridOlymp(); };
            tbOlympSubject.TextChanged += (a, b) => { ApplyFiltersToGridOlymp(); };
        }

        void CardExamInEntryBlock_Shown(object sender, EventArgs e)
        {
            new CardExamInEntryUnit(null, new UnitListUpdateHandler(UnitListAdd)).Show();
        }
        protected override void SetAllFieldsEnabled()
        {
            base.SetAllFieldsEnabled();
        }
        protected override void SetAllFieldsNotEnabled()
        {
            base.SetAllFieldsNotEnabled();
            tc.Enabled = true;
        }
        protected override void SetReadOnlyFieldsAfterFill()
        {
            base.SetReadOnlyFieldsAfterFill();

            if (!MainClass.IsEntryChanger())
                btnSaveChange.Enabled = false;
            btnTimeTable.Enabled = true;
        }


        protected override void FillCard()
        {
            if (_Id == null)
            {
                lstUnit = new List<ExamenBlockUnit>();
                return;
            }
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    ExamInEntryBlock ent =
                        (from ex in context.ExamInEntryBlock
                         where ex.Id == gId
                         select ex).FirstOrDefault();

                    BlockName = ent.Name;
                    IsProfil = ent.IsProfil;
                    IsNotInTotalSum = ent.IsNotInTotalSum;
                    OrderNumber = ent.OrderNumber;
                    ParentExamInEntryId = ent.ParentExamInEntryBlockId;

                    var lst = (from ex_unit in context.ExamInEntryBlockUnit
                               join ex in context.Exam on ex_unit.ExamId equals ex.Id
                               join ex_name in context.ExamName on ex.ExamNameId equals ex_name.Id
                               where ex_unit.ExamInEntryBlockId == gId
                               select new ExamenBlockUnit
                               {
                                   ExamId = ex_unit.ExamId,
                                   UnitId = ex_unit.Id,
                                   EgeMin = ex_unit.EgeMin,
                                   ExamUnitName = ex_name.Name,
                               }).ToList();
                    lstUnit = lst;

                    UpdateGridOlymps();
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при заполнении формы ", exc);
            }
        }

        private void UpdateGridOlymps()
        {
            var obj = new {
                OlympTypeId = OlympTypeId,
                OlympLevelId = OlympLevelId,
                OlympYear = OlympYear
            };

            GridAsync.UpdateGridAsync(dgvOlymps, (sender, e) =>
            {
                using (PriemEntities context = new PriemEntities())
                {
                    dynamic arg = e.Argument;
                    int? _OlympTypeId = arg.OlympTypeId;
                    int? _OlympLevelId = arg.OlympLevelId;
                    int? _OlympYear = arg.OlympYear;
                    var data =
                        (from Res in context.OlympResultToMaxMark
                         join OL in context.extOlympBook on Res.OlympBookId equals OL.Id
                         join OlympVal in context.OlympValue on Res.OlympValueId equals OlympVal.Id
                         where Res.ExamInEntryBlockId == GuidId
                         && (_OlympTypeId.HasValue ? OL.OlympTypeId == _OlympTypeId : true)
                         && (_OlympLevelId.HasValue ? OL.OlympLevelId == _OlympLevelId : true)
                         && (_OlympYear.HasValue ? OL.OlympYear == _OlympYear : true)
                         select new
                         {
                             Res.Id,
                             OL.OlympYear,
                             OlympLevel = OL.OlympLevelName,
                             OlympName = OL.OlympNameName,
                             OlympProfile = OL.OlympProfileName,
                             OlympSubject = OL.OlympSubjectName,
                             OlympValue = OlympVal.Name,
                             Res.MinEge
                         }).OrderBy(x => x.OlympName).ThenBy(x => x.OlympSubject).ThenBy(x => x.OlympValue).ThenBy(x => x.OlympYear);

                    _tblSource = Converter.ConvertToDataTable(data.ToArray());
                    e.Result = _tblSource;
                }
            }, () =>
            {
                dgvOlymps.Columns["Id"].Visible = false;
                dgvOlymps.Columns["OlympYear"].HeaderText = "Год";
                dgvOlymps.Columns["OlympYear"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvOlymps.Columns["OlympLevel"].HeaderText = "Уровень";
                dgvOlymps.Columns["OlympLevel"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvOlymps.Columns["OlympName"].HeaderText = "Название";
                dgvOlymps.Columns["OlympProfile"].HeaderText = "Профиль олимпиады";
                dgvOlymps.Columns["OlympSubject"].HeaderText = "Предмет олимпиады";
                dgvOlymps.Columns["OlympValue"].HeaderText = "Статус";
                dgvOlymps.Columns["OlympValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvOlymps.Columns["MinEge"].HeaderText = "Мин. ЕГЭ";
                dgvOlymps.Columns["MinEge"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                lblCountOlymps.Text = dgvOlymps.Rows.Count.ToString();
                ApplyFiltersToGridOlymp();
            }, obj);
        }
        private void ApplyFiltersToGridOlymp()
        {
            var dic = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(tbOlympName.Text.Trim()))
                dic.Add("OlympName", tbOlympName.Text.Trim());
            if (!string.IsNullOrEmpty(tbOlympSubject.Text.Trim()))
                dic.Add("OlympSubject", tbOlympSubject.Text.Trim());

            if (dic.Count > 0)
                lblCountOlymps.Text = WinFormsServ.FilterGrid(ref dgvOlymps, dic, _tblSource).ToString();
        }

        protected override bool CheckFields()
        {
            epError.Clear();
            bool bRet = true;

            if (lstUnit.Count == 0)
            {
                WinFormsServ.Error("Не выбраны экзамены!");
                epError.SetError(lbExams, "Не выбраны экзамены!");
                bRet = false;
            }

            return bRet;
        }
        protected override string Save()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    if (_Id == null)
                    {
                        ObjectParameter entId = new ObjectParameter("Id", typeof(Guid));
                        using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
                        { 
                            try
                            {
                                InsertRec(context, entId);

                                context.SaveChanges();
                                transaction.Complete();
                            }
                            catch (Exception exc)
                            {
                                throw exc;
                            }
                        }
                        return entId.ToString();
                    }
                    else
                    {
                        UpdateRec(context, gId.Value);
                        return _Id;
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        protected override void InsertRec(PriemEntities context, ObjectParameter idParam)
        {
            Guid entId = Guid.NewGuid();
            idParam.Value = entId;
            string queryBlock = @" INSERT INTO dbo.ExamInEntryBlock ([Id], [EntryId], [Name]) VALUES (@Id, @EntryId, @Name)";
            string queryBlockUnit = @" INSERT INTO dbo.ExamInEntryBlockUnit ([Id], [ExamInEntryBlockId], [ExamId], EgeMin) 
                                        VALUES (@Id, @ExamInEntryBlockId, @ExamId, @EgeMin)";

            if (OrderNumber == null)
            {
                int num = (from x in context.ExamInEntryBlock
                           where x.EntryId == _entryId
                           select x.OrderNumber).OrderByDescending(x => x).FirstOrDefault();
                OrderNumber = (byte)(num + 1);
            }

            Entry curEnt = (from ent in context.Entry
                            where ent.Id == _entryId
                            select ent).FirstOrDefault();
            context.ExamInEntryBlock.Add(new ExamInEntryBlock()
            {
                Id = entId,
                EntryId = curEnt.Id,
                Name = BlockName,
                IsNotInTotalSum = IsNotInTotalSum,
                OrderNumber = OrderNumber ?? 1,
                ParentExamInEntryBlockId = ParentExamInEntryId,
            });
            SortedList<string, object> sl = new SortedList<string, object>();
            sl.Add("@Id", entId);
            sl.Add("@EntryId", curEnt.Id);
            sl.Add("@Name", BlockName);
            
            MainClass.BdcOnlineReadWrite.ExecuteQuery(queryBlock, sl);
            foreach (ExamenBlockUnit x in lstUnit)
            {
                context.ExamInEntryBlockUnit.Add(new ExamInEntryBlockUnit()
                {
                    Id = x.UnitId,
                    ExamId = x.ExamId,
                    EgeMin = x.EgeMin,
                    ExamInEntryBlockId = entId,
                });
                SortedList<string, object> _sl = new SortedList<string, object>();
                _sl.Add("@Id", x.UnitId);
                _sl.Add("@ExamInEntryBlockId", entId);
                _sl.Add("@ExamId", x.ExamId);
                if (x.EgeMin.HasValue)
                    _sl.Add("@EgeMin", x.EgeMin);
                else
                    _sl.Add("@EgeMin", DBNull.Value);

                MainClass.BdcOnlineReadWrite.ExecuteQuery(queryBlockUnit, _sl);
            }

            IEnumerable<Entry> ents =
                from ent in context.Entry
                where ent.FacultyId == curEnt.FacultyId
                && ent.LicenseProgramId == curEnt.LicenseProgramId
                && ent.ObrazProgramId == curEnt.ObrazProgramId
                && ent.ProfileId == curEnt.ProfileId
                && ent.Id != curEnt.Id
                && ent.IsForeign == curEnt.IsForeign
                && ent.IsParallel == curEnt.IsParallel
                && ent.IsReduced == curEnt.IsReduced
                && ent.IsSecond == curEnt.IsSecond
                select ent;

            if (!chbToAllStudyBasis.Checked)
                ents = ents.Where(c => c.StudyBasisId == curEnt.StudyBasisId);
            foreach (Entry e in ents)
            {
                Guid blId = Guid.NewGuid();
                context.ExamInEntryBlock.Add(new ExamInEntryBlock()
                {
                    Id = blId,
                    EntryId = e.Id,
                    Name = BlockName,
                    IsNotInTotalSum = IsNotInTotalSum,
                    OrderNumber = OrderNumber ?? 1,
                });
                SortedList<string, object> sl_ = new SortedList<string, object>();
                sl_.Add("@Id", blId);
                sl_.Add("@EntryId", e.Id);
                sl_.Add("@Name", BlockName);
                MainClass.BdcOnlineReadWrite.ExecuteQuery(queryBlock, sl_);
                foreach (ExamenBlockUnit x in lstUnit)
                {
                    Guid UnitId = Guid.NewGuid();
                    context.ExamInEntryBlockUnit.Add(new ExamInEntryBlockUnit()
                    {
                        Id = UnitId,
                        ExamId = x.ExamId,
                        EgeMin = x.EgeMin,
                        ExamInEntryBlockId = blId,
                    });
                    SortedList<string, object> _sl = new SortedList<string, object>();
                    _sl.Add("@Id", UnitId);
                    _sl.Add("@ExamInEntryBlockId", blId);
                    _sl.Add("@ExamId", x.ExamId);
                    if (x.EgeMin.HasValue)
                        _sl.Add("@EgeMin", x.EgeMin);
                    else
                        _sl.Add("@EgeMin", DBNull.Value);
                    MainClass.BdcOnlineReadWrite.ExecuteQuery(queryBlockUnit, _sl);
                }
            }
        }
        protected override void UpdateRec(PriemEntities context, Guid id)
        {
            if (OrderNumber == null)
            {
                int num = (from x in context.ExamInEntryBlock
                           where x.EntryId == _entryId
                           select x.OrderNumber).OrderByDescending(x => x).FirstOrDefault();
                OrderNumber = (byte)(num + 1);
            }
            ExamInEntryBlock block = context.ExamInEntryBlock.Where(x => x.Id == gId).First();
            block.Name = BlockName;
            block.IsNotInTotalSum = IsNotInTotalSum;
            block.IsProfil = IsProfil;
            block.OrderNumber = OrderNumber ?? 1;
            block.ParentExamInEntryBlockId = ParentExamInEntryId;

            context.SaveChanges();

            var gUnits = lstUnit.Select(x => x.UnitId).ToList();
            var lst = context.ExamInEntryBlockUnit.Where(x => x.ExamInEntryBlockId == gId.Value && !gUnits.Contains(x.Id)).ToList();
            foreach (var x in lst)
            {
                context.ExamInEntryBlockUnit.Remove(x);
                MainClass.BdcOnlineReadWrite.ExecuteQuery(String.Format("delete from dbo.ExamInEntryBlockUnit where Id = '{0}'", x.Id));
            }
            string queryBlock = @" UPDATE dbo.ExamInEntryBlock set Name = @Name WHERE Id = @Id";
            SortedList<string, object> sl = new SortedList<string, object>();
            sl.Add("@Id", id);
            sl.Add("@Name", BlockName);
            MainClass.BdcOnlineReadWrite.ExecuteQuery(queryBlock, sl);

            string queryBlockUnitInsert = @" INSERT INTO dbo.ExamInEntryBlockUnit ([Id], [ExamInEntryBlockId], [ExamId], EgeMin) 
                                        VALUES (@Id, @ExamInEntryBlockId, @ExamId, @EgeMin)";
            string queryBlockUnitUpdate = @" UPDATE dbo.ExamInEntryBlockUnit SET [ExamId] = @ExamId, EgeMin = @EgeMin 
                                        WHERE Id =@Id";

            foreach (var x in lstUnit)
            {
                ExamInEntryBlockUnit ex_unit = context.ExamInEntryBlockUnit.Where(ex => ex.Id == x.UnitId).FirstOrDefault();
                if (ex_unit == null)
                {
                    context.ExamInEntryBlockUnit.Add(new ExamInEntryBlockUnit()
                    {
                        Id = x.UnitId,
                        ExamId = x.ExamId,
                        EgeMin = x.EgeMin,
                        ExamInEntryBlockId = gId.Value,
                    });
                    context.SaveChanges();
                    SortedList<string, object> _sl = new SortedList<string, object>();
                    _sl.Add("@Id", x.UnitId);
                    _sl.Add("@ExamInEntryBlockId", gId.Value);
                    _sl.Add("@ExamId", x.ExamId);
                    if (x.EgeMin.HasValue)
                        _sl.Add("@EgeMin", x.EgeMin);
                    else
                        _sl.Add("@EgeMin", DBNull.Value);
                    MainClass.BdcOnlineReadWrite.ExecuteQuery(queryBlockUnitInsert, _sl);
                }
                else
                {
                    ex_unit.ExamId = x.ExamId;
                    ex_unit.EgeMin = x.EgeMin;
                    context.SaveChanges();
                    SortedList<string, object> _sl = new SortedList<string, object>();
                    _sl.Add("@Id", ex_unit.Id);
                    _sl.Add("@ExamId", x.ExamId);
                    if (x.EgeMin.HasValue)
                        _sl.Add("@EgeMin", x.EgeMin);
                    else
                        _sl.Add("@EgeMin", DBNull.Value);
                    MainClass.BdcOnlineReadWrite.ExecuteQuery(queryBlockUnitUpdate, _sl);
                }
            }
        }
        protected override void CloseCardAfterSave()
        {
            this.Close();
        }

        private void btnExamUnitAdd_Click(object sender, EventArgs e)
        {
            new CardExamInEntryUnit(null, new UnitListUpdateHandler(UnitListAdd)).Show();
        }

        private void btnExamUnitDelete_Click(object sender, EventArgs e)
        {
            var lst = new List<KeyValuePair<string, string>>();
            foreach (var x in lbExams.SelectedItems)
            {
                if (x is KeyValuePair<string, string>)
                {
                    Guid _g = Guid.Parse(((KeyValuePair<string, string>)x).Key.ToString());
                    var unit = lstUnit.Where(u => u.UnitId == _g).FirstOrDefault();
                    if (unit != null)
                        lstUnit.Remove(unit);
                    lst.Add((KeyValuePair<string, string>)x);
                }
            }
            foreach (var x in lst)
                lbExams.Items.Remove(x);
        }

        private void UnitListAdd(ExamenBlockUnit unit)
        {
            if (lstUnit.Count == 0 && String.IsNullOrEmpty(tbBlockName.Text.Trim()))
            {
                tbBlockName.Text = unit.ExamUnitName;
            }

            if (lstUnit.Where(x=>x.UnitId == unit.UnitId).Count()>0)
            {
                var Item = lstUnit.Where(x => x.UnitId == unit.UnitId).Select(x => x).First();
                lstUnit.Remove(Item);

                foreach ( KeyValuePair<string, string>  lbItem in lbExams.Items)
                {
                    if (lbItem.Key == unit.UnitId.ToString())
                    {
                        lbExams.Items.Remove(lbItem);
                        break;
                    }
                }
            }
            if (lstUnit.Where(x=>x.ExamId == unit.ExamId).Count()>0)
            {
                MessageBox.Show("Экзамен уже был добавлен в текущую группу экзаменов");
                return;
            }
            
            using (PriemEntities context = new PriemEntities())
            {
                var lst = (from x in context.extExamInEntry
                           where x.EntryId == _entryId
                           && x.ExamId == unit.ExamId
                           select x.Id).ToList();
                if (lst.Count > 0)
                {
                    MessageBox.Show("Экзамен уже был добавлен в текущий конкурс");
                    return;
                }
            }

            lstUnit.Add(unit);
            KeyValuePair<string, string> ex = new KeyValuePair<string,string>( unit.UnitId.ToString(), unit.ExamUnitName + 
                (unit.EgeMin==null?"":" ("+unit.EgeMin.ToString()+")"));
            lbExams.Items.Add(ex);
        }

        private void lbExams_DoubleClick(object sender, EventArgs e)
        {
            if (lbExams.SelectedItem == null)
                return;

            var Item = (KeyValuePair<string, string>)lbExams.SelectedItem;
            Guid gid = Guid.Parse(Item.Key.ToString());
            CardExamInEntryUnit crd = new CardExamInEntryUnit(Item.Key, new UnitListUpdateHandler(UnitListAdd));
            crd.ExamId = lstUnit.Where(x => x.UnitId == gid).First().ExamId;
            crd.EgeMin = lstUnit.Where(x => x.UnitId == gid).First().EgeMin;
            crd.Show();
        }

        private void btnTimeTable_Click(object sender, EventArgs e)
        {
            if (lstUnit.Count>0)
                new CardExamTimeTable(lstUnit).Show();
        }

        private void btnAddOlympiad_Click(object sender, EventArgs e)
        {
            OpenCardOlympiad(null);
        }
        private void btnOpenOlympiad_Click(object sender, EventArgs e)
        {
            if (dgvOlymps.SelectedCells.Count == 0)
                return;
            OpenCardOlympiad(dgvOlymps.SelectedCells[0].RowIndex);
        }
        private void btnDeleteOlympiad_Click(object sender, EventArgs e)
        {

        }
        private void dgvOlymps_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > 0)
                OpenCardOlympiad(e.RowIndex);
        }
        private void OpenCardOlympiad(int? rowIndex)
        {
            if (!GuidId.HasValue)
                return;

            CardOlympResultToMaxMark crd;
            if (!rowIndex.HasValue)
            {
                crd = new CardOlympResultToMaxMark(null, GuidId.Value);
            }
            else
            {
                string sId = dgvOlymps["Id", rowIndex.Value].Value.ToString();
                crd = new CardOlympResultToMaxMark(sId, GuidId.Value);
            }

            crd.ToUpdateList += UpdateGridOlymps;
            crd.Show();
        }
    }
}
