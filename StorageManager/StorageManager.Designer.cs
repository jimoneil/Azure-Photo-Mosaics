namespace StorageManager
{
    partial class StorageManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StorageManager));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAccount = new System.Windows.Forms.TextBox();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.rbDelete = new System.Windows.Forms.RadioButton();
            this.rbCreate = new System.Windows.Forms.RadioButton();
            this.btnGo = new System.Windows.Forms.Button();
            this.lbStatus = new System.Windows.Forms.ListBox();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.rbCreateLibrary = new System.Windows.Forms.RadioButton();
            this.chkImage01 = new System.Windows.Forms.CheckBox();
            this.chkImage02 = new System.Windows.Forms.CheckBox();
            this.chkRequiredTables = new System.Windows.Forms.CheckBox();
            this.chkRequiredQueues = new System.Windows.Forms.CheckBox();
            this.chkRequiredContainers = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Account Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Account Key:";
            // 
            // txtAccount
            // 
            this.txtAccount.Location = new System.Drawing.Point(108, 11);
            this.txtAccount.Name = "txtAccount";
            this.txtAccount.Size = new System.Drawing.Size(156, 20);
            this.txtAccount.TabIndex = 1;
            // 
            // txtKey
            // 
            this.txtKey.Location = new System.Drawing.Point(108, 44);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(156, 20);
            this.txtKey.TabIndex = 3;
            // 
            // rbDelete
            // 
            this.rbDelete.AutoSize = true;
            this.rbDelete.Location = new System.Drawing.Point(108, 272);
            this.rbDelete.Name = "rbDelete";
            this.rbDelete.Size = new System.Drawing.Size(108, 17);
            this.rbDelete.TabIndex = 12;
            this.rbDelete.Text = "Delete everything";
            this.rbDelete.UseVisualStyleBackColor = true;
            this.rbDelete.CheckedChanged += new System.EventHandler(this.rbDelete_CheckedChanged);
            // 
            // rbCreate
            // 
            this.rbCreate.AutoSize = true;
            this.rbCreate.Checked = true;
            this.rbCreate.Location = new System.Drawing.Point(108, 85);
            this.rbCreate.Name = "rbCreate";
            this.rbCreate.Size = new System.Drawing.Size(148, 17);
            this.rbCreate.TabIndex = 5;
            this.rbCreate.TabStop = true;
            this.rbCreate.Text = "Create application storage";
            this.rbCreate.UseVisualStyleBackColor = true;
            this.rbCreate.CheckedChanged += new System.EventHandler(this.rbCreate_CheckedChanged);
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(30, 319);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(85, 27);
            this.btnGo.TabIndex = 13;
            this.btnGo.Text = "Go!";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // lbStatus
            // 
            this.lbStatus.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbStatus.FormattingEnabled = true;
            this.lbStatus.Location = new System.Drawing.Point(305, 0);
            this.lbStatus.Margin = new System.Windows.Forms.Padding(0);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(474, 379);
            this.lbStatus.TabIndex = 15;
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(158, 320);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(85, 27);
            this.btnClearLog.TabIndex = 14;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Operation:";
            // 
            // rbCreateLibrary
            // 
            this.rbCreateLibrary.AutoSize = true;
            this.rbCreateLibrary.Location = new System.Drawing.Point(108, 188);
            this.rbCreateLibrary.Name = "rbCreateLibrary";
            this.rbCreateLibrary.Size = new System.Drawing.Size(125, 17);
            this.rbCreateLibrary.TabIndex = 9;
            this.rbCreateLibrary.Text = "Create image libraries";
            this.rbCreateLibrary.UseVisualStyleBackColor = true;
            this.rbCreateLibrary.CheckedChanged += new System.EventHandler(this.rbCreateLibrary_CheckedChanged);
            // 
            // chkImage01
            // 
            this.chkImage01.AutoSize = true;
            this.chkImage01.Enabled = false;
            this.chkImage01.Location = new System.Drawing.Point(129, 212);
            this.chkImage01.Name = "chkImage01";
            this.chkImage01.Size = new System.Drawing.Size(80, 17);
            this.chkImage01.TabIndex = 10;
            this.chkImage01.Text = "checkBox1";
            this.chkImage01.UseVisualStyleBackColor = true;
            this.chkImage01.Visible = false;
            // 
            // chkImage02
            // 
            this.chkImage02.AutoSize = true;
            this.chkImage02.Enabled = false;
            this.chkImage02.Location = new System.Drawing.Point(129, 236);
            this.chkImage02.Name = "chkImage02";
            this.chkImage02.Size = new System.Drawing.Size(80, 17);
            this.chkImage02.TabIndex = 11;
            this.chkImage02.Text = "checkBox2";
            this.chkImage02.UseVisualStyleBackColor = true;
            this.chkImage02.Visible = false;
            // 
            // chkRequiredTables
            // 
            this.chkRequiredTables.AutoSize = true;
            this.chkRequiredTables.Checked = true;
            this.chkRequiredTables.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRequiredTables.Location = new System.Drawing.Point(129, 109);
            this.chkRequiredTables.Name = "chkRequiredTables";
            this.chkRequiredTables.Size = new System.Drawing.Size(58, 17);
            this.chkRequiredTables.TabIndex = 6;
            this.chkRequiredTables.Text = "Tables";
            this.chkRequiredTables.UseVisualStyleBackColor = true;
            // 
            // chkRequiredQueues
            // 
            this.chkRequiredQueues.AutoSize = true;
            this.chkRequiredQueues.Checked = true;
            this.chkRequiredQueues.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRequiredQueues.Location = new System.Drawing.Point(129, 132);
            this.chkRequiredQueues.Name = "chkRequiredQueues";
            this.chkRequiredQueues.Size = new System.Drawing.Size(63, 17);
            this.chkRequiredQueues.TabIndex = 7;
            this.chkRequiredQueues.Text = "Queues";
            this.chkRequiredQueues.UseVisualStyleBackColor = true;
            // 
            // chkRequiredContainers
            // 
            this.chkRequiredContainers.AutoSize = true;
            this.chkRequiredContainers.Checked = true;
            this.chkRequiredContainers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRequiredContainers.Location = new System.Drawing.Point(129, 155);
            this.chkRequiredContainers.Name = "chkRequiredContainers";
            this.chkRequiredContainers.Size = new System.Drawing.Size(76, 17);
            this.chkRequiredContainers.TabIndex = 8;
            this.chkRequiredContainers.Text = "Containers";
            this.chkRequiredContainers.UseVisualStyleBackColor = true;
            // 
            // StorageManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(779, 379);
            this.Controls.Add(this.chkRequiredContainers);
            this.Controls.Add(this.chkRequiredQueues);
            this.Controls.Add(this.chkRequiredTables);
            this.Controls.Add(this.chkImage02);
            this.Controls.Add(this.chkImage01);
            this.Controls.Add(this.rbCreateLibrary);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.rbCreate);
            this.Controls.Add(this.rbDelete);
            this.Controls.Add(this.txtKey);
            this.Controls.Add(this.txtAccount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StorageManager";
            this.Text = "Photo Mosaics Storage Manager";
            this.Load += new System.EventHandler(this.StorageManager_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAccount;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.RadioButton rbDelete;
        private System.Windows.Forms.RadioButton rbCreate;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.ListBox lbStatus;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rbCreateLibrary;
        private System.Windows.Forms.CheckBox chkImage01;
        private System.Windows.Forms.CheckBox chkImage02;
        private System.Windows.Forms.CheckBox chkRequiredTables;
        private System.Windows.Forms.CheckBox chkRequiredQueues;
        private System.Windows.Forms.CheckBox chkRequiredContainers;
    }
}

