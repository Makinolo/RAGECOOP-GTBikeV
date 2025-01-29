﻿#undef DEBUG
using GTA;
using System;
using System.Windows.Forms;
namespace RageCoop.Client
{
    /// <summary>
    /// Don't use it!
    /// </summary>
    public class Settings
    {
        [NonSerialized]
        private string _username = "Player";
        /// <summary>
        /// Get or set local player's username, set won't be effective if already connected to a server.
        /// </summary>
        public string Username
        {
            get => _username;
            set
            {
                if (Networking.IsOnServer || string.IsNullOrEmpty(value))
                {
                    return;
                }
                _username = value;
            }
        }
        /// <summary>
        /// The password used to authenticate when connecting to a server.
        /// </summary>
        public string Password { get; set; } = "";
        /// <summary>
        /// Don't use it!
        /// </summary>
        public string LastServerAddress { get; set; } = "127.0.0.1:4499";
        /// <summary>
        /// Don't use it!
        /// </summary>
        public string MasterServer { get; set; } = "https://masterserver.ragecoop.com/";
        /// <summary>
        /// Don't use it!
        /// </summary>
        public bool FlipMenu { get; set; } = false;
        /// <summary>
        /// Don't use it!
        /// </summary>
        public bool Voice { get; set; } = false;

        /// <summary>
        /// LogLevel for RageCoop.
        /// 0:Trace, 1:Debug, 2:Info, 3:Warning, 4:Error
        /// </summary>
        public int LogLevel = 00;

        /// <summary>
        /// The key to open menu
        /// </summary>
        public Keys MenuKey { get; set; } = Keys.F9;

        /// <summary>
        /// The key to enter a vehicle as passenger.
        /// </summary>
        public Keys PassengerKey { get; set; } = Keys.G;

        /// <summary>
        /// Disable world NPC traffic, mission entities won't be affected
        /// </summary>
        public bool DisableTraffic { get; set; } = false;

        /// <summary>
        /// Bring up pause menu but don't freeze time when FrontEndPauseAlternate(Esc) is pressed.
        /// </summary>
        public bool DisableAlternatePause { get; set; } = true;

        /// <summary>
        /// The game won't spawn more NPC traffic if the limit is exceeded. -1 for unlimited (not recommended).
        /// </summary>
        public int WorldVehicleSoftLimit { get; set; } = 20;

        /// <summary>
        /// The game won't spawn more NPC traffic if the limit is exceeded. -1 for unlimited (not recommended).
        /// </summary>
        public int WorldPedSoftLimit { get; set; } = 30;

        /// <summary>
        /// The directory where log and resources downloaded from server will be placed.
        /// </summary>
        public string DataDirectory { get; set; } = "Scripts\\RageCoop\\Data";

        /// <summary>
        /// Show the owner name of the entity you're aiming at
        /// </summary>
        public bool ShowEntityOwnerName { get; set; } = false;

        /// <summary>
        ///     Show other player's nametag on your screen
        /// </summary>
        public bool ShowPlayerNameTag { get; set; } = true;

        /// <summary>
        ///     Show other player's blip on map
        /// </summary>
        public bool ShowPlayerBlip { get; set; } = true;

        /// <summary>
        /// Enable automatic respawn for this player.
        /// </summary>
        public bool EnableAutoRespawn { get; set; } = true;

        /// <summary>
        /// Get or set player's blip color
        /// </summary>
        public BlipColor BlipColor { get; set; } = BlipColor.White;

        /// <summary>
        /// Get or set player's blip sprite
        /// </summary>
        public BlipSprite BlipSprite { get; set; } = BlipSprite.Standard;

        /// <summary>
        /// Get or set scale of player's blip
        /// </summary>
        public float BlipScale { get; set; } = 1;

        /// <summary>
        /// In non interactive mode the menus are always hidden and all
        /// the interaction is handled by the API
        /// </summary>
        public bool Interactive = false;
    }
}
