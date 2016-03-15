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
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // cbStudyLevel
            // 
            this.cbStudyLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbStudyLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStudyLevel.FormattingEnabled = true;
            this.cbStudyLevel.Location = new System.Drawing.Point(167, 12);
            this.cbStudyLevel.Name = "cbStudyLevel";
            this.cbStudyLevel.Size = new System.Drawing.Size(616, 21);
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
            this.dgv.Location = new System.Drawing.Point(12, 255);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.Size = new System.Drawing.Size(771, 264);
            this.dgv.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(110, 15);
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
            this.cbLicenseProgram.Location = new System.Drawing.Point(167, 39);
            this.cbLicenseProgram.Name = "cbLicenseProgram";
            this.cbLicenseProgram.Size = new System.Drawing.Size(616, 21);
            this.cbLicenseProgram.TabIndex = 0;
            this.cbLicenseProgram.SelectedIndexChanged += new System.EventHandler(this.cbLicenseProgram_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(86, 42);
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
            this.cbObrazProgram.Location = new System.Drawing.Point(167, 66);
            this.cbObrazProgram.Name = "cbObrazProgram";
            this.cbObrazProgram.Size = new System.Drawing.Size(616, 21);
            this.cbObrazProgram.TabIndex = 0;
            this.cbObrazProgram.SelectedIndexChanged += new System.EventHandler(this.cbObrazProgram_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(62, 69);
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
            this.cbExamenBlock.Location = new System.Drawing.Point(167, 108);
            this.cbExamenBlock.Name = "cbExamenBlock";
            this.cbExamenBlock.Size = new System.Drawing.Size(616, 21);
            this.cbExamenBlock.TabIndex = 0;
            this.cbExamenBlock.SelectedIndexChanged += new System.EventHandler(this.cbExamenBlock_SelectedIndexChanged);
            // 
            // cbExamenUnit
            // 
            this.cbExamenUnit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbExamenUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbExamenUnit.FormattingEnabled = true;
            this.cbExamenUnit.Location = new System.Drawing.Point(167, 135);
            this.cbExamenUnit.Name = "cbExamenUnit";
            this.cbExamenUnit.Size = new System.Drawing.Size(616, 21);
            this.cbExamenUnit.TabIndex = 0;
            this.cbExamenUnit.SelectedIndexChanged += new System.EventHandler(this.cbExamenUnit_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(60, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Группа экзаменов";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(109, 138);
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
            this.cbExamTimeTable.Location = new System.Drawing.Point(167, 162);
            this.cbExamTimeTable.Name = "cbExamTimeTable";
            this.cbExamTimeTable.Size = new System.Drawing.Size(616, 21);
            this.cbExamTimeTable.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 165);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(146, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Место и время проведения";
            // 
            // btnFillGrid
            // 
            this.btnFillGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFillGrid.Location = new System.Drawing.Point(668, 226);
            this.btnFillGrid.Name = "btnFillGrid";
            this.btnFillGrid.Size = new System.Drawing.Size(115, 23);
            this.btnFillGrid.TabIndex = 4;
            this.btnFillGrid.Text = "Обновить";
            this.btnFillGrid.UseVisualStyleBackColor = true;
            this.btnFillGrid.Click += new System.EventHandler(this.btnFillGrid_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(12, 226);
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
            this.label4.Location = new System.Drawing.Point(656, 193);
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
            this.label8.Location = new System.Drawing.Point(621, 522);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Количество:";
            // 
            // lblCount
            // 
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(696, 522);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(13, 13);
            this.lblCount.TabIndex = 7;
            this.lblCount.Text = "0";
            // 
            // CardExamTimeTableAbitList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 544);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnFillGrid);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.cbExamTimeTable);
            this.Controls.Add(this.cbExamenUnit);
            this.Controls.Add(this.cbExamenBlock);
            this.Controls.Add(this.cbLicenseProgram);
            this.Controls.Add(this.cbObrazProgram);
            this.Controls.Add(this.cbStudyLevel);
            this.MinimumSize = new System.Drawing.Size(586, 582);
            this.Name = "CardExamTimeTableAbitList";
            this.Text = "ExamTimeTable";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}