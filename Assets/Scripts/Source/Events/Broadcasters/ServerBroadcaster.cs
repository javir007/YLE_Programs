using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerBroadcaster : IEventBroadcaster {

	public class EventName
	{
		public static readonly string OnRequest = "data_request";
		public static readonly string OnLoaded = "data_loaded";
	}

    public EventGroup Group{ get { return EventGroup.Server; }}

    public Action<string, object> OnEventFired { get; set; }

	public void Fire(string eventName, object content = null)
	{
		if (OnEventFired != null)
			OnEventFired(eventName, content);
	}

  
}
