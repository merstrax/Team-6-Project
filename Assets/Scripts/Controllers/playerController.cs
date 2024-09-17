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
    [SerializeField] float regenDelay;

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
    float lastHitTime;
    bool isRegenerating;

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

        UpdateMovement();
        UpdateSprint();

        UpdateWeapon();

        if(Time.time - lastHitTime > regenDelay && !isRegenerating)
            StartCoroutine(HealthRegen());
    }

    void UpdateMovement()
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
    }

    void UpdateSprint()
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

    IEnumerator WalkAudio()
    {
        isWalkAudio = true;
        int rand = Random.Range(0, audioWalk.Length);
        audioPlayer.PlayOneShot(audioWalk[rand], 0.5f);

        float audioDelay = 1.5f;

        if (isSprinting)
            audioDelay = 1.0f;

        yield return new WaitForSeconds(audioWalk[rand].length * audioDelay);
        isWalkAudio = false;
    }

    void UpdateWeapon()
    {
        if (weaponEquipped.IsAutomatic())
        {
            if (Input.GetButton("Shoot") && !isShooting && !isSprinting && !isSwapWeapon)
            {
                StartCoroutine(Shoot());
            }
        }
        else
        {
            if (Input.GetButtonDown("Shoot") && !isShooting && !isSprinting && !isSwapWeapon)
            {
                StartCoroutine(Shoot());
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0 && CanSwapWeapon())
        {
            StartCoroutine(SwapWeapon());
        }

        if (Input.GetButtonDown("Reload"))
        {
            weaponEquipped.DoReload();
        }
    }

    public void PlayNewWaveSound()
    {
        audioPlayer.Play();
    }

    public weaponHandler GetWeapon()
    {
        return weaponEquipped;
    }

    public int GetWeaponSlot()
    {
        return weaponEquipped == weapon1 ? 1 : 2;
    }

    public void EquipWeapon(weaponHandler newWeapon, int slot)
    {
        if(weapon2 == null)
        {
            weapon2 = Instantiate(newWeapon);
            weapon2.gameObject.transform.SetParent(gunPos);
            weapon2.transform.localScale = newWeapon.transform.localScale;
            weapon2.transform.localRotation = newWeapon.transform.localRotation;
            weapon2.transform.localPosition = newWeapon.transform.localPosition;
            weapon2.gameObject.SetActive(false);
            return;
        }

        switch(slot)
        {
            case 1:
                if(weaponEquipped == weapon1)
                {
                    weaponEquipped = null;
                }
                Destroy(weapon1.gameObject);
                weapon1 = Instantiate(newWeapon);
                weapon1.gameObject.transform.SetParent(gunPos);
                weapon1.transform.localScale = newWeapon.transform.localScale;
                weapon1.transform.localRotation = newWeapon.transform.localRotation;
                weapon1.transform.localPosition = newWeapon.transform.localPosition;
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

                    Destroy(weapon2.gameObject);
                }
                weapon2 = Instantiate(newWeapon);
                weapon2.gameObject.transform.SetParent(gunPos);
                weapon2.transform.localScale = newWeapon.transform.localScale;
                weapon2.transform.localRotation = newWeapon.transform.localRotation;
                weapon2.transform.localPosition = newWeapon.transform.localPosition;
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

        weaponEquipped.UpdateUI();
        yield return new WaitForSeconds(0.5f);

        weaponEquipped.UpdateUI();
        isSwapWeapon = false;
    }

    public void TakeDamage(float amount, Vector3 loc, Quaternion rotation, bool headshot = false)
    {
        TakeDamage(amount);
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

        // stops regen
        StopHealthRegen();
    }

    IEnumerator DamageFlash()
    {
        gameManager.instance.damagePanel.SetActive(true);
        yield return new WaitForSeconds(0.25f);

        gameManager.instance.damagePanel.SetActive(false);
    }

    IEnumerator HealthRegen()
    {
        isRegenerating = true;
       
        // Gradual regeneration over time
        // Increase health gradually
        healthCurrent = Mathf.Min(healthCurrent + 1, healthMax); 
        game.GetPlayerInterface().UpdatePlayerHealth(healthCurrent, healthMax);

        // Adjust the wait time for regeneration speed
        yield return new WaitForSeconds(regenDelay / 2);

        isRegenerating = false;
    }

    void StopHealthRegen()
    {
        // updates last hit time
        lastHitTime = Time.time;

        if (isRegenerating)
        {
            StopCoroutine(HealthRegen());
        }

        isRegenerating = false;
    }
}
