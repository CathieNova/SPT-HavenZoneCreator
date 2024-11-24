using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SPT.Reflection.Patching;

namespace HavenZoneCreator.Patches;

public static class PatchManager
{
    public static void EnablePatches()
    {
        foreach (var patch in GetAllPatches())
        {
            ((ModulePatch)Activator.CreateInstance(patch)).Enable();
        }
    }
    
    private static IEnumerable<Type> GetAllPatches()
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.BaseType == typeof(ModulePatch));
    }
}