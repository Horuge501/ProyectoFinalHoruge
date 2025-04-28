using UnityEngine;
using UnityEngine.InputSystem;

public enum AttackMode {
    PHYSICAL,
    MAGIC
}

[System.Serializable]
public struct Hitboxes {
    public GameObject lightPhysicalAttack;
    public GameObject heavyPhysicalAttack;
    public GameObject lightMagicAttack;
    public GameObject heavyMagicAttack;
}

public class MainCharacter : MonoBehaviour
{
    [SerializeField] protected AttackMode currentAttackMode;
    [SerializeField] public Hitboxes hitboxes;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    private Rigidbody rb;
    private Vector2 moveInput;

    [SerializeField] protected GameObject currentLightAttack;
    [SerializeField] protected GameObject currentHeavyAttack;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        Vector3 newVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        rb.linearVelocity = newVelocity;

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context) {
        if (context.performed) {
            moveSpeed = moveSpeed * 2f;
        }
        if (context.canceled) {
            moveSpeed = moveSpeed / 2f;
        }
    }
}
