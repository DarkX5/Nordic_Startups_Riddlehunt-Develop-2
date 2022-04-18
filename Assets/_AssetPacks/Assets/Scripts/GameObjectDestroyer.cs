using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameObjectDestroyer
{
    public void Destroy();
}
public class GameObjectDestroyer : MonoBehaviour, IGameObjectDestroyer
{
    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
