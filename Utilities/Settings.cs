using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace HavenZoneCreator.Utilities
{
    internal class Settings
    {
        private static readonly List<ConfigEntryBase> ConfigEntries = [];
        
        #region Categories

        private const string ZoneInformation = "1. Zone Information";
        private const string ZoneBoxSettings = "2. Zone Box Settings";
        private const string VCQLZoneSettings = "3. VCQL Zone Settings";
        private const string LooseLootSettings = "4. LooseLoot Settings";

        #endregion
        
        #region 1. Zone Information
        
        public static ConfigEntry<Vector3> CurrentLookPosition { get; set; }
        public static ConfigEntry<Quaternion> CurrentLookRotation { get; set; }
        public static ConfigEntry<Vector3> CurrentLookScale { get; set; }
        
        public static ConfigEntry<float> LookPositionCubeTransparency { get; set; }
        
        #endregion
        
        #region 2. Zone Box Settings
        
        public static ConfigEntry<float> TransformSpeed { get; set; }
        public static ConfigEntry<KeyboardShortcut> FetchLookPosition { get; set; }
        public static ConfigEntry<KeyboardShortcut> RemoveLookPosition { get; set; }
        public static ConfigEntry<KeyboardShortcut> PositiveXKey { get; set; }
        public static ConfigEntry<KeyboardShortcut> NegativeXKey { get; set; }
        public static ConfigEntry<KeyboardShortcut> PositiveYKey { get; set; }
        public static ConfigEntry<KeyboardShortcut> NegativeYKey { get; set; }
        public static ConfigEntry<KeyboardShortcut> PositiveZKey { get; set; }
        public static ConfigEntry<KeyboardShortcut> NegativeZKey { get; set; }
        public static ConfigEntry<KeyboardShortcut> PositionModeKey { get; set; }
        public static ConfigEntry<KeyboardShortcut> ScaleModeKey { get; set; }
        public static ConfigEntry<KeyboardShortcut> RotateModeKey { get; set; }
        public static ConfigEntry<bool> LockXAndZRotation { get; set; }
        public static ConfigEntry<Vector3> DefaultScale { get; set; }
        
        #endregion
        
        #region 3. VCQL Zone Settings
        
        public static ConfigEntry<string> ZoneId { get; set; }
        public static ConfigEntry<string> ZoneName { get; set; }
        public static ConfigEntry<EZoneTypes> ZoneType { get; set; }
        public static ConfigEntry<EFlareTypes> FlareType { get; set; }
        
        #endregion
        
        #region Loose Loot Settings

        public static ConfigEntry<float> LooseLootProbability { get; set; }
        public static ConfigEntry<bool> LooseLootUseGravity { get; set; }
        public static ConfigEntry<bool> LooseLootRandomRotation { get; set; }
        public static ConfigEntry<string> LooseLootItemId { get; set; }

        #endregion

        public static void Init(ConfigFile config)
        {
            #region Config 1. Zone Information
            
            ConfigEntries.Add(CurrentLookPosition = config.Bind(ZoneInformation, "Current Look Position", Vector3.zero, new ConfigDescription(
                "The position you were looking at when pressing 'Fetch Look Position'", null, new ConfigurationManagerAttributes { ReadOnly = true})));
            
            ConfigEntries.Add(CurrentLookRotation = config.Bind(ZoneInformation, "Current Look Rotation", Quaternion.identity, new ConfigDescription(
                "The rotation you were looking at when pressing 'Fetch Look Position'", null, new ConfigurationManagerAttributes { ReadOnly = true})));
            
            ConfigEntries.Add(CurrentLookScale = config.Bind(ZoneInformation, "Current Look Scale", Vector3.zero, new ConfigDescription(
                "The scale you were looking at when pressing 'Fetch Look Position'", null, new ConfigurationManagerAttributes { ReadOnly = true})));
            
            ConfigEntries.Add(LookPositionCubeTransparency = config.Bind(ZoneInformation, "Look Position Cube Transparency", 0.25f, new ConfigDescription(
                "Transparency of the look position cube", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { })));
            
            #endregion
            
            #region Config 2. Zone Box Settings
            
            ConfigEntries.Add(TransformSpeed = config.Bind(ZoneBoxSettings, "Transform Speed", 1f, new ConfigDescription(
                "The speed at which the object is transformed", new AcceptableValueRange<float>(0.01f, 10f), new ConfigurationManagerAttributes { })));
            
            ConfigEntries.Add(FetchLookPosition = config.Bind(ZoneBoxSettings, "Fetch Look Position", new KeyboardShortcut(KeyCode.Keypad0), new ConfigDescription(
                "Fetches the position you are looking at and generates a Zone Cube.", null, new ConfigurationManagerAttributes { }, true)));
            
            ConfigEntries.Add(RemoveLookPosition = config.Bind(ZoneBoxSettings, "Remove Look Position", new KeyboardShortcut(KeyCode.KeypadEnter), new ConfigDescription(
                "Removes the look position Zone Cube.", null, new ConfigurationManagerAttributes { }, true)));
            
            ConfigEntries.Add(PositiveXKey = config.Bind(ZoneBoxSettings, "Transform Positive X", new KeyboardShortcut(KeyCode.Keypad1), new ConfigDescription(
                "Move Positive X axis", null, new ConfigurationManagerAttributes { })));
            
            ConfigEntries.Add(NegativeXKey = config.Bind(ZoneBoxSettings, "Transform Negative X", new KeyboardShortcut(KeyCode.Keypad4), new ConfigDescription(
                "Move Negative X axis", null, new ConfigurationManagerAttributes { })));
            
            ConfigEntries.Add(PositiveYKey = config.Bind(ZoneBoxSettings, "Transform Positive Y", new KeyboardShortcut(KeyCode.Keypad2), new ConfigDescription(
                "Positive Y axis", null, new ConfigurationManagerAttributes { })));
            
            ConfigEntries.Add(NegativeYKey = config.Bind(ZoneBoxSettings, "Transform Negative Y", new KeyboardShortcut(KeyCode.Keypad5), new ConfigDescription(
                "Negative Y axis", null, new ConfigurationManagerAttributes { })));
            
            ConfigEntries.Add(PositiveZKey = config.Bind(ZoneBoxSettings, "Transform Positive Z", new KeyboardShortcut(KeyCode.Keypad3), new ConfigDescription(
                "Positive Z axis", null, new ConfigurationManagerAttributes { })));
            
            ConfigEntries.Add(NegativeZKey = config.Bind(ZoneBoxSettings, "Transform Negative Z", new KeyboardShortcut(KeyCode.Keypad6), new ConfigDescription(
                "Negative Z axis", null, new ConfigurationManagerAttributes { })));
            
            ConfigEntries.Add(PositionModeKey = config.Bind(ZoneBoxSettings, "Position", new KeyboardShortcut(KeyCode.Keypad7), new ConfigDescription(
                "Position mode", null, new ConfigurationManagerAttributes { }, true)));
            
            ConfigEntries.Add(ScaleModeKey = config.Bind(ZoneBoxSettings, "Scale", new KeyboardShortcut(KeyCode.Keypad8), new ConfigDescription(
                "Change to Scale mode", null, new ConfigurationManagerAttributes { }, true)));
            
            ConfigEntries.Add(RotateModeKey = config.Bind(ZoneBoxSettings, "Rotate", new KeyboardShortcut(KeyCode.Keypad9), new ConfigDescription(
                "Change to Rotate mode", null, new ConfigurationManagerAttributes { }, true)));
            
            ConfigEntries.Add(LockXAndZRotation = config.Bind(ZoneBoxSettings, "Lock X And Z Rotation Axes", true, new ConfigDescription(
                "Change to Lock X and Z rotation axes", null, new ConfigurationManagerAttributes { }, true)));
            
            ConfigEntries.Add(DefaultScale = config.Bind(ZoneBoxSettings, "Default Scale", new Vector3(0.5f, 0.5f, 0.5f), new ConfigDescription(
                "The default scale of the Zone Cube.", null, new ConfigurationManagerAttributes { })));
            
            #endregion
            
            #region Config 3. VCQL Zone Settings
            
            ConfigEntries.Add(ZoneId = config.Bind(VCQLZoneSettings, "Zone Id", "", new ConfigDescription(
                "The id of the zone", null, new ConfigurationManagerAttributes { })));
            
            ConfigEntries.Add(ZoneName = config.Bind(VCQLZoneSettings, "Zone Name", "", new ConfigDescription(
                "The name of the zone", null, new ConfigurationManagerAttributes { })));
            
            ConfigEntries.Add(ZoneType = config.Bind(VCQLZoneSettings, "Zone Type", EZoneTypes.placeitem, new ConfigDescription(
                "The type of the zone", null, new ConfigurationManagerAttributes { })));
            
            ConfigEntries.Add(FlareType = config.Bind(VCQLZoneSettings, "Flare Type", EFlareTypes.none, new ConfigDescription(
                "The type of the flare", null, new ConfigurationManagerAttributes { })));
            
            config.Bind(VCQLZoneSettings, "Generate VCQL Zone", false, new ConfigDescription(
                "Generates the Zone in the VCQL Zone folder.", null, new ConfigurationManagerAttributes { CustomDrawer = GenerateJson }));
            
            #endregion
            
            #region Config 4. LooseLoot Settings
            
            ConfigEntries.Add(LooseLootProbability = config.Bind(LooseLootSettings, "Probability", 1f, new ConfigDescription(
                "The probability of the loose loot.", new AcceptableValueRange<float>(0f, 1f), new ConfigurationManagerAttributes { })));
            
            ConfigEntries.Add(LooseLootUseGravity = config.Bind(LooseLootSettings, "Use Gravity", false, new ConfigDescription(
                "Use gravity for the loose loot?", null, new ConfigurationManagerAttributes { })));
            
            ConfigEntries.Add(LooseLootRandomRotation = config.Bind(LooseLootSettings, "Random Rotation", false, new ConfigDescription(
                "Random rotation for the loose loot?", null, new ConfigurationManagerAttributes { })));
            
            ConfigEntries.Add(LooseLootItemId = config.Bind(LooseLootSettings, "Item Id", "", new ConfigDescription(
                "The Id of the item", null, new ConfigurationManagerAttributes { })));
            
            ConfigEntries.Add(config.Bind(LooseLootSettings, "Generate Loose Loot", false, new ConfigDescription(
                "Generates the Loose Loot in the folder \"_EXPORTED_LOOT_\" in your SPT folder.", null, new ConfigurationManagerAttributes { CustomDrawer = GenerateJson })));
            
            #endregion

            #region Subscriptions
            
            TransformSpeed.Subscribe(value => {});
            FetchLookPosition.Subscribe(value => {});
            CurrentLookPosition.Subscribe(value => {});
            LookPositionCubeTransparency.Subscribe(value => {});
            PositiveXKey.Subscribe(value => {});
            PositiveYKey.Subscribe(value => {});
            PositiveZKey.Subscribe(value => {});
            PositionModeKey.Subscribe(value => {});
            ScaleModeKey.Subscribe(value => {});
            RotateModeKey.Subscribe(value => {});
            LockXAndZRotation.Subscribe(value => {});
            
            #endregion

            CurrentLookPosition.Value = Vector3.zero;
            
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

        private static void GenerateJson(ConfigEntryBase entry)
        {
            if (GUILayout.Button("Generate VCQL Zone", GUILayout.ExpandWidth(true)))
                ExportJsonFile.GenerateJson(ExportJsonFile.JsonType.VCQL);
            else if (GUILayout.Button("Generate Loose Loot", GUILayout.ExpandWidth(true)))
                ExportJsonFile.GenerateJson(ExportJsonFile.JsonType.LooseLoot);
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
                    NotificationManagerClass.DisplayMessageNotification($"Setting {configEntry.Value} changed to {configEntry.Value}");
            };
        }
        
        public static void Bind<T>(this ConfigEntry<T> configEntry, Action<T> onChange, bool notification = false)
        {
            configEntry.Subscribe(onChange, notification);
            onChange(configEntry.Value);
        }
    }
}