using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class ChatManager : MonoBehaviour{
    SocketIOComponent socket;
    [SerializeField]Text chatText;
    [SerializeField]InputField sentTextField;
    [SerializeField]SessionManager sessionManager;
    [SerializeField]string playerID;
    [SerializeField]string roomName;
    enum RoomType {
        Menu,
        Lobby
    }
    [SerializeField]RoomType roomType;

    void Start(){
        sessionManager = FindObjectOfType<SessionManager>();
        playerID = sessionManager.myID;
        socket = FindObjectOfType<ConnectionManager>().socket;
        socket.On("OnClientChatUpdate", OnClientChatUpdate);
        switch(roomType){
            case RoomType.Menu : {
                roomName = "global";
                break;
            }
            case RoomType.Lobby : {
                roomName = FindObjectOfType<SessionManager>().roomName;
                break;
            }
        }
    }

    void Update(){
        
    }

    public void OnSendText(){
        string message = GetFieldText();
        ResetChatBox();
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("sender",playerID);
        data.Add("message",message);
        data.Add("channel",roomName);
        JSONObject jSONObject = new JSONObject(data);
        socket.Emit("OnClientSendMessage", jSONObject);
    }

    #region Callback Group
    void OnClientChatUpdate(SocketIOEvent obj){
        Debug.Log("OnClientChatUpdate : "+ obj.data.ToString());
        chatText.text += obj.data["sender"];
        chatText.text += " ";
        chatText.text += obj.data["message"] + "\n";
    }

    #endregion

    string GetFieldText(){
        sentTextField.Select();
        return sentTextField.text;
    }
    void ResetChatBox(){
        sentTextField.Select();
        sentTextField.text = "";
    }
}
