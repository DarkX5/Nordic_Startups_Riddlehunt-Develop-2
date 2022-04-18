using System;
using System.Collections;
using System.Collections.Generic;
using RHPackages.Core.Scripts.UI;
using UnityEngine;

public interface ILoadingAnimator
{
    public void Start();
    public void Stop();
}
public class LoadingAnimator: ILoadingAnimator{
    Animator Animator;

    public LoadingAnimator(Animator animator)
    {
        Animator = animator;
    }
    public void Start()
    {
        Animator.SetBool("Hidden", false);
    }

    public void Stop()
    {
        Animator.SetBool("Hidden", true);
    }
}
public interface ILoadingView : IViewActions
{
}
[RequireComponent(typeof(CanvasGroupFaderWithAudioFade))]
public class LogoLoadingController : BaseFullscreenView, ILoadingView
{
    public static ILoadingView Factory(GameObject prefab, RectTransform parent)
    {
        var component = prefab.GetComponent<LogoLoadingController>();
        if (component == null)
            throw new ArgumentException("missing component: LogoLoadingController");
        LogoLoadingController behaviour = Instantiate(component, parent);
        behaviour.Initialize();
        return behaviour;
    }
    [SerializeField] private CanvasGroupFaderWithAudioFade _fader;
    [SerializeField] private Animator _animator;
    public class Dependencies
    {
        public ICanvasGroupFaderWithAudioFade CanvasGroupFader;
        public ILoadingAnimator _animator;
    }

    private void Initialize()
    {
        if (_dependencies == null)
        {
            SetDependencies(new Dependencies()
            {
                CanvasGroupFader = _fader,
                _animator = new LoadingAnimator(_animator)
            });
        }
    }

    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }
    
    public override void Display()
    {
        Initialize();
        this.gameObject.SetActive(true);
        Open();
    }
    public override void Hide()
    {
        Initialize();
        Close(StopAnimating);
    }
    
    private void Open()
    {
        _dependencies.CanvasGroupFader.Open();
        _dependencies._animator.Start();
    }

    private void Close(Action completeAction)
    {
        _dependencies.CanvasGroupFader.Close(completeAction);
    }

    private void StopAnimating()
    {
        _dependencies._animator.Stop();
        this.gameObject.SetActive(false);
    }
    
    public override bool IsShown()
    {
        return _dependencies.CanvasGroupFader.IsOpen();
    }
}
