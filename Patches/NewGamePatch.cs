using System.Reflection;
using EFT;
using HavenZoneCreator.Features;
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
            FetchLookPositionComponent.Enable();
        }
    }
}