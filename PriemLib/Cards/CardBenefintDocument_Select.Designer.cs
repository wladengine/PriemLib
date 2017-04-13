namespace PriemLib
{
    partial class CardBenefintDocument_Select
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
            this.btnSelect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbBenefitDocumentType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnSelect.Location = new System.Drawing.Point(286, 52);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "Выбрать";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(260, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Выберите тип документа";
            // 
            // cbBenefitDocumentType
            // 
            this.cbBenefitDocumentType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbBenefitDocumentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBenefitDocumentType.FormattingEnabled = true;
            this.cbBenefitDocumentType.Location = new System.Drawing.Point(12, 25);
            this.cbBenefitDocumentType.Name = "cbBenefitDocumentType";
            this.cbBenefitDocumentType.Size = new System.Drawing.Size(605, 21);
            this.cbBenefitDocumentType.TabIndex = 2;
            // 
            // CardBenefintDocument_Select
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 86);
            this.Controls.Add(this.cbBenefitDocumentType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSelect);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CardBenefintDocument_Select";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CardBenefintDocument_Select";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbBenefitDocumentType;
    }
}