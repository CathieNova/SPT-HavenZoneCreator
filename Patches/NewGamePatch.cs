using System.Reflection;
using Comfort.Common;
using EFT;
using HavenZoneCreator.Features;
using HavenZoneCreator.Utilities;
using SPT.Reflection.Patching;

namespace HavenZoneCreator.Patches
{
    internal class NewGamePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));

        [PatchPrefix]
        private static void PatchPrefix()
        {
            HavenZoneCubeComponent.Enable();
            
            Settings.CurrentMapName.Value = Singleton<GameWorld>.Instance.MainPlayer.Location;
        }
    }
}