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
            this.btnCopyToCrimea = new System.Windows.Forms.Button();
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
            // btnCopyToCrimea
            // 
            this.btnCopyToCrimea.Location = new System.Drawing.Point(12, 152);
            this.btnCopyToCrimea.Name = "btnCopyToCrimea";
            this.btnCopyToCrimea.Size = new System.Drawing.Size(296, 23);
            this.btnCopyToCrimea.TabIndex = 6;
            this.btnCopyToCrimea.Text = "Скопировать в КРЫМ";
            this.btnCopyToCrimea.UseVisualStyleBackColor = true;
            this.btnCopyToCrimea.Click += new System.EventHandler(this.btnCopyToCrimea_Click);
            // 
            // CardLoadEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 196);
            this.Controls.Add(this.btnCopyToCrimea);
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
        private System.Windows.Forms.Button btnCopyToCrimea;
    }
}