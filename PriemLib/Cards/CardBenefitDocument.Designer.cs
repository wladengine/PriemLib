﻿namespace PriemLib
{
    partial class CardBenefitDocument
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
            this.cbBenefitDocumentType = new System.Windows.Forms.ComboBox();
            this.tbSeries = new System.Windows.Forms.TextBox();
            this.tbNumber = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.lblFIO = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbAuthor = new System.Windows.Forms.TextBox();
            this.cbBenefitDocument = new System.Windows.Forms.ComboBox();
            this.chbHasOriginals = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.epError)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(413, 398);
            // 
            // btnSaveChange
            // 
            this.btnSaveChange.Location = new System.Drawing.Point(15, 214);
            // 
            // btnSaveAsNew
            // 
            this.btnSaveAsNew.Location = new System.Drawing.Point(276, 398);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Тип документа";
            // 
            // cbBenefitDocumentType
            // 
            this.cbBenefitDocumentType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbBenefitDocumentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBenefitDocumentType.DropDownWidth = 500;
            this.cbBenefitDocumentType.FormattingEnabled = true;
            this.cbBenefitDocumentType.Location = new System.Drawing.Point(101, 46);
            this.cbBenefitDocumentType.Name = "cbBenefitDocumentType";
            this.cbBenefitDocumentType.Size = new System.Drawing.Size(334, 21);
            this.cbBenefitDocumentType.TabIndex = 1;
            this.cbBenefitDocumentType.SelectedIndexChanged += new System.EventHandler(this.cbBenefitDocumentType_SelectedIndexChanged);
            // 
            // tbSeries
            // 
            this.tbSeries.Location = new System.Drawing.Point(101, 100);
            this.tbSeries.Name = "tbSeries";
            this.tbSeries.Size = new System.Drawing.Size(100, 20);
            this.tbSeries.TabIndex = 2;
            // 
            // tbNumber
            // 
            this.tbNumber.Location = new System.Drawing.Point(101, 126);
            this.tbNumber.Name = "tbNumber";
            this.tbNumber.Size = new System.Drawing.Size(200, 20);
            this.tbNumber.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(57, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Серия";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(54, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Номер";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(21, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 31);
            this.label4.TabIndex = 6;
            this.label4.Text = "Дата выдачи (если есть)";
            // 
            // dtpDate
            // 
            this.dtpDate.Checked = false;
            this.dtpDate.Location = new System.Drawing.Point(101, 152);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.ShowCheckBox = true;
            this.dtpDate.Size = new System.Drawing.Size(200, 20);
            this.dtpDate.TabIndex = 7;
            // 
            // lblFIO
            // 
            this.lblFIO.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblFIO.Location = new System.Drawing.Point(12, 12);
            this.lblFIO.Name = "lblFIO";
            this.lblFIO.Size = new System.Drawing.Size(410, 23);
            this.lblFIO.TabIndex = 40;
            this.lblFIO.Text = "FIO";
            this.lblFIO.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 181);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 41;
            this.label5.Text = "Кем выдан";
            // 
            // tbAuthor
            // 
            this.tbAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAuthor.Location = new System.Drawing.Point(101, 178);
            this.tbAuthor.Name = "tbAuthor";
            this.tbAuthor.Size = new System.Drawing.Size(337, 20);
            this.tbAuthor.TabIndex = 42;
            // 
            // cbBenefitDocument
            // 
            this.cbBenefitDocument.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbBenefitDocument.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBenefitDocument.DropDownWidth = 500;
            this.cbBenefitDocument.FormattingEnabled = true;
            this.cbBenefitDocument.Location = new System.Drawing.Point(101, 73);
            this.cbBenefitDocument.Name = "cbBenefitDocument";
            this.cbBenefitDocument.Size = new System.Drawing.Size(334, 21);
            this.cbBenefitDocument.TabIndex = 44;
            // 
            // chbHasOriginals
            // 
            this.chbHasOriginals.Location = new System.Drawing.Point(317, 107);
            this.chbHasOriginals.Name = "chbHasOriginals";
            this.chbHasOriginals.Size = new System.Drawing.Size(108, 35);
            this.chbHasOriginals.TabIndex = 45;
            this.chbHasOriginals.Text = "Оригиналы предоставлены";
            this.chbHasOriginals.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 46;
            this.label6.Text = "Вид документа";
            // 
            // CardBenefitDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 249);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.chbHasOriginals);
            this.Controls.Add(this.cbBenefitDocument);
            this.Controls.Add(this.tbAuthor);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbNumber);
            this.Controls.Add(this.tbSeries);
            this.Controls.Add(this.cbBenefitDocumentType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblFIO);
            this.Name = "CardBenefitDocument";
            this.Text = "CardBenefitDocument";
            this.Controls.SetChildIndex(this.lblFIO, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.cbBenefitDocumentType, 0);
            this.Controls.SetChildIndex(this.tbSeries, 0);
            this.Controls.SetChildIndex(this.tbNumber, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.dtpDate, 0);
            this.Controls.SetChildIndex(this.btnSaveAsNew, 0);
            this.Controls.SetChildIndex(this.btnSaveChange, 0);
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.Controls.SetChildIndex(this.label5, 0);
            this.Controls.SetChildIndex(this.tbAuthor, 0);
            this.Controls.SetChildIndex(this.cbBenefitDocument, 0);
            this.Controls.SetChildIndex(this.chbHasOriginals, 0);
            this.Controls.SetChildIndex(this.label6, 0);
            ((System.ComponentModel.ISupportInitialize)(this.epError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbBenefitDocumentType;
        private System.Windows.Forms.TextBox tbSeries;
        private System.Windows.Forms.TextBox tbNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label lblFIO;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbAuthor;
        private System.Windows.Forms.ComboBox cbBenefitDocument;
        private System.Windows.Forms.CheckBox chbHasOriginals;
        private System.Windows.Forms.Label label6;
    }
}