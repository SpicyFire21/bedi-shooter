using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;

    private Character owner;

    public void Init(Character caster)
    {
        owner = caster;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Destruction iminente");
        // vérifier si c'est un character différent du lanceur
        Character target = other.GetComponent<Character>();
        if (target != null && target != owner)
        {
            // target.TakeDamage(20);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Destructible"))
        {
            Destroy(other.gameObject); // on detruit l'objet avec qui on est rentré en collision (temporaire)
            Destroy(gameObject);
        }
    }

}
