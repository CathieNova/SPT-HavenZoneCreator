using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using EFT.Communications;
using EFT.UI;
using HavenZoneCreator.Utilities;
using UnityEngine;

namespace HavenZoneCreator.Features;

public class HavenZoneCubeComponent : MonoBehaviour
{
    private static Player Player;
    private static Camera Camera;
    private GameObject LookPositionGameObject;
    private bool isIncreaseKeyHeld = false;
    private bool isDecreaseKeyHeld = false;
    private static readonly Dictionary<string, (AssetBundle Bundle, int RefCount)> LoadedBundles = new();

    public EInputMode Mode = EInputMode.Position;

    public enum EInputMode
    {
        Position,
        Scale,
        Rotate
    }
    
    private static readonly string[] PrefixesToSkip = new[]
    {
        "Default", "Base Human", "Root_Joint", "Player", "AICollider",
        "BornPositions", "BP.", "AITerrain", "TEMP_", "Slice"
    };

    protected ManualLogSource Logger { get; private set; }

    private HavenZoneCubeComponent()
    {
        if (Logger == null)
            Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(HavenZoneCubeComponent));
    }
    
    internal static void Enable()
    {
        if (Singleton<IBotGame>.Instantiated)
        {
            var gameWorld = Singleton<GameWorld>.Instance;
            gameWorld.GetOrAddComponent<HavenZoneCubeComponent>();
            Player = gameWorld.MainPlayer;
            Camera = Camera.main;
            Settings.CurrentZoneCubePosition.Value = Vector3.zero;
        }
    }

    private void Start()
    {
        Settings.ZoneCubeTransparency.SettingChanged += (sender, args) =>
        {
            SetTransparentColor(Settings.ZoneCubeTransparency.Value);
        };
    }

    private void Update()
    {
        if (!Player || !Camera) return;

        TransformSpeedCheck();

        if (Settings.RemoveHavenZoneCube.Value.IsDown() && LookPositionGameObject)
        {
            Destroy(LookPositionGameObject);
            LookPositionGameObject = null;
            Settings.CurrentZoneCubePosition.Value = Vector3.zero;
            Settings.CurrentZoneCubeRotation.Value = Quaternion.identity;
            Settings.CurrentZoneCubeScale.Value = Vector3.zero;
            NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Removed 'Haven Zone Cube'.");
            return;
        }

        if (Settings.IsKeyPressed(Settings.HavenZoneCube.Value))
        {
            Vector3 hitPoint = Vector3.zero;
            bool validHitFound = false;

            if (Settings.SpawnHavenZoneCubeAtLookingPosition.Value)
            {
                var ray = new Ray(Camera.transform.position, Camera.transform.forward);
                int layerMask = LayerMask.GetMask("HighPolyCollider", "LowPolyCollider", "Interactive", "Loot", "Terrain", "DoorLowPolyCollider", "Water");
                RaycastHit[] hits = new RaycastHit[500];
                int hitCount = Physics.RaycastNonAlloc(ray, hits, Mathf.Infinity, layerMask);

                for (int i = 0; i < hitCount; i++)
                {
                    var hit = hits[i];
                    if (ShouldSkipObject(hit.collider.gameObject.name)) continue;

                    hitPoint = hit.point;
                    validHitFound = true;
                    break;
                }
            }

            if (!validHitFound && Settings.SpawnHavenZoneCubeAtLookingPosition.Value)
            {
                Plugin.Logger.LogError("[HavenZoneCreator] No valid hit found to spawn the 'Haven Zone Cube'.");
                return;
            }

            if (!LookPositionGameObject)
            {
                GameObject instance;
                if (string.IsNullOrWhiteSpace(Settings.ZoneCubePrefab.Value))
                {
                    // Create a default cube
                    instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    instance.GetComponent<Renderer>().enabled = true;
                    
                    var collider = instance.GetComponent<Collider>();
                    if (collider != null)
                    {
                        Destroy(collider);
                    }
                }
                else
                {
                    // Load prefab from AssetBundle
                    string prefabPath = Path.Combine($@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\HavenZoneCreator\Prefabs\",
                        $"{Settings.ZoneCubePrefab.Value}.bundle");
                    string prefabName = Path.GetFileNameWithoutExtension(Settings.ZoneCubePrefab.Value);

                    // Use the helper method to load the prefab
                    var prefab = LoadPrefabFromBundle(prefabPath, prefabName);
                    if (prefab == null) return;

                    // Instantiate the prefab
                    instance = Instantiate(prefab);
                    
                    var colliders = instance.GetComponentsInChildren<Collider>();
                    foreach (var collider in colliders)
                    {
                        Destroy(collider);
                    }

                    // Note: Do not unload the AssetBundle immediately if you still need it later in the runtime.
                    // If unloading is necessary at this point, call:
                    UnloadAssetBundle(prefabPath);
                }

                // Place and scale the object
                Vector3 spawnPosition = Settings.SpawnHavenZoneCubeAtLookingPosition.Value
                    ? hitPoint + (Player.Transform.position - hitPoint).normalized * 0.01f
                    : Camera.transform.position;

                instance.transform.position = spawnPosition;
                instance.transform.localScale = string.IsNullOrWhiteSpace(Settings.ZoneCubePrefab.Value) 
                    ? Settings.DefaultScale.Value 
                    : Vector3.one;
                instance.name = "Haven Zone Cube";

                // Apply transparency if it's a default cube
                if (string.IsNullOrWhiteSpace(Settings.ZoneCubePrefab.Value))
                {
                    var renderer = instance.GetComponent<Renderer>();
                    var material = new Material(Shader.Find("Standard"));
                    material.SetFloat("_Mode", 3);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.SetFloat("_Glossiness", 0f);
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    renderer.material = material;
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }

                LookPositionGameObject = instance;
                Settings.CurrentZoneCubePosition.Value = instance.transform.position;
                Settings.CurrentZoneCubeRotation.Value = Quaternion.Euler(0, Camera.transform.localRotation.eulerAngles.y, 0);
                instance.transform.rotation = Settings.CurrentZoneCubeRotation.Value;

                if (Settings.ZoneCubePrefab.Value == "")
                {
                    SetColor(Color.green);
                    SetTransparentColor(Settings.ZoneCubeTransparency.Value);
                    NotificationManagerClass.DisplayMessageNotification($"[HavenZoneCreator] Created new 'Haven Zone Cube' at {spawnPosition}");
                }
                else
                {
                    NotificationManagerClass.DisplayMessageNotification($"[HavenZoneCreator] Created new 'Haven Zone Cube' from prefab '{Settings.ZoneCubePrefab.Value}' at {spawnPosition}");
                }
            }
            else
            {
                // Move existing object
                Vector3 movePosition = Settings.SpawnHavenZoneCubeAtLookingPosition.Value
                    ? hitPoint + (Camera.transform.position - hitPoint).normalized * 0.01f
                    : Camera.transform.position;

                LookPositionGameObject.transform.position = movePosition;
                Settings.CurrentZoneCubePosition.Value = movePosition;

                float currentY = Camera.transform.localRotation.eulerAngles.y;
                Settings.CurrentZoneCubeRotation.Value = Quaternion.Euler(0, currentY, 0);
                LookPositionGameObject.transform.rotation = Settings.CurrentZoneCubeRotation.Value;

                NotificationManagerClass.DisplayMessageNotification($"[HavenZoneCreator] Moved 'Haven Zone Cube' to {movePosition}");
            }
        }

        if (!LookPositionGameObject) return;

        if (Settings.IsKeyPressed(Settings.PositionModeKey.Value)) ChangeMode(EInputMode.Position);
        if (Settings.IsKeyPressed(Settings.ScaleModeKey.Value)) ChangeMode(EInputMode.Scale);
        if (Settings.IsKeyPressed(Settings.RotateModeKey.Value)) ChangeMode(EInputMode.Rotate);

        float delta = Time.deltaTime;
        float speed = Settings.TransformSpeed.Value;

        switch (Mode)
        {
            case EInputMode.Position:
                HandlePosition(speed, delta);
                break;
            case EInputMode.Rotate:
                HandleRotation(speed, delta);
                break;
            case EInputMode.Scale:
                HandleScaling(speed, delta);
                break;
        }

        if (Settings.IsKeyPressed(Settings.AddMapLocationToListKey.Value))
        {
            AddCubeData();
        }

        if (Settings.IsKeyPressed(Settings.RemoveMapLocationFromListKey.Value) && Settings.CubeDataList.Count > 0)
        {
            Settings.CubeDataList.RemoveAt(Settings.CubeDataList.Count - 1);
            NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Removed last Map Position from the list.");
        }
    }

    private AssetBundle GetOrLoadAssetBundle(string prefabPath)
    {
        if (LoadedBundles.TryGetValue(prefabPath, out var bundleInfo))
        {
            // Increment the reference count and return the existing bundle
            LoadedBundles[prefabPath] = (bundleInfo.Bundle, bundleInfo.RefCount + 1);
            return bundleInfo.Bundle;
        }

        // Load the bundle if not already cached
        var bundle = AssetBundle.LoadFromFile(prefabPath);
        if (bundle == null)
        {
            Plugin.Logger.LogError($"[HavenZoneCreator] Failed to load AssetBundle: {prefabPath}");
            return null;
        }

        // Cache the bundle with an initial reference count
        LoadedBundles[prefabPath] = (bundle, 1);
        return bundle;
    }

    private void UnloadAssetBundle(string prefabPath)
    {
        if (LoadedBundles.TryGetValue(prefabPath, out var bundleInfo))
        {
            if (bundleInfo.RefCount > 1)
            {
                LoadedBundles[prefabPath] = (bundleInfo.Bundle, bundleInfo.RefCount - 1);
            }
            else
            {
                bundleInfo.Bundle.Unload(false);
                LoadedBundles.Remove(prefabPath);
                Plugin.Logger.LogInfo($"[HavenZoneCreator] Unloaded AssetBundle: {prefabPath}");
            }
        }
    }
    
    private GameObject LoadPrefabFromBundle(string prefabPath, string prefabName)
    {
        // Load or get the AssetBundle
        var bundle = GetOrLoadAssetBundle(prefabPath);
        if (bundle == null)
        {
            Plugin.Logger.LogError($"[HavenZoneCreator] Failed to load AssetBundle: {prefabPath}");
            return null;
        }

#if DEBUG
        Plugin.Logger.LogInfo($"[HavenZoneCreator] Successfully loaded AssetBundle: {prefabPath}");
#endif

        // Log contents of the AssetBundle
        var assetNames = bundle.GetAllAssetNames();
        foreach (var assetName in assetNames)
        {
#if DEBUG
            Plugin.Logger.LogError($"AssetBundle contains: {assetName}");
#endif
        }

        // Find the prefab in the AssetBundle (case-insensitive match for safety)
        var matchingAssetName = assetNames.FirstOrDefault(asset => 
            asset.EndsWith($"{prefabName}.prefab", StringComparison.OrdinalIgnoreCase));

        if (matchingAssetName == null)
        {
            Plugin.Logger.LogError($"[HavenZoneCreator] Prefab '{prefabName}' not found in AssetBundle: {prefabPath}");
            UnloadAssetBundle(prefabPath);
            return null;
        }

        // Load the prefab using the exact path
        var prefab = bundle.LoadAsset<GameObject>(matchingAssetName);
        if (prefab == null)
        {
            Plugin.Logger.LogError($"[HavenZoneCreator] Failed to load prefab: {matchingAssetName}");
            UnloadAssetBundle(prefabPath);
            return null;
        }

#if DEBUG
        Plugin.Logger.LogInfo($"[HavenZoneCreator] Successfully loaded prefab: {matchingAssetName}");
#endif
        return prefab;
    }

    private void TransformSpeedCheck()
    {
        // Increase transform speed
        if (Settings.IsKeyPressed(Settings.IncreaseTransformSpeed.Value, true) && LookPositionGameObject)
        {
            isIncreaseKeyHeld = true;
            Settings.TransformSpeed.Value = Mathf.Clamp(Mathf.Round((Settings.TransformSpeed.Value + 0.1f) * 100f) / 100f, 0.25f, 10f);
        }

        if (isIncreaseKeyHeld && Settings.IsKeyReleased(Settings.IncreaseTransformSpeed.Value))
        {
            isIncreaseKeyHeld = false;
            NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Transform Speed Increased to " + Settings.TransformSpeed.Value);
        }

        // Decrease transform speed
        if (Settings.IsKeyPressed(Settings.DecreaseTransformSpeed.Value, true) && LookPositionGameObject)
        {
            isDecreaseKeyHeld = true;
            Settings.TransformSpeed.Value = Mathf.Clamp(Mathf.Round((Settings.TransformSpeed.Value - 0.1f) * 100f) / 100f, 0.25f, 10f);
        }

        if (isDecreaseKeyHeld && Settings.IsKeyReleased(Settings.DecreaseTransformSpeed.Value))
        {
            isDecreaseKeyHeld = false;
            NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Transform Speed Decreased to " + Settings.TransformSpeed.Value);
        }
    }

    public void ChangeMode(EInputMode _mode)
    {
        if (Settings.PositionModeKey.Value.IsDown())
        {
            Mode = EInputMode.Position;
            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuInstallModFunc);
            if (Settings.ZoneCubePrefab.Value == "")
                SetColor(Color.green);
            NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Translation Mode Activated.");
        }

        if (Settings.ScaleModeKey.Value.IsDown())
        {
            Mode = EInputMode.Scale;
            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuInstallModGear);
            if (Settings.ZoneCubePrefab.Value == "")
                SetColor(Color.blue);
            NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Scaling Mode Activated.");
        }

        if (Settings.RotateModeKey.Value.IsDown())
        {
            Mode = EInputMode.Rotate;
            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuInstallModVital);
            if (Settings.ZoneCubePrefab.Value == "")
                SetColor(Color.red);
            NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Rotation Mode Activated.");
        }
    }

    public void HandlePosition(float speed, float delta)
    {
        if (Settings.NegativeXKey.Value.IsPressed())
            MoveLP("x", -(speed * delta));
        else if (Settings.PositiveXKey.Value.IsPressed())
            MoveLP("x", speed * delta);
        
        if (Settings.NegativeYKey.Value.IsPressed())
            MoveLP("y", -(speed * delta));
        else if (Settings.PositiveYKey.Value.IsPressed())
            MoveLP("y", speed * delta);
        
        if (Settings.NegativeZKey.Value.IsPressed())
            MoveLP("z", -(speed * delta));
        else if (Settings.PositiveZKey.Value.IsPressed())
            MoveLP("z", speed * delta);
        
        if (Settings.CurrentZoneCubePosition.Value != LookPositionGameObject.transform.position)
            Settings.CurrentZoneCubePosition.Value = LookPositionGameObject.transform.position;
    }

    public void HandleRotation(float speed, float delta)
    {
        float rotSpeed = speed * 25;

        if (Settings.PositiveXKey.Value.IsPressed())
            RotateLP("x", -rotSpeed * delta);
        else if (Settings.NegativeXKey.Value.IsPressed())
            RotateLP("x", rotSpeed * delta);

        if (Settings.PositiveYKey.Value.IsPressed())
            RotateLP("y", -rotSpeed * delta);
        else if (Settings.NegativeYKey.Value.IsPressed())
            RotateLP("y", rotSpeed * delta);

        if (Settings.PositiveZKey.Value.IsPressed())
            RotateLP("z", -rotSpeed * delta);
        else if (Settings.NegativeZKey.Value.IsPressed())
            RotateLP("z", rotSpeed * delta);

        if (Settings.CurrentZoneCubeRotation.Value != LookPositionGameObject.transform.rotation)
            LookPositionGameObject.transform.rotation = Settings.CurrentZoneCubeRotation.Value;
    }

    public void HandleScaling(float speed, float delta)
    {
        if (Settings.NegativeXKey.Value.IsPressed())
            ScaleLP("x", -(speed * delta));
        else if (Settings.PositiveXKey.Value.IsPressed())
            ScaleLP("x", speed * delta);

        if (Settings.NegativeYKey.Value.IsPressed())
            ScaleLP("y", -(speed * delta));
        else if (Settings.PositiveYKey.Value.IsPressed())
            ScaleLP("y", speed * delta);

        if (Settings.NegativeZKey.Value.IsPressed())
            ScaleLP("z", -(speed * delta));
        else if (Settings.PositiveZKey.Value.IsPressed())
            ScaleLP("z", speed * delta);

        if (Settings.CurrentZoneCubeScale.Value != LookPositionGameObject.transform.localScale)
            Settings.CurrentZoneCubeScale.Value = LookPositionGameObject.transform.localScale;
    }

    public void MoveLP(string axis, float amount)
    {
        Vector3 translation = new Vector3(0, 0, 0);

        switch (axis)
        {
            case "x": translation = new Vector3(amount, 0, 0); break;
            case "y": translation = new Vector3(0, amount, 0); break;
            case "z": translation = new Vector3(0, 0, amount); break;
        }

        LookPositionGameObject.transform.Translate(translation, Space.Self);
    }
    
    public void ScaleLP(string axis, float amount)
    {
        Vector3 scaleAmount = new Vector3(0, 0, 0);

        switch (axis)
        {
            case "x": scaleAmount = new Vector3(amount, 0, 0); break;
            case "y": scaleAmount = new Vector3(0, amount, 0); break;
            case "z": scaleAmount = new Vector3(0, 0, amount); break;
        }

        Vector3 newScale = LookPositionGameObject.gameObject.transform.localScale + scaleAmount;
        LookPositionGameObject.gameObject.transform.localScale = new Vector3(
            Mathf.Max(newScale.x, 0.01f),
            Mathf.Max(newScale.y, 0.01f),
            Mathf.Max(newScale.z, 0.01f)
        );
    }

    public void RotateLP(string axis, float amount)
    {
        Vector3 rotation = new Vector3(0, 0, 0);

        switch (axis)
        {
            case "x": rotation = new Vector3(0, amount, 0); break;
            case "y": rotation = new Vector3(0, 0, amount); break;
            case "z": rotation = new Vector3(amount, 0, 0); break;
        }

        if (axis == "x")
        {
            Settings.CurrentZoneCubeRotation.Value *= Quaternion.Euler(rotation);
            LookPositionGameObject.transform.localRotation = Settings.CurrentZoneCubeRotation.Value;
        }
        else if (!Settings.LockXAndZRotation.Value)
        {
            Settings.CurrentZoneCubeRotation.Value *= Quaternion.Euler(rotation);
            LookPositionGameObject.transform.localRotation = Settings.CurrentZoneCubeRotation.Value;
        }
    }

    public void SetColor(Color color)
    {
        if (!LookPositionGameObject) return;
        
        LookPositionGameObject.GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b);
        SetTransparentColor(Settings.ZoneCubeTransparency.Value);
    }

    public Color GetColor()
    {
        if (!LookPositionGameObject) return Color.green;
        return LookPositionGameObject.GetComponent<Renderer>().material.color;
    }
    
    public void SetTransparentColor(float transparency)
    {
        if (!LookPositionGameObject) return;
        
        Color color = GetColor();
        LookPositionGameObject.GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, transparency);
    }
    
    public Color GetTransparentColor(Color color, float transparency)
    {
        if (!LookPositionGameObject) return new Color(color.r, color.g, color.b, transparency);
        
        return new Color(color.r, color.g, color.b, transparency);
    }
    
    private bool ShouldSkipObject(string objectName)
    {
        foreach (var prefix in PrefixesToSkip)
        {
            if (objectName.StartsWith(prefix))
                return true;
        }

        return false;
    }
    
    public static void AddCubeData()
    {
        var position = Settings.CurrentZoneCubePosition.Value;
        var rotation = Settings.CurrentZoneCubeRotation.Value.eulerAngles;

        var location = new Settings.Location
        {
            Position = position,
            Rotation = rotation
        };

        Settings.CubeDataList.Add(location);
        NotificationManagerClass.DisplayMessageNotification("[HavenZoneCreator] Zone Cube Location added to the list.");
    }
}