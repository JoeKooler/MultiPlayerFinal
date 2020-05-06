using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

[RequireComponent(typeof(SocketIOComponent))]
public class ConnectionManager : MonoBehaviour{

    public SocketIOComponent socket;
    private void Awake() {
        DontDestroyOnLoad(this);
        socket = GetComponent<SocketIOComponent>();
    }
    void Start(){
    }

    void Update(){
        
    }
}
