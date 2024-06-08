using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoidManager.CustomGUI;
using VoidManager.Utilities;
using static UnityEngine.GUILayout;

namespace SteamBanning
{
    class BanningGUI : ModSettingsMenu
    {
        public override string Name()
        {
            return "Steam Banning";
        }

        UserDisplayObject SelectedUser;

        UserDisplayObject[] UserDisplayObjects = null;

        Vector2 ScrollPosition = Vector2.zero;

        private void UpdateUserDisplayObjectsForBanning()
        {
            List<UserDisplayObject> UDOs;
            Player[] players = PhotonNetwork.PlayerListOthers; //Cache to prevent multiple get operations;


            //First time build
            if (UserDisplayObjects == null)
            {
                UDOs = new List<UserDisplayObject>();

                foreach (Player photonPlayer in players)
                {
                    if (ulong.TryParse(photonPlayer.UserId, out ulong result))
                    {
                        UDOs.Add(new UserDisplayObject((CSteamID)result, photonPlayer));
                    }
                    else
                    {
                        BepinPlugin.Log.LogWarning($"Could not parse steam ID for user: {photonPlayer.NickName}");
                    }
                }
                UserDisplayObjects = UDOs.ToArray();
                return;
            }


            //Load and process previous UDOs for differences.
            UDOs = UserDisplayObjects.ToList();

            //Remove players that left the room from UDOs;
            List<UserDisplayObject> UDOsForRemoval = new List<UserDisplayObject>();
            foreach (UserDisplayObject UDO in UDOs)
            {
                bool found = false;
                foreach (Player photonPlayer in players)
                {
                    if (photonPlayer == UDO.PhotonPlayer)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    UDOsForRemoval.Add(UDO);
                }
            }
            foreach (UserDisplayObject UDO in UDOsForRemoval)
            {
                if (SelectedUser == UDO)
                {
                    SelectedUser = null;
                }
                UDOs.Remove(UDO);
            }

            //Loop for duplicates. If none, add to UDOs
            foreach (Player photonPlayer in players)
            {
                bool found = false;
                foreach (UserDisplayObject UDO in UDOs)
                {
                    if (photonPlayer == UDO.PhotonPlayer)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    if (ulong.TryParse(photonPlayer.UserId, out ulong result))
                    {
                        UDOs.Add(new UserDisplayObject((CSteamID)result, photonPlayer));
                    }
                    else
                    {
                        BepinPlugin.Log.LogWarning($"Could not parse steam ID for user: {photonPlayer.NickName}");
                    }
                }
            }


            UserDisplayObjects = UDOs.ToArray();
        }

        private void UpdateUDOsForUnBanning()
        {
            List<UserDisplayObject> UDOs;


            //First time build
            if (UserDisplayObjects == null)
            {
                UDOs = new List<UserDisplayObject>();

                foreach (var BannedUser in BanListManager.Instance.BannedUsers)
                {
                    UDOs.Add(new UserDisplayObject(BannedUser.Key, BannedUser.Value));
                }
                UserDisplayObjects = UDOs.ToArray();
                return;
            }


            //Load and process previous UDOs for differences.
            UDOs = UserDisplayObjects.ToList();

            //Remove players unbanned user UDOs;
            List<UserDisplayObject> UDOsForRemoval = new List<UserDisplayObject>();
            foreach (UserDisplayObject UDO in UDOs)
            {
                if (!BanListManager.Instance.BanListContainsSteamID(UDO.SteamID))
                {
                    UDOsForRemoval.Add(UDO);
                }
            }
            foreach (UserDisplayObject UDO in UDOsForRemoval)
            {
                if (SelectedUser == UDO)
                {
                    SelectedUser = null;
                }
                UDOs.Remove(UDO);
            }

            UserDisplayObjects = UDOs.ToArray();
        }


        float LastUpdateTime = 0f;




