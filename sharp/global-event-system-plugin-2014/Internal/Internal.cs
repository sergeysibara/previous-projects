using System;
using System.Collections.Generic;

namespace EventSystem.Internal
{
    [Serializable]
    internal class SubscriberActionPairBase
    {
        public Object Subscriber;
        public string SubscriberName;
        public string ActionName;
    }

    [Serializable]
    internal class SubscriberActionPair : SubscriberActionPairBase
    {
        public Action Callback;
    }

    [Serializable]
    internal class SubscriberActionPair<T> : SubscriberActionPairBase
    {
        public Action<T> Callback;
    }

    [Serializable]
    internal class EventInfo
    {
        public GameEvent Event;
        public List<SubscriberInfo> Subscribers = new List<SubscriberInfo>();



        [Serializable]
        internal class SubscriberInfo
        {
            public UnityEngine.Object Subscriber;
            public string SubscriberName;
            public string ActionName;

            public void Add(Object obj, string subscriberName, string actionName)
            {
                SubscriberName = subscriberName;
                ActionName = actionName;
                Subscriber = obj as UnityEngine.Object;
            }
        }
    }

    internal static class Utils
    {
        public static bool IsActive(object obj)
        {
            var monobeh = obj as UnityEngine.MonoBehaviour;
            return (ReferenceEquals(monobeh,null) || (monobeh.gameObject.activeInHierarchy && monobeh.enabled));
        }

        public static void RemoveItems<T>(List<T> pairs, List<T> removedPairs)
        {
            foreach (var rp in removedPairs)
            {
                pairs.Remove(rp);
            }
        }

    }

}
