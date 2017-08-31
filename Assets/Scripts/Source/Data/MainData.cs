using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainData  {
    
	

	[JsonDeserialize("data")]
    public List<ProgramData> programs = new List<ProgramData>();

	public static MainData Load(string jsonSource)
	{

		MainData demoData = null;
		if (!string.IsNullOrEmpty(jsonSource))
		{
			demoData = JsonHelper.Deserialize<MainData>(jsonSource);
			Debug.Log("DemoData loaded succesfully with json: " + jsonSource);
		}
		else
			Debug.LogError("The Json source was null or empty");

		return demoData;
	}
}
