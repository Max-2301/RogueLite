using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Gun : WeaponBase
{
    private int maxAmmo;
    private int ammo;
    private int maxMagazineAmmo;
    private int magazineAmmo;
    private float reloadTime;
    private GameObject bullet;
    [SerializeField] private Transform bulletSpawnPoint;
    private bool reloading = false;
    private AudioClip reloadAudio;
    [SerializeField] private AudioSource reloadAudioSource;

    private float upgradeReloadMultiplier = 1;

    protected override void UseWeapon()
    {
        if (weapon == null) return;
        if (magazineAmmo <= 0 && !reloading)
        {
            magazineAmmo = 0;
            StartCoroutine(Reload(1));
        }
        else
        {
            GameObject b = Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation, null);
            Bullet scrB = b.GetComponent<Bullet>();
            scrB.SetPlayer(player);
            scrB.SetDeleteTime(range);
            scrB.SetDamage(dmg);
            rb.AddForce(-transform.right * knockBack, ForceMode2D.Impulse);
            magazineAmmo--;
            inventory.SetGunText(magazineAmmo, maxMagazineAmmo, ammo);
            weaponAudioSource.Play();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        inventory.onUpgradeCollected += FastReload;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        inventory.onUpgradeCollected -= FastReload;
    }

    public IEnumerator Reload(float multiplier)
    {
        if (magazineAmmo >= maxMagazineAmmo || ammo <= 0) yield break;
        reloading = true;
        ammo += magazineAmmo;
        magazineAmmo = 0;
        reloadAudioSource.Play();

        int i = inventory.HasUpgrade(UpgradeScritbaleObject.UpgradeType.reloadSpeed);
        if (i >= 0) upgradeReloadMultiplier = inventory.GetUpgrade(i).ammount;

        yield return new WaitForSeconds(reloadTime * multiplier / upgradeReloadMultiplier);
        reloadAudioSource.Stop();
        if (ammo - maxMagazineAmmo >= 0)
        {
            ammo -= maxMagazineAmmo;
            magazineAmmo = maxMagazineAmmo;
        }
        else if (ammo > 0)
        {
            magazineAmmo = ammo;
            ammo = 0;
        }
        else
        {
            #if UNITY_EDITOR
            //out of ammo
            Debug.Log("out of ammo");
            #endif
        }
        inventory.SetGunText(magazineAmmo, maxMagazineAmmo, ammo);
        reloading = false;
    }

    public override void ChangeWeapon(WeaponScriptableObject w)
    {
        base.ChangeWeapon(w);
        maxAmmo = weapon.maxAmmo;
        ammo = maxAmmo;
        maxMagazineAmmo = weapon.maxMagazineAmmo;
        magazineAmmo = maxMagazineAmmo;
        reloadTime = weapon.reloadTime;
        bullet = weapon.spawnable;
        reloadAudio = weapon.reloadAudio;
        reloadAudioSource.clip = reloadAudio;
    }

    public bool GetReloading()
    {
        return reloading;
    }

    public float GetReloadTime()
    {
        return reloadTime;
    }

    public void FastReload(UpgradeScritbaleObject.UpgradeType type, int ammount)
    {
        if (type == UpgradeScritbaleObject.UpgradeType.reloadSpeed)
        {
            upgradeReloadMultiplier = ammount;
            Debug.Log("reloadUpgrade");
        }
    }

    public override void HideAndShow(bool dead)
    {
        base.HideAndShow(dead);
        weapon = null;
        spriteRenderer.sprite = null;
        upgradeReloadMultiplier = 1;
    }
}
