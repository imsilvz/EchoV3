using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

using EchoV3.Interfaces.Deucalion;
using EchoV3.Interfaces.FFXIV;
using EchoV3.Models.FFXIV;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Reflection;
using EchoV3.Models.FFXIV.Events;

namespace EchoV3.Services
{
    public delegate IFFXIVEventFactory? EventFactoryFunc();

    public class FFXIVEventService
    {
        // private readonly HttpClient _httpClient = new HttpClient();
        private Dictionary<uint, EventFactoryFunc> _clientEventFactoryMap;
        private Dictionary<uint, EventFactoryFunc> _serverEventFactoryMap;
        private RegionalOpcodeData _regionalOpcodeData;

        // DI
        private DeucalionService _deucalionService;
        public FFXIVEventService(DeucalionService deucalionService)
        {
            // fetch opcode dictionary
            // var json = _httpClient.GetStringAsync("https://raw.githubusercontent.com/karashiiro/FFXIVOpcodes/master/opcodes.min.json").Result;
            var json = File.ReadAllText("Resources/opcodes.json");
            var opcodeData = JsonSerializer.Deserialize<List<RegionalOpcodeData>>(json)?.Where((data) => data.Region == "Global").First();
            if (opcodeData is not null)
            {
                _regionalOpcodeData = opcodeData;
            }
            else
            {
                throw new HttpRequestException("Unexpected Opcode Payload Recieved!");
            }

            // register segment factories
            var factoryInterfaceType = typeof(IFFXIVEventFactory);
            var factoryTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => factoryInterfaceType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);
            _clientEventFactoryMap = new Dictionary<uint, EventFactoryFunc>();
            foreach (var ipcData in opcodeData.Lists.ClientZoneIpcType)
            {
                Type? factoryType = factoryTypes.Where((type) => type.Name == $"{ipcData.Name}EventFactory").FirstOrDefault();
                if (factoryType is not null)
                {
                    _clientEventFactoryMap.Add(ipcData.Code, () =>
                    {
                        IFFXIVEventFactory? factory = (IFFXIVEventFactory?)Activator.CreateInstance(factoryType); ;
                        return factory;
                    });
                }
            }
            _serverEventFactoryMap = new Dictionary<uint, EventFactoryFunc>();
            foreach (var ipcData in opcodeData.Lists.ServerZoneIpcType)
            {
                Type? factoryType = factoryTypes.Where((type) => type.Name == $"{ipcData.Name}EventFactory").FirstOrDefault();
                if (factoryType is not null)
                {
                    _serverEventFactoryMap.Add(ipcData.Code, () =>
                    {
                        IFFXIVEventFactory? factory = (IFFXIVEventFactory?)Activator.CreateInstance(factoryType); ;
                        return factory;
                    });
                }
            }
            _deucalionService = deucalionService;
            _deucalionService.IpcFrameProcessed += ProcessEvent;
        }

        public void ProcessEvent(object? sender, IFFXIVIpcFrame frame)
        {
            //Debug.WriteLine(frame);
            IFFXIVEventFactory? factory = null;
            EventFactoryFunc? factoryFunc = null;
            switch(frame.Operation)
            {
                case IDeucalionFrame.DeucalionOperation.Recv:
                    if (_serverEventFactoryMap.TryGetValue(frame.FFXIVIpcHeader.OpCode, out factoryFunc))
                    {
                        factory = factoryFunc();
                    }
                    break;
                case IDeucalionFrame.DeucalionOperation.Send:
                    if (_clientEventFactoryMap.TryGetValue(frame.FFXIVIpcHeader.OpCode, out factoryFunc))
                    {
                        factory = factoryFunc();
                    }
                    break;
                default:
                    break;
            }

            // no processor available for this event
            if (factory is null) 
            {
                //Debug.WriteLine(frame);
                return; 
            }
            BaseFFXIVEvent xivEvent = factory.GetEvent(frame);
            //Debug.WriteLine(xivEvent);

            // trigger static event if exists
            var eventType = xivEvent.GetType();
            MethodInfo? eventHandler = eventType.GetMethod("RaiseEvent", BindingFlags.Public | BindingFlags.Static);
            eventHandler?.Invoke(null, new[] { this, (object)xivEvent });
        }
    }
}
