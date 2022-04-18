using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hunt;
using riddlehouse_libraries.products.Assets.Adressables;
using UnityEngine;
using UnityEngine.Serialization;

public interface IMapCanvasControllerInstantiator
{
    public Task<IMapCanvasController> CreateOrCollectInstance(AddressableWithTag path);
    public bool HasInstance();
}
public class MapCanvasControllerInstantiator : IMapCanvasControllerInstantiator
{
    private IAddressableAssetLoader _loader;
    public MapCanvasControllerInstantiator(IAddressableAssetLoader loader)
    {
        _loader = loader;
    }
    private IMapCanvasController _instance;
    public async Task<IMapCanvasController> CreateOrCollectInstance(AddressableWithTag addressable)
    {
        if (!HasInstance())
        {
            _instance = GetGlobalInstance(addressable.Tag);
            _instance ??= MapCanvasController.Factory((await _loader.DownloadGameobject(addressable.Address)).GetComponent<MapCanvasController>(), null);
        }
        return _instance;
    }

    public bool HasInstance()
    {
        return _instance != null;
    }

    private MapCanvasController GetGlobalInstance(string tag)
    {
        var go = GameObject.FindWithTag(tag);
        MapCanvasController instance = null;
        if (go != null)
            instance = go.GetComponent<MapCanvasController>();
        return instance;
    }
}