using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponRanged : MonoBehaviour
{ 
    [Header("Properties")]
    public float fireRate = 0.1f;
    public float damage = 20f;
    public float range = 100f;
    [Tooltip("Force applied on objects that raycast hit")]
    public float bulletForce = 10f;                     //Force applied in objects that was hit
    [Tooltip("Zoom applied to main camera when aiming down sight")]
    public float ADSZoomMainCamera = 50f;                      //Zoom amount when aim down sight
    [Tooltip("Zoom applied to weapon camera when aiming down sight")]
    public float ADSZoomWeaponCamera = 25f;                      //Zoom amount when aim down sight
    [Tooltip("Speed to apply zoom on camera when aiming down sight")]
    public float zoomSpeed = 5f;                        //Aim down sight speed    
    [Tooltip("If scope vision is displayed when aim down sights")]
    public bool hasScope;
    [Tooltip("Time to wait until show scope overlay when press ADS button")]
    public float showScopeDelay = 0.2f;
    [Tooltip("The weapon bullet spread")]
    public float spreadFactor = 0.1f;
    [Tooltip("Time to wait to reload bullets in magazine")]
    public float reloadTime = 1f;
    [Tooltip("If weapon has auto reload when magazine is empty")]
    public bool autoReload;
    [Tooltip("Total magazine capacity")]
    public int bulletsPerMag = 30;
    [Tooltip("Maximun ammo amount that the player can carry in his pocket")]
    public int maxAmmo = 200;                       //Bullets in your pocket
    [HideInInspector]
    public int currentBullets;
    [Tooltip("Delay time to spit bullet shells")]
    public float timeToSpitBulletShells = 0f;                     //Time to wait until spit bullet shells
    [Tooltip("Time to wait until show bullets mesh in magazine when reload animation is played")]
    public float timeToShowBullets;                     //Time to wait until show bullets in magazine when reloading
    [Tooltip("If camera shaking when shooting")]
    public bool cameraShakeWhenShoot;
    public float cameraShakeDuration = .03f;
    public float cameraShakeMagnitude = .05f;
    public enum WeaponType { Auto, Semi }
    public WeaponType weaponType;

    [Header("Components")]
    [Tooltip("Start point of raycast when shooting")]
    public Transform shootPoint;                       //Point to instantiate bullet
    [Tooltip("Particles effect when raycast hit on other objects")]
    public GameObject hitParticles;
    [Tooltip("Bullet impact decal")]
    public GameObject bulletImpact;                    //Bullet hole
    [Tooltip("Bullets mesh to show/hide in weapon magazine when reload animation is played")]
    public GameObject magazineBullets;                 //Get bullets mesh in magazine to show/hide on reload animation
    public ParticleSystem muzzleFlash;
    [Tooltip("The main camera")]
    public Camera mainCamera;
    [Tooltip("The weapon camera on a separated layer to avoid weapon pass through other objects")]
    public Camera weaponCamera;
    [Tooltip("The scope overlay to show when aim down sight")]
    public GameObject scopeOverlay;
    [Tooltip("If weapon eject shells when shoot")]
    public bool spitShells;                     // Whether or not this weapon should spit shells out of the side
    [Tooltip("Shell prefab")]
    public GameObject shell;                            // A shell prop to spit out the side of the weapon
    [Tooltip("Start point that shell will be ejected")]
    public Transform shellSpitPosition;					// The spot where the weapon should spit shells from
    public AudioClip[] shellSounds;                     // The sound played when shell is ejected
    public float shellSpitForce = 1.0f;                 // The force with which shells will be spit out of the weapon
    public float shellForceRandom = 0.5f;               // The variant by which the spit force can change + or - for each shot
    public float shellSpitTorqueX = 0.0f;               // The torque with which the shells will rotate on the x axis
    public float shellSpitTorqueY = 0.0f;               // The torque with which the shells will rotate on the y axis
    public float shellTorqueRandom = 1.0f;              // The variant by which the spit torque can change + or - for each shot

    [Header("Sounds")]    
    AudioSource audioSource;
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip emptyShotSound;
    [SerializeField] AudioClip drawGunSound;
    [SerializeField] AudioClip holsterGunSound;
    [SerializeField] AudioClip dropMagazine;            //When magazine drop in reload animation
    [SerializeField] AudioClip inputMagazine;           //When magazine inserted in reload animation
    [SerializeField] AudioClip lockMagazine;            //When magazine lock button pressed in reload animation
    [SerializeField] AudioClip aiming;
    [SerializeField] AudioClip[] footstepSounds;

    [Header("UI")]
    [Tooltip("Ammo amount displayed on HUD")]
    public Text ammoText;                               //Ammo amount displayed in HUD
    [Tooltip("Crosshair displayed on HUD")]
    public GameObject crossHair;

    Animator anim;
    float fireTimer;
    [HideInInspector] public bool isReloading;
    [HideInInspector] public bool isAiming;
    [HideInInspector] public bool shootInput;
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isMeleeAttack;
    float walkSpeed;
    float runSpeed;
    float mainCameraFOV;
    float weaponCameraFOV;

    private void OnEnable()
    {
        if (ammoText != null)
            ammoText.enabled = true; //Enable ammo amount on HUD when arms with a fire weapon is enabled
        
        UpdateAmmoText();
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponentInParent<AudioSource>();
    }

    void Start()
    {
        walkSpeed = GetComponentInParent<FPSController>().walkSpeed;
        runSpeed = GetComponentInParent<FPSController>().runSpeed;
        mainCameraFOV = mainCamera.fieldOfView;
        weaponCameraFOV = weaponCamera.fieldOfView;
        currentBullets = bulletsPerMag;
        UpdateAmmoText();

        //Get bullets mesh in magazine and disable the render
        magazineBullets.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (weaponType)
        {
            case WeaponType.Auto:
                shootInput = Input.GetButton("Fire1");
                break;

            case WeaponType.Semi:
                shootInput = Input.GetButtonDown("Fire1");
                break;
        }
        
        //Shoot
        if (shootInput)
        {
            if (currentBullets > 0)
            {
                Fire();
            }
            else if (maxAmmo > 0 && !isAiming && !isReloading)
            {
                if (autoReload)
                {
                    StartCoroutine(ReloadAnim());
                }
                else
                {
                    if (emptyShotSound != null)
                    {
                        PlaySound(emptyShotSound);
                    }
                }
                
            }
                
        }
        
        //Reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentBullets < bulletsPerMag && maxAmmo > 0 && !isReloading && !isAiming)
            {
                StartCoroutine(ReloadAnim());
            }
        }

        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (!isAiming && !isJumping)
            {
                StartCoroutine(Jump());
            }
        }

        if (fireTimer < fireRate)
            fireTimer += Time.deltaTime;

        //Aim down sight
        if (!isReloading)
        {
            AimDownSights();
        }            
    }

    private void FixedUpdate()
    {
        anim.SetBool("ads", isAiming);
        anim.SetInteger("bullets", currentBullets);

        //Check pistol slider position
        if(currentBullets <= 0 && !isReloading)
        {
            anim.SetLayerWeight(1, 1.0f);
        }
        else
        {
            anim.SetLayerWeight(1, 0.0f);
        }
    }

    void Fire()
    {
        if (fireTimer < fireRate || currentBullets <= 0 || isReloading) return;

        if (cameraShakeWhenShoot)
        {
            //Apply camera shake
            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            CameraShake cameraShake = mainCamera.GetComponent<CameraShake>();

            //Check if not shaking to prevent bug shaking when shoot while something explodes
            if (cameraShake != null && !cameraShake.isShaking)
            {
                StartCoroutine(cameraShake.Shake(cameraShakeDuration, cameraShakeMagnitude));
            }
        }

        //If not aiming
        if (!isAiming)
            anim.CrossFadeInFixedTime("Fire", 0.01f); //Play fire animation
        else
            anim.CrossFadeInFixedTime("ADS Fire", 0.01f); //Play ADS fire animation       

        RaycastHit hit;

        if (muzzleFlash != null) {
            StartCoroutine(MuzzeFlash());
        }

        Vector3 shootDirection = shootPoint.transform.forward;
        //Apply bullet spread
        shootDirection = shootDirection + shootPoint.TransformDirection
            (new Vector3(Random.Range(-spreadFactor, spreadFactor), Random.Range(-spreadFactor, spreadFactor)));

        //If shot hit something
        if (Physics.Raycast(shootPoint.position, shootDirection, out hit, range))
        {
            GameObject hitParticleEffect = Instantiate(hitParticles, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            hitParticleEffect.transform.SetParent(hit.transform);
            GameObject bulletHole = Instantiate(bulletImpact, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            bulletHole.transform.SetParent(hit.transform);

            Destroy(hitParticleEffect, 1f);
            Destroy(bulletHole, 5f);

            //If other object is destructible barrel
            if (hit.collider.gameObject.tag == "Barrel")
            {
                DestructibleObject destructible = hit.transform.gameObject.GetComponent<DestructibleObject>();

                if (destructible != null)
                {
                    destructible.FractureObject();
                }
            }
        }

        PlaySound(shootSound);

        // Instantiate shell props
        if (spitShells)
        {
            StartCoroutine(SpitShells());
        }

        currentBullets--;
        UpdateAmmoText();
        fireTimer = 0.0f;
    }

    void AimDownSights()
    {
        if(Input.GetButton("Fire2"))
        {
            isAiming = true;
            crossHair.SetActive(false);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, ADSZoomMainCamera, Time.deltaTime * zoomSpeed);
            weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, ADSZoomWeaponCamera, Time.deltaTime * zoomSpeed);
            GetComponentInParent<FPSController>().runSpeed = walkSpeed;

            if (hasScope)
                StartCoroutine(OnScoped());
        }
        else
        {
            isAiming = false;
            crossHair.SetActive(true);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, mainCameraFOV, Time.deltaTime * zoomSpeed);
            weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, weaponCameraFOV, Time.deltaTime * zoomSpeed);
            GetComponentInParent<FPSController>().runSpeed = runSpeed;

            if (hasScope)
                OnUnscoped();
        }
    }
    
    //Check if is reloading before try reload again
    IEnumerator ReloadAnim()
    {
        isReloading = true;
        anim.SetTrigger("reload");
        StartCoroutine(DisableBulletMesh());
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;
    }

    IEnumerator Jump()
    {
        isJumping = true;
        anim.SetTrigger("jump");
        yield return new WaitForSeconds(1f);
        isJumping = false;
    }

    IEnumerator OnScoped()
    {
        yield return new WaitForSeconds(showScopeDelay);
        scopeOverlay.SetActive(true);
        weaponCamera.enabled = false;
    }

    void OnUnscoped()
    {
        scopeOverlay.SetActive(false);
        weaponCamera.enabled = true;
    }

    //Eject shell when shoot
    IEnumerator SpitShells()
    {
        yield return new WaitForSeconds(timeToSpitBulletShells);
        GameObject shellGO = Instantiate(shell, shellSpitPosition.position, shellSpitPosition.rotation) as GameObject;
        shellGO.transform.SetParent(transform); //Set parent to avoid weird behaviour when shell spawns
        shellGO.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(shellSpitForce + Random.Range(0, shellForceRandom), 0, 0), ForceMode.Impulse);
        shellGO.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(shellSpitTorqueX + Random.Range(-shellTorqueRandom, shellTorqueRandom), shellSpitTorqueY + Random.Range(-shellTorqueRandom, shellTorqueRandom), 0), ForceMode.Impulse);
        StartCoroutine(PlayShellSound());
        yield return new WaitForSeconds(1f);
        shellGO.transform.parent = null; //Unparent shell to not follow the player
    }

    IEnumerator MuzzeFlash()
    {
        muzzleFlash.Play();
        yield return new WaitForSeconds(1f);
        muzzleFlash.Stop();
    }

    void PlaySound(AudioClip clipSound)
    {
        audioSource.PlayOneShot(clipSound);
    }

    IEnumerator PlayShellSound()
    {
        AudioClip clip = GetRandomClip(shellSounds);
        yield return new WaitForSeconds(0.8f); //Time to wait until bullet shell hit the ground
        PlaySound(clip);
    }

    public void UpdateAmmoText()
    {
        if(ammoText != null)
            ammoText.text = currentBullets + " / " + maxAmmo;
    }    

    IEnumerator DisableBulletMesh()
    {
        if (magazineBullets != null)
        {
            //Wait time until player put new full magazine in the hand to show bullets
            yield return new WaitForSeconds(timeToShowBullets);
            magazineBullets.SetActive(true);
            //Wait 2 seconds and hide bullets again when weapon is reloaded
            yield return new WaitForSeconds(2f);
            magazineBullets.SetActive(false);
        }

    }

    //Choose a random audioclip in an array
    public AudioClip GetRandomClip(AudioClip[] soundsArray)
    {
        return soundsArray[UnityEngine.Random.Range(0, soundsArray.Length)];
    }

    //Footstep sound be played based on animation time, this function is called in animation events
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

    public void DropMag()
    {
        audioSource.PlayOneShot(dropMagazine);
    }

    public void InputMag()
    {
        audioSource.PlayOneShot(inputMagazine);
    }

    public void LockMag()
    {
        audioSource.PlayOneShot(lockMagazine);
    }

    //Aim Down Sight sound
    public void ADS()
    {
        audioSource.PlayOneShot(aiming);
    }

    //Called on Animation event to insert bullets into mag when reload animation is finished
    public void Reload()
    {
        if (maxAmmo <= 0) return;

        int bulletsToLoad = bulletsPerMag - currentBullets;
        int bulletsToDeduct = (maxAmmo >= bulletsToLoad) ? bulletsToLoad : maxAmmo;

        maxAmmo -= bulletsToDeduct;
        currentBullets += bulletsToDeduct;

        UpdateAmmoText();
    }
}