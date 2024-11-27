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
        if (Settings.CurrentZoneCubePosition.Value == Vector3.zero)
        {
            NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] You must generate a FetchLookPosition first!");
            return;
        }
        if (Settings.ZoneId.Value == "")
        {
            NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] You must set a Zone ID first!");
            return;
        }
        if (Settings.ZoneName.Value == "")
        {
            NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] You must set a Zone Name first!");
            return;
        }
        
        string filePath = Path.Combine(basePath, "user", "mods", "Virtual's Custom Quest Loader", "database", "zones", $"{Settings.ZoneId.Value.ToLower()}.json");
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
                ZoneId = Settings.ZoneId.Value.ToLower().Replace(" ", "_"),
                ZoneName = Settings.ZoneName.Value.ToLower().Replace(" ", "_"),
                ZoneLocation = Singleton<GameWorld>.Instance.MainPlayer.Location,
                ZoneType = Settings.ZoneType.Value.ToString(),
                FlareType = GetFlareType(),
                Position = new
                {
                    X = $"{Settings.CurrentZoneCubePosition.Value.x}",
                    Y = $"{Settings.CurrentZoneCubePosition.Value.y}",
                    Z = $"{Settings.CurrentZoneCubePosition.Value.z}",
                    W = "0"
                },
                Rotation = new
                {
                    X = $"{Settings.CurrentZoneCubeRotation.Value.x}",
                    Y = $"{Settings.CurrentZoneCubeRotation.Value.y}",
                    Z = $"{Settings.CurrentZoneCubeRotation.Value.z}",
                    W = $"{Settings.CurrentZoneCubeRotation.Value.w}"
                },
                Scale = new
                {
                    X = $"{Settings.CurrentZoneCubeScale.Value.x}",
                    Y = $"{Settings.CurrentZoneCubeScale.Value.y}",
                    Z = $"{Settings.CurrentZoneCubeScale.Value.z}",
                    W = "0"
                }
            }
        };

        WriteJsonFile(filePath, zoneData);
        NotificationManagerClass.DisplayMessageNotification($"[HavenZoneCreator] VCQL JSON file generated at {filePath} for zone {Settings.ZoneId.Value}");
    }

    private static string GetFlareType()
    {
        if (Settings.FlareType.Value == EFlareTypes.none)
            return "";
        
        return Settings.FlareType.Value.ToString();
    }

    private static void GenerateLooseLootJson()
    {
        if (Settings.CurrentZoneCubePosition.Value == Vector3.zero)
        {
            NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] You must generate a FetchLookPosition first!");
            return;
        }
        
        if (Settings.LooseLootItemId.Value == "")
        {
            NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] You must set a Loose Loot Item ID first!");
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
            locationId = $"({Settings.CurrentZoneCubePosition.Value.x}, {Settings.CurrentZoneCubePosition.Value.y}, {Settings.CurrentZoneCubePosition.Value.z})",
            probability = Settings.LooseLootProbability.Value,
            template = new
            {
                Id = $"{MongoID.Generate()}",
                IsContainer = false,
                useGravity = Settings.LooseLootUseGravity.Value,
                randomRotation = Settings.LooseLootRandomRotation.Value,
                Position = new
                {
                    x = $"{Settings.CurrentZoneCubePosition.Value.x}",
                    y = $"{Settings.CurrentZoneCubePosition.Value.y}",
                    z = $"{Settings.CurrentZoneCubePosition.Value.z}"
                },
                Rotation = new
                {
                    x = $"{Settings.CurrentZoneCubeRotation.Value.x}",
                    y = $"{Settings.CurrentZoneCubeRotation.Value.y}",   
                    z = $"{Settings.CurrentZoneCubeRotation.Value.z}"
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
        NotificationManagerClass.DisplayMessageNotification($"[HavenZoneCreator] Loose Loot JSON file generated at {filePath}");
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