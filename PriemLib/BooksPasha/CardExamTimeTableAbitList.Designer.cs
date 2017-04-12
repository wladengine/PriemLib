namespace PriemLib
{
    partial class CardExamTimeTableAbitList
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
            this.cbStudyLevel = new System.Windows.Forms.ComboBox();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.cbLicenseProgram = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbObrazProgram = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbExamenBlock = new System.Windows.Forms.ComboBox();
            this.cbExamenUnit = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbExamTimeTable = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnFillGrid = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lblCount2 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.btnFillNotRegistered = new System.Windows.Forms.Button();
            this.dgvNotRegistered = new System.Windows.Forms.DataGridView();
            this.chbHideObProgram = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotRegistered)).BeginInit();
            this.SuspendLayout();
            // 
            // cbStudyLevel
            // 
            this.cbStudyLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbStudyLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStudyLevel.FormattingEnabled = true;
            this.cbStudyLevel.Location = new System.Drawing.Point(160, 6);
            this.cbStudyLevel.Name = "cbStudyLevel";
            this.cbStudyLevel.Size = new System.Drawing.Size(598, 21);
            this.cbStudyLevel.TabIndex = 0;
            this.cbStudyLevel.SelectedIndexChanged += new System.EventHandler(this.cbStudyLevel_SelectedIndexChanged);
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(11, 249);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.Size = new System.Drawing.Size(747, 249);
            this.dgv.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(103, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Уровень";
            // 
            // cbLicenseProgram
            // 
            this.cbLicenseProgram.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLicenseProgram.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLicenseProgram.FormattingEnabled = true;
            this.cbLicenseProgram.Location = new System.Drawing.Point(160, 33);
            this.cbLicenseProgram.Name = "cbLicenseProgram";
            this.cbLicenseProgram.Size = new System.Drawing.Size(598, 21);
            this.cbLicenseProgram.TabIndex = 0;
            this.cbLicenseProgram.SelectedIndexChanged += new System.EventHandler(this.cbLicenseProgram_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(79, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Направление";
            // 
            // cbObrazProgram
            // 
            this.cbObrazProgram.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbObrazProgram.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbObrazProgram.FormattingEnabled = true;
            this.cbObrazProgram.Location = new System.Drawing.Point(160, 60);
            this.cbObrazProgram.Name = "cbObrazProgram";
            this.cbObrazProgram.Size = new System.Drawing.Size(598, 21);
            this.cbObrazProgram.TabIndex = 0;
            this.cbObrazProgram.SelectedIndexChanged += new System.EventHandler(this.cbObrazProgram_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(55, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Образ.программа";
            // 
            // cbExamenBlock
            // 
            this.cbExamenBlock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbExamenBlock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbExamenBlock.FormattingEnabled = true;
            this.cbExamenBlock.Location = new System.Drawing.Point(160, 102);
            this.cbExamenBlock.Name = "cbExamenBlock";
            this.cbExamenBlock.Size = new System.Drawing.Size(598, 21);
            this.cbExamenBlock.TabIndex = 0;
            this.cbExamenBlock.SelectedIndexChanged += new System.EventHandler(this.cbExamenBlock_SelectedIndexChanged);
            // 
            // cbExamenUnit
            // 
            this.cbExamenUnit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbExamenUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbExamenUnit.FormattingEnabled = true;
            this.cbExamenUnit.Location = new System.Drawing.Point(160, 129);
            this.cbExamenUnit.Name = "cbExamenUnit";
            this.cbExamenUnit.Size = new System.Drawing.Size(598, 21);
            this.cbExamenUnit.TabIndex = 0;
            this.cbExamenUnit.SelectedIndexChanged += new System.EventHandler(this.cbExamenUnit_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(53, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Группа экзаменов";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(102, 132);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Экзамен";
            // 
            // cbExamTimeTable
            // 
            this.cbExamTimeTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbExamTimeTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbExamTimeTable.FormattingEnabled = true;
            this.cbExamTimeTable.Location = new System.Drawing.Point(160, 156);
            this.cbExamTimeTable.Name = "cbExamTimeTable";
            this.cbExamTimeTable.Size = new System.Drawing.Size(598, 21);
            this.cbExamTimeTable.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 159);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(146, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Место и время проведения";
            // 
            // btnFillGrid
            // 
            this.btnFillGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFillGrid.Location = new System.Drawing.Point(643, 220);
            this.btnFillGrid.Name = "btnFillGrid";
            this.btnFillGrid.Size = new System.Drawing.Size(115, 23);
            this.btnFillGrid.TabIndex = 4;
            this.btnFillGrid.Text = "Обновить";
            this.btnFillGrid.UseVisualStyleBackColor = true;
            this.btnFillGrid.Click += new System.EventHandler(this.btnFillGrid_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(11, 220);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(115, 23);
            this.btnPrint.TabIndex = 4;
            this.btnPrint.Text = "Распечатать";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.label4.Location = new System.Drawing.Point(631, 187);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(127, 30);
            this.label4.TabIndex = 5;
            this.label4.Text = "Нажмите \"Обновить\" для построения списка";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(640, 501);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Количество:";
            // 
            // lblCount
            // 
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(715, 501);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(13, 13);
            this.lblCount.TabIndex = 7;
            this.lblCount.Text = "0";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(774, 549);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chbHideObProgram);
            this.tabPage1.Controls.Add(this.lblCount);
            this.tabPage1.Controls.Add(this.cbStudyLevel);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.cbObrazProgram);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.cbLicenseProgram);
            this.tabPage1.Controls.Add(this.btnPrint);
            this.tabPage1.Controls.Add(this.cbExamenBlock);
            this.tabPage1.Controls.Add(this.btnFillGrid);
            this.tabPage1.Controls.Add(this.cbExamenUnit);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.cbExamTimeTable);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.dgv);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(766, 523);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Зарегистированные";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lblCount2);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.checkBox1);
            this.tabPage2.Controls.Add(this.btnFillNotRegistered);
            this.tabPage2.Controls.Add(this.dgvNotRegistered);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(766, 523);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Не зарегистрированные";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lblCount2
            // 
            this.lblCount2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCount2.AutoSize = true;
            this.lblCount2.Location = new System.Drawing.Point(679, 500);
            this.lblCount2.Name = "lblCount2";
            this.lblCount2.Size = new System.Drawing.Size(13, 13);
            this.lblCount2.TabIndex = 11;
            this.lblCount2.Text = "0";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(604, 500);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(69, 13);
            this.label10.TabIndex = 10;
            this.label10.Text = "Количество:";
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(13, 500);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(225, 17);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "Скрывать Русский язык для 10 класса";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // btnFillNotRegistered
            // 
            this.btnFillNotRegistered.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFillNotRegistered.Location = new System.Drawing.Point(645, 8);
            this.btnFillNotRegistered.Name = "btnFillNotRegistered";
            this.btnFillNotRegistered.Size = new System.Drawing.Size(115, 23);
            this.btnFillNotRegistered.TabIndex = 6;
            this.btnFillNotRegistered.Text = "Обновить";
            this.btnFillNotRegistered.UseVisualStyleBackColor = true;
            this.btnFillNotRegistered.Click += new System.EventHandler(this.btnFillNotRegistered_Click);
            // 
            // dgvNotRegistered
            // 
            this.dgvNotRegistered.AllowUserToAddRows = false;
            this.dgvNotRegistered.AllowUserToDeleteRows = false;
            this.dgvNotRegistered.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvNotRegistered.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvNotRegistered.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNotRegistered.Location = new System.Drawing.Point(13, 37);
            this.dgvNotRegistered.Name = "dgvNotRegistered";
            this.dgvNotRegistered.ReadOnly = true;
            this.dgvNotRegistered.Size = new System.Drawing.Size(747, 440);
            this.dgvNotRegistered.TabIndex = 5;
            // 
            // chbHideObProgram
            // 
            this.chbHideObProgram.AutoSize = true;
            this.chbHideObProgram.Checked = true;
            this.chbHideObProgram.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbHideObProgram.Location = new System.Drawing.Point(514, 226);
            this.chbHideObProgram.Name = "chbHideObProgram";
            this.chbHideObProgram.Size = new System.Drawing.Size(123, 17);
            this.chbHideObProgram.TabIndex = 8;
            this.chbHideObProgram.Text = "Скрывать профиль";
            this.chbHideObProgram.UseVisualStyleBackColor = true;
            // 
            // CardExamTimeTableAbitList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 559);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(586, 582);
            this.Name = "CardExamTimeTableAbitList";
            this.Text = "ExamTimeTable";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotRegistered)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbStudyLevel;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbLicenseProgram;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbObrazProgram;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbExamenBlock;
        private System.Windows.Forms.ComboBox cbExamenUnit;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbExamTimeTable;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnFillGrid;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnFillNotRegistered;
        private System.Windows.Forms.DataGridView dgvNotRegistered;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label lblCount2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chbHideObProgram;
    }
}