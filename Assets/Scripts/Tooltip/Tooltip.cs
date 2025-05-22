using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _headerField;
    [SerializeField] private TextMeshProUGUI _contentField;
    [SerializeField] private LayoutElement _layoutElement;

    public void SetText(string content, string header)
    {
        if (string.IsNullOrEmpty(header))
        {
            _headerField.gameObject.SetActive(false);
        }
        else
        {
            _headerField.gameObject.SetActive(true);
            _headerField.text = header;
        }
        _contentField.text = content;

        _layoutElement.enabled = Mathf.Max(_headerField.preferredWidth, _contentField.preferredWidth) >= _layoutElement.preferredWidth;

    }

    private void Update()
    {
        if (Application.isEditor)
        {
            _layoutElement.enabled = Mathf.Max(_headerField.preferredWidth, _contentField.preferredWidth) >= _layoutElement.preferredWidth;
        }

        Vector2 mousePos = Input.mousePosition;

        transform.position = mousePos;
    }

}
