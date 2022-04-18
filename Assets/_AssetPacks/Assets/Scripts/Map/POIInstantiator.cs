using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using riddlehouse_libraries.products.Assets.Adressables;
using UnityEngine;

public interface IPOIInstantiator
{
    public Task<IPOIController> Create(Addressable addressablePrefab);
}
public class POIInstantiator : IPOIInstantiator
{
    private IAddressableAssetLoader _loader;
    public POIInstantiator(IAddressableAssetLoader loader)
    {
        _loader = loader;
    }
    
    public async Task<IPOIController> Create(Addressable addressablePrefab)
    {
        var go = (await _loader.DownloadGameobject(addressablePrefab.Address));
        var prefab = go.GetComponent<POIController>();
        return  POIController.Factory(prefab, null);
    }
}