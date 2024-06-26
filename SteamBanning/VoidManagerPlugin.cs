﻿using VoidManager;
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

        public override string Author => "Dragon";

        public override string Description => "Provides banning feature to hosts.";
    }
}
