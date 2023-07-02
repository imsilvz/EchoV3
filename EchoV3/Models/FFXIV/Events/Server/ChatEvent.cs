﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EchoV3.Models.FFXIV.Events.Server
{
    public class ChatEvent : BaseFFXIVEvent
    {
        public enum ChatMessageType
        {
            LogKindError,
            ServerDebug,
            ServerUrgent,
            ServerNotice,
            Unused4,
            Unused5,
            Unused6,
            Unused7,
            Unused8,
            Unused9,
            Say,
            Shout,
            Tell,
            TellReceive,
            Party,
            Alliance,
            LS1,
            LS2,
            LS3,
            LS4,
            LS5,
            LS6,
            LS7,
            LS8,
            FreeCompany,
            Unused25,
            Unused26,
            NoviceNetwork,
            CustomEmote,
            StandardEmote,
            Yell,
            Unknown31,
            PartyUnk2,
            Unused33,
            Unused34,
            Unused35,
            Unused36,
            Unused37,
            Unused38,
            Unused39,
            Unused40,
            BattleDamage,
            BattleFailed,
            BattleActions,
            BattleItems,
            BattleHealing,
            BattleBeneficial,
            BattleDetrimental,
            BattleUnk48,
            BattleUnk49,
            Unused50,
            Unused51,
            Unused52,
            Unused53,
            Unused54,
            Unused55,
            Echo,
            SystemMessage,
            SystemErrorMessage,
            BattleSystem,
            GatheringSystem,
            NPCMessage,
            LootMessage,
            Unused63,
            CharProgress,
            Loot,
            Crafting,
            Gathering,
            NPCAnnouncement,
            FCAnnouncement,
            FCLogin,
            RetainerSale,
            PartySearch,
            PCSign,
            DiceRoll,
            NoviceNetworkNotice,
            Unknown76,
            Unused77,
            Unused78,
            Unused79,
            GMTell,
            GMSay,
            GMShout,
            GMYell,
            GMParty,
            GMFreeCompany,
            GMLS1,
            GMLS2,
            GMLS3,
            GMLS4,
            GMLS5,
            GMLS6,
            GMLS7,
            GMLS8,
            GMNoviceNetwork,
            Unused95,
            Unused96,
            Unused97,
            Unused98,
            Unused99,
            Unused100
        }

        public static EventHandler<ChatEvent>? OnEventFired;
        public static void RaiseEvent(object? sender, BaseFFXIVEvent xivEvent)
        {
            OnEventFired?.Invoke(sender, (ChatEvent)xivEvent);
        }

        public required ChatMessageType MessageType { get; set; }
        public required uint SenderId { get; set; }
        public required string SenderName { get; set; }
        public required string Message { get; set; }

        public override string ToString()
        {
            return $"Chat({SenderId}) {MessageType} - {SenderName}: {Message}";
        }
    }
}
