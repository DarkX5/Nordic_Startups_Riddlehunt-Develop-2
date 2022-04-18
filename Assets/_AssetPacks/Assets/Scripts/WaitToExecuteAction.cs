using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWaitToExecuteAction
{
    public void Configure(Action actionToExecute, float timeToWait);
    public void BeginWaiting();
    public void StopWaiting();
    public bool IsWaiting();
}
public class WaitToExecuteAction : MonoBehaviour, IWaitToExecuteAction
{
    [SerializeField] private bool _waiting = false;
    public bool IsWaiting()
    {
        return _waiting;
    }

    private Action _storedActionToExecute;
    private Action _tmpActionToExecute;
    private float _timeToWait;
    private bool _isConfigured = false;
    public void Configure(Action actionToExecute, float timeToWait)
    {
        _storedActionToExecute = actionToExecute;
        _timeToWait = timeToWait;
        _isConfigured = true;
    }

    public void BeginWaiting()
    {
        if (!_waiting && _isConfigured)
        {
            _tmpActionToExecute = _storedActionToExecute;
            StartCoroutine(WaitToExecute());
        }
    }

    public void StopWaiting()
    {   
        if (_isConfigured)
        {
            _tmpActionToExecute = null;
            _waiting = false;
        }
    }

    public void OnDisable()
    {
        StopWaiting();
    }

    private IEnumerator WaitToExecute()
    {
        _waiting = true;
        yield return new WaitForSeconds(_timeToWait);
        if(_tmpActionToExecute != null)
            _tmpActionToExecute?.Invoke();
        StopWaiting();
    }
}
