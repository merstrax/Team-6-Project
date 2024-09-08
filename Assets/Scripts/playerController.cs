using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Player logic variables
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

    //Player Movement
    [SerializeField] float speed;
    [SerializeField] float sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;

    //Player Shoot
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] float shootDistance;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;

    bool isSprinting;
    bool isShooting;

    // Start is called before the first frame update
    void Start()
    {

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
        }

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Shoot") && !isShooting)
        {
            StartCoroutine(Shoot());
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

    IEnumerator Shoot()
    {
        isShooting = true;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, shootDistance, ~ignoreMask))
        {
            //IDamage dmg = hit.collider.GetComponent<IDamage>();
            //dmg?.TakeDamage(shootDamage);
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
}
