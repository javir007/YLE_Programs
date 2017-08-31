using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MiniJSON;
using System.Globalization;

public class JsonHelper
{
    public static T Deserialize<T>(string json, bool reqiureAttribute = true) where T : new()
    {
        var jsonDict = Json.Deserialize(json);
        return DeserializeFromJson<T>(jsonDict, reqiureAttribute);
    }

    public static T DeserializeFromJson<T>(object json, bool reqiureAttribute = true) where T : new()
    {
        return (T)DeserializeJson(typeof(T), json, reqiureAttribute);
    }

    public static object DeserializeJson(Type wantedType, object jsonObj, bool reqiureAttribute = true)
    {
        if (wantedType == typeof(Int32))
            return (int)((long)jsonObj);
        if (wantedType == typeof(Int64))
            return (long)jsonObj;
        if (wantedType == typeof(UInt64))
            return UInt64.Parse(jsonObj as string);
        if (wantedType == typeof(byte))
            return (byte)((long)jsonObj);
        if (wantedType == typeof(float))
            return jsonObj is double ? (float)((double)(jsonObj)) : (float)((long)(jsonObj));
        if (wantedType == typeof(double))
            return jsonObj is double ? (double)(jsonObj) : (double)((long)(jsonObj));
        if (wantedType == typeof(decimal))
            return (decimal)(jsonObj is double ? (double)(jsonObj) : (double)((long)(jsonObj)));
        if (wantedType == typeof(bool))
            return (bool)jsonObj;
        if (wantedType == typeof(string))
            return ((string)jsonObj);
        if (wantedType == typeof(DateTime))
            return DateTime.Parse((string)jsonObj, new CultureInfo("en-us"), DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        if (wantedType == typeof(object))
            return jsonObj;
        if (wantedType.IsEnum)
        {
            var val = jsonObj as string;

            if (val == null)
                return Enum.ToObject(wantedType, 0);

            if (!reqiureAttribute)
                return Enum.Parse(wantedType, val);

            var members = wantedType.GetMembers(BindingFlags.Static | BindingFlags.Public);
            foreach (var el in members)
            {
                var attrs = el.GetCustomAttributes(typeof(JsonDeserializeAttribute), false);
                var attr = attrs.Length > 0 ? attrs[0] as JsonDeserializeAttribute : null;
                if (attr != null && attr.Name == val)
                    return Enum.Parse(wantedType, el.Name);
            }
            return null;
        }
        if (wantedType.IsGenericType && wantedType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            if (jsonObj == null) return null;

            var innerType = Nullable.GetUnderlyingType(wantedType);
            return DeserializeJson(innerType, jsonObj, reqiureAttribute);
        }
        if (wantedType.FullName.StartsWith("System.Collections.Generic.List`1"))
        {
            var constructor = wantedType.GetConstructor(new Type[] { });

            var newObj = constructor.Invoke(new object[] { }) as IList;

            var innerType = wantedType.GetGenericArguments()[0];
            foreach (var el in jsonObj as List<object>)
                newObj.Add(DeserializeJson(innerType, el, reqiureAttribute));

            return newObj;
        }
        if (wantedType.FullName.StartsWith("System.Collections.Generic.Dictionary`2"))
        {
            if (jsonObj == null) return null;

            var constructor = wantedType.GetConstructor(new Type[] { });

            var newObj = constructor.Invoke(new object[] { }) as IDictionary;

            var argsTypes = wantedType.GetGenericArguments();
            var innerType = argsTypes[0];
            var innerType2 = argsTypes[1];

            Func<string, object> keyFunc = null;
            if (innerType == typeof(string))
                keyFunc = el => el;
            else if (innerType == typeof(int))
                keyFunc = el => Int32.Parse(el);
            else if (innerType.IsEnum)
                keyFunc = el => DeserializeJson(innerType, el, reqiureAttribute);
            else
                throw new Exception("Dictionalty deserialization error: key type should be string or int. Currently it is " + innerType);

            var jsonDict = jsonObj as Dictionary<string, object>;
            foreach (var el in jsonDict)
            {
                var val = DeserializeJson(innerType2, el.Value, reqiureAttribute);
                newObj.Add(keyFunc(el.Key), val);
            }

            return newObj;
        }
        else
        {
            var constructor = wantedType.GetConstructor(new Type[] { });

            if (constructor == null)
                throw new Exception("Json parser does not support " + wantedType);

            var newObj = constructor.Invoke(new object[] { });

            var jsonDict = jsonObj as Dictionary<string, object>;

            if (typeof(IJsonManual).IsInstanceOfType(newObj))
            {
                var iObj = newObj as IJsonManual;
                iObj.Deserialize(jsonDict);
                return iObj;
            }

            foreach (var el in GetProperties(wantedType, reqiureAttribute))
            {
                object jsonValue;
                if (jsonDict.TryGetValue(el.Key.GetName(el.Value.Name), out jsonValue))
                    el.Value.SetValue(newObj, DeserializeJson(el.Value.FieldType, jsonValue, reqiureAttribute));
                else if (el.Key.Required)
                    throw new Exception("Json doesn't has field " + el.Key.Name + " for required member " + el.Value.Name + ", " + wantedType);
            }

            return newObj;
        }
    }

