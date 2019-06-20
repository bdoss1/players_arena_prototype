using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ArmedObject : PlayersArena
{
    // Network variables
    private PhotonView PV;

    // Crossarrows
    private WeaponCrossarrows Sights;
    private Vector2 CrossarrowSize = new Vector2(5, 5);
    public Texture Crossarrows_Unarmed;
    public bool Aiming = false;

    // Link to player sight
    private PlayerSight PlayerEyes;

    // Link to Backpack
    private Backpack CurrentBackpack;
    private float BackpackSlotSize;
    private bool BackpackOpen;
    private List<Vector2> BackpackSlotXYs = new List<Vector2>();

    // Backpack textures - slots
    public Texture Backpack_Background_Disabled;
    public Texture Backpack_Background_Empty;
    public Texture Backpack_Background_Weapon;

    // Backpack textures - items
    public Texture Backpack_Items_Backpack;
    public Texture Backpack_Items_LightAmmo;

    // WAD Textures
    private Texture WAD_Background;
    private Texture WAD_HeartBreaker_Primary;
    private Texture WAD_HeartBreaker_Secondary;
    private Texture WAD_Hornet_Primary;
    private Texture WAD_Hornet_Secondary;

    // Look controller
    private PlayerControllerLook LookController;

    // Pickup Panels
    public Texture PickupPanel_LightAmmo;
    public Texture PickupPanel_HeartBreaker;
    public Texture PickupPanel_Hornet;

    private void OnEnable()
    {
        PV = GetComponent<PhotonView>();
        BackpackSlotSize = Screen.width * 0.05f;
        PlayerEyes = GetComponent<PlayerSight>();

        // Pickup panels
        PickupPanel_HeartBreaker = (Texture)Resources.Load("PickupPanels/PP_Pistol_HeartBreaker");
        PickupPanel_LightAmmo = (Texture)Resources.Load("PickupPanels/PP_LightAmmo");
        PickupPanel_Hornet = (Texture)Resources.Load("PickupPanels/PP_SMG_Hornet");

    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize sights to unarmed
        Sights = WeaponCrossarrows.Unarmed;

        // Get look controller
        LookController = gameObject.GetComponent<PlayerControllerLook>();
        
        // Start with level 0 backpack
        CurrentBackpack = gameObject.AddComponent<Backpack>();
        CurrentBackpack.LinkArmedObject(this);
        BackpackOpen = false;

        // Texture loads
        WAD_Background = (Texture)Resources.Load("WAD/WAD_Empty");
        WAD_HeartBreaker_Primary = (Texture)Resources.Load("WAD/WAD_HeartBreaker_Primary");
        WAD_HeartBreaker_Secondary = (Texture)Resources.Load("WAD/WAD_HeartBreaker_Secondary");
        WAD_Hornet_Primary = (Texture)Resources.Load("WAD/WAD_Hornet_Primary");
        WAD_Hornet_Secondary = (Texture)Resources.Load("WAD/WAD_Hornet_Secondary");

    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine) return;

        // Watch for opening/closing of backpack
        WatchForOpenCloseBackpack();

    }

    public void WatchForOpenCloseBackpack()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            BackpackOpen = !BackpackOpen;
        }
    }

    public Backpack GetCurrentBackpack()
    {
        return CurrentBackpack;
    }

    void OnGUI()
    {
        // If backpack is not open then
        // show sights
        if (!BackpackOpen)
        {
            switch (Sights)
            {
                case WeaponCrossarrows.Unarmed:
                    if (!Aiming)
                    {
                        GUI.DrawTexture(new Rect(Screen.width / 2 - (CrossarrowSize.x / 2), Screen.height / 2 - (CrossarrowSize.y / 2), CrossarrowSize.x, CrossarrowSize.y), Crossarrows_Unarmed, ScaleMode.ScaleToFit);
                    }
                    break;

                default:
                    if (!Aiming)
                    {
                        GUI.DrawTexture(new Rect(Screen.width / 2 - (CrossarrowSize.x / 2), Screen.height / 2 - (CrossarrowSize.y / 2), CrossarrowSize.x, CrossarrowSize.y), Crossarrows_Unarmed, ScaleMode.ScaleToFit);
                    }
                    break;

            }
        }

        // Disable looking while backpack is open
        LookController.SetEnabled(!BackpackOpen);

        // Handling backpacking
        Backpacking();

        // Weapons and ammo
        WeaponAndAmmoDisplay();

        // Displaying of panels when looking at pickup
        DisplayPickupPanel();

    }

    private void DisplayPickupPanel()
    {
        if (!BackpackOpen)
        {
            if(PlayerEyes.ViewingObject != null)
            {
                // Setup panel size
                float panel_scaler = Screen.width * 0.25f; ;
                Vector2 pickup_panel_size = new Vector2(panel_scaler * 0.6f, panel_scaler * 0.2f);
                Rect draw_rect = new Rect((Screen.width / 2) + (pickup_panel_size.x / 12), (Screen.height / 2) + (pickup_panel_size.y * 0.1f), pickup_panel_size.x, pickup_panel_size.y);

                bool draw_line = false;
                // Draw on screen what to do for pickup or action
                switch (PlayerEyes.ViewingObject.PickupType)
                {
                    case PickupTypes.Light_Ammo:
                        GUI.DrawTexture(draw_rect, PickupPanel_LightAmmo);
                        draw_line = true;
                        break;
                    case PickupTypes.Pistol_Heartbreaker:
                        GUI.DrawTexture(draw_rect, PickupPanel_HeartBreaker);
                        draw_line = true;
                        break;
                    case PickupTypes.SMG_Hornet:
                        GUI.DrawTexture(draw_rect, PickupPanel_Hornet);
                        draw_line = true;
                        break;
                }

                // Draw line to object
                if (draw_line)
                {
                    Vector2 edge_of_panel = new Vector2((Screen.width / 2) + (pickup_panel_size.x / 12), (Screen.height / 2) + (pickup_panel_size.y * 0.1f));
                    Vector2 middle_of_screen = new Vector2(Screen.width / 2, Screen.height / 2);
                    Drawing.DrawLine(edge_of_panel, middle_of_screen, C_LightBlue, 2);
                }
            }
        }
    }

    private void WeaponAndAmmoDisplay()
    {
        float wad_rect_x = Screen.width - (Screen.width * 0.23f);
        float wad_rect_y = Screen.height - (Screen.height * 0.25f);

        // Size ratio = 200 x 140 px
        float wad_rect_width = 400 * 0.75f;
        float wad_rect_height = 280 * 0.75f;

        // Draw background
        GUI.DrawTexture(new Rect(wad_rect_x,wad_rect_y, wad_rect_width, wad_rect_height), WAD_Background, ScaleMode.ScaleToFit);

        // Draw primary weapon
        if (CurrentBackpack.PrimaryWeapon != null)
        {
            switch (CurrentBackpack.PrimaryWeapon.PickupType)
            {
                case PickupTypes.Pistol_Heartbreaker:
                    GUI.DrawTexture(new Rect(wad_rect_x, wad_rect_y, wad_rect_width / 2, wad_rect_height), WAD_HeartBreaker_Primary, ScaleMode.ScaleToFit);
                    break;
                case PickupTypes.SMG_Hornet:
                    GUI.DrawTexture(new Rect(wad_rect_x, wad_rect_y, wad_rect_width / 2, wad_rect_height), WAD_Hornet_Primary, ScaleMode.ScaleToFit);
                    break;
            }
        }

        // Draw secondary weapon
        if (CurrentBackpack.SecondaryWeapon != null)
        {
            switch (CurrentBackpack.SecondaryWeapon.PickupType)
            {
                case PickupTypes.Pistol_Heartbreaker:
                    GUI.DrawTexture(new Rect(wad_rect_x + (wad_rect_width / 2), wad_rect_y, wad_rect_width / 2, wad_rect_height), WAD_HeartBreaker_Secondary, ScaleMode.ScaleToFit);
                    break;
                case PickupTypes.SMG_Hornet:
                    GUI.DrawTexture(new Rect(wad_rect_x, wad_rect_y, wad_rect_width / 2, wad_rect_height), WAD_Hornet_Secondary, ScaleMode.ScaleToFit);
                    break;
            }
        }
    }

    private void Backpacking()
    {
        // Show backpack
        if (BackpackOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Draw slots
            // Get postion to offset slots so they appear in middle of screen
            float slot_start_x = (Screen.width / 2) - (Screen.width * 0.25f);

            // Draw weapon slots
            float weapon_slot_width_multiplier = 5f;
            float weapon_slot_height_multiplier = 2.5f;
            float weapon_slot_y = Screen.height - (2.5f * (weapon_slot_height_multiplier * BackpackSlotSize));
            float backpack_padding = (BackpackSlotSize * 0.05f);
            float full_weapons_slot_width = (2 * (weapon_slot_width_multiplier * BackpackSlotSize)) + backpack_padding;

            GUI.DrawTexture(new Rect(slot_start_x, weapon_slot_y, weapon_slot_width_multiplier * BackpackSlotSize, weapon_slot_height_multiplier * BackpackSlotSize), Backpack_Background_Weapon);
            GUI.DrawTexture(new Rect(slot_start_x + (weapon_slot_width_multiplier * BackpackSlotSize) + (BackpackSlotSize * 0.1f), weapon_slot_y, weapon_slot_width_multiplier * BackpackSlotSize, weapon_slot_height_multiplier * BackpackSlotSize), Backpack_Background_Weapon);

            // Determine where to disable slots
            int disable_after_column = 3;
            switch (CurrentBackpack.GetBackpackLevel())
            {
                case PickupLevel.Level_0:
                    disable_after_column = 3;
                    break;

                case PickupLevel.Level_1:
                    disable_after_column = 4;
                    break;

                case PickupLevel.Level_2:
                    disable_after_column = 5;
                    break;

                case PickupLevel.Level_3:
                    disable_after_column = 6;
                    break;

                case PickupLevel.Level_4:
                    disable_after_column = 7;
                    break;
            }

            // Get XY locations of slot positions and draw backgrounds
            float full_slots_width = 7 * BackpackSlotSize;
            slot_start_x += (full_weapons_slot_width - full_slots_width) / 2;
            for (int i = 0; i <= 6; i++)
            {
                for (int j = 0; j <= 1; j++)
                {
                    float draw_x = (i * BackpackSlotSize) + slot_start_x;
                    float draw_y = (weapon_slot_y + (BackpackSlotSize * weapon_slot_height_multiplier) + backpack_padding) + (j * BackpackSlotSize);

                    BackpackSlotXYs.Add(new Vector2(draw_x, draw_y));

                    Texture slot_type = i > disable_after_column ? Backpack_Background_Disabled : Backpack_Background_Empty;
                    GUI.DrawTexture(new Rect(draw_x + backpack_padding, draw_y + backpack_padding, BackpackSlotSize * 0.9f, BackpackSlotSize * 0.9f), slot_type);
                }
            }

            DrawBackpackItems();

        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void DrawBackpackItems()
    {

        // Populate backpack display with items in backpack
        int item_number = 1;
        int backpack_index = -1;
        foreach (BackpackItem item in CurrentBackpack.BackpackSlots)
        {
            backpack_index++;

            float item_x = BackpackSlotXYs[item_number - 1].x;
            float item_y = BackpackSlotXYs[item_number - 1].y;

            Texture item_texture = Backpack_Background_Empty;

            switch ((PickupTypes)item.Type)
            {
                case PickupTypes.Light_Ammo:
                    item_texture = Backpack_Items_LightAmmo;
                    break;

                case PickupTypes.Backpack:
                    item_texture = Backpack_Items_Backpack;
                    break;

                default:
                    item_number++;
                    continue;
            }

            // Draw item
            GUI.DrawTexture(new Rect(item_x + (BackpackSlotSize * 0.05f), item_y + (BackpackSlotSize * 0.05f), BackpackSlotSize * 0.9f, BackpackSlotSize * 0.9f), item_texture);

            // If item has quantity then show that as well
            switch ((PickupTypes)item.Type)
            {
                case PickupTypes.Light_Ammo:
                    GUI.Label(new Rect(item_x + (BackpackSlotSize * 0.32f), item_y + (BackpackSlotSize * 0.62f), BackpackSlotSize * 0.8f, BackpackSlotSize * 0.8f), CurrentBackpack.BackpackSlots[backpack_index].Quantity.ToString(), CS_BACKPACK_LIGHT_AMMO_STYLE);
                    break;

                case PickupTypes.Backpack:
                    break;

                default:
                    item_number++;
                    continue;
            }

            // Increase items
            item_number++;

        }
    }

}
