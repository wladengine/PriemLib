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
    public partial class CardExamInEntryUnit : BookCard
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
        protected int? ExamId
        {
            get { return ComboServ.GetComboIdInt(cbExam); }
            set { ComboServ.SetComboId(cbExam, value); }
        }
        protected int? EgeMin
        {
            get
            {
                int j;
                if (int.TryParse(tbEgeMin.Text.Trim(), out j))
                    return j;
                else
                    return null;
            }
            set { tbEgeMin.Text = value.ToString(); }
        }
        #endregion

        private bool _isForModified;
        UnitListUpdateHandler _hndlr;

        public CardExamInEntryUnit(string id, bool isForModified, UnitListUpdateHandler _h)
            : base(id)
        {
            InitializeComponent();
            _isForModified = isForModified;
            _hndlr = _h;
            InitControls();
            SetAllFieldsEnabled();
        }

        protected override void ExtraInit()
        {
            base.ExtraInit();
            _title = "Экзамен";
            _tableName = "ed.[ExamInEntryBlockUnit]";
            btnClose.Visible = btnSaveAsNew.Visible = false;
            btnSaveChange.Visible = true;

            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    List<KeyValuePair<string, string>> lst =
                        ((from f in context.Exam
                          join en in context.ExamName
                          on f.ExamNameId equals en.Id
                          select new
                          {
                              Id = f.Id,
                              Name = en.Name,
                              IsAdd = f.IsAdditional
                          }).Distinct()).ToList()
                          .Select(u => new KeyValuePair<string, string>(u.Id.ToString(), u.Name + (u.IsAdd ? " (доп)" : "")))
                          .OrderBy(x => x.Value)
                          .ToList();
                    ComboServ.FillCombo(cbExam, lst, false, false);
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

            cbExam.Enabled = true;
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
                return;
        }

        protected override string Save()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    if (_Id == null)
                    {
                        Guid _gId = Guid.NewGuid();
                            if (_hndlr != null && ExamId.HasValue)
                            {
                                _hndlr(new ExamenBlockUnit()
                                {
                                    ExamUnitName = ((KeyValuePair<string, string>)cbExam.SelectedItem).Value.ToString(),
                                    UnitId = _gId,
                                    EgeMin = EgeMin,
                                    ExamId = ExamId.Value,
                                });
                            }
                        return _gId.ToString();
                    }
                    else
                    {
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

    }
}
