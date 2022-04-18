using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageGridDemo : MonoBehaviour
{
    [SerializeField] private Sprite img1;
    [SerializeField] private Sprite img2;
    [SerializeField] private Sprite img3;
    [SerializeField] private Sprite img4;
    [SerializeField] private Sprite img5;
    [SerializeField] private Sprite img6;

    [SerializeField] private ImageGridComponentBehaviour gridBehaviour;

    public void Start()
    {
        gridBehaviour.Configure(new List<Sprite>(){img1,img2,img3,img4,img5,img6});
    }
}
