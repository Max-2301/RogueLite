using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class Inventory : MonoBehaviour
{
    private Inventory inventory;

    [Tooltip("the object which holds the weapon type")]
    [SerializeField] private GameObject[] weaponObjects = new GameObject[3];
    [Tooltip("0 melee | 1 gun | 2 explosive")]
    [SerializeField] private WeaponScriptableObject[] activeWeapons = new WeaponScriptableObject[3];
    [SerializeField, Tooltip("Must be melee")] private BuyableScritableObject startingWeapon;

    [SerializeField] private List<UpgradeScritbaleObject> upgrades = new();
    private List<Image> upgradesUI = new();
    [SerializeField] private int maxUpgradesAmmount = 3;

    private PlayerInput playerInput;
    [SerializeField] private InputActionReference buyRef;
    private InputAction buy;

    [SerializeField] private int coins = 0;

    private TextMeshProUGUI buyText, gunText, coinText;
    private Image buyImage, gunImage, meleeImage;

    public delegate void UpgradeCollected(UpgradeScritbaleObject.UpgradeType type, int ammount);
    public UpgradeCollected onUpgradeCollected;

    private PlayerHealth playerHealth;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerHealth = GetComponent<PlayerHealth>();
        buy = playerInput.actions[buyRef.name];
        buy.started += CheckBuy;
    }

    private void Start()
    {
        inventory = this;
        activeWeapons[0] = startingWeapon.weapon;
    }

    private void OnEnable()
    {
        playerHealth.OnPlayerDeath += ResetAllItems;
    }
    private void OnDisable()
    {
        playerHealth.OnPlayerDeath -= ResetAllItems;
    }

    public void CheckBuy(InputAction.CallbackContext context)
    {
        List<Collider2D> colliders = new();
        Physics2D.OverlapCollider(GetComponent<Collider2D>(), colliders);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent<IBuyable>(out var ibuy))
            {
                int cost = ibuy.GetCost(inventory);
                if (coins - cost >= 0)
                {
                    ScriptableObject itemBought = ibuy.Buy();
                    if (itemBought is WeaponScriptableObject)
                    {
                        WeaponScriptableObject weapon = itemBought as WeaponScriptableObject;
                        WeaponScriptableObject.WeaponType type = weapon.weaponType;
                        int weaponIndex = 0;
                        switch (type)
                        {
                            case WeaponScriptableObject.WeaponType.melee:
                                weaponIndex = 0;
                                meleeImage.sprite = ibuy.GetSprite();
                                break;
                            case WeaponScriptableObject.WeaponType.gun:
                                weaponIndex = 1;
                                gunImage.sprite = ibuy.GetSprite();
                                gunImage.enabled = true;
                                break;
                            case WeaponScriptableObject.WeaponType.explosive:
                                weaponIndex = 2;
                                break;
                        }
                        weaponObjects[weaponIndex].GetComponent<WeaponBase>().ChangeWeapon(weapon);
                        activeWeapons[weaponIndex] = weapon;
                        SetGunText(weapon.maxMagazineAmmo, weapon.maxMagazineAmmo, weapon.maxAmmo);
                        buyText.text = ibuy.GetCost(inventory).ToString();
                    }
                    else if (itemBought is UpgradeScritbaleObject)
                    {
                        UpgradeScritbaleObject upgrade = itemBought as UpgradeScritbaleObject;
                        if (upgrades.Contains(upgrade) || upgrades.Count >= maxUpgradesAmmount) return;
                        else 
                        { 
                            upgrades.Add(upgrade);
                            upgradesUI[upgrades.IndexOf(upgrade)].sprite = upgrade.sprite;
                            upgradesUI[upgrades.IndexOf(upgrade)].enabled = true;
                            onUpgradeCollected.Invoke(upgrade.type, upgrade.ammount);
                        }
                    }
                    coins -= cost;
                    coinText.text = coins.ToString();
                }
            }
        }
    }

    public GameObject GetWeapon(int index)
    {
        return weaponObjects[index];
    }

    public WeaponScriptableObject GetActiveWeapon(int index)
    {
        return activeWeapons[index];
    }

    public int HasUpgrade(UpgradeScritbaleObject.UpgradeType type)
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (upgrades[i].type == type) return i;
        }
        return -1;
    }

    public UpgradeScritbaleObject GetUpgrade(int i)
    {
        return upgrades[i];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IBuyable>(out var ibuy))
        {
            buyText.text = ibuy.GetCost(inventory).ToString();
            buyImage.sprite = ibuy.GetSprite();
            buyImage.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<IBuyable>() != null)
        {
            buyText.text = "";
            buyImage.sprite = null;
            buyImage.enabled = false;
        }
    }

    public void AddCoins(int ammount)
    {
        coins += ammount;
        coinText.text = coins.ToString();
    }

    public void SetUI(TextMeshProUGUI buytext, Image buyimg, TextMeshProUGUI guntext, Image gunimg, Image meleeimg, List<Image> upgradelist, TextMeshProUGUI cointext)
    {
        buyText = buytext;
        gunText = guntext;
        buyImage = buyimg;
        gunImage = gunimg;
        meleeImage = meleeimg;
        upgradesUI = upgradelist;
        coinText = cointext;
        cointext.text = coins.ToString();
    }

    public void SetGunText(int magAmmo, int maxMagAmmo, int ammo)
    {
        gunText.text = magAmmo.ToString() + "/" + maxMagAmmo.ToString() + "<br>" + ammo.ToString();
    }

    public void ResetAllItems(bool dead)
    {
        if (dead)
        {
            activeWeapons[0] = startingWeapon.weapon;
            weaponObjects[0].GetComponent<WeaponBase>().ChangeWeapon(activeWeapons[0]);
            activeWeapons[1] = null;
            activeWeapons[2] = null;
            meleeImage.sprite = startingWeapon.sprite; 
            gunImage.sprite = null;
            gunImage.enabled = false;
            gunText.text = null;
            upgrades.Clear();
            for (int i = 0; i < upgradesUI.Count; i++)
            {
                upgradesUI[i].sprite = null;
                upgradesUI[i].enabled = false;
            }
        }
    }
}
