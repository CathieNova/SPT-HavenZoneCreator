using BepInEx;
using BepInEx.Logging;
using HavenZoneCreator.Patches;
using HavenZoneCreator.Utilities;

namespace HavenZoneCreator
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.SPT.custom", "3.10.0")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; set; }
        
        public void Awake()
        {
            Logger ??= BepInEx.Logging.Logger.CreateLogSource("HavenZoneCreator");
            
            Settings.Init(Config);
            new NewGamePatch().Enable();
        }
    }
}