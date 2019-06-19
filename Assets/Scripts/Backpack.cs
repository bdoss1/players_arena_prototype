using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : Pickup
{
    // Slots for pickups in backpack
    public List<BackpackItem> BackpackSlots = new List<BackpackItem>();

    // Weapon slots
    public Pickup PrimaryWeapon = null;
    public Pickup SecondaryWeapon = null;
    public Weapon CurrentWeapon;
    public WeaponSlots EquippedWeapon = WeaponSlots.None;

    // Armed object
    public ArmedObject PlayerArms;
    public PlayerSight PlayerEyes;

    // Initializes backpack
    void Start()
    {
        
    }

    public void LinkArmedObject(ArmedObject armed_object)
    {
        PlayerArms = armed_object;
    }

    public void LinkPlayerSight(PlayerSight eyes)
    {
        PlayerEyes = eyes;
    }

    // Used for adding pickups to the backpack
    public bool AddPickupToBackpack(Pickup pickup)
    {

        // Determine how many items fit into
        // backpack
        int max_pickups = 6;
        switch (LevelOfPickup)
        {
            case PickupLevel.Level_0:
                max_pickups = 6;
                break;

            case PickupLevel.Level_1:
                max_pickups = 8;
                break;

            case PickupLevel.Level_2:
                max_pickups = 10;
                break;

            case PickupLevel.Level_3:
                max_pickups = 12;
                break;

            case PickupLevel.Level_4:
                max_pickups = 14;
                break;

            default:
                max_pickups = 6;
                break;
        }

        // If ammo then pile up to 60 in one slot
        if (pickup.PickupType == PickupTypes.Light_Ammo)
        {
            // Search for light ammo
            int backpack_index = -1;
            foreach (BackpackItem item in BackpackSlots)
            {
                backpack_index++;
                //------------------------------------------
                //          LIGHT AMMO PILING
                //------------------------------------------
                if (item.Type == (int)PickupTypes.Light_Ammo)
                {
                    // If ammo slot already full
                    if (item.Quantity >= 60) continue;

                    if (((LightAmmo)pickup).NumberOfBullets + item.Quantity <= 60)
                    {
                        // Found a spot for the light ammo
                        // Add ammo from pickup into slot
                        BackpackItem inserting_item = new BackpackItem((int)PickupTypes.Light_Ammo, ((LightAmmo)pickup).NumberOfBullets + item.Quantity);
                        BackpackSlots[backpack_index] = inserting_item;

                        // Break from search and return true since ammo was added
                        return true;

                    }

                    if (((LightAmmo)pickup).NumberOfBullets + item.Quantity > 60)
                    {

                        // Reset pickup to only have left over bullets
                        ((LightAmmo)pickup).NumberOfBullets = Mathf.Abs(item.Quantity - ((LightAmmo)pickup).NumberOfBullets);

                        // Space in ammo slot but will overfill with ammo if adding full amount
                        // so only add up to 60
                        BackpackItem inserting_item = new BackpackItem((int)PickupTypes.Light_Ammo, 60);
                        BackpackSlots[backpack_index] = inserting_item;

                        // Do not continue searching to add bullets
                        // just go to adding a pickup again
                        break;

                    }

                }

            }
        }

        // If backpack has open slot then add
        // pickup
        if (BackpackSlots.Count < max_pickups)
        {
            switch (pickup.PickupType)
            {
                case PickupTypes.Light_Ammo:
                    BackpackItem light_ammo_item = new BackpackItem((int)PickupTypes.Light_Ammo, ((LightAmmo)pickup).NumberOfBullets);
                    BackpackSlots.Add(light_ammo_item);
                    break;

                case PickupTypes.Backpack:
                    // picking up backpack
                    break;

                // Weapons
                case PickupTypes.Pistol_Heartbreaker:
                case PickupTypes.SMG_Hornet:

                    AddOrChangeWeapon(pickup);

                    break;

                default:
                    BackpackItem item = new BackpackItem((int)pickup.PickupType, 1);
                    BackpackSlots.Add(item);
                    break;
            }            

            return true;
        }

        return false;

    }

    // Looks at weapon status
    public bool HasTwoWeapons()
    {
        return PrimaryWeapon != null && SecondaryWeapon != null;
    }

    // Adding or changing weapons
    private void AddOrChangeWeapon(Pickup weapon, bool swap = false)
    {
        if (weapon == null) return;

        if (PrimaryWeapon == null)
        {
            PrimaryWeapon = weapon;
            EquippedWeapon = WeaponSlots.Primary;
            PrimaryWeapon = PlayerEyes.ChangeWeapon();
        }
        else if (SecondaryWeapon == null)
        {
            SecondaryWeapon = weapon;
            EquippedWeapon = WeaponSlots.Secondary;
            SecondaryWeapon = PlayerEyes.ChangeWeapon();

        }
        else if(swap)
        {
            switch (EquippedWeapon)
            {
                case WeaponSlots.Primary:

                    PrimaryWeapon = weapon;
                    PrimaryWeapon = PlayerEyes.ChangeWeapon();

                    break;

                case WeaponSlots.Secondary:

                    SecondaryWeapon = weapon;
                    SecondaryWeapon = PlayerEyes.ChangeWeapon();

                    break;
            }
        }
    }

    // Holster all weapons
    public void HolsterWeapons()
    {
        EquippedWeapon = WeaponSlots.None;
    }

    // Pull ammo out of backpack into weapon
    public void PullAmmo(PickupTypes ammo)
    {
        // Sum up all ammo in backpack
        foreach (BackpackItem item in BackpackSlots)
        {
            if (item.Type == (int)ammo)
            {
                item.Quantity--;

                if(item.Quantity < 1)
                {
                    // If ammo pile is empty - clear
                    // slot in backpack
                    RemovePickupFromBackpack(item);
                }

                break;
            }
        }
    }

    // Get ammo amount
    public int GetAmmoAmount(PickupTypes ammo)
    {
        // Sum up all ammo in backpack
        int sum_of_ammo = 0;
        foreach(BackpackItem item in BackpackSlots)
        {
            if(item.Type == (int)ammo)
            {
                sum_of_ammo += item.Quantity;
            }
        }

        return sum_of_ammo;
    }

    // Switches weapons
    public void SwitchWeapons(WeaponSlots slot = WeaponSlots.None)
    {
        // If switching without toggle
        switch (slot)
        {
            case WeaponSlots.Primary:
                EquippedWeapon = WeaponSlots.Secondary;
                break;

            case WeaponSlots.Secondary:
                EquippedWeapon = WeaponSlots.Primary;
                break;
        }

        // Determine which weapon is equipped
        switch (EquippedWeapon)
        {
            case WeaponSlots.Primary:

                // If secondary weapon exist, switch to it
                if(SecondaryWeapon != null)
                {
                    EquippedWeapon = WeaponSlots.Secondary;
                }

                break;

            case WeaponSlots.Secondary:

                // If primary weapon exist, switch to it
                if (PrimaryWeapon != null)
                {
                    EquippedWeapon = WeaponSlots.Primary;
                }

                break;
        }
    }

    // Used for removing items from the backpack
    public bool RemovePickupFromBackpack(BackpackItem item)
    {
        // If pickup in backpack then remove it
        if (BackpackSlots.Contains(item))
        {
            BackpackSlots.Remove(item);
            return true;
        }

        return false;

    }

    // Get the level of the backpack
    public PickupLevel GetBackpackLevel()
    {
        return LevelOfPickup;
    }

}
