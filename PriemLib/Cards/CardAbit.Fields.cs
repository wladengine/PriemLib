using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EducServLib;

namespace PriemLib
{
    public partial class CardAbit
    {
        public Guid? EntryId { get; set; }
        
        public int? FacultyId
        {
            get; set;
        }

        public int? LicenseProgramId
        {
            get; set;
        }

        public int? ObrazProgramId
        {
            get; set;
        }

        public int ProfileId
        {
            get; set;
        }

        public int? StudyFormId
        {
            get; set;
        }

        public int? StudyBasisId
        {
            get; set;
        }

        public int? CompetitionId
        {
            get { return ComboServ.GetComboIdInt(cbCompetition); }
            set { ComboServ.SetComboId(cbCompetition, value); }
        }

        public Guid? OlympiadId
        {
            get { return ComboServ.GetComboIdGuid(cbBenefitOlympSource); }
            set { ComboServ.SetComboId(cbBenefitOlympSource, value); }
        }

        public bool IsSecond
        {
            get { return chbIsSecond.Checked; }
            set { chbIsSecond.Checked = value; }
        }

        public bool IsReduced
        {
            get { return chbIsReduced.Checked; }
            set { chbIsReduced.Checked = value; }
        }

        public bool IsParallel
        {
            get { return chbIsParallel.Checked; }
            set { chbIsParallel.Checked = value; }
        }

        public bool IsListener
        {
            get { return chbIsListener.Checked; }
            set { chbIsListener.Checked = value; }
        }

        public bool IsPaid
        {
            get { return chbIsPaid.Checked; }
            set { chbIsPaid.Checked = value; }
        }

        public bool HasEntryConfirm
        {
            get { return chbHasEntryConfirm.Checked; }
            set { chbHasEntryConfirm.Checked = value; }
        }
        public bool NoConfirm
        {
            get { return chbNoConfirm.Checked; }
            set { chbNoConfirm.Checked = value; }
        }

        public bool HasDisabledEntryConfirm
        {
            get { return chbHasDisabledEntryConfirm.Checked; }
            set { chbHasDisabledEntryConfirm.Checked = value; }
        }

        public bool BackDoc
        {
            get { return chbBackDoc.Checked; }
            set 
            { 
                chbBackDoc.Checked = value;
                if (BackDoc)
                    chbBackDoc.ForeColor = System.Drawing.Color.Red;                
            }
        }

        public DateTime? BackDocDate
        {
            get 
            {
                if (BackDoc)
                    return dtBackDocDate.Value.Date;
                else
                    return null;
            }
            set 
            {
                if (BackDoc)
                {
                    if (value.HasValue)
                        dtBackDocDate.Value = value.Value;
                }               
            }
        }

        public DateTime? DocDate
        {
            get { return dtDocDate.Value.Date; }
            set 
            {
                if (value.HasValue)
                    dtDocDate.Value = value.Value; 
            }          
        }

        public DateTime? DocInsertDate
        {
            get { return dtDocInsertDate.Value.Date; }
            set 
            {
                if (value.HasValue)
                    dtDocInsertDate.Value = value.Value; 
            }
        }

        public bool AttDocOrigin
        {
            get { return chbAttOriginal.Checked; }
            set { chbAttOriginal.Checked = value; }
        }

        public bool Checked
        {
            get { return chbChecked.Checked; }
            set { chbChecked.Checked = value; }
        }

        public bool NotEnabled
        {
            get { return chbNotEnabled.Checked; }
            set { chbNotEnabled.Checked = value; }
        }

        public string Sum
        {
            get { return tbSum.Text.Trim(); }
            set { tbSum.Text = value; }
        }

        public bool BackDocByAdmissionHigh
        {
            get { return lblBackDocByAdmissionHigh.Visible; }
            set { lblBackDocByAdmissionHigh.Visible = value; }
        }

        public double? Coefficient
        {
            get
            {
                double j;
                if (double.TryParse(tbCoefficient.Text.Trim().Replace('.', ','), out j))
                    return j;
                else
                    return null;
            }
            set { tbCoefficient.Text = Util.ToStr(value); }
        }

        public int? OtherCompetitionId
        {
            get 
            {
                if (CompetitionId == 6 && cbOtherCompetition.SelectedIndex != 0)
                    return ComboServ.GetComboIdInt(cbOtherCompetition);
                else
                    return null;                
            }
            set 
            {
                if (CompetitionId == 6)
                    if (value != null)
                        ComboServ.SetComboId(cbOtherCompetition, value);                 
            }
        }

        public int? CelCompetitionId
        {
            get 
            {
                if (CompetitionId == 6 && cbCelCompetition.SelectedIndex != 0)
                    return ComboServ.GetComboIdInt(cbCelCompetition);
                else
                    return null;                
            }
            set 
            {
                if (CompetitionId == 6)
                    if (value != null)
                        ComboServ.SetComboId(cbCelCompetition, value);                     
            } 
        }

        public string CelCompetitionText
        {
            get
            {
                if (CompetitionId == 6)
                    return tbCelCompetitionText.Text;
                else
                    return string.Empty;
            }
            set
            {
                if (CompetitionId == 6)
                    tbCelCompetitionText.Text = value;                    
            }
        }

        public bool CompFromOlymp
        {
            set { lblCompFromOlymp.Visible = value; }           
        }

        public int? LanguageId
        {
            get { return ComboServ.GetComboIdInt(cbLanguage); }
            set { ComboServ.SetComboId(cbLanguage, value); }
        }

        public bool HasOriginals
        {
            get { return chbHasOriginals.Checked; }
            set { chbHasOriginals.Checked = value; }
        }
        public bool IsForeign
        {
            get { return chbIsForeign.Checked; }
            set { chbIsForeign.Checked = value; }
        }

        public int? Priority
        {
            get
            {
                int j;
                if (int.TryParse(tbPriority.Text.Trim(), out j))
                    return j;
                else
                    return null;
            }
            set { tbPriority.Text = Util.ToStr(value); }
        }

        public Guid? InnerEntryInEntryId
        {
            get { return ComboServ.GetComboIdGuid(cbInnerEntryInEntry); }
            set { ComboServ.SetComboId(cbInnerEntryInEntry, value); }
        }

        public bool HasManualExams { get; set; }
        public List<ExamenBlock> lstExamInEntryBlock { get; set; }
    }
}
