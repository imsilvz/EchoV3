using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Text.Json;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using Microsoft.Web.WebView2.Core;

using EchoV3.Services;
using EchoV3.Models.FFXIV.Events.Client;
using EchoV3.Models.FFXIV.Events.Server;
using EchoV3.Models.Echo.Ipc;
using EchoV3.Models.FFXIV.GameData;
using System.Text.Json.Serialization;
using EchoV3.Utility;

namespace EchoV3.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(FFXIVEventService eventService)
        {
            InitializeComponent();
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            // webview init
            webView.DefaultBackgroundColor = System.Drawing.Color.Transparent;
            string app = LoadResource("EchoV3.Resources.Ui.index.html");
            var env = await CoreWebView2Environment.CreateAsync(
                userDataFolder: Path.Combine(
                    Path.GetTempPath(),
                    "EchoV3Browser"
                )
            );
            await webView.EnsureCoreWebView2Async(env);
            webView.NavigateToString(app);
        }

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            webView.Visibility = Visibility.Visible;
            ChatEvent.OnEventFired += OnChatEvent;
            ChatHandlerEvent.OnEventFired += OnChatHandlerEvent;
            ClientTriggerEvent.OnEventFired += OnClientTriggerEvent;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            
            var wp = this.GetPlacement();
            Settings.SaveWindowPosition(wp);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            try
            {
                var wp = Settings.GetSavedWindowPosition();
                if (wp is not null)
                {
                    this.SetPlacement((WINDOWPLACEMENT)wp);
                }
            }
            catch
            {
                // ignored
            }
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Keep On top!
        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void OnChatEvent(object? sender, ChatEvent chatEvent)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Debug.WriteLine(chatEvent);
                EchoChatIpc chatMsg = new EchoChatIpc
                {
                    Timestamp = chatEvent.Timestamp,
                    SourceActorId = chatEvent.SourceActorId,
                    DestinationActorId = chatEvent.DestinationActorId,
                    MessageType = chatEvent.MessageType.ToString(),
                    SenderId = chatEvent.SenderId,
                    SenderName = chatEvent.SenderName,
                    SenderActor = App.FFXIVState?.GetPlayer(chatEvent.SenderId),
                    Message = chatEvent.Message,
                };

                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                webView.CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(chatMsg, serializeOptions));
            });
        }

        public void OnChatHandlerEvent(object? sender, ChatHandlerEvent chatHandlerEvent)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Debug.WriteLine(chatHandlerEvent);
                string characterName = "You";
                PlayerActor? character = App.FFXIVState?.GetPlayer(chatHandlerEvent.SourceActorId);
                if (character is not null)
                    characterName = character.PlayerName;
                EchoChatIpc chatMsg = new EchoChatIpc
                {
                    Timestamp = chatHandlerEvent.Timestamp,
                    SourceActorId = chatHandlerEvent.SourceActorId,
                    DestinationActorId = chatHandlerEvent.DestinationActorId,
                    MessageType = chatHandlerEvent.MessageType.ToString(),
                    SenderId = chatHandlerEvent.SourceActorId,
                    SenderName = characterName,
                    SenderActor = App.FFXIVState?.GetPlayer(chatHandlerEvent.SourceActorId),
                    Message = chatHandlerEvent.Message,
                };

                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                webView.CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(chatMsg, serializeOptions));
            });
        }

        public void OnClientTriggerEvent(object? sender, ClientTriggerEvent clientTriggerEvent) 
        { 
            if (clientTriggerEvent.CommandId == ClientTriggerEvent.ClientTriggerType.ChangeTarget)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    // this seems to be the id passed whenever clearing your target
                    // uint worldId = 3758096384;
                    var targetIpc = new EchoLocalTargetIpc
                    {
                        Timestamp = clientTriggerEvent.Timestamp,
                        SourceActorId = clientTriggerEvent.SourceActorId,
                        DestinationActorId = clientTriggerEvent.DestinationActorId,
                        TargetId = clientTriggerEvent.Param1,
                    };

                    var serializeOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    webView.CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(targetIpc, serializeOptions));
                });
            }
        }

        private string LoadResource(string name)
        {
            string data;
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream s = assembly.GetManifestResourceStream(name))
            {
                using (StreamReader r = new StreamReader(s))
                {
                    data = r.ReadToEnd();
                }
            }
            return data;
        }
    }
}
