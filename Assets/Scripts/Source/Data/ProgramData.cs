using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramData  {

    public string Language = "fi";
	[JsonDeserialize("title")]
    public Dictionary<string, string> Titles = new Dictionary<string, string>();
    [JsonDeserialize("typeMedia", false)]
    public string mediaType;
	[JsonDeserialize("duration", false)]
	public string durationProgram;
	[JsonDeserialize("id", false)]
	public string idProgram;
	[JsonDeserialize("indexDataModified", false)]
	public string indexData;
    [JsonDeserialize("collection", false)]
	public string programCollection;


}


