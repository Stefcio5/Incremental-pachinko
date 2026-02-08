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
    [SerializeField] private Vector2 _mouseOffset = new Vector2(-5f, -5f);
    [SerializeField] private float _edgePadding = 0f;

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
        if (Application.isEditor)
        {
            _layoutElement.enabled = Mathf.Max(_headerField.preferredWidth, _contentField.preferredWidth) >= _layoutElement.preferredWidth;
        }

        Vector2 mousePos = Input.mousePosition;

        float pivotX = mousePos.x / Screen.width;
        float pivotY = mousePos.y / Screen.height;
        _rectTransform.pivot = new Vector2(pivotX, pivotY);

        transform.position = mousePos + _mouseOffset;
    }

    private void FollowTarget()
    {
        if (_canvas == null || _target == null) return;

        var canvasRect = (RectTransform)_canvas.transform;
        var cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;

        // Get target world corners and convert right edge to canvas-local space
        Vector3[] wc = new Vector3[4];
        _target.GetWorldCorners(wc); // 0:BL, 1:TL, 2:TR, 3:BR

        Vector2 rightTop = WorldToCanvasLocal(canvasRect, cam, wc[2]);
        Vector2 rightBottom = WorldToCanvasLocal(canvasRect, cam, wc[3]);
        Vector2 rightCenter = (rightTop + rightBottom) * 0.5f;

        // Ensure tooltip size is up to date
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
        Vector2 tooltipSize = _rectTransform.rect.size;

        // Place tooltip so its left edge sits next to target's right edge
        _rectTransform.pivot = new Vector2(0f, 0.5f);
        Vector2 anchored = new Vector2(rightCenter.x + _edgePadding, rightCenter.y);

        // Clamp vertically within canvas
        float halfH = tooltipSize.y * 0.5f;
        float minY = canvasRect.rect.yMin + halfH;
        float maxY = canvasRect.rect.yMax - halfH;
        anchored.y = Mathf.Clamp(anchored.y, minY, maxY);

        // Clamp horizontally within canvas (keep at right edge, but do not overflow)
        float maxX = canvasRect.rect.xMax - tooltipSize.x;
        anchored.x = Mathf.Min(anchored.x, maxX);

        _rectTransform.anchoredPosition = anchored;
    }

    private static Vector2 WorldToCanvasLocal(RectTransform canvasRect, Camera cam, Vector3 worldPos)
    {
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screen, cam, out var local);
        return local;
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
