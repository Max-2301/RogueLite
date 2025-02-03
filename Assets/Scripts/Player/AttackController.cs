using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Inventory))]
public class AttackController : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerController playerController;
    private Inventory inventory;

    [SerializeField] private InputActionReference meleeRef, gunRef, explosiveRef, reloadRef;
    private InputAction melee, gun, explosive, reload;

    private GameObject meleeObj, gunObj, explosiveObj;
    private enum WeaponStatus
    {
        none,
        usingMelee,
        usingGun,
        usingExplosive
    }
    private WeaponStatus weaponStatus = WeaponStatus.none;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        melee = playerInput.actions[meleeRef.name];
        gun = playerInput.actions[gunRef.name];
        explosive = playerInput.actions[explosiveRef.name];
        reload = playerInput.actions[reloadRef.name];
        playerController = GetComponent<PlayerController>();
    }
    private void Start()
    {
        inventory = GetComponent<Inventory>();
        meleeObj = inventory.GetWeapon(0);
        gunObj = inventory.GetWeapon(1);
        //explosiveObj = inventory.GetWeapon(2);
    }
    private void Update()
    {
        UpdateWeaponStatus();
    }
    private void UpdateWeaponStatus()
    {
        PlayerController.MovementStatus status = playerController.GetMovementStatus();
        if (!meleeObj.GetComponent<IUsable>().GetUsed() && !gunObj.GetComponent<IUsable>().GetUsed() )
        {
            if (status == PlayerController.MovementStatus.dead || gunObj.GetComponent<Gun>().GetReloading())
            {
                weaponStatus = WeaponStatus.none;
                return;
            }
            else if (melee.ReadValue<float>() > 0)
            {
                weaponStatus = WeaponStatus.usingMelee;
            }
            else if (status != PlayerController.MovementStatus.running && gun.ReadValue<float>() > 0)
            {
                weaponStatus = WeaponStatus.usingGun;
            }
            //else if (explosive.ReadValue<float>() > 0)
            //{
            //    weaponStatus = WeaponStatus.usingExplosive;
            //}
            else if (reload.ReadValue<float>() > 0)
            {
                StartCoroutine(gunObj.GetComponent<Gun>().Reload(0.5f));
                return;
            }
        }
        else
        {
            weaponStatus = WeaponStatus.none;
        }
        UseWeapons();
    }
    private void UseWeapons()
    {
        switch (weaponStatus)
        {
            case WeaponStatus.none:
                return;
            case WeaponStatus.usingMelee:
                StartCoroutine(meleeObj.GetComponent<IUsable>()?.Use());
                break;
            case WeaponStatus.usingGun:
                StartCoroutine(gunObj.GetComponent<IUsable>()?.Use());
                break;
            case WeaponStatus.usingExplosive:
                StartCoroutine(explosiveObj.GetComponent<IUsable>()?.Use());
                break;
            default:
                break;
        }
    }
}
