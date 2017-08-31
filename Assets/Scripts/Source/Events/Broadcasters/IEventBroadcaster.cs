using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventBroadcaster
{
    EventGroup Group { get; }
    Action<string, object> OnEventFired { get; set; }
    void Fire(string eventName, object content = null);
}

public interface IEventListener
{
    void OnEvent(string eventName, object content);
}

public enum EventGroup
{
   Server
}