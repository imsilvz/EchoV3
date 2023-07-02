using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Models.FFXIV.Events.Client
{
    public class ClientTriggerEvent : BaseFFXIVEvent
    {
        public enum ClientTriggerType : ushort
        {
            ToggleSheathe = 0x01,
            ToggleAutoAttack = 0x02,
            ChangeTarget = 0x03,

            DismountReq = 0x65,
            SpawnCompanionReq = 0x66,
            DespawnCompanionReq = 0x67,
            RemoveStatusEffect = 0x68,
            CastCancel = 0x69,

            Return = 0xC8, // return dead / accept raise
            FinishZoning = 0xC9,
            Teleport = 0xCA,

            Examine = 0x12C,
            MarkPlayer = 0x12D, // Mark player, visible to party only
            SetTitleReq = 0x12E,
            TitleList = 0x12F,

            UpdatedSeenHowTos = 0x133,
            CutscenePlayed = 0x134, // param1 = cutscene id
            AllotAttribute = 0x135,

            ClearFieldMarkers = 0x13A,
            CameraMode = 0x13B, // param11, 1 = enable, 0 = disable
            CharaNameReq = 0x13D, // requests character name by content id
            HuntingLogDetails = 0x194,

            Timers = 0x1AB,

            DyeItem = 0x1B0, // updated 5.21

            RequestChocoboInventory = 0x1C4,

            EmoteReq = 0x1F4,
            EmoteCancel = 0x1F6,
            PersistentEmoteCancel = 0x1F7,
            /*!
             * param2 = pose ID
             *      0 = idle pose 0 (just standing)
             *      1 = idle pose 1
             *    2-4 = idle poses 2-4
             */
            PoseChange = 0x1F9,
            PoseReapply = 0x1FA,
            PoseCancel = 0x1FB,

            AchievementCrit = 0x202,
            AchievementComp = 0x203,
            AchievementCatChat = 0x206,

            RequestEventBattle = 0x232C,

            QuestJournalUpdateQuestVisibility = 0x2BE,
            QuestJournalClosed = 0x2BF,

            AbandonQuest = 0x320,

            DirectorInitFinish = 0x321,

            DirectorSync = 0x328, // unsure what exactly triggers it, starts director when returning to instance though

            EnterTerritoryEventFinished = 0x330,
            RequestInstanceLeave = 0x333, // df menu button

            AchievementCritReq = 0x3E8,
            AchievementList = 0x3E9,

            SetEstateLightingLevel = 0x40B, // param1 = light level 0 - 5 maps to UI val 5-0
            RequestHousingBuildPreset = 0x44C,
            RequestEstateExteriorRemodel = 0x044D, // param11 = land id
            RequestEstateInteriorRemodel = 0x44E,
            RequestEstateHallRemoval = 0x44F,
            RequestBuildPreset = 0x450, // no idea what this is, it gets sent with BuildPresetHandler and has the plot id in param1
            RequestLandSignFree = 0x451,
            RequestLandSignOwned = 0x452,
            RequestWardLandInfo = 0x453,
            RequestLandRelinquish = 0x454,
            RequestLandInventory = 0x0458,
            RequestHousingItemRemove = 0x0459,
            RequestEstateRename = 0x45A,
            RequestEstateEditGreeting = 0x45B,
            RequestEstateGreeting = 0x45C, // sends FFXIVIpcHousingEstateGreeting in return
            RequestEstateEditGuestAccessSettings = 0x45D,
            UpdateEstateGuestAccess = 0x45E,
            RequestEstateTagSettings = 0x45F,
            RequestEstateInventory = 0x0461,
            RequestHousingItemUI = 0x463,
            RequestSharedEstateSettings = 0x46F,
            UpdateEstateLightingLevel = 0x471,
            HousingItemSelectedInUI = 0x47E,

            CompanionAction = 0x6A4,
            CompanionSetBarding = 0x6A5,
            CompanionActionUnlock = 0x6A6,

            OpenPerformInstrumentUI = 0x71C,

            StartReplay = 0x7BC,
            EndReplay = 0x7BD, // request for restoring the original player state (actor, buff, gauge, etc..)

            OpenDuelUI = 0x898, // Open a duel ui
            DuelRequestResult = 0x899, // either accept/reject
        }

        public static EventHandler<ClientTriggerEvent>? OnEventFired;
        public static void RaiseEvent(object? sender, BaseFFXIVEvent xivEvent)
        {
            OnEventFired?.Invoke(sender, (ClientTriggerEvent)xivEvent);
        }

        public required ClientTriggerType CommandId { get; set; }
        public required uint Param1 { get; set; }
        public required uint Param2 { get; set; }
        public required uint Param3 { get; set; }
        public required uint Param4 { get; set;}
        public required uint Param5 { get; set; }
        public required ulong Param6 { get; set; }

        public override string ToString()
        {
            return $"ClientTrigger({SourceActorId}) {(short)CommandId}: {Param1}, {Param2}, {Param3}, {Param4}, {Param5}, {Param6}";
        }
    }
}
