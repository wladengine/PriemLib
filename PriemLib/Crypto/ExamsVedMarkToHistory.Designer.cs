namespace PriemLib
{
    partial class ExamsVedMarkToHistory
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
            this.label1 = new System.Windows.Forms.Label();
            this.cbFaculty = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbExam = new System.Windows.Forms.ComboBox();
            this.btnUpdateMarks = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cbExamVed = new System.Windows.Forms.ComboBox();
            this.lblPersonCount = new System.Windows.Forms.Label();
            this.cbStudyLevelGroup = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbStudyBasis = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lbmarktype = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Факультет";
            // 
            // cbFaculty
            // 
            this.cbFaculty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFaculty.FormattingEnabled = true;
            this.cbFaculty.Location = new System.Drawing.Point(80, 50);
            this.cbFaculty.Name = "cbFaculty";
            this.cbFaculty.Size = new System.Drawing.Size(185, 21);
            this.cbFaculty.TabIndex = 1;
            this.cbFaculty.SelectedIndexChanged += new System.EventHandler(this.cbFaculty_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Экзамен";
            // 
            // cbExam
            // 
            this.cbExam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbExam.FormattingEnabled = true;
            this.cbExam.Location = new System.Drawing.Point(80, 77);
            this.cbExam.Name = "cbExam";
            this.cbExam.Size = new System.Drawing.Size(445, 21);
            this.cbExam.TabIndex = 1;
            this.cbExam.SelectedIndexChanged += new System.EventHandler(this.cbExam_SelectedIndexChanged);
            // 
            // btnUpdateMarks
            // 
            this.btnUpdateMarks.Location = new System.Drawing.Point(312, 211);
            this.btnUpdateMarks.Name = "btnUpdateMarks";
            this.btnUpdateMarks.Size = new System.Drawing.Size(175, 23);
            this.btnUpdateMarks.TabIndex = 2;
            this.btnUpdateMarks.Text = "Перенести результаты";
            this.btnUpdateMarks.UseVisualStyleBackColor = true;
            this.btnUpdateMarks.Click += new System.EventHandler(this.btnUpdateMarks_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Ведомость";
            // 
            // cbExamVed
            // 
            this.cbExamVed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbExamVed.FormattingEnabled = true;
            this.cbExamVed.Location = new System.Drawing.Point(80, 104);
            this.cbExamVed.Name = "cbExamVed";
            this.cbExamVed.Size = new System.Drawing.Size(445, 21);
            this.cbExamVed.TabIndex = 1;
            this.cbExamVed.SelectedIndexChanged += new System.EventHandler(this.cbExamVed_SelectedIndexChanged);
            // 
            // lblPersonCount
            // 
            this.lblPersonCount.Location = new System.Drawing.Point(80, 169);
            this.lblPersonCount.Name = "lblPersonCount";
            this.lblPersonCount.Size = new System.Drawing.Size(445, 39);
            this.lblPersonCount.TabIndex = 3;
            this.lblPersonCount.Text = " ";
            // 
            // cbStudyLevelGroup
            // 
            this.cbStudyLevelGroup.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbStudyLevelGroup.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbStudyLevelGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStudyLevelGroup.DropDownWidth = 350;
            this.cbStudyLevelGroup.FormattingEnabled = true;
            this.cbStudyLevelGroup.Location = new System.Drawing.Point(80, 9);
            this.cbStudyLevelGroup.Name = "cbStudyLevelGroup";
            this.cbStudyLevelGroup.Size = new System.Drawing.Size(445, 21);
            this.cbStudyLevelGroup.TabIndex = 149;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 148;
            this.label4.Text = "Уровень";
            // 
            // cbStudyBasis
            // 
            this.cbStudyBasis.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbStudyBasis.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbStudyBasis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStudyBasis.FormattingEnabled = true;
            this.cbStudyBasis.Location = new System.Drawing.Point(389, 50);
            this.cbStudyBasis.Name = "cbStudyBasis";
            this.cbStudyBasis.Size = new System.Drawing.Size(136, 21);
            this.cbStudyBasis.TabIndex = 151;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(289, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 13);
            this.label5.TabIndex = 150;
            this.label5.Text = "Основа обучения";
            // 
            // lbmarktype
            // 
            this.lbmarktype.FormattingEnabled = true;
            this.lbmarktype.Location = new System.Drawing.Point(534, 35);
            this.lbmarktype.Name = "lbmarktype";
            this.lbmarktype.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbmarktype.Size = new System.Drawing.Size(176, 199);
            this.lbmarktype.TabIndex = 152;
            this.lbmarktype.SelectedIndexChanged += new System.EventHandler(this.lbmarktype_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(531, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 13);
            this.label6.TabIndex = 153;
            this.label6.Text = "Категории оценок";
            // 
            // ExamsVedMarkToHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 255);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lbmarktype);
            this.Controls.Add(this.cbStudyBasis);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbStudyLevelGroup);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblPersonCount);
            this.Controls.Add(this.btnUpdateMarks);
            this.Controls.Add(this.cbExamVed);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbExam);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbFaculty);
            this.Controls.Add(this.label1);
            this.Name = "ExamsVedMarkToHistory";
            this.Text = "Перенос оценок ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbFaculty;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbExam;
        private System.Windows.Forms.Button btnUpdateMarks;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbExamVed;
        private System.Windows.Forms.Label lblPersonCount;
        public System.Windows.Forms.ComboBox cbStudyLevelGroup;
        protected System.Windows.Forms.Label label4;
        protected System.Windows.Forms.ComboBox cbStudyBasis;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox lbmarktype;
        private System.Windows.Forms.Label label6;
    }
}