using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramData  {

    public string Language = "fi";
	[JsonDeserialize("title")]
    public Dictionary<string, string> Titles = new Dictionary<string, string>();
}


