using UnityEngine;

public abstract class MeleeWeapon : Weapon
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // 
    public override void Attack(Player player)
    {
        DoAnimation(player);
        Monster target = GetTarget(weaponRange, player);
        player.SetAttackTarget(target);
    }

    public Monster GetTarget(float maxDistance, Player player)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, hitMask))
        {
            if (hit.collider.TryGetComponent<Monster>(out Monster character))
            {
                // direction du joueur
                Vector3 forward = player.transform.forward; 
                Vector3 toTarget = (character.transform.position - player.transform.position).normalized;

                // angle entre la direction du joueur et la target
                float angle = Vector3.Angle(forward, toTarget);

                if (angle <= 155 / 2f)
                {
                    return character;
                } // on prend l'angle vers lequel le player est tourné pour pouvoir n'attaquer que les cibles en face de nous
            }
        }

        return null;
    }


    public abstract void DoAnimation(Player player);
}
