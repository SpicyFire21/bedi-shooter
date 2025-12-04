using UnityEngine;

public class KamiGhost : Monster
{
    [Header("Ghost Settings")]
    public float hoverAmplitude = 0.5f;
    public float hoverSpeed = 2f;
    public float rotationSpeed = 3f;

    [Header("Speed Settings")]
    public float accelerationMultiplier = 100000000f;   // vitesse quand le joueur est dans la detection range

    [Header("Kamikaze Settings")]
    public GameObject explosionVFX;   // prefab d'explosion (optionnel)
    public float explosionRadius = 3f; // rayon de l'explosion
    public LayerMask damageMask;      // couches qui recevront les dégâts

    private float baseMoveSpeed;
    private float hoverOffset;
    private bool isTouchingPlayer = false;
    private bool hasExploded = false;

    protected override void Start()
    {
        base.Start();

        hoverOffset = Random.Range(0f, 100f);
        baseMoveSpeed = moveSpeed;  // on garde la valeur de base

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public override void Update()
    {
        base.Update();
        if (isDead || player == null || hasExploded) return;

        HandleSpeedChange();

        if (!isTouchingPlayer)
            MoveTowardsPlayer();

        ApplyHoverEffect();
    }

    private void HandleSpeedChange()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Si le joueur entre dans la detection range → accélération
        if (distance <= data.detectionRange)
            moveSpeed = (baseMoveSpeed + 2) * accelerationMultiplier;
        else
            moveSpeed = baseMoveSpeed;
    }

    private void MoveTowardsPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;

        // Déplacement
        transform.position += dir * moveSpeed * Time.deltaTime;

        // Rotation horizontale
        Vector3 flatDir = dir;
        flatDir.y = 0f;

        if (flatDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(flatDir),
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void ApplyHoverEffect()
    {
        Vector3 pos = transform.position;
        pos.y += Mathf.Sin((Time.time + hoverOffset) * hoverSpeed) * hoverAmplitude * Time.deltaTime;
        transform.position = pos;
    }

    private void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        // Explosion visuelle
        if (explosionVFX != null)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

        // Inflige les dégâts dans un rayon
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, damageMask);
        foreach (Collider hit in hits)
        {
            Character target = hit.GetComponent<Character>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }

        // Destruction du fantôme
        DeathHandler(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return;

        if (other.CompareTag("Player"))
        {
            isTouchingPlayer = true;
            Explode(); // déclenche l'explosion au contact
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isTouchingPlayer = false;
    }

    public override void Attack(float value) { }

    public override void Spawn(Vector3 spawnPosition, int level)
    {
        data.IncrementOnField();
        SetLevel(level);
        transform.position = spawnPosition;
    }
}
