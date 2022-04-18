using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.resources;
using UnityEngine;
using Color = UnityEngine.Color;

public interface IWispFooter
{
   public void Configure(Color frameColor, Sprite icon, Action acceptAction, Action abortAction);
}
public class WispFooter : MonoBehaviour, IWispFooter
{
   public class Dependencies
   {
      public ICharacterIconDisplay CharacterIconDisplay;
   }

   public void Initialize()
   {
      _characterIconDisplay.Initialize();
      SetDependencies(new Dependencies()
      {
         CharacterIconDisplay = _characterIconDisplay
      });
   }
   public Dependencies _dependencies { get; private set; }

   public void SetDependencies(Dependencies dependencies)
   {
      _dependencies = dependencies;
   }
   
   [SerializeField] private CharacterIconDisplay _characterIconDisplay;
   private Action _acceptAction;
   private Action _abortAction;
   public void Configure(Color frameColor, Sprite icon, Action acceptAction, Action abortAction)
   {
      _dependencies.CharacterIconDisplay.Configure(frameColor, icon);
      _acceptAction = acceptAction;
      _abortAction = abortAction;
   }
   public void AcceptBtn()
   {
      _acceptAction.Invoke();
   }
   public void AbortBtn() 
   {
      _abortAction.Invoke();
   }
}
