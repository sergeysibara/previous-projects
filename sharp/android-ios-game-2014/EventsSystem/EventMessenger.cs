using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class EventMessenger : MonoSingleton<EventMessenger>
{
    [Serializable]
    private class EventSubscribersPair
    {
        [Serializable]
        public class SubscriberActionPair
        {
            public Object Subscriber;
            public Action Action;
            public string ActionName;
        }

        public GameEvent Event;
        public List<SubscriberActionPair> SubscriberActionPairs = new List<SubscriberActionPair>();


        public void AddSubscriberActionPair(Object subscriber, Action action)
        {
            if (!ContainsSubscriber(subscriber))
                SubscriberActionPairs.Add(new SubscriberActionPair { Subscriber = subscriber, Action = action, ActionName = action.Method.Name });
            // если уже содержит подписчика, то он НЕ добавляется
        }

        private bool ContainsSubscriber(Object subscriber)
        {
            return SubscriberActionPairs.Any(s => Equals(s.Subscriber, subscriber));
        }        
    }

    [SerializeField]
    private List<EventSubscribersPair> _eventSubscribersPairs = new List<EventSubscribersPair>();


    /// <summary>
    /// Очищает до Awake, но перед Start
    /// </summary>
    private void OnLevelWasLoaded(int level)
    {
        //var a=_eventSubscribersPairs.Any(p => p.SubscriberActionPairs.Any(p2 => p2.Subscriber != null && p2.Subscriber.GetType() == typeof (UIPanelFix)));
        //if (a)
        //    Debug.LogWarning("UIPanelFix");
        _eventSubscribersPairs.Clear();
    }

    public static void Subscribe(GameEvent gameEvent, Object subscriber, Action action)
    {
        if (subscriber == null || action == null || Instance == null)
            return;

        var eventSubscribersPair = Instance._eventSubscribersPairs.FirstOrDefault(p => p.Event == gameEvent);
        if (eventSubscribersPair != null)
        {
            eventSubscribersPair.AddSubscriberActionPair(subscriber, action);
        }
        else
        {
            Instance.AddEventSubscribersPair(gameEvent, subscriber, action);
        }
    }

    public static void UnSubscribe(GameEvent gameEvent, Object subscriber)
    {
        if (subscriber == null || Instance==null)
            return;

        var eventSubscribersPair = Instance._eventSubscribersPairs.FirstOrDefault(p => p.Event == gameEvent);
        if (eventSubscribersPair != null)
        {
            var subscriberActionPair= eventSubscribersPair.SubscriberActionPairs.FirstOrDefault(p => p.Subscriber.GetInstanceID() == subscriber.GetInstanceID());
            if (subscriberActionPair!=null)
                eventSubscribersPair.SubscriberActionPairs.Remove(subscriberActionPair);
        }
    }

    public static void SendMessage(GameEvent gameEvent, System.Object sender)
    {
        if (Instance!=null)
            Instance.SendMessage(gameEvent);
    }

    public static void SendMessage(GameEvent gameEvent, Object sender)
    {
        if (Instance != null)
            Instance.SendMessage(gameEvent);
    }

    private void SendMessage(GameEvent gameEvent)
    {
        var eventSubscribersPair = _eventSubscribersPairs.FirstOrDefault(p => p.Event == gameEvent);
        if (eventSubscribersPair != null)
        {
            List<EventSubscribersPair.SubscriberActionPair> removedSubscribers = new List<EventSubscribersPair.SubscriberActionPair>();
            foreach (var pair in eventSubscribersPair.SubscriberActionPairs)
            {
                if (pair.Subscriber != null)
                    pair.Action();
                else
                    removedSubscribers.Add(pair);
            }

            //автоОтписка удаленных слушателей
            foreach (var item in removedSubscribers)
                eventSubscribersPair.SubscriberActionPairs.Remove(item);
        }
    }

    private void AddEventSubscribersPair(GameEvent gameEvent, Object subscriber, Action action)
    {
        var eventSubscribersPairs = Instance._eventSubscribersPairs;
        EventSubscribersPair eventSubscribersPair = new EventSubscribersPair {Event = gameEvent};
        eventSubscribersPair.AddSubscriberActionPair(subscriber, action);
        eventSubscribersPairs.Add(eventSubscribersPair);
    }

}
