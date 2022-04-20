using System.Collections.Generic;
using UnityEngine;

public class NarrationTextInitializer : MonoBehaviour
{
    public static NarrationTextInitializer Instance { get; private set; }
    [SerializeField] private List<string> textContents = null;
    [SerializeField] private float textAnimationSpeed = 1f;
    
    public float TextAnimationSpeed { get { return textAnimationSpeed; } }

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    
    public List<string> GetMissionNarration() {
        return textContents;
    }
    public float GetTextAnimationSpeed()
    {
        return textAnimationSpeed;
    }
}
