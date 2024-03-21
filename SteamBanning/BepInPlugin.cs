using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace SteamBanning
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Void Crew.exe")]
    [BepInDependency("VoidManager")]
    public class BepinPlugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        private void Awake()
        {
            Log = Logger;
            Bindings.BanList = Config.Bind("General", "BanList", string.Empty, "Local Ban List. Formt: 'steamID:username'. separate multiple instances with ','");
            BanListManager.Instance = new BanListManager();
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        internal class Bindings
        {
            internal static ConfigEntry<string> BanList;
        }
    }
}