using FrooxEngine;
using HarmonyLib;
using Elements.Core;
using ResoniteModLoader;

namespace FixHandPose;

// More info on creating mods can be found https://github.com/resonite-modding-group/ResoniteModLoader/wiki/Creating-Mods
public class FixHandPose : ResoniteMod {
    internal const string VERSION_CONSTANT = "1.0.0"; // Changing the version here updates it in all locations needed
    public override string Name => "FixHandPose";
    public override string Author => "nullobsi";
    public override string Version => VERSION_CONSTANT;
    public override string Link => "https://github.com/nullobsi/FixHandPose";

    public override void OnEngineInit() {
        Harmony harmony = new("FixHandPose");
        harmony.PatchAll();
    }

    // Example of how a HarmonyPatch can be formatted, Note that the following isn't a real patch and will not compile.
    [HarmonyPatch(typeof(OffsetableTrackedObject), "Initialize")]
    class OffsetableTrackedObject_Initialize_Patch {
        static void Postfix(OffsetableTrackedObject __instance, float3 defaultPositionOffset, floatQ defaultRotationOffset) {
            Msg("Fixing hand pose");
			__instance.UpdateOffset(defaultPositionOffset, defaultRotationOffset);
			// Is this needed or will it compound?
			__instance.BodyNodePositionOffset = defaultPositionOffset;
			__instance.BodyNodeRotationOffset = defaultRotationOffset;
        }
    }
}
