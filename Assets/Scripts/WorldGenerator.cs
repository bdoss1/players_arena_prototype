using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    // Map sizing
    public Vector2 map_x_min_max;
    public Vector2 map_z_min_max;
    public int GrassDensity;

    void Start()
    {
        // If map size not set, don't generate
        if (map_x_min_max == null || map_z_min_max == null) return;

        // Figure out how many blades of grass to make
        int total_grass = Random.Range(GrassDensity / 2, GrassDensity) + GrassDensity / 2;

        // Create grass
        while (total_grass > 0)
        {
            float rnd_x = Random.Range(map_x_min_max[0], map_x_min_max[1]);
            float rnd_z = Random.Range(map_z_min_max[0], map_z_min_max[1]);
            float y = transform.position.y;

            RaycastHit hit;
            if (Physics.Raycast(new Vector3(rnd_x, y, rnd_z), transform.TransformDirection(-Vector3.up), out hit, Mathf.Infinity))
            {
                // If ground hit
                if(hit.transform.gameObject.name == "Map")
                {
                    float create_y = hit.point.y;
                    GameObject grass_1 = (GameObject)Resources.Load("EnvironmentPrefabs/Grass_1");
                    Instantiate(grass_1, new Vector3(rnd_x, create_y, rnd_z), Quaternion.identity, gameObject.transform);
                }
            }

            // Subtract grass just made
            total_grass--;
        }
    }

}
