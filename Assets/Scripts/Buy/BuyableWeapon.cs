using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class BuyableWeapon : BuyableBase
{
    WeaponScriptableObject weaponSrc;
    protected override void Start()
    {
        base.Start();
        weaponSrc = buyable.weapon;
        spriteRenderer.sprite = weaponSrc.sprite;
    }
    public override ScriptableObject Buy()
    {
        audioSource.Play();
        return weaponSrc;
    }
    public override int GetCost(Inventory inv)
    {
        if (weaponSrc == inv.GetActiveWeapon(1))
        {
            return cost / 2;
        }
        else if (weaponSrc == inv.GetActiveWeapon(0)) return 0;
        return cost;
    }
}
