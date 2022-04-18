using System;
using Riddlehouse.Core.Helpers.Helpers;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Editor.HelperTests
{
    public class UIFittersTests
    {

        [Test]
        public void TestFitToFullscreen_Sets_The_Child_To_Fill_Parent_UI()
        {
            //Given a parent and a child of type RectTransform
            //When FitToFullScreen is called
            //Then the child fits to fullscreen.
            
            var parent = new GameObject();
            var parentTransform = parent.AddComponent<RectTransform>();
            var child = new GameObject();
            var childTransform = child.AddComponent<RectTransform>();

            var sut = new UIFitters();
            sut.FitToFullscreen(childTransform, parentTransform);
            
            Assert.AreEqual(parentTransform, childTransform.parent);
            Assert.AreEqual(new Vector3(1, 1, 1), childTransform.localScale);
            Assert.AreEqual(Vector2.zero, childTransform.anchorMin);
            Assert.AreEqual(Vector2.one, childTransform.anchorMax);
            Assert.AreEqual(Vector2.zero, childTransform.offsetMin);
            Assert.AreEqual(Vector2.zero, childTransform.offsetMax);
        }
        
        [Test]
        public void TestFitToFullscreen_Parent_Null_Throws_ArgumentException()
        {
            //Given a null parent and a child of type RectTransform
            //When FitToFullScreen is called
            //Then the system throws an argumentexception.
            
           // RectTransform parentTransform = null;
            var child = new GameObject();
            var childTransform = child.AddComponent<RectTransform>();
            
            var sut = new UIFitters();
            Assert.Throws<ArgumentException>(() =>
               sut.FitToFullscreen(childTransform, null));

        }
                
        [Test]
        public void TestFitToFullscreen_Child_Null_Throws_ArgumentException()
        {
            //Given a parent of type RectTransform and a null child 
            //When FitToFullScreen is called
            //Then the system throws an argumentexception.
            
            var parent = new GameObject();
            var parentTransform = parent.AddComponent<RectTransform>();
            RectTransform childTransform = null;
            var sut = new UIFitters();
            Assert.Throws<ArgumentException>(() =>
                sut.FitToFullscreen(childTransform, parentTransform));
        }
    }
}