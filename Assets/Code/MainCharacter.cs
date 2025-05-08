using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

[System.Serializable]
public struct Cooldowns
{
    public float lightCooldown;
    public float heavyCooldown;
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

    [SerializeField] public Cooldowns attackCooldowns;
    public bool attackEnabled;

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

    public void LightAttacking() {
        switch (currentAttackMode) {
            case AttackMode.PHYSICAL:
                PerfomingAttack(hitboxes.lightPhysicalAttack, attackCooldowns.lightCooldown);
                break;
            case AttackMode.MAGIC:
                PerfomingAttack(hitboxes.lightMagicAttack, attackCooldowns.lightCooldown);
                break;
        }
    }

    public void HeavyAttacking() {
        switch (currentAttackMode) {
            case AttackMode.PHYSICAL:
                PerfomingAttack(hitboxes.heavyPhysicalAttack, attackCooldowns.heavyCooldown);
                break;
            case AttackMode.MAGIC:
                PerfomingAttack(hitboxes.heavyMagicAttack, attackCooldowns.heavyCooldown);
                break;
        }
    }

    private void PerfomingAttack(GameObject hitbox, float cooldown)
    {
        hitbox.SetActive(true);
        attackEnabled = false;
        StartCoroutine(WaitForCooldown(hitbox, cooldown));
    }

    IEnumerator WaitForCooldown(GameObject hitbox,float timeToWait)
    {
        yield return new WaitForSeconds(0.1f);
        //yield return null;
        hitbox.SetActive(false);
        yield return new WaitForSeconds(timeToWait);
        attackEnabled = true;
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

    public void OnLightAttack(InputAction.CallbackContext context) {
        if (context.performed && attackEnabled) {
            LightAttacking();
        }
    }

    public void OnHeavyAttack(InputAction.CallbackContext context) {
        if (context.performed && attackEnabled) {
            HeavyAttacking();
        }
    }

    public void OnChangeAttackMode(InputAction.CallbackContext context) {
        if (context.performed) {
            currentAttackMode = (currentAttackMode == AttackMode.PHYSICAL) ? AttackMode.MAGIC : AttackMode.PHYSICAL;

            Debug.Log("Current Mode: " + currentAttackMode);
        }
    }
}