using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Steamworks;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scnCLS), nameof(scnCLS.EnterLevel))]
internal static class scnCLS_EnterLevel
{
    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions
    )
    {
        return new CodeMatcher(instructions)
            .MatchForward(
                new CodeMatch(
                    OpCodes.Call,
                    AccessTools.Method(
                        typeof(Persistence),
                        nameof(Persistence.IncrementCLSTotalPlays)
                    )
                )
            )
            .Insert(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(typeof(scnCLS_EnterLevel), nameof(StartPlaytimeTracking))
                )
            )
            .InstructionEnumeration();
    }

    private static void StartPlaytimeTracking(scnCLS __instance)
    {
        if (!__instance.steamworksAvailable || !__instance.currentLevelIsWorkshop)
        {
#if DEBUG
            AdofaiQolMod.Logger.LogDebug(
                $"Skipped playtime tracking {nameof(__instance.steamworksAvailable)}:{__instance.steamworksAvailable}"
            );
#endif
            return;
        }

        try
        {
            SteamUGC.Internal.StartPlaytimeTracking([ulong.Parse(__instance.levelToSelect)], 1);
#if DEBUG
            AdofaiQolMod.Logger.LogDebug(
                $"Started playtime tracking for level {__instance.levelToSelect}"
            );
#endif
        }
        catch (Exception e)
        {
            AdofaiQolMod.Logger.LogError(
                $"Failed to start playtime tracking for level {__instance.levelToSelect}: {e}"
            );
        }
    }
}
