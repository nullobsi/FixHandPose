using HarmonyLib;
using Renderite.Shared;

namespace FixHandPose.Shared
{
    [HarmonyPatch(typeof(Renderite.Shared.VR_ControllerState))]
    class VR_ControllerState_Patch
    {
        [HarmonyTranspiler]
        [HarmonyPatch("Pack")]
        static IEnumerable<CodeInstruction> Pack_Transpiler(IEnumerable<CodeInstruction> instructions)
        {

        }

        [HarmonyTranspiler]
        [HarmonyPatch("Unpack")]
        static IEnumerable<CodeInstruction> Unpack_Transpiler(IEnumerable<CodeInstruction> instructions)
        {

        }
    }
}

