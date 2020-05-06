using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{
    [SerializeField]float movementVelocity;
    Rigidbody2D rb;
    Vector2 movement;
    Vector2 mousePosition;
    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    void Update(){
        GetInput();
        MouseAngleUpdate();
    }

    private void FixedUpdate() {
        Move();
        Aim();
    }

    void GetInput(){
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void MouseAngleUpdate(){
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void Aim(){
        Vector2 lookDir = mousePosition - rb.position;
        float angle = Mathf.Atan2(lookDir.y,lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    void Move(){
        rb.MovePosition(rb.position + movement * movementVelocity * Time.fixedDeltaTime);
    }
}
