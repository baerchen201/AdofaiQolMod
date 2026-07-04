using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scnCLS), nameof(scnCLS.CreateFloors))]
internal static class scnCLS_CreateFloors
{
    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    )
    {
        return new CodeMatcher(instructions, generator)
            .MatchForward(
                new CodeMatch(
                    OpCodes.Call,
                    AccessTools.Method(typeof(RDFile), nameof(RDFile.Exists))
                )
            )
            .SetInstruction(
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(typeof(scnCLS_CreateFloors), nameof(RDFile_Exists))
                )
            )
            .MatchForward(
                new CodeMatch(
                    OpCodes.Call,
                    AccessTools.Method(
                        typeof(RDFile),
                        nameof(RDFile.WriteAllLines),
                        [typeof(string), typeof(IEnumerable<string>), typeof(Encoding)]
                    )
                )
            )
            .SetInstruction(
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(typeof(scnCLS_CreateFloors), nameof(RDFile_WriteAllLines))
                )
            )
            .InstructionEnumeration();
    }

    private static bool RDFile_Exists(string path)
    {
        return false;
    }

    private static void RDFile_WriteAllLines(
        string path,
        IEnumerable<string> content,
        Encoding encoding
    ) { }
}