        private void BanningUI()
        {
            //Not in room, do not build Game Display Objects
            if (!PhotonNetwork.InRoom)
            {
                if (UserDisplayObjects != null)
                {
                    UserDisplayObjects = null;
                }
                Label("Not in a room");
                return;
            }

            //Refresh DisplayObjects array every 5 seconds
            if (Time.time > LastUpdateTime + 5f)
            {
                LastUpdateTime = Time.time;
                UpdateUserDisplayObjectsForBanning();
            }

            if (UserDisplayObjects == null)
            {
                Label("No Data");
                return;
            }

            //Button display
            Label("Player List");
            ScrollPosition = BeginScrollView(ScrollPosition);
            foreach (UserDisplayObject UDO in UserDisplayObjects)
            {
                bool onBanList = BanListManager.Instance.BanListContainsSteamID(UDO.SteamID);
                if (onBanList)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;


                if (Button($"{UDO.SteamID} - {UDO.PlayerName}{(onBanList ? " [BANNED]" : string.Empty)}"))
                {
                    SelectedUser = UDO;
                }
            }
            EndScrollView();


            //Ban Button
            Label("Selected user: " + (SelectedUser != null ? SelectedUser.PlayerName : "No User Selected"));
            BeginHorizontal();
            GUI.color = Color.red;
            if (Button($"Ban"))
            {
                if (SelectedUser != null)
                {
                    MenuScreenController.Instance.ShowConfirmationPopup("Ban User: " + SelectedUser.PlayerName, "Are you sure?", new System.Action(BanDelegate));
                }
            }

            GUI.color = Color.white;
            if (Button("Unban"))
            {
                if (SelectedUser != null)
                {
                    BanListManager.Instance.RemoveFromBanlist(SelectedUser.SteamID);
                    BepinPlugin.Log.LogInfo("Unbanned user: " + SelectedUser.PlayerName);
                    Messaging.Echo("Unbanned user: " + SelectedUser.PlayerName, false);
                }
            }
            EndHorizontal();
        }



        private void UnBanningUI()
        {
            //Refresh DisplayObjects array every 5 seconds
            if (Time.time > LastUpdateTime + 5f)
            {
                LastUpdateTime = Time.time;
                UpdateUDOsForUnBanning();
            }

            if (UserDisplayObjects == null)
            {
                Label("No Data");
                return;
            }


            //Button display
            Label("Player List");
            ScrollPosition = BeginScrollView(ScrollPosition);
            foreach (UserDisplayObject UDO in UserDisplayObjects)
            {
                bool onBanList = BanListManager.Instance.BanListContainsSteamID(UDO.SteamID);
                if (onBanList)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;


                if (Button($"{UDO.SteamID} - {UDO.PlayerName}{(onBanList ? " [BANNED]" : " [UNBANNED]")}"))
                {
                    SelectedUser = UDO;
                }
            }
            EndScrollView();



            //Unban Button
            GUI.color = Color.white;
            Label("Selected user: " + (SelectedUser != null ? SelectedUser.PlayerName : "No User Selected"));
            if (Button("Unban"))
            {
                if (SelectedUser != null)
                {
                    BanListManager.Instance.RemoveFromBanlist(SelectedUser.SteamID);
                    BepinPlugin.Log.LogInfo("Unbanned user: " + SelectedUser.PlayerName);
                    if (Game.InGame)
                    {
                        Messaging.Echo("Unbanned user: " + SelectedUser.PlayerName, false);
                    }
                }
            }
        }

        bool BanUIOpen = true;

        public override void Draw()
        {
            if(BanUIOpen)
            {
                BanningUI();
            }
            else
            {
                UnBanningUI();
            }

            if (Button(BanUIOpen ? "View Banlist" : "Open Banning Menu"))
            {
                BanUIOpen = !BanUIOpen;
                SelectedUser = null;
                UserDisplayObjects = null;
            }
        }

        public override void OnOpen()
        {
            ScrollPosition = Vector2.zero;
        }

        void BanDelegate()
        {
            if (SelectedUser == null)
            {
                BepinPlugin.Log.LogWarning("Attepted banning user while selected user was null.");
                return;
            }
            BepinPlugin.Log.LogInfo("Banning user: " + SelectedUser.PlayerName);
            BanListManager.Instance.AddToBanlist(SelectedUser.SteamID, SelectedUser.PlayerName);
            if (PhotonNetwork.IsMasterClient)
            {
                Messaging.Echo("Banned user: " + SelectedUser.PlayerName, false);
                Game.Instance.KickPlayer(Game.Instance.GetPlayerCharacterByActorNumber(SelectedUser.PhotonPlayer.ActorNumber));
            }
            else
            {
                Messaging.Echo("Added user to local ban list: " + SelectedUser.PlayerName);
            }
        }

        class UserDisplayObject
        {
            public UserDisplayObject(CSteamID steamID, Player photonPlayer)
            {
                PlayerName = photonPlayer.NickName;
                SteamID = steamID;
                PhotonPlayer = photonPlayer;
            }

            public UserDisplayObject(CSteamID steamID, string playerName)
            {
                PlayerName = playerName;
                SteamID = steamID;
            }

            public string PlayerName;
            public CSteamID SteamID;
            public Player PhotonPlayer;
        }
    }
}
