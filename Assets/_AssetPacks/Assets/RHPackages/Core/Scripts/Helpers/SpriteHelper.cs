using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public interface ISpriteHelper
    {
        public Sprite GetSpriteFromByteArray(Byte[] image, float pixelsPrUnit);
        public Sprite GetSpriteFromByteArray(Byte[] image);
        public List<Sprite> ConvertByteArrayListToSpriteList(List<Byte[]> rawIcons);
    }
    public class SpriteHelper : ISpriteHelper
    {
        public Sprite GetSpriteFromByteArray(Byte[] image, float pixelsPrUnit)
        {
            //https://docs.unity3d.com/540/Documentation/ScriptReference/Texture2D.LoadImage.html
            Texture2D tex = new Texture2D(2, 2); //size will be replaced.
            tex.LoadImage(image);
            var sprite = Sprite.Create(tex, 
                new Rect(0, 0, tex.width, tex.height), 
                new Vector2(0.5f, 0.5f), pixelsPrUnit);
            return sprite;
        }
        
        public Sprite GetSpriteFromByteArray(Byte[] image)
        {
            //https://docs.unity3d.com/540/Documentation/ScriptReference/Texture2D.LoadImage.html
            Texture2D tex = new Texture2D(2, 2); //size will be replaced.
            tex.LoadImage(image);
            var sprite = Sprite.Create(tex, 
                new Rect(0, 0, tex.width, tex.height), 
                new Vector2(0.5f, 0.5f));
            return sprite;
        }
        
        public List<Sprite> ConvertByteArrayListToSpriteList(List<Byte[]> rawIcons)
        {
            var icons = new List<Sprite>();
            foreach (var rawIcon in rawIcons)
            {
                icons.Add(GetSpriteFromByteArray(rawIcon));
            }
            return icons;
        }
    }
}