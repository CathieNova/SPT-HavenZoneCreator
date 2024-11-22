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

    private static string assemblyLocation = Assembly.GetExecutingAssembly().Location;
    private static string basePath = Path.GetFullPath(Path.Combine(assemblyLocation, @"..\..\..\"));

    public static void GenrateJson(JsonType type)
    {
        if (Settings.CurrentLookPosition.Value == Vector3.zero) return;
        
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

        // Serialize the object to JSON and write to the file
        using (StreamWriter streamWriter = File.CreateText(filePath))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.Serialize(streamWriter, zoneData);
        }
    }


    private static string GetFlareType()
    {
        if (Settings.FlareType.Value == EFlareTypes.none)
            return "";
        
        return Settings.FlareType.Value.ToString();
    }

    private static void GenerateLooseLootJson()
    {
        
    }
}