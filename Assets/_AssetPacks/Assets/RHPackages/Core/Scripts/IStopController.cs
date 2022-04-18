using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStopController
{
    public void OpenStop();
    public void EndStop(bool completed);
    public void StartStep(string id);
    public void StartStop(string id);
    public void CloseStop();
    public void DestroySelf();
    public string GetId();
}