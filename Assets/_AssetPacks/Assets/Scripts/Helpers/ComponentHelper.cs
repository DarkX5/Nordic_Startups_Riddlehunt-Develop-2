using System;
using UnityEngine;

namespace Riddlehouse.Core.Helpers.Helpers
{
    public class ComponentHelper<TBehaviour>
    {
        public TBehaviour GetBehaviourIfExists(GameObject go)
        {
            var behavior = go.GetComponent<TBehaviour>();
            if (behavior == null)
            {
                throw new ArgumentException(typeof(TBehaviour)+" missing on prefab");
            }
            return behavior;
        }

        public TBehaviour SetLogicInstance(TBehaviour value, TBehaviour field)
        {
            if (field == null)
            {
               return value;
            }
            throw new ArgumentException("Can only be set once.");
        }
    }
}