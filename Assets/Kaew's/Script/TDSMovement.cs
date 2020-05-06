using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDSMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Camera cam;
    Vector2 movement;
    Vector2 mousePos;
    public int HP = 5;
    public Animator animator;
    public Transform firePoint;
    public GameObject bulletPrefab;

    public bool canMove = false;
    // Update is called once per frame
    private void Start() {
        cam = FindObjectOfType<Camera>();
    }
    void Update(){
        if(!canMove)
            return;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        animator.SetFloat("Speed",movement.sqrMagnitude);

        if(Input.GetButtonDown("Fire1")){
            Shoot();
        }
    }

    void FixedUpdate() {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;   
    }

    void Shoot() {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.name = GamePlayManager.uniqueBullet.ToString() + FindObjectOfType<SessionManager>().myID + "Bullet";
        GamePlayManager.uniqueBullet++;
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * 25f, ForceMode2D.Impulse);
    }
}
