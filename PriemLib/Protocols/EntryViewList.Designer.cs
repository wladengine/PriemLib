﻿namespace PriemLib
{
    partial class EntryViewList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvViews = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnPrintOrder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancelView = new System.Windows.Forms.Button();
            this.chbIsForeign = new System.Windows.Forms.CheckBox();
            this.btnOrderReview = new System.Windows.Forms.Button();
            this.chbIsListener = new System.Windows.Forms.CheckBox();
            this.chbIsSecond = new System.Windows.Forms.CheckBox();
            this.chbIsReduced = new System.Windows.Forms.CheckBox();
            this.chbIsParallel = new System.Windows.Forms.CheckBox();
            this.cbStudyForm = new System.Windows.Forms.ComboBox();
            this.cbStudyBasis = new System.Windows.Forms.ComboBox();
            this.cbLicenseProgram = new System.Windows.Forms.ComboBox();
            this.cbFaculty = new System.Windows.Forms.ComboBox();
            this.chbCel = new System.Windows.Forms.CheckBox();
            this.cbStudyLevelGroup = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.gbProtocolInfo = new System.Windows.Forms.GroupBox();
            this.lblProtocolCompetitions = new System.Windows.Forms.Label();
            this.lblProtocolPersonsCount = new System.Windows.Forms.Label();
            this.lblHasForeigners = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSetAdmissionProtocol = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cbAdmissionProtocol = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvViews)).BeginInit();
            this.gbProtocolInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvViews
            // 
            this.dgvViews.AllowUserToAddRows = false;
            this.dgvViews.AllowUserToDeleteRows = false;
            this.dgvViews.AllowUserToResizeRows = false;
            this.dgvViews.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvViews.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvViews.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvViews.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvViews.Location = new System.Drawing.Point(12, 170);
            this.dgvViews.Name = "dgvViews";
            this.dgvViews.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvViews.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvViews.Size = new System.Drawing.Size(647, 246);
            this.dgvViews.TabIndex = 84;
            this.dgvViews.SelectionChanged += new System.EventHandler(this.dgvViews_SelectionChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 82;
            this.label2.Text = "Факультет";
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(19, 69);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(94, 13);
            this.lblLanguage.TabIndex = 80;
            this.lblLanguage.Text = "Основа обучения";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 78;
            this.label3.Text = "Форма обучения";
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrint.Location = new System.Drawing.Point(12, 422);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(121, 40);
            this.btnPrint.TabIndex = 77;
            this.btnPrint.Text = "Печать представления";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreate.Location = new System.Drawing.Point(558, 422);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(101, 40);
            this.btnCreate.TabIndex = 85;
            this.btnCreate.Text = "Создать представление";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnPrintOrder
            // 
            this.btnPrintOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrintOrder.Location = new System.Drawing.Point(12, 518);
            this.btnPrintOrder.Name = "btnPrintOrder";
            this.btnPrintOrder.Size = new System.Drawing.Size(121, 40);
            this.btnPrintOrder.TabIndex = 77;
            this.btnPrintOrder.Text = "Печать приказа";
            this.btnPrintOrder.UseVisualStyleBackColor = true;
            this.btnPrintOrder.Click += new System.EventHandler(this.btnPrintOrder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 86;
            this.label1.Text = "Направление";
            // 
            // btnCancelView
            // 
            this.btnCancelView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelView.Location = new System.Drawing.Point(558, 465);
            this.btnCancelView.Name = "btnCancelView";
            this.btnCancelView.Size = new System.Drawing.Size(101, 43);
            this.btnCancelView.TabIndex = 88;
            this.btnCancelView.Text = "Отменить представление";
            this.btnCancelView.UseVisualStyleBackColor = true;
            this.btnCancelView.Click += new System.EventHandler(this.btnCancelView_Click);
            // 
            // chbIsForeign
            // 
            this.chbIsForeign.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chbIsForeign.Location = new System.Drawing.Point(139, 524);
            this.chbIsForeign.Name = "chbIsForeign";
            this.chbIsForeign.Size = new System.Drawing.Size(94, 31);
            this.chbIsForeign.TabIndex = 89;
            this.chbIsForeign.Text = "для иностранцев";
            this.chbIsForeign.UseVisualStyleBackColor = true;
            // 
            // btnOrderReview
            // 
            this.btnOrderReview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOrderReview.Location = new System.Drawing.Point(12, 468);
            this.btnOrderReview.Name = "btnOrderReview";
            this.btnOrderReview.Size = new System.Drawing.Size(121, 40);
            this.btnOrderReview.TabIndex = 77;
            this.btnOrderReview.Text = "Печать выписки";
            this.btnOrderReview.UseVisualStyleBackColor = true;
            this.btnOrderReview.Click += new System.EventHandler(this.btnOrderReview_Click);
            // 
            // chbIsListener
            // 
            this.chbIsListener.AutoSize = true;
            this.chbIsListener.Location = new System.Drawing.Point(119, 147);
            this.chbIsListener.Name = "chbIsListener";
            this.chbIsListener.Size = new System.Drawing.Size(80, 17);
            this.chbIsListener.TabIndex = 90;
            this.chbIsListener.Text = "слушатели";
            this.chbIsListener.UseVisualStyleBackColor = true;
            this.chbIsListener.CheckedChanged += new System.EventHandler(this.chbIsListener_CheckedChanged);
            // 
            // chbIsSecond
            // 
            this.chbIsSecond.AutoSize = true;
            this.chbIsSecond.Location = new System.Drawing.Point(469, 68);
            this.chbIsSecond.Name = "chbIsSecond";
            this.chbIsSecond.Size = new System.Drawing.Size(137, 17);
            this.chbIsSecond.TabIndex = 149;
            this.chbIsSecond.Text = "для лиц, имеющих ВО";
            this.chbIsSecond.UseVisualStyleBackColor = true;
            this.chbIsSecond.CheckedChanged += new System.EventHandler(this.chbIsSecond_CheckedChanged);
            // 
            // chbIsReduced
            // 
            this.chbIsReduced.AutoSize = true;
            this.chbIsReduced.Location = new System.Drawing.Point(348, 68);
            this.chbIsReduced.Name = "chbIsReduced";
            this.chbIsReduced.Size = new System.Drawing.Size(95, 17);
            this.chbIsReduced.TabIndex = 148;
            this.chbIsReduced.Text = "сокращенная";
            this.chbIsReduced.UseVisualStyleBackColor = true;
            this.chbIsReduced.CheckedChanged += new System.EventHandler(this.chbIsReduced_CheckedChanged);
            // 
            // chbIsParallel
            // 
            this.chbIsParallel.AutoSize = true;
            this.chbIsParallel.Location = new System.Drawing.Point(348, 91);
            this.chbIsParallel.Name = "chbIsParallel";
            this.chbIsParallel.Size = new System.Drawing.Size(98, 17);
            this.chbIsParallel.TabIndex = 147;
            this.chbIsParallel.Text = "параллельная";
            this.chbIsParallel.UseVisualStyleBackColor = true;
            this.chbIsParallel.CheckedChanged += new System.EventHandler(this.chbIsParallel_CheckedChanged);
            // 
            // cbStudyForm
            // 
            this.cbStudyForm.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbStudyForm.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbStudyForm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStudyForm.FormattingEnabled = true;
            this.cbStudyForm.Location = new System.Drawing.Point(119, 93);
            this.cbStudyForm.Name = "cbStudyForm";
            this.cbStudyForm.Size = new System.Drawing.Size(202, 21);
            this.cbStudyForm.TabIndex = 144;
            // 
            // cbStudyBasis
            // 
            this.cbStudyBasis.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbStudyBasis.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbStudyBasis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStudyBasis.FormattingEnabled = true;
            this.cbStudyBasis.Location = new System.Drawing.Point(119, 66);
            this.cbStudyBasis.Name = "cbStudyBasis";
            this.cbStudyBasis.Size = new System.Drawing.Size(202, 21);
            this.cbStudyBasis.TabIndex = 143;
            // 
            // cbLicenseProgram
            // 
            this.cbLicenseProgram.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbLicenseProgram.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbLicenseProgram.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLicenseProgram.FormattingEnabled = true;
            this.cbLicenseProgram.Location = new System.Drawing.Point(119, 120);
            this.cbLicenseProgram.Name = "cbLicenseProgram";
            this.cbLicenseProgram.Size = new System.Drawing.Size(540, 21);
            this.cbLicenseProgram.TabIndex = 142;
            // 
            // cbFaculty
            // 
            this.cbFaculty.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbFaculty.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbFaculty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFaculty.FormattingEnabled = true;
            this.cbFaculty.Location = new System.Drawing.Point(119, 39);
            this.cbFaculty.Name = "cbFaculty";
            this.cbFaculty.Size = new System.Drawing.Size(540, 21);
            this.cbFaculty.TabIndex = 141;
            // 
            // chbCel
            // 
            this.chbCel.AutoSize = true;
            this.chbCel.Location = new System.Drawing.Point(348, 147);
            this.chbCel.Name = "chbCel";
            this.chbCel.Size = new System.Drawing.Size(76, 17);
            this.chbCel.TabIndex = 150;
            this.chbCel.Text = "Целевики";
            this.chbCel.UseVisualStyleBackColor = true;
            // 
            // cbStudyLevelGroup
            // 
            this.cbStudyLevelGroup.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbStudyLevelGroup.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbStudyLevelGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStudyLevelGroup.FormattingEnabled = true;
            this.cbStudyLevelGroup.Location = new System.Drawing.Point(66, 12);
            this.cbStudyLevelGroup.Name = "cbStudyLevelGroup";
            this.cbStudyLevelGroup.Size = new System.Drawing.Size(593, 21);
            this.cbStudyLevelGroup.TabIndex = 152;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 151;
            this.label4.Text = "Уровень";
            // 
            // gbProtocolInfo
            // 
            this.gbProtocolInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gbProtocolInfo.Controls.Add(this.lblProtocolCompetitions);
            this.gbProtocolInfo.Controls.Add(this.lblProtocolPersonsCount);
            this.gbProtocolInfo.Controls.Add(this.lblHasForeigners);
            this.gbProtocolInfo.Controls.Add(this.label7);
            this.gbProtocolInfo.Controls.Add(this.label6);
            this.gbProtocolInfo.Controls.Add(this.btnSetAdmissionProtocol);
            this.gbProtocolInfo.Controls.Add(this.label5);
            this.gbProtocolInfo.Controls.Add(this.cbAdmissionProtocol);
            this.gbProtocolInfo.Location = new System.Drawing.Point(139, 422);
            this.gbProtocolInfo.Name = "gbProtocolInfo";
            this.gbProtocolInfo.Size = new System.Drawing.Size(413, 100);
            this.gbProtocolInfo.TabIndex = 153;
            this.gbProtocolInfo.TabStop = false;
            this.gbProtocolInfo.Text = "Информация";
            this.gbProtocolInfo.Visible = false;
            // 
            // lblProtocolCompetitions
            // 
            this.lblProtocolCompetitions.AutoSize = true;
            this.lblProtocolCompetitions.Location = new System.Drawing.Point(164, 69);
            this.lblProtocolCompetitions.Name = "lblProtocolCompetitions";
            this.lblProtocolCompetitions.Size = new System.Drawing.Size(13, 13);
            this.lblProtocolCompetitions.TabIndex = 158;
            this.lblProtocolCompetitions.Text = "0";
            // 
            // lblProtocolPersonsCount
            // 
            this.lblProtocolPersonsCount.AutoSize = true;
            this.lblProtocolPersonsCount.Location = new System.Drawing.Point(143, 52);
            this.lblProtocolPersonsCount.Name = "lblProtocolPersonsCount";
            this.lblProtocolPersonsCount.Size = new System.Drawing.Size(13, 13);
            this.lblProtocolPersonsCount.TabIndex = 157;
            this.lblProtocolPersonsCount.Text = "0";
            // 
            // lblHasForeigners
            // 
            this.lblHasForeigners.AutoSize = true;
            this.lblHasForeigners.Location = new System.Drawing.Point(206, 52);
            this.lblHasForeigners.Name = "lblHasForeigners";
            this.lblHasForeigners.Size = new System.Drawing.Size(116, 13);
            this.lblHasForeigners.TabIndex = 156;
            this.lblHasForeigners.Text = "ЕСТЬ ИНОСТРАНЦЫ";
            this.lblHasForeigners.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(149, 13);
            this.label7.TabIndex = 155;
            this.label7.Text = "Категории лиц в протоколе:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(128, 13);
            this.label6.TabIndex = 154;
            this.label6.Text = "Число лиц в протоколе:";
            // 
            // btnSetAdmissionProtocol
            // 
            this.btnSetAdmissionProtocol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSetAdmissionProtocol.Location = new System.Drawing.Point(330, 43);
            this.btnSetAdmissionProtocol.Name = "btnSetAdmissionProtocol";
            this.btnSetAdmissionProtocol.Size = new System.Drawing.Size(77, 26);
            this.btnSetAdmissionProtocol.TabIndex = 153;
            this.btnSetAdmissionProtocol.Text = "Установить";
            this.btnSetAdmissionProtocol.UseVisualStyleBackColor = true;
            this.btnSetAdmissionProtocol.Click += new System.EventHandler(this.btnSetAdmissionProtocol_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 152;
            this.label5.Text = "Основание:";
            // 
            // cbAdmissionProtocol
            // 
            this.cbAdmissionProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAdmissionProtocol.FormattingEnabled = true;
            this.cbAdmissionProtocol.Location = new System.Drawing.Point(81, 19);
            this.cbAdmissionProtocol.Name = "cbAdmissionProtocol";
            this.cbAdmissionProtocol.Size = new System.Drawing.Size(326, 21);
            this.cbAdmissionProtocol.TabIndex = 0;
            // 
            // EntryViewList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 575);
            this.Controls.Add(this.gbProtocolInfo);
            this.Controls.Add(this.cbStudyLevelGroup);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chbCel);
            this.Controls.Add(this.chbIsSecond);
            this.Controls.Add(this.chbIsReduced);
            this.Controls.Add(this.chbIsParallel);
            this.Controls.Add(this.cbStudyForm);
            this.Controls.Add(this.cbStudyBasis);
            this.Controls.Add(this.cbLicenseProgram);
            this.Controls.Add(this.cbFaculty);
            this.Controls.Add(this.chbIsListener);
            this.Controls.Add(this.chbIsForeign);
            this.Controls.Add(this.btnCancelView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.dgvViews);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblLanguage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnPrintOrder);
            this.Controls.Add(this.btnOrderReview);
            this.Controls.Add(this.btnPrint);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "EntryViewList";
            this.Text = "Печать представления";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EntryViewList_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgvViews)).EndInit();
            this.gbProtocolInfo.ResumeLayout(false);
            this.gbProtocolInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DataGridView dgvViews;        
        private System.Windows.Forms.Label label2;       
        private System.Windows.Forms.Label lblLanguage;       
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnPrint;
        public System.Windows.Forms.Button btnCreate;
        public System.Windows.Forms.Button btnPrintOrder;       
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button btnCancelView;
        public System.Windows.Forms.CheckBox chbIsForeign;
        private System.Windows.Forms.Button btnOrderReview;
        private System.Windows.Forms.CheckBox chbIsListener;
        private System.Windows.Forms.CheckBox chbIsSecond;
        private System.Windows.Forms.CheckBox chbIsReduced;
        private System.Windows.Forms.CheckBox chbIsParallel;
        public System.Windows.Forms.ComboBox cbStudyForm;
        public System.Windows.Forms.ComboBox cbStudyBasis;
        public System.Windows.Forms.ComboBox cbLicenseProgram;
        public System.Windows.Forms.ComboBox cbFaculty;
        private System.Windows.Forms.CheckBox chbCel;
        public System.Windows.Forms.ComboBox cbStudyLevelGroup;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gbProtocolInfo;
        private System.Windows.Forms.Label lblProtocolCompetitions;
        private System.Windows.Forms.Label lblProtocolPersonsCount;
        public System.Windows.Forms.Label lblHasForeigners;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSetAdmissionProtocol;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbAdmissionProtocol;
    }
}