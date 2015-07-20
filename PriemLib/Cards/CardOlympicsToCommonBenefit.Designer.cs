namespace PriemLib
{
    partial class CardOlympicsToCommonBenefit
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
            this.cbOlympType = new System.Windows.Forms.ComboBox();
            this.cbOlympLevel = new System.Windows.Forms.ComboBox();
            this.cbOlympSubject = new System.Windows.Forms.ComboBox();
            this.cbOlympValue = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.epError)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(541, 177);
            // 
            // btnSaveAsNew
            // 
            this.btnSaveAsNew.Location = new System.Drawing.Point(404, 179);
            // 
            // cbOlympType
            // 
            this.cbOlympType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOlympType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbOlympType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbOlympType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOlympType.FormattingEnabled = true;
            this.cbOlympType.Location = new System.Drawing.Point(144, 12);
            this.cbOlympType.Name = "cbOlympType";
            this.cbOlympType.Size = new System.Drawing.Size(478, 21);
            this.cbOlympType.TabIndex = 128;
            this.cbOlympType.SelectedIndexChanged += new System.EventHandler(this.cbOlympType_SelectedIndexChanged);
            // 
            // cbOlympLevel
            // 
            this.cbOlympLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOlympLevel.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbOlympLevel.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbOlympLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOlympLevel.FormattingEnabled = true;
            this.cbOlympLevel.Location = new System.Drawing.Point(144, 42);
            this.cbOlympLevel.Name = "cbOlympLevel";
            this.cbOlympLevel.Size = new System.Drawing.Size(478, 21);
            this.cbOlympLevel.TabIndex = 127;
            this.cbOlympLevel.SelectedIndexChanged += new System.EventHandler(this.cbOlympName_SelectedIndexChanged);
            // 
            // cbOlympSubject
            // 
            this.cbOlympSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOlympSubject.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbOlympSubject.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbOlympSubject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOlympSubject.FormattingEnabled = true;
            this.cbOlympSubject.Location = new System.Drawing.Point(144, 72);
            this.cbOlympSubject.Name = "cbOlympSubject";
            this.cbOlympSubject.Size = new System.Drawing.Size(478, 21);
            this.cbOlympSubject.TabIndex = 126;
            this.cbOlympSubject.SelectedIndexChanged += new System.EventHandler(this.cbOlympSubject_SelectedIndexChanged);
            // 
            // cbOlympValue
            // 
            this.cbOlympValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOlympValue.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbOlympValue.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbOlympValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOlympValue.FormattingEnabled = true;
            this.cbOlympValue.Location = new System.Drawing.Point(144, 99);
            this.cbOlympValue.Name = "cbOlympValue";
            this.cbOlympValue.Size = new System.Drawing.Size(296, 21);
            this.cbOlympValue.TabIndex = 125;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 13);
            this.label5.TabIndex = 123;
            this.label5.Text = "Уровень олимпиады";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 121;
            this.label3.Text = "Степень диплома";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 120;
            this.label2.Text = "Предмет олимпиады";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 119;
            this.label1.Text = "Вид олимпиады";
            // 
            // CardOlympicsToCommonBenefit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 214);
            this.Controls.Add(this.cbOlympType);
            this.Controls.Add(this.cbOlympLevel);
            this.Controls.Add(this.cbOlympSubject);
            this.Controls.Add(this.cbOlympValue);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "CardOlympicsToCommonBenefit";
            this.Text = "CardOlympicsToCommonBenefit";
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.label5, 0);
            this.Controls.SetChildIndex(this.cbOlympValue, 0);
            this.Controls.SetChildIndex(this.cbOlympSubject, 0);
            this.Controls.SetChildIndex(this.cbOlympLevel, 0);
            this.Controls.SetChildIndex(this.cbOlympType, 0);
            this.Controls.SetChildIndex(this.btnSaveChange, 0);
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.Controls.SetChildIndex(this.btnSaveAsNew, 0);
            ((System.ComponentModel.ISupportInitialize)(this.epError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbOlympType;
        private System.Windows.Forms.ComboBox cbOlympLevel;
        private System.Windows.Forms.ComboBox cbOlympSubject;
        private System.Windows.Forms.ComboBox cbOlympValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}