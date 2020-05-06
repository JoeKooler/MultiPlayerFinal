using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class LobbyManager : MonoBehaviour{
    SocketIOComponent socket;
    [SerializeField]string userID;
    [SerializeField]List<Text> playerSlots;
    [SerializeField]int playerCount;
    void Start(){
        socket = FindObjectOfType<ConnectionManager>().socket;
        socket.On("OnLeaveSuccess",OnLeaveSuccess);
        socket.On("OnRoomUpdate",OnRoomUpdate);
        socket.On("gogogo",Gogogo);
        userID = FindObjectOfType<SessionManager>().myID;
        OnRequestRoomUpdate();
    }

    void Update(){
        
    }

    void OnGoSuccess(SocketIOEvent obj){
        SceneLoader.ToScene("GameScene");
    }

    void OnLeaveSuccess(SocketIOEvent obj){
        FindObjectOfType<SessionManager>().roomName = string.Empty;
        // Debug.Log("Leave Success");
        SceneLoader.ToScene("MenuScene");
    }

    public void OnLeaveLobby(){
        // Debug.Log("Press Leave!");
        Dictionary<string,string> data = new Dictionary<string, string>();
        string lobbyNameText = FindObjectOfType<SessionManager>().roomName;
        string eiei = lobbyNameText.Replace("\"",string.Empty);
        data.Add("id", userID);
        data.Add("roomName", eiei);

        JSONObject jSONObject = new JSONObject(data);

        socket.Emit("OnLeaveLobby", jSONObject);
    }

    void OnRoomUpdate(SocketIOEvent obj){
        Debug.Log("Lobby Info " + obj.data["lobbyData"]);
        JSONObject payload = obj.data["lobbyData"];
        for(int i = 0 ; i < 4 ; i++){
            playerSlots[i].text = "";
            Debug.Log("Test payload " + payload[i]);
            Debug.Log("Test payload id" + payload[i]["id"].ToString().Replace("\"",string.Empty));
            string eiei = payload[i]["id"].ToString().Replace("\"",string.Empty);
            playerSlots[i].text = eiei;
            playerCount = obj.data["lobbyData"].Count;
        }
    }
    void Gogogo(SocketIOEvent obj){
        SceneLoader.ToScene("GameScene");
    }
    void OnRequestRoomUpdate(){
        Dictionary<string,string> data = new Dictionary<string, string>();
        string lobbyNameText = FindObjectOfType<SessionManager>().roomName;
        string eiei = lobbyNameText.Replace("\"",string.Empty);
        data.Add("roomName",eiei);
        JSONObject jSONObject = new JSONObject(data);
        socket.Emit("OnRequestRoomUpdate",jSONObject);
    }

    public void OnGo(){
        if(playerCount % 2 == 0 && playerCount > 0){
            Dictionary<string,string> data = new Dictionary<string, string>();
            string lobbyNameText = FindObjectOfType<SessionManager>().roomName;
            string eiei = lobbyNameText.Replace("\"",string.Empty);
            data.Add("roomName",eiei);
            JSONObject jSONObject = new JSONObject(data);
            socket.Emit("OnToGo",jSONObject);
        }
    }

}
