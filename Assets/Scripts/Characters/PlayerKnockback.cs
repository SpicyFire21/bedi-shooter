using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerKnockback : MonoBehaviour
{
    private CharacterController controller;

    private Vector3 knockbackVelocity;
    private float knockbackTimer;
    public bool isKnockbackActive => knockbackTimer > 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (knockbackTimer > 0)
        {
            controller.Move(knockbackVelocity * Time.deltaTime);
            knockbackTimer -= Time.deltaTime;
        }
    }

    public void ApplyKnockback(Vector3 direction, float force, float duration)
    {
        direction.y = 0f;
        knockbackVelocity = direction.normalized * force;
        knockbackTimer = duration;
    }
}
