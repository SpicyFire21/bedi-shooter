using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(CharacterController))]
public class PlayerKnockback : MonoBehaviour
{
    private CharacterController controller;
    private ThirdPersonController tps;

    [Header("Gravity Settings")]
    public float knockGravity = -12f;

    private Vector3 verticalVelocity;

    private Vector3 knockDirection;
    private float knockForce;
    private float knockDuration;
    private float knockTimer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        tps = GetComponent<ThirdPersonController>();
    }

    void Update()
    {
        if (knockTimer > 0f)
        {
            knockTimer -= Time.deltaTime;

            // Force horizontale (impulsion lissée)
            Vector3 move = knockDirection * knockForce;

            // Gravité douce
            verticalVelocity.y += knockGravity * Time.deltaTime;

            move += verticalVelocity * Time.deltaTime;

            controller.Move(move * Time.deltaTime);

            // Empêche le mouvement normal du joueur
            tps.canMove = false;

            if (knockTimer <= 0f)
            {
                // Reset à la fin
                tps.canMove = true;
                verticalVelocity = Vector3.zero;
            }
        }
    }

    public void ApplyKnockback(Vector3 direction, float force, float hauteur, float duration)
    {
        direction.y = hauteur;
        knockDirection = direction.normalized;
        knockForce = force;
        knockDuration = duration;
        knockTimer = duration;

        // donne une impulsion verticale initiale
        verticalVelocity = new Vector3(0, hauteur * 6f, 0);
    }
}
