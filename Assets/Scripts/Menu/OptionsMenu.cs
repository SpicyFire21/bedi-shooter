using UnityEngine;

public class OptionsMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject controlsPanel;

    [SerializeField]
    private GameObject generalPanel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
    }

    public void EnableDisableGeneralPanel()
    {
        generalPanel.SetActive(!generalPanel.activeSelf);
        controlsPanel.SetActive(false);
    }

    public void EnableDisableControlsPanel()
    {
        controlsPanel.SetActive(!controlsPanel.activeSelf);
        generalPanel.SetActive(false);
        Debug.Log("entrée controles");
    }

}
