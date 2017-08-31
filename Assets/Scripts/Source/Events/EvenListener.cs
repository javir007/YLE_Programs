using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvenListener : MonoBehaviour
{
    [SerializeField]
    EventGroup group;

    IEventListener listener;

    void OnEnable()
    {
        AppEvents.Instance.Broadcasters[group].OnEventFired += OnChanged;
    }

    private void Awake()
    {
        listener = GetComponent<IEventListener>();
    }

    void OnDisable()
    {
        AppEvents.Instance.Broadcasters[group].OnEventFired -= OnChanged;
    }

    void OnChanged(string eventName, object content)
    {
        if (listener != null)
            listener.OnEvent(eventName, content);
        else
            Debug.LogWarning("IEventListener not found in " + gameObject.name);
    }
        
}
