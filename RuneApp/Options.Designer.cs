﻿namespace RuneApp
{
    partial class Options
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
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.cOtherStats = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.gTestShow = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.gTestRun = new System.Windows.Forms.TextBox();
			this.cOtherUpdate = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.cColorTeams = new System.Windows.Forms.CheckBox();
			this.cDisplayGray = new System.Windows.Forms.CheckBox();
			this.cDisplaySplit = new System.Windows.Forms.CheckBox();
			this.cGenLockTest = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.button1 = new System.Windows.Forms.Button();
			this.btnHelp = new System.Windows.Forms.Button();
			this.gTestTime = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.tabPage1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.cOtherStats);
			this.tabPage1.Controls.Add(this.label6);
			this.tabPage1.Controls.Add(this.gTestTime);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.gTestShow);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.gTestRun);
			this.tabPage1.Controls.Add(this.cOtherUpdate);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this.cColorTeams);
			this.tabPage1.Controls.Add(this.cDisplayGray);
			this.tabPage1.Controls.Add(this.cDisplaySplit);
			this.tabPage1.Controls.Add(this.cGenLockTest);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(675, 311);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "General";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// cOtherStats
			// 
			this.cOtherStats.AutoSize = true;
			this.cOtherStats.Location = new System.Drawing.Point(465, 42);
			this.cOtherStats.Name = "cOtherStats";
			this.cOtherStats.Size = new System.Drawing.Size(160, 17);
			this.cOtherStats.TabIndex = 6;
			this.cOtherStats.Text = "Disable runestats generation";
			this.cOtherStats.UseVisualStyleBackColor = true;
			this.cOtherStats.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(62, 70);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(73, 13);
			this.label5.TabIndex = 5;
			this.label5.Text = "Tests to show";
			// 
			// gTestShow
			// 
			this.gTestShow.Location = new System.Drawing.Point(6, 67);
			this.gTestShow.Name = "gTestShow";
			this.gTestShow.Size = new System.Drawing.Size(50, 20);
			this.gTestShow.TabIndex = 4;
			this.gTestShow.Text = "100";
			this.gTestShow.TextChanged += new System.EventHandler(this.num_TextChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(62, 43);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(63, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "Tests to run";
			// 
			// gTestRun
			// 
			this.gTestRun.Location = new System.Drawing.Point(6, 40);
			this.gTestRun.Name = "gTestRun";
			this.gTestRun.Size = new System.Drawing.Size(50, 20);
			this.gTestRun.TabIndex = 4;
			this.gTestRun.Text = "5000";
			this.gTestRun.TextChanged += new System.EventHandler(this.num_TextChanged);
			// 
			// cOtherUpdate
			// 
			this.cOtherUpdate.AutoSize = true;
			this.cOtherUpdate.Location = new System.Drawing.Point(465, 19);
			this.cOtherUpdate.Name = "cOtherUpdate";
			this.cOtherUpdate.Size = new System.Drawing.Size(130, 17);
			this.cOtherUpdate.TabIndex = 3;
			this.cOtherUpdate.Text = "Disable update check";
			this.cOtherUpdate.UseVisualStyleBackColor = true;
			this.cOtherUpdate.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(462, 3);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(33, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Other";
			// 
			// cColorTeams
			// 
			this.cColorTeams.AutoSize = true;
			this.cColorTeams.Location = new System.Drawing.Point(246, 65);
			this.cColorTeams.Name = "cColorTeams";
			this.cColorTeams.Size = new System.Drawing.Size(127, 17);
			this.cColorTeams.TabIndex = 1;
			this.cColorTeams.Text = "Color Team Relations";
			this.cColorTeams.UseVisualStyleBackColor = true;
			this.cColorTeams.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
			// 
			// cDisplayGray
			// 
			this.cDisplayGray.AutoSize = true;
			this.cDisplayGray.Location = new System.Drawing.Point(246, 42);
			this.cDisplayGray.Name = "cDisplayGray";
			this.cDisplayGray.Size = new System.Drawing.Size(159, 17);
			this.cDisplayGray.TabIndex = 1;
			this.cDisplayGray.Text = "Gray tests with locked runes";
			this.cDisplayGray.UseVisualStyleBackColor = true;
			this.cDisplayGray.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
			// 
			// cDisplaySplit
			// 
			this.cDisplaySplit.AutoSize = true;
			this.cDisplaySplit.Location = new System.Drawing.Point(246, 19);
			this.cDisplaySplit.Name = "cDisplaySplit";
			this.cDisplaySplit.Size = new System.Drawing.Size(109, 17);
			this.cDisplaySplit.TabIndex = 1;
			this.cDisplaySplit.Text = "Split assign/swap";
			this.cDisplaySplit.UseVisualStyleBackColor = true;
			this.cDisplaySplit.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
			// 
			// cGenLockTest
			// 
			this.cGenLockTest.AutoSize = true;
			this.cGenLockTest.Location = new System.Drawing.Point(9, 19);
			this.cGenLockTest.Name = "cGenLockTest";
			this.cGenLockTest.Size = new System.Drawing.Size(144, 17);
			this.cGenLockTest.TabIndex = 1;
			this.cGenLockTest.Text = "Use locked runes in Test";
			this.cGenLockTest.UseVisualStyleBackColor = true;
			this.cGenLockTest.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(243, 3);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Display";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Generation";
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(683, 337);
			this.tabControl1.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(620, 355);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// btnHelp
			// 
			this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnHelp.Location = new System.Drawing.Point(569, 355);
			this.btnHelp.Name = "btnHelp";
			this.btnHelp.Size = new System.Drawing.Size(45, 23);
			this.btnHelp.TabIndex = 90;
			this.btnHelp.Text = "Help?";
			this.btnHelp.UseVisualStyleBackColor = true;
			this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
			// 
			// gTestTime
			// 
			this.gTestTime.Location = new System.Drawing.Point(6, 93);
			this.gTestTime.Name = "gTestTime";
			this.gTestTime.Size = new System.Drawing.Size(50, 20);
			this.gTestTime.TabIndex = 4;
			this.gTestTime.Text = "20";
			this.gTestTime.TextChanged += new System.EventHandler(this.num_TextChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(62, 96);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(85, 13);
			this.label6.TabIndex = 5;
			this.label6.Text = "Seconds to Test";
			// 
			// Options
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(707, 390);
			this.Controls.Add(this.btnHelp);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.tabControl1);
			this.Name = "Options";
			this.Text = "Options";
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cGenLockTest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cDisplaySplit;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox cOtherUpdate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cDisplayGray;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox gTestRun;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox gTestShow;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.CheckBox cOtherStats;
        private System.Windows.Forms.CheckBox cColorTeams;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox gTestTime;
	}
}