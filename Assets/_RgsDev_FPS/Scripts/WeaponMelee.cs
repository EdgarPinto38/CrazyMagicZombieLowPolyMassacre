using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponMelee : MonoBehaviour
{
    [Header("Properties")]
    public float attackRate = 0.5f;
    public float damage = 10f;
    public float range = 1f;
    [Tooltip("Wait time to show damage effect after attack on object collided")]
    public float impactDelay = .5f;             

    [Header("Components")]
    [Tooltip("The start point of the attack raycast hit")]
    public Transform attackPoint;               //Point to instantiate bullet
    [Tooltip("The effects when attack raycast hit something")]
    public GameObject hitParticles;
    [Tooltip("The impact hit decal")]
    public GameObject impactHole;				// The spot where the weapon should spit shells from

    [Header("Sounds")]    
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip drawGunSound;
    [SerializeField] AudioClip holsterGunSound;
    [SerializeField] AudioClip[] footstepSounds;

    AudioSource audioSource;
    Animator anim;
    float attackTimer;
    [HideInInspector] public bool attackInput;
    [HideInInspector] public bool isJumping;

    [Header("UI")]
    [Tooltip("Ammo amount displayed on HUD")]
    public Text ammoText;                               //Ammo amount displayed in HUD
    [Tooltip("Crosshair displayed on HUD")]
    public GameObject crossHair;

    private void OnEnable()
    {
        //Disable ammo amount on HUD when arms with a melee weapon is enabled
        if(ammoText != null)
            ammoText.enabled = false;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponentInParent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        attackInput = Input.GetButton("Fire1");

        //Attack
        if (attackInput)
        {
            Attack();
        }

        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            if(!isJumping)
                StartCoroutine(Jump());
        }

        if (attackTimer < attackRate)
            attackTimer += Time.deltaTime;
    }
    
    void Attack()
    {
        if (attackTimer < attackRate) return;

        anim.SetTrigger("attack");
        int attackType = Random.Range(0, 2);
        anim.SetInteger("attackType", attackType); //Play a random attack type animation

        RaycastHit hit;
        Vector3 shootDirection = attackPoint.transform.forward;

        //If attack hit something
        if (Physics.Raycast(attackPoint.position, shootDirection, out hit, range))
        {
            StartCoroutine(ImpactEffectDelay(hit));
        }
        attackTimer = 0.0f;
    }

    IEnumerator Jump()
    {
        isJumping = true;
        anim.SetTrigger("jump");
        yield return new WaitForSeconds(1f);
        isJumping = false;
    }

    //Wait a delay time to show impact effect in collided object
    IEnumerator ImpactEffectDelay(RaycastHit hit)
    {
        yield return new WaitForSeconds(impactDelay);
        GameObject hitParticleEffect = Instantiate(hitParticles, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
        hitParticleEffect.transform.SetParent(hit.transform);
        GameObject bulletHole = Instantiate(impactHole, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
        bulletHole.transform.SetParent(hit.transform);
        Destroy(bulletHole, 5f);
        Destroy(hitParticleEffect, 1f);
    }

    //Choose a random audioclip in an array
    public AudioClip GetRandomClip(AudioClip[] soundsArray)
    {
        return soundsArray[UnityEngine.Random.Range(0, soundsArray.Length)];
    }

    //If you want footstep sound be played based on animation time, just call this function in animation events
    public void Footstep()
    {
        AudioClip clip = GetRandomClip(footstepSounds);
        audioSource.PlayOneShot(clip);
    }

    //Functions called in Animation Events
    public void DrawWeapon()
    {
        audioSource.PlayOneShot(drawGunSound);
    }

    public void HolsterWeapon()
    {
        audioSource.PlayOneShot(holsterGunSound);
    }

    public void Melee()
    {
        audioSource.PlayOneShot(attackSound);
    }
}