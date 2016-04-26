namespace PriemLib
{
    partial class CardExamTimeTable
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
            this.cbUnitList = new System.Windows.Forms.ComboBox();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.dtpExamDate = new System.Windows.Forms.DateTimePicker();
            this.tbExamAddress = new System.Windows.Forms.TextBox();
            this.dtpDateOfClose = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbBaseExamTimeTable = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbExamTimeTableRestriction = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbUnitList
            // 
            this.cbUnitList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUnitList.FormattingEnabled = true;
            this.cbUnitList.Location = new System.Drawing.Point(12, 12);
            this.cbUnitList.Name = "cbUnitList";
            this.cbUnitList.Size = new System.Drawing.Size(494, 21);
            this.cbUnitList.TabIndex = 0;
            this.cbUnitList.SelectedIndexChanged += new System.EventHandler(this.cbUnitList_SelectedIndexChanged);
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(12, 39);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(494, 158);
            this.dgv.TabIndex = 1;
            this.dgv.CurrentCellChanged += new System.EventHandler(this.dgv_CurrentCellChanged);
            // 
            // dtpExamDate
            // 
            this.dtpExamDate.CustomFormat = "dd MMMM yyyy HH:mm:ss";
            this.dtpExamDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpExamDate.Location = new System.Drawing.Point(124, 16);
            this.dtpExamDate.Name = "dtpExamDate";
            this.dtpExamDate.Size = new System.Drawing.Size(351, 20);
            this.dtpExamDate.TabIndex = 2;
            // 
            // tbExamAddress
            // 
            this.tbExamAddress.Location = new System.Drawing.Point(124, 42);
            this.tbExamAddress.Multiline = true;
            this.tbExamAddress.Name = "tbExamAddress";
            this.tbExamAddress.Size = new System.Drawing.Size(351, 64);
            this.tbExamAddress.TabIndex = 3;
            // 
            // dtpDateOfClose
            // 
            this.dtpDateOfClose.CustomFormat = "dd MMMM yyyy HH:mm:ss";
            this.dtpDateOfClose.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateOfClose.Location = new System.Drawing.Point(185, 112);
            this.dtpDateOfClose.Name = "dtpDateOfClose";
            this.dtpDateOfClose.Size = new System.Drawing.Size(290, 20);
            this.dtpDateOfClose.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Дата экзамена";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Место проведения";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(163, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Время окончания регистрации";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(6, 221);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(127, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Обновить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddNew.Location = new System.Drawing.Point(379, 203);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(127, 23);
            this.btnAddNew.TabIndex = 6;
            this.btnAddNew.Text = "Добавить новый";
            this.btnAddNew.UseVisualStyleBackColor = true;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.lbExamTimeTableRestriction);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cbBaseExamTimeTable);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbExamAddress);
            this.groupBox1.Controls.Add(this.dtpDateOfClose);
            this.groupBox1.Controls.Add(this.dtpExamDate);
            this.groupBox1.Location = new System.Drawing.Point(16, 232);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(490, 250);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Экзамен";
            // 
            // cbBaseExamTimeTable
            // 
            this.cbBaseExamTimeTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBaseExamTimeTable.FormattingEnabled = true;
            this.cbBaseExamTimeTable.Location = new System.Drawing.Point(167, 145);
            this.cbBaseExamTimeTable.Name = "cbBaseExamTimeTable";
            this.cbBaseExamTimeTable.Size = new System.Drawing.Size(308, 21);
            this.cbBaseExamTimeTable.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(42, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Основной день сдачи:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 175);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(139, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Недопустимые дни сдачи:";
            // 
            // lbExamTimeTableRestriction
            // 
            this.lbExamTimeTableRestriction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbExamTimeTableRestriction.FormattingEnabled = true;
            this.lbExamTimeTableRestriction.Location = new System.Drawing.Point(170, 175);
            this.lbExamTimeTableRestriction.Name = "lbExamTimeTableRestriction";
            this.lbExamTimeTableRestriction.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbExamTimeTableRestriction.Size = new System.Drawing.Size(304, 69);
            this.lbExamTimeTableRestriction.TabIndex = 8;
            // 
            // CardExamTimeTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 494);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnAddNew);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.cbUnitList);
            this.MaximumSize = new System.Drawing.Size(534, 999);
            this.Name = "CardExamTimeTable";
            this.Text = "ExamTimeTable";
            this.Load += new System.EventHandler(this.CardExamTimeTable_Load);
            this.Shown += new System.EventHandler(this.CardExamTimeTable_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbUnitList;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.DateTimePicker dtpExamDate;
        private System.Windows.Forms.TextBox tbExamAddress;
        private System.Windows.Forms.DateTimePicker dtpDateOfClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbBaseExamTimeTable;
        private System.Windows.Forms.ListBox lbExamTimeTableRestriction;
        private System.Windows.Forms.Label label5;
    }
}