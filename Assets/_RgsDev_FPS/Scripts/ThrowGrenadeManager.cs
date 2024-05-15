using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenadeManager : MonoBehaviour
{
    [Tooltip("Sound of grenade pull pin in animation")]
    [SerializeField] AudioClip grenadePullPin;
    [Tooltip("Sound of throw grenade in animation")]
    [SerializeField] AudioClip throwGrenade;
    [Tooltip("Grenade instantiated on player's hand when throw grenade animation is finished")]
    [SerializeField] GameObject grenadePrefab;          //Grenade instantiated on player's hand when throw grenade animation is finished
    [Tooltip("Force applied to grenade when throwed")]
    [SerializeField] float throwGrenadeForce = 11f;
    [Tooltip("Grenade spawn point when throwed")]
    [SerializeField] Transform grenadeSpawnPoint;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponentInParent<AudioSource>();
    }

    //Called in animation event
    public void PullPin()
    {
        audioSource.PlayOneShot(grenadePullPin);
    }

    public void Throw()
    {
        audioSource.PlayOneShot(throwGrenade);
    }

    //Called in animation event and instantite grenade when animation finish
    public void ThrowGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, grenadeSpawnPoint.position, grenadeSpawnPoint.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(grenadeSpawnPoint.forward * throwGrenadeForce, ForceMode.Impulse);
    }
}
