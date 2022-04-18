using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExecuteActionAtEndOfFixedFrame
{
    public void Configure(Action actionToExecuteDuringWait, Action actionToExecuteAfterWait);
    public void BeginWaiting();
    public void StopWaiting(bool terminate);
    public bool IsWaiting();
}
public class ExecuteActionAtEndOfFixedFrame : MonoBehaviour, IExecuteActionAtEndOfFixedFrame
{
    private bool _waiting = false;
    private bool _shouldBeWaiting = false;
    private Action storedActionToExecute;
    private Action _ActionToExecuteDuringWait;
    private Action tmpActionToExecute;
    private bool _isConfigured = false;

    public void Configure(Action actionToExecuteDuringWait, Action actionToExecuteAfterWait)
    {
        storedActionToExecute = actionToExecuteAfterWait;
        _ActionToExecuteDuringWait = actionToExecuteDuringWait;
        _isConfigured = true;
    }

    public void BeginWaiting()
    {
        if (!_waiting && _isConfigured)
        {
            _shouldBeWaiting = true;
            tmpActionToExecute = storedActionToExecute;
            StartCoroutine(WaitToExecute());
        }
    }

    public void StopWaiting(bool terminate)
    {
        if (_isConfigured)
        {
            _shouldBeWaiting = false;
            if (terminate)
            {
                tmpActionToExecute = null;
                _waiting = false;
            }
        }
    }

    public bool IsWaiting()
    {
        return _waiting;
    }
    
    private IEnumerator WaitToExecute()
    {
        _waiting = true;
        while (_shouldBeWaiting) //expects StopWaiting to be called via. the invoke when the waiting is done.
        {
            _ActionToExecuteDuringWait.Invoke();
            yield return new WaitForFixedUpdate();
            tmpActionToExecute?.Invoke();
            if (tmpActionToExecute == null)
                _shouldBeWaiting = false;
        }
        StopWaiting(true);
    }
}
