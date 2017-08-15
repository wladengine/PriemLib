namespace PriemLib
{
    partial class ListAbitRatingKofGroupChanging
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
            this.btnFilters = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 424);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(446, 423);
            // 
            // btnFilters
            // 
            this.btnFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilters.Location = new System.Drawing.Point(103, 425);
            this.btnFilters.Name = "btnFilters";
            this.btnFilters.Size = new System.Drawing.Size(92, 23);
            this.btnFilters.TabIndex = 2;
            this.btnFilters.Text = "Фильтры";
            this.btnFilters.UseVisualStyleBackColor = true;
            this.btnFilters.Click += new System.EventHandler(this.btnFilters_Click);
            // 
            // ListAbitRatingKofGroupChanging
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 460);
            this.Controls.Add(this.btnFilters);
            this.Name = "ListAbitRatingKofGroupChanging";
            this.Text = "ListAbitRatingKofGroupChanging";
            this.Controls.SetChildIndex(this.btnSave, 0);
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.Controls.SetChildIndex(this.btnFilters, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnFilters;
    }
}