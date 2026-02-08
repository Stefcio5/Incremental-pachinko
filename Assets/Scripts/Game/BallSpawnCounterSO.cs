using BreakInfinity;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "BallSpawnCounter", menuName = "Game/Ball Spawn Counter")]
public class BallSpawnCounterSO : ScriptableObject
{
    [SerializeField] private BigDoubleSO _maxCount;
    [SerializeField] private BigDouble _currentCount = 0;
    public UnityEvent<BigDouble> OnCountChanged;

    public BigDouble CurrentCount => _currentCount;
    public BigDoubleSO MaxCount => _maxCount;

    private void OnEnable()
    {
        _maxCount.onValueChanged += InvokeEvent;
        _currentCount = 0;
    }

    private void OnDisable()
    {
        _maxCount.onValueChanged -= InvokeEvent;
    }

    public void InvokeEvent()
    {
        OnCountChanged?.Invoke(_currentCount);
    }

    public void ResetCount()
    {
        _currentCount = 0;
        OnCountChanged?.Invoke(_currentCount);
    }

    public bool Add()
    {
        if (_currentCount < _maxCount.DisplayValue)
        {
            _currentCount++;
            OnCountChanged?.Invoke(_currentCount);
            return true;
        }
        return false;
    }

    public void Remove(BigDouble amount)
    {
        _currentCount = BigDouble.Max(_currentCount - amount, 0);
        OnCountChanged?.Invoke(_currentCount);
    }
}