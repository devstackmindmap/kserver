using AkaDB.MySql;
using AkaSerializer;
using CommonProtocol;
using KnightUWP.Dao;
using KnightUWP.Servicecs.Network;
using KnightUWP.Servicecs.Protocol;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonProtocol.PubSub;

namespace KnightUWP.Servicecs.Net
{
    static class Pubsub
    {
        private static void Pubsub_ConnectedEvent(UserInfo userInfo, bool connected)
        {

        }

        private static void Pubsub_DataReceived( UserInfo userInfo, DataEventArgs e )
        {
            return;
        }


        private static bool Connect(UserInfo userInfo)
        {
            if (VOProvider.Instance.EnablePubsub == false)
                return false;

            if (userInfo.PubsubConnecter?.IsConnected == true)
                return true;

            userInfo.PubsubConnecter = new ClientNetworkConnector($"{userInfo.accounts.socialAccount}_p_connector");
            userInfo.PubsubConnecter.Connect(userInfo.LoginInfo.PubSubServerIp, userInfo.LoginInfo.PubSubServerPort, Pubsub_ConnectedEvent, Pubsub_DataReceived, userInfo);

            if (false == userInfo.PubsubConnecter.WaitConnect(1000))
                return false;
            Login(userInfo);
            return true;
        }


        private static void Login(UserInfo userInfo)
        {
            var friends = userInfo.LoginInfo?.Friends?.Select(friendInfo => friendInfo.UserId).ToList() ??
                          new List<uint>();
            var clan = userInfo.LoginInfo?.ClanProfileAndMembers?.ClanProfile.ClanId ?? 0;
            var clanMemebers =
                userInfo.LoginInfo?.ClanProfileAndMembers?.ClanMembers.Select(member => member.UserId).ToList() ??
                new List<uint>();
            userInfo.PubsubConnecter.Send(MessageType.PubSubLogin, AkaSerializer<CommonProtocol.PubSub.ProtoLogin>.Serialize(new CommonProtocol.PubSub.ProtoLogin
            {
                MessageType = MessageType.PubSubLogin,
                UserId = userInfo.accounts.userId,
                FriendIds = friends,
                StateMessageType = MessageType.PubSubLogin,
                ClanMemberIds = clanMemebers,
                ClanId = clan
            }));
        }

        private static void SendState(UserInfo userInfo, MessageType message)
        {
            try
            {
                if (Connect(userInfo))
                {
                    userInfo.PubsubConnecter.Send(message, AkaSerializer<ProtoOne2One>.Serialize(new ProtoOne2One()
                    {
                        MessageType = message,
                        UserId = userInfo.accounts.userId
                    }));
                }
            }
            catch (Exception)
            {

            }
        }

        public static void IamLogin(UserInfo usreInfo)
        {
            try
            {
                Connect(usreInfo);
            }
            catch (Exception)
            {

            }
        }
        public static void Online(UserInfo userInfo)
        {
            SendState(userInfo, MessageType.PubSubOnline);
        }
        public static void Battle(UserInfo userInfo)
        {
            SendState(userInfo, MessageType.PubSubBattle);
        }

        public static void Matching(UserInfo userInfo)
        {
            SendState(userInfo, MessageType.PubSubMatchmaking);
        }


    }
}
