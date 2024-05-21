using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemyController = other.gameObject.GetComponent<Enemy>();
            if (enemyController != null)
            {
                enemyController.TriggerHitAnimation();
            }
            Destroy(gameObject);
        }
    }

}
