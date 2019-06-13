using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Bullet : PlayersArena
{
    PhotonView PV;
    private float Timer = 120;

    // Setup and start
    void Awake()
    {
        PV = GetComponent<PhotonView>();   
    }

    public void Shoot(Weapons weapon, Transform parent, RaycastHit shot_raycast, ArmedObject arms)
    {
        float speed = 500f;
        switch (weapon)
        {
            case Weapons.Pistol_HeartBreaker:
                speed = Pistol_HeartBreaker.BulletSpeed;
                break;
        }

        // If nothing in front of bullet just launch towards center
        if (shot_raycast.collider == null)
        {
            GetComponent<Rigidbody>().AddForce(parent.transform.forward * speed);
        }
        else
        {
            // If something in front of bullet, use raycast hit to shot bullet at raycast
            parent.transform.LookAt(shot_raycast.point);
            GetComponent<Rigidbody>().AddForce(parent.transform.forward * speed);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        PV.RPC("CreateBulletHole", RpcTarget.All, collision.GetContact(0).point, collision.GetContact(0).normal);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
        }
        else
        {
            // Destroy self
            Destroy(gameObject);
        }
    }

    [PunRPC]
    void CreateBulletHole(Vector3 position, Vector3 norm)
    {
        Instantiate(Resources.Load("BulletDecal"), position, Quaternion.LookRotation(norm));
    }

}
