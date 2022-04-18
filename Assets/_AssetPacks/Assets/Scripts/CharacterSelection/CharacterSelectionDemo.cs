using System.Collections;
using System.Collections.Generic;
using CharacterSelection;
using CharacterSelection.Components;
using Newtonsoft.Json;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.huntProduct;
using UnityEngine;


public class CharacterSelectionDemo : MonoBehaviour
{
    [SerializeField] private CharacterSelectionFlowController _flowControllerPrefab;
    private ICharacterSelectionFlowController _flowController;

    private ProductResourceService productResourceService;

    // Start is called before the first frame update
   async void Start()
    {
        productResourceService = new ProductResourceService(DataPathHelper.PersistentDataPath, "productID");
        await productResourceService.IsReady();
        _flowController = CharacterSelectionFlowController.Factory(_flowControllerPrefab, (RectTransform)this.transform);
        _flowController.Configure(new CharacterSelectionFlowController.Config()
        {
            Resource = productResourceService.CharacterSelectionResources.CharacterSelectionFlowResource,
            FlowComplete = FlowComplete,
            FlowAbandoned = FlowAbandoned
        });
    }

    public void FlowComplete(List<HuntCharacterData> players)
    {
        Debug.Log(JsonConvert.SerializeObject(players));
        Debug.Log("Flow completed");
    }

    public void FlowAbandoned()
    {
        Debug.Log("Stopping Flow");
    }
}
