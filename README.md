[![](https://img.shields.io/badge/-Void_Crew_Modding_Team-111111?style=just-the-label&logo=github&labelColor=24292f)](https://github.com/Void-Crew-Modding-Team)
![](https://img.shields.io/badge/Game%20Version-0.27.0-111111?style=flat&labelColor=24292f&color=111111)
[![](https://img.shields.io/discord/1180651062550593536.svg?&logo=discord&logoColor=ffffff&style=flat&label=Discord&labelColor=24292f&color=111111)](https://discord.gg/g2u5wpbMGu "Void Crew Modding Discord")

# Steam Banning

Version 1.0.2  
For Game Version 0.27.0  
Developed by Dragon  
Requires:  BepInEx-BepInExPack-5.4.2100, VoidCrewModdingTeam-VoidManager-1.2.0 


---------------------

### 💡 Functions - **Provides banning features**

- Provides UI for banning and unbanning players.
- Kicks banned players as they join while local client is hosting.
- Notifies local client when a banned player has joined the session (while not hosting).

### 🎮 Client Usage

- Use F5 menu to access banning UI and Banlist UI. F5 > Mod Settings > Steam Banning
- If you have a steam ID, users can be added to the banlist config at Void Crew\BepInEx\config. Format: 'steamID:username'. Separate multiple instances with ','

### 👥 Multiplayer Functionality

- ✅ Client
  - As client, user will be notified when a banned user has joined the session. UI is interactable and users can be banned from future sessions.
- ✅ Host
  - Only the host needs this mod.
- ✅ Session
  - Marks the room as Mod_Session when hosting.
  - Enforces ban list while hosting.

---------------------

## 🔧 Install Instructions - **Install following the normal BepInEx procedure.**

Ensure that you have [BepInEx 5](https://thunderstore.io/c/void-crew/p/BepInEx/BepInExPack/) (stable version 5 **MONO**) and [VoidManager](https://thunderstore.io/c/void-crew/p/VoidCrewModdingTeam/VoidManager/) installed.

#### ✔️ Mod installation - **Unzip the contents into the BepInEx plugin directory**

Drag and drop `SteamBanning.dll` into `Void Crew\BepInEx\plugins`
