using AkaConfig;
using AkaData;
using AkaDB;
using AkaEnum;
using AkaSerializer;
using BattleLogic;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleNotificationTest
{
    public partial class BattleNotificationForm : Form
    {
        //delegate void CrossThreadSafetySetText(Control ctl, BattleBehaviorType battleBehaviorType, IBattleBehavior battleBehavior);
        delegate void CrossThreadSafetySetNotice(Control ctl, string str);

        MyBattleTest _myBattle;
        BattleController controller;

        DateTime battleStartTime;
        int totalBulletMilliSecond = 0;

        public BattleNotificationForm()
        {
            InitializeComponent();
            Init();
            MatchingJob();
        }

        private void MatchingJob()
        {

        }

        private async Task Init()
        {
            C2SInfo.InstanceInit(Program.runMode);
            Config.AllServerInitConfig(Program.runMode);

            DBEnv.AllSetUp();

            var loader = new FileLoader(FileType.Table, Program.runMode, 0);
            var task = loader.GetFileLists();
            task.Wait();
            var fileList = task.Result;

            DataSetter dataSetter = new DataSetter();
            dataSetter.DataSet(fileList);

            textBoxPlayer1.Text = "15";
            textBoxPlayer2.Text = "16";

            textBoxPlayer1DeckNum.Text = "0";
            textBoxPlayer2DeckNum.Text = "0";
        }

        private void buttonBattleStart_Click(object sender, EventArgs e)
        {
            _myBattle = new MyBattleTest(uint.Parse(textBoxPlayer1.Text), byte.Parse(textBoxPlayer1DeckNum.Text),
                uint.Parse(textBoxPlayer2.Text), byte.Parse(textBoxPlayer2DeckNum.Text));

            Thread thread = new Thread(new ThreadStart(_myBattle.BattleStart));
            thread.IsBackground = true;
            thread.Start();
        }

        private void ButtonGetController_Click(object sender, EventArgs e)
        {
            controller = _myBattle.GetBattleController();
        }

        private void UnitsAttackProgress(Dictionary<AkaEnum.Battle.PlayerType, Player> players)
        {
            //progressBarPlayer1Unit1Attack.Minimum = 0;
            //progressBarPlayer1Unit1Attack.Maximum = players[PlayerType.Player1].Units[0].UnitBattleData.UnitStatus.AttackSpeed;
            //progressBarPlayer1Unit1Attack.Step =
            //progressBarPlayer1Unit1Attack.Value = 0;
            //progressBarPlayer1Unit1Attack.Increment

        }

        private void UnitHpView(Dictionary<AkaEnum.Battle.PlayerType, Player> players)
        {

            if (players[AkaEnum.Battle.PlayerType.Player1].Units.ContainsKey(0))
                UnitView(players[AkaEnum.Battle.PlayerType.Player1].Units[0], labelPlayer1Unit1Hp);
            else
                labelPlayer1Unit1Hp.Text = "Dead";

            if (players[AkaEnum.Battle.PlayerType.Player1].Units.ContainsKey(1))
                UnitView(players[AkaEnum.Battle.PlayerType.Player1].Units[1], labelPlayer1Unit2Hp);
            else
                labelPlayer1Unit2Hp.Text = "Dead";

            if (players[AkaEnum.Battle.PlayerType.Player1].Units.ContainsKey(2))
                UnitView(players[AkaEnum.Battle.PlayerType.Player1].Units[2], labelPlayer1Unit3Hp);
            else
                labelPlayer1Unit3Hp.Text = "Dead";

            if (players[AkaEnum.Battle.PlayerType.Player2].Units.ContainsKey(0))
                UnitView(players[AkaEnum.Battle.PlayerType.Player2].Units[0], labelPlayer2Unit1Hp);
            else
                labelPlayer2Unit1Hp.Text = "Dead";

            if (players[AkaEnum.Battle.PlayerType.Player2].Units.ContainsKey(1))
                UnitView(players[AkaEnum.Battle.PlayerType.Player2].Units[1], labelPlayer2Unit2Hp);
            else
                labelPlayer2Unit2Hp.Text = "Dead";

            if (players[AkaEnum.Battle.PlayerType.Player2].Units.ContainsKey(2))
                UnitView(players[AkaEnum.Battle.PlayerType.Player2].Units[2], labelPlayer2Unit3Hp);
            else
                labelPlayer2Unit3Hp.Text = "Dead";
        }

        private void UnitView(Unit unit, Label label)
        {
            label.Text = unit.UnitData.UnitStatus.Hp.ToString();
        }

        private void SetCardComponent(Dictionary<int, Card> handCards, Queue<Card> waitCards,
            Button hand1, Button hand2, Button hand3, Button hand4, Label wait)
        {
            for (int i = 0; i < 4; ++i)
            {
                if (handCards.ContainsKey(i))
                {
                    if (i == 0)
                        hand1.Text = handCards[i].CardStatId.ToString();
                    else if (i == 1)
                        hand2.Text = handCards[i].CardStatId.ToString();
                    else if (i == 2)
                        hand3.Text = handCards[i].CardStatId.ToString();
                    else if (i == 3)
                        hand4.Text = handCards[i].CardStatId.ToString();
                }
                else
                {
                    if (i == 0)
                        hand1.Text = "";
                    else if (i == 1)
                        hand2.Text = "";
                    else if (i == 2)
                        hand3.Text = "";
                    else if (i == 3)
                        hand4.Text = "";
                }
            }

            wait.Text = "";
            foreach (var card in waitCards)
            {
                if (card.IsDeath)
                    continue;

                wait.Text += card.DataCardStat.ToString();
                wait.Text += "|";
            }
        }

        private void ButtonPlayer1Card1Use_Click(object sender, EventArgs e)
        {
            UnitSelectPopupForm unitSelectPopupForm = new UnitSelectPopupForm(this, 0, AkaEnum.Battle.PlayerType.Player1);
            unitSelectPopupForm.StartPosition = FormStartPosition.CenterScreen;
            unitSelectPopupForm.Show();
        }

        private void ButtonPlayer1Card2Use_Click(object sender, EventArgs e)
        {
            UnitSelectPopupForm unitSelectPopupForm = new UnitSelectPopupForm(this, 1, AkaEnum.Battle.PlayerType.Player1);
            unitSelectPopupForm.StartPosition = FormStartPosition.CenterScreen;
            unitSelectPopupForm.Show();
        }

        private void ButtonPlayer1Card3Use_Click(object sender, EventArgs e)
        {
            UnitSelectPopupForm unitSelectPopupForm = new UnitSelectPopupForm(this, 2, AkaEnum.Battle.PlayerType.Player1);
            unitSelectPopupForm.StartPosition = FormStartPosition.CenterScreen;
            unitSelectPopupForm.Show();
        }

        private void ButtonPlayer1Card4Use_Click(object sender, EventArgs e)
        {
            UnitSelectPopupForm unitSelectPopupForm = new UnitSelectPopupForm(this, 3, AkaEnum.Battle.PlayerType.Player1);
            unitSelectPopupForm.StartPosition = FormStartPosition.CenterScreen;
            unitSelectPopupForm.Show();
        }

        private void ButtonPlayer2Card1Use_Click(object sender, EventArgs e)
        {
            UnitSelectPopupForm unitSelectPopupForm = new UnitSelectPopupForm(this, 0, AkaEnum.Battle.PlayerType.Player2);
            unitSelectPopupForm.StartPosition = FormStartPosition.CenterScreen;
            unitSelectPopupForm.Show();
        }

        private void ButtonPlayer2Card2Use_Click(object sender, EventArgs e)
        {
            UnitSelectPopupForm unitSelectPopupForm = new UnitSelectPopupForm(this, 1, AkaEnum.Battle.PlayerType.Player2);
            unitSelectPopupForm.StartPosition = FormStartPosition.CenterScreen;
            unitSelectPopupForm.Show();
        }

        private void ButtonPlayer2Card3Use_Click(object sender, EventArgs e)
        {
            UnitSelectPopupForm unitSelectPopupForm = new UnitSelectPopupForm(this, 2, AkaEnum.Battle.PlayerType.Player2);
            unitSelectPopupForm.StartPosition = FormStartPosition.CenterScreen;
            unitSelectPopupForm.Show();
        }

        private void ButtonPlayer2Card4Use_Click(object sender, EventArgs e)
        {
            UnitSelectPopupForm unitSelectPopupForm = new UnitSelectPopupForm(this, 3, AkaEnum.Battle.PlayerType.Player2);
            unitSelectPopupForm.StartPosition = FormStartPosition.CenterScreen;
            unitSelectPopupForm.Show();
        }

        public void CardUse(int handIndex, AkaEnum.Battle.PlayerType playerType, ProtoTarget target)
        {
            //controller.CardUse(new ProtoCardUse
            //{
            //    HandIndex = handIndex,
            //    Performer = new ProtoTarget() { PlayerType = playerType },
            //    Target = target

            //});
        }

        private void buttonBattleExit_Click(object sender, EventArgs e)
        {

        }

        private void buttonTryMatching_Click(object sender, EventArgs e)
        {
            var res = WebServerRequestor.Instance.Request(MessageType.GetBattleServer,
                AkaSerializer<ProtoGetBattleServer>.Serialize(new ProtoGetBattleServer
                {
                    MessageType = MessageType.GetBattleServer
                }));

            var battleServerInfo = AkaSerializer<ProtoOnGetBattleServer>.Deserialize(res);

            MatchingServerConnector.Instance.SetFrom(this);
            BattleServerConnector.Instance.SetFrom(this);

            MatchingServerConnector.Instance.Connect(C2SInfo.Instance.ServerInfo.MatchingServerIp, C2SInfo.Instance.ServerInfo.MatchingServerPort);

            //while (!MatchingServerConnector.Instance.IsConnected())
            //{
            //    System.Threading.Thread.Sleep(100);
            //}

            MatchingServerConnector.Instance.Send(MessageType.TryMatching, AkaSerializer<ProtoTryMatching>.Serialize(new ProtoTryMatching
            {
                BattleServerIp = battleServerInfo.BattleServerIp,
                DeckNum = byte.Parse(textBoxPlayer1DeckNum.Text),
                UserId = uint.Parse(textBoxPlayer1.Text),
                BattleType = AkaEnum.Battle.BattleType.LeagueBattle,
                //DeckModeType = ModeType.PVP,
                MessageType = MessageType.TryMatching,
            }));
            Notice("매칭시도...");
        }

        public void Notice(string str)
        {
            this.Invoke(new Action(
                delegate ()
                {
                    Notice2(str);
                }));

        }

        public void Notice2(string str)
        {
            richTextBoxNotice.Text = str + "\n" + richTextBoxNotice.Text;
        }

        public uint GetUserId()
        {
            return uint.Parse(textBoxPlayer1.Text);
        }

        public byte GetDeckNum()
        {
            return byte.Parse(textBoxPlayer1DeckNum.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var mTest = new NumberOfMatchingTest();
            mTest.Run();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MatchingServerConnector.Instance.Close();
        }

        private void buttonMatchingCancel_Click(object sender, EventArgs e)
        {
            var newMatchingConnector = new MatchingServerConnector();
            newMatchingConnector.SetFrom(this);
            newMatchingConnector.Connect(C2SInfo.Instance.ServerInfo.MatchingServerIp, C2SInfo.Instance.ServerInfo.MatchingServerPort);
            while (!newMatchingConnector.IsConnected()) ;
            newMatchingConnector.Send(MessageType.MatchingCancel,
                AkaSerializer<ProtoMatchingCancel>.Serialize(new ProtoMatchingCancel
                {
                    UserId = uint.Parse(textBoxPlayer1.Text)
                }));
            Notice("매칭취소시도...");
        }

        private void ButtonReEnterBattleRoom_Click(object sender, EventArgs e)
        {
            BattleServerConnector.Instance.Close();

            var res = WebServerRequestor.Instance.Request(MessageType.Login,
                AkaSerializer<ProtoLogin>.Serialize(new ProtoLogin
                {
                    MessageType = MessageType.Login
                }));

            var onLogin = AkaSerializer<ProtoOnLogin>.Deserialize(res);

            if (onLogin.BattlePlayingInfo == null)
            {
                Notice("전투 정보가 없습니다.");
                return;
            }

            BattleServerConnector.Instance.Connect(onLogin.BattlePlayingInfo.BattleServerIp, C2SInfo.Instance.ServerInfo.BattleServerPort);

            while (!BattleServerConnector.Instance.IsConnected()) ;
            BattleServerConnector.Instance.Send(MessageType.ReEnterRoom, AkaSerializer<ProtoReEnterRoom>.Serialize(new ProtoReEnterRoom
            {
                MessageType = MessageType.ReEnterRoom,
                UserId = uint.Parse(textBoxPlayer1.Text)
            }));
        }

        private void TryStartPveRoom_Click(object sender, EventArgs e)
        {
            BattleServerConnector.Instance.Close();

            var res = WebServerRequestor.Instance.Request(MessageType.GetBattleServer,
                AkaSerializer<ProtoGetBattleServer>.Serialize(new ProtoGetBattleServer
                {
                    MessageType = MessageType.GetBattleServer
                }));

            var battleServerInfo = AkaSerializer<ProtoOnGetBattleServer>.Deserialize(res);

            BattleServerConnector.Instance.SetFrom(this);


            BattleServerConnector.Instance.Connect(battleServerInfo.BattleServerIp, C2SInfo.Instance.ServerInfo.BattleServerPort);

            while (!BattleServerConnector.Instance.IsConnected())
            {
                System.Threading.Thread.Sleep(100);
            }

            BattleServerConnector.Instance.Send(MessageType.EnterPveRoom, AkaSerializer<ProtoEnterPveRoom>.Serialize(new ProtoEnterPveRoom
            {
                MessageType = MessageType.EnterPveRoom,
                UserId = uint.Parse(textBoxPlayer1.Text),
                StageRoundId = uint.Parse(stageRoundId.Text),
                DeckNum = byte.Parse(textBoxPlayer1DeckNum.Text),
            }));
        }

        private void ProgressBarPlayer1Unit1Attack_Click(object sender, EventArgs e)
        {

        }
    }
}
