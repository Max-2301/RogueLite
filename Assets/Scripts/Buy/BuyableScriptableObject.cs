using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Buyable", menuName = "Scriptable Objects/Buyable")]
public class BuyableScritableObject : ScriptableObject
{

    public string buyableName;
    public int cost;
    public Sprite sprite;
    public enum BuyableType
    {
        weapon,
        trap,
        upgrade,
        door
    }
    public BuyableType buyableType;
    //weapon
    [HideInInspector]
    public WeaponScriptableObject weapon;
    [HideInInspector]
    public GameObject trap;
    [HideInInspector]
    public UpgradeScritbaleObject upgrade;
    [HideInInspector] 
    public Color upgradeColor = Color.red;
    [HideInInspector] 
    public GameObject door;
}
#if UNITY_EDITOR
[CustomEditor(typeof(BuyableScritableObject))]
public class BuyableScript_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields

        BuyableScritableObject script = (BuyableScritableObject)target;

        //enable weapon scrObj
        script.buyableType = (BuyableScritableObject.BuyableType)EditorGUILayout.IntField("BuyableType", (int)script.buyableType);
        if (script.buyableType == BuyableScritableObject.BuyableType.weapon) // if bool is true, show other fields
        {
            script.weapon = EditorGUILayout.ObjectField("Weapon", script.weapon, typeof(WeaponScriptableObject), true) as WeaponScriptableObject;
        }
        else if (script.buyableType == BuyableScritableObject.BuyableType.trap)
        {
            script.trap = EditorGUILayout.ObjectField("Trap", script.trap, typeof(GameObject), true) as GameObject;
        }
        else if (script.buyableType == BuyableScritableObject.BuyableType.upgrade)
        {
            script.upgrade = EditorGUILayout.ObjectField("Upgrade", script.upgrade, typeof(UpgradeScritbaleObject), true) as UpgradeScritbaleObject;
            script.upgradeColor = EditorGUILayout.ColorField("Upgrade Color", script.upgradeColor);
        }
        else if (script.buyableType == BuyableScritableObject.BuyableType.door)
        {
            script.door = EditorGUILayout.ObjectField("Door", script.door, typeof(GameObject), true) as GameObject;
        }
        EditorUtility.SetDirty(target);
    }
}
#endif