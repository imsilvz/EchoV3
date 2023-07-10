using EchoV3.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace EchoV3.Utility
{
    public static class Settings
    {
        public const string WINDOWPLACEMENT_FILENAME = "window.json";
        private static string _settingsPath { get; set; }

        static Settings()
        {
            string settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _settingsPath = $"{settingsPath}\\EchoV3";
            Directory.CreateDirectory(_settingsPath);
        }

        public static WINDOWPLACEMENT? GetSavedWindowPosition()
        {
            if (File.Exists($"{_settingsPath}\\${WINDOWPLACEMENT_FILENAME}"))
            {
                var wpJson = File.ReadAllText($"{_settingsPath}\\${WINDOWPLACEMENT_FILENAME}");

                WINDOWPLACEMENT? wp;
                var serializeOptions = new JsonSerializerOptions
                {
                    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    IncludeFields = true,
                    WriteIndented = true,
                };
                wp = JsonSerializer.Deserialize<WINDOWPLACEMENT>(wpJson, serializeOptions);
                return wp;
            }
            return null;
        }

        public static void SaveWindowPosition(WINDOWPLACEMENT placement)
        {
            string settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Debug.WriteLine(_settingsPath);

            var serializeOptions = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                ReadCommentHandling = JsonCommentHandling.Skip,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IncludeFields = true,
                WriteIndented = true,
            };
            var encoded = JsonSerializer.Serialize(placement, serializeOptions);
            File.WriteAllText($"{_settingsPath}\\${WINDOWPLACEMENT_FILENAME}", encoded);
        }
    }
}
