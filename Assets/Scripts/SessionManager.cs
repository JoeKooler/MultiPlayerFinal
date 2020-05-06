using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour{
    public string myID;
    public string roomName;
    private void Awake() {
        DontDestroyOnLoad(this);
    }
}
