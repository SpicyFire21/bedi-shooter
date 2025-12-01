using UnityEngine;
using UnityEngine.AI;

public class Monster : Character
{
    public MonsterData data;
    public NavMeshAgent agent;
    public Animator anim;
    public Transform player;
    protected float lastAttackTime;

    public override void DeathHandler(float destructionTime)
    {
        if (agent != null) agent.isStopped = true;
        if (anim != null) anim.SetBool("Is_dead", true);

        if (!deathSoundPlayed && data.deathSound != null)
        {
            AudioSource.PlayClipAtPoint(data.deathSound, transform.position, 1f);
            deathSoundPlayed = true;
        }

        Destroy(gameObject, destructionTime);
    }

    public virtual void Update()
    {
        HandleDeath(); // vérifie la mort
    }


}
