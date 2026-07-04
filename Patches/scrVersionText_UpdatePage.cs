using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scrVersionText), nameof(scrVersionText.UpdatePage))]
internal static class scrVersionText_UpdatePage
{
    private static IEnumerable<CodeInstruction> Transpiler()
    {
        return
        [
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(
                OpCodes.Call,
                AccessTools.Method(typeof(scrVersionText_UpdatePage), nameof(UpdatePage))
            ),
            new CodeInstruction(OpCodes.Ret),
        ];
    }

    private static void UpdatePage(scrVersionText __instance)
    {
        __instance.text.text = (__instance.page = (__instance.page + 1) % 3) switch
        {
            1 => __instance.page1,
            2 => AdofaiQolMod.GetVersionText(),
            _ => __instance.page0,
        };
    }
}
