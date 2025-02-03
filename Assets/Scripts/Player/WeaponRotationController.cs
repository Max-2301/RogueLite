using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerInput))]
public class WeaponRotationController : MonoBehaviour
{
    PlayerInput playerInput;
    [SerializeField] private float rotationSpeed;
    [SerializeField]
    private InputActionReference lookRef;
    private InputAction look;
    [SerializeField] private GameObject rotatingObject;

    [SerializeField] private InputDevice currentControl;
    private PlayerController controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        look = playerInput.actions[lookRef.name];
    }
    void Start()
    {
        controller = GetComponent<PlayerController>();
    }
    private void OnEnable()
    {
        look.performed += CheckActiveControl;
    }
    private void OnDisable()
    {
        look.performed -= CheckActiveControl;
    }
    // Update is called once per frame
    void Update()
    {
        if (controller.GetMovementStatus() == PlayerController.MovementStatus.running || rotatingObject.GetComponentInChildren<Gun>().GetReloading())
        {
            RotateGun(Quaternion.Euler(0,0,300));
        }
        else if (currentControl is Gamepad)
        {
            if (look.ReadValue<Vector2>() != Vector2.zero)
            {
                ControllerRotation();
            }
        }
        else if (currentControl is Mouse)
        {
            MouseRotation();
        }
    }
    private void MouseRotation()
    {
        Vector2 mousePos = look.ReadValue<Vector2>();

        Vector3 objectPos = Camera.main.WorldToScreenPoint(rotatingObject.transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;
        RotateGun(CalculateRotation(mousePos.x, mousePos.y));
    }
    private void ControllerRotation()
    {
        Vector2 stickRotation = look.ReadValue<Vector2>();
        RotateGun(CalculateRotation(stickRotation.x, stickRotation.y));
    }
    public void CheckActiveControl(InputAction.CallbackContext context)
    {
        if (context.performed) currentControl = context.control.device;
    }
    /// <summary>
    /// Calculate rotation 
    /// </summary>
    /// <param name="x">x pos</param>
    /// <param name="y">y pos</param>
    /// <returns></returns>
    private quaternion CalculateRotation(float x, float y)
    {
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        return rotation;
    }
    private void RotateGun(quaternion rotation)
    {
        rotatingObject.transform.rotation = Quaternion.Slerp(rotatingObject.transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        //flip sprite when gun is either above 90 or -90
        if (rotatingObject.transform.eulerAngles.z > 90 && rotatingObject.transform.eulerAngles.z < 270)
        {
            rotatingObject.GetComponentInChildren<SpriteRenderer>().flipY = true;
        }
        else
        {
            rotatingObject.GetComponentInChildren<SpriteRenderer>().flipY = false;
        }
    }
}
