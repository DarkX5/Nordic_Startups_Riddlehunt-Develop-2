using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityDestroyOnStart : MonoBehaviour
{
    /* Use on testing objects (parents) so they don't show up in the scene */
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject);
    }
}
