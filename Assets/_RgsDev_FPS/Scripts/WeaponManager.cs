using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Tooltip("All weapons on player's hand")]
    [SerializeField] GameObject[] weapons;              //List of weapons
    [Tooltip("The wait time to finish draw gun animation and change weapon")]
    [SerializeField] float switchDelay = 1f;            //Time to switch between weapons
    [Tooltip("The grenade mesh in arms that will show/hide when throw animation is played")]
    [SerializeField] GameObject grenade;                //Used to show/hide grenade in the hands just for animation
    [Tooltip("The arms with grenade to show when throw grenade animation is played")]
    [SerializeField] GameObject handsWithGrenade;
    [Tooltip("The arms with melee weapon to show when melee attack animation is played")]
    [SerializeField] GameObject handsWithMeleeWeapon;
    [Tooltip("Time to wait before hide grenade mesh in player's hand")]
    [SerializeField] float timeToHideGrenade = 1f;      //Time to wait before hide grenade in hand
    [Tooltip("Time to wait before show current weapon in player's hand again")]
    [SerializeField] float timeToShowWeapon = 0.2f;     //Time to wait before show weapons again
    bool throwing;
    bool meleeAttack;

    int index;                                          //Current equipped weapon
    bool isSwitching;
    Animator anim;

    bool isReloading;
    bool isAiming;
    bool shootInput;


    public static bool pistola = false;
    public static bool m4 = false;
    public static bool francotirador = false;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        handsWithGrenade.SetActive(false);
        handsWithMeleeWeapon.SetActive(false);
        EnableFirstWeapon();
    }

    void Update()
    {
        if (GetComponentInChildren<WeaponRanged>() != null)
        {
            isReloading = GetComponentInChildren<WeaponRanged>().isReloading;
            shootInput = GetComponentInChildren<WeaponRanged>().shootInput;
            isAiming = GetComponentInChildren<WeaponRanged>().isAiming;

            if (weapons[1].activeSelf)
            {
                pistola = true;
                m4 = false;
                francotirador = false;
            }
            else if (weapons[2].activeSelf)
            {
                pistola = false;
                m4 = true;
                francotirador = false;
            }
            else if (weapons[3].activeSelf)
            {
                pistola = false;
                m4 = false;
                francotirador = true;
            }
            else
            {
                pistola = false;
                m4 = false;
                francotirador = false;
            }
        }

        //If not reloading and not shooting
        if (!isReloading && !shootInput && !throwing)
        {
            //Switch to previous weapon
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && !isSwitching)
            {
                index--;

                if (index < 0)
                {
                    index = weapons.Length - 1;
                }

                StartCoroutine(SwitchAfterDelay(index));
            }
            //Switch to next weapon
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && !isSwitching)
            {
                index++;

                if (index >= weapons.Length)
                {
                    index = 0;
                }

                StartCoroutine(SwitchAfterDelay(index));
            }
        }

        if (!isReloading && !throwing)
        {
            //Throw grenade
            if (Input.GetKeyDown(KeyCode.G))
            {
                StartCoroutine(ThrowGrenadeAnimation());
            }
        }

        
        if (!shootInput && !isReloading && !isAiming && !meleeAttack && !throwing)
        {
            //Melee attack
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(MeleeAttack());
            }
        }
    }

    //Weapon enabled when game start
    void EnableFirstWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }
        weapons[0].SetActive(true);
    }

    //Time to wait for switch weapons
    IEnumerator SwitchAfterDelay(int newIndex)
    {
        isSwitching = true;
        anim.SetTrigger("weaponSwitchTrigger");
        yield return new WaitForSeconds(switchDelay);
        isSwitching = false;        
        SwitchWeapons(newIndex);
    }

    void SwitchWeapons(int newIndex)
    {
        //Hide all weapons
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }
        //Show current weapon
        weapons[newIndex].SetActive(true);
        //Set animator component with the new active weapon
        anim = GetComponentInChildren<Animator>();
    }

    IEnumerator MeleeAttack()
    {
        meleeAttack = true;
        handsWithMeleeWeapon.SetActive(true);   //Active hands with melee weapon
        anim = GetComponentInChildren<Animator>(); //Set animator component with the new active weapon

        //Hide all weapons
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);   //Wait draw weapon animation finished and show hand with weapons again
        handsWithMeleeWeapon.SetActive(false);
        weapons[index].SetActive(true);
        anim = GetComponentInChildren<Animator>();
        meleeAttack = false;
    }

    IEnumerator ThrowGrenadeAnimation()
    {
        throwing = true;
        handsWithGrenade.SetActive(true);   //Active hands with grenade
        anim = GetComponentInChildren<Animator>(); //Set animator component with the new active weapon

        //Hide all weapons
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }   
        
        //Deactive hands with weapons
        yield return new WaitForSeconds(timeToHideGrenade);
        grenade.SetActive(false);   //When grenade is thrown, hide grenade in hand
        yield return new WaitForSeconds(timeToShowWeapon);   //Wait draw weapon animation finished and show hand with weapons again
        handsWithGrenade.SetActive(false);
        weapons[index].SetActive(true);
        anim = GetComponentInChildren<Animator>();
        grenade.SetActive(true);
        throwing = false;
    }
}
