using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestDataPathHelper : MonoBehaviour
{
    [Test]
    public void TestDataPathHelper_PersistantDataPathGetter()
    {
       Assert.AreEqual(Application.persistentDataPath, DataPathHelper.PersistentDataPath); 
    }
    
    [Test]
    public void TestDataPathHelper_PersistantDataPathGetter_Android()
    {
        Assert.AreEqual("file:/"+Application.persistentDataPath, DataPathHelper.GetPersistantDataPath_Android()); 
    }
    
    [Test]
    public void TestDataPathHelper_PersistantDataPathGetter_ios()
    {
        Assert.AreEqual(Application.persistentDataPath, DataPathHelper.GetPersistantDataPath_Ios()); 
    }
}
