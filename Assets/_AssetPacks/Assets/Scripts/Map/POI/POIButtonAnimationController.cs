using System;
using UnityEngine;

public interface IPOIButtonAnimationController
{
    public void ClickAnimation();
    public void SetDisabled();
    public void SetIdle();
    public bool IsDisabled();
    public bool IsCompleted();
    public bool IsHighlighted();

    public void SetHighlighted();
    public void SetCompleted();
}

public class POIButtonAnimationController : MonoBehaviour, IPOIButtonAnimationController
{
    public class Dependencies
    {
        public Animator AC { get; set; }
    }

    public void Initialize()
    {
        SetDependencies(new Dependencies()
        {
            AC = ac
        });
    }
    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }
    
    [SerializeField] Animator ac;
    public void ClickAnimation()
    {
        if (!_dependencies.AC.GetBool("Disabled"))
        {
            _dependencies.AC.SetTrigger("GoToClicked");
        }
    }
    
    public void SetDisabled()
    {
        if (!_dependencies.AC.GetBool("Disabled"))
        {
            _dependencies.AC.SetBool("Disabled", true);
            _dependencies.AC.SetBool("Highlighted", false);
            _dependencies.AC.SetBool("Completed", false);
        }
    }

    public void SetHighlighted()
    {
        _dependencies.AC.SetBool("Disabled", false);
        _dependencies.AC.SetBool("Completed", false);
        _dependencies.AC.SetBool("Highlighted", true);
    }
    
    public void SetCompleted()
    {
        _dependencies.AC.SetBool("Disabled", false);
        _dependencies.AC.SetBool("Highlighted", false);
        _dependencies.AC.SetBool("Completed", true);
    }
    
    public void SetIdle()
    {
        _dependencies.AC.SetBool("Disabled", false);
        _dependencies.AC.SetBool("Completed", false);
        _dependencies.AC.SetBool("Highlighted", false);
    }

    public bool IsDisabled()
    {
        return _dependencies.AC.GetBool("Disabled");
    }

    public bool IsCompleted()
    {
        return _dependencies.AC.GetBool("Completed");
    }
    public bool IsHighlighted()
    {
        return _dependencies.AC.GetBool("Highlighted");
    }

#if UNITY_EDITOR
    //ForTesting.
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            SetDisabled();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            SetIdle();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            SetHighlighted();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            SetCompleted();
        }
    }
#endif
}
