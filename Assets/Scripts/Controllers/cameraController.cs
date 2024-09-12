using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    [SerializeField] int fovNormal;
    [SerializeField] int fovADS;

    [SerializeField] Transform weaponPos;
    [SerializeField] Transform weaponPosADS;
    [SerializeField] Transform weaponPosNorm;

    float rotX;

    // Start is called before the first frame update
    void Start()
    {
        //Setup cursor logic to be visisble and locked
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Get input
        float recoilX = 0.0f;
        float recoilY = 0.0f;

        if(gameManager.instance.playerScript.IsShooting())
        {
            recoilX = Random.Range(-0.07f, 0.07f);
            recoilY = Random.Range(-0.01f, 0.1f);
        }

        if (Input.GetButton("Aim"))
        {
            Camera.main.fieldOfView = fovADS;
            weaponPos.localPosition = weaponPosADS.localPosition;
            recoilX *= 0.5f;
            recoilY *= 0.5f;
        }

        if(Input.GetButtonUp("Aim"))
        {
            Camera.main.fieldOfView = fovNormal;
            weaponPos.localPosition = weaponPosNorm.localPosition;
        }

        float mouseY = (Input.GetAxis("Mouse Y") + recoilY) * sensitivity * Time.deltaTime;
        float mouseX = (Input.GetAxis("Mouse X") + recoilX) * sensitivity * Time.deltaTime;

        //Invert Y camera
        if (invertY)
        {
            rotX += mouseY;
        }
        else
            rotX -= mouseY;

        //Clamp the rotX on the x-axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        //Rotate the camera on the x-axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        //Rotate the player on the y-axis
        transform.parent.Rotate(Vector3.up * mouseX); 
    }
}
