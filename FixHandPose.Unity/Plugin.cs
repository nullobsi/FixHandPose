using BepInEx;
using BepInEx.Logging;
/* BepInEx 6
using BepInEx.Unity.Mono;
*/

namespace FixHandPose.Unity;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        try
        {
            Harmony harmony = new Harmony("dog.unix.FixHandPose.Unity");
            harmony.PatchAll();

            Log.LogInfo("FixHandPose: Unity patches applied");
        }
        catch (Exception ex)
        {
            Log.LogError($"FixHandPose: Unity failed to patch: {ex}");
        }
    }
}
