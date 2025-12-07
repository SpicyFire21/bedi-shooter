using UnityEngine;
using System.Collections;

public class DarkSkeleton : Skeleton
{
    [Header("Dark Skeleton Settings")]
    public float screamCooldown = 6f;       // Temps minimal entre deux cris
    public float screamDuration = 1.5f;    // Durée exacte de l’animation de scream
    public AudioClip screamSound;

    private float lastScreamTime = -999f;
    private bool isScreaming = false;

    protected override void Start()
    {
        base.Start();

        // Toutes les animations normales sont plus rapides
        anim.speed = 1.5f;

        // DarkSkeleton plus rapide que Skeleton
        runSpeedMultiplier = 3f;
        runAnimMultiplier = 2f;
    }

    public override void Update()
    {
        base.Update();
        HandleScream();
    }

    private void HandleScream()
    {
        if (player == null || isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Scream si le joueur sort de la detection range
        if (!isScreaming && distance > data.detectionRange)
            TryScream();
    }

    private void TryScream()
    {
        if (Time.time < lastScreamTime + screamCooldown)
            return; // encore en cooldown

        StartCoroutine(ScreamRoutine());
    }

    private IEnumerator ScreamRoutine()
    {
        isScreaming = true;
        lastScreamTime = Time.time;

        // Stop la locomotion et l'animation de déplacement
        agent.isStopped = true;
        anim.SetFloat("Speed", 0f);

        // Déclenche l'animation Scream
        anim.SetTrigger("Scream");

        // On attend la fin de l'animation
        yield return new WaitForSeconds(screamDuration);

        // Relance la locomotion
        agent.isStopped = false;
        isScreaming = false;
    }

    public void PlayScreamSound()
    {
        if (screamSound != null)
            AudioSource.PlayClipAtPoint(screamSound, transform.position, 0.7f);
    }


    // Empêche le squelette de bouger pendant le scream
    protected override void SpeedAnimManager()
    {
        if (isScreaming)
        {
            anim.SetFloat("Speed", 0f);
            agent.isStopped = true;
            return;
        }

        base.SpeedAnimManager();
    }
}
