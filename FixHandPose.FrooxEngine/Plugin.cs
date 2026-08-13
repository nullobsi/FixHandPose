using BepInEx;
using BepInEx.Logging;
using BepInEx.NET.Common;
using BepInExResoniteShim;
using BepisResoniteWrapper;
using HarmonyLib;
using FrooxEngine;
using Elements.Core;

namespace FixHandPose.FrooxEngine;

[ResonitePlugin(PluginMetadata.GUID, PluginMetadata.NAME, PluginMetadata.VERSION, PluginMetadata.AUTHORS, PluginMetadata.REPOSITORY_URL)]
[BepInDependency(BepInExResoniteShim.PluginMetadata.GUID, BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log = null!;

    public override void Load()
    {
        Log = base.Log;
        ResoniteHooks.OnEngineReady += OnEngineReady;
        Log.LogInfo($"Plugin {PluginMetadata.GUID} is loaded!");
    }

    private void OnEngineReady()
    {
        try
        {
            Harmony harmony = new Harmony("dog.unix.FixHandPose.FrooxEngine");
            harmony.PatchAll();

            Log.LogInfo("FixHandPose: FrooxEngine patches applied");
        }
        catch (Exception ex)
        {
            Log.LogError($"FixHandPose: FrooxEngine failed to patch: {ex}");
        }
    }

    [HarmonyPatch(typeof(OffsetableTrackedObject), nameof(OffsetableTrackedObject.Initialize)]
    class OffsetableTrackedObject_Initialize_Patch
    {
        // It seems this doesn't work because VR_ControllerState.Pack
        // only packs handPosition and handRotation when this.isTracking
        // is true, which it does not default to when it is
        // initialized...?
        static void Postfix(OffsetableTrackedObject __instance, string uniqueIdentifier, float3 defaultPositionOffset, floatQ defaultRotationOffset)
        {
            Log.LogInfo($"Fixing hand pose for {uniqueIdentifier}");
            __instance.UpdateOffset(defaultPositionOffset, defaultRotationOffset);
            // Is this needed or will it compound?
            __instance.BodyNodePositionOffset = defaultPositionOffset;
            __instance.BodyNodeRotationOffset = defaultRotationOffset;
            //Log.LogInfo("pos, rot, bnpo, bnro:", defaultRotationOffset.ToString(), defaultRotationOffset.ToString(), __instance.BodyNodePositionOffset.ToString(), __instance.BodyNodeRotationOffset.ToString());
        }
    }
}
