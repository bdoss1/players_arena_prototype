using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyBox : Pickup
{
    // Random items
    private string ItemLeftPrefabName;
    private string ItemRightPrefabName;
    private string ItemFrontPrefabName;
    private string ItemBackPrefabName;

    // Item locations
    public Transform ItemLeft_Pos;
    public Transform ItemRight_Pos;
    public Transform ItemFront_Pos;
    public Transform ItemBack_Pos;

    // Opening variables
    private bool Opening = false;
    private bool Opened = false;

    // Link Top of box so it can be moved up
    public GameObject TopOfSupplyBox;
    public LootLevel LevelOfLoot;

    private GameObject ItemL;
    private GameObject ItemR;
    private GameObject ItemF;
    private GameObject ItemB;
        
    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        IsPickup = false;
        IsWeapon = false;
        IsSupplyBox = true;

    }

    void Start()
    {
        // Create network supply box items
        ItemLeftPrefabName = GetRandomPickupWithLootLevel(LevelOfLoot);
        ItemRightPrefabName = GetRandomPickupWithLootLevel(LevelOfLoot);
        ItemFrontPrefabName = GetRandomPickupWithLootLevel(LevelOfLoot);
        ItemBackPrefabName = GetRandomPickupWithLootLevel(LevelOfLoot);

        // Randomly choose items to put in box
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("RPC_CreateItems", RpcTarget.All, ItemLeftPrefabName, ItemRightPrefabName, ItemFrontPrefabName, ItemBackPrefabName);
        }
    }

    [PunRPC]
    void RPC_CreateItems(string ItemLeftPrefabName, string ItemRightPrefabName, string ItemFrontPrefabName, string ItemBackPrefabName)
    {
        print(ItemLeftPrefabName);
        GameObject objL = (GameObject)Resources.Load(ItemLeftPrefabName);
        ItemL = Instantiate(objL, ItemLeft_Pos.transform.position, Quaternion.identity);

        GameObject objR = (GameObject)Resources.Load(ItemRightPrefabName);
        ItemR = Instantiate(objR, ItemRight_Pos.transform.position, Quaternion.identity);

        Quaternion rotated_sideways = Quaternion.Euler(0, 90, 0);
        GameObject objF = (GameObject)Resources.Load(ItemFrontPrefabName);
        ItemF = Instantiate(objF, ItemFront_Pos.transform.position, rotated_sideways);

        GameObject objB = (GameObject)Resources.Load(ItemBackPrefabName);
        ItemB = Instantiate(objB, ItemBack_Pos.transform.position, rotated_sideways);
    }

    [PunRPC]
    public void RPC_Open()
    {
        Opening = true;
    }

    void Update()
    {
        if (Opening && !Opened)
        {
            float y_pos = Mathf.Lerp(TopOfSupplyBox.transform.position.y, 2.2f, 0.05f);
            if (y_pos == 4.5f) Opened = true;
            Vector3 new_pos = new Vector3(TopOfSupplyBox.transform.position.x, y_pos, TopOfSupplyBox.transform.position.z);
            TopOfSupplyBox.transform.position = new_pos;

            if(ItemL != null)
            {
                float item_y_pos = Mathf.Lerp(ItemL.transform.position.y, 1.2f, 0.05f);
                Vector3 new_item_left_pos = new Vector3(ItemL.transform.position.x, item_y_pos, ItemL.transform.position.z);
                ItemL.transform.position = new_item_left_pos;
            }
            
            if(ItemR != null)
            {
                float item_y_pos = Mathf.Lerp(ItemR.transform.position.y, 1.2f, 0.05f);
                Vector3 new_item_right_pos = new Vector3(ItemR.transform.position.x, item_y_pos, ItemR.transform.position.z);
                ItemR.transform.position = new_item_right_pos;
            }
            
            if(ItemF!= null)
            {
                float item_y_pos = Mathf.Lerp(ItemF.transform.position.y, 1.2f, 0.05f);
                Vector3 new_item_front_pos = new Vector3(ItemF.transform.position.x, item_y_pos, ItemF.transform.position.z);
                ItemF.transform.position = new_item_front_pos;
            }
            
            if(ItemB != null)
            {
                float item_y_pos = Mathf.Lerp(ItemB.transform.position.y, 1.2f, 0.05f);
                Vector3 new_item_back_pos = new Vector3(ItemB.transform.position.x, item_y_pos, ItemB.transform.position.z);
                ItemB.transform.position = new_item_back_pos;
            }

        }
    }

}
