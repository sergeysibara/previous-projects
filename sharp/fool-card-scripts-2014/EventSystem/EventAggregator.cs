using System;
using System.Collections.Generic;
using System.Linq;
using EventSystem.Internal;
using UnityEngine;
using Object = System.Object;

public class EventAggregator : EventSystem.MonoSingleton<EventAggregator>
{
    public static bool IsApplicationShuttingDown
    {
        get { return Instance == null; }
    }

    [SerializeField]
    private readonly Dictionary<GameEvent, List<SubscriberActionPairBase>> _eventDictionary = new Dictionary<GameEvent, List<SubscriberActionPairBase>>();

    private void OnLevelWasLoaded(int level)
    {
        _eventDictionary.Clear();		
    }

    public static void Subscribe(GameEvent gameEvent, Object subscriber, Action action, string actionName = "", string subscriberName = "")
    {
        if (subscriber == null || action == null || Instance == null)
            return;

        if (!Instance._eventDictionary.ContainsKey(gameEvent))
            Instance._eventDictionary.Add(gameEvent, new List<SubscriberActionPairBase>());

        var pairs = Instance._eventDictionary[gameEvent];
        if (!ContainsSubscriber(pairs, subscriber))
        {
            AddSubscriberActionPair(pairs, subscriber, action, actionName, subscriberName);
        }
    }

    public static void Subscribe<T>(GameEvent gameEvent, Object subscriber, Action<T> action, string actionName = "", string subscriberName = "")
    {
        if (subscriber == null || action == null || Instance == null)
            return;

        if (!Instance._eventDictionary.ContainsKey(gameEvent))
            Instance._eventDictionary.Add(gameEvent, new List<SubscriberActionPairBase>());

        var pairs = Instance._eventDictionary[gameEvent];
        if (!ContainsSubscriber(pairs, subscriber))
        {
            AddSubscriberActionPair(pairs, subscriber, action, actionName, subscriberName);
        }
    }

    public static void UnSubscribe(GameEvent gameEvent, Object subscriber)
    {
        if (subscriber == null || Instance == null)
            return;

        var pairs = Instance._eventDictionary[gameEvent];
        if (pairs != null)
        {
            var subscriberActionPair = pairs.FirstOrDefault(p => ReferenceEquals(p.Subscriber, subscriber));
            if (subscriberActionPair != null)
                pairs.Remove(subscriberActionPair);
        }
    }

    public static void Publish(GameEvent gameEvent, object sender, bool publishFromInactive = false, bool publichToInactive = false, string publisherName = null)
    {
		if (!publishFromInactive && !EventSystem.Internal.Utils.IsActive(sender))
            return;
        if (Instance == null)
            return;

        if (!Instance._eventDictionary.ContainsKey(gameEvent))
            return;

        var pairs = Instance._eventDictionary[gameEvent];
        InvokeWithRemovingUnused(pairs, (P) => ((SubscriberActionPair) P).Callback(), true);
    }

    public static void PublishT<T>(GameEvent gameEvent, object sender, T data, bool publishFromInactive = false, bool publichToInactive = false, string publisherName = null)
    {
		if (!publishFromInactive && !EventSystem.Internal.Utils.IsActive(sender))
            return;
        if (Instance == null)
            return;

        if (!Instance._eventDictionary.ContainsKey(gameEvent))
            return;

        var pairs = Instance._eventDictionary[gameEvent];
        InvokeWithRemovingUnused(pairs, (P) => ((SubscriberActionPair<T>) P).Callback(data), publichToInactive);
    }


    private static bool ContainsSubscriber(IEnumerable<SubscriberActionPairBase> pairs, Object subscriber)
    {
        return pairs.Any(s => ReferenceEquals(s.Subscriber, subscriber));
    }

    private static void AddSubscriberActionPair(List<SubscriberActionPairBase> pairs, Object subscriber, Action action, string actionName, string subscriberName)
    {
        var sName = DetermineName(subscriber, subscriberName);
        var aName = (string.IsNullOrEmpty(actionName)) ? action.Method.Name : actionName;

		var newPair = new SubscriberActionPair { Subscriber = subscriber, Callback = action, ActionName = aName, SubscriberName = sName};
        pairs.Add(newPair);
    }

    private static void AddSubscriberActionPair<T>(List<SubscriberActionPairBase> pairs, Object subscriber, Action<T> action, string actionName, string subscriberName)
    {
        var sName = DetermineName(subscriber, subscriberName);
        var aName = (string.IsNullOrEmpty(actionName)) ? action.Method.Name : actionName;
		var newPair = new SubscriberActionPair<T> { Subscriber = subscriber, Callback = action, ActionName = aName, SubscriberName = sName};
        pairs.Add(newPair);
    }

    private static void InvokeWithRemovingUnused<T>(List<T> pairs, Action<SubscriberActionPairBase> action, bool notifyInactive) where T : SubscriberActionPairBase
    {
        List<T> removedPairs = new List<T>();
        foreach (T pair in pairs)
        {
            if (ReferenceEquals(pair.Subscriber, null))
            {
                removedPairs.Add(pair);
            }
			else if (notifyInactive || EventSystem.Internal.Utils.IsActive(pair.Subscriber))
            {
                action.Invoke(pair);
            }
        }
		EventSystem.Internal.Utils.RemoveItems(pairs, removedPairs);
    }

    /// <summary>
    /// Determine subscriberName or pubscriberName depending on object type.
    /// </summary>
    private static string DetermineName(Object @object, string objectName)
    {
        if (string.IsNullOrEmpty(objectName)) 
        {
            var obj = @object as UnityEngine.Object;
            return !ReferenceEquals(obj, null) ? obj.name : @object.ToString();
        }
        return objectName;
    }

    private int _gizmoDrawCounter = 100;

    [SerializeField]
    private List<EventInfo> _eventsInfo = new List<EventInfo>(); //only for inspector 

    private void OnDrawGizmosSelected()
    {
        _gizmoDrawCounter++;
        if (_gizmoDrawCounter % 100 == 0) //condition for reduce a load during rendering
        {
            _eventsInfo.Clear();
            foreach (var keyValuePair in _eventDictionary)
            {
                EventInfo eInfo = new EventInfo();
                eInfo.Event = keyValuePair.Key;
                foreach (var s in keyValuePair.Value)
                {
                    EventInfo.SubscriberInfo sInfo = new EventInfo.SubscriberInfo();
                    sInfo.Add(s.Subscriber, s.SubscriberName, s.ActionName);
                    eInfo.Subscribers.Add(sInfo);
                }

                _eventsInfo.Add(eInfo);
            }
        }
    }

}
