using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool isMenuOpened = false;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;

    void Start()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);

        LockCursor(true);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuOpened)
                ResumeGame();
            else
                OpenPauseMenu();
        }
    }


    public void OpenPauseMenu()
    {
        isMenuOpened = true;

        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);

        Time.timeScale = 0f;
        LockCursor(false);
    }

    public void ResumeGame()
    {
        isMenuOpened = false;

        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);

        Time.timeScale = 1f;
        LockCursor(true);
    }

    public void OpenOptions()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    private void LockCursor(bool locked) // sans cette fonction, le curseur disparait après un clic
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
