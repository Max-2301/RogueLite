using UnityEngine;

public class BuyableUpgrade : BuyableBase
{
    private UpgradeScritbaleObject upgradeScr;
    protected override void Start()
    {
        base.Start();
        upgradeScr = buyable.upgrade;
        GetComponent<SpriteRenderer>().color = buyable.upgradeColor;
    }
    public override ScriptableObject Buy()
    {
        return upgradeScr;
    }
    public override int GetCost(Inventory inv)
    {
        int ammount = (inv.HasUpgrade(upgradeScr.type) < 0) ? cost : 0;
        return ammount;
    }
}
