using System;


public class JsonDeserializeAttribute : Attribute
{
    public string Name = null;
    public bool Required = true;

    public JsonDeserializeAttribute()
    {
    }

    public JsonDeserializeAttribute(string name)
    {
        this.Name = name;
    }

    public JsonDeserializeAttribute(string name, bool required)
    {
        this.Name = name;
        this.Required = required;
    }

    public string GetName(string fieldName)
    {
        if (string.IsNullOrEmpty(Name)) return fieldName;
        return Name;
    }
}