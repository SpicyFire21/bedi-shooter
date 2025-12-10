using UnityEngine;

public class Cinematique : MonoBehaviour
{
    public Camera playerCamera;       // Caméra du personnage
    public Camera cinematicCamera;    // Caméra pour la cinématique

    public void EndCinematic()
{
    cinematicCamera.gameObject.SetActive(false);
    playerCamera.gameObject.SetActive(true);
}

}
