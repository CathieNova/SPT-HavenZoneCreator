using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using EFT.Communications;
using HavenZoneCreator.Features;
using Newtonsoft.Json;
using UnityEngine;

namespace HavenZoneCreator.Utilities;
internal class Settings
{
    private static readonly List<ConfigEntryBase> ConfigEntries = [];
    public static List<Location> CubeDataList = new List<Location>();
    internal static string Directory;

    #region Categories

    private const string ZoneInformation = "1. Zone Information";
    private const string ZoneBoxSettings = "2. Zone Box Settings";
    private const string VCQLZoneSettings = "3. VCQL Zone Settings";
    private const string LooseLootSettings = "4. LooseLoot Settings";
    private const string MapLocations = "5. Map Locations";

    #endregion

    #region 1. Zone Information

    public static ConfigEntry<Vector3> CurrentZoneCubePosition { get; set; }
    public static ConfigEntry<Quaternion> CurrentZoneCubeRotation { get; set; }
    public static ConfigEntry<Vector3> CurrentZoneCubeScale { get; set; }
    public static ConfigEntry<string> CurrentMapName { get; set; }

    public static ConfigEntry<float> ZoneCubeTransparency { get; set; }

    #endregion

    #region 2. Zone Box Settings

    public static ConfigEntry<float> TransformSpeed { get; set; }
    public static ConfigEntry<KeyboardShortcut> HavenZoneCube { get; set; }
    public static ConfigEntry<KeyboardShortcut> RemoveHavenZoneCube { get; set; }
    public static ConfigEntry<bool> SpawnHavenZoneCubeAtLookingPosition { get; set; }
    public static ConfigEntry<KeyboardShortcut> PositiveXKey { get; set; }
    public static ConfigEntry<KeyboardShortcut> NegativeXKey { get; set; }
    public static ConfigEntry<KeyboardShortcut> PositiveYKey { get; set; }
    public static ConfigEntry<KeyboardShortcut> NegativeYKey { get; set; }
    public static ConfigEntry<KeyboardShortcut> PositiveZKey { get; set; }
    public static ConfigEntry<KeyboardShortcut> NegativeZKey { get; set; }
    public static ConfigEntry<KeyboardShortcut> PositionModeKey { get; set; }
    public static ConfigEntry<KeyboardShortcut> ScaleModeKey { get; set; }
    public static ConfigEntry<KeyboardShortcut> RotateModeKey { get; set; }
    public static ConfigEntry<KeyboardShortcut> IncreaseTransformSpeed { get; set; }
    public static ConfigEntry<KeyboardShortcut> DecreaseTransformSpeed { get; set; }
    public static ConfigEntry<bool> LockXAndZRotation { get; set; }
    public static ConfigEntry<Vector3> DefaultScale { get; set; }

    #endregion

    #region 3. VCQL Zone Settings

    public static ConfigEntry<string> ZoneId { get; set; }
    public static ConfigEntry<string> ZoneName { get; set; }
    public static ConfigEntry<EZoneTypes> ZoneType { get; set; }
    public static ConfigEntry<EFlareTypes> FlareType { get; set; }

    #endregion

    #region 4. Loose Loot Settings

    public static ConfigEntry<float> LooseLootProbability { get; set; }
    public static ConfigEntry<bool> LooseLootUseGravity { get; set; }
    public static ConfigEntry<bool> LooseLootRandomRotation { get; set; }
    public static ConfigEntry<string> LooseLootItemId { get; set; }

    #endregion

    #region 5. Map Locations

    public static ConfigEntry<KeyboardShortcut> AddMapLocationToListKey { get; set; }
    public static ConfigEntry<KeyboardShortcut> RemoveMapLocationFromListKey { get; set; }

    #endregion

