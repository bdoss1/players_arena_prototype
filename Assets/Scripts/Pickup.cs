using Photon.Pun;

public class Pickup : PlayersArena
{

    // ID
    public string PickupID;
    public PickupTypes PickupType;

    // Attributes
    public PickupLevel LevelOfPickup;

    public PhotonView PV;
    
    void Start()
    {
        // Create unique ID for pickup
        PickupID = CreateID();

    }

    public PickupTypes GetPickupType()
    {
        return PickupType;
    }

}
