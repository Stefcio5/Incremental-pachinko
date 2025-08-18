using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _headerField;
    [SerializeField] private TextMeshProUGUI _contentField;
    [SerializeField] private LayoutElement _layoutElement;
    [SerializeField] private RectTransform _rectTransform;
    private bool _useMouse = true;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string content, string header)
    {
        _rectTransform.pivot = new Vector2(0, 0.5f);
        _useMouse = true;
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

    private void LateUpdate()
    {
        if (Application.isEditor)
        {
            _layoutElement.enabled = Mathf.Max(_headerField.preferredWidth, _contentField.preferredWidth) >= _layoutElement.preferredWidth;
        }

        Vector2 mousePos = Input.mousePosition;

        float pivotX = mousePos.x / Screen.width;
        float pivotY = mousePos.y / Screen.height;

        _rectTransform.pivot = new Vector2(0, 0.5f);

        transform.position = mousePos;

    }

}
