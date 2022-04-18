using System.Collections;
using System.Collections.Generic;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using UnityEngine;
public class TabButtonTitlesBehaviour : MonoBehaviour
{
    [SerializeField] private string serializedStoryTitle;
    
    [SerializeField] private string serializedRiddleTitle;
    
    [SerializeField] private string serializedValidationTitle;

    [SerializeField] private string serializedResolutionTitle;

    private string _storyTitle;
    
    private string _riddleTitle;
    
    private string _validationTitle;
    
    private string _resolutionTitle;
    public Dictionary<ComponentType, string> titleMap { get; private set; }
    public void Awake()
    {
        SetDependencies(serializedStoryTitle, serializedRiddleTitle, serializedValidationTitle, serializedResolutionTitle);
        GenerateTabTitleMap();
    }

    public void SetDependencies(string storyTitle, string riddleTitle, string validationTitle, string resolutionTitle)
    {
        _storyTitle = storyTitle;
        _riddleTitle = riddleTitle;
        _validationTitle = validationTitle;
        _resolutionTitle = resolutionTitle;
        GenerateTabTitleMap();
    }
    private void GenerateTabTitleMap()
    {
        titleMap = new Dictionary<ComponentType, string>();
        titleMap.Add(ComponentType.Story, _storyTitle);
        titleMap.Add(ComponentType.Riddle, _riddleTitle);
        titleMap.Add(ComponentType.Scanning, _validationTitle);
        titleMap.Add(ComponentType.RiddleTab, _riddleTitle);
        titleMap.Add(ComponentType.Resolution, _resolutionTitle);
    }
}
