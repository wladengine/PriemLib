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
                        new CardExamInEntryUnit(null, true, new UnitListUpdateHandler(UnitListAdd)).Show();
                    }
                    else
                        foreach (var x in lst)
                            lbExams.Items.Add(x);
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при инициализации формы ", exc);
            }
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

        protected override string Save()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    if (_Id == null)
                    {
                        Guid entId = Guid.NewGuid();
                        using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
                        { 
                            try
                            {
                                if (OrderNumber == null)
                                {
                                    int num = (from x in context.ExamInEntryBlock
                                               where x.EntryId == _entryId
                                               select x.OrderNumber).OrderByDescending(x => x).FirstOrDefault();
                                    OrderNumber = (byte)(num + 1);    
                                }
                                context.ExamInEntryBlock.AddObject(new ExamInEntryBlock() {
                                    Id = entId,
                                    EntryId = _entryId.Value,
                                    Name = BlockName,
                                    IsCrimea = IsCrimea,
                                    IsGosLine = IsGosLine,
                                    OrderNumber = OrderNumber ?? 1,
                                    });

                                foreach (ExamenBlockUnit x in lstUnit)
                                {
                                    context.ExamInEntryBlockUnit.AddObject(new ExamInEntryBlockUnit()
                                    {
                                        Id = x.UnitId,
                                        ExamId = x.ExamId,
                                        EgeMin = x.EgeMin,
                                        ExamInEntryBlockId = entId,
                                    });
                                }
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
                        if (OrderNumber == null)
                        {
                            int num = (from x in context.ExamInEntryBlock
                                       where x.EntryId == _entryId
                                       select x.OrderNumber).OrderByDescending(x => x).FirstOrDefault();
                            OrderNumber = (byte)(num + 1);
                        }
                        ExamInEntryBlock block = context.ExamInEntryBlock.Where(x=>x.Id == gId).First();
                        block.Name = BlockName;
                        block.IsCrimea = IsCrimea;
                        block.IsGosLine = IsGosLine;
                        block.OrderNumber = OrderNumber ?? 1;

                        var gUnits = lstUnit.Select(x => x.UnitId).ToList();
                        var lst = context.ExamInEntryBlockUnit.Where(x => x.ExamInEntryBlockId == gId.Value && !gUnits.Contains(x.Id)).ToList();
                        foreach (var x in lst)
                            context.ExamInEntryBlockUnit.DeleteObject(x);

                        foreach (var x in lstUnit)
                        {
                            ExamInEntryBlockUnit ex_unit = context.ExamInEntryBlockUnit.Where(ex=> ex.Id == x.UnitId).FirstOrDefault();
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
                            }
                            else
                            {
                                ex_unit.ExamId = x.ExamId;
                                ex_unit.EgeMin = x.EgeMin;
                                context.SaveChanges();
                            }
                        }
                        return _Id;
                    }
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        protected override void CloseCardAfterSave()
        {
            this.Close();
        }

        private void btnExamUnitAdd_Click(object sender, EventArgs e)
        {
            new CardExamInEntryUnit(null, true, new UnitListUpdateHandler(UnitListAdd)).Show();
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
            if (lstUnit.Where(x=>x.ExamId == unit.ExamId).Count()>0)
            {
                MessageBox.Show("Экзамен уже был добавлен");
                return;
            }
            lstUnit.Add(unit);
            KeyValuePair<string, string> ex = new KeyValuePair<string,string>( unit.UnitId.ToString(), unit.ExamUnitName + 
                (unit.EgeMin==null?"":" ("+unit.EgeMin.ToString()+")"));
            lbExams.Items.Add(ex);
        }
    }
}
