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
        protected bool IsGosLine
        {
            get { return chbGosLine.Checked; }
            set { chbGosLine.Checked = value; }
        }
        protected string BlockName
        {
            get { return tbBlockName.Text.Trim(); }
            set { tbBlockName.Text = value.Trim(); }
        }
        protected bool IsCrimea
        {
            get { return chbCrimea.Checked; }
            set { chbCrimea.Checked = value; }
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
        protected UnitListUpdateHandler _hndl;
        #endregion

        private bool _isForModified;
        private Guid? _entryId;

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
                            }).Distinct()).ToList()
                          .Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name))
                          .OrderBy(x => x.Value)
                          .ToList();
                    ComboServ.FillCombo(cbParentExamInEntry, lst, true, false);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы ", exc);
            }
        }

        void CardExamInEntryBlock_Shown(object sender, EventArgs e)
        {
            new CardExamInEntryUnit(null, new UnitListUpdateHandler(UnitListAdd)).Show();
        }
        protected override void SetAllFieldsEnabled()
        {
            base.SetAllFieldsEnabled();

        }
        protected override void SetReadOnlyFieldsAfterFill()
        {
            base.SetReadOnlyFieldsAfterFill();

            if (!MainClass.IsEntryChanger())
                btnSaveChange.Enabled = false;
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
                    ExamInEntryBlock ent = (from ex in context.ExamInEntryBlock
                                           where ex.Id == gId
                                           select ex).FirstOrDefault();
                    BlockName = ent.Name;
                    IsProfil = ent.IsProfil;
                    IsGosLine = ent.IsGosLine;
                    IsCrimea = ent.IsCrimea;
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
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при заполнении формы ", exc);
            }
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
            context.ExamInEntryBlock.AddObject(new ExamInEntryBlock()
            {
                Id = entId,
                EntryId = curEnt.Id,
                Name = BlockName,
                IsCrimea = IsCrimea,
                IsGosLine = IsGosLine,
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
                context.ExamInEntryBlockUnit.AddObject(new ExamInEntryBlockUnit()
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
            
            IEnumerable<Entry> ents = from ent in context.Entry
                                      where
                                      ent.FacultyId == curEnt.FacultyId
                                      && ent.LicenseProgramId == curEnt.LicenseProgramId
                                      && ent.ObrazProgramId == curEnt.ObrazProgramId
                                      && (curEnt.ProfileId == null ? ent.ProfileId == null : ent.ProfileId == curEnt.ProfileId)
                                      && ent.Id != curEnt.Id
                                      && ent.IsCrimea == curEnt.IsCrimea
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
                context.ExamInEntryBlock.AddObject(new ExamInEntryBlock()
                {
                    Id = blId,
                    EntryId = e.Id,
                    Name = BlockName,
                    IsCrimea = IsCrimea,
                    IsGosLine = IsGosLine,
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
                    context.ExamInEntryBlockUnit.AddObject(new ExamInEntryBlockUnit()
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
            block.IsCrimea = IsCrimea;
            block.IsGosLine = IsGosLine;
            block.OrderNumber = OrderNumber ?? 1;
            block.ParentExamInEntryBlockId = ParentExamInEntryId;
            context.SaveChanges();

            var gUnits = lstUnit.Select(x => x.UnitId).ToList();
            var lst = context.ExamInEntryBlockUnit.Where(x => x.ExamInEntryBlockId == gId.Value && !gUnits.Contains(x.Id)).ToList();
            foreach (var x in lst)
            {
                context.ExamInEntryBlockUnit.DeleteObject(x);
                MainClass.BdcOnlineReadWrite.ExecuteQuery(String.Format("delete from dbo.ExamInEntryBlockUnit where Id = '{0}'", x.Id), null);
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
                    context.ExamInEntryBlockUnit.AddObject(new ExamInEntryBlockUnit()
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
    }
}
