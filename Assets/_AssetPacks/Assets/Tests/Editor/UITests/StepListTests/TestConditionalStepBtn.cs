using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.models;
using UnityEngine;
[TestFixture]
public class TestConditionalStepBtn
{
    [Test]
    public void TestFactory()
    {
        GameObject go = new GameObject();
        go.AddComponent<ConditionalStepBtnBehavior>();
        var stepBtn = ConditionalStepBtn.Factory(go);
        Assert.IsNotNull(stepBtn);
    }
    [Test]
    public void TestFactory_Throws()
    {
        GameObject go = new GameObject();
        Assert.Throws<ArgumentException>(() => ConditionalStepBtn.Factory(go));
    }

    [Test]
    public void TestConfigure_State_Next()
    {
        string stepId = "opgave-1";
        string labelText = "opgave 1";
        Action<string> buttonAction = (theStepId) => { };
        var stepButtonActionsMock = new Mock<IConditionalStepButtonActions>();
        stepButtonActionsMock.Setup(x => x.Configure(ConditionalStepButtonState.Incomplete, stepId, labelText, buttonAction)).Verifiable();
        var sut = new ConditionalStepBtn(stepButtonActionsMock.Object);
        sut.Configure(ConditionalStepButtonState.Incomplete, stepId, labelText, buttonAction);
        stepButtonActionsMock.Verify(x => x.Configure(ConditionalStepButtonState.Incomplete, stepId, labelText, buttonAction));
        Assert.AreEqual(ConditionalStepButtonState.Incomplete, sut.GetState());
    }
    [Test]
    public void TestConfigure_State_Disabled()
    {
        string stepId = "opgave-1";
        string labelText = "opgave 1";
        Action<string> buttonAction = (theStepId) => { };
        var stepButtonActionsMock = new Mock<IConditionalStepButtonActions>();
        stepButtonActionsMock.Setup(x => x.Configure(ConditionalStepButtonState.Disabled, stepId, labelText, buttonAction)).Verifiable();
        var sut = new ConditionalStepBtn(stepButtonActionsMock.Object);
        sut.Configure(ConditionalStepButtonState.Disabled, stepId, labelText, buttonAction);
        stepButtonActionsMock.Verify(x => x.Configure(ConditionalStepButtonState.Disabled, stepId, labelText, buttonAction));
        Assert.AreEqual(ConditionalStepButtonState.Disabled, sut.GetState());
    }
    [Test]
    public void TestConfigure_State_Hidden()
    {
        string stepId = "opgave-1";
        string labelText = "opgave 1";
        Action<string> buttonAction = (theStepId) => { };
        var stepButtonActionsMock = new Mock<IConditionalStepButtonActions>();
        stepButtonActionsMock.Setup(x => x.Configure(ConditionalStepButtonState.Hidden, stepId, labelText, buttonAction)).Verifiable();
        var sut = new ConditionalStepBtn(stepButtonActionsMock.Object);
        sut.Configure(ConditionalStepButtonState.Hidden, stepId, labelText, buttonAction);
        stepButtonActionsMock.Verify(x => x.Configure(ConditionalStepButtonState.Hidden, stepId, labelText, buttonAction));
        Assert.AreEqual(ConditionalStepButtonState.Hidden, sut.GetState());
    }
    [Test]
    public void TestConfigure_State_Complete()
    {
        string stepId = "opgave-1";
        string labelText = "opgave 1";
        Action<string> buttonAction = (theStepId) => { };
        var stepButtonActionsMock = new Mock<IConditionalStepButtonActions>();
        stepButtonActionsMock.Setup(x => x.Configure(ConditionalStepButtonState.Complete, stepId, labelText, buttonAction)).Verifiable();
        var sut = new ConditionalStepBtn(stepButtonActionsMock.Object);
        sut.Configure(ConditionalStepButtonState.Complete, stepId, labelText, buttonAction);
        stepButtonActionsMock.Verify(x => x.Configure(ConditionalStepButtonState.Complete, stepId, labelText, buttonAction));
        Assert.AreEqual(ConditionalStepButtonState.Complete, sut.GetState());
    }

    [Test]
    public void TestPerformAction()
    {
        string stepId = "opgave-0";
        string labelText = "opgave 0";
        Action<string> buttonAction = (theStepId) => { };
        var stepButtonActionsMock = new Mock<IConditionalStepButtonActions>();
        stepButtonActionsMock.Setup(x => x.Configure(ConditionalStepButtonState.Complete, stepId, labelText, buttonAction)).Verifiable();
        stepButtonActionsMock.Setup(x => x.PerformAction()).Verifiable();
        var sut = new ConditionalStepBtn(stepButtonActionsMock.Object);
        sut.Configure(ConditionalStepButtonState.Complete, stepId, labelText, buttonAction);
        sut.PerformAction();
        stepButtonActionsMock.Verify(x => x.PerformAction());
    }
}
