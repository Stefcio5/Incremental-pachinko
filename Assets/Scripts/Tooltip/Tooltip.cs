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
    [SerializeField] private Vector2 mouseOffset = new Vector2(20f, -20f);

    private Canvas _canvas;
    private bool _followMouse;
    private RectTransform _target; // Target RectTransform to follow, if any

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        gameObject.SetActive(false);
    }

    public void SetText(string content, string header, bool followMouse, RectTransform target = null)
    {
        _followMouse = followMouse;
        _target = target;

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

        if (_followMouse)
            FollowMouse();
        else if (_target != null)
            FollowTarget();
    }

    private void LateUpdate()
    {
        if (!gameObject.activeSelf) return;

        if (_followMouse)
        {
            FollowMouse();
        }
        else if (_target != null)
        {
            FollowTarget();
        }
    }

    private void FollowMouse()
    {
        // Vector2 localPos;
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //     _canvas.transform as RectTransform,
        //     Input.mousePosition,
        //     _canvas.worldCamera,
        //     out localPos
        // );

        // localPos += mouseOffset;
        // _rectTransform.localPosition = ClampToCanvas(localPos);
        if (Application.isEditor)
        {
            _layoutElement.enabled = Mathf.Max(_headerField.preferredWidth, _contentField.preferredWidth) >= _layoutElement.preferredWidth;
        }

        Vector2 mousePos = Input.mousePosition;

        float pivotX = mousePos.x / Screen.width;
        float pivotY = mousePos.y / Screen.height;
        _rectTransform.pivot = new Vector2(pivotX, pivotY);

        transform.position = mousePos;
    }

    private void FollowTarget()
    {
        Vector3[] corners = new Vector3[4];
        _target.GetWorldCorners(corners);

        // środek targetu
        Vector3 worldCenter = (corners[0] + corners[2]) / 2f;

        // rozmiary w screen space
        Vector2 tooltipSize = _rectTransform.sizeDelta * _canvas.scaleFactor;
        float targetWidth = _target.rect.width * _canvas.scaleFactor;

        // ile miejsca mamy do krawędzi
        float spaceLeft = worldCenter.x;
        float spaceRight = Screen.width - worldCenter.x;

        // konwersja na lokalne
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            worldCenter,
            _canvas.worldCamera,
            out localPos
        );

        // domyślnie po prawej
        Vector2 offset = new Vector2(targetWidth / 2f + tooltipSize.x / 2f + 10f, 0f);

        if (spaceRight < tooltipSize.x && spaceLeft > spaceRight)
        {
            // brak miejsca → po lewej
            offset = new Vector2(-(targetWidth / 2f + tooltipSize.x / 2f + 10f), 0f);
        }

        // pivot w centrum → tooltip zawsze wyśrodkowany pionowo
        _rectTransform.pivot = new Vector2(0.5f, 0.5f);
        _rectTransform.localPosition = ClampToCanvas(localPos + offset);
    }

    private Vector2 ClampToCanvas(Vector2 pos)
    {
        Vector2 canvasSize = (_canvas.transform as RectTransform).sizeDelta;
        Vector2 tooltipSize = _rectTransform.sizeDelta;

        pos.x = Mathf.Clamp(pos.x, -canvasSize.x / 2f, canvasSize.x / 2f - tooltipSize.x);
        pos.y = Mathf.Clamp(pos.y, -canvasSize.y / 2f, canvasSize.y / 2f - tooltipSize.y);

        return pos;
    }
}
