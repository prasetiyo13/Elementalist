using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAndShieldController : MonoBehaviour {

    //serializefield variable
    [SerializeField] float moveDampingTime = 0.5f;


    Animator anim;
    


    void Awake () {
        anim = GetComponent<Animator>();
    }

    void Update () {
        Move();
    }

    void Move () {
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("Forward", v, moveDampingTime, Time.deltaTime);
        anim.SetFloat("Strafe", h, moveDampingTime, Time.deltaTime);
    }

}
