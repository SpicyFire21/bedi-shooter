using UnityEngine;

public class FireArrow : MonoBehaviour
{
    public SupremeSword supremeSword;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("GROUND via layer !");
            supremeSword.FinalAbilityCast(gameObject);
        }
    }
}
