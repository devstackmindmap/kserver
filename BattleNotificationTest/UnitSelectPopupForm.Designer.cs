namespace BattleNotificationTest
{
    partial class UnitSelectPopupForm
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
            this.buttonSelectPlayer1Unit1 = new System.Windows.Forms.Button();
            this.buttonSelectPlayer1Unit2 = new System.Windows.Forms.Button();
            this.buttonSelectPlayer1Unit3 = new System.Windows.Forms.Button();
            this.buttonSelectPlayer2Unit1 = new System.Windows.Forms.Button();
            this.buttonSelectPlayer2Unit2 = new System.Windows.Forms.Button();
            this.buttonSelectPlayer2Unit3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSelectPlayer1Unit1
            // 
            this.buttonSelectPlayer1Unit1.Location = new System.Drawing.Point(22, 21);
            this.buttonSelectPlayer1Unit1.Name = "buttonSelectPlayer1Unit1";
            this.buttonSelectPlayer1Unit1.Size = new System.Drawing.Size(75, 23);
            this.buttonSelectPlayer1Unit1.TabIndex = 0;
            this.buttonSelectPlayer1Unit1.Text = "P1Unit1";
            this.buttonSelectPlayer1Unit1.UseVisualStyleBackColor = true;
            this.buttonSelectPlayer1Unit1.Click += new System.EventHandler(this.buttonSelectPlayer1Unit1_Click);
            // 
            // buttonSelectPlayer1Unit2
            // 
            this.buttonSelectPlayer1Unit2.Location = new System.Drawing.Point(22, 50);
            this.buttonSelectPlayer1Unit2.Name = "buttonSelectPlayer1Unit2";
            this.buttonSelectPlayer1Unit2.Size = new System.Drawing.Size(75, 23);
            this.buttonSelectPlayer1Unit2.TabIndex = 1;
            this.buttonSelectPlayer1Unit2.Text = "P1Unit2";
            this.buttonSelectPlayer1Unit2.UseVisualStyleBackColor = true;
            this.buttonSelectPlayer1Unit2.Click += new System.EventHandler(this.buttonSelectPlayer1Unit2_Click);
            // 
            // buttonSelectPlayer1Unit3
            // 
            this.buttonSelectPlayer1Unit3.Location = new System.Drawing.Point(22, 79);
            this.buttonSelectPlayer1Unit3.Name = "buttonSelectPlayer1Unit3";
            this.buttonSelectPlayer1Unit3.Size = new System.Drawing.Size(75, 23);
            this.buttonSelectPlayer1Unit3.TabIndex = 2;
            this.buttonSelectPlayer1Unit3.Text = "P1Unit3";
            this.buttonSelectPlayer1Unit3.UseVisualStyleBackColor = true;
            this.buttonSelectPlayer1Unit3.Click += new System.EventHandler(this.buttonSelectPlayer1Unit3_Click);
            // 
            // buttonSelectPlayer2Unit1
            // 
            this.buttonSelectPlayer2Unit1.Location = new System.Drawing.Point(126, 21);
            this.buttonSelectPlayer2Unit1.Name = "buttonSelectPlayer2Unit1";
            this.buttonSelectPlayer2Unit1.Size = new System.Drawing.Size(75, 23);
            this.buttonSelectPlayer2Unit1.TabIndex = 3;
            this.buttonSelectPlayer2Unit1.Text = "P2Unit1";
            this.buttonSelectPlayer2Unit1.UseVisualStyleBackColor = true;
            this.buttonSelectPlayer2Unit1.Click += new System.EventHandler(this.buttonSelectPlayer2Unit1_Click);
            // 
            // buttonSelectPlayer2Unit2
            // 
            this.buttonSelectPlayer2Unit2.Location = new System.Drawing.Point(126, 50);
            this.buttonSelectPlayer2Unit2.Name = "buttonSelectPlayer2Unit2";
            this.buttonSelectPlayer2Unit2.Size = new System.Drawing.Size(75, 23);
            this.buttonSelectPlayer2Unit2.TabIndex = 4;
            this.buttonSelectPlayer2Unit2.Text = "P2Unit2";
            this.buttonSelectPlayer2Unit2.UseVisualStyleBackColor = true;
            this.buttonSelectPlayer2Unit2.Click += new System.EventHandler(this.buttonSelectPlayer2Unit2_Click);
            // 
            // buttonSelectPlayer2Unit3
            // 
            this.buttonSelectPlayer2Unit3.Location = new System.Drawing.Point(126, 79);
            this.buttonSelectPlayer2Unit3.Name = "buttonSelectPlayer2Unit3";
            this.buttonSelectPlayer2Unit3.Size = new System.Drawing.Size(75, 23);
            this.buttonSelectPlayer2Unit3.TabIndex = 5;
            this.buttonSelectPlayer2Unit3.Text = "P2Unit3";
            this.buttonSelectPlayer2Unit3.UseVisualStyleBackColor = true;
            this.buttonSelectPlayer2Unit3.Click += new System.EventHandler(this.buttonSelectPlayer2Unit3_Click);
            // 
            // UnitSelectPopupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 123);
            this.Controls.Add(this.buttonSelectPlayer2Unit3);
            this.Controls.Add(this.buttonSelectPlayer2Unit2);
            this.Controls.Add(this.buttonSelectPlayer2Unit1);
            this.Controls.Add(this.buttonSelectPlayer1Unit3);
            this.Controls.Add(this.buttonSelectPlayer1Unit2);
            this.Controls.Add(this.buttonSelectPlayer1Unit1);
            this.Name = "UnitSelectPopupForm";
            this.Text = "UnitSelectPopupForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSelectPlayer1Unit1;
        private System.Windows.Forms.Button buttonSelectPlayer1Unit2;
        private System.Windows.Forms.Button buttonSelectPlayer1Unit3;
        private System.Windows.Forms.Button buttonSelectPlayer2Unit1;
        private System.Windows.Forms.Button buttonSelectPlayer2Unit2;
        private System.Windows.Forms.Button buttonSelectPlayer2Unit3;
    }
}