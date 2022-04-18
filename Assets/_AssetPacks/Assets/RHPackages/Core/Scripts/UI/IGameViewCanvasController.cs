using System.Collections;
using System.Collections.Generic;
using RHPackages.Core.Scripts.UI;
using UnityEngine;
public interface IGameViewCanvasController
{
    public void Hide();
    public void Display();

    public void AttachViewToCanvas(IViewActions view, int index);
}