using UnityEngine;

public interface IBuyable
{
    ScriptableObject Buy();
    int GetCost(Inventory inv);
    Sprite GetSprite();
}
