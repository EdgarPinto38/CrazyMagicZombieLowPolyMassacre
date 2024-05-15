using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [Tooltip("Shattered object spawned when intact object is destroyed")]
    public GameObject shatteredObject;
    bool destroyed;

    private void Start()
    {
        destroyed = false;
    }

    public void FractureObject()
    {
        if (!destroyed)
        {
            destroyed = true;
            Destroy(gameObject);
            //Instantiate the shattered object version
            Instantiate(shatteredObject, gameObject.transform.position, gameObject.transform.rotation);
        }
    }
}
