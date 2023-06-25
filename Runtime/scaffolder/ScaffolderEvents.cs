using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// where to plug logic/capacity to react to specific external
/// (idealy only global system related) events
/// ex : plug / unplug controller
/// </summary>
namespace fwp.engine.scaffolder
{
    static public class ScaffolderEvents
    {

        static public Dictionary<string, LogicEvent> events = new Dictionary<string, LogicEvent>();

        /* basic declaration */
        static public T createEvent<T>(string name) where T : LogicEvent
        {
            if (!events.ContainsKey(name)) events.Add(name, default(T));
            return events[name] as T;
        }

        /* if params are T on event creation */
        static public LogicEvent createEvent(string name, LogicEvent newEvent)
        {
            if (!events.ContainsKey(name)) events.Add(name, newEvent);
            return events[name];
        }

    }

    public class LogicEvent
    {
        public string name = "";
    }

    public class LogicEventController : LogicEvent
    {
        public Action<int> onControllerConnected;
        public Action<int> onControllerDisconnected;
    }
}
