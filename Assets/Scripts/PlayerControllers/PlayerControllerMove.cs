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
    public bool Running = false;
    public float PlayerRightStraffe = 0f;
    private float JumpForce = 10f;
    public bool PlayerIsOnGround = false;

    // Link to player sight
    private PlayerSight PlayerEyes;
    public Rigidbody PlayerBody;
    private CapsuleCollider BodyCollider;

    public Animator PlayerAnimator;

    // Backpack
    Backpack CurrentBackpack;

    private void OnEnable()
    {
        PV = GetComponent<PhotonView>();

        PlayerEyes = transform.Find("PlayerEyes").GetComponent<PlayerSight>();
        PlayerBody = GetComponent<Rigidbody>();
        BodyCollider = GetComponent<CapsuleCollider>();

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

        CheckGrounded();

        // Start at walk speed
        float player_speed = PlayerWalk;

        // Add run if running
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            // If aiming and attempting to run
            // stop aiming and run
            if (PlayerEyes.Aiming)
            {
                PlayerEyes.Aiming = false;
            }

            // Toggle run
            Running = !Running;
        }

        if (Running)
        {
            player_speed += PlayerRunAddition;
            PlayerEyes.Aiming = false;
        }

        // Subtract based on weight of weapon
        if (CurrentBackpack != null)
        {
            if (CurrentBackpack.CurrentWeapon != null)
            {
                // A weight of 100 makes a 8 unit walk decrease
                player_speed -= CurrentBackpack.CurrentWeapon.Weight * 0.08f;
            }
        }

        float translation = Input.GetAxisRaw("Vertical") * player_speed;

        // Cannot run backwards
        if (translation < 0 && Running) player_speed -= PlayerRunAddition;

        // Stop running if player stops
        if (translation < 1) Running = false;

        float straffe = Input.GetAxisRaw("Horizontal") * player_speed;
        translation *= Time.deltaTime;
        straffe *= Time.deltaTime;

        // Update animations
        if (Running)
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

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (PlayerIsOnGround)
            {
                PlayerBody.AddForce(new Vector3(0, JumpForce, 0));
            }

        }

        if (Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
        }

    }

    private void CheckGrounded()
    {
        // Check if player is on the ground
        PlayerIsOnGround = true;
    }
}
