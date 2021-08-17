using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.PubSubTest
{
    public class PubSubReceive
    {
        public static void Run(BaseProtocol proto, MessageType msgType)
        {
            if (msgType == MessageType.PubSubOnLogin)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"친구 {res.UserId} 가 접속 했습니다.");
            }
            else if (msgType == MessageType.PubSubOnOtherLogin)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"내({res.UserId}) 가 다른 곳에서 접속 했습니다.");
            }
            else if (msgType == MessageType.PubSubLogout)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"친구 {res.UserId} 가 게임을 종료했습니다.");
            }

            else if (msgType == MessageType.PubSubMatchmaking)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"친구 {res.UserId} 가 매치 메이킹 중 입니다.");
            }

            else if (msgType == MessageType.PubSubBattle)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"친구 {res.UserId} 가 전투 중 입니다.");
            }

            else if (msgType == MessageType.PubSubOnline)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"친구 {res.UserId} 가 온라인 입니다.");
            }

            else if (msgType == MessageType.PubSubOnFriendlyBattleInvite)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"친구 {res.UserId} 가 친선전 초대하였습니다.");
            }

            else if (msgType == MessageType.PubSubOnFriendlyBattleAccept)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"친구 {res.UserId} 가 친선전 초대 수락하였습니다.");
            }

            else if (msgType == MessageType.PubSubOnFriendlyBattleDecline)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"친구 {res.UserId} 가 친선전 초대 거절하였습니다.");
            }

            else if (msgType == MessageType.PubSubOnFriendlyBattleCancel)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"친구 {res.UserId} 가 친선전이 취소되었습니다.");
            }

            else if (msgType == MessageType.PubSubOnFriendlyBattleReady)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"친구 {res.UserId} 가 친선전 준비완료 되었습니다.");
            }

            else if (msgType == MessageType.PubSubOnFriendlyBattleReadyCancel)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"친구 {res.UserId} 가 친선전 준비해제 되었습니다.");
            }

            else if (msgType == MessageType.PubSubOnFriendAsked)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"유저 {res.UserId} 가 친구 신청하였습니다.");
            }
            else if (msgType == MessageType.PubSubOnFriendSigned)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"유저 {res.UserId} 와 친구가 되었습니다.");
            }

            else if (msgType == MessageType.PubSubOnFriendRemoved)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"유저 {res.UserId} 와 친구가 해제 되었습니다");
            }
            else if (msgType == MessageType.PubSubPublicNotice)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"공지");
            }
            else if (msgType == MessageType.PubSubOnClanJoin)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"유저 {res.UserId} 가 클랜에 가입했습니다.");
            }
            else if (msgType == MessageType.PubSubOnClanOut)
            {
                var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                Console.WriteLine($"유저 {res.UserId} 가 클랜에 탈퇴했습니다.");
            }
            else if (msgType == MessageType.PubSubOnClanBanish)
            {
                var res = proto as ProtoUserIdTargetId;

                Console.WriteLine($"{res.UserId} 유저가 {res.TargetId} 유저를 추방하였습니다.");
            }
            else if (msgType == MessageType.PubSubOnClanModifyMemberGradeUp)
            {
                var res = proto as ProtoUserIdTargetId;

                Console.WriteLine($"{res.UserId} 유저가 {res.TargetId} 유저를 등급 상승 하였습니다.");
            }
            else if (msgType == MessageType.PubSubOnClanModifyMemberGradeDown)
            {
                var res = proto as ProtoUserIdTargetId;

                Console.WriteLine($"{res.UserId} 유저가 {res.TargetId} 유저를 등급 하락 하였습니다.");
            }
            else if (msgType == MessageType.PubSubClanProfileModify)
            {
                var res = proto as CommonProtocol.PubSub.ProtoClanId;

                Console.WriteLine($"클랜 프로필이 바뀌었습니다.");
            }
            else if (msgType == MessageType.PubSubOnClanChatting)
            {
                var res = proto as ProtoUserIdStringValue;

                Console.WriteLine($"UserId{res.UserId} : {res.Value}");
            }
        }
    }
}
