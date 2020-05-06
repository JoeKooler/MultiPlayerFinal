using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour{

    public Transform gunPoint;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
    void Start(){
        
    }

    void Update(){
        GetInput();
    }

    void GetInput(){
        if(Input.GetMouseButtonDown(0)){
            Shoot();
        }
    }

    void Shoot(){
        GameObject bullet = Instantiate(bulletPrefab,gunPoint.position,gunPoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(gunPoint.up * bulletForce, ForceMode2D.Impulse);
    }
}
