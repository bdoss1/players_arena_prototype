using System.Collections.Generic;
using UnityEngine;

public class PlayersArena : MonoBehaviour
{
    public static bool BOOTED = false;

    // Game settings
    public static float MouseSensitivity = 3f;
    public static float MouseSmoothing = 0.5f;

    // Game colors/styles
    public static GUIStyle CS_BACKPACK_LIGHT_AMMO_STYLE = new GUIStyle();
    public static Color C_LightBlue = new Color(50, 99, 133);

    // Pickups
    public enum LootLevel { Low, Medium, High };
    private static Dictionary<PickupTypes, string> PickupTypePrefabPath = new Dictionary<PickupTypes, string>();
    public bool IsPickup = false;
    public bool IsSupplyBox = false;
    public enum PickupTypes { Backpack, Light_Ammo, Pistol_Heartbreaker, SMG_Hornet };
    public enum PickupLevel { Level_0, Level_1, Level_2, Level_3, Level_4};

    // Weapons
    public bool IsWeapon = false;
    public enum Weapons { Pistol_HeartBreaker, SMG_Hornet};
    public enum WeaponAmmoTypes { Light, Heavy, Shotgun};
    public enum WeaponFireModes { Single, Burst, Auto};
    public enum WeaponReloadModes { Single, Full };
    public enum WeaponSlots { Primary, Secondary, None };
    public enum WeaponSights { x1, x2, x3, x4, x5, x6 };

    // Weapon positions ----------------------------
    public struct WeaponPosition
    {
        public Vector3 Hip;
        public Vector3 Aim;
    }
    // --------------------------------------------
    
    // Crossarrows
    public enum WeaponCrossarrows { Unarmed};

    void Awake()
    {
        // Only boot game once
        if (BOOTED) return;
        print("Game Started");
        BOOTED = true;
        
        // Light ammo backpack
        CS_BACKPACK_LIGHT_AMMO_STYLE.font = (Font)Resources.Load("Fonts/Lato-Bold");
        CS_BACKPACK_LIGHT_AMMO_STYLE.fontSize = 25;
        CS_BACKPACK_LIGHT_AMMO_STYLE.normal.textColor = new Color(1, 1, 1, 0.55f);

        // Define prefab names
        PickupTypePrefabPath.Add(PickupTypes.Light_Ammo, "PickupPrefabs/Ammo/Pickup_LightAmmo");
        PickupTypePrefabPath.Add(PickupTypes.Pistol_Heartbreaker, "PickupPrefabs/Weapons/Pickup_Heartbreaker");
        PickupTypePrefabPath.Add(PickupTypes.SMG_Hornet, "PickupPrefabs/Weapons/Pickup_SMG_Hornet");

    }

    // --------------------------------------------------
    /*
     *      Define level of loot of all pickups
     *      Function to grab a random pickup with a
     *      certain loot level
     * 
    // --------------------------------------------------*/

    public string GetRandomPickupWithLootLevel(LootLevel loot_level)
    {
        List<PickupTypes> possibles = new List<PickupTypes>();

        // Setup all loot levels for all items
        switch (loot_level)
        {
            case LootLevel.Low:
                possibles.Add(PickupTypes.Light_Ammo);
                possibles.Add(PickupTypes.Pistol_Heartbreaker);
                possibles.Add(PickupTypes.SMG_Hornet);
                break;

            case LootLevel.Medium:

                break;

            case LootLevel.High:

                break;
        }

        // Randomly select a pickup of the loot level
        int rnd_index = Random.Range(0, possibles.Count);
        return PickupTypePrefabPath[possibles[rnd_index]];

    }

    // Public functions
    public string CreateID()
    {
        return System.DateTime.Now.Second.ToString() + RandomLetter() + Random.Range(0, 1000).ToString() + RandomLetter();
    }
    
    private string RandomLetter()
    {
        string s = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char c = s[Random.Range(0, s.Length)];
        return c.ToString();
    }

    public bool IsAnimatorAnimationPlaying(Animator animator, string state_name)
    {
        return (animator.GetCurrentAnimatorStateInfo(0).IsName(state_name) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) ;
    }

}
