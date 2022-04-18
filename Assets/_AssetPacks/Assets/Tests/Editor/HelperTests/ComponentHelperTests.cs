using System;
using Riddlehouse.Core.Helpers.Helpers;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Editor.HelperTests
{
    public class ComponentHelperTests
    {
        [Test]
        public void TestCheckIfBehaviorExists_No_Behaviour_On_Prefab_Throws_ArgumentException()
        {
            // Given the gameobject has no behavior
            // When constructing ProductStartPanel
            // Then an exception is thrown
            GameObject gameObject = new GameObject();
            Assert.Throws<ArgumentException>(() => 
                new ComponentHelper<StoryHuntComponentBehaviour>().GetBehaviourIfExists(gameObject));
        }

        [Test]
        public void TestCheckIfBehaviorExists_Prefab_Has_Component_Then_Returns_Component()
        {
            // Given the gameobject has a StoryComponentBehaviour
            // When constructing StoryComponent
            // Then the component is returned.
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<StoryHuntComponentBehaviour>();
            var component = new ComponentHelper<StoryHuntComponentBehaviour>().GetBehaviourIfExists(gameObject);
            Assert.IsNotNull(component);
        }

        [Test]
        public void TestSetLogicInstance()
        {
            //Given a field carrying null, and a value to set.
            //When ComponentHelper.setLogicInstance is called.
            //Then the helper sets the instance to the "toSet" value.
            string value = "test";
            string field = new ComponentHelper<string>().SetLogicInstance(value, null);
            Assert.AreEqual(value, field);
        }
        [Test]
        public void TestSetLogicInstance_throws()
        {
            //Given a set field, and a value to set.
            //When ComponentHelper.setLogicInstance is called.
            //Then the helper detects that the instance isn't null, and throws and error.
            string field = "alreadySet";
            string value = "test";
            Assert.Throws<ArgumentException>(() => 
               new ComponentHelper<string>().SetLogicInstance(value, field));
        }
    }
}