    public static void Init(ConfigFile config)
    {
        #region Config 1. Zone Information

        ConfigEntries.Add(CurrentZoneCubePosition = config.Bind(ZoneInformation, "Current Zone Cube Position", Vector3.zero, new ConfigDescription(
            "The current position of the Zone Cube.", null, new ConfigurationManagerAttributes { ReadOnly = true })));

        ConfigEntries.Add(CurrentZoneCubeRotation = config.Bind(ZoneInformation, "Current Zone Cube Rotation", Quaternion.identity, new ConfigDescription(
            "The current rotation of the Zone Cube.", null, new ConfigurationManagerAttributes { ReadOnly = true })));

        ConfigEntries.Add(CurrentZoneCubeScale = config.Bind(ZoneInformation, "Current Zone Cube Scale", Vector3.zero, new ConfigDescription(
            "The current scale of the Zone Cube.", null, new ConfigurationManagerAttributes { ReadOnly = true })));

        ConfigEntries.Add(CurrentMapName = config.Bind(ZoneInformation, "Current Map Name", "", new ConfigDescription(
            "The ID of the current map.", null, new ConfigurationManagerAttributes { ReadOnly = true })));

        ConfigEntries.Add(ZoneCubeTransparency = config.Bind(ZoneInformation, "Zone Cube Transparency", 0.5f, new ConfigDescription(
            "Transparency of the look position cube.", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { })));

        #endregion

        #region Config 2. Zone Box Settings

        ConfigEntries.Add(TransformSpeed = config.Bind(ZoneBoxSettings, "Transform Speed", 1f, new ConfigDescription(
            "The speed Zone Cube is transformed.", new AcceptableValueRange<float>(0.01f, 10f), new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(HavenZoneCube = config.Bind(ZoneBoxSettings, "Generate Haven Zone Cube", new KeyboardShortcut(KeyCode.Keypad0),
            new ConfigDescription(
                "Fetches the position you are looking at and generates a Zone Cube.", null, new ConfigurationManagerAttributes { }, true)));

        ConfigEntries.Add(RemoveHavenZoneCube = config.Bind(ZoneBoxSettings, "Remove Haven Zone Cube", new KeyboardShortcut(KeyCode.KeypadEnter),
            new ConfigDescription(
                "Removes the look position Zone Cube.", null, new ConfigurationManagerAttributes { }, true)));

        ConfigEntries.Add(SpawnHavenZoneCubeAtLookingPosition = config.Bind(ZoneBoxSettings, "Spawn Haven Zone Cube at Looking Position", true,
            new ConfigDescription(
                "Spawns the Zone Cube at the position you are looking at, if false it will spawn it at your feet.", null,
                new ConfigurationManagerAttributes { }, true)));

        ConfigEntries.Add(PositiveXKey = config.Bind(ZoneBoxSettings, "Transform Positive X", new KeyboardShortcut(KeyCode.Keypad1), new ConfigDescription(
            "Change Cube on the Positive X axis", null, new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(NegativeXKey = config.Bind(ZoneBoxSettings, "Transform Negative X", new KeyboardShortcut(KeyCode.Keypad4), new ConfigDescription(
            "Change Cube on the Negative X axis", null, new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(PositiveYKey = config.Bind(ZoneBoxSettings, "Transform Positive Y", new KeyboardShortcut(KeyCode.Keypad2), new ConfigDescription(
            "Change Cube on the Positive Y axis", null, new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(NegativeYKey = config.Bind(ZoneBoxSettings, "Transform Negative Y", new KeyboardShortcut(KeyCode.Keypad5), new ConfigDescription(
            "Change Cube on the Negative Y axis", null, new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(PositiveZKey = config.Bind(ZoneBoxSettings, "Transform Positive Z", new KeyboardShortcut(KeyCode.Keypad3), new ConfigDescription(
            "Change Cube on the Positive Z axis", null, new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(NegativeZKey = config.Bind(ZoneBoxSettings, "Transform Negative Z", new KeyboardShortcut(KeyCode.Keypad6), new ConfigDescription(
            "Change Cube on the Negative Z axis", null, new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(PositionModeKey = config.Bind(ZoneBoxSettings, "Position Mode", new KeyboardShortcut(KeyCode.Keypad7), new ConfigDescription(
            "Change to Position mode.", null, new ConfigurationManagerAttributes { }, true)));

        ConfigEntries.Add(ScaleModeKey = config.Bind(ZoneBoxSettings, "Scale Mode", new KeyboardShortcut(KeyCode.Keypad8), new ConfigDescription(
            "Change to Scale mode.", null, new ConfigurationManagerAttributes { }, true)));

        ConfigEntries.Add(RotateModeKey = config.Bind(ZoneBoxSettings, "Rotate Mode", new KeyboardShortcut(KeyCode.Keypad9), new ConfigDescription(
            "Change to Rotate mode.", null, new ConfigurationManagerAttributes { }, true)));

        ConfigEntries.Add(IncreaseTransformSpeed = config.Bind(ZoneBoxSettings, "Increase Transform Speed", new KeyboardShortcut(KeyCode.KeypadPlus),
            new ConfigDescription(
                "Increase the speed at which the object is transformed by 0.25", null, new ConfigurationManagerAttributes { }, true)));

        ConfigEntries.Add(DecreaseTransformSpeed = config.Bind(ZoneBoxSettings, "Decrease Transform Speed", new KeyboardShortcut(KeyCode.KeypadMinus),
            new ConfigDescription(
                "Decrease the speed at which the object is transformed by 0.25", null, new ConfigurationManagerAttributes { }, true)));

        ConfigEntries.Add(LockXAndZRotation = config.Bind(ZoneBoxSettings, "Lock X And Z Rotation Axes", true, new ConfigDescription(
            "Change to Lock X and Z rotation axes.", null, new ConfigurationManagerAttributes { }, true)));

        ConfigEntries.Add(DefaultScale = config.Bind(ZoneBoxSettings, "Default Scale", new Vector3(0.5f, 0.5f, 0.5f), new ConfigDescription(
            "The default scale of the Zone Cube.", null, new ConfigurationManagerAttributes { })));

        #endregion

        #region Config 3. VCQL Zone Settings

        ConfigEntries.Add(ZoneId = config.Bind(VCQLZoneSettings, "Zone Id", "", new ConfigDescription(
            "The id of the zone (spaces will be replaced with underscores and make it lowercase)", null, new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(ZoneName = config.Bind(VCQLZoneSettings, "Zone Name", "", new ConfigDescription(
            "The name of the zone (spaces will be replaced with underscores and make it lowercase)", null, new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(ZoneType = config.Bind(VCQLZoneSettings, "Zone Type", EZoneTypes.placeitem, new ConfigDescription(
            "The type of zone", null, new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(FlareType = config.Bind(VCQLZoneSettings, "Flare Type", EFlareTypes.none, new ConfigDescription(
            "The type of flare", null, new ConfigurationManagerAttributes { })));

        config.Bind(VCQLZoneSettings, "Generate VCQL Zone", false, new ConfigDescription(
            "Generates the Zone in the VCQL Zone folder.", null, new ConfigurationManagerAttributes { CustomDrawer = GenerateVCQLJson }));

        #endregion

        #region Config 4. LooseLoot Settings

        ConfigEntries.Add(LooseLootProbability = config.Bind(LooseLootSettings, "Probability", 1f, new ConfigDescription(
            "The probability of the loose loot.", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(LooseLootUseGravity = config.Bind(LooseLootSettings, "Use Gravity", false, new ConfigDescription(
            "Use gravity for the loose loot?", null, new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(LooseLootRandomRotation = config.Bind(LooseLootSettings, "Random Rotation", false, new ConfigDescription(
            "Random rotation for the loose loot?", null, new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(LooseLootItemId = config.Bind(LooseLootSettings, "Item Id (tpl)", "", new ConfigDescription(
            "The Id of the item (tpl)", null, new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(config.Bind(LooseLootSettings, "Generate Loose Loot", false, new ConfigDescription(
            "Generates the Loose Loot in the folder \"BepInEx\\plugins\\HavenZoneCreator\".", null,
            new ConfigurationManagerAttributes { CustomDrawer = GenerateLooseLootJson })));

        #endregion

        #region Config 5. Map Locations

        ConfigEntries.Add(AddMapLocationToListKey = config.Bind(ZoneBoxSettings, "Add Map Location to List", new KeyboardShortcut(KeyCode.UpArrow),
            new ConfigDescription(
                "Hotkey to add the current Zone Cube location to a list.", null, new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(RemoveMapLocationFromListKey = config.Bind(ZoneBoxSettings, "Remove Last Map Location from List",
            new KeyboardShortcut(KeyCode.DownArrow),
            new ConfigDescription(
                "Hotkey to remove the last Zone Cube location from the list.", null, new ConfigurationManagerAttributes { })));

        ConfigEntries.Add(config.Bind(MapLocations, "Spawn Map Location Cubes", false, new ConfigDescription(
            "Spawns cubes in the current map from the Map Locations List in the folder \"BepInEx\\plugins\\HavenZoneCreator\".", null,
            new ConfigurationManagerAttributes { CustomDrawer = SpawnMapLocationsList })));

        ConfigEntries.Add(config.Bind(MapLocations, "Remove Map Location Cubes", false, new ConfigDescription(
            "Removes all Map Location Cubes from the map.", null, new ConfigurationManagerAttributes { CustomDrawer = RemoveMapLocationCubes })));

        ConfigEntries.Add(config.Bind(MapLocations, "Remove Map Location List", false, new ConfigDescription(
            "Removes all Map Location Cubes from the map.", null, new ConfigurationManagerAttributes { CustomDrawer = RemoveMapLocationList })));

        ConfigEntries.Add(config.Bind(MapLocations, "Reset Map Locations", false, new ConfigDescription(
            "Resets the Map Locations List. (not the json file)", null, new ConfigurationManagerAttributes { CustomDrawer = ResetMapLocationsList })));

        ConfigEntries.Add(config.Bind(MapLocations, "Generate Map Locations File", false, new ConfigDescription(
            "Generates the Map Locations List in the folder \"BepInEx\\plugins\\HavenZoneCreator\".", null,
            new ConfigurationManagerAttributes { CustomDrawer = GenerateMapLocationsJson })));

        #endregion

        #region Subscriptions

        TransformSpeed.Subscribe(value => { });
        HavenZoneCube.Subscribe(value => { });
        CurrentZoneCubePosition.Subscribe(value => { });
        SpawnHavenZoneCubeAtLookingPosition.Subscribe(value => { });
        ZoneCubeTransparency.Subscribe(value => { });
        PositiveXKey.Subscribe(value => { });
        PositiveYKey.Subscribe(value => { });
        PositiveZKey.Subscribe(value => { });
        PositionModeKey.Subscribe(value => { });
        ScaleModeKey.Subscribe(value => { });
        RotateModeKey.Subscribe(value => { });
        LockXAndZRotation.Subscribe(value => { });

        #endregion

        #region Default Values

        CurrentZoneCubePosition.Value = Vector3.zero;
        CurrentZoneCubeScale.Value = DefaultScale.Value;
        CurrentZoneCubeRotation.Value = Quaternion.identity;
        CurrentMapName.Value = "";
        ZoneId.Value = "";
        ZoneName.Value = "";
        LooseLootItemId.Value = "";
        ZoneType.Value = EZoneTypes.placeitem;
        FlareType.Value = EFlareTypes.none;

        #endregion

        RecalcOrder();
    }

    private static void RecalcOrder()
    {
        // Set the Order field for all settings, to avoid unnecessary changes when adding new settings
        int settingOrder = ConfigEntries.Count;
        foreach (var entry in ConfigEntries)
        {
            if (entry.Description.Tags[0] is ConfigurationManagerAttributes attributes)
            {
                attributes.Order = settingOrder;
            }

            settingOrder--;
        }
    }

    // Credit to DrakiaXYZ, Modified by CathieNova!
    public static bool IsKeyPressed(KeyboardShortcut key, bool holdKey = false)
    {
        if (holdKey)
        {
            if (!UnityInput.Current.GetKey(key.MainKey))
            {
                return false;
            }

            foreach (var modifier in key.Modifiers)
            {
                if (!UnityInput.Current.GetKey(modifier))
                {
                    return false;
                }
            }
        }
        else
        {
            if (!UnityInput.Current.GetKeyDown(key.MainKey))
            {
                return false;
            }

            foreach (var modifier in key.Modifiers)
            {
                if (!UnityInput.Current.GetKey(modifier))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool IsKeyReleased(KeyboardShortcut key)
    {
        if (!UnityInput.Current.GetKey(key.MainKey))
        {
            foreach (var modifier in key.Modifiers)
            {
                if (!UnityInput.Current.GetKey(modifier))
                {
                    return false;
                }
            }

            return true; // Main key and modifiers are no longer pressed
        }

        return false; // Main key is still pressed
    }

    private static void GenerateVCQLJson(ConfigEntryBase entry)
    {
        if (GUILayout.Button("Generate VCQL Zone", GUILayout.ExpandWidth(true)))
            ExportJsonFile.GenerateJson(ExportJsonFile.JsonType.VCQL);
    }

    private static void GenerateLooseLootJson(ConfigEntryBase entry)
    {
        if (GUILayout.Button("Generate Loose Loot", GUILayout.ExpandWidth(true)))
            ExportJsonFile.GenerateJson(ExportJsonFile.JsonType.LooseLoot);
    }

    private static void GenerateMapLocationsJson(ConfigEntryBase entry)
    {
        if (GUILayout.Button("Generate Map Locations File", GUILayout.ExpandWidth(true)))
            ExportJsonFile.GenerateJson(ExportJsonFile.JsonType.MapLocation);
    }

    private static void RemoveMapLocationCubes(ConfigEntryBase entry)
    {
        if (GUILayout.Button("Remove Map Location Cubes", GUILayout.ExpandWidth(true)))
        {
            if (CurrentMapName.Value == "")
            {
                Plugin.Logger.LogMessage("[HavenZoneCreator] Go to a map to remove Map Location Cubes.");
                NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Go to a map to remove Map Location Cubes.",
                    ENotificationDurationType.Default, ENotificationIconType.Alert);
                return;
            }

            var mapLocationCubes = GameObject.FindObjectsOfType<MapLocationMarker>();
            foreach (var mapLocationCube in mapLocationCubes)
            {
                GameObject.Destroy(mapLocationCube.gameObject);
            }

            Plugin.Logger.LogMessage("[HavenZoneCreator] Removed All Map Location Cubes.");
            NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Removed All Map Location Cubes.");
        }
    }

    private static void RemoveMapLocationList(ConfigEntryBase entry)
    {
        if (GUILayout.Button("Remove Map Location List", GUILayout.ExpandWidth(true)))
        {
            if (CurrentMapName.Value == "")
            {
                NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Go to a map to remove Map Location Cubes.",
                    ENotificationDurationType.Default, ENotificationIconType.Alert);
                return;
            }

            if (CubeDataList.Count == 0)
            {
                Plugin.Logger.LogMessage("[HavenZoneCreator] Map Locations List is already empty.");
                NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Map Locations List is already empty.",
                    ENotificationDurationType.Default, ENotificationIconType.Alert);
                return;
            }

            CubeDataList.Clear();
            Plugin.Logger.LogMessage("[HavenZoneCreator] Map Locations List has been cleared.");
            NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Map Locations List has been cleared.");
        }
    }

    private static void ResetMapLocationsList(ConfigEntryBase entry)
    {
        if (GUILayout.Button("Reset Map Locations", GUILayout.ExpandWidth(true)))
        {
            if (CurrentMapName.Value == "")
            {
                NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Go to a map to reset Map Locations List.",
                    ENotificationDurationType.Default, ENotificationIconType.Alert);
                return;
            }

            if (CubeDataList.Count == 0)
            {
                Plugin.Logger.LogMessage("[HavenZoneCreator] Map Locations List is already empty.");
                NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Map Locations List is already empty.",
                    ENotificationDurationType.Default, ENotificationIconType.Alert);
                return;
            }

            CubeDataList.Clear();
            Plugin.Logger.LogMessage("[HavenZoneCreator] Map Locations List has been reset.");
            NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Map Locations List has been reset.");
        }
    }

    private async static void SpawnMapLocationsList(ConfigEntryBase entry)
    {
        if (GUILayout.Button("Spawn Map Location Cubes", GUILayout.ExpandWidth(true)))
        {
            if (CurrentMapName.Value == "")
            {
                NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Go to a map to spawn Map Location Cubes.",
                    ENotificationDurationType.Default, ENotificationIconType.Alert);
                return;
            }

            Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = Path.Combine(Directory, "HavenZoneCreator", "MapLocations.json");

            MapLocations.MapsLocations mapLocationsData;
            try
            {
                string fileContent = File.ReadAllText(filePath);
                mapLocationsData = JsonConvert.DeserializeObject<MapLocations.MapsLocations>(fileContent);
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogMessage($"[HavenZoneCreator]: Error parsing MapLocations.json: {ex.Message}");
                NotificationManagerClass.DisplayMessageNotification($"[HavenZoneCreator]: Error parsing MapLocations.json: {ex.Message}",
                    ENotificationDurationType.Default, ENotificationIconType.Alert);
                return;
            }

            if (mapLocationsData == null)
            {
                Plugin.Logger.LogMessage("[HavenZoneCreator]: Failed to parse MapLocations.json.");
                NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator]: Failed to parse MapLocations.json.",
                    ENotificationDurationType.Default, ENotificationIconType.Alert);
                return;
            }

            if (!MapIdToNameMap.TryGetValue(CurrentMapName.Value, out string location))
            {
                Plugin.Logger.LogMessage($"[HavenZoneCreator]: Unknown map ID '{location}'.");
                NotificationManagerClass.DisplayMessageNotification($"[HavenZoneCreator]: Unknown map ID '{location}'.", ENotificationDurationType.Default,
                    ENotificationIconType.Alert);
                return;
            }

            List<MapLocations.Location> mapLocations = GetMapLocations(location, mapLocationsData);
            if (mapLocations == null || mapLocations.Count == 0)
            {
                Plugin.Logger.LogMessage($"[HavenZoneCreator]: No locations found for {location}.");
                NotificationManagerClass.DisplayMessageNotification($"[HavenZoneCreator]: No locations found for {location}.",
                    ENotificationDurationType.Default, ENotificationIconType.Alert);
                return;
            }
            Plugin.Logger.LogMessage($"[HavenZoneCreator]: Spawning {mapLocations.Count} Zone Cubes on map '{location}'.");
#if DEBUG
            NotificationManagerClass.DisplayMessageNotification($"[HavenZoneCreator]: Spawning {mapLocations.Count} Zone Cubes on map '{location}'.",
                ENotificationDurationType.Default, ENotificationIconType.Alert);
#endif
            for (int i = 0; i < mapLocations.Count; i++)
            {
                var locationData = mapLocations[i];
                GameObject mapLocationsCube = GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube),
                    new Vector3(locationData.Position.X, locationData.Position.Y, locationData.Position.Z),
                    Quaternion.Euler(locationData.Rotation.X, locationData.Rotation.Y, locationData.Rotation.Z)
                );
                mapLocationsCube.transform.localScale = DefaultScale.Value;
                mapLocationsCube.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.5f);
                mapLocationsCube.AddComponent<MapLocationMarker>();
                await Task.Delay(2);
            }
        }
    }

    private static List<MapLocations.Location> GetMapLocations(string location, MapLocations.MapsLocations mapLocationsData)
    {
        return location switch
        {
            "Interchange" => mapLocationsData.Interchange,
            "FactoryDay" => mapLocationsData.FactoryDay,
            "FactoryNight" => mapLocationsData.FactoryNight,
            "Customs" => mapLocationsData.Customs,
            "Woods" => mapLocationsData.Woods,
            "Lighthouse" => mapLocationsData.Lighthouse,
            "Shoreline" => mapLocationsData.Shoreline,
            "Reserve" => mapLocationsData.Reserve,
            "Laboratory" => mapLocationsData.Laboratory,
            "StreetsOfTarkov" => mapLocationsData.StreetsOfTarkov,
            "GroundZero" => mapLocationsData.GroundZero,
            "GroundZero21+" => mapLocationsData.GroundZero21,
            _ => null
        };
    }


    public static readonly Dictionary<string, string> MapIdToNameMap = new()
    {
        { "Interchange", "Interchange" },
        { "factory4_day", "FactoryDay" },
        { "factory4_night", "FactoryNight" },
        { "bigmap", "Customs" },
        { "Woods", "Woods" },
        { "Lighthouse", "Lighthouse" },
        { "Shoreline", "Shoreline" },
        { "RezervBase", "Reserve" },
        { "laboratory", "Laboratory" },
        { "TarkovStreets", "StreetsOfTarkov" },
        { "Sandbox", "Sandbox" },
        { "Sandbox_high", "Sandbox_high" }
    };

    public class Location
    {
        public Vector3 Position { get; set; } = Vector3.zero;
        public Vector3 Rotation { get; set; } = Vector3.zero;
    }
}

internal static class SettingExtensions
{
    public static void Subscribe<T>(this ConfigEntry<T> configEntry, Action<T> onChange, bool notification = false)
    {
        configEntry.SettingChanged += (_, _) =>
        {
            onChange(configEntry.Value);
            if (notification)
                NotificationManagerClass.DisplayMessageNotification($"[HavenZoneCreator] Setting {configEntry.Value} changed to {configEntry.Value}");
        };
    }

    public static void Bind<T>(this ConfigEntry<T> configEntry, Action<T> onChange, bool notification = false)
    {
        configEntry.Subscribe(onChange, notification);
        onChange(configEntry.Value);
    }
}