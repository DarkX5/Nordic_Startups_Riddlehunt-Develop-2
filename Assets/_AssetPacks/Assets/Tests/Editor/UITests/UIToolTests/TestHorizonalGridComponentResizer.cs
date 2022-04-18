using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UI.UITools;
using UnityEngine;
using UnityEngine.UI;

public class TestHorizonalGridComponentResizer {

    [Test]
    public void TestSetDependencies()
    {
        var go = new GameObject();
        var rectTransform = go.AddComponent<RectTransform>();
        var GridLayoutGroup = go.AddComponent<GridLayoutGroup>();
        var sut = new GameObject().AddComponent<HorizontalGridComponentResizer>();
        
        sut.SetDependencies(new HorizontalGridComponentResizer.Dependencies()
        {
            GridTransform = rectTransform,
            GridLayoutGroup = GridLayoutGroup
        });
        
        Assert.AreSame(rectTransform, sut._dependencies.GridTransform);
        Assert.AreSame(GridLayoutGroup, sut._dependencies.GridLayoutGroup);
    }

    [Test]
    public void TestResizeGrid_Sets5ElementsInGrid_CalulatesCellSize_And_Updates()
    {   
        var go = new GameObject();
        var rectTransform = go.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(1360, 500); // new Rect(Vector2.zero, new Vector2(1360, 500));
        Debug.Log(rectTransform.rect.width);
        var gridLayoutGroup = go.AddComponent<GridLayoutGroup>();

        gridLayoutGroup.padding = new RectOffset(30, 30, 30, 30);
        gridLayoutGroup.cellSize = new Vector2(300, 300);
        gridLayoutGroup.spacing = new Vector2(30, 30);
        
        var sut = new GameObject().AddComponent<HorizontalGridComponentResizer>();
        
        sut.SetDependencies(new HorizontalGridComponentResizer.Dependencies()
        {
            GridTransform = rectTransform,
            GridLayoutGroup = gridLayoutGroup
        });
        
        sut.ResizeGrid(5, 300);
        
        Assert.AreEqual(236,  gridLayoutGroup.cellSize.x);
        Assert.AreEqual(236,  gridLayoutGroup.cellSize.y);
        Assert.IsTrue(gridLayoutGroup.cellSize.x < 300f);
    }

}