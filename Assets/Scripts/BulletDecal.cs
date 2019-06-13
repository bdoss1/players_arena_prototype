using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDecal : MonoBehaviour
{
    private float Timer = 15;
   
    // Update is called once per frame
    void Update()
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
}
