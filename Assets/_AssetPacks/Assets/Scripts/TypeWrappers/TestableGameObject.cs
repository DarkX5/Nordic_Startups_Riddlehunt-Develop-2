using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestableGameObject : ITestable<GameObject>
{
    private GameObject _component;
    public TestableGameObject(GameObject component)
    {
        _component = component;
    }
    public GameObject Get()
    {
        if (_component == null)
            return new GameObject();
        else
            return _component;
    }
}
