using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerMove : PlayersArena
{
    // Network variables
    PhotonView PV;

    // Walk speeds
    private float PlayerWalk = 6f;
    private float PlayerRunAddition = 10f;

    public Animator PlayerAnimator;

    // Backpack
    Backpack CurrentBackpack;

    private void OnEnable()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Setup animations - state at unarmed_idle
        PlayerAnimator = GetComponent<Animator>();
        PlayerAnimator.SetBool("Pistol", false);
        PlayerAnimator.SetBool("Rifle", false);
        PlayerAnimator.SetBool("Walking", false);
        PlayerAnimator.SetBool("Running", false);
        PlayerAnimator.SetBool("Forward", true);
    }

    // Link to backpack
    public void LinkBackpack(Backpack backpack)
    {
        CurrentBackpack = backpack;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine) return;

        // Start at walk speed
        float player_speed = PlayerWalk;

        // Add run if running
        bool running = false;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            player_speed += PlayerRunAddition;
            running = true;
        }

        // Subtract based on weight of weapon
        if(CurrentBackpack != null)
        {
            if (CurrentBackpack.CurrentWeapon != null)
            {
                // A weight of 100 makes a 8 unit walk decrease
                player_speed -= CurrentBackpack.CurrentWeapon.Weight * 0.08f;
            }
        }

        float translation = Input.GetAxisRaw("Vertical") * player_speed;

        // Cannot run backwards
        if (translation < 0 && running) player_speed -= PlayerRunAddition;

        float straffe = Input.GetAxisRaw("Horizontal") * player_speed;
        translation *= Time.deltaTime;
        straffe *= Time.deltaTime;

        // Update animations
        if (running)
        {
            if (translation > 0)
            {
                PlayerAnimator.SetBool("Walking", false);
                PlayerAnimator.SetBool("Running", true);
                PlayerAnimator.SetBool("Forward", true);
            }
            else if (translation < 0)
            {
                PlayerAnimator.SetBool("Walking", true);
                PlayerAnimator.SetBool("Running", false);
                PlayerAnimator.SetBool("Forward", false);
            }
            else
            {
                PlayerAnimator.SetBool("Walking", false);
                PlayerAnimator.SetBool("Running", false);
            }
        }
        else
        {
            if (translation > 0)
            {
                PlayerAnimator.SetBool("Walking", true);
                PlayerAnimator.SetBool("Running", false);
                PlayerAnimator.SetBool("Forward", true);
            }
            else if (translation < 0)
            {
                PlayerAnimator.SetBool("Walking", true);
                PlayerAnimator.SetBool("Running", false);
                PlayerAnimator.SetBool("Forward", false);
            }
            else
            {
                PlayerAnimator.SetBool("Walking", false);
                PlayerAnimator.SetBool("Running", false);
            }
        }

        // Move the player
        transform.Translate(straffe, 0, translation);

        if (Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
        }

    }
}
