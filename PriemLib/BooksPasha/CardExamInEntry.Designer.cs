namespace PriemLib
{
    partial class CardExamInEntry
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbExam = new System.Windows.Forms.ComboBox();
            this.chbIsProfil = new System.Windows.Forms.CheckBox();
            this.tbEgeMin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chbToAllStudyBasis = new System.Windows.Forms.CheckBox();
            this.chbCrimea = new System.Windows.Forms.CheckBox();
            this.chbGosLine = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbOrderNumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbParentExamInEntry = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.epError)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(359, 207);
            // 
            // btnSaveChange
            // 
            this.btnSaveChange.Location = new System.Drawing.Point(12, 207);
            // 
            // btnSaveAsNew
            // 
            this.btnSaveAsNew.Location = new System.Drawing.Point(215, 207);
            // 
            // cbExam
            // 
            this.cbExam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbExam.FormattingEnabled = true;
            this.cbExam.Location = new System.Drawing.Point(12, 32);
            this.cbExam.Name = "cbExam";
            this.cbExam.Size = new System.Drawing.Size(428, 21);
            this.cbExam.TabIndex = 29;
            // 
            // chbIsProfil
            // 
            this.chbIsProfil.AutoSize = true;
            this.chbIsProfil.Location = new System.Drawing.Point(12, 99);
            this.chbIsProfil.Name = "chbIsProfil";
            this.chbIsProfil.Size = new System.Drawing.Size(92, 17);
            this.chbIsProfil.TabIndex = 30;
            this.chbIsProfil.Text = "Профильный";
            this.chbIsProfil.UseVisualStyleBackColor = true;
            // 
            // tbEgeMin
            // 
            this.tbEgeMin.Location = new System.Drawing.Point(12, 167);
            this.tbEgeMin.Name = "tbEgeMin";
            this.tbEgeMin.Size = new System.Drawing.Size(100, 20);
            this.tbEgeMin.TabIndex = 31;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 151);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 32;
            this.label1.Text = "Минимальный балл";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Экзамен";
            // 
            // chbToAllStudyBasis
            // 
            this.chbToAllStudyBasis.AutoSize = true;
            this.chbToAllStudyBasis.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chbToAllStudyBasis.Location = new System.Drawing.Point(312, 99);
            this.chbToAllStudyBasis.Name = "chbToAllStudyBasis";
            this.chbToAllStudyBasis.Size = new System.Drawing.Size(128, 30);
            this.chbToAllStudyBasis.TabIndex = 35;
            this.chbToAllStudyBasis.Text = "размножить \r\nна основы обучения";
            this.chbToAllStudyBasis.UseVisualStyleBackColor = true;
            // 
            // chbCrimea
            // 
            this.chbCrimea.AutoSize = true;
            this.chbCrimea.Location = new System.Drawing.Point(12, 122);
            this.chbCrimea.Name = "chbCrimea";
            this.chbCrimea.Size = new System.Drawing.Size(55, 17);
            this.chbCrimea.TabIndex = 36;
            this.chbCrimea.Text = "Крым";
            this.chbCrimea.UseVisualStyleBackColor = true;
            // 
            // chbGosLine
            // 
            this.chbGosLine.AutoSize = true;
            this.chbGosLine.Location = new System.Drawing.Point(73, 122);
            this.chbGosLine.Name = "chbGosLine";
            this.chbGosLine.Size = new System.Drawing.Size(74, 17);
            this.chbGosLine.TabIndex = 37;
            this.chbGosLine.Text = "Гослиния";
            this.chbGosLine.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(334, 151);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 39;
            this.label3.Text = "Порядковый номер";
            // 
            // tbOrderNumber
            // 
            this.tbOrderNumber.Location = new System.Drawing.Point(340, 167);
            this.tbOrderNumber.Name = "tbOrderNumber";
            this.tbOrderNumber.Size = new System.Drawing.Size(100, 20);
            this.tbOrderNumber.TabIndex = 38;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(259, 13);
            this.label4.TabIndex = 41;
            this.label4.Text = "Родительский экзамен (для многоуровневых ВИ)";
            // 
            // cbParentExamInEntryId
            // 
            this.cbParentExamInEntry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbParentExamInEntry.FormattingEnabled = true;
            this.cbParentExamInEntry.Location = new System.Drawing.Point(12, 72);
            this.cbParentExamInEntry.Name = "cbParentExamInEntryId";
            this.cbParentExamInEntry.Size = new System.Drawing.Size(428, 21);
            this.cbParentExamInEntry.TabIndex = 40;
            // 
            // CardExamInEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 242);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbParentExamInEntry);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbOrderNumber);
            this.Controls.Add(this.chbGosLine);
            this.Controls.Add(this.chbCrimea);
            this.Controls.Add(this.chbToAllStudyBasis);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbEgeMin);
            this.Controls.Add(this.chbIsProfil);
            this.Controls.Add(this.cbExam);
            this.Name = "CardExamInEntry";
            this.Text = "CardExamInEntry";
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.Controls.SetChildIndex(this.btnSaveAsNew, 0);
            this.Controls.SetChildIndex(this.btnSaveChange, 0);
            this.Controls.SetChildIndex(this.cbExam, 0);
            this.Controls.SetChildIndex(this.chbIsProfil, 0);
            this.Controls.SetChildIndex(this.tbEgeMin, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.chbToAllStudyBasis, 0);
            this.Controls.SetChildIndex(this.chbCrimea, 0);
            this.Controls.SetChildIndex(this.chbGosLine, 0);
            this.Controls.SetChildIndex(this.tbOrderNumber, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.cbParentExamInEntry, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            ((System.ComponentModel.ISupportInitialize)(this.epError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbExam;
        private System.Windows.Forms.CheckBox chbIsProfil;
        private System.Windows.Forms.TextBox tbEgeMin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chbToAllStudyBasis;
        private System.Windows.Forms.CheckBox chbCrimea;
        private System.Windows.Forms.CheckBox chbGosLine;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbOrderNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbParentExamInEntry;
    }
}