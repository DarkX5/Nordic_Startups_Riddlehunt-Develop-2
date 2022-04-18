using System.Threading.Tasks;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.models;
using UnityEngine;

public interface IProductCardStartPanelDataGetter
{
   public Task<StartPanelData> GetPanelData(string id);
}

public class ProductCardStartPanelDataGetter : IProductCardStartPanelDataGetter
{
    public ProductCardStartPanelDataGetter()
    {
    }
    public  async Task<StartPanelData> GetPanelData(string id)
    {
       ProductService productService = new ProductService(DataPathHelper.PersistentDataPath);
       var panelData = await productService.GetStartPanelData(id);
       return panelData;
    }
}
