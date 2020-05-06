using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class Bullet : MonoBehaviour{
    SocketIOComponent socket;
    private void Awake() {
        socket = FindObjectOfType<ConnectionManager>().socket;
        StartCoroutine(BulletUpdateMove());
    }
    public GameObject hitEffect;
    void OnCollisionEnter2D(Collision2D other) {
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        if(other.gameObject.GetComponent<TDSMovement>() != null){
            other.gameObject.GetComponent<TDSMovement>().HP--;
        }
        Destroy(effect, 0.25f);
        Destroy(gameObject);
    }

    IEnumerator BulletUpdateMove(){
        while(true){
            Dictionary<string, string> data = new Dictionary<string, string>();
            string roomName = FindObjectOfType<SessionManager>().roomName.ToString().Replace("\"",string.Empty);
            data.Add("roomName", roomName);
            data.Add("bulletName", gameObject.name);
            data.Add("x", transform.position.x.ToString());
            data.Add("y", transform.position.y.ToString());
            data.Add("angle", transform.rotation.z.ToString());
            JSONObject jSONObject = new JSONObject(data);
            socket.Emit("OnBulletUpdateMove" ,jSONObject);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
