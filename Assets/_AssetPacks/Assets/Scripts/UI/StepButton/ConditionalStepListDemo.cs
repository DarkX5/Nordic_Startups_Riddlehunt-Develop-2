using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalStepListDemo : MonoBehaviour
{
    [SerializeField] private ConditionalStepListView prefab;
    private IConditionalStepListView _conditionalStepList;

    public void Start()
    {
        _conditionalStepList = ConditionalStepListView.Factory(prefab, (RectTransform)this.transform);
       // _conditionalStepList.Configure(); // - can't configure since I have no data available.
    }
}
