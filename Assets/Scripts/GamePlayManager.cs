using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class GamePlayManager : MonoBehaviour{
    SocketIOComponent socket;
    public static int uniqueBullet = 0;
    [SerializeField]GameObject clientPlayer;
    [SerializeField]List<GameObject> players;
    [SerializeField]List<string> playerIDLists;
    [SerializeField]string userID;
    [SerializeField]List<Transform> spawners;
    [SerializeField]List<GameObject> playerPrefabs;
    [SerializeField]List<GameObject> bullets;
    [SerializeField]List<string> bulletsStrings;
    [SerializeField]GameObject bulletPrefab;
    int playerNum;
    bool isUpdated = false;
    private void Start() {
        socket = FindObjectOfType<ConnectionManager>().socket;
        userID = FindObjectOfType<SessionManager>().myID;
        playerIDLists = new List<string>();
        players = new List<GameObject>();
        bullets = new List<GameObject>();
        bulletsStrings = new List<string>();
        socket.On("OnInGameInfoUpdate", RoomUpdate);
        socket.On("UpdateMove", UpdateOthers);
        socket.On("UpdateBullets",UpdateBullets);
        socket.On("DieStatus", CheckDie);
        socket.On("BacktoLobby", BacktoLobby);
        RequestLobbyInfo();
    }
    
    // Update is called once per frame
    void Update(){
        if(clientPlayer != null){
            if(clientPlayer.GetComponent<TDSMovement>().HP <= 0){
                Die();
            }
        }
    }
    void CheckDie(SocketIOEvent obj){
        string id = obj.data["id"].ToString().Replace("\"",string.Empty);
        string roomName = FindObjectOfType<SessionManager>().roomName.ToString().Replace("\"",string.Empty);
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("id", userID);
        data.Add("roomName",roomName);
        JSONObject jSONObject = new JSONObject(data);
        if(id != userID){
            socket.Emit("Alive",jSONObject);
        }
    }

    void BacktoLobby(SocketIOEvent obj){
        SceneLoader.ToScene("MenuScene");
    }
    void Die(){
        Dictionary<string, string> data = new Dictionary<string, string>();
        string roomName = FindObjectOfType<SessionManager>().roomName.ToString().Replace("\"",string.Empty);
        data.Add("roomName", roomName);
        data.Add("playerNumber",playerNum.ToString());
        data.Add("uid", userID);
        JSONObject jSONObject = new JSONObject(data);
        socket.Emit("Die" ,jSONObject);
        SceneLoader.ToScene("MenuScene");
    }
    void RoomUpdate(SocketIOEvent obj){
        if(!isUpdated){
            Debug.Log("Room Update in game");
            JSONObject payload = obj.data["lobbyData"];
            for(int i = 0 ; i < payload.Count ; i++){
                playerIDLists.Add(payload[i]["id"].ToString().Replace("\"",string.Empty));
                if(payload[i]["id"].ToString().Replace("\"",string.Empty) == userID){
                    playerNum = i;
                }
            }
            isUpdated = true;
            CreatePlayers();
        }
    }

    void CreatePlayers(){
        Debug.Log("Trying to create players!");
        for(int i = 0; i < playerIDLists.Count ; i++){
            Debug.Log("Create One" + playerIDLists.Count + "Index " + i);
            players.Add(Instantiate(playerPrefabs[i], spawners[i].position ,Quaternion.identity));
            if(playerNum == i){
                clientPlayer = players[i];
                clientPlayer.GetComponent<TDSMovement>().canMove = true;
            }
            StartCoroutine(ClientUpdateMove());
        }
    }

    public void RequestLobbyInfo(){
        Dictionary<string, string> data = new Dictionary<string, string>();
        string roomName = FindObjectOfType<SessionManager>().roomName.ToString().Replace("\"",string.Empty);
        data.Add("roomName", roomName);
        JSONObject jSONObject = new JSONObject(data);
        socket.Emit("OnInGameRequestUpdate",jSONObject);
    }

    public void UpdateOthers(SocketIOEvent obj){
        string playerIDNum = obj.data["playerNumber"].ToString().Replace("\"",string.Empty);
        int actualNum = int.Parse(playerIDNum);
        if(playerNum != actualNum){
            float x = float.Parse( obj.data["x"].ToString().Replace("\"",string.Empty));
            float y = float.Parse( obj.data["y"].ToString().Replace("\"",string.Empty));
            float angle = float.Parse( obj.data["angle"].ToString().Replace("\"",string.Empty));
            players[actualNum].transform.position = new Vector3 (x,y,0);
            players[actualNum].transform.rotation = new Quaternion(
                players[actualNum].transform.rotation.x,
                players[actualNum].transform.rotation.y,
                angle,
                players[actualNum].transform.rotation.w
            );
        }
    }

    public void UpdateBullets(SocketIOEvent obj){
        string bulletsName = obj.data["bulletName"].ToString().Replace("\"",string.Empty);
        float x = float.Parse( obj.data["x"].ToString().Replace("\"",string.Empty));
        float y = float.Parse( obj.data["y"].ToString().Replace("\"",string.Empty));
        float angle = float.Parse( obj.data["angle"].ToString().Replace("\"",string.Empty));
        Vector3 pos = new Vector3(x,y,0);
        Quaternion qua = new Quaternion(0,0,angle,0);
        if(!bulletsStrings.Contains(bulletsName)){
            bulletsStrings.Add(bulletsName);
            bullets.Add(Instantiate(bulletPrefab, pos , qua));
            bullets[bullets.Count].gameObject.name = bulletsName;
        }else{
            int bulletIndex = bulletsStrings.IndexOf(bulletsName);
            bullets[bulletIndex].transform.position = new Vector3 (x,y,0);
            bullets[bulletIndex].transform.rotation = new Quaternion(
                bullets[bulletIndex].transform.rotation.x,
                bullets[bulletIndex].transform.rotation.y,
                angle,
                bullets[bulletIndex].transform.rotation.w);
        }
        Debug.Log("Bullets" + bullets);
    }

    IEnumerator ClientUpdateMove(){
        while(true){
            Debug.Log("Send!!!!!");
            Dictionary<string, string> data = new Dictionary<string, string>();
            string roomName = FindObjectOfType<SessionManager>().roomName.ToString().Replace("\"",string.Empty);
            data.Add("roomName", roomName);
            data.Add("playerNumber",playerNum.ToString());
            data.Add("uid", userID);
            data.Add("x", clientPlayer.transform.position.x.ToString());
            data.Add("y", clientPlayer.transform.position.y.ToString());
            data.Add("angle", clientPlayer.transform.rotation.z.ToString());
            JSONObject jSONObject = new JSONObject(data);
            socket.Emit("OnClientUpdateMove" ,jSONObject);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
