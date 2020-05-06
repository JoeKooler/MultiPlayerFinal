using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

[RequireComponent(typeof(SocketIOComponent))]
public class PlayerPosUpdater : MonoBehaviour{
    SocketIOComponent socket;
    void Start(){
        socket = GetComponent<SocketIOComponent>();
        StartCoroutine(UpdateClientPlayerData());
    }

    void Update(){

    }

    IEnumerator UpdateClientPlayerData(){
        while (true){
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("x", transform.position.x.ToString());
            data.Add("Z", transform.position.z.ToString());
            data.Add("Angle", transform.rotation.z.ToString());

            JSONObject jSONObject = new JSONObject(data);

            socket.Emit("OnClientUpdateMove", jSONObject);
            yield return new WaitForSeconds(1f);
        }
    }
}
