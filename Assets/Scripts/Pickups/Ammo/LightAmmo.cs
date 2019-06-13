using Photon.Pun;

public class LightAmmo : Ammo
{
    
    void Start()
    {
        PV = GetComponent<PhotonView>();

        // Setup pickup type
        PickupType = PickupTypes.Light_Ammo;

        // Set ammo type
        AmmoType = WeaponAmmoTypes.Light;

        // Set pickup ammo bullet amount
        NumberOfBullets = 20;

    }

    [PunRPC]
    void RPC_Destroy()
    {
        Destroy(gameObject);
    }

}
