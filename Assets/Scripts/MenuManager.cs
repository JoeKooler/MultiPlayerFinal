using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;


public class MenuManager : MonoBehaviour{
    SocketIOComponent socket;
    List<string> lobbies;
    [SerializeField]GameObject lobbyPrefab;
    [SerializeField]Text lobbyName;
    [SerializeField]Text lobbyStatus;
    [SerializeField]string userID;
    List<GameObject> lobbyObjects;
    [SerializeField] GameObject lobbiesHolder;
    void Start(){
        lobbies = new List<string>();
        lobbyObjects = new List<GameObject>();
        userID = FindObjectOfType<SessionManager>().myID;
        socket = FindObjectOfType<ConnectionManager>().socket;
        socket.On("OnCreateLobbySuccess", OnCreateLobbySuccess);
        socket.On("OnJoinLobbySuccess", OnJoinLobbySuccess);
        socket.On("OnJoinLobbyFailed", OnJoinLobbyFailed);
        socket.On("OnUpdateLobby", OnUpdateLobby);
    }

    void Update(){
        
    }

    void OnCreateLobbySuccess(SocketIOEvent obj){
        SceneLoader.ToScene("LobbyScene");
    }

    void OnJoinLobbySuccess(SocketIOEvent obj){
        // Debug.Log("Success");
        string eiei = obj.data["roomName"].ToString().Replace("\"",string.Empty);
        FindObjectOfType<SessionManager>().roomName = eiei;
        SceneLoader.ToScene("LobbyScene");
    }

    void OnJoinLobbyFailed(SocketIOEvent obj){
        lobbyStatus.text = obj.data["err"].ToString();
    }

    void OnUpdateLobby(SocketIOEvent obj){
        lobbies.Clear();
        // Debug.Log("Lobbies " + obj.data["lobby"]);
        JSONObject payload = obj.data["lobby"];
        for(int i = 0 ; i < payload.Count ; i++){
            lobbies.Add(payload[i].ToString());
        }
        InstantiateLobbies();
    }

    void InstantiateLobbies(){
        DestroyOldLobbies();
        lobbyObjects.Clear();
        for(int i = 0 ; i < lobbies.Count ; i++ ){
            Vector3 spawnPos = new Vector3(760,840 - (60*i) ,0);
            lobbyObjects.Add(Instantiate(lobbyPrefab,spawnPos,Quaternion.identity));
            lobbyObjects[i].transform.parent = lobbiesHolder.transform;
            Button lobbyButton = lobbyObjects[i].GetComponentInChildren<Button>();
            Text lobbyNameText = lobbyObjects[i].GetComponentInChildren<Text>();
            lobbyNameText.text = lobbies[i];
            lobbyButton.onClick.AddListener(() => JoinLobby(lobbyNameText));
        }
    }

    void DestroyOldLobbies(){
        for(int i = 0 ; i < lobbyObjects.Count ; i++){
            Destroy(lobbyObjects[i]);
        }
    }

    public void CreateLobby(){
        Dictionary<string,string> data = new Dictionary<string, string>();
        data.Add("creator", userID);
        string eiei = lobbyName.text.Replace("\"",string.Empty);
        data.Add("roomName", eiei);

        FindObjectOfType<SessionManager>().roomName = eiei;

        JSONObject jSONObject = new JSONObject(data);

        socket.Emit("OnCreateLobby", jSONObject);
    }

    public void JoinLobby(Text lobbyNameText){
        // Debug.Log("Press Join!");
        Dictionary<string,string> data = new Dictionary<string, string>();
        string eiei = lobbyNameText.text.Replace("\"",string.Empty);
        data.Add("id", userID);
        data.Add("roomName", eiei);

        // Debug.Log("Lobbynametext " + lobbyNameText.text);
        JSONObject jSONObject = new JSONObject(data);

        socket.Emit("OnJoinLobby", jSONObject);
    }

    public void RefreshLobby(){
        socket.Emit("OnUpdateLobbyRequest");
    }
}
