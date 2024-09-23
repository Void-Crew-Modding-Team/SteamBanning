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

        public override SessionChangedReturn OnSessionChange(SessionChangedInput input)
        {
            //Mod only functions while hosting, but needs to be marked as a mod_session room for guideline compliance.
            return new SessionChangedReturn() { SetMod_Session = true };
        }
    }
}
