using GTA;
using GTA.Math;
using Newtonsoft.Json.Linq;
using RageCoop.Core;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using static RageCoop.Core.Packets;

namespace RageCoop.Client
{
    /// <summary>
    /// ?
    /// </summary>
    public partial class SyncedPed : SyncedEntity
    {
        private bool _isRelevant = false;
        internal bool IsRelevant 
        { 
            get
            {
                return _isRelevant; 
            }
            set
            {
                _isRelevant = value;
                if(CurrentVehicle != null) {
                    Main.Logger.Debug($"Setting {DisplayName} vehicle {CurrentVehicle.ID} as Relevant");
                    CurrentVehicle.IsRelevant = value;
                }
            }
        }

        internal string DisplayName = "";
        internal Color Color = Color.White;
        internal Blip PedBlip = null;
        internal BlipColor BlipColor = (BlipColor)255;
        internal BlipSprite BlipSprite = 0;
        internal float BlipScale = 1;

        private int _vehicleID = 0; 
        internal int VehicleID
        {
            get => _vehicleID;
            set
            {
                if (value != 0 && (CurrentVehicle == null || value != CurrentVehicle?.ID))
                {
                    _vehicleID = value;
                    CurrentVehicle = EntityPool.GetVehicleByID(_vehicleID);
                    if (CurrentVehicle == null)
                    {
                        Main.Logger.Error($"VehicleID set => Setting current vehicle of Ped {this.DisplayName} to a vehicle {value} that is not found");
                    }
                }
            }
        }

        internal SyncedVehicle CurrentVehicle { get; private set; } = null;
       
        internal VehicleSeat Seat {get; set;}
        public bool IsPlayer { get => OwnerID == ID && ID != 0; }
        
        private Ped _mainPed = null;
        public Ped MainPed 
        { 
            get { return _mainPed; }

            internal set
            { 
                _mainPed = value;
                if (_mainPed != null && _mainPed.IsInVehicle())
                {
                    CurrentVehicle = new SyncedVehicle(_mainPed.CurrentVehicle);
                    EntityPool.Add(CurrentVehicle);
                }
            } 
        }
        internal int Health { get; set; }

        internal Vector3 HeadPosition { get; set; }
        internal Vector3 RightFootPosition { get; set; }
        internal Vector3 LeftFootPosition { get; set; }

        internal byte WeaponTint { get; set; }
        private bool _lastRagdoll = false;
        private ulong _lastRagdollTime = 0;
        private bool _lastInCover = false;
        private byte[] _lastClothes = null;
        internal byte[] Clothes { get; set; }

        internal float Heading { get; set; }

        internal ulong LastSpeakingTime { get; set; } = 0;
        internal bool IsSpeaking { get; set; } = false;
        internal PedMovingType MovingType { get; set; }
        private bool _lastIsJumping = false;
        internal PedDataFlags Flags;

        internal bool IsAiming => Flags.HasPedFlag(PedDataFlags.IsAiming);
        internal bool _lastDriveBy;
        internal bool IsReloading => Flags.HasPedFlag(PedDataFlags.IsReloading);
        internal bool IsJumping => Flags.HasPedFlag(PedDataFlags.IsJumping);
        internal bool IsRagdoll => Flags.HasPedFlag(PedDataFlags.IsRagdoll);
        internal bool IsOnFire => Flags.HasPedFlag(PedDataFlags.IsOnFire);
        internal bool IsInParachuteFreeFall => Flags.HasPedFlag(PedDataFlags.IsInParachuteFreeFall);
        internal bool IsParachuteOpen => Flags.HasPedFlag(PedDataFlags.IsParachuteOpen);
        internal bool IsOnLadder => Flags.HasPedFlag(PedDataFlags.IsOnLadder);
        internal bool IsVaulting => Flags.HasPedFlag(PedDataFlags.IsVaulting);
        internal bool IsInCover => Flags.HasPedFlag(PedDataFlags.IsInCover);
        internal bool IsInLowCover => Flags.HasPedFlag(PedDataFlags.IsInLowCover);
        internal bool IsInCoverFacingLeft => Flags.HasPedFlag(PedDataFlags.IsInCoverFacingLeft);
        internal bool IsBlindFiring => Flags.HasPedFlag(PedDataFlags.IsBlindFiring);
        internal bool IsInStealthMode => Flags.HasPedFlag(PedDataFlags.IsInStealthMode);
        internal Prop ParachuteProp { get; set; } = null;
        internal uint CurrentWeaponHash { get; set; }
        private Dictionary<uint, bool> _lastWeaponComponents = null;
        internal Dictionary<uint, bool> WeaponComponents { get; set; } = null;
        private Entity _weaponObj;
        internal Vector3 AimCoords { get; set; }


        private readonly string[] _currentAnimation = new string[2] { "", "" };

        private bool LastMoving;

    }
}
