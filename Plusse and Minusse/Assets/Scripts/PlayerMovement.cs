using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 10f;

    public bool freezePlayer = false;

    private float _horizontal;
    private Animator _playerAnimator;

    private void Start()
    {
        _playerAnimator = rb.gameObject.GetComponent<Animator>();
    }
    
    private void Update()
    {
        // Gives a value between -1 and 1
        _horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left, 1 is right
    }

    void FixedUpdate()
    {
        if(!freezePlayer){

            if(_horizontal == 0){
                _playerAnimator.Play("Player Idle");
            }
            if(_horizontal == -1){
                _playerAnimator.Play("Player Walk");
                GetComponent<SpriteRenderer>().flipX = true;
            }
            if(_horizontal == 1){
                _playerAnimator.Play("Player Walk");
                GetComponent<SpriteRenderer>().flipX = false;
            }
            
            rb.velocity = new Vector2(_horizontal * speed, 0);
        }
        else{
            _playerAnimator.Play("Player Idle");
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        
    }

}
