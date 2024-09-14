using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour, IDamage
{
    //Player logic variables
    [Header("Player Controller")]
    [SerializeField] CharacterController controller;
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
    [SerializeField] weaponHandler weapon;
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
    
    bool isShooting;
    public bool IsShooting() { return isShooting; }

    bool isWalkAudio;

    gameManager game;

    // Start is called before the first frame update
    void Start()
    {
        game = gameManager.instance;
        shootRate = weapon.GetFireRate();
        weapon.UpdateUI();
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
        //moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); --World Transform
        //transform.position += moveDir * speed * Time.deltaTime; --No Collision

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

        if (Input.GetButton("Shoot") && !isShooting)
        {
            StartCoroutine(Shoot());
        }

        if(Input.GetButtonDown("Reload"))
        {
            weapon.DoReload();
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

    IEnumerator Shoot()
    {
        isShooting = true;
        weapon.Fire();
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
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
        return weapon;
    }
}
