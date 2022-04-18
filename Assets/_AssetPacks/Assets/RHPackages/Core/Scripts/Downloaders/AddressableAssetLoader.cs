using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public interface IAddressableAssetLoader
{
    public void ReleaseAsset(string path);
    public Task<GameObject> DownloadGameobject(string path);
    public bool IsReady { get; }
    public IEnumerator WaitForReady(Action completionEvent);
}
public class AddressableAssetLoader : MonoBehaviour, IAddressableAssetLoader
{
    private Dictionary<string, AsyncOperationHandle> _existingAssets;
    public bool IsReady { get; private set; }

    private void Awake()
    {
        _existingAssets = new Dictionary<string, AsyncOperationHandle>();
        Addressables.InitializeAsync().Completed += AdressablesManager_Completed;
    }

    public async Task<GameObject> DownloadGameobject(string path)
    {
        if (IsReady)
        {
            var task = Addressables.LoadAssetAsync<GameObject>(path);
            if (!_existingAssets.ContainsKey(path))
                _existingAssets.Add(path, task);
            return await task.Task;
        }
        throw new ArgumentException("Addressable downloader not ready, please use the 'wait for ready' coroutine");
    }

    public void ReleaseAsset(string path)
    {
        if (IsReady)
        {
            if (_existingAssets.ContainsKey(path))
            {
                Addressables.Release(_existingAssets[path]);
                _existingAssets.Remove(path);
            }
        }
    }
    
    private void AdressablesManager_Completed(AsyncOperationHandle<IResourceLocator> obj)
    {
        IsReady = true;
    }
    
    public IEnumerator WaitForReady(Action completionEvent)
    {
        while (!IsReady)
        {
            yield return new WaitForEndOfFrame();
        }
        completionEvent();
    }
}
