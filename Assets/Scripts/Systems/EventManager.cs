using System;
using Utils;

namespace Systems
{
    public class EventManager : MonoBehaviourSingleton<EventManager>
    {
        public Action OnRadialGridCreated;
        public void RadialGridCreated()
        {
            print("Radial Grid Created");
            OnRadialGridCreated?.Invoke();
        }
    }
}