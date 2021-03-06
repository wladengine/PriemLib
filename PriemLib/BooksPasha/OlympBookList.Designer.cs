﻿namespace PriemLib
{
    partial class OlympBookList
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
            this.dgvOlymps = new System.Windows.Forms.DataGridView();
            this.cbOlympType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbOlympYear = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOlymps)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvOlymps
            // 
            this.dgvOlymps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvOlymps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOlymps.Location = new System.Drawing.Point(12, 91);
            this.dgvOlymps.Name = "dgvOlymps";
            this.dgvOlymps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOlymps.Size = new System.Drawing.Size(553, 246);
            this.dgvOlymps.TabIndex = 25;
            // 
            // cbOlympType
            // 
            this.cbOlympType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOlympType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbOlympType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbOlympType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOlympType.FormattingEnabled = true;
            this.cbOlympType.Location = new System.Drawing.Point(12, 25);
            this.cbOlympType.Name = "cbOlympType";
            this.cbOlympType.Size = new System.Drawing.Size(351, 21);
            this.cbOlympType.TabIndex = 130;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 129;
            this.label1.Text = "Вид олимпиады";
            // 
            // cbOlympYear
            // 
            this.cbOlympYear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOlympYear.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbOlympYear.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbOlympYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOlympYear.FormattingEnabled = true;
            this.cbOlympYear.Location = new System.Drawing.Point(369, 25);
            this.cbOlympYear.Name = "cbOlympYear";
            this.cbOlympYear.Size = new System.Drawing.Size(196, 21);
            this.cbOlympYear.TabIndex = 132;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(366, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 131;
            this.label2.Text = "Год олимпиады";
            // 
            // tbName
            // 
            this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbName.Location = new System.Drawing.Point(12, 65);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(553, 20);
            this.tbName.TabIndex = 133;
            this.tbName.TextChanged += new System.EventHandler(this.tbName_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 134;
            this.label3.Text = "Поиск по названию";
            // 
            // OlympBookList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 407);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.cbOlympYear);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbOlympType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvOlymps);
            this.Name = "OlympBookList";
            this.Text = "OlympBookList";
            this.Controls.SetChildIndex(this.dgvOlymps, 0);
            this.Controls.SetChildIndex(this.lblCount, 0);
            this.Controls.SetChildIndex(this.btnCard, 0);
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.Controls.SetChildIndex(this.btnAdd, 0);
            this.Controls.SetChildIndex(this.btnRemove, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.cbOlympType, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.cbOlympYear, 0);
            this.Controls.SetChildIndex(this.tbName, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOlymps)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvOlymps;
        private System.Windows.Forms.ComboBox cbOlympType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbOlympYear;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label3;
    }
}