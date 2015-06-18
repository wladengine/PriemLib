﻿using System;
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
    public partial class CardExamInEntry : BookCard
    {
        private bool _isForModified;
        private Guid? _entryId;

        public CardExamInEntry(Guid? entryId, string id, bool isForModified)
            : base(id)
        {
            InitializeComponent();
            _entryId = entryId;
            _isForModified = isForModified;
            InitControls();
        }

        protected int? IntId
        {
            get
            {
                if (_Id == null)
                    return null;
                else
                    return int.Parse(_Id);
            }
        }

        protected override void SetReadOnlyFieldsAfterFill()
        {
            base.SetReadOnlyFieldsAfterFill();

            if (!MainClass.IsEntryChanger())
                btnSaveChange.Enabled = false;
        }     


        protected override void ExtraInit()
        {
            base.ExtraInit();
            _title = "Экзамен";
            _tableName = "ed.[ExamInEntry]";

            try
            {
                if (_Id != null)
                    chbToAllStudyBasis.Visible = false;
                chbToAllStudyBasis.Checked = true;

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

        protected int? ExamId
        {
            get { return ComboServ.GetComboIdInt(cbExam); }
            set { ComboServ.SetComboId(cbExam, value); }
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

        protected bool IsCrimea
        {
            get { return chbCrimea.Checked; }
            set { chbCrimea.Checked = value; }
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

        protected override void FillCard()
        {
            if (_Id == null)                
                return;
            
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    ExamInEntry ent = (from ex in context.ExamInEntry
                                       where ex.Id == IntId
                                       select ex).FirstOrDefault();


                    ExamId = ent.ExamId;
                    IsProfil = ent.IsProfil;
                    IsGosLine = ent.IsGosLine;
                    IsCrimea = ent.IsCrimea;
                    EgeMin = ent.EgeMin;
                    OrderNumber = ent.OrderNumber;
                }
            }
            catch (Exception exc)
            {
                WinFormsServ.Error("Ошибка при заполнении формы ", exc);
            }
        }

        protected override void SetAllFieldsEnabled()
        {
            base.SetAllFieldsEnabled();

            if (_Id != null)
                cbExam.Enabled = false;
        }

        protected override string Save()
        {
            try
            {
                using (PriemEntities context = new PriemEntities())
                {
                    if (_Id == null)
                    {
                        ObjectParameter exId;
                        
                        using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.RequiresNew))
                        { 
                            try
                            {
                                exId = new ObjectParameter("id", typeof(Int32));
                                context.ExamInEntry_Insert(ExamId, _entryId, IsProfil, EgeMin, IsCrimea, IsGosLine, OrderNumber, exId);

                                Entry curEnt = (from ent in context.Entry
                                              where ent.Id == _entryId
                                              select ent).FirstOrDefault();

                                IEnumerable<Entry> ents = from ent in context.Entry
                                                          where                                                           
                                                          ent.FacultyId == curEnt.FacultyId
                                                          && ent.LicenseProgramId == curEnt.LicenseProgramId
                                                          && ent.ObrazProgramId == curEnt.ObrazProgramId                                                         
                                                          && (curEnt.ProfileId == null ? ent.ProfileId == null : ent.ProfileId == curEnt.ProfileId) 
                                                          && ent.Id != curEnt.Id
                                                          select ent;

                                if (!chbToAllStudyBasis.Checked)
                                    ents = ents.Where(c => c.StudyBasisId == curEnt.StudyBasisId);

                                foreach (Entry e in ents)
                                {
                                    exId = new ObjectParameter("id", typeof(Int32));
                                    context.ExamInEntry_Insert(ExamId, e.Id, IsProfil, EgeMin, IsCrimea, IsGosLine, OrderNumber, exId);
                                }

                                transaction.Complete();
                            }
                            catch (Exception exc)
                            {
                                throw exc;
                            }
                        }

                        return exId.Value.ToString();
                    }
                    else
                    {
                        context.ExamInEntry_Update(ExamId, IsProfil, EgeMin, IsCrimea, IsGosLine, OrderNumber, IntId);
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
