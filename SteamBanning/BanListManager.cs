using BepInEx;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using System.Collections.Generic;
using VoidManager;
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

        //List<CSteamID> BannedSteamIDs;
        internal Dictionary<CSteamID, string> BannedUsers;

        public BanListManager()
        {
            LoadBannedSteamIDs();
        }

        internal void OnPlayerJoin(object source,  Events.PlayerEventArgs PlayerEventArg)
        {
            Player JoiningPlayer = PlayerEventArg.player;
            CSteamID UserSteamID = (CSteamID)ulong.Parse(JoiningPlayer.UserId);
            if (BanListContainsSteamID(UserSteamID))
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    BepinPlugin.Log.LogInfo($"Closing connection from banned user {JoiningPlayer.NickName} Steam ID: {JoiningPlayer.UserId}");
                    PhotonNetwork.CloseConnection(JoiningPlayer); //Vanilla 0.25.1 cannot handle getting kicked this early, and client will get stuck in a ghost room. VoidManager handles it, so most modded players are fine.
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
            if(BannedUsers.ContainsKey(steamID))
            {
                return true;
            }
            return false;
        }

        public void RemoveFromBanlist(CSteamID steamID)
        {
            if(BanListContainsSteamID(steamID))
            {
                BannedUsers.Remove(steamID);
                UpdateConfigFile();
            }
        }

        public void AddToBanlist(CSteamID steamID, string UserName)
        {
            if(!BanListContainsSteamID(steamID))
            {
                BannedUsers.Add(steamID, UserName);
                UpdateConfigFile();
            }
        }

        private void LoadBannedSteamIDs()
        {
            BannedUsers = new Dictionary<CSteamID, string>();
            string[] UserList = BepinPlugin.Bindings.BanList.Value.Split(',');
            if (UserList[0].IsNullOrWhiteSpace())
            {
                return;
            }
            foreach (string id in UserList)
            {
                string[] User = id.Split(new char[] { ':' }, 2); //SteamID::UserName
                if(User.Length != 2)
                {
                    BepinPlugin.Log.LogError(User.Length + " Bad user data: " + id );
                    continue;
                }
                if (ulong.TryParse(User[0], out ulong ulongid))
                {
                    CSteamID SteamID = (CSteamID)ulongid;
                    if(SteamID.IsValid())
                    {
                        BannedUsers.Add(SteamID, User[1]);
                    }
                }
                else
                {
                    BepinPlugin.Log.LogError("Bad SteamID: " + id);
                }
            }
        }

        private void UpdateConfigFile()
        {
            bool firstLoop = true;
            string userListString = string.Empty;
            foreach(var UserData in BannedUsers)
            {
                if (firstLoop)
                {
                    firstLoop = false;
                }
                else
                {
                    userListString += ",";
                }

                userListString += $"{UserData.Key}:{UserData.Value}";
            }

            BepinPlugin.Bindings.BanList.Value = userListString;
        }
    }
}
