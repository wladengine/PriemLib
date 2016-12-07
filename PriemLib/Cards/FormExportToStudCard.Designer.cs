namespace PriemLib
{
    partial class FormExportToStudCard
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
            this.dtpMinOrderDate = new System.Windows.Forms.DateTimePicker();
            this.btnStartExport = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblCnt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dtpMinOrderDate.Checked = false;
            this.dtpMinOrderDate.Location = new System.Drawing.Point(12, 25);
            this.dtpMinOrderDate.Name = "dateTimePicker1";
            this.dtpMinOrderDate.ShowCheckBox = true;
            this.dtpMinOrderDate.Size = new System.Drawing.Size(200, 20);
            this.dtpMinOrderDate.TabIndex = 0;
            // 
            // btnStartExport
            // 
            this.btnStartExport.Location = new System.Drawing.Point(316, 22);
            this.btnStartExport.Name = "btnStartExport";
            this.btnStartExport.Size = new System.Drawing.Size(136, 23);
            this.btnStartExport.TabIndex = 1;
            this.btnStartExport.Text = "Экспортировать";
            this.btnStartExport.UseVisualStyleBackColor = true;
            this.btnStartExport.Click += new System.EventHandler(this.btnStartExport_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Загружать с приказами от";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(218, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Всего:";
            // 
            // lblCnt
            // 
            this.lblCnt.AutoSize = true;
            this.lblCnt.Location = new System.Drawing.Point(264, 29);
            this.lblCnt.Name = "lblCnt";
            this.lblCnt.Size = new System.Drawing.Size(13, 13);
            this.lblCnt.TabIndex = 4;
            this.lblCnt.Text = "0";
            // 
            // FormExportToStudCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 73);
            this.Controls.Add(this.lblCnt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStartExport);
            this.Controls.Add(this.dtpMinOrderDate);
            this.Name = "FormExportToStudCard";
            this.Text = "FormExportToStudCard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpMinOrderDate;
        private System.Windows.Forms.Button btnStartExport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblCnt;
    }
}