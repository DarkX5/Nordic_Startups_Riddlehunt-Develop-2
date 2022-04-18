using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StopControllers
{
    public interface IStopControllerContainer
    {
        public void Add(IStopController controller);
        public IStopController Get(string id);
        public void RemoveAndDestroyAllControllers();
    }

    public class StopControllerContainer : IStopControllerContainer
    {
        private Dictionary<string, IStopController> activeControllers;

        public StopControllerContainer()
        {
            activeControllers = new Dictionary<string, IStopController>();
        }
        public void Add(IStopController controller)
        {
            activeControllers.Add(controller.GetId(),controller);
        }

        public IStopController Get(string id)
        {
            if(activeControllers.ContainsKey(id))
                return activeControllers[id];
            return null;
        }

        public void RemoveAndDestroyAllControllers()
        {
            foreach (var key in activeControllers.Keys)
            {
                activeControllers[key].DestroySelf();
            }
            activeControllers = new Dictionary<string, IStopController>();
        }
    }
}