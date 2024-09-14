using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour, IDamage
{
    //Player logic variables
    [Header("Player Controller")]
    [SerializeField] CharacterController controller;
    [SerializeField] Transform gunPos;
    [SerializeField] LayerMask ignoreMask;

    [Header("Player Stats")]
    [SerializeField] int healthMax;

    //Player Movement
    [Header("Player Movement")]
    [SerializeField] float speed;
    [SerializeField] float sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;

    //Player Shoot
    [Header("Player Weapon")]
    [SerializeField] weaponHandler weaponEquipped;
    [SerializeField] weaponHandler weapon1;
    [SerializeField] weaponHandler weapon2;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] float shootDistance;

    [Header("Player Audio")]
    [SerializeField] AudioSource audioPlayer;
    [SerializeField] AudioClip[] audioDamage;
    [SerializeField] AudioClip[] audioWalk;
    [SerializeField] AudioClip[] audioJump;

    Vector3 moveDir;
    Vector3 playerVel;

    int healthCurrent;

    int jumpCount;

    bool isSprinting;
    public bool IsSprinting() {  return isSprinting; }

    bool isSwapWeapon;
    bool isShooting;
    public bool IsShooting() { return isShooting; }
    public bool CanSwapWeapon() { 
        return !isShooting && !isSwapWeapon && !weaponEquipped.IsReloading() && weapon2 != null;
    }

    bool isWalkAudio;

    gameManager game;

    // Start is called before the first frame update
    void Start()
    {
        game = gameManager.instance;
        
        //Setup weapons
        weaponEquipped = weapon1;
        weaponEquipped.UpdateUI();
        shootRate = weaponEquipped.GetFireRate();

        healthCurrent = healthMax;
        game.GetPlayerInterface().UpdatePlayerHealth(healthCurrent, healthMax);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);

        Movement();
        Sprint();
    }


    void Movement()
    {
        //Reset jump variables
        if (controller.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCount = 0;
        }

        //Transform and move based on local space
        moveDir = Input.GetAxis("Horizontal") * transform.right +
                    Input.GetAxis("Vertical") * transform.forward;

        controller.Move(speed * Time.deltaTime * moveDir);


        //Jump logic
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
            int rand = Random.Range(0, audioJump.Length);
            audioPlayer.PlayOneShot(audioJump[rand]);
        }

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (moveDir != Vector3.zero && !isWalkAudio)
        {
            StartCoroutine(WalkAudio());
        }

        if (weaponEquipped.IsAutomatic())
        {
            if (Input.GetButton("Shoot") && !isShooting && !isSprinting)
            {
                StartCoroutine(Shoot());
            }
        }
        else
        {
            if (Input.GetButtonDown("Shoot") && !isShooting && !isSprinting)
            {
                StartCoroutine(Shoot());
            }
        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0 && CanSwapWeapon())
        {
            StartCoroutine(SwapWeapon());
        }

        if (Input.GetButtonDown("Reload"))
        {
            weaponEquipped.DoReload();
        }
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }

    }

    public void PlayNewWaveSound()
    {
        audioPlayer.Play();
    }

    public void EquipWeapon(weaponHandler newWeapon, int slot)
    {
        switch(slot)
        {
            case 1:
                if(weaponEquipped == weapon1)
                {
                    weaponEquipped = null;
                }
                Destroy(weapon1);
                weapon1 = Instantiate(newWeapon);
                weapon1.gameObject.transform.SetParent(gunPos);
                if(weaponEquipped == null)
                    weaponEquipped = weapon1;
                break;
            case 2:
                if (weapon2 != null)
                {
                    if (weaponEquipped == weapon2)
                    {
                        weaponEquipped = null;
                    }

                    Destroy(weapon2);
                }
                weapon2 = Instantiate(newWeapon);
                weapon2.gameObject.transform.SetParent(gunPos);
                if (weaponEquipped == null)
                    weaponEquipped = weapon2;
                break;
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        weaponEquipped.Fire();
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator SwapWeapon()
    {
        isSwapWeapon = true;

        if (weaponEquipped == weapon1)
        {
            weapon2.gameObject.SetActive(true);
            weaponEquipped = weapon2;
            weapon1.gameObject.SetActive(false);
        }
        else
        {
            weapon1.gameObject.SetActive(true);
            weaponEquipped = weapon1;
            weapon2.gameObject.SetActive(false);
        }
        

        yield return new WaitForSeconds(0.5f);

        weaponEquipped.UpdateUI();
        isSwapWeapon = false;
    }

    IEnumerator WalkAudio()
    {
        isWalkAudio = true;
        int rand = Random.Range(0, audioWalk.Length);
        audioPlayer.PlayOneShot(audioWalk[rand], 0.2f);

        float audioDelay = 1.5f;

        if (isSprinting)
            audioDelay = 1.0f;

        yield return new WaitForSeconds(audioWalk[rand].length * audioDelay);
        isWalkAudio = false;
    }

    public void TakeDamage(float amount, bool headshot = false)
    {
        healthCurrent -= (int)amount;
        game.GetPlayerInterface().UpdatePlayerHealth(healthCurrent, healthMax);
        StartCoroutine(DamageFlash()); 

        int rand = Random.Range(0, audioDamage.Length);
        audioPlayer.PlayOneShot(audioDamage[rand], 0.5f);

        if(healthCurrent <= 0)
        {
            gameManager.instance.YouLose();
        }
    }

    IEnumerator DamageFlash() 
    {
        gameManager.instance.damagePanel.SetActive(true);
        yield return new WaitForSeconds(0.25f); 

        gameManager.instance.damagePanel.SetActive(false); 
    }

    public void TakeDamage(float amount, Vector3 loc, Quaternion rotation, bool headshot = false)
    {
        TakeDamage(amount);
    }

    public weaponHandler GetWeapon()
    {
        return weaponEquipped;
    }
}
