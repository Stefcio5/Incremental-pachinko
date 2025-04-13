using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;

    public void SetText(string content, string header)
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }
        contentField.text = content;

        layoutElement.enabled = Mathf.Max(headerField.preferredWidth, contentField.preferredWidth) >= layoutElement.preferredWidth;

    }

    private void Update()
    {
        if (Application.isEditor)
        {
            layoutElement.enabled = Mathf.Max(headerField.preferredWidth, contentField.preferredWidth) >= layoutElement.preferredWidth;
        }

        Vector2 mousePos = Input.mousePosition;

        transform.position = mousePos;
    }

}
