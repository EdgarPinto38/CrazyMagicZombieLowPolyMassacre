using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent enemy;
    public Transform[] destinations;
    private int i = 0;
    public float distanceToPath = 2;
    public bool followPlayer;
    private GameObject player;
    private float distanceToPlayer;
    public float distanceToFollow = 10;
    private Animator animator;
    //public int life = 50;
    
    

    void Start()
    {
        enemy.destination = destinations[0].transform.position;
        player = FindObjectOfType<FirstPersonController>().gameObject;
        animator = GetComponent<Animator>();
    }

     void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if(distanceToPlayer <= distanceToFollow && followPlayer )
        {
            FollowPlayer();
        }
        else
        {
            EnemyPath();
        }

       /* if (life==0)
        {
            //animacion de morir
            //corutina de morir
            Destroy(this.gameObject);
        }*/
    }

    public void TriggerHitAnimation()
    {
        animator.SetTrigger("Hit");
    }

    public void EnemyPath()
    {

        enemy.destination = destinations[i].position;

        if (Vector3.Distance(transform.position, destinations[i].position) <= distanceToPath)
        {
            if (destinations[i] != destinations[destinations.Length - 1]) 
            {
                i ++;
            }
            else
            {
                i = 0;
            }
        }
    }

    public void FollowPlayer()
    {
        enemy.destination = player.transform.position;
    }

    


}
