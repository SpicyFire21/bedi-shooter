using UnityEngine;

public class KamiGhost : Monster
{
    [Header("Ghost Settings")]
    public float hoverAmplitude = 0.5f;   // amplitude du mouvement vertical
    public float hoverSpeed = 2f;         // vitesse du mouvement vertical
    public float followHeight = 2f;       // hauteur de vol par rapport au sol

    private float baseY;

    protected override void Start()
    {
        base.Start();

        baseY = transform.position.y;

        //moveSpeed = data.moveSpeed;   // déjà présent dans MonsterData
        //maxHealth = data.maxHealth;
        //currentHealth = maxHealth;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public override void Update()
    {
        base.Update();

        if (isDead || player == null) return;

        HoverMotion();
        FollowPlayer();
    }

    private void HoverMotion()
    {
        // Mouvement vertical flottant
        float newY = baseY + Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        Vector3 pos = transform.position;
        pos.y = newY;
        transform.position = pos;
    }

    private void FollowPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;

        // On ignore la hauteur pour le déplacement horizontal
        dir.y = 0;

        // Déplacement fluide vers la cible
        transform.position += dir * moveSpeed * Time.deltaTime;

        // Rotation vers le joueur
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 3f
            );
        }
    }

    public override void Attack()
    {
        // tu implémenteras plus tard
    }

    public override void Spawn(Vector3 spawnPosition)
    {
        Debug.Log("un monstre a spawn : " + data.name);
        data.IncrementOnField();
        transform.position = spawnPosition;
    }
}
