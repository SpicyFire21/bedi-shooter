using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField]
    private Text headerText;

    [SerializeField]
    private Text contentText;

    [SerializeField]
    private LayoutElement layoutElement;

    [SerializeField]
    private int maxCharacter;

    [SerializeField]
    private RectTransform rectTransform;

    public void SetText(string header, string content)
    {
        headerText.gameObject.SetActive(true);
        headerText.text = header;
        contentText.gameObject.SetActive(true);
        contentText.text = content;

        int headerLength = headerText.text.Length;
        int contentLength = contentText.text.Length;

        layoutElement.enabled = (headerLength > maxCharacter || contentLength > 0) ? true : false;
    }

    private void Update()
    {
        Vector2 mousePosition = Input.mousePosition;

        // si element tout a droite et sort de l'écran --> tooltip a gauche
        float pivotX = mousePosition.x / Screen.width;
        float pivotY = mousePosition.x / Screen.height;
        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = mousePosition;
    }
}
