namespace PriemLib
{
    partial class CardDisabilityDocument
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
            this.chbHasOriginals = new System.Windows.Forms.CheckBox();
            this.cbDisabilityType = new System.Windows.Forms.ComboBox();
            this.lblDisabilityType = new System.Windows.Forms.Label();
            this.tbAuthor = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbNumber = new System.Windows.Forms.TextBox();
            this.tbSeries = new System.Windows.Forms.TextBox();
            this.lblFIO = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.epError)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(357, 183);
            // 
            // btnSaveChange
            // 
            this.btnSaveChange.Location = new System.Drawing.Point(12, 183);
            // 
            // btnSaveAsNew
            // 
            this.btnSaveAsNew.Location = new System.Drawing.Point(220, 182);
            // 
            // chbHasOriginals
            // 
            this.chbHasOriginals.Location = new System.Drawing.Point(333, 70);
            this.chbHasOriginals.Name = "chbHasOriginals";
            this.chbHasOriginals.Size = new System.Drawing.Size(108, 35);
            this.chbHasOriginals.TabIndex = 59;
            this.chbHasOriginals.Text = "Оригиналы предоставлены";
            this.chbHasOriginals.UseVisualStyleBackColor = true;
            // 
            // cbBenefitDocument
            // 
            this.cbDisabilityType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDisabilityType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDisabilityType.FormattingEnabled = true;
            this.cbDisabilityType.Location = new System.Drawing.Point(117, 43);
            this.cbDisabilityType.Name = "cbBenefitDocument";
            this.cbDisabilityType.Size = new System.Drawing.Size(321, 21);
            this.cbDisabilityType.TabIndex = 58;
            // 
            // lblDisabilityType
            // 
            this.lblDisabilityType.Location = new System.Drawing.Point(31, 40);
            this.lblDisabilityType.Name = "lblDisabilityType";
            this.lblDisabilityType.Size = new System.Drawing.Size(80, 29);
            this.lblDisabilityType.TabIndex = 57;
            this.lblDisabilityType.Text = "Группа инвалидности";
            this.lblDisabilityType.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbAuthor
            // 
            this.tbAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAuthor.Location = new System.Drawing.Point(117, 151);
            this.tbAuthor.Name = "tbAuthor";
            this.tbAuthor.Size = new System.Drawing.Size(324, 20);
            this.tbAuthor.TabIndex = 56;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(45, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 55;
            this.label5.Text = "Кем выдан";
            // 
            // dtpDate
            // 
            this.dtpDate.Checked = false;
            this.dtpDate.Location = new System.Drawing.Point(117, 125);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.ShowCheckBox = true;
            this.dtpDate.Size = new System.Drawing.Size(200, 20);
            this.dtpDate.TabIndex = 53;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(36, 121);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 31);
            this.label4.TabIndex = 52;
            this.label4.Text = "Дата выдачи (если есть)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(70, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 51;
            this.label3.Text = "Номер";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(73, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 50;
            this.label2.Text = "Серия";
            // 
            // tbNumber
            // 
            this.tbNumber.Location = new System.Drawing.Point(117, 96);
            this.tbNumber.Name = "tbNumber";
            this.tbNumber.Size = new System.Drawing.Size(200, 20);
            this.tbNumber.TabIndex = 49;
            // 
            // tbSeries
            // 
            this.tbSeries.Location = new System.Drawing.Point(117, 70);
            this.tbSeries.Name = "tbSeries";
            this.tbSeries.Size = new System.Drawing.Size(100, 20);
            this.tbSeries.TabIndex = 48;
            // 
            // lblFIO
            // 
            this.lblFIO.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblFIO.Location = new System.Drawing.Point(28, 9);
            this.lblFIO.Name = "lblFIO";
            this.lblFIO.Size = new System.Drawing.Size(410, 23);
            this.lblFIO.TabIndex = 54;
            this.lblFIO.Text = "FIO";
            this.lblFIO.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CardDisabilityDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 218);
            this.Controls.Add(this.chbHasOriginals);
            this.Controls.Add(this.cbDisabilityType);
            this.Controls.Add(this.lblDisabilityType);
            this.Controls.Add(this.tbAuthor);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbNumber);
            this.Controls.Add(this.tbSeries);
            this.Controls.Add(this.lblFIO);
            this.Name = "CardDisabilityDocument";
            this.Text = "CardDisabilityDocument";
            this.Controls.SetChildIndex(this.btnSaveChange, 0);
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.Controls.SetChildIndex(this.btnSaveAsNew, 0);
            this.Controls.SetChildIndex(this.lblFIO, 0);
            this.Controls.SetChildIndex(this.tbSeries, 0);
            this.Controls.SetChildIndex(this.tbNumber, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.dtpDate, 0);
            this.Controls.SetChildIndex(this.label5, 0);
            this.Controls.SetChildIndex(this.tbAuthor, 0);
            this.Controls.SetChildIndex(this.lblDisabilityType, 0);
            this.Controls.SetChildIndex(this.cbDisabilityType, 0);
            this.Controls.SetChildIndex(this.chbHasOriginals, 0);
            ((System.ComponentModel.ISupportInitialize)(this.epError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chbHasOriginals;
        private System.Windows.Forms.ComboBox cbDisabilityType;
        private System.Windows.Forms.Label lblDisabilityType;
        private System.Windows.Forms.TextBox tbAuthor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbNumber;
        private System.Windows.Forms.TextBox tbSeries;
        private System.Windows.Forms.Label lblFIO;
    }
}