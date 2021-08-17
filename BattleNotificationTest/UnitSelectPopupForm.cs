using System;
using System.Windows.Forms;
using CommonProtocol;

namespace BattleNotificationTest
{
    public partial class UnitSelectPopupForm : Form
    {
        BattleNotificationForm _battleNotificationForm;
        int _handIndex;
        AkaEnum.Battle.PlayerType _playerType;

        public UnitSelectPopupForm(BattleNotificationForm battleNotificationForm, int handIndex, AkaEnum.Battle.PlayerType playerType)
        {
            InitializeComponent();
            _battleNotificationForm = battleNotificationForm;
            _handIndex = handIndex;
            _playerType = playerType;
        }

        private void buttonSelectPlayer1Unit1_Click(object sender, EventArgs e)
        {
            //BattleServerConnector.Instance.Send(MessageType.CardUse, AkaSerializer.AkaSerializer<ProtoCardUse>.Serialize(new ProtoCardUse
            //{
            //    HandIndex = _handIndex,
            //    MessageType = MessageType.CardUse,
            //    Performer = new ProtoTarget { PlayerType = PlayerInfo.Instance.PlayerType, UnitPositionIndex = 0 },
            //    RoomId = PlayerInfo.Instance.RoomId,
            //    Target = new ProtoTarget { PlayerType = PlayerInfo.Instance.PlayerType, UnitPositionIndex = 0 },
            //}));

            //_battleNotificationForm.CardUse( _handIndex, _playerType, 
            //    new ProtoTarget { PlayerType = AkaEnum.Battle.PlayerType.Player1, UnitPositionIndex = 0 } );
            this.Close();
        }

        private void buttonSelectPlayer1Unit2_Click(object sender, EventArgs e)
        {
            //BattleServerConnector.Instance.Send(MessageType.CardUse, AkaSerializer.AkaSerializer<ProtoCardUse>.Serialize(new ProtoCardUse
            //{
            //    HandIndex = _handIndex,
            //    MessageType = MessageType.CardUse,
            //    Performer = new ProtoTarget { PlayerType = PlayerInfo.Instance.PlayerType, UnitPositionIndex = 0 },
            //    RoomId = PlayerInfo.Instance.RoomId,
            //    Target = new ProtoTarget { PlayerType = PlayerInfo.Instance.PlayerType, UnitPositionIndex = 1 },

            //}));

            //_battleNotificationForm.CardUse(_handIndex, _playerType,
            //    new ProtoTarget { PlayerType = AkaEnum.Battle.PlayerType.Player1, UnitPositionIndex = 1 });
            this.Close();
        }

        private void buttonSelectPlayer1Unit3_Click(object sender, EventArgs e)
        {
            _battleNotificationForm.CardUse(_handIndex, _playerType,
                new ProtoTarget { PlayerType = AkaEnum.Battle.PlayerType.Player1, UnitPositionIndex = 2 });
            this.Close();
        }

        private void buttonSelectPlayer2Unit1_Click(object sender, EventArgs e)
        {
            _battleNotificationForm.CardUse(_handIndex, _playerType,
                new ProtoTarget { PlayerType = AkaEnum.Battle.PlayerType.Player2, UnitPositionIndex = 0 });
            this.Close();
        }

        private void buttonSelectPlayer2Unit2_Click(object sender, EventArgs e)
        {
            _battleNotificationForm.CardUse(_handIndex, _playerType,
                new ProtoTarget { PlayerType = AkaEnum.Battle.PlayerType.Player2, UnitPositionIndex = 1 });
            this.Close();
        }

        private void buttonSelectPlayer2Unit3_Click(object sender, EventArgs e)
        {
            _battleNotificationForm.CardUse(_handIndex, _playerType,
                new ProtoTarget { PlayerType = AkaEnum.Battle.PlayerType.Player2, UnitPositionIndex = 2 });
            this.Close();
        }
    }
}
