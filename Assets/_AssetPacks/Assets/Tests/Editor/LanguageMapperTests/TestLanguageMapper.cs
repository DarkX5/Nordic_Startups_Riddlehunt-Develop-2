using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using UnityEngine;

public class TestTabButtonTitles
{
    [Test]
    public void TestSetDependencies()
    {
        //Given a new TabButtonTitles object
        //When SetDependencies is called
        //Then a map is created with the passed titles
        
        //Arrange
        string storyTitle = "Historie";
        string riddleTitle = "Gåde";
        string validationTitle = "Korrekt";
        string resolutionTitle = "Afslutning";
        //Act
        var sut = new GameObject().AddComponent<TabButtonTitlesBehaviour>();
        sut.SetDependencies(storyTitle, riddleTitle, validationTitle, resolutionTitle);
        //Assert
        Assert.AreEqual(storyTitle, sut.titleMap[ComponentType.Story]);
        Assert.AreEqual(riddleTitle, sut.titleMap[ComponentType.Riddle]);
        Assert.AreEqual(validationTitle, sut.titleMap[ComponentType.Scanning]);
        Assert.AreEqual(resolutionTitle, sut.titleMap[ComponentType.Resolution]);
    }
}
