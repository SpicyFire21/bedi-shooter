using UnityEngine;

public class Waterball : SpellBase
{
    public float speed = 10f;
    public float lifeTime = 3f;

    protected override void Cast()
    {
        Vector3 dir = (destination - transform.position).normalized;
        GetComponent<Rigidbody>().linearVelocity = dir * speed;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        Character target = other.GetComponent<Character>();

        if (target != null && target != caster)
        {
            target.TakeDamage(data.damage);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Destructible"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
