using VoidManager;
using VoidManager.MPModChecks;

namespace SteamBanning
{
    public class VoidManagerPlugin : VoidManager.VoidPlugin
    {
        public VoidManagerPlugin()
        {
            Events.Instance.PlayerEnteredRoom += BanListManager.Instance.OnPlayerJoin;
        }

        public override MultiplayerType MPType => MultiplayerType.Host;

        public override string Author => MyPluginInfo.PLUGIN_AUTHORS;

        public override string Description => MyPluginInfo.PLUGIN_DESCRIPTION;

        public override string ThunderstoreID => MyPluginInfo.PLUGIN_THUNDERSTORE_ID;
    }
}
