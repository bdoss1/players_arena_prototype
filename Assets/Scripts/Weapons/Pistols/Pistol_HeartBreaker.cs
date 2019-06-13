/*
 * 
 *  The HeartBreaker - Single shot, 
 * 
 * 
 */

using Photon.Pun;
using UnityEngine;

public class Pistol_HeartBreaker : Weapon
{
    // Speed of bullet
    public static float BulletSpeed = 500f;

    // Creation of HeartBreaker
    void Start()
    {
        PV = GetComponent<PhotonView>();

        // Setup pickup
        IsPickup = true;
        IsWeapon = true;
        PickupType = PickupTypes.Pistol_Heartbreaker;
        LevelOfPickup = PickupLevel.Level_0;

        // Setup weapon
        PickupType = PickupTypes.Pistol_Heartbreaker;

        ClipSize = 6; // No extended mag - clip size
        BulletBodyDamage = 30; // Damage on body @ 1m
        BulletHeadDamage = 50; // Damage on head @ 1m
        BulletLimbDamage = 20; // Damage on limb @ 1m
        BurstAmount = -1; // If firemode is burst, amount of bullets per burst
        RateOfFire = 380; // Rate of fire
        VerticalRecoil = 8; // Amount of upwards recoil
        HorizontalRecoil = 2; // Amount of right-wards recoil
        Weight = 50; // Weight of weapon
        TimeToAim = 0.05f; // Time to pull up weapon and aim
        Sight = WeaponSights.x1; // Start with 1x aim sights
        MaxAccuracyError = 0; // 3 degree variation
        ReloadTime = 20; // Time to reload
        FireMode = WeaponFireModes.Single;
        AmmoType = WeaponAmmoTypes.Light; // Type of ammo

    }

    [PunRPC]
    void RPC_Destroy()
    {
        Destroy(gameObject);
    }
}
