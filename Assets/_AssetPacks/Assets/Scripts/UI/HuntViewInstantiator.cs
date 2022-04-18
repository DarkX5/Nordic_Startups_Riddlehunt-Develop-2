using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public interface IHuntViewInstantiator
{
    IStoryComponent RetrieveStoryView();
    IRiddleTabComponent RetrieveRiddleView();
}
public class HuntViewInstantiator : MonoBehaviour, IHuntViewInstantiator
{
    [SerializeField] private StoryHuntComponentBehaviour storyComponentPrefab;
    [SerializeField] private RiddleTabComponent riddleTabComponentPrefab;
    public HuntViewInstantiator(StoryHuntComponentBehaviour storyComponentPrefab, RiddleTabComponent riddleTabComponentPrefab)
    {
        this.storyComponentPrefab = storyComponentPrefab;
        this.riddleTabComponentPrefab = riddleTabComponentPrefab;
    }

    private IStoryComponent _story;
    public IStoryComponent RetrieveStoryView()
    {
        if (_story == null)
        {
            var behaviour = Instantiate(storyComponentPrefab.gameObject);
            _story = StoryComponent.Factory(behaviour);
        }

        return _story;
    }
        
    private IRiddleTabComponent _riddle;
    public IRiddleTabComponent RetrieveRiddleView()
    {
        if (_riddle == null)
        {
            _riddle = RiddleTabComponent.Factory(riddleTabComponentPrefab, null);
        }
        return _riddle;
    }
}
