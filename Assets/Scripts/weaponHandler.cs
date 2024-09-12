using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class weaponHandler : MonoBehaviour
{
    [Header("Weapon Components")]
    [SerializeField] Transform firePos;
    [SerializeField] GameObject bullet;
    [SerializeField] ParticleSystem partSystem;
    [SerializeField] AudioSource audioSystem;
    [SerializeField] AudioClip audioReload;
    [SerializeField] AudioClip audioEmpty;

    [Header("Weapon Stats")]
    [SerializeField] int damage;
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] int magazineSize = 35;
    [SerializeField] float reloadRate = 3.0f;

    public float GetFireRate() { return fireRate; }
    public int GetCurrentMagazine() { return magazineCurrent; }
    public int GetMagazineSize() { return magazineSize; }

    int magazineCurrent = 0;

    bool isReloading;
    bool isEmptyMagazineSound;

    void Start()
    {
        magazineCurrent = magazineSize;
    }

    public void Fire()
    {
        if (isReloading){ return; }

        if(!HasAmmo())
        {
            if(!isEmptyMagazineSound)
                StartCoroutine(EmptyMagazine());
            return;
        }


        Vector3 ScreenCentreCoordinates = new Vector3(0.5f, 0.5f, 0f);
        Ray ray = Camera.main.ViewportPointToRay(ScreenCentreCoordinates);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.TakeDamage(damage, hit.point + hit.normal * 0.001f, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            }
            else
            {
                GameObject bulletDecal = Instantiate(bullet, hit.point + hit.normal * 0.001f, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                Destroy(bulletDecal, 3.0f);
            }
        }

        //Instantiate(bullet, firePos.position, firePos.rotation);
        partSystem.Play();
        audioSystem.PlayOneShot(audioSystem.clip);

        magazineCurrent -= 1;
    }

    public bool HasAmmo()
    {
        return magazineCurrent > 0;
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    public void DoReload()
    {
        if (!isReloading)
            StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        isReloading = true;
        audioSystem.PlayOneShot(audioReload);
        
        yield return new WaitForSeconds(audioReload.length * 1.2f);

        isReloading = false;

        magazineCurrent = magazineSize;
        gameManager.instance.GetPlayerInterface().UpdatePlayerAmmo(magazineCurrent.ToString(), magazineSize.ToString());
    }

    IEnumerator EmptyMagazine()
    {
        isEmptyMagazineSound = true;
        audioSystem.PlayOneShot(audioEmpty);
        yield return new WaitForSeconds(audioEmpty.length);

        isEmptyMagazineSound = false;
    }
}
