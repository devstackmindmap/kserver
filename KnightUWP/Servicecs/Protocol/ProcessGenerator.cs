using AkaDB.MySql;
using CommonProtocol;
using KnightUWP.Dao;
using Microsoft.Toolkit.Uwp.Helpers;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KnightUWP.Servicecs.Protocol
{
    public static class ProtocolProcessGenerator
    {
        static Dictionary<MessageType, Type> _processMap;

        static ProtocolProcessGenerator(){

            var myAssem = Assembly.GetExecutingAssembly();
            var messageProcess = myAssem.GetTypes().Where(type => type.GetCustomAttributes(typeof(MessageAttribute), false).Any());

            _processMap = messageProcess.ToDictionary(process =>
            {
                return (process.GetCustomAttribute(typeof(MessageAttribute)) as MessageAttribute).MessageType;
            },
            process => process);

        }

        public static void DoProcess<TContext>(TContext context, DataEventArgs e)
        {
            int offset = 0;
            int intSize = sizeof(int);
            var sizeHeader = sizeof(MessageType);

            while (offset < e.Length)
            {
                var header = e.Data.CloneRange(offset, sizeHeader);
                var length = e.Data.CloneRange(offset + sizeHeader, intSize);
                var realLength = BitConverter.ToInt32(length, 0);
                var body = e.Data.CloneRange(offset + sizeHeader + intSize, realLength);

                offset += sizeHeader + intSize + realLength;

                var msgType = (MessageType)BitConverter.ToInt32(header, 0);

                var processor = GetProcess(msgType);
                if (processor != null)
                {
                    var protocol = processor.OnResponseDataToProtocol(context, body);
                    if (protocol is UnknownProtocol)
                    {
                        (protocol as UnknownProtocol).MessageType = msgType;
                    }

                    processor.OnPreResponse(context, protocol)
                                .OnResponse(context, protocol);
                }
            }

        }

        public static BaseProcess GetProcess(MessageType message)
        {
            if (_processMap.TryGetValue(message, out var type))
                return Activator.CreateInstance(type) as BaseProcess;
            return new UnknownProcess();

            /*
        switch (type)
        {
                //TODO cancel
        case MessageType.MatchingCancelSuccess:
            return new MatchingCancelSuccess();
        case MessageType.MatchingCancelFail:
            return new MatchingCancelFail();
        case MessageType.ReEnterRoomFail:
            return new ReEnterRoomFail();
        case MessageType.ReEnterRoomSuccess:
            return new ReEnterRoomSuccess();

        case MessageType.CardUseResult:
            return new CardUseResult();
        case MessageType.StartExtensionTime:
            return new BattleExtension();
        case MessageType.StartBoosterTime:
            return new BattleStartBoosterTime();
        case MessageType.UpdateUnitAttackTime:
            return new UpdateUnitAttackTime();
        case MessageType.InvalidateCardUse:
            return new InvalidateCardUse();
        case MessageType.UnitDeath:
            return new UnitDeath();
        case MessageType.Poison:
            return new Poison();
        case MessageType.AddElixir:
            return new AddElixir();
        case MessageType.ElectricShock:
            return new ElectricShock();
        case MessageType.AttackUnit:
            return new NormalAttack();
        case MessageType.Skill:
            return new Skill();
        case MessageType.ValidateCard:
            return new ValidateCard();
        case MessageType.GetBattleResult:
            return new BattleResult();
        case MessageType.BattleRoundResult:
            return new BattleRoundResult();
        case MessageType.EnqueuedAction:
            return new EnqueuedAction();
        case MessageType.OnEmoticonUse:
            return new EmoticonUse();
        case MessageType.PubSubOnLogin:
            return new PubSubOnLogin();
        case MessageType.PubSubLogout:
            return new PubSubOnLogout();
        case MessageType.PubSubOnFriendAsked:
            return new PubSubOnAsked();
        case MessageType.PubSubOnFriendSigned:
            return new PubSubOnSigned();
        case MessageType.PubSubBattle:
            return new PubSubBattle();
        case MessageType.PubSubMatchmaking:
            return new PubSubMatchmaking();
        case MessageType.PubSubOnline:
            return new PubSubOnline();
        case MessageType.PubSubOnFriendRemoved:
            return new PubSubOnFriendRemoved();
        case MessageType.PubSubOnFriendlyBattleInvite:
            return new PubSubOnFriendlyBattleInvite();
        case MessageType.PubSubOnFriendlyBattleAccept:
            return new PubSubOnFriendlyBattleAccept();
        case MessageType.PubSubOnFriendlyBattleDecline:
            return new PubSubOnFriendlyBattleDecline();
        case MessageType.PubSubOnFriendlyBattleCancel:
            return new PubSubOnFriendlyBattleCancel();
        case MessageType.PubSubOnFriendlyBattleReady:
            return new PubSubOnFriendlyBattleReady();
        case MessageType.PubSubOnFriendlyBattleReadyCancel:
            return new PubSubOnFriendlyBattleReadyCancel();
        case MessageType.PubSubUpdateQuest:
            return new PubSubOnUpdateQuest();
        case MessageType.IgnitionBomb:
            return new IgnitionBomb();
            default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            */
        }
    }
}
