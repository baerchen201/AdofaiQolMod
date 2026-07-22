using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scnCLS), nameof(scnCLS.Start))]
internal static class scnCLS_Start
{
    internal static scnCLS.Category _category;

    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions
    )
    {
        return new CodeMatcher(instructions)
            .MatchForward(
                new CodeMatch(
                    OpCodes.Ldsfld,
                    AccessTools.Field(typeof(GCS), nameof(GCS.customLevelPaths))
                )
            )
            .Insert(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(typeof(scnCLS_Start), nameof(EnterCategory))
                ),
                new CodeInstruction(OpCodes.Ret)
            )
            .InstructionEnumeration();
    }

    private static void EnterCategory(scnCLS __instance)
    {
#if DEBUG
        AdofaiQolMod.Logger.LogDebug(
            $"{nameof(scnCLS_Start)}.{nameof(EnterCategory)}({__instance}) {nameof(_category)}:{_category} {nameof(__instance.category)}:{__instance.category} {nameof(__instance.featuredLevelsSource)}:{__instance.featuredLevelsSource} {nameof(__instance.steamworksAvailable)}:{__instance.steamworksAvailable}"
        );
#endif
        if (_category != default)
            __instance.EnterCategory(_category);
        __instance.initializing = false; // can't forget this or the menu is fucked
    }
}
