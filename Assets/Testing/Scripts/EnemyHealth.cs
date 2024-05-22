using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 100;
    public Animator animator;

    public void TakeDamage(int amount, string attackType)
    {
        health -= amount;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            //////// animación basada en el tipo de ataque
            animator.SetTrigger(attackType);
        }
    }

    void Die()
    {
        animator.SetTrigger("Die");
        ////////// lógica para la muerte del enemigo
        Destroy(gameObject, 2f);
    }
}
