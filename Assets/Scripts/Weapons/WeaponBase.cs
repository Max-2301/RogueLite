using JetBrains.Annotations;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IUsable
{
    [SerializeField] protected WeaponScriptableObject weapon;
    private string weaponName;
    protected int dmg;
    protected float knockBack;
    protected float range;
    private Sprite sprite;
    protected float useTime = 0;
    private AudioClip weaponAudio;
    [SerializeField]protected AudioSource weaponAudioSource;
    protected SpriteRenderer spriteRenderer;

    protected float useTimeMultipler = 1;
    protected bool used = false;

    [Tooltip("To tell the game who used this attack and award accoundingly"), SerializeField] protected GameObject player;
    protected Rigidbody2D rb;
    [SerializeField] protected Inventory inventory;

    [SerializeField] protected PlayerHealth playerHealth;

    [SerializeField] private PlayerController playerController;
    public virtual IEnumerator Use()
    {
        if (used || weapon == null || playerController.GetMovementStatus() == PlayerController.MovementStatus.dead) yield break;
        used = true;
        UseWeapon();
        yield return new WaitForSeconds(useTime / useTimeMultipler);
        used = false;
    }
    
    protected abstract void UseWeapon();
    
    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = player.GetComponent<Rigidbody2D>();
        if (weapon != null) ChangeWeapon(weapon);
    }

    protected virtual void OnEnable()
    {
        inventory.onUpgradeCollected += DoublePower;
        playerHealth.OnPlayerDeath += HideAndShow;
    }

    protected virtual void OnDisable()
    {
        inventory.onUpgradeCollected -= DoublePower;
        playerHealth.OnPlayerDeath -= HideAndShow;
    }

    public virtual void ChangeWeapon(WeaponScriptableObject w)
    {
        weapon = w;
        weaponName = weapon.weaponName;
        useTime = weapon.useTime;
        dmg = weapon.dmg;
        range = weapon.range;
        sprite = weapon.sprite;
        weaponAudio = weapon.weaponUseAudio;
        weaponAudioSource.clip = weaponAudio;
        knockBack = weapon.knockBack;
        spriteRenderer.sprite = sprite;
    }

    public bool GetUsed() {
        return used;
    }

    public void DoublePower(UpgradeScritbaleObject.UpgradeType type, int ammount)
    {
        if (type == UpgradeScritbaleObject.UpgradeType.attackSpeed) useTimeMultipler = ammount;
    }

    public virtual void HideAndShow(bool dead)
    {
        Debug.Log(dead);
        if (dead) { spriteRenderer.enabled = false; useTimeMultipler = 1; }
        else { spriteRenderer.enabled = true; return; }
    }
}
