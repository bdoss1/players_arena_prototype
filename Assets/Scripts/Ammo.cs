public class Ammo : Pickup
{

    public WeaponAmmoTypes AmmoType;
    public int NumberOfBullets;

    void Start()
    {
        // Set ammo as pickup
        IsPickup = true;
    }

}
