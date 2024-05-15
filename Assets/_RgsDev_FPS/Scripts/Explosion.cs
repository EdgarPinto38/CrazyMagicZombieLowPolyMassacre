using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public ParticleSystem explosion;            //Explosion particle effect
    public float minForce;
    public float maxForce;
    public float radius;
    public float cameraShakeDuration = .03f;
    public float cameraShakeMagnitude = 50f;
    public bool destroyAfterTime;   //If will be destroyed after some time
    public float timeToDestroy = 5f;
    public enum objType { wall, barrel };
    public objType objectType;

    public AudioClip[] explosionSounds;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Explode();
    }

    public void Explode()
    {
        if(explosion != null)
        {
            explosion.Play();
        }
        
        ExplosionSound();

        //Apply explision force for each shattered piece
        foreach(Transform t in transform)
        {
            var rb = t.GetComponent<Rigidbody>();

            if(rb != null)
            {
                //Apply force to each object part that contains a rigid body
                rb.AddExplosionForce(Random.Range(minForce, maxForce), transform.position, radius);
            }
        }

        if (objectType == objType.barrel)
        {
            //Apply camera shake
            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            CameraShake cameraShake = mainCamera.GetComponent<CameraShake>();

            if (cameraShake != null)
            {
                StartCoroutine(cameraShake.Shake(cameraShakeDuration, cameraShakeMagnitude));
            }

            //Get all other colliders inside contact radius
            Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider nearbyObject in collidersToDestroy)
            {
                DestructibleObject destructible = nearbyObject.GetComponent<DestructibleObject>();

                if (destructible != null)
                {
                    //Instantiate shattered object version that was been hit
                    destructible.FractureObject();
                }
            }

            Collider[] collidersToMove = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider nearbyObject in collidersToMove)
            {
                if (nearbyObject.gameObject.tag == "Destructible" || nearbyObject.gameObject.tag == "Barrel")
                {
                    Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

                    //Apply physics in all other object parts that was been hit
                    if (rb != null)
                    {
                        rb.isKinematic = false;
                        rb.AddExplosionForce(Random.Range(minForce, maxForce), transform.position, radius);
                    }
                }
            }
        }

        if (destroyAfterTime)
        {
            Destroy(gameObject, timeToDestroy);
        }
    }

    //Get a random sound clip in an array of clips
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
