namespace PriemLib
{
    partial class CardExamInEntryBlock
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
            this.chbIsProfil = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chbToAllStudyBasis = new System.Windows.Forms.CheckBox();
            this.chbCrimea = new System.Windows.Forms.CheckBox();
            this.chbGosLine = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbOrderNumber = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbBlockName = new System.Windows.Forms.TextBox();
            this.lbExams = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnExamUnitAdd = new System.Windows.Forms.Button();
            this.btnExamUnitDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.epError)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(359, 284);
            // 
            // btnSaveChange
            // 
            this.btnSaveChange.Location = new System.Drawing.Point(12, 284);
            // 
            // btnSaveAsNew
            // 
            this.btnSaveAsNew.Location = new System.Drawing.Point(215, 284);
            // 
            // chbIsProfil
            // 
            this.chbIsProfil.AutoSize = true;
            this.chbIsProfil.Location = new System.Drawing.Point(12, 210);
            this.chbIsProfil.Name = "chbIsProfil";
            this.chbIsProfil.Size = new System.Drawing.Size(92, 17);
            this.chbIsProfil.TabIndex = 30;
            this.chbIsProfil.Text = "Профильный";
            this.chbIsProfil.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Экзамен";
            // 
            // chbToAllStudyBasis
            // 
            this.chbToAllStudyBasis.AutoSize = true;
            this.chbToAllStudyBasis.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chbToAllStudyBasis.Location = new System.Drawing.Point(312, 220);
            this.chbToAllStudyBasis.Name = "chbToAllStudyBasis";
            this.chbToAllStudyBasis.Size = new System.Drawing.Size(128, 30);
            this.chbToAllStudyBasis.TabIndex = 35;
            this.chbToAllStudyBasis.Text = "размножить \r\nна основы обучения";
            this.chbToAllStudyBasis.UseVisualStyleBackColor = true;
            // 
            // chbCrimea
            // 
            this.chbCrimea.AutoSize = true;
            this.chbCrimea.Location = new System.Drawing.Point(12, 233);
            this.chbCrimea.Name = "chbCrimea";
            this.chbCrimea.Size = new System.Drawing.Size(55, 17);
            this.chbCrimea.TabIndex = 36;
            this.chbCrimea.Text = "Крым";
            this.chbCrimea.UseVisualStyleBackColor = true;
            // 
            // chbGosLine
            // 
            this.chbGosLine.AutoSize = true;
            this.chbGosLine.Location = new System.Drawing.Point(73, 233);
            this.chbGosLine.Name = "chbGosLine";
            this.chbGosLine.Size = new System.Drawing.Size(74, 17);
            this.chbGosLine.TabIndex = 37;
            this.chbGosLine.Text = "Гослиния";
            this.chbGosLine.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(334, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 39;
            this.label3.Text = "Порядковый номер";
            // 
            // tbOrderNumber
            // 
            this.tbOrderNumber.Location = new System.Drawing.Point(340, 25);
            this.tbOrderNumber.Name = "tbOrderNumber";
            this.tbOrderNumber.Size = new System.Drawing.Size(100, 20);
            this.tbOrderNumber.TabIndex = 38;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(210, 13);
            this.label1.TabIndex = 42;
            this.label1.Text = "Название группы экзаменов по выбору";
            // 
            // tbBlockName
            // 
            this.tbBlockName.Location = new System.Drawing.Point(12, 25);
            this.tbBlockName.Name = "tbBlockName";
            this.tbBlockName.Size = new System.Drawing.Size(303, 20);
            this.tbBlockName.TabIndex = 43;
            // 
            // lbExams
            // 
            this.lbExams.FormattingEnabled = true;
            this.lbExams.Location = new System.Drawing.Point(12, 77);
            this.lbExams.Name = "lbExams";
            this.lbExams.Size = new System.Drawing.Size(325, 121);
            this.lbExams.TabIndex = 44;
            this.lbExams.DoubleClick += new System.EventHandler(this.lbExams_DoubleClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(219, 13);
            this.label4.TabIndex = 45;
            this.label4.Text = "Список экзаменов, включенных в группу:";
            // 
            // btnExamUnitAdd
            // 
            this.btnExamUnitAdd.Location = new System.Drawing.Point(343, 77);
            this.btnExamUnitAdd.Name = "btnExamUnitAdd";
            this.btnExamUnitAdd.Size = new System.Drawing.Size(97, 23);
            this.btnExamUnitAdd.TabIndex = 46;
            this.btnExamUnitAdd.Text = "Добавить";
            this.btnExamUnitAdd.UseVisualStyleBackColor = true;
            this.btnExamUnitAdd.Click += new System.EventHandler(this.btnExamUnitAdd_Click);
            // 
            // btnExamUnitDelete
            // 
            this.btnExamUnitDelete.Location = new System.Drawing.Point(343, 106);
            this.btnExamUnitDelete.Name = "btnExamUnitDelete";
            this.btnExamUnitDelete.Size = new System.Drawing.Size(97, 23);
            this.btnExamUnitDelete.TabIndex = 47;
            this.btnExamUnitDelete.Text = "Удалить";
            this.btnExamUnitDelete.UseVisualStyleBackColor = true;
            this.btnExamUnitDelete.Click += new System.EventHandler(this.btnExamUnitDelete_Click);
            // 
            // CardExamInEntryBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 319);
            this.Controls.Add(this.btnExamUnitDelete);
            this.Controls.Add(this.btnExamUnitAdd);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbExams);
            this.Controls.Add(this.tbBlockName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbOrderNumber);
            this.Controls.Add(this.chbGosLine);
            this.Controls.Add(this.chbCrimea);
            this.Controls.Add(this.chbToAllStudyBasis);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chbIsProfil);
            this.Name = "CardExamInEntryBlock";
            this.Text = "CardExamInEntry";
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.Controls.SetChildIndex(this.btnSaveAsNew, 0);
            this.Controls.SetChildIndex(this.btnSaveChange, 0);
            this.Controls.SetChildIndex(this.chbIsProfil, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.chbToAllStudyBasis, 0);
            this.Controls.SetChildIndex(this.chbCrimea, 0);
            this.Controls.SetChildIndex(this.chbGosLine, 0);
            this.Controls.SetChildIndex(this.tbOrderNumber, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.tbBlockName, 0);
            this.Controls.SetChildIndex(this.lbExams, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.btnExamUnitAdd, 0);
            this.Controls.SetChildIndex(this.btnExamUnitDelete, 0);
            ((System.ComponentModel.ISupportInitialize)(this.epError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chbIsProfil;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chbToAllStudyBasis;
        private System.Windows.Forms.CheckBox chbCrimea;
        private System.Windows.Forms.CheckBox chbGosLine;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbOrderNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbBlockName;
        private System.Windows.Forms.ListBox lbExams;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnExamUnitAdd;
        private System.Windows.Forms.Button btnExamUnitDelete;
    }
}