using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent enemy;
    public Transform[] destinations;
    private int i = 0;
    private GameObject player;
    public float distanceToPath = 2;
    public float distanceToFollow = 10;
    private float distanceToPlayer;
    public bool followPlayer;

    private void Start()
    {
        enemy.destination = destinations[0].transform.position;
        player = FindObjectOfType<FirstPersonController>().gameObject;
    }

    private void Update()
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
    }

    public void EnemyPath()
    {
        if (Vector3.Distance(transform.position, destinations[i].position) <= distanceToPath)
        {
            if (destinations[i] != destinations[destinations.Length - 1]) 
            {
                i++;
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
