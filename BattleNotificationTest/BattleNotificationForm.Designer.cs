using System;

namespace BattleNotificationTest
{
    partial class BattleNotificationForm
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
            this.LabelPlayer1 = new System.Windows.Forms.Label();
            this.buttonBattleStart = new System.Windows.Forms.Button();
            this.textBoxPlayer1 = new System.Windows.Forms.TextBox();
            this.labelPlayer1DeckNum = new System.Windows.Forms.Label();
            this.textBoxPlayer1DeckNum = new System.Windows.Forms.TextBox();
            this.textBoxPlayer2 = new System.Windows.Forms.TextBox();
            this.labelPlayer2 = new System.Windows.Forms.Label();
            this.textBoxPlayer2DeckNum = new System.Windows.Forms.TextBox();
            this.labelPlayer2DeckNum = new System.Windows.Forms.Label();
            this.labelBattleTime = new System.Windows.Forms.Label();
            this.labelBattleTimeView = new System.Windows.Forms.Label();
            this.labelPlayer1Unit2 = new System.Windows.Forms.Label();
            this.progressBarPlayer1Unit1Attack = new System.Windows.Forms.ProgressBar();
            this.progressBarPlayer1Unit2Attack = new System.Windows.Forms.ProgressBar();
            this.labelPlayer1WaitCards = new System.Windows.Forms.Label();
            this.ButtonGetController = new System.Windows.Forms.Button();
            this.ButtonPlayer1Card1Use = new System.Windows.Forms.Button();
            this.ButtonPlayer1Card2Use = new System.Windows.Forms.Button();
            this.ButtonPlayer1Card3Use = new System.Windows.Forms.Button();
            this.ButtonPlayer1Card4Use = new System.Windows.Forms.Button();
            this.ButtonPlayer2Card1Use = new System.Windows.Forms.Button();
            this.ButtonPlayer2Card2Use = new System.Windows.Forms.Button();
            this.ButtonPlayer2Card3Use = new System.Windows.Forms.Button();
            this.ButtonPlayer2Card4Use = new System.Windows.Forms.Button();
            this.labelPlayer2WaitCards = new System.Windows.Forms.Label();
            this.labelPlayer1Unit1 = new System.Windows.Forms.Label();
            this.labelPlayer1Unit3 = new System.Windows.Forms.Label();
            this.labelPlayer2Unit1 = new System.Windows.Forms.Label();
            this.labelPlayer2Unit2 = new System.Windows.Forms.Label();
            this.labelPlayer2Unit3 = new System.Windows.Forms.Label();
            this.labelPlayer1Hp = new System.Windows.Forms.Label();
            this.labelPlayer2Hp = new System.Windows.Forms.Label();
            this.labelPlayer1Unit1Hp = new System.Windows.Forms.Label();
            this.labelPlayer1Unit2Hp = new System.Windows.Forms.Label();
            this.labelPlayer1Unit3Hp = new System.Windows.Forms.Label();
            this.labelPlayer2Unit1Hp = new System.Windows.Forms.Label();
            this.labelPlayer2Unit2Hp = new System.Windows.Forms.Label();
            this.labelPlayer2Unit3Hp = new System.Windows.Forms.Label();
            this.progressBarPlayer1Unit3Attack = new System.Windows.Forms.ProgressBar();
            this.progressBarPlayer2Unit1Attack = new System.Windows.Forms.ProgressBar();
            this.progressBarPlayer2Unit2Attack = new System.Windows.Forms.ProgressBar();
            this.progressBarPlayer2Unit3Attack = new System.Windows.Forms.ProgressBar();
            this.labelBattleEndDateTime = new System.Windows.Forms.Label();
            this.labelBattleStartEndBetween = new System.Windows.Forms.Label();
            this.labelBattleStartDateTime = new System.Windows.Forms.Label();
            this.labelEndDateTime = new System.Windows.Forms.Label();
            this.labelBetweenTimeSpan = new System.Windows.Forms.Label();
            this.labelTotalBullet = new System.Windows.Forms.Label();
            this.labelTotalBulletTime = new System.Windows.Forms.Label();
            this.buttonBattleExit = new System.Windows.Forms.Button();
            this.buttonTryMatching = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.richTextBoxNotice = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonMatchingCancel = new System.Windows.Forms.Button();
            this.buttonReEnterBattleRoom = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.stageRoundId = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelPlayer1
            // 
            this.LabelPlayer1.AutoSize = true;
            this.LabelPlayer1.Location = new System.Drawing.Point(172, 193);
            this.LabelPlayer1.Name = "LabelPlayer1";
            this.LabelPlayer1.Size = new System.Drawing.Size(56, 15);
            this.LabelPlayer1.TabIndex = 0;
            this.LabelPlayer1.Text = "Player1";
            // 
            // buttonBattleStart
            // 
            this.buttonBattleStart.Location = new System.Drawing.Point(29, 188);
            this.buttonBattleStart.Name = "buttonBattleStart";
            this.buttonBattleStart.Size = new System.Drawing.Size(135, 25);
            this.buttonBattleStart.TabIndex = 1;
            this.buttonBattleStart.Text = "BattleStart";
            this.buttonBattleStart.UseVisualStyleBackColor = true;
            this.buttonBattleStart.Click += new System.EventHandler(this.buttonBattleStart_Click);
            // 
            // textBoxPlayer1
            // 
            this.textBoxPlayer1.Location = new System.Drawing.Point(230, 188);
            this.textBoxPlayer1.Name = "textBoxPlayer1";
            this.textBoxPlayer1.Size = new System.Drawing.Size(43, 25);
            this.textBoxPlayer1.TabIndex = 4;
            // 
            // labelPlayer1DeckNum
            // 
            this.labelPlayer1DeckNum.AutoSize = true;
            this.labelPlayer1DeckNum.Location = new System.Drawing.Point(290, 193);
            this.labelPlayer1DeckNum.Name = "labelPlayer1DeckNum";
            this.labelPlayer1DeckNum.Size = new System.Drawing.Size(117, 15);
            this.labelPlayer1DeckNum.TabIndex = 5;
            this.labelPlayer1DeckNum.Text = "Player1DeckNum";
            // 
            // textBoxPlayer1DeckNum
            // 
            this.textBoxPlayer1DeckNum.Location = new System.Drawing.Point(413, 188);
            this.textBoxPlayer1DeckNum.Name = "textBoxPlayer1DeckNum";
            this.textBoxPlayer1DeckNum.Size = new System.Drawing.Size(45, 25);
            this.textBoxPlayer1DeckNum.TabIndex = 6;
            // 
            // textBoxPlayer2
            // 
            this.textBoxPlayer2.Location = new System.Drawing.Point(539, 185);
            this.textBoxPlayer2.Name = "textBoxPlayer2";
            this.textBoxPlayer2.Size = new System.Drawing.Size(45, 25);
            this.textBoxPlayer2.TabIndex = 10;
            // 
            // labelPlayer2
            // 
            this.labelPlayer2.AutoSize = true;
            this.labelPlayer2.Location = new System.Drawing.Point(477, 188);
            this.labelPlayer2.Name = "labelPlayer2";
            this.labelPlayer2.Size = new System.Drawing.Size(56, 15);
            this.labelPlayer2.TabIndex = 9;
            this.labelPlayer2.Text = "Player2";
            // 
            // textBoxPlayer2DeckNum
            // 
            this.textBoxPlayer2DeckNum.Location = new System.Drawing.Point(713, 185);
            this.textBoxPlayer2DeckNum.Name = "textBoxPlayer2DeckNum";
            this.textBoxPlayer2DeckNum.Size = new System.Drawing.Size(50, 25);
            this.textBoxPlayer2DeckNum.TabIndex = 8;
            // 
            // labelPlayer2DeckNum
            // 
            this.labelPlayer2DeckNum.AutoSize = true;
            this.labelPlayer2DeckNum.Location = new System.Drawing.Point(590, 191);
            this.labelPlayer2DeckNum.Name = "labelPlayer2DeckNum";
            this.labelPlayer2DeckNum.Size = new System.Drawing.Size(117, 15);
            this.labelPlayer2DeckNum.TabIndex = 7;
            this.labelPlayer2DeckNum.Text = "Player2DeckNum";
            // 
            // labelBattleTime
            // 
            this.labelBattleTime.AutoSize = true;
            this.labelBattleTime.Location = new System.Drawing.Point(45, 262);
            this.labelBattleTime.Name = "labelBattleTime";
            this.labelBattleTime.Size = new System.Drawing.Size(74, 15);
            this.labelBattleTime.TabIndex = 11;
            this.labelBattleTime.Text = "BattleTime";
            // 
            // labelBattleTimeView
            // 
            this.labelBattleTimeView.AutoSize = true;
            this.labelBattleTimeView.Location = new System.Drawing.Point(45, 316);
            this.labelBattleTimeView.Name = "labelBattleTimeView";
            this.labelBattleTimeView.Size = new System.Drawing.Size(17, 15);
            this.labelBattleTimeView.TabIndex = 12;
            this.labelBattleTimeView.Text = "::";
            // 
            // labelPlayer1Unit2
            // 
            this.labelPlayer1Unit2.AutoSize = true;
            this.labelPlayer1Unit2.Location = new System.Drawing.Point(160, 400);
            this.labelPlayer1Unit2.Name = "labelPlayer1Unit2";
            this.labelPlayer1Unit2.Size = new System.Drawing.Size(40, 15);
            this.labelPlayer1Unit2.TabIndex = 14;
            this.labelPlayer1Unit2.Text = "Unit2";
            // 
            // progressBarPlayer1Unit1Attack
            // 
            this.progressBarPlayer1Unit1Attack.Location = new System.Drawing.Point(45, 366);
            this.progressBarPlayer1Unit1Attack.Name = "progressBarPlayer1Unit1Attack";
            this.progressBarPlayer1Unit1Attack.Size = new System.Drawing.Size(100, 23);
            this.progressBarPlayer1Unit1Attack.TabIndex = 15;
            this.progressBarPlayer1Unit1Attack.Click += new System.EventHandler(this.ProgressBarPlayer1Unit1Attack_Click);
            // 
            // progressBarPlayer1Unit2Attack
            // 
            this.progressBarPlayer1Unit2Attack.Location = new System.Drawing.Point(45, 400);
            this.progressBarPlayer1Unit2Attack.Name = "progressBarPlayer1Unit2Attack";
            this.progressBarPlayer1Unit2Attack.Size = new System.Drawing.Size(100, 23);
            this.progressBarPlayer1Unit2Attack.TabIndex = 16;
            // 
            // labelPlayer1WaitCards
            // 
            this.labelPlayer1WaitCards.AutoSize = true;
            this.labelPlayer1WaitCards.Location = new System.Drawing.Point(75, 477);
            this.labelPlayer1WaitCards.Name = "labelPlayer1WaitCards";
            this.labelPlayer1WaitCards.Size = new System.Drawing.Size(153, 15);
            this.labelPlayer1WaitCards.TabIndex = 17;
            this.labelPlayer1WaitCards.Text = "labelPlayer1WaitCards";
            // 
            // ButtonGetController
            // 
            this.ButtonGetController.Location = new System.Drawing.Point(789, 184);
            this.ButtonGetController.Name = "ButtonGetController";
            this.ButtonGetController.Size = new System.Drawing.Size(122, 23);
            this.ButtonGetController.TabIndex = 24;
            this.ButtonGetController.Text = "GetController";
            this.ButtonGetController.UseVisualStyleBackColor = true;
            this.ButtonGetController.Click += new System.EventHandler(this.ButtonGetController_Click);
            // 
            // ButtonPlayer1Card1Use
            // 
            this.ButtonPlayer1Card1Use.Location = new System.Drawing.Point(57, 519);
            this.ButtonPlayer1Card1Use.Name = "ButtonPlayer1Card1Use";
            this.ButtonPlayer1Card1Use.Size = new System.Drawing.Size(199, 23);
            this.ButtonPlayer1Card1Use.TabIndex = 25;
            this.ButtonPlayer1Card1Use.Text = "ButtonPlayer1Card1Use";
            this.ButtonPlayer1Card1Use.UseVisualStyleBackColor = true;
            this.ButtonPlayer1Card1Use.Click += new System.EventHandler(this.ButtonPlayer1Card1Use_Click);
            // 
            // ButtonPlayer1Card2Use
            // 
            this.ButtonPlayer1Card2Use.Location = new System.Drawing.Point(57, 563);
            this.ButtonPlayer1Card2Use.Name = "ButtonPlayer1Card2Use";
            this.ButtonPlayer1Card2Use.Size = new System.Drawing.Size(199, 23);
            this.ButtonPlayer1Card2Use.TabIndex = 26;
            this.ButtonPlayer1Card2Use.Text = "ButtonPlayer1Card2Use";
            this.ButtonPlayer1Card2Use.UseVisualStyleBackColor = true;
            this.ButtonPlayer1Card2Use.Click += new System.EventHandler(this.ButtonPlayer1Card2Use_Click);
            // 
            // ButtonPlayer1Card3Use
            // 
            this.ButtonPlayer1Card3Use.Location = new System.Drawing.Point(57, 616);
            this.ButtonPlayer1Card3Use.Name = "ButtonPlayer1Card3Use";
            this.ButtonPlayer1Card3Use.Size = new System.Drawing.Size(199, 23);
            this.ButtonPlayer1Card3Use.TabIndex = 27;
            this.ButtonPlayer1Card3Use.Text = "ButtonPlayer1Card3Use";
            this.ButtonPlayer1Card3Use.UseVisualStyleBackColor = true;
            this.ButtonPlayer1Card3Use.Click += new System.EventHandler(this.ButtonPlayer1Card3Use_Click);
            // 
            // ButtonPlayer1Card4Use
            // 
            this.ButtonPlayer1Card4Use.Location = new System.Drawing.Point(60, 664);
            this.ButtonPlayer1Card4Use.Name = "ButtonPlayer1Card4Use";
            this.ButtonPlayer1Card4Use.Size = new System.Drawing.Size(199, 23);
            this.ButtonPlayer1Card4Use.TabIndex = 28;
            this.ButtonPlayer1Card4Use.Text = "ButtonPlayer1Card4Use";
            this.ButtonPlayer1Card4Use.UseVisualStyleBackColor = true;
            this.ButtonPlayer1Card4Use.Click += new System.EventHandler(this.ButtonPlayer1Card4Use_Click);
            // 
            // ButtonPlayer2Card1Use
            // 
            this.ButtonPlayer2Card1Use.Location = new System.Drawing.Point(593, 519);
            this.ButtonPlayer2Card1Use.Name = "ButtonPlayer2Card1Use";
            this.ButtonPlayer2Card1Use.Size = new System.Drawing.Size(199, 23);
            this.ButtonPlayer2Card1Use.TabIndex = 29;
            this.ButtonPlayer2Card1Use.Text = "ButtonPlayer2Card1Use";
            this.ButtonPlayer2Card1Use.UseVisualStyleBackColor = true;
            this.ButtonPlayer2Card1Use.Click += new System.EventHandler(this.ButtonPlayer2Card1Use_Click);
            // 
            // ButtonPlayer2Card2Use
            // 
            this.ButtonPlayer2Card2Use.Location = new System.Drawing.Point(593, 563);
            this.ButtonPlayer2Card2Use.Name = "ButtonPlayer2Card2Use";
            this.ButtonPlayer2Card2Use.Size = new System.Drawing.Size(199, 23);
            this.ButtonPlayer2Card2Use.TabIndex = 30;
            this.ButtonPlayer2Card2Use.Text = "ButtonPlayer2Card2Use";
            this.ButtonPlayer2Card2Use.UseVisualStyleBackColor = true;
            this.ButtonPlayer2Card2Use.Click += new System.EventHandler(this.ButtonPlayer2Card2Use_Click);
            // 
            // ButtonPlayer2Card3Use
            // 
            this.ButtonPlayer2Card3Use.Location = new System.Drawing.Point(593, 616);
            this.ButtonPlayer2Card3Use.Name = "ButtonPlayer2Card3Use";
            this.ButtonPlayer2Card3Use.Size = new System.Drawing.Size(199, 23);
            this.ButtonPlayer2Card3Use.TabIndex = 31;
            this.ButtonPlayer2Card3Use.Text = "ButtonPlayer2Card3Use";
            this.ButtonPlayer2Card3Use.UseVisualStyleBackColor = true;
            this.ButtonPlayer2Card3Use.Click += new System.EventHandler(this.ButtonPlayer2Card3Use_Click);
            // 
            // ButtonPlayer2Card4Use
            // 
            this.ButtonPlayer2Card4Use.Location = new System.Drawing.Point(593, 664);
            this.ButtonPlayer2Card4Use.Name = "ButtonPlayer2Card4Use";
            this.ButtonPlayer2Card4Use.Size = new System.Drawing.Size(199, 23);
            this.ButtonPlayer2Card4Use.TabIndex = 32;
            this.ButtonPlayer2Card4Use.Text = "ButtonPlayer2Card4Use";
            this.ButtonPlayer2Card4Use.UseVisualStyleBackColor = true;
            this.ButtonPlayer2Card4Use.Click += new System.EventHandler(this.ButtonPlayer2Card4Use_Click);
            // 
            // labelPlayer2WaitCards
            // 
            this.labelPlayer2WaitCards.AutoSize = true;
            this.labelPlayer2WaitCards.Location = new System.Drawing.Point(600, 477);
            this.labelPlayer2WaitCards.Name = "labelPlayer2WaitCards";
            this.labelPlayer2WaitCards.Size = new System.Drawing.Size(153, 15);
            this.labelPlayer2WaitCards.TabIndex = 33;
            this.labelPlayer2WaitCards.Text = "labelPlayer2WaitCards";
            // 
            // labelPlayer1Unit1
            // 
            this.labelPlayer1Unit1.AutoSize = true;
            this.labelPlayer1Unit1.Location = new System.Drawing.Point(160, 366);
            this.labelPlayer1Unit1.Name = "labelPlayer1Unit1";
            this.labelPlayer1Unit1.Size = new System.Drawing.Size(40, 15);
            this.labelPlayer1Unit1.TabIndex = 13;
            this.labelPlayer1Unit1.Text = "Unit1";
            // 
            // labelPlayer1Unit3
            // 
            this.labelPlayer1Unit3.AutoSize = true;
            this.labelPlayer1Unit3.Location = new System.Drawing.Point(160, 437);
            this.labelPlayer1Unit3.Name = "labelPlayer1Unit3";
            this.labelPlayer1Unit3.Size = new System.Drawing.Size(40, 15);
            this.labelPlayer1Unit3.TabIndex = 34;
            this.labelPlayer1Unit3.Text = "Unit3";
            // 
            // labelPlayer2Unit1
            // 
            this.labelPlayer2Unit1.AutoSize = true;
            this.labelPlayer2Unit1.Location = new System.Drawing.Point(579, 366);
            this.labelPlayer2Unit1.Name = "labelPlayer2Unit1";
            this.labelPlayer2Unit1.Size = new System.Drawing.Size(40, 15);
            this.labelPlayer2Unit1.TabIndex = 35;
            this.labelPlayer2Unit1.Text = "Unit1";
            // 
            // labelPlayer2Unit2
            // 
            this.labelPlayer2Unit2.AutoSize = true;
            this.labelPlayer2Unit2.Location = new System.Drawing.Point(579, 400);
            this.labelPlayer2Unit2.Name = "labelPlayer2Unit2";
            this.labelPlayer2Unit2.Size = new System.Drawing.Size(40, 15);
            this.labelPlayer2Unit2.TabIndex = 36;
            this.labelPlayer2Unit2.Text = "Unit2";
            // 
            // labelPlayer2Unit3
            // 
            this.labelPlayer2Unit3.AutoSize = true;
            this.labelPlayer2Unit3.Location = new System.Drawing.Point(579, 437);
            this.labelPlayer2Unit3.Name = "labelPlayer2Unit3";
            this.labelPlayer2Unit3.Size = new System.Drawing.Size(40, 15);
            this.labelPlayer2Unit3.TabIndex = 37;
            this.labelPlayer2Unit3.Text = "Unit3";
            // 
            // labelPlayer1Hp
            // 
            this.labelPlayer1Hp.AutoSize = true;
            this.labelPlayer1Hp.Location = new System.Drawing.Point(230, 344);
            this.labelPlayer1Hp.Name = "labelPlayer1Hp";
            this.labelPlayer1Hp.Size = new System.Drawing.Size(25, 15);
            this.labelPlayer1Hp.TabIndex = 38;
            this.labelPlayer1Hp.Text = "Hp";
            // 
            // labelPlayer2Hp
            // 
            this.labelPlayer2Hp.AutoSize = true;
            this.labelPlayer2Hp.Location = new System.Drawing.Point(641, 344);
            this.labelPlayer2Hp.Name = "labelPlayer2Hp";
            this.labelPlayer2Hp.Size = new System.Drawing.Size(25, 15);
            this.labelPlayer2Hp.TabIndex = 39;
            this.labelPlayer2Hp.Text = "Hp";
            // 
            // labelPlayer1Unit1Hp
            // 
            this.labelPlayer1Unit1Hp.AutoSize = true;
            this.labelPlayer1Unit1Hp.Location = new System.Drawing.Point(220, 366);
            this.labelPlayer1Unit1Hp.Name = "labelPlayer1Unit1Hp";
            this.labelPlayer1Unit1Hp.Size = new System.Drawing.Size(0, 15);
            this.labelPlayer1Unit1Hp.TabIndex = 40;
            // 
            // labelPlayer1Unit2Hp
            // 
            this.labelPlayer1Unit2Hp.AutoSize = true;
            this.labelPlayer1Unit2Hp.Location = new System.Drawing.Point(220, 400);
            this.labelPlayer1Unit2Hp.Name = "labelPlayer1Unit2Hp";
            this.labelPlayer1Unit2Hp.Size = new System.Drawing.Size(0, 15);
            this.labelPlayer1Unit2Hp.TabIndex = 41;
            // 
            // labelPlayer1Unit3Hp
            // 
            this.labelPlayer1Unit3Hp.AutoSize = true;
            this.labelPlayer1Unit3Hp.Location = new System.Drawing.Point(220, 437);
            this.labelPlayer1Unit3Hp.Name = "labelPlayer1Unit3Hp";
            this.labelPlayer1Unit3Hp.Size = new System.Drawing.Size(0, 15);
            this.labelPlayer1Unit3Hp.TabIndex = 42;
            // 
            // labelPlayer2Unit1Hp
            // 
            this.labelPlayer2Unit1Hp.AutoSize = true;
            this.labelPlayer2Unit1Hp.Location = new System.Drawing.Point(641, 366);
            this.labelPlayer2Unit1Hp.Name = "labelPlayer2Unit1Hp";
            this.labelPlayer2Unit1Hp.Size = new System.Drawing.Size(0, 15);
            this.labelPlayer2Unit1Hp.TabIndex = 43;
            // 
            // labelPlayer2Unit2Hp
            // 
            this.labelPlayer2Unit2Hp.AutoSize = true;
            this.labelPlayer2Unit2Hp.Location = new System.Drawing.Point(641, 400);
            this.labelPlayer2Unit2Hp.Name = "labelPlayer2Unit2Hp";
            this.labelPlayer2Unit2Hp.Size = new System.Drawing.Size(0, 15);
            this.labelPlayer2Unit2Hp.TabIndex = 44;
            // 
            // labelPlayer2Unit3Hp
            // 
            this.labelPlayer2Unit3Hp.AutoSize = true;
            this.labelPlayer2Unit3Hp.Location = new System.Drawing.Point(641, 437);
            this.labelPlayer2Unit3Hp.Name = "labelPlayer2Unit3Hp";
            this.labelPlayer2Unit3Hp.Size = new System.Drawing.Size(0, 15);
            this.labelPlayer2Unit3Hp.TabIndex = 45;
            // 
            // progressBarPlayer1Unit3Attack
            // 
            this.progressBarPlayer1Unit3Attack.Location = new System.Drawing.Point(45, 437);
            this.progressBarPlayer1Unit3Attack.Name = "progressBarPlayer1Unit3Attack";
            this.progressBarPlayer1Unit3Attack.Size = new System.Drawing.Size(100, 23);
            this.progressBarPlayer1Unit3Attack.TabIndex = 46;
            // 
            // progressBarPlayer2Unit1Attack
            // 
            this.progressBarPlayer2Unit1Attack.Location = new System.Drawing.Point(473, 366);
            this.progressBarPlayer2Unit1Attack.Name = "progressBarPlayer2Unit1Attack";
            this.progressBarPlayer2Unit1Attack.Size = new System.Drawing.Size(100, 23);
            this.progressBarPlayer2Unit1Attack.TabIndex = 47;
            // 
            // progressBarPlayer2Unit2Attack
            // 
            this.progressBarPlayer2Unit2Attack.Location = new System.Drawing.Point(473, 400);
            this.progressBarPlayer2Unit2Attack.Name = "progressBarPlayer2Unit2Attack";
            this.progressBarPlayer2Unit2Attack.Size = new System.Drawing.Size(100, 23);
            this.progressBarPlayer2Unit2Attack.TabIndex = 48;
            // 
            // progressBarPlayer2Unit3Attack
            // 
            this.progressBarPlayer2Unit3Attack.Location = new System.Drawing.Point(473, 437);
            this.progressBarPlayer2Unit3Attack.Name = "progressBarPlayer2Unit3Attack";
            this.progressBarPlayer2Unit3Attack.Size = new System.Drawing.Size(100, 23);
            this.progressBarPlayer2Unit3Attack.TabIndex = 49;
            // 
            // labelBattleEndDateTime
            // 
            this.labelBattleEndDateTime.AutoSize = true;
            this.labelBattleEndDateTime.Location = new System.Drawing.Point(220, 316);
            this.labelBattleEndDateTime.Name = "labelBattleEndDateTime";
            this.labelBattleEndDateTime.Size = new System.Drawing.Size(17, 15);
            this.labelBattleEndDateTime.TabIndex = 50;
            this.labelBattleEndDateTime.Text = "::";
            // 
            // labelBattleStartEndBetween
            // 
            this.labelBattleStartEndBetween.AutoSize = true;
            this.labelBattleStartEndBetween.Location = new System.Drawing.Point(410, 316);
            this.labelBattleStartEndBetween.Name = "labelBattleStartEndBetween";
            this.labelBattleStartEndBetween.Size = new System.Drawing.Size(17, 15);
            this.labelBattleStartEndBetween.TabIndex = 51;
            this.labelBattleStartEndBetween.Text = "::";
            // 
            // labelBattleStartDateTime
            // 
            this.labelBattleStartDateTime.AutoSize = true;
            this.labelBattleStartDateTime.Location = new System.Drawing.Point(42, 290);
            this.labelBattleStartDateTime.Name = "labelBattleStartDateTime";
            this.labelBattleStartDateTime.Size = new System.Drawing.Size(37, 15);
            this.labelBattleStartDateTime.TabIndex = 52;
            this.labelBattleStartDateTime.Text = "Start";
            // 
            // labelEndDateTime
            // 
            this.labelEndDateTime.AutoSize = true;
            this.labelEndDateTime.Location = new System.Drawing.Point(220, 290);
            this.labelEndDateTime.Name = "labelEndDateTime";
            this.labelEndDateTime.Size = new System.Drawing.Size(32, 15);
            this.labelEndDateTime.TabIndex = 53;
            this.labelEndDateTime.Text = "End";
            // 
            // labelBetweenTimeSpan
            // 
            this.labelBetweenTimeSpan.AutoSize = true;
            this.labelBetweenTimeSpan.Location = new System.Drawing.Point(410, 290);
            this.labelBetweenTimeSpan.Name = "labelBetweenTimeSpan";
            this.labelBetweenTimeSpan.Size = new System.Drawing.Size(63, 15);
            this.labelBetweenTimeSpan.TabIndex = 54;
            this.labelBetweenTimeSpan.Text = "Between";
            // 
            // labelTotalBullet
            // 
            this.labelTotalBullet.AutoSize = true;
            this.labelTotalBullet.Location = new System.Drawing.Point(603, 290);
            this.labelTotalBullet.Name = "labelTotalBullet";
            this.labelTotalBullet.Size = new System.Drawing.Size(105, 15);
            this.labelTotalBullet.TabIndex = 55;
            this.labelTotalBullet.Text = "TotalBulletTime";
            // 
            // labelTotalBulletTime
            // 
            this.labelTotalBulletTime.AutoSize = true;
            this.labelTotalBulletTime.Location = new System.Drawing.Point(606, 316);
            this.labelTotalBulletTime.Name = "labelTotalBulletTime";
            this.labelTotalBulletTime.Size = new System.Drawing.Size(17, 15);
            this.labelTotalBulletTime.TabIndex = 56;
            this.labelTotalBulletTime.Text = "::";
            // 
            // buttonBattleExit
            // 
            this.buttonBattleExit.Location = new System.Drawing.Point(29, 219);
            this.buttonBattleExit.Name = "buttonBattleExit";
            this.buttonBattleExit.Size = new System.Drawing.Size(135, 23);
            this.buttonBattleExit.TabIndex = 57;
            this.buttonBattleExit.Text = "BattleExit";
            this.buttonBattleExit.UseVisualStyleBackColor = true;
            this.buttonBattleExit.Click += new System.EventHandler(this.buttonBattleExit_Click);
            // 
            // buttonTryMatching
            // 
            this.buttonTryMatching.Location = new System.Drawing.Point(1, 20);
            this.buttonTryMatching.Name = "buttonTryMatching";
            this.buttonTryMatching.Size = new System.Drawing.Size(75, 23);
            this.buttonTryMatching.TabIndex = 58;
            this.buttonTryMatching.Text = "매칭시도";
            this.buttonTryMatching.UseVisualStyleBackColor = true;
            this.buttonTryMatching.Click += new System.EventHandler(this.buttonTryMatching_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.richTextBoxNotice);
            this.panel1.Location = new System.Drawing.Point(155, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(768, 166);
            this.panel1.TabIndex = 59;
            // 
            // richTextBoxNotice
            // 
            this.richTextBoxNotice.Location = new System.Drawing.Point(3, 3);
            this.richTextBoxNotice.Name = "richTextBoxNotice";
            this.richTextBoxNotice.Size = new System.Drawing.Size(759, 160);
            this.richTextBoxNotice.TabIndex = 0;
            this.richTextBoxNotice.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 82);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(133, 23);
            this.button1.TabIndex = 60;
            this.button1.Text = "대량매칭테스트";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(44, 114);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 61;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonMatchingCancel
            // 
            this.buttonMatchingCancel.Location = new System.Drawing.Point(1, 49);
            this.buttonMatchingCancel.Name = "buttonMatchingCancel";
            this.buttonMatchingCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonMatchingCancel.TabIndex = 62;
            this.buttonMatchingCancel.Text = "매칭취소";
            this.buttonMatchingCancel.UseVisualStyleBackColor = true;
            this.buttonMatchingCancel.Click += new System.EventHandler(this.buttonMatchingCancel_Click);
            // 
            // buttonReEnterBattleRoom
            // 
            this.buttonReEnterBattleRoom.Location = new System.Drawing.Point(79, 20);
            this.buttonReEnterBattleRoom.Name = "buttonReEnterBattleRoom";
            this.buttonReEnterBattleRoom.Size = new System.Drawing.Size(75, 23);
            this.buttonReEnterBattleRoom.TabIndex = 63;
            this.buttonReEnterBattleRoom.Text = "재진입";
            this.buttonReEnterBattleRoom.UseVisualStyleBackColor = true;
            this.buttonReEnterBattleRoom.Click += new System.EventHandler(this.ButtonReEnterBattleRoom_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(172, 223);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 15);
            this.label1.TabIndex = 64;
            this.label1.Text = "StageRound";
            // 
            // stageRoundId
            // 
            this.stageRoundId.Location = new System.Drawing.Point(266, 217);
            this.stageRoundId.Name = "stageRoundId";
            this.stageRoundId.Size = new System.Drawing.Size(80, 25);
            this.stageRoundId.TabIndex = 65;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(352, 219);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 66;
            this.button3.Text = "Pve시작";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.TryStartPveRoom_Click);
            // 
            // BattleNotificationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(923, 715);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.stageRoundId);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonReEnterBattleRoom);
            this.Controls.Add(this.buttonMatchingCancel);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonTryMatching);
            this.Controls.Add(this.buttonBattleExit);
            this.Controls.Add(this.labelTotalBulletTime);
            this.Controls.Add(this.labelTotalBullet);
            this.Controls.Add(this.labelBetweenTimeSpan);
            this.Controls.Add(this.labelEndDateTime);
            this.Controls.Add(this.labelBattleStartDateTime);
            this.Controls.Add(this.labelBattleStartEndBetween);
            this.Controls.Add(this.labelBattleEndDateTime);
            this.Controls.Add(this.progressBarPlayer2Unit3Attack);
            this.Controls.Add(this.progressBarPlayer2Unit2Attack);
            this.Controls.Add(this.progressBarPlayer2Unit1Attack);
            this.Controls.Add(this.progressBarPlayer1Unit3Attack);
            this.Controls.Add(this.labelPlayer2Unit3Hp);
            this.Controls.Add(this.labelPlayer2Unit2Hp);
            this.Controls.Add(this.labelPlayer2Unit1Hp);
            this.Controls.Add(this.labelPlayer1Unit3Hp);
            this.Controls.Add(this.labelPlayer1Unit2Hp);
            this.Controls.Add(this.labelPlayer1Unit1Hp);
            this.Controls.Add(this.labelPlayer2Hp);
            this.Controls.Add(this.labelPlayer1Hp);
            this.Controls.Add(this.labelPlayer2Unit3);
            this.Controls.Add(this.labelPlayer2Unit2);
            this.Controls.Add(this.labelPlayer2Unit1);
            this.Controls.Add(this.labelPlayer1Unit3);
            this.Controls.Add(this.labelPlayer2WaitCards);
            this.Controls.Add(this.ButtonPlayer2Card4Use);
            this.Controls.Add(this.ButtonPlayer2Card3Use);
            this.Controls.Add(this.ButtonPlayer2Card2Use);
            this.Controls.Add(this.ButtonPlayer2Card1Use);
            this.Controls.Add(this.ButtonPlayer1Card4Use);
            this.Controls.Add(this.ButtonPlayer1Card3Use);
            this.Controls.Add(this.ButtonPlayer1Card2Use);
            this.Controls.Add(this.ButtonPlayer1Card1Use);
            this.Controls.Add(this.ButtonGetController);
            this.Controls.Add(this.labelPlayer1WaitCards);
            this.Controls.Add(this.progressBarPlayer1Unit2Attack);
            this.Controls.Add(this.progressBarPlayer1Unit1Attack);
            this.Controls.Add(this.labelPlayer1Unit2);
            this.Controls.Add(this.labelPlayer1Unit1);
            this.Controls.Add(this.labelBattleTimeView);
            this.Controls.Add(this.labelBattleTime);
            this.Controls.Add(this.textBoxPlayer2);
            this.Controls.Add(this.labelPlayer2);
            this.Controls.Add(this.textBoxPlayer2DeckNum);
            this.Controls.Add(this.labelPlayer2DeckNum);
            this.Controls.Add(this.textBoxPlayer1DeckNum);
            this.Controls.Add(this.labelPlayer1DeckNum);
            this.Controls.Add(this.textBoxPlayer1);
            this.Controls.Add(this.buttonBattleStart);
            this.Controls.Add(this.LabelPlayer1);
            this.Name = "BattleNotificationForm";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabelPlayer1;
        private System.Windows.Forms.Button buttonBattleStart;
        private System.Windows.Forms.TextBox textBoxPlayer1;
        private System.Windows.Forms.Label labelPlayer1DeckNum;
        private System.Windows.Forms.TextBox textBoxPlayer1DeckNum;
        private System.Windows.Forms.TextBox textBoxPlayer2;
        private System.Windows.Forms.Label labelPlayer2;
        private System.Windows.Forms.TextBox textBoxPlayer2DeckNum;
        private System.Windows.Forms.Label labelPlayer2DeckNum;
        private System.Windows.Forms.Label labelBattleTime;
        private System.Windows.Forms.Label labelBattleTimeView;
        private System.Windows.Forms.Label labelPlayer1Unit2;
        private System.Windows.Forms.ProgressBar progressBarPlayer1Unit1Attack;
        private System.Windows.Forms.ProgressBar progressBarPlayer1Unit2Attack;
        private System.Windows.Forms.Label labelPlayer1WaitCards;
        private System.Windows.Forms.Button ButtonGetController;
        private System.Windows.Forms.Button ButtonPlayer1Card1Use;
        private System.Windows.Forms.Button ButtonPlayer1Card2Use;
        private System.Windows.Forms.Button ButtonPlayer1Card3Use;
        private System.Windows.Forms.Button ButtonPlayer1Card4Use;
        private System.Windows.Forms.Button ButtonPlayer2Card1Use;
        private System.Windows.Forms.Button ButtonPlayer2Card2Use;
        private System.Windows.Forms.Button ButtonPlayer2Card3Use;
        private System.Windows.Forms.Button ButtonPlayer2Card4Use;
        private System.Windows.Forms.Label labelPlayer2WaitCards;
        private System.Windows.Forms.Label labelPlayer1Unit1;
        private System.Windows.Forms.Label labelPlayer1Unit3;
        private System.Windows.Forms.Label labelPlayer2Unit1;
        private System.Windows.Forms.Label labelPlayer2Unit2;
        private System.Windows.Forms.Label labelPlayer2Unit3;
        private System.Windows.Forms.Label labelPlayer1Hp;
        private System.Windows.Forms.Label labelPlayer2Hp;
        private System.Windows.Forms.Label labelPlayer1Unit1Hp;
        private System.Windows.Forms.Label labelPlayer1Unit2Hp;
        private System.Windows.Forms.Label labelPlayer1Unit3Hp;
        private System.Windows.Forms.Label labelPlayer2Unit1Hp;
        private System.Windows.Forms.Label labelPlayer2Unit2Hp;
        private System.Windows.Forms.Label labelPlayer2Unit3Hp;
        private System.Windows.Forms.ProgressBar progressBarPlayer1Unit3Attack;
        private System.Windows.Forms.ProgressBar progressBarPlayer2Unit1Attack;
        private System.Windows.Forms.ProgressBar progressBarPlayer2Unit2Attack;
        private System.Windows.Forms.ProgressBar progressBarPlayer2Unit3Attack;
        private System.Windows.Forms.Label labelBattleEndDateTime;
        private System.Windows.Forms.Label labelBattleStartEndBetween;
        private System.Windows.Forms.Label labelBattleStartDateTime;
        private System.Windows.Forms.Label labelEndDateTime;
        private System.Windows.Forms.Label labelBetweenTimeSpan;
        private System.Windows.Forms.Label labelTotalBullet;
        private System.Windows.Forms.Label labelTotalBulletTime;
        private System.Windows.Forms.Button buttonBattleExit;
        private System.Windows.Forms.Button buttonTryMatching;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox richTextBoxNotice;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button buttonMatchingCancel;
        private System.Windows.Forms.Button buttonReEnterBattleRoom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox stageRoundId;
        private System.Windows.Forms.Button button3;
    }
}

