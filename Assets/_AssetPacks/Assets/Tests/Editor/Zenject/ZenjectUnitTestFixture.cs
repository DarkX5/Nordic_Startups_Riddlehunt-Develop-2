using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Zenject;

public abstract class ZenjectUnitTestFixture
{
    DiContainer _container;

    protected DiContainer Container
    {
        get
        {
            return _container;
        }
    }

    [SetUp]
    public virtual void Setup()
    {
        _container = new DiContainer();
    }
}
