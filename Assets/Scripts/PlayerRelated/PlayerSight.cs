using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSight : PlayersArena
{
    // Network variables
    PhotonView PV;

    // Camera link
    public Camera PlayerEyes;
    public ArmedObject PlayerArms;
        
    // Max distance player can see when picking
    // up item
    private readonly float PlayerSightDistance = 2.75f;
    
    // Reference to player game object
    private GameObject PlayerObject;

    // Reference to look script
    public PlayerControllerLook LookController;

    // Viewing object
    public Pickup ViewingObject;

    // Link to backpack
    private Backpack CurrentBackpack;

    // Held Weapons
    public GameObject HeldWeapon;
    public bool Aiming;
    private float AimFOV = 60f;
    private float AimZoomVelocity;
    private Vector3 AimingVelocity = Vector3.zero;
    private Dictionary<int, GameObject> HeldWeapons = new Dictionary<int, GameObject>();

    private PlayerControllerMove PlayerMoveController;

    private void OnEnable()
    {
        PV = GetComponent<PhotonView>();
        LookController = GetComponent<PlayerControllerLook>();
        PlayerMoveController = GetComponentInParent<PlayerControllerMove>();
    }

    // Start is called before the first frame update
    void Start()
    {

        // Setup player
        PlayerObject = transform.parent.gameObject;

        // Link camera
        PlayerEyes = GetComponent<Camera>();
        PlayerArms = GetComponent<ArmedObject>();

        // Not a pickup
        IsPickup = false;

        // Link backpack
        CurrentBackpack = GetComponent<ArmedObject>().GetCurrentBackpack();

        // Link backpack to move controller
        gameObject.transform.parent.gameObject.GetComponent<PlayerControllerMove>().LinkBackpack(CurrentBackpack);

        // Link self to backpack
        CurrentBackpack.LinkPlayerSight(this);

        // Link up all weapons that can be held
        LinkWeapons();
        Aiming = false;

    }

    void LinkWeapons()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponents<Weapon>().Length != 0)
            {
                // Cast weapon so we can get its type
                Weapon weapon = child.gameObject.GetComponent<Weapon>();

                // Add weapon to held weapon list
                HeldWeapons.Add((int)weapon.PickupType, child.gameObject);

            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine) return;

        // Watch for weapon switch
        WatchForWeaponSwitch();

        // Watch for aiming
        AimIfAiming();

        // Watch for reload
        WatchForReload();

        // Look for pickups to show panels for
        LookForPickups();

        // Watch for clicking
        WatchForActionKey();

        // Watch for left click
        WatchForWeaponShoot();

    }

    void WatchForReload()
    {
        // Reload on R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            if(HeldWeapon != null)
            {
                HeldWeapon.GetComponent<Weapon>().ReloadWeapon();
            }
        }
    }

    void WatchForWeaponShoot()
    {
        // If no weapon equipped then ignore
        if (HeldWeapon == null) return;

        // Check weapon shot mode
        HeldWeapon.GetComponent<Weapon>().WatchForShoot(this, PlayerArms);
        
    }

    void WatchForWeaponSwitch()
    {
        // Reload on R key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentBackpack.SwitchWeapons(WeaponSlots.Primary);
            ChangeWeapon(WeaponSlots.Primary);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentBackpack.SwitchWeapons(WeaponSlots.Secondary);
            ChangeWeapon(WeaponSlots.Secondary);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrentBackpack.HolsterWeapons();
            ChangeWeapon(WeaponSlots.None);
        }
    }

    void AimIfAiming()
    {
        if (HeldWeapon == null) return;

        // Switch aiming on right click
        if (Input.GetMouseButtonDown(1)) Aiming = !Aiming;

        // Stop running if aiming too
        if (Aiming && PlayerMoveController.Running)
        {
            // Stop running
            PlayerMoveController.Running = false;
        }

        GetComponent<ArmedObject>().Aiming = Aiming;

        // If aiming smooth damp to aim
        Vector3 weapon_position;
        if (!Aiming)
        {
            weapon_position = PlayerEyes.transform.position + transform.forward * 0.4f;
            weapon_position = weapon_position + transform.right * 0.175f;
            weapon_position = weapon_position + transform.up * -0.2f;

            // Reduce zoom
            PlayerEyes.fieldOfView = Mathf.SmoothDamp(PlayerEyes.fieldOfView, AimFOV, ref AimZoomVelocity, HeldWeapon.GetComponent<Weapon>().TimeToAim);
        }
        else
        {
            weapon_position = PlayerEyes.transform.position + transform.forward * 0.4f;
            weapon_position = weapon_position + transform.right * 0f;
            weapon_position = weapon_position + transform.up * -0.0745f;

            // Zoom
            float zoom = 14 * ((int)HeldWeapon.GetComponent<Weapon>().Sight + 1);
            PlayerEyes.fieldOfView = Mathf.SmoothDamp(PlayerEyes.fieldOfView, AimFOV - zoom, ref AimZoomVelocity, HeldWeapon.GetComponent<Weapon>().TimeToAim);

        }

        // Move to position
        HeldWeapon.transform.position = Vector3.SmoothDamp(HeldWeapon.transform.position, weapon_position, ref AimingVelocity, HeldWeapon.GetComponent<Weapon>().TimeToAim);

    }

    public Pickup ChangeWeapon(WeaponSlots slot)
    {
        // Hide old weapon (if needed) and display new weapon
        Pickup held_weapon = null;
        switch (CurrentBackpack.EquippedWeapon)
        {
            case WeaponSlots.Primary:
                held_weapon = CurrentBackpack.PrimaryWeapon;
                break;

            case WeaponSlots.Secondary:
                held_weapon = CurrentBackpack.SecondaryWeapon;
                break;

            case WeaponSlots.None:
                held_weapon = null;
                break;
        }

        // Remove old weapon
        if(HeldWeapon != null)
        {
            if (HeldWeapon.scene.IsValid()) Destroy(HeldWeapon); // TODO - weapon switching without NULL
            HeldWeapon = null;
        }

        if (held_weapon == null)
        {
            return null;
        }

        // Equip new weapon
        Vector3 create_position;
        switch (held_weapon.PickupType)
        {
            case PickupTypes.Pistol_Heartbreaker:

                // Positioning for heartbreaker
                GameObject heartbreaker = (GameObject)Resources.Load("PlayerHeldWeapons/Pistols/Pistol_HeartBreaker");
                create_position = PlayerEyes.transform.position + transform.forward * 0.4f;
                create_position = create_position + transform.right * 0.175f;
                create_position = create_position + transform.up * -0.2f;

                // Create the weapon
                HeldWeapon = Instantiate(heartbreaker, create_position, Quaternion.identity, transform);

                // Link backpack so we have access to ammo
                HeldWeapon.GetComponent<Weapon>().SetBackpack(CurrentBackpack);
                break;

            case PickupTypes.SMG_Hornet:

                // Positioning for heartbreaker
                GameObject hornet = (GameObject)Resources.Load("PlayerHeldWeapons/SMGs/SMG_Hornet");
                create_position = PlayerEyes.transform.position + transform.forward * 0.4f;
                create_position = create_position + transform.right * 0.175f;
                create_position = create_position + transform.up * -0.2f;

                // Create the weapon
                HeldWeapon = Instantiate(hornet, create_position, Quaternion.identity, transform);

                // Link backpack so we have access to ammo
                HeldWeapon.GetComponent<Weapon>().SetBackpack(CurrentBackpack);
                break;
        }

        // Rotate to camera
        if (HeldWeapon != null)
        {
            // Rotate to camera
            Quaternion new_rotation = new Quaternion(0, 0, 0, 0);
            HeldWeapon.transform.localRotation = new_rotation;
        }

        return HeldWeapon.GetComponent<Weapon>();

    }

    void WatchForActionKey()
    {
        // Left click
        if (Input.GetKeyDown(KeyCode.E))
        {

            bool added_to_backpack = false;

            // Pickups (no weapons)
            if(ViewingObject != null)
            {

                // If viewing supply box, open box
                if (ViewingObject.IsSupplyBox)
                {
                    ViewingObject.GetComponent<SupplyBox>().PV.RPC("RPC_Open", RpcTarget.All);
                    ViewingObject = null;
                    return;
                }

                // If viewing weapon
                if (ViewingObject.IsWeapon)
                {
                    // If backpack weapons NOT full
                    if (!CurrentBackpack.HasTwoWeapons())
                    {
                        // Pickup weapon
                        if (CurrentBackpack.AddPickupToBackpack(ViewingObject))
                        {
                            added_to_backpack = true;
                        }
                    }
                    else
                    {
                        // Hold to swap for new gun
                    }

                }
                else
                {
                    // Add any item to backpack
                    if (CurrentBackpack.AddPickupToBackpack(ViewingObject))
                    {
                        added_to_backpack = true;
                    }
                }
            }

            // Remove the pickup on the ground and panel if picked up
            if (added_to_backpack)
            {
                ViewingObject.PV.RPC("RPC_Destroy", RpcTarget.All);
            }
        }
    }

    void LookForPickups(){
        
        // Constant raycasting for pickups
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        GameObject obj;
        if (Physics.Raycast(ray, out hit, PlayerSightDistance)) {

            // Object was hit - this is object
            obj = hit.transform.gameObject;

            if (obj.name.Contains("SupplyBox"))
            {
                ViewingObject = obj.GetComponent<SupplyBox>();
                return;
            }

            // Determine location of panel
            Vector3 pickup_position = obj.transform.position;
            Vector3 dir = pickup_position - PlayerObject.transform.position;
            var pickup_face_direction = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 90, 0);
            Vector3 display_at = new Vector3(pickup_position.x, PlayerObject.transform.position.y, pickup_position.z);

            // If gameobject is a pickup
            if (obj.GetComponentsInParent<Pickup>().Length != 0)
            {
                Pickup pickup = obj.GetComponentInParent<Pickup>();
                ViewingObject = pickup;

                // If not a pickup then don't continue
                if (!pickup.IsPickup) return;

            }
            else
            {
                ViewingObject = null;
            }
        }
        else
        {
            ViewingObject = null;
        }

    }

}
