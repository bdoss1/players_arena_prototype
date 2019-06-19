/*
 * 
 *  Class info for all weapons
 *  
 * 
 */
using Photon.Pun;
using UnityEngine;
public class Weapon : Pickup
{

    Animator AnimManager;

    // Attriubutes
    public bool Enabled = true; // Start enabled
    public int ClipSize; // No extended mag - clip size
    public int BulletsInClip; // Bullets in weapon
    public int BulletBodyDamage; // Damage on body @ 1m
    public int BulletHeadDamage; // Damage on head @ 1m
    public int BulletLimbDamage; // Damage on limb @ 1m
    public int BurstAmount; // If firemode is burst, amount of bullets per burst
    public float RateOfFire; // Speed of fire
    public float VerticalRecoil; // Amount of upwards recoil
    public float HorizontalRecoil; // Amount of right-wards recoil
    public float RecoilTime = 10; // Time of recoil
    public float Weight; // Weight of weapon --> A weight of 100 makes a 4 unit walk decrease
    public float TimeToAim; // Time to pull up weapon and aim
    public float ReloadTime; // Time to reload per bullet
    public float MaxAccuracyError; // --- Max X,Y offset in DEGREES of bullet creation
    public WeaponSights Sight;
    public WeaponFireModes FireMode; // Mode of fire
    public WeaponAmmoTypes AmmoType; // Type of ammo
    public WeaponReloadModes ReloadMode; // Reload mode

    // Backpack link
    public Backpack CurrentBackpack;

    // Link to bullet spawn object
    public GameObject BulletSpawnPosition;

    // Shooting variables
    private bool ReadyToShoot = true;
    private float ResetShootTimer = -1f;
    private int BurstShotAmount;

    // Link to backpack
    public void SetBackpack(Backpack backpack)
    {
        CurrentBackpack = backpack;
    }

    // Reload (returns bullets that were added, this is
    // used for calculating time it took to load)
    // --------------------
    // -1 NO reload needed
    // -2 NO ammo
    public int ReloadWeapon()
    {
        int bullets_loaded = 0;
        bool filled_clip = false;

        if(BulletsInClip < ClipSize)
        {
            // If room in clip for more bullets
            int ammo_in_backpack = CurrentBackpack.GetAmmoAmount(PickupTypes.Light_Ammo);

            //If ammo in backpack
            if (ammo_in_backpack > 0)
            {
                // Load ammo into weapon
                while(BulletsInClip < ClipSize && CurrentBackpack.GetAmmoAmount(PickupTypes.Light_Ammo) > 0)
                {
                    // Put bullet in clip
                    BulletsInClip++;

                    // Pull ammo from backpack
                    CurrentBackpack.PullAmmo(PickupTypes.Light_Ammo);

                    // Keep track of how many bullets were loaded
                    bullets_loaded++;

                    // Check if clip is full
                    filled_clip = BulletsInClip == ClipSize;

                }
            }
            else
            {
                return -2; // No ammo
            }
        }
        else
        {
            return -1; // No reloading needing
        }

        if(ReloadMode == WeaponReloadModes.Single || !filled_clip)
        {
            return bullets_loaded;
        }

        return ClipSize;
    }

    // Shoot weapon
    public void WatchForShoot(PlayerSight eyes, ArmedObject arms)
    {
        // Constantly keep shoot timer updated
        if (!ReadyToShoot)
        {
            ResetShootTimer -= (RateOfFire / 50);

            // Watch for reset of ready to shoot
            if (ResetShootTimer < 0)
            {
                ReadyToShoot = true;
            }
            else
            {
                return;
            }
        }

        // If shooting burst
        if (FireMode == WeaponFireModes.Burst && BurstShotAmount > 0)
        {
            // Shoot bullet and decrease bullets in clip
            // Reset shoot timer to wait for next shot
            if (ReadyToShoot)
            {
                Shoot(eyes, arms);
                ResetShootTimer = RateOfFire;
                ReadyToShoot = false;
            }
            
            return;
        }

        switch (FireMode)
        {
            case WeaponFireModes.Auto:

                if (Input.GetMouseButton(0) && ReadyToShoot)
                {
                    // Check ammo in clip
                    if(BulletsInClip > 0)
                    {
                        // Shoot bullet and decrease bullets in clip
                        // Reset shoot timer to wait for next shot
                        Shoot(eyes, arms);
                        ResetShootTimer = (500 / RateOfFire) * 50;
                        ReadyToShoot = false;
                    }
                }

                break;

            case WeaponFireModes.Single:

                if (Input.GetMouseButtonDown(0) && ReadyToShoot)
                {
                    // Check ammo in clip
                    if (BulletsInClip > 0)
                    {
                        // Shoot bullet and decrease bullets in clip
                        // Reset shoot timer to wait for next shot
                        Shoot(eyes, arms);
                        ResetShootTimer = (500 / RateOfFire) * 50;
                        ReadyToShoot = false;
                    }
                }

                break;

            case WeaponFireModes.Burst:

                if (Input.GetMouseButtonDown(0) && ReadyToShoot)
                {
                    // Check ammo in clip
                    if (BulletsInClip > 0)
                    {
                        // Get number of bullets to shoot
                        int bullets_to_shoot = BurstAmount;
                        // If clip has less than brust amount
                        if(BulletsInClip < BurstAmount)
                        {
                            bullets_to_shoot = BulletsInClip;
                        }

                        // Start burst
                        Shoot(eyes, arms);
                        BurstShotAmount = bullets_to_shoot;

                    }
                }

                break;
        }
    }

