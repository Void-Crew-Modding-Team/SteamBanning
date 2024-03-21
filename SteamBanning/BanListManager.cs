using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using System.Collections.Generic;
using VoidManager.Utilities;

namespace SteamBanning
{
    internal class BanListManager
    {
        public static BanListManager Instance
        {
            get;
            internal set;
        }

        List<CSteamID> BannedSteamIDs;

        public BanListManager()
        {
            LoadBannedSteamIDs();
        }

        internal void OnPlayerJoin(Player JoiningPlayer)
        {
            CSteamID UserSteamID = (CSteamID)ulong.Parse(JoiningPlayer.UserId);
            if (BanListContainsSteamID(UserSteamID))
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    BepinPlugin.Log.LogInfo($"Closing connection from banned user {JoiningPlayer.NickName} Steam ID: {JoiningPlayer.UserId}");
                    PhotonNetwork.CloseConnection(JoiningPlayer);
                }
                else
                {
                    string message = $"Banned user {JoiningPlayer.NickName} has joined the session. SteamID: {JoiningPlayer.UserId}";
                    BepinPlugin.Log.LogInfo(message);
                    Messaging.Echo(message);
                }
            }
        }

        public bool BanListContainsSteamID(CSteamID steamID)
        {
            if(BannedSteamIDs.Contains(steamID))
            {
                return true;
            }
            return false;
        }

        public void RemoveFromBanlist(CSteamID steamID)
        {
            if(BanListContainsSteamID(steamID))
            {
                BannedSteamIDs.Remove(steamID);
                UpdateConfigFile();
            }
        }

        public void AddToBanlist(CSteamID steamID)
        {
            if(!BanListContainsSteamID(steamID))
            {
                BannedSteamIDs.Add(steamID);
                UpdateConfigFile();
            }
        }

        private void LoadBannedSteamIDs()
        {
            BannedSteamIDs= new List<CSteamID>();
            string[] IDList = BepinPlugin.Bindings.BanList.Value.Split(',');
            foreach (string id in IDList)
            {
                if (ulong.TryParse(id, out ulong ulongid))
                {
                    CSteamID SteamID = (CSteamID)ulongid;
                    if(SteamID.IsValid())
                    {
                        BannedSteamIDs.Add(SteamID);
                    }
                }
            }
        }

        private void UpdateConfigFile()
        {
            bool firstLoop = true;
            string IDString = string.Empty;
            foreach(CSteamID id in BannedSteamIDs)
            {
                if (firstLoop)
                {
                    firstLoop = false;
                }
                else
                {
                    IDString += ",";
                }

                IDString += id.ToString();
            }

            BepinPlugin.Bindings.BanList.Value = IDString;
        }
    }
}
