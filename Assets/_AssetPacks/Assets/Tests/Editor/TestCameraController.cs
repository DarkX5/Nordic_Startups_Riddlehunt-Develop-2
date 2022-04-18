using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor
{
    public class TestCameraController
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestFollowOK()
        {
            // Camera set up to always be the distance above the target, pointing towards target
            var camera = new GameObject();
            camera.transform.position = new Vector3(0, 100, -20);
            var player = new GameObject();
            player.transform.position = new Vector3(0, 0, 0);
            //var transformerMock = new Mock<ITransformer>(MockBehavior.Strict);
            var distance = 200f;
            Vector3 newPosition = new Vector3(0, distance, 0);
            Vector3 newRotation = new Vector3(90, 0, 0);
            //transformerMock.Setup(x => x.LookAt(
            //        It.Is<Vector3>(v => v == player.transform.position)))
            //    .Returns<Vector3>(s => LookAtAndReturnRotation(camera));
            //transformerMock.Setup(x => x.MoveTo(It.Is<Vector3>(v => v == newPosition)))
            //    .Callback<Vector3>(v => camera.transform.position = v);
            var sut = new MyCameraController(camera.transform, distance);
            sut.Follow(player.transform.position);
            // Use the Assert class to test conditions
            //Vector3 LookAtAndReturnRotation(GameObject camera)
            //{
            //    camera.transform.LookAt(player.transform.position);
            //    return camera.transform.rotation.eulerAngles;
            //}
            Assert.AreEqual(newRotation, camera.transform.rotation.eulerAngles, "Unexpected rotation");
            Assert.AreEqual(newPosition, camera.transform.position, "Unexpected position");
        }
    }
}