using System;
using System.Collections;
using System.Collections.Generic;
using Riddlehunt.Beta.Environment.Controls;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public interface ITouchCounter
{
     public int GetTouchCount();
}

public class TouchCounter : ITouchCounter
{
     public int GetTouchCount()
     {
          return Input.touchCount;
     }
}

[RequireComponent(typeof(WaitToExecuteAction))]
public class BetaTesterUIActions : MonoBehaviour
{
     public class Dependencies
     {
          public ITouchCounter TouchCounter { get; set; }
          public CanvasLayerManager Clm { get; set; }
          public IWaitToExecuteAction WaitToExecuteAction { get; set; }
          
     }
     [Inject] private CanvasLayerManager _clm;
     private IBetaTesterController _controller;
     public void Start()
     {
          SetDependencies(new Dependencies()
          {
               TouchCounter = new TouchCounter(),
               Clm = _clm,
               WaitToExecuteAction = GetComponent<WaitToExecuteAction>()
          });
     }
     public Dependencies _dependencies { get; private set; }
     public void SetDependencies(Dependencies dependencies)
     {
          _dependencies = dependencies;
          _controller = _dependencies.Clm.GetBetaTesterController();
          if (_controller != null)
          {
               _dependencies.WaitToExecuteAction.Configure(() =>
               {
                    _controller.Configure();
               }, 0.5f);
          }
     }

#if UNITY_EDITOR //For unity testing purposes.
     [Tooltip("Button to press while holding the hold button. Unity Editor Only.")]
     [SerializeField] private KeyCode triggerToActivate = KeyCode.Space;
     [Tooltip("Button to hold while pressing the trigger button. Unity Editor Only.")]
     [SerializeField] private KeyCode holdButton = KeyCode.F1;
     public void Update()
     {
          if (Input.GetKeyDown(triggerToActivate) && Input.GetKey(holdButton))
          {
               _dependencies.WaitToExecuteAction.BeginWaiting();
          }
     }
#endif

     public void FixedUpdate()
     {
          if (_controller != null)
          {
               if (!_controller.IsCurrentlyActive())
               {
                    var touchCount = _dependencies.TouchCounter.GetTouchCount();
                    if (!_dependencies.WaitToExecuteAction.IsWaiting())
                    {
                         if (touchCount >= 3)
                         {
                              _dependencies.WaitToExecuteAction.BeginWaiting();
                         }
                    }
                    else if (_dependencies.WaitToExecuteAction.IsWaiting())
                    {
                         if (touchCount < 3)
                         {
                              _dependencies.WaitToExecuteAction.StopWaiting();
                         }
                    }
               }
          }
     }
}
