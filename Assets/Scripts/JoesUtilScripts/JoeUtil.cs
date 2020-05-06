using System.Collections;
using System.Collections.Generic;
public static class JoeUtil{
    public static JSONObject JSONPacker(string contentName,string content){
        JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
        jSONObject.AddField(contentName, content);
        return jSONObject;
    }

    public static JSONObject JSONPacker(Dictionary<string,string> jsonContent){
        JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
        foreach(KeyValuePair<string,string> kvp in jsonContent){
            jSONObject.AddField(kvp.Key,kvp.Value);
        }
        return jSONObject;
    }
}
