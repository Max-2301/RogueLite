using UnityEngine;

public abstract class BuyableBase : MonoBehaviour, IBuyable
{
    [SerializeField]protected BuyableScritableObject buyable;
    private string buyName;
    protected int cost;
    protected Sprite sprite;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected AudioSource audioSource;
    protected virtual void Start()
    {
        buyName = buyable.name;
        cost = buyable.cost;
        sprite = buyable.sprite;
        spriteRenderer.sprite = sprite;
    }
    public abstract ScriptableObject Buy();
    public virtual int GetCost(Inventory inv)
    {
        return cost;
    }
    public Sprite GetSprite()
    {
        return sprite;
    }
    public string GetName()
    {
        return buyName;
    }
}