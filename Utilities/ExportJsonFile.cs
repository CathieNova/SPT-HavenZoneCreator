using System;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using Comfort.Common;
using EFT;
using UnityEngine;

namespace HavenZoneCreator.Utilities;

public static class ExportJsonFile
{
    public static JsonType JsonExportType;

    public enum JsonType
    {
        VCQL,
        LooseLoot
    }

    private static readonly string assemblyLocation = Assembly.GetExecutingAssembly().Location;
    private static readonly string basePath = Path.GetFullPath(Path.Combine(assemblyLocation, @"..\..\..\"));

    public static void GenerateJson(JsonType type)
    {
        switch (type)
        {
            case JsonType.VCQL:
                GenerateVCQLJson();
                break;
            case JsonType.LooseLoot:
                GenerateLooseLootJson();
                break;
            default:
                break;
        }
    }

    private static void GenerateVCQLJson()
    {

        if (Settings.CurrentLookPosition.Value == Vector3.zero)
        {
            NotificationManagerClass.DisplayMessageNotification("You must generate a FetchLookPosition first!");
            return;
        }
        if (Settings.ZoneId.Value == "")
        {
            NotificationManagerClass.DisplayMessageNotification("You must set a Zone ID first!");
            return;
        }
        if (Settings.ZoneName.Value == "")
        {
            NotificationManagerClass.DisplayMessageNotification("You must set a Zone Name first!");
            return;
        }
        
        string filePath = Path.Combine(basePath, "user", "mods", "Virtual's Custom Quest Loader", "database", "zones", $"{Settings.ZoneId.Value}.json");
        string directoryPath = Path.GetDirectoryName(filePath);

        // Ensure the directory exists
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var zoneData = new[]
        {
            new
            {
                ZoneId = Settings.ZoneId.Value,
                ZoneName = Settings.ZoneName.Value,
                ZoneLocation = Singleton<GameWorld>.Instance.MainPlayer.Location,
                ZoneType = Settings.ZoneType.Value.ToString(),
                FlareType = GetFlareType(),
                Position = new
                {
                    X = Settings.CurrentLookPosition.Value.x,
                    Y = Settings.CurrentLookPosition.Value.y,
                    Z = Settings.CurrentLookPosition.Value.z,
                    W = "0"
                },
                Rotation = new
                {
                    X = Settings.CurrentLookRotation.Value.x,
                    Y = Settings.CurrentLookRotation.Value.y,
                    Z = Settings.CurrentLookRotation.Value.z,
                    W = Settings.CurrentLookRotation.Value.w
                },
                Scale = new
                {
                    X = Settings.CurrentLookScale.Value.x,
                    Y = Settings.CurrentLookScale.Value.y,
                    Z = Settings.CurrentLookScale.Value.z,
                    W = "0"
                }
            }
        };

        WriteJsonFile(filePath, zoneData);
    }

    private static string GetFlareType()
    {
        if (Settings.FlareType.Value == EFlareTypes.none)
            return "";
        
        return Settings.FlareType.Value.ToString();
    }

    private static void GenerateLooseLootJson()
    {
        if (Settings.CurrentLookPosition.Value == Vector3.zero)
        {
            NotificationManagerClass.DisplayMessageNotification("You must generate a FetchLookPosition first!");
            return;
        }
        
        if (Settings.LooseLootItemId.Value == "")
        {
            NotificationManagerClass.DisplayMessageNotification("You must set a Loose Loot Item ID first!");
            return;
        }
        
        string filePath = Path.Combine(basePath, "_EXPORTED_LOOT_", $"{Settings.LooseLootItemId.Value}_looseLoot.json");
        string directoryPath = Path.GetDirectoryName(filePath);

        // Ensure the directory exists
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var rootID = MongoID.Generate();
        // Gather data for the JSON
        var looseLootData = new
        {
            locationId = $"({Settings.CurrentLookPosition.Value.x}, {Settings.CurrentLookPosition.Value.y}, {Settings.CurrentLookPosition.Value.z})",
            probability = Settings.LooseLootProbability.Value,
            template = new
            {
                Id = MongoID.Generate(),
                IsContainer = false,
                useGravity = Settings.LooseLootUseGravity.Value,
                randomRotation = Settings.LooseLootRandomRotation.Value,
                Position = new
                {
                    x = Settings.CurrentLookPosition.Value.x,
                    y = Settings.CurrentLookPosition.Value.y,
                    z = Settings.CurrentLookPosition.Value.z
                },
                Rotation = new
                {
                    x = Settings.CurrentLookRotation.Value.x,
                    y = Settings.CurrentLookRotation.Value.y,   
                    z = Settings.CurrentLookRotation.Value.z
                },
                IsGroupPosition = false,
                GroupPositions = Array.Empty<object>(),
                IsAlwaysSpawn = false,
                Root = rootID,
                Items = new[]
                {
                    new
                    {
                        _id = rootID,
                        _tpl = Settings.LooseLootItemId.Value,
                        upd = new
                        {
                            StackObjectsCount = 1
                        }
                    }
                }
            }
        };

        WriteJsonFile(filePath, looseLootData);
    }

    private static void WriteJsonFile(string filePath, object looseLootData)
    {
        using (StreamWriter streamWriter = File.CreateText(filePath))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.Serialize(streamWriter, looseLootData);
        }
    }
}