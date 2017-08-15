namespace PriemLib.Cards
{
    partial class CardAddMarkFromAchievement
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
            this.cbAchievementType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cbOlympiad = new System.Windows.Forms.ComboBox();
            this.tbMark = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.epError)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(528, 367);
            // 
            // btnSaveChange
            // 
            this.btnSaveChange.Location = new System.Drawing.Point(12, 366);
            // 
            // btnSaveAsNew
            // 
            this.btnSaveAsNew.Location = new System.Drawing.Point(391, 366);
            // 
            // cbAchievementType
            // 
            this.cbAchievementType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAchievementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAchievementType.FormattingEnabled = true;
            this.cbAchievementType.Location = new System.Drawing.Point(107, 9);
            this.cbAchievementType.Name = "cbAchievementType";
            this.cbAchievementType.Size = new System.Drawing.Size(568, 21);
            this.cbAchievementType.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Вид ИД";
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(589, 93);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(86, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Добавить";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 30);
            this.label2.TabIndex = 4;
            this.label2.Text = "Олимпиада (если требуется)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbOlympiad
            // 
            this.cbOlympiad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOlympiad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOlympiad.FormattingEnabled = true;
            this.cbOlympiad.Location = new System.Drawing.Point(107, 36);
            this.cbOlympiad.Name = "cbOlympiad";
            this.cbOlympiad.Size = new System.Drawing.Size(568, 21);
            this.cbOlympiad.TabIndex = 3;
            // 
            // tbMark
            // 
            this.tbMark.Location = new System.Drawing.Point(107, 63);
            this.tbMark.Name = "tbMark";
            this.tbMark.Size = new System.Drawing.Size(100, 20);
            this.tbMark.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Балл за ИД:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(107, 93);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(86, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // CardAddMarkFromAchievement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 130);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbMark);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbOlympiad);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbAchievementType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "CardAddMarkFromAchievement";
            this.Text = "CardAddMarkFromAchievement";
            this.Controls.SetChildIndex(this.cbAchievementType, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.btnAdd, 0);
            this.Controls.SetChildIndex(this.cbOlympiad, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.tbMark, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnSaveChange, 0);
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.Controls.SetChildIndex(this.btnSaveAsNew, 0);
            ((System.ComponentModel.ISupportInitialize)(this.epError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbAchievementType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbOlympiad;
        private System.Windows.Forms.TextBox tbMark;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCancel;
    }
}