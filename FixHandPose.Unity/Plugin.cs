using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
/* BepInEx 6
using BepInEx.Unity.Mono;
*/

namespace FixHandPose.Unity
{

    [BepInPlugin("FixHandPose.Unity", "FixHandPose (for Unity)", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin FixHandPose.Unity is loaded!");

            try
            {
                Harmony harmony = new Harmony("dog.unix.FixHandPose.Unity");
                harmony.PatchAll();

                Logger.LogInfo("FixHandPose: Unity patches applied");
            }
            catch (Exception ex)
            {
                Logger.LogError($"FixHandPose: Unity failed to patch: {ex}");
            }
        }
    }
}
