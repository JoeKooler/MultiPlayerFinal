using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class LoginManager : MonoBehaviour{
    [SerializeField]SocketIOComponent socket;
    [SerializeField]Text idText;
    [SerializeField]InputField passwordField;
    [SerializeField]Text statusText;

    void Start(){
        socket = FindObjectOfType<ConnectionManager>().socket;
        socket.On("OnLoginStatus",OnLoginStatus);
        socket.On("OnRegisterStatus",OnRegisterStatus);
    }

    void Update(){
        
    }

    #region Callback Group
    void OnLoginStatus(SocketIOEvent obj){
        string status = obj.data["result"].ToString();
        status = status.Replace("\"", string.Empty);
        statusText.text = status;
        if(status == "Success"){
            FindObjectOfType<SessionManager>().myID = idText.text;
            SceneLoader.ToScene("MenuScene");
        }
    }

    void OnRegisterStatus(SocketIOEvent obj){
        string status = obj.data["result"].ToString();
        status = status.Replace("\"", string.Empty);
        statusText.text = status;
    }

    #endregion

    public void OnLogin(){
        Dictionary<string,string> data = new Dictionary<string, string>();
        data.Add("id",idText.text);
        data.Add("password",passwordField.text);
        JSONObject jSONObject = new JSONObject(data);
        socket.Emit("OnLogin",jSONObject);
    }
    public void OnRegister(){
        Dictionary<string,string> data = new Dictionary<string, string>();
        data.Add("id",idText.text);
        data.Add("password",passwordField.text);
        JSONObject jSONObject = new JSONObject(data);
        socket.Emit("OnRegister",jSONObject);
    }
}
