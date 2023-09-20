using System.Collections.Generic;

namespace FinaleSignalR_Client
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.openConnection = new System.Windows.Forms.Button();
            this.messageInput = new System.Windows.Forms.TextBox();
            this.sendMessage = new System.Windows.Forms.Button();
            this.messages = new System.Windows.Forms.ListBox();
            this.LeftTimer = new System.Windows.Forms.Timer(this.components);
            this.RightTimer = new System.Windows.Forms.Timer(this.components);
            this.UpTimer = new System.Windows.Forms.Timer(this.components);
            this.DownTimer = new System.Windows.Forms.Timer(this.components);
            this.ServerTimer = new System.Windows.Forms.Timer(this.components);
            this.playerBoxes = new System.Windows.Forms.PictureBox[50];
            this.SuspendLayout();
            // 
            // openConnection
            // 
            this.openConnection.Location = new System.Drawing.Point(785, 27);
            this.openConnection.Name = "openConnection";
            this.openConnection.Size = new System.Drawing.Size(151, 41);
            this.openConnection.TabIndex = 0;
            this.openConnection.Text = "Open Connection";
            this.openConnection.UseVisualStyleBackColor = true;
            this.openConnection.Click += new System.EventHandler(this.button1_Click);
            // 
            // messageInput
            // 
            this.messageInput.Location = new System.Drawing.Point(637, 136);
            this.messageInput.Name = "messageInput";
            this.messageInput.Size = new System.Drawing.Size(445, 20);
            this.messageInput.TabIndex = 1;
            // 
            // sendMessage
            // 
            this.sendMessage.Enabled = false;
            this.sendMessage.Location = new System.Drawing.Point(796, 74);
            this.sendMessage.Name = "sendMessage";
            this.sendMessage.Size = new System.Drawing.Size(140, 43);
            this.sendMessage.TabIndex = 2;
            this.sendMessage.Text = "Send Message";
            this.sendMessage.UseVisualStyleBackColor = true;
            this.sendMessage.Click += new System.EventHandler(this.sendMessage_Click);
            // 
            // messages
            // 
            this.messages.FormattingEnabled = true;
            this.messages.Location = new System.Drawing.Point(637, 179);
            this.messages.Name = "messages";
            this.messages.Size = new System.Drawing.Size(445, 173);
            this.messages.TabIndex = 3;
            // 
            // LeftTimer
            // 
            this.LeftTimer.Interval = 5;
            this.LeftTimer.Tick += new System.EventHandler(this.LeftTimer_Tick_1);
            // 
            // RightTimer
            // 
            this.RightTimer.Interval = 5;
            this.RightTimer.Tick += new System.EventHandler(this.RightTimer_Tick_1);
            // 
            // UpTimer
            // 
            this.UpTimer.Interval = 5;
            this.UpTimer.Tick += new System.EventHandler(this.UpTimer_Tick_1);
            // 
            // DownTimer
            // 
            this.DownTimer.Interval = 5;
            this.DownTimer.Tick += new System.EventHandler(this.DownTimer_Tick);
            // 
            // ServerTimer
            // 
            this.ServerTimer.Interval = 10;
            this.ServerTimer.Tick += new System.EventHandler(this.ServerTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1147, 799);
            this.Controls.Add(this.messages);
            this.Controls.Add(this.sendMessage);
            this.Controls.Add(this.messageInput);
            this.Controls.Add(this.openConnection);
            this.Name = "Form1";
            this.Text = "Form1";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyIsDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyIsUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox[] playerBoxes;
        private System.Windows.Forms.Button openConnection;
        private System.Windows.Forms.TextBox messageInput;
        private System.Windows.Forms.Button sendMessage;
        private System.Windows.Forms.ListBox messages;
        private System.Windows.Forms.Timer LeftTimer;
        private System.Windows.Forms.Timer RightTimer;
        private System.Windows.Forms.Timer UpTimer;
        private System.Windows.Forms.Timer DownTimer;
        private System.Windows.Forms.Timer ServerTimer;
    }
}

