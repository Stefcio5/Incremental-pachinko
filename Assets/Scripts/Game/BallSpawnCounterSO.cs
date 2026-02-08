using BreakInfinity;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "BallSpawnCounter", menuName = "Game/Ball Spawn Counter")]
public class BallSpawnCounterSO : ScriptableObject
{
    [SerializeField] private BigDoubleSO _maxCount;
    [SerializeField] private BigDouble _currentCount = 0;
    public UnityEvent<BigDouble> OnCountChanged;
    private bool _isMaxCountListenerRegistered;
    private bool _isPrestigeListenerRegistered;

    public BigDouble CurrentCount => _currentCount;

    public BigDoubleSO MaxCount => _maxCount;

    private void OnEnable()
    {
        _currentCount = 0;
        RegisterRuntimeListeners();
    }

    private void OnDisable()
    {
        UnregisterRuntimeListeners();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RebindOnPlay()
    {
        foreach (var counter in Resources.FindObjectsOfTypeAll<BallSpawnCounterSO>())
        {
            counter.RegisterRuntimeListeners();
        }
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

    private void RegisterRuntimeListeners()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (!_isMaxCountListenerRegistered && _maxCount != null)
        {
            _maxCount.onValueChanged += InvokeEvent;
            _isMaxCountListenerRegistered = true;
        }

        if (!_isPrestigeListenerRegistered)
        {
            var dataController = DataController.TryGetInstance();
            if (dataController != null)
            {
                dataController.OnPrestige += ResetCount;
                _isPrestigeListenerRegistered = true;
            }
        }
    }

    private void UnregisterRuntimeListeners()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (_isMaxCountListenerRegistered && _maxCount != null)
        {
            _maxCount.onValueChanged -= InvokeEvent;
            _isMaxCountListenerRegistered = false;
        }

        if (_isPrestigeListenerRegistered)
        {
            var dataController = DataController.TryGetInstance();
            if (dataController != null)
            {
                dataController.OnPrestige -= ResetCount;
            }
            _isPrestigeListenerRegistered = false;
        }
    }
}