using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class weaponHandler : MonoBehaviour
{
    [Header("Weapon Info")]
    [SerializeField] string weaponName;
    [SerializeField] int weaponCost;

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
    [SerializeField] float reloadRate = 3.0f;
    [SerializeField] int magazineSize = 35;
    [SerializeField] int ammoMax;
    [SerializeField] bool isAutomatic = false;
    [SerializeField] bool canPenetrate = false;

    public string GetWeaponName() { return weaponName; }
    public int GetWeaponCost() {  return weaponCost; }

    public float GetFireRate() { return fireRate; }
    public int GetCurrentMagazine() { return magazineCurrent; }
    public int GetMagazineSize() { return magazineSize; }

    public bool IsAutomatic() { return isAutomatic; }

    int magazineCurrent;
    int ammoCurrent;

    bool isReloading;
    bool isEmptyMagazineSound;

    AudioClip audioShot;

    void Start()
    {
        magazineCurrent = magazineSize;
        ammoCurrent = ammoMax;
        audioShot = audioSystem.clip;
    }

    public void Fire()
    {
        if (isReloading || gameManager.instance.isPaused){ return; }

        if(!HasAmmo())
        {
            if(!isEmptyMagazineSound)
                StartCoroutine(EmptyMagazine());
            return;
        }

        Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);
        Ray ray = Camera.main.ViewportPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            IDamage dmg = hit.collider.gameObject.GetComponentInParent<IDamage>();

            if (dmg != null)
            {
                bool _headShot = hit.collider.CompareTag("Head");
                dmg.TakeDamage(damage, hit.point + hit.normal * 0.001f, Quaternion.FromToRotation(Vector3.forward, hit.normal), _headShot);
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

        UpdateUI();
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
        if (!isReloading && magazineCurrent != magazineSize && ammoCurrent > 0)
            StartCoroutine(Reload());
    }

    public void Resupply(int amount = 0)
    {
        if (amount == 0)
            ammoCurrent = ammoMax;
        else
            ammoCurrent = Mathf.Min(ammoCurrent + amount, ammoMax);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        audioSystem.clip = audioReload;
        audioSystem.loop = true;
        audioSystem.Play();
        
        yield return new WaitForSeconds(reloadRate);

        audioSystem.Stop();
        audioSystem.clip = audioShot;
        audioSystem.loop = false;
        
        isReloading = false;

        if ((magazineSize - magazineCurrent) <= ammoCurrent) 
        {
            ammoCurrent -= (magazineSize - magazineCurrent);
            magazineCurrent = magazineSize;
        }
        else
        {
            magazineCurrent += ammoCurrent;
            ammoCurrent = 0;
        }

        UpdateUI();
    }

    IEnumerator EmptyMagazine()
    {
        isEmptyMagazineSound = true;
        audioSystem.PlayOneShot(audioEmpty);
        yield return new WaitForSeconds(audioEmpty.length);

        isEmptyMagazineSound = false;
    }

    public int GetCurrentAmmo() { return ammoCurrent; }

    public void SetAmmoToMax()
    {
        ammoCurrent = ammoMax;
        UpdateUI(); 
    }

    public void UpdateUI()
    {
        gameManager.instance.GetPlayerInterface().UpdatePlayerAmmo(magazineCurrent.ToString(), ammoCurrent.ToString());
    }
}
