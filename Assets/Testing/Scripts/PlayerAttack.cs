using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage = 20;
    public string attackType = "Hit";

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
            {
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage, attackType);
                }
            }
        }
    }
}
