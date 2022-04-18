using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.models;
using UnityEngine;
[TestFixture]
public class TestStepBtn
{
    [Test]
    public void TestFactory()
    {
        GameObject go = new GameObject();
        go.AddComponent<StepBtnBehavior>();
        var stepBtn = StepBtn.Factory(go);
        Assert.IsNotNull(stepBtn);
    }
    [Test]
    public void TestFactory_Throws()
    {
        GameObject go = new GameObject();
        Assert.Throws<ArgumentException>(() => StepBtn.Factory(go));
    }

    [Test]
    public void TestConfigure_State_future()
    {
        int idx = 1;
        int reached = 0;
        string labelText = "opgave1";
        Action<int> buttonAction = (index) => { };
        var stepButtonActionsMock = new Mock<IStepButtonActions>();
        stepButtonActionsMock.Setup(x => x.Configure(StepButtonState.future, idx, labelText, buttonAction)).Verifiable();
        var sut = new StepBtn(stepButtonActionsMock.Object);
        sut.Configure(reached, idx, labelText, buttonAction);
        stepButtonActionsMock.Verify(x => x.Configure(StepButtonState.future, idx, labelText, buttonAction));
        Assert.AreEqual(StepButtonState.future, sut.GetState());
    }
    [Test]
    public void TestConfigure_State_Next()
    {
        int idx = 1;
        int reached = 1;
        string labelText = "opgave1";
        Action<int> buttonAction = (index) => { };
        var stepButtonActionsMock = new Mock<IStepButtonActions>();
        stepButtonActionsMock.Setup(x => x.Configure(StepButtonState.next, idx, labelText, buttonAction)).Verifiable();
        var sut = new StepBtn(stepButtonActionsMock.Object);
        sut.Configure(reached, idx, labelText, buttonAction);
        stepButtonActionsMock.Verify(x => x.Configure(StepButtonState.next, idx, labelText, buttonAction));
        Assert.AreEqual(StepButtonState.next, sut.GetState());
    }
    [Test]
    public void TestConfigure_State_Previous()
    {
        int idx = 0;
        int reached = 2;
        string labelText = "opgave0";
        Action<int> buttonAction = (index) => { };
        var stepButtonActionsMock = new Mock<IStepButtonActions>();
        stepButtonActionsMock.Setup(x => x.Configure(StepButtonState.previous, idx, labelText, buttonAction)).Verifiable();
        var sut = new StepBtn(stepButtonActionsMock.Object);
        sut.Configure(reached, idx, labelText, buttonAction);
        stepButtonActionsMock.Verify(x => x.Configure(StepButtonState.previous, idx, labelText, buttonAction));
        Assert.AreEqual(StepButtonState.previous, sut.GetState());
    }

    [Test]
    public void TestPerformAction()
    {
        int idx = 0;
        int reached = 0;
        string labelText = "opgave0";
        Action<int> buttonAction = (index) => {};
        var stepButtonActionsMock = new Mock<IStepButtonActions>();
        stepButtonActionsMock.Setup(x => x.Configure(StepButtonState.previous, idx, labelText, buttonAction)).Verifiable();
        stepButtonActionsMock.Setup(x => x.PerformAction()).Verifiable();
        var sut = new StepBtn(stepButtonActionsMock.Object);
        sut.Configure(reached, idx, labelText, It.IsAny<Action<int>>());
        sut.PerformAction();
        stepButtonActionsMock.Verify(x => x.PerformAction());
    }
}
