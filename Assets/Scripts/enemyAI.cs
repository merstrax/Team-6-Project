using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("Render Components")]
    [SerializeField] Animator animator;
    [SerializeField] Renderer model;
    [SerializeField] ParticleSystem particle;

    [Header("AI Nav")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] int faceTargetSpeed;

    [Header("Audio")]
    [SerializeField] AudioSource audioPlayer; 
    [SerializeField] AudioClip audioHitMarker;
    [SerializeField] AudioClip audioHeadShot;

    [Header("Enemy Combat")]
    [SerializeField] float HP;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject rightHandPos;
    [SerializeField] GameObject leftHandPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float attackRate;

    Color colorOrig;

    bool playerInRange;
    bool isAttacking;
    bool isDead;
    public bool isMoving;
    public bool canAttack = true;

    Vector3 playerDir;
    

    // Start is called before the first frame update
    void Start()
    {
        //colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            playerDir = gameManager.instance.player.transform.position - headPos.position;

            agent.SetDestination(gameManager.instance.player.transform.position);
            isMoving = !((agent.remainingDistance <= agent.stoppingDistance) || (agent.velocity == Vector3.zero));


            animator.SetBool("IsMoving", isMoving);
            if (isMoving)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Z_Attack"))
                    animator.Play("Z_Run_InPlace");
            }
            else
            {
                FaceTarget();
                if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Z_Attack"))
                    animator.Play("Z_Idle");
            }

            animator.SetBool("CanAttack", playerInRange);
            if (playerInRange)
            { 
                if (canAttack)
                {
                    StartCoroutine(DoAttack());
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }


    public void TakeDamage(float amount, bool headshot = false)
    {
        HP -= amount;

        //StartCoroutine(DamageTakenFlash());
        PlayHitMarkerSound();

        if (headshot && !isDead)
        {
            audioPlayer.PlayOneShot(audioHeadShot);
        }

        if (HP <= 0 && !isDead)
        {
            OnDeath();
        }
    }

    public void TakeDamage(float amount, Vector3 loc, Quaternion rot, bool headshot = false)
    {
        TakeDamage(amount, headshot);
        particle.transform.SetPositionAndRotation(loc, rot);
        particle.Play();
    }

    IEnumerator DamageTakenFlash()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        model.material.color = colorOrig;
    }

    void PlayHitMarkerSound()
    {
        if (audioPlayer != null && audioHitMarker != null)
        {
            audioPlayer.PlayOneShot(audioHitMarker);
        }
    }

    void OnDeath()
    {
        gameManager.instance.UpdateGameGoal();
        animator.Play("Z_FallingBack");
        isDead = true;
        agent.isStopped = true;

        Destroy(GetComponentInChildren<CapsuleCollider>());
    }

    void AfterDeath()
    {
        Destroy(gameObject, 0.5f);
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    IEnumerator DoAttack()
    {
        canAttack = false;
        animator.Play("Z_Attack");
        yield return new WaitForSeconds(attackRate);
        canAttack = true;
    }

    IEnumerator Shoot()
    {
        isAttacking = true;
        animator.Play("Z_Attack");
        //Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(attackRate);
        isAttacking = false;
    }

    void RightAttackEnable()
    {
        rightHandPos.SetActive(true);
    }

    void LeftAttackEndable()
    {
        leftHandPos.SetActive(true);
    }
}
