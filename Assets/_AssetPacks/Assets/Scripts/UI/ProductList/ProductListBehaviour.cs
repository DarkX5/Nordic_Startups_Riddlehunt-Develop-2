using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Riddlehouse.Core.Helpers.Helpers;
using riddlehouse_libraries;
using UnityEngine;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;
using Zenject;

public interface IProductDataGetter
{
   public Task<List<ProductBasicsDTO>> GetCardList();
}

public class ProductDataGetter : IProductDataGetter
{
   public async Task<List<ProductBasicsDTO>> GetCardList()
   {
      ProductService productService = new ProductService(DataPathHelper.PersistentDataPath);
      var list = await productService.GetProductList();
      return list;
   }
}

public interface IProductList
{
   public Task Configure(Action _readyForDisplay);
   void SetSiblingIndex(int siblingIndex);
   void DestroyGameObject();
}
/// <summary>
/// Deprecated - has no effect on anything
/// </summary>
public interface IProductEvents 
{
   public void ProductStarted();
   public void ProductAborted();
   public void ProductCompleted();
}
public class ProductListBehaviour : MonoBehaviour, IProductList, IProductEvents
{
   public static IProductList Factory(ProductListBehaviour prefab, RectTransform parent)
   {
      ProductListBehaviour behaviour = Instantiate(prefab, parent);
      UIFitters uiFitters = new UIFitters();
      uiFitters.FitToFullscreen((RectTransform)behaviour.transform, parent);
      var dependencies = new Dependencies()
      {
         ContentParent = behaviour.contentBox,
         ProductDataGetter = new ProductDataGetter(),
         Instantiater = behaviour.Instantiater
      };
      behaviour.SetDependencies(dependencies);
      return behaviour;
   }
   [SerializeField] //can't be moved since it belongs to the monobehaviour.
   public Transform contentBox;
   [Inject] //can't be moved because zenject only injects into monobehaviours. (also added the auto-zen-inject component to the prefab)
   private IProductCardInstantiater Instantiater;

   public class Dependencies
   {
      public Transform ContentParent { get; set; }
      public IProductDataGetter ProductDataGetter { get; set; }
      public IProductCardInstantiater Instantiater { get; set; }
   }

   public Dependencies _dependencies { get; private set; }
   public void SetDependencies(Dependencies dependencies)
   {
      _dependencies = dependencies;
   }
   
   public async Task Configure(Action _readyForDisplay)
   {
      bool hasInvoked = false;
      int ReadyCount = 0;
      object MyLock = new object();
      
      var cardsData = (await _dependencies.ProductDataGetter.GetCardList());

     // var cardsData = (await _dependencies.ProductDataGetter.GetCardList());
      foreach (var cardData in cardsData)
      {
         var productCard = _dependencies.Instantiater.Create(cardData, (RectTransform)_dependencies.ContentParent, this);

         // - configure the newly created card.
         //When all cards are created, inform the MainController that the product list is ready for viewing.
         AddCardToList(productCard, cardData,() =>
            {
               lock (MyLock)
               {
                  ReadyCount++;
                  if (ReadyCount == cardsData.Count)
                  {
                     _readyForDisplay.Invoke();
                     hasInvoked = true;
                  }
               }
            });
      }

      if (ReadyCount == cardsData.Count && !hasInvoked)
      {
         _readyForDisplay.Invoke();
      }
   }
   
   private void AddCardToList(IProductCard productCard, ProductBasicsDTO productData, Action cardReady)
   {
         productCard.Configure(productData, cardReady);
   }

   public void ProductStarted() //should be deprecated
   {
   }

   public void ProductAborted() //should be deprecated
   {
   }

   public void ProductCompleted() //should be deprecated
   {
   }

   public void SetSiblingIndex(int siblingIndex)
   {
      transform.SetSiblingIndex(siblingIndex);
   }

   public void DestroyGameObject()
   {
      DestroyImmediate(gameObject);
   }
}