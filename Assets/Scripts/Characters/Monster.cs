using UnityEngine;
using UnityEngine.SceneManagement;

public class Monster : Character
{

    [Header("Statistics")]
    public float attackRange = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void NewGameButton() // public pour attribuer la methode a un bouton
    {
        SceneManager.LoadScene("TestSpell");
    }
}
