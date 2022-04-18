using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoaderView
{
    public void Display();
    public void Hide();
}
public class LoaderView : MonoBehaviour,ILoaderView
{
    public static ILoaderView Factory(LoaderView prefab, Transform parent)
    {
        return Instantiate(prefab, parent);
    }
    public void Display()
    {
      this.gameObject.SetActive(true);
    }

    public void Hide()
    {
      this.gameObject.SetActive(false);
    }
} 
