using UnityEngine.AI;
using UnityEngine;

public class Monster : Character
{
    public MonsterData data;

    [SerializeField]
    public NavMeshAgent agent;

    [SerializeField]
    public Animator anim;

    [SerializeField]
    public Transform player;

    protected float lastAttackTime;
    public void DeathHandler(float destructionTime)
    {
        if (agent != null) agent.isStopped = true;
        if (anim != null) anim.SetBool("Is_dead", true);
        if (data.deathSound != null)
            AudioSource.PlayClipAtPoint(data.deathSound, transform.position, 1f);

        Destroy(gameObject, destructionTime);
    }
}
