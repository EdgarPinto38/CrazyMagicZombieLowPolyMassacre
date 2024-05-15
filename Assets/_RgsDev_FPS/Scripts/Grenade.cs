using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Tooltip("The wait time to grenade explodes")]
    public float delay = 3f;
    public float radius = 5f;
    public float force = 700f;
    public float cameraShakeDuration = .03f;
    public float cameraShakeMagnitude = 30f;
    public GameObject explosionEffect; //Particle effect
    public AudioClip[] explosionSounds;
    float countdown; //Counter to explode grenade
    bool hasExploded = false;
    AudioSource audioSource;
    MeshRenderer grenadeMesh;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        grenadeMesh = GetComponent<MeshRenderer>();
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;

        //Destroy grenade after delay time
        if(countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        //Apply camera shake
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        CameraShake cameraShake = mainCamera.GetComponent<CameraShake>();

        if(cameraShake != null)
        {
            StartCoroutine(cameraShake.Shake(cameraShakeDuration, cameraShakeMagnitude));
        }

        ExplosionSound();
        Quaternion newRotation = Quaternion.Euler(-90, 0, 0); //fix rotation of particle explosion
        Instantiate(explosionEffect, transform.position, newRotation);

        //Get all other colliders inside contact radius
        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider nearbyObject in collidersToDestroy)
        {
            DestructibleObject destructible = nearbyObject.GetComponent<DestructibleObject>();

            if(destructible != null)
            {
                //Instantiate shattered object version that was been hit
                destructible.FractureObject();
            }
        }

        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider nearbyObject in collidersToMove)
        {
            if(nearbyObject.gameObject.tag == "Destructible" || nearbyObject.gameObject.tag == "Barrel")
            {
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    //Apply physics in all other object parts that was been hit
                    rb.isKinematic = false;
                    rb.AddExplosionForce(force, transform.position, radius);
                }
            }
        }

        //Disable granade mesh on player's hand after throw it
        grenadeMesh.enabled = false;
        Destroy(gameObject, 2.5f);
    }

    //Get a random sound clip in array
    public AudioClip GetRandomClip(AudioClip[] soundsArray)
    {
        return soundsArray[UnityEngine.Random.Range(0, soundsArray.Length)];
    }

    void ExplosionSound()
    {
        AudioClip clip = GetRandomClip(explosionSounds);
        audioSource.PlayOneShot(clip);
    }
}
