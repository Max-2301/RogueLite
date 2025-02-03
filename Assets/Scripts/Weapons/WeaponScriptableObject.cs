using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "new Weapon", menuName = "Scriptable Objects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    [Header("All types")]
    public string weaponName;
    public enum WeaponType
    {
        melee,
        explosive,
        gun
    }
    public WeaponType weaponType;
    [Tooltip("ShotgunBullet gives 6 pallets each dealing the dmg")]
    public int dmg;
    public float knockBack;
    [Tooltip("Melee/Explosive: range for aoe | Gun: Deletetime for bullets")]
    public float range;
    public Sprite sprite;
    public AudioClip weaponUseAudio;

    [Tooltip("Melee: Time it takes to attack again | Weapon: Time between each shot")]
    public float useTime;
    [Header("Gun/Explosive")]
    [HideInInspector]
    public GameObject spawnable;
    [HideInInspector]
    public int maxAmmo;
    [Header("Gun")]
    [HideInInspector]
    public int maxMagazineAmmo;
    [HideInInspector]
    public float reloadTime;
    [HideInInspector]
    public AudioClip reloadAudio;
}
#if UNITY_EDITOR
[CustomEditor(typeof(WeaponScriptableObject))]
public class WeaponScript_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields

        WeaponScriptableObject script = (WeaponScriptableObject)target;

        //enable weapon scrObj
        script.weaponType = (WeaponScriptableObject.WeaponType)EditorGUILayout.IntField("WeaponType", (int)script.weaponType);
        if (script.weaponType == WeaponScriptableObject.WeaponType.explosive || script.weaponType == WeaponScriptableObject.WeaponType.gun) // if bool is true, show other fields
        {
            script.spawnable = EditorGUILayout.ObjectField("Spawnable", script.spawnable, typeof(GameObject), true) as GameObject;
            script.maxAmmo = EditorGUILayout.IntField("MaxAmmo", script.maxAmmo);

        }
        if (script.weaponType == WeaponScriptableObject.WeaponType.gun)
        {
            script.maxMagazineAmmo = EditorGUILayout.IntField("MaxMagazineAmmo", script.maxMagazineAmmo);
            script.reloadTime = EditorGUILayout.FloatField("ReloadTime", script.reloadTime);
            script.reloadAudio = EditorGUILayout.ObjectField("ReloadAudio", script.reloadAudio, typeof(AudioClip), true) as AudioClip;
        }
    }
}
#endif
