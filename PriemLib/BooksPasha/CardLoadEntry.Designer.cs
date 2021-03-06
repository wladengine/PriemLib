﻿namespace PriemLib
{
    partial class CardLoadEntry
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
            this.btnLoadAll = new System.Windows.Forms.Button();
            this.btnCheckUpdates = new System.Windows.Forms.Button();
            this.btnLoadUpdates = new System.Windows.Forms.Button();
            this.btnUpdateKCP = new System.Windows.Forms.Button();
            this.btnOnlineLoadUpdate = new System.Windows.Forms.Button();
            this.btnUpdateBaseDics = new System.Windows.Forms.Button();
            this.btnCopyToForeign = new System.Windows.Forms.Button();
            this.btnLoadUpdateOlympiads = new System.Windows.Forms.Button();
            this.btnLoadUpdateExams = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnLoadAll
            // 
            this.btnLoadAll.Location = new System.Drawing.Point(12, 12);
            this.btnLoadAll.Name = "btnLoadAll";
            this.btnLoadAll.Size = new System.Drawing.Size(296, 23);
            this.btnLoadAll.TabIndex = 0;
            this.btnLoadAll.Text = "Загрузить все";
            this.btnLoadAll.UseVisualStyleBackColor = true;
            this.btnLoadAll.Click += new System.EventHandler(this.btnLoadAll_Click);
            // 
            // btnCheckUpdates
            // 
            this.btnCheckUpdates.Location = new System.Drawing.Point(12, 54);
            this.btnCheckUpdates.Name = "btnCheckUpdates";
            this.btnCheckUpdates.Size = new System.Drawing.Size(145, 23);
            this.btnCheckUpdates.TabIndex = 1;
            this.btnCheckUpdates.Text = "Проверить обновления";
            this.btnCheckUpdates.UseVisualStyleBackColor = true;
            this.btnCheckUpdates.Click += new System.EventHandler(this.btnCheckUpdates_Click);
            // 
            // btnLoadUpdates
            // 
            this.btnLoadUpdates.Location = new System.Drawing.Point(12, 83);
            this.btnLoadUpdates.Name = "btnLoadUpdates";
            this.btnLoadUpdates.Size = new System.Drawing.Size(145, 23);
            this.btnLoadUpdates.TabIndex = 2;
            this.btnLoadUpdates.Text = "Загрузить добавленное";
            this.btnLoadUpdates.UseVisualStyleBackColor = true;
            this.btnLoadUpdates.Click += new System.EventHandler(this.btnLoadUpdates_Click);
            // 
            // btnUpdateKCP
            // 
            this.btnUpdateKCP.Location = new System.Drawing.Point(12, 112);
            this.btnUpdateKCP.Name = "btnUpdateKCP";
            this.btnUpdateKCP.Size = new System.Drawing.Size(145, 23);
            this.btnUpdateKCP.TabIndex = 3;
            this.btnUpdateKCP.Text = "Обновить КЦ";
            this.btnUpdateKCP.UseVisualStyleBackColor = true;
            this.btnUpdateKCP.Click += new System.EventHandler(this.btnUpdateKCP_Click);
            // 
            // btnOnlineLoadUpdate
            // 
            this.btnOnlineLoadUpdate.Location = new System.Drawing.Point(163, 54);
            this.btnOnlineLoadUpdate.Name = "btnOnlineLoadUpdate";
            this.btnOnlineLoadUpdate.Size = new System.Drawing.Size(145, 23);
            this.btnOnlineLoadUpdate.TabIndex = 4;
            this.btnOnlineLoadUpdate.Text = "Online Load/Update";
            this.btnOnlineLoadUpdate.UseVisualStyleBackColor = true;
            this.btnOnlineLoadUpdate.Click += new System.EventHandler(this.btnOnlineLoadUpdate_Click);
            // 
            // btnUpdateBaseDics
            // 
            this.btnUpdateBaseDics.Location = new System.Drawing.Point(163, 112);
            this.btnUpdateBaseDics.Name = "btnUpdateBaseDics";
            this.btnUpdateBaseDics.Size = new System.Drawing.Size(145, 23);
            this.btnUpdateBaseDics.TabIndex = 5;
            this.btnUpdateBaseDics.Text = "Обн. Базовые Справ";
            this.btnUpdateBaseDics.UseVisualStyleBackColor = true;
            this.btnUpdateBaseDics.Click += new System.EventHandler(this.btnUpdateBaseDics_Click);
            // 
            // btnCopyToForeign
            // 
            this.btnCopyToForeign.Location = new System.Drawing.Point(16, 181);
            this.btnCopyToForeign.Name = "btnCopyToForeign";
            this.btnCopyToForeign.Size = new System.Drawing.Size(296, 23);
            this.btnCopyToForeign.TabIndex = 7;
            this.btnCopyToForeign.Text = "Скопировать в ИНОСТР";
            this.btnCopyToForeign.UseVisualStyleBackColor = true;
            this.btnCopyToForeign.Click += new System.EventHandler(this.btnCopyToForeign_Click);
            // 
            // btnLoadUpdateOlympiads
            // 
            this.btnLoadUpdateOlympiads.Location = new System.Drawing.Point(12, 210);
            this.btnLoadUpdateOlympiads.Name = "btnLoadUpdateOlympiads";
            this.btnLoadUpdateOlympiads.Size = new System.Drawing.Size(145, 23);
            this.btnLoadUpdateOlympiads.TabIndex = 8;
            this.btnLoadUpdateOlympiads.Text = "Обн. Олимпиады";
            this.btnLoadUpdateOlympiads.UseVisualStyleBackColor = true;
            this.btnLoadUpdateOlympiads.Click += new System.EventHandler(this.btnLoadUpdateOlympiads_Click);
            // 
            // btnLoadUpdateExams
            // 
            this.btnLoadUpdateExams.Location = new System.Drawing.Point(163, 210);
            this.btnLoadUpdateExams.Name = "btnLoadUpdateExams";
            this.btnLoadUpdateExams.Size = new System.Drawing.Size(145, 23);
            this.btnLoadUpdateExams.TabIndex = 9;
            this.btnLoadUpdateExams.Text = "Обн. Экзамены";
            this.btnLoadUpdateExams.UseVisualStyleBackColor = true;
            this.btnLoadUpdateExams.Click += new System.EventHandler(this.btnLoadUpdateExams_Click);
            // 
            // CardLoadEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 275);
            this.Controls.Add(this.btnLoadUpdateExams);
            this.Controls.Add(this.btnLoadUpdateOlympiads);
            this.Controls.Add(this.btnCopyToForeign);
            this.Controls.Add(this.btnUpdateBaseDics);
            this.Controls.Add(this.btnOnlineLoadUpdate);
            this.Controls.Add(this.btnUpdateKCP);
            this.Controls.Add(this.btnLoadUpdates);
            this.Controls.Add(this.btnCheckUpdates);
            this.Controls.Add(this.btnLoadAll);
            this.Name = "CardLoadEntry";
            this.Text = "LoadEntry";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CardLoadEntry_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoadAll;
        private System.Windows.Forms.Button btnCheckUpdates;
        private System.Windows.Forms.Button btnLoadUpdates;
        private System.Windows.Forms.Button btnUpdateKCP;
        private System.Windows.Forms.Button btnOnlineLoadUpdate;
        private System.Windows.Forms.Button btnUpdateBaseDics;
        private System.Windows.Forms.Button btnCopyToForeign;
        private System.Windows.Forms.Button btnLoadUpdateOlympiads;
        private System.Windows.Forms.Button btnLoadUpdateExams;
    }
}