    void Awake()
    {
        AnimManager = GetComponent<Animator>();
    }

    public void HolsterWeapon()
    {
        Enabled = false;
        UpdateDisplay();
    }

    public void PullUpWeapon()
    {
        Enabled = true;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        foreach (Transform child in transform)
        {
            if (gameObject.GetComponent<Renderer>() != null) gameObject.GetComponent<Renderer>().enabled = Enabled;
        }
    }

    // Shoot bullet
    public void Shoot(PlayerSight eyes, ArmedObject arms)
    {
        if (!Enabled) return;

        // Create bullet at bullet start position
        BulletsInClip -= 1;

        // If burst shooting, decrease burst amount
        if (BurstShotAmount > 0) BurstShotAmount -= 1;

        // Get position of weapon's spawn position
        GameObject spawn_object = transform.Find("BulletSpawnPosition").gameObject;

        // Start shoot animation
        AnimManager.SetBool("Shooting", true);

        // Randomly determine amount of accuracy error
        float error = MaxAccuracyError * (arms.Aiming ? 1 : 1.4f); // add more if not aiming
        float rnd_x = Random.Range(-error, error);
        float rnd_y = Random.Range(-error, error);

        // Alter shoot angle by accuracy error
        float new_x = transform.rotation.eulerAngles.x + rnd_x;
        float new_y = transform.rotation.eulerAngles.y + rnd_y;
        Quaternion shoot_angle = Quaternion.Euler(new_x, new_y, transform.rotation.eulerAngles.z);

        // Check in front of player for objects (use for close range shots not being centered)
        RaycastHit raycast_shot_hit;
        Ray ray = eyes.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Physics.Raycast(ray, out raycast_shot_hit, Mathf.Infinity);

        // Create bullet on network at spawn point and shoot using Bullet class
        GameObject shot_bullet = PhotonNetwork.Instantiate("Bullet", spawn_object.transform.position, shoot_angle);

        // Get weapon type of held weapon
        Weapons weapon_type = Weapons.Pistol_HeartBreaker;
        switch (eyes.HeldWeapon.GetComponent<Weapon>().PickupType)
        {
            case PickupTypes.Pistol_Heartbreaker:
                weapon_type = Weapons.Pistol_HeartBreaker;
                break;

            case PickupTypes.SMG_Hornet:
                weapon_type = Weapons.SMG_Hornet;
                break;
        }

        // Set bullet speed from held weapon variable
        shot_bullet.GetComponent<Bullet>().Shoot(weapon_type, spawn_object.transform, raycast_shot_hit, arms);

        // Apply recoil (swap x and y - since axis on camera are different)
        eyes.LookController.AimingRecoilTotals.x = eyes.HeldWeapon.GetComponent<Weapon>().HorizontalRecoil * 0.05f;
        eyes.LookController.AimingRecoilTotals.y = eyes.HeldWeapon.GetComponent<Weapon>().VerticalRecoil * 0.05f;
        eyes.LookController.RecoilTimer = eyes.HeldWeapon.GetComponent<Weapon>().RecoilTime / 100;

    }

    void Update()
    {
        // Reset animations
        if(AnimManager != null)
        {

            // Reset shoot animation if finished
            if (IsAnimatorAnimationPlaying(AnimManager, "Shoot"))
            {
                AnimManager.SetBool("Shooting", false);
            }

        }
    }

}
