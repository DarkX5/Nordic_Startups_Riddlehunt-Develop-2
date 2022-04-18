using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface ITestable<TComponent>
{
    TComponent Get();
}
public class TestableComponent<TComponent> : ITestable<TComponent> where TComponent : Component
{
    private TComponent _component;
    public TestableComponent(TComponent component)
    {
        _component = component;
    }
    public TComponent Get()
    {
        if (_component == null)
            _component = new GameObject().AddComponent<TComponent>();
        return _component;
    }
}
