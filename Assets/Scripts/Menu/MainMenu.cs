using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField]
    private Dropdown resolutionsDropdown;

    [SerializeField]
    private GameObject optionsPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Resolution[] resolutions = Screen.resolutions; // recupere les resolutions dans la classe Screen (unity)
        resolutionsDropdown.ClearOptions(); // vide les options disponible par défaut

        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + "(" + resolutions[i].refreshRateRatio + " Hz)";
            resolutionOptions.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionsDropdown.AddOptions(resolutionOptions); // add des options dans le dropdown
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue(); // refresh le dropdown
    }

    // Update is called once per frame
    public void NewGameButton() // public pour pouvoir assigner la methode au bouton
    {
        SceneManager.LoadScene("SpellTest");
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MenuTest");
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = Screen.resolutions[resolutionIndex]; // recupere la resolution selectionné
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen); 
        // Screen.fullScreen --> reste dans le mode ou on est (fenetré si fenetré, fullscreen si fullscreen)
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void EnableDisableOptionsPanel()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
        gameObject.SetActive(!gameObject.activeSelf);
        Debug.Log("debug options panel");
    }
}
