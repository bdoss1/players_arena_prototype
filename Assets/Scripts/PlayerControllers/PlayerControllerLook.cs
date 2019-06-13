using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerLook : PlayersArena
{
    // Network variables
    PhotonView PV;

    // Allows for recoil from weapons
    public Vector2 AimingRecoilTotals = new Vector2(0, 0);
    public float RecoilTimer = -1;

    Vector2 MouseLook;
    Vector2 SmoothV;

    GameObject player;

    // Variable for enabling and disabling look
    private bool EnableLook;

    private void OnEnable()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        EnableLook = true;
        player = transform.parent.gameObject;

        // Hide cursor and lock
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        // If not local object - disable input
        if (!PV.IsMine) return;

        // Recoil for some time then stop
        if (RecoilTimer > 0)
        {
            RecoilTimer -= 1f * Time.deltaTime;
        }
        else
        {
            // Reset recoil variables
            if (AimingRecoilTotals.x != 0 || AimingRecoilTotals.y != 0) AimingRecoilTotals = new Vector2(0, 0);
        }
        
        if (!EnableLook) return;

        // Determine values to match camera to mouse
        Vector2 mouse_delta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        
        // Smooth and apply recoils
        mouse_delta = Vector2.Scale(mouse_delta, new Vector2(MouseSensitivity * MouseSmoothing, MouseSensitivity * MouseSmoothing));
        SmoothV.x = Mathf.Lerp(SmoothV.x, mouse_delta.x + AimingRecoilTotals.x, 1f / MouseSmoothing);
        SmoothV.y = Mathf.Lerp(SmoothV.y, mouse_delta.y + AimingRecoilTotals.y, 1f / MouseSmoothing);
        MouseLook += SmoothV;

        // Apply final values
        transform.localRotation = Quaternion.AngleAxis(-MouseLook.y, Vector3.right);
        player.transform.localRotation = Quaternion.AngleAxis(MouseLook.x, player.transform.up);

    }

    public void SetEnabled(bool value)
    {
        EnableLook = value;
    }
}
