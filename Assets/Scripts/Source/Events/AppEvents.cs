using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppEvents 
{
    public static AppEvents Instance = new AppEvents();
    public Dictionary<EventGroup, IEventBroadcaster> Broadcasters;
    public ServerBroadcaster Server = new ServerBroadcaster();


    public AppEvents()
    {
        Broadcasters = new Dictionary<EventGroup, IEventBroadcaster>();
     
        Broadcasters.Add(EventGroup.Server,Server);
    }
}
