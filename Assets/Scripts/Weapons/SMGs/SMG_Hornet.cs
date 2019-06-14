using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMG_Hornet : Weapon
{
    // Speed of bullet
    public static float BulletSpeed = 500;

    // Creation of HeartBreaker
    void Start()
    {
        PV = GetComponent<PhotonView>();

        // Setup pickup
        IsPickup = true;
        IsWeapon = true;
        PickupType = PickupTypes.SMG_Hornet;
        LevelOfPickup = PickupLevel.Level_0;

        // Setup weapon
        PickupType = PickupTypes.SMG_Hornet;

        ClipSize = 40; // No extended mag - clip size
        BulletBodyDamage = 10; // Damage on body @ 1m
        BulletHeadDamage = 15; // Damage on head @ 1m
        BulletLimbDamage = 5; // Damage on limb @ 1m
        BurstAmount = -1; // If firemode is burst, amount of bullets per burst
        RateOfFire = 650; // Rate of fire
        VerticalRecoil = 3; // Amount of upwards recoil
        HorizontalRecoil = 1.24f; // Amount of right-wards recoil
        Weight = 80; // Weight of weapon
        TimeToAim = 0.07f; // Time to pull up weapon and aim
        Sight = WeaponSights.x1; // Start with 1x aim sights
        MaxAccuracyError = 0; // 3 degree variation
        ReloadTime = 30; // Time to reload
        FireMode = WeaponFireModes.Auto;
        AmmoType = WeaponAmmoTypes.Light; // Type of ammo

    }

    [PunRPC]
    void RPC_Destroy()
    {
        Destroy(gameObject);
    }
}
