using UnityEngine;

public class FloatingTextFlyweight : Flyweight
{
    new FloatingTextFlyweightSettings settings => (FloatingTextFlyweightSettings)base.settings;
    private TextMesh _textMesh;

    private void Awake()
    {
        if (_textMesh == null)
        {
            _textMesh = GetComponent<TextMesh>();
        }
    }

    void OnEnable()
    {
        Despawn();
    }

    public void SetText(string text)
    {
        if (_textMesh != null)
        {
            _textMesh.text = "+" + text;
        }
        else
        {
            Debug.LogWarning("TextMesh component not found on FloatingText.");
        }
    }
}