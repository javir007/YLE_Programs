using System;
using System.Collections.Generic;


public interface IJsonManual
{
    void Deserialize(Dictionary<string, object> json);

    object Serialize();
}