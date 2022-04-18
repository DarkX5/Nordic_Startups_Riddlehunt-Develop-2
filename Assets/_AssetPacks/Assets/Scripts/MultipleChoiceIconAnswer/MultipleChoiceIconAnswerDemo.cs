using System;
using System.Collections;
using System.Collections.Generic;
using Answers.MultipleChoice.Components;
using Answers.MultipleChoice.Data.Icon;
using UnityEngine;
using UnityEngine.Serialization;

public class MultipleChoiceIconAnswerDemo : MonoBehaviour
{
    [SerializeField] private VideoBasedRiddleViewController videoBasedRiddleViewControllerPrefab;

    private VideoBasedRiddleViewController videoBasedRiddleViewController;
    [SerializeField] private Sprite img1;
    [SerializeField] private Sprite img2;
    [SerializeField] private Sprite img3;
    [SerializeField] private Sprite img4;
    [SerializeField] private Sprite img5;
    public void Start()
    {
        var correctAnswers = new List<IconWithValue>();
        correctAnswers.Add(new IconWithValue()
        {
            Correct = true,
            Icon = img1,
            Value = "a"
        });
        var incorrectAnswers = new List<IconWithValue>();
        incorrectAnswers.Add(new IconWithValue()
        {
            Correct = false,
            Icon = img2,
            Value = "b"
        });
        correctAnswers.Add(new IconWithValue()
        {
            Correct = true,
            Icon = img3,
            Value = "c"
        });
        incorrectAnswers.Add(new IconWithValue()
        {
            Correct = false,
            Icon = img4,
            Value = "d"
        });        
        incorrectAnswers.Add(new IconWithValue()
        {
            Correct = false,
            Icon = img5,
            Value = "e"
        });

        videoBasedRiddleViewController = VideoBasedRiddleViewController.Factory(videoBasedRiddleViewControllerPrefab, this.transform);
        
        videoBasedRiddleViewController.Configure(new VideoBasedRiddleViewController.Config()
        {
            AnswerData = new IconMultipleChoiceAnswerData("multipleChoiceIcon-demo",correctAnswers, incorrectAnswers, (success) => { }),
            IntroVideo =
                "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/det_glemte_test_produkt_022/steps/shared/intro.mp4",
            OutroVideo =
                "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/det_glemte_test_produkt_022/steps/shared/intro.mp4",
            GoBack = () => { Debug.Log("GoBack called");},
            StepCompleted = () => { Debug.Log("Step completed, registered in demo.");}
        });
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Start();
        }
    }
}
