using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private int startMaxHealth = 2;

    private Inventory inventory;

    bool invincible = false;
    [SerializeField] float invincibleTime = 0.5f;
    private int lasResortInvincibleTime = -1;
    private bool lastResortUsed = true;

    public delegate void PlayerDeath(bool deads);
    public event PlayerDeath OnPlayerDeath;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Color damagedColor;

    private SetHearts heartsUI;

    private Coroutine healthCor;

    bool dead = false;

    [SerializeField] private AudioSource audioSource;
    private void Awake()
    {
        maxHealth = startMaxHealth;
        health = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        inventory = GetComponent<Inventory>();
        inventory.onUpgradeCollected += LastResort;
        inventory.onUpgradeCollected += AddHealth;
    }

    private void OnDisable()
    {
        inventory.onUpgradeCollected -= LastResort;
        inventory.onUpgradeCollected -= AddHealth;
    }

    public void TakeDamage(int damage, GameObject hit)
    {
        if (!invincible)
        {
            health -= damage;
            if (healthCor != null) StopCoroutine(healthCor);
            if (health <= 0)
            {
                if (!lastResortUsed) 
                { 
                    StartCoroutine(Invincible(lasResortInvincibleTime)); 
                    health = 1; 
                    lastResortUsed = true; 
                }
                else Death();
            }
            else StartCoroutine(Invincible(invincibleTime));
            heartsUI.ActivateHearts(health);
        }
    }

    private IEnumerator Invincible(float time)
    {
        invincible = true;
        for (int i = 0; i < 4; i++)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
            yield return new WaitForSeconds(time/8);
            SetColorDamaged((((float)health / (float)maxHealth)));
            yield return new WaitForSeconds(time/8);
        }
        invincible = false;
        healthCor = StartCoroutine(RegainHealth());
    }

    private IEnumerator RegainHealth()
    {
        yield return new WaitForSeconds(5);
        while (health < maxHealth && !invincible)
        {
            yield return new WaitForSeconds(0.5f);
            health++;
            SetColorDamaged(((float)health / (float)maxHealth));
            heartsUI.ActivateHearts(health);
        }
    }
    private void Death()
    {
        OnPlayerDeath.Invoke(true);
        audioSource.Play();
        dead = true;
        maxHealth = startMaxHealth;
        lastResortUsed = true;
        lasResortInvincibleTime = -1;
        spriteRenderer.color = Color.white;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Boundarie>().enabled = false;
    }

    public void Revive()
    {
        dead = false;
        OnPlayerDeath.Invoke(false);
        health = 1;
        StartCoroutine(RegainHealth());
        GetComponent<Collider2D>().enabled = true;
        GetComponent<Boundarie>().enabled = true;
    }

    public void AddHealth(UpgradeScritbaleObject.UpgradeType type, int ammount)
    {
        if (type == UpgradeScritbaleObject.UpgradeType.addHealth) { maxHealth += ammount; health = maxHealth; }
        SetColorDamaged((((float)health / (float)maxHealth)));
        heartsUI.SetHeartsImg(maxHealth);
    }

    public void LastResort(UpgradeScritbaleObject.UpgradeType type, int ammount)
    {
        if (type == UpgradeScritbaleObject.UpgradeType.invincibiltyOnDeath) { lasResortInvincibleTime = ammount; lastResortUsed = false; }
    }

    private void SetColorDamaged(float ammount)
    {
        spriteRenderer.color = Color.Lerp(damagedColor, Color.white, ammount);
    }

    public void SetUI(SetHearts heartsSrc)
    {
        Debug.Log("hearts");
        heartsUI = heartsSrc;
        heartsUI.SetHeartsImg(maxHealth);
    }

    public bool GetDeath()
    {
        return dead;
    }
}