    public static string SerializeJson(object obj, bool reqiureAttribute = true)
    {
        return Json.Serialize(SerializeMiniJson(obj, reqiureAttribute));
    }

    static object SerializeMiniJson(object obj, bool reqiureAttribute = true)
    {
        if (obj == null) return null;

        var type = obj.GetType();

        if (type == typeof(Int32))
            return obj;
        if (type == typeof(Int64))
            return obj;
        if (type == typeof(UInt64))
            return obj;
        if (type == typeof(float))
            return obj;
        if (type == typeof(double))
            return obj;
        if (type == typeof(decimal))
            return obj;
        if (type == typeof(bool))
            return obj;
        if (type == typeof(string))
            return obj.ToString();
        if (type.IsEnum)
        {
            var members = type.GetMembers(BindingFlags.Static | BindingFlags.Public);
            foreach (var el in members)
            {
                if (el.Name == obj.ToString())
                {
                    if (!reqiureAttribute)
                        return obj.ToString();

                    var attrs = el.GetCustomAttributes(typeof(JsonDeserializeAttribute), false);
                    var attr = attrs.Length > 0 ? attrs[0] as JsonDeserializeAttribute : null;
                    if (attr != null)
                        return attr.Name;
                }
            }
            return null;
        }
        if (type.FullName.StartsWith("System.Collections.Generic.List`1"))
        {
            var res = new List<object>();
            foreach (var el in obj as IEnumerable)
                res.Add(SerializeMiniJson(el, reqiureAttribute));
            return res;
        }
        if (obj is IDictionary)
        {
            var dic = obj as IDictionary;
            var res = new Dictionary<string, object>();
            foreach (var el in dic.Keys)
            {
                object value = dic[el];
                var serValue = SerializeMiniJson(value, reqiureAttribute);
                res[el.ToString()] = serValue;
            }
            return res;
        }
        else
        {
            if (type.IsAssignableFrom(typeof(IJsonManual)))
            {
                var iObj = obj as IJsonManual;
                return iObj.Serialize();
            }

            var res = new Dictionary<string, object>();
            foreach (var el in GetProperties(type, reqiureAttribute))
            {
                object value = el.Value.GetValue(obj);
                var serValue = SerializeMiniJson(value, reqiureAttribute);
                res[el.Key.GetName(el.Value.Name)] = serValue;
            }
            return res;
        }
    }

    static List<KeyValuePair<JsonDeserializeAttribute, FieldInfo>> GetProperties(Type type, bool reqiureAttribute)
    {
        var res = new List<KeyValuePair<JsonDeserializeAttribute, FieldInfo>>();
        foreach (var el in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (!reqiureAttribute)
                res.Add(new KeyValuePair<JsonDeserializeAttribute, FieldInfo>(new JsonDeserializeAttribute(el.Name), el));
            else
            {
                var attrArr = el.GetCustomAttributes(typeof(JsonDeserializeAttribute), true);
                if (attrArr != null && attrArr.Length > 0)
                    res.Add(new KeyValuePair<JsonDeserializeAttribute, FieldInfo>(attrArr[0] as JsonDeserializeAttribute, el));
            }
        }
        return res;
    }
}