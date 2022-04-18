using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestMapBorder
{
    private MapBorder.Dependencies CreateDependencies(GameObject go)
    {
        return new MapBorder.Dependencies()
        {
            Map = go.AddComponent<SpriteRenderer>(),
            BoxCollider2D = go.AddComponent<BoxCollider2D>()
        };
    }

    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<MapBorder>();
        var dependencies = CreateDependencies(sut.gameObject);
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }

    [TestCase(MapBorderDirection.North)]
    [TestCase(MapBorderDirection.South)]
    [TestCase(MapBorderDirection.East)]
    [TestCase(MapBorderDirection.West)]
    [Test]
    public void TestConfigure(MapBorderDirection direction)
    {
        //Given an initialized MapBorder
        //When Configured with a direction
        //Then the border positions and sizes itself based on the sprite in the map.
        
        //Arrange
        var go = new GameObject();
        go.transform.localScale = Vector3.one;
        var sut = go.AddComponent<MapBorder>();
        var dependencies = CreateDependencies(sut.gameObject);
        dependencies.Map.sprite =
            Sprite.Create(
                Texture2D.blackTexture,
                new Rect(new Vector2(0, 0), new Vector2(1f, 1f)),
                new Vector2(0.5f, 0.5f)
                );

        //Note - the calculations in the AdjustAndPositionHorizontal turns out extremely precise floats.
        //The math is size.y = 1, localscale.y = 1, so it should land at 0.5.
        //But unity has a few other parameters internally that makes it swing alittle.
        //So far this test succeeds, but if it fails, check the hardcoded values.
        var expectedPosition = GetExpectedPosition(direction);
        
        sut.SetDependencies(dependencies);
        //Act
        sut.Configure(direction);
        //Assert
        Assert.AreEqual(expectedPosition, go.transform.localPosition);
    }

    private Vector3 GetExpectedPosition(MapBorderDirection direction)
    {
        switch (direction)
        {
            case MapBorderDirection.North:
                return new Vector3(0f, 0.504999995f, 0f);
            case MapBorderDirection.South:
                return new Vector3(0f, -0.504999995f, 0f);
            case MapBorderDirection.East:
                return new Vector3(0.504999995f, 0f, 0f);
            case MapBorderDirection.West:
                return new Vector3(-0.504999995f, 0f, 0f);
            default:
                throw new ArgumentException("no such case");
        }
    }
    
    [Test]
    public void TestGetBorderDirection()
    {
        //Given a configured MapBorder
        //When GetBorderDirection is called
        //Then the function returns the direction.
        
        //Arrange
        var go = new GameObject();
        go.transform.localScale = Vector3.one;
        var sut = go.AddComponent<MapBorder>();
        var dependencies = CreateDependencies(sut.gameObject);
        dependencies.Map.sprite =
            Sprite.Create(
                Texture2D.blackTexture,
                new Rect(new Vector2(0, 0), new Vector2(1f, 1f)),
                new Vector2(0.5f, 0.5f)
            );
        
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure(MapBorderDirection.North);
        
        //Assert
        Assert.AreEqual(sut.GetBorderDirection(), MapBorderDirection.North);
    }
}
