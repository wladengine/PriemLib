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
            this.dtpDateOfClose = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.tbExamAddress = new System.Windows.Forms.TextBox();
            this.lbExamTimeTableRestriction = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbBaseExamTimeTable = new System.Windows.Forms.ComboBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvExamBaseTimetable = new System.Windows.Forms.DataGridView();
            this.label7 = new System.Windows.Forms.Label();
            this.btnExaminEntryBlockUnitTTAdd = new System.Windows.Forms.Button();
            this.cbExamInEntryBlockUnitTimetable = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cbSubject = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExamBaseTimetable)).BeginInit();
            this.SuspendLayout();
            // 
            // cbUnitList
            // 
            this.cbUnitList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.cbUnitList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUnitList.FormattingEnabled = true;
            this.cbUnitList.Location = new System.Drawing.Point(12, 12);
            this.cbUnitList.Name = "cbUnitList";
            this.cbUnitList.Size = new System.Drawing.Size(466, 21);
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
            this.dgv.Location = new System.Drawing.Point(12, 47);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(466, 257);
            this.dgv.TabIndex = 1;
            // 
            // dtpExamDate
            // 
            this.dtpExamDate.CustomFormat = "dd MMMM yyyy HH:mm";
            this.dtpExamDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpExamDate.Location = new System.Drawing.Point(109, 208);
            this.dtpExamDate.Name = "dtpExamDate";
            this.dtpExamDate.Size = new System.Drawing.Size(178, 20);
            this.dtpExamDate.TabIndex = 2;
            // 
            // dtpDateOfClose
            // 
            this.dtpDateOfClose.CustomFormat = "dd MMMM yyyy HH:mm";
            this.dtpDateOfClose.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateOfClose.Location = new System.Drawing.Point(189, 317);
            this.dtpDateOfClose.Name = "dtpDateOfClose";
            this.dtpDateOfClose.Size = new System.Drawing.Size(178, 20);
            this.dtpDateOfClose.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 214);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Дата экзамена";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 231);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Место проведения";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 323);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(163, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Время окончания регистрации";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Location = new System.Drawing.Point(189, 473);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(127, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Обновить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.Location = new System.Drawing.Point(16, 510);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(127, 23);
            this.btnAddNew.TabIndex = 6;
            this.btnAddNew.Text = "Добавить новый";
            this.btnAddNew.UseVisualStyleBackColor = true;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // tbExamAddress
            // 
            this.tbExamAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbExamAddress.Location = new System.Drawing.Point(16, 247);
            this.tbExamAddress.Multiline = true;
            this.tbExamAddress.Name = "tbExamAddress";
            this.tbExamAddress.Size = new System.Drawing.Size(450, 64);
            this.tbExamAddress.TabIndex = 9;
            // 
            // lbExamTimeTableRestriction
            // 
            this.lbExamTimeTableRestriction.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbExamTimeTableRestriction.Enabled = false;
            this.lbExamTimeTableRestriction.FormattingEnabled = true;
            this.lbExamTimeTableRestriction.HorizontalScrollbar = true;
            this.lbExamTimeTableRestriction.Location = new System.Drawing.Point(15, 398);
            this.lbExamTimeTableRestriction.Name = "lbExamTimeTableRestriction";
            this.lbExamTimeTableRestriction.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbExamTimeTableRestriction.Size = new System.Drawing.Size(450, 69);
            this.lbExamTimeTableRestriction.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 382);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(139, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Недопустимые дни сдачи:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 342);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Основной день сдачи:";
            // 
            // cbBaseExamTimeTable
            // 
            this.cbBaseExamTimeTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbBaseExamTimeTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBaseExamTimeTable.Enabled = false;
            this.cbBaseExamTimeTable.FormattingEnabled = true;
            this.cbBaseExamTimeTable.Location = new System.Drawing.Point(16, 358);
            this.cbBaseExamTimeTable.Name = "cbBaseExamTimeTable";
            this.cbBaseExamTimeTable.Size = new System.Drawing.Size(450, 21);
            this.cbBaseExamTimeTable.TabIndex = 6;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Location = new System.Drawing.Point(351, 310);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(127, 23);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "Удалить выбранный";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cbSubject);
            this.groupBox2.Controls.Add(this.dgvExamBaseTimetable);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.btnSave);
            this.groupBox2.Controls.Add(this.btnAddNew);
            this.groupBox2.Controls.Add(this.lbExamTimeTableRestriction);
            this.groupBox2.Controls.Add(this.tbExamAddress);
            this.groupBox2.Controls.Add(this.dtpExamDate);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cbBaseExamTimeTable);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.dtpDateOfClose);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(487, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(472, 550);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Редактирование расписания сдачи предмета";
            // 
            // dgvExamBaseTimetable
            // 
            this.dgvExamBaseTimetable.AllowUserToAddRows = false;
            this.dgvExamBaseTimetable.AllowUserToDeleteRows = false;
            this.dgvExamBaseTimetable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvExamBaseTimetable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvExamBaseTimetable.Location = new System.Drawing.Point(17, 69);
            this.dgvExamBaseTimetable.Name = "dgvExamBaseTimetable";
            this.dgvExamBaseTimetable.ReadOnly = true;
            this.dgvExamBaseTimetable.Size = new System.Drawing.Size(447, 125);
            this.dgvExamBaseTimetable.TabIndex = 11;
            this.dgvExamBaseTimetable.CurrentCellChanged += new System.EventHandler(this.dgvExamBaseTimetable_CurrentCellChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 494);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(143, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Создать новый день сдачи";
            // 
            // btnExaminEntryBlockUnitTTAdd
            // 
            this.btnExaminEntryBlockUnitTTAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExaminEntryBlockUnitTTAdd.Location = new System.Drawing.Point(345, 413);
            this.btnExaminEntryBlockUnitTTAdd.Name = "btnExaminEntryBlockUnitTTAdd";
            this.btnExaminEntryBlockUnitTTAdd.Size = new System.Drawing.Size(127, 23);
            this.btnExaminEntryBlockUnitTTAdd.TabIndex = 10;
            this.btnExaminEntryBlockUnitTTAdd.Text = "Добавить";
            this.btnExaminEntryBlockUnitTTAdd.UseVisualStyleBackColor = true;
            // 
            // cbExamInEntryBlockUnitTimetable
            // 
            this.cbExamInEntryBlockUnitTimetable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbExamInEntryBlockUnitTimetable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbExamInEntryBlockUnitTimetable.FormattingEnabled = true;
            this.cbExamInEntryBlockUnitTimetable.Location = new System.Drawing.Point(12, 386);
            this.cbExamInEntryBlockUnitTimetable.Name = "cbExamInEntryBlockUnitTimetable";
            this.cbExamInEntryBlockUnitTimetable.Size = new System.Drawing.Size(460, 21);
            this.cbExamInEntryBlockUnitTimetable.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 370);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(210, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Добавить в расписание экзамена дату:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Предмет";
            // 
            // cbSubject
            // 
            this.cbSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSubject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSubject.Enabled = false;
            this.cbSubject.FormattingEnabled = true;
            this.cbSubject.Location = new System.Drawing.Point(15, 39);
            this.cbSubject.Name = "cbSubject";
            this.cbSubject.Size = new System.Drawing.Size(450, 21);
            this.cbSubject.TabIndex = 0;
            this.cbSubject.SelectedIndexChanged += new System.EventHandler(this.cbUnitList_SelectedIndexChanged);
            // 
            // CardExamTimeTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(998, 562);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbExamInEntryBlockUnitTimetable);
            this.Controls.Add(this.btnExaminEntryBlockUnitTTAdd);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cbUnitList);
            this.Name = "CardExamTimeTable";
            this.Text = "ExamTimeTable";
            this.Shown += new System.EventHandler(this.CardExamTimeTable_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExamBaseTimetable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbUnitList;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.DateTimePicker dtpExamDate;
        private System.Windows.Forms.DateTimePicker dtpDateOfClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbBaseExamTimeTable;
        private System.Windows.Forms.ListBox lbExamTimeTableRestriction;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbExamAddress;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvExamBaseTimetable;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnExaminEntryBlockUnitTTAdd;
        private System.Windows.Forms.ComboBox cbExamInEntryBlockUnitTimetable;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbSubject;
        private System.Windows.Forms.Label label8;
    }
}