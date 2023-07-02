using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using EchoV3.Services;
using System.Diagnostics;
using EchoV3.Models.FFXIV.Events.Server;
using EchoV3.Models.FFXIV.Events.Client;
using EchoV3.Models.FFXIV.GameData;

namespace EchoV3.Utility
{
    public class FFXIVStateManager
    {
        private FFXIVEventService _eventService;

        private uint? _localPlayerId { get; set; }
        private Dictionary<uint, BaseActor> _actorMap { get; set; }
        public FFXIVStateManager(ServiceProvider serviceProvider)
        {
            _actorMap = new Dictionary<uint, BaseActor>();
            _eventService = serviceProvider.GetRequiredService<FFXIVEventService>();
            ClientTriggerEvent.OnEventFired += OnClientTrigger;
            PlayerSetupEvent.OnEventFired += OnPlayerSetup;
            PlayerSpawnEvent.OnEventFired += OnPlayerSpawn;
        }

        public PlayerActor? GetPlayer(uint playerId)
        {
            BaseActor? actor = null;
            if (_actorMap.TryGetValue(playerId, out actor))
            {
                return (PlayerActor)actor;
            }
            return null;
        }

        public PlayerActor? GetLocalPlayer()
        {
            if (_localPlayerId == null)
                return null;
            return GetPlayer((uint)_localPlayerId);
        }

        public void OnClientTrigger(object? sender, ClientTriggerEvent e)
        {
            // Debug.WriteLine(e);
        }

        public void OnPlayerSetup(object? sender, PlayerSetupEvent e) 
        {
            _localPlayerId = e.PlayerId;
            _actorMap[(uint)_localPlayerId] = new PlayerActor
            {
                ActorId = e.PlayerId,
                PlayerName = e.PlayerName,
            };
        }

        public void OnPlayerSpawn(object? sender, PlayerSpawnEvent e) 
        {
            Debug.WriteLine(e);
            _actorMap[e.SourceActorId] = new PlayerActor
            {
                ActorId = e.SourceActorId,
                PlayerName = e.Name,
                Job = e.Job,
                JobLevel = e.JobLevel,
            };
        }
    }
}
