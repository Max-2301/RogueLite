using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]
public class UpgradeScritbaleObject : ScriptableObject
{
    public string description;
    public Sprite sprite;
    public enum UpgradeType
    {
        attackSpeed, addHealth, reloadSpeed, invincibiltyOnDeath
    }
    public UpgradeType type;
    [Tooltip("The ammount of benefit it gives to the certain upgrade")]public int ammount;
}
