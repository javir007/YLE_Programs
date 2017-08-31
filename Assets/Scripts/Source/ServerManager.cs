using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour,IEventListener{  

	string jsonUrl = "https://external.api.yle.fi/v1/programs/items.json?app_id=606c8dd7&app_key=6b5dfd20fc5abe41a53bd591df933f2d&limit=100&availability=ondemand&mediaobject=video&q=";



	public void DataRequest(string titleRequested){
        if (!string.IsNullOrEmpty(titleRequested))
        {
            

            StartCoroutine(GetData(jsonUrl+titleRequested));
		}
	}

	IEnumerator GetData(string url){
        OperationResult<MainData> operation = API<MainData>(url);
		while (!operation.IsReady)
			yield return 0;
        
        AppEvents.Instance.Server.Fire(ServerBroadcaster.EventName.OnLoaded, operation.Data);

	}

	public OperationResult<T> API<T>(string url) where T : class, new(){
		var operation = new OperationResult<T>();
		StartCoroutine(APIInternal<T>(url, operation));
		return operation;
	}

	IEnumerator APIInternal<T>(string url, OperationResult<T> operation) where T : class, new(){
        //WWW www = new WWW(url);
        UnityWebRequest web = UnityWebRequest.Get(url);
        yield return web.Send();
        operation.ResolveData(web.downloadHandler.text);
		operation.IsReady = true;
	}

    public void OnEvent(string eventName, object content){
        if (eventName == ServerBroadcaster.EventName.OnRequest)
		{
		    DataRequest((string)content);
		}
       
    }
}
