﻿using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using EFT.UI;
using HavenZoneCreator.Utilities;
using UnityEngine;

namespace HavenZoneCreator.Features;

public class FetchLookPositionComponent : MonoBehaviour
{
    private static Player Player;
    private GameObject LookPositionGameObject;
    public EInputMode Mode = EInputMode.Position;

    public enum EInputMode
    {
        Position,
        Scale,
        Rotate
    }

    protected ManualLogSource Logger { get; private set; }

    private FetchLookPositionComponent()
    {
        if (Logger == null)
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(FetchLookPositionComponent));
        }
    }
    
    internal static void Enable()
    {
        if (Singleton<IBotGame>.Instantiated)
        {
            var gameWorld = Singleton<GameWorld>.Instance;
            gameWorld.GetOrAddComponent<FetchLookPositionComponent>();
            Player = gameWorld.MainPlayer;
            Settings.CurrentLookPosition.Value = Vector3.zero;
        }
    }

    private void Start()
    {
        Settings.LookPositionCubeTransparency.SettingChanged += (sender, args) =>
        {
            SetTransparentColor(Settings.LookPositionCubeTransparency.Value);
        };
    }

    private void Update()
    {
        if (!Player) return;

        if (Settings.RemoveLookPosition.Value.IsDown())
        {
            if (LookPositionGameObject)
            {
                Destroy(LookPositionGameObject);
                LookPositionGameObject = null;
                Settings.CurrentLookPosition.Value = Vector3.zero;
            }
        }
        
        if (Settings.IsKeyPressed(Settings.FetchLookPosition.Value))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = new RaycastHit[250];
            int hitcount = Physics.RaycastNonAlloc(ray, hits);

            for (int i = 0; i < hitcount; i++)
            {
                var hit = hits[i];
                
                var currentTransform = hit.collider.transform;
                MeshRenderer meshRenderer = null;

                while (currentTransform != null)
                {
                    meshRenderer = currentTransform.GetComponentInChildren<MeshRenderer>();
                    if (meshRenderer != null)
                        break;

                    currentTransform = currentTransform.parent;
                }
                
                if (meshRenderer)
                {
                    if (!LookPositionGameObject)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.GetComponent<Renderer>().enabled = true;
                        cube.GetComponent<Collider>().enabled = false;
                        cube.transform.position = hit.point;
                        cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        cube.transform.localRotation = Quaternion.identity;
                        cube.name = "Haven Zone Object";

                        // Change Shader to Transparent Support
                        var renderer = cube.GetComponent<Renderer>();
                        var material = new Material(Shader.Find("Standard"));
                        material.SetFloat("_Mode", 3);
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.EnableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.SetFloat("_Glossiness", 0f);
                        material.renderQueue = 3000;
                        renderer.material = material;
                        
                        LookPositionGameObject = cube;
                        SetColor(Color.green);
                        SetTransparentColor(Settings.LookPositionCubeTransparency.Value);
                        break;
                    }
                    else
                    {
                        LookPositionGameObject.transform.position = hit.point;
                        LookPositionGameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        LookPositionGameObject.transform.localRotation = Quaternion.identity;
                        ChangeMode(EInputMode.Position);
                        break;
                    }
                }
            }
        }

        if (LookPositionGameObject == null) return;

        if (Settings.IsKeyPressed(Settings.PositionModeKey.Value))
            ChangeMode(EInputMode.Position);
        if (Settings.IsKeyPressed(Settings.ScaleModeKey.Value))
            ChangeMode(EInputMode.Scale);
        if (Settings.IsKeyPressed(Settings.RotateModeKey.Value))
            ChangeMode(EInputMode.Rotate);
        
        float delta = Time.deltaTime;
        float speed = Settings.TransformSpeed.Value;

        switch (Mode) 
        {
            case EInputMode.Position:
            {
                HandlePosition(speed, delta);
                break;
            }
            case EInputMode.Rotate:
            {
                HandleRotation(speed, delta);
                break;
            }
            case EInputMode.Scale:
            {
                HandleScaling(speed, delta);
                break;
            }
        }
    }

    public void ChangeMode(EInputMode _mode)
    {
        if (Settings.PositionModeKey.Value.IsDown())
        {
            Mode = EInputMode.Position;
            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuInstallModFunc);
            SetColor(Color.green);
            NotificationManagerClass.DisplayMessageNotification("Translation Mode Activated.");
        }

        if (Settings.ScaleModeKey.Value.IsDown())
        {
            Mode = EInputMode.Scale;
            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuInstallModGear);
            SetColor(Color.blue);
            NotificationManagerClass.DisplayMessageNotification("Scaling Mode Activated.");
        }

        if (Settings.RotateModeKey.Value.IsDown())
        {
            Mode = EInputMode.Rotate;
            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuInstallModVital);
            SetColor(Color.red);
            NotificationManagerClass.DisplayMessageNotification("Rotation Mode Activated.");
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
        
        if (Settings.CurrentLookPosition.Value != LookPositionGameObject.transform.position)
            Settings.CurrentLookPosition.Value = LookPositionGameObject.transform.position;
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

        if (Settings.CurrentLookRotation.Value != LookPositionGameObject.transform.rotation)
            Settings.CurrentLookRotation.Value = LookPositionGameObject.transform.rotation;
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

        if (Settings.CurrentLookScale.Value != LookPositionGameObject.transform.localScale)
            Settings.CurrentLookScale.Value = LookPositionGameObject.transform.localScale;
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

        LookPositionGameObject.gameObject.transform.localScale += scaleAmount;
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
            LookPositionGameObject.transform.localRotation *= Quaternion.Euler(rotation);
        }
        else if (!Settings.LockXAndZRotation.Value)
        {
            LookPositionGameObject.transform.localRotation *= Quaternion.Euler(rotation);
        }
    }

    public void SetColor(Color color)
    {
        if (!LookPositionGameObject) return;
        
        LookPositionGameObject.GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b);
        SetTransparentColor(Settings.LookPositionCubeTransparency.Value);
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
}