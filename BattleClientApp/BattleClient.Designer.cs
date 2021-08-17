namespace BattleClientApp
{
    partial class BattleClient
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonMakeRoom = new System.Windows.Forms.Button();
            this.buttonTryMatching = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonMakeRoom
            // 
            this.buttonMakeRoom.Location = new System.Drawing.Point(32, 50);
            this.buttonMakeRoom.Name = "buttonMakeRoom";
            this.buttonMakeRoom.Size = new System.Drawing.Size(122, 23);
            this.buttonMakeRoom.TabIndex = 0;
            this.buttonMakeRoom.Text = "MakeRoom";
            this.buttonMakeRoom.UseVisualStyleBackColor = true;
            this.buttonMakeRoom.Click += new System.EventHandler(this.buttonMakeRoom_Click);
            // 
            // buttonTryMatching
            // 
            this.buttonTryMatching.Location = new System.Drawing.Point(182, 50);
            this.buttonTryMatching.Name = "buttonTryMatching";
            this.buttonTryMatching.Size = new System.Drawing.Size(160, 23);
            this.buttonTryMatching.TabIndex = 1;
            this.buttonTryMatching.Text = "TryMatching";
            this.buttonTryMatching.UseVisualStyleBackColor = true;
            this.buttonTryMatching.Click += new System.EventHandler(this.buttonTryMatching_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(55, 170);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // BattleClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonTryMatching);
            this.Controls.Add(this.buttonMakeRoom);
            this.Name = "BattleClient";
            this.Text = "BattleClient";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonMakeRoom;
        private System.Windows.Forms.Button buttonTryMatching;
        private System.Windows.Forms.Button button1;
    }
}

