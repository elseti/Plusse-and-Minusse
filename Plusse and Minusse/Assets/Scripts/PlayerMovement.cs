using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 10f;

    public AudioClip[] footstepsList;

    private float _horizontal;
    private Animator _playerAnimator;
    private AudioSource _playerAudio;
    private float _timeElapsed;
    private bool _freezePlayer;
    private bool _isMoving;

    private void Start()
    {
        _playerAnimator = rb.gameObject.GetComponent<Animator>();
        _playerAudio = GetComponent<AudioSource>();
    }
    
    private void Update()
    {
        // Gives a value between -1 and 1
        _horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left, 1 is right
        _timeElapsed += Time.deltaTime;
        if (_timeElapsed > 0.65 && _isMoving &&!_freezePlayer)
        {
            _playerAudio.PlayOneShot(footstepsList[Random.Range(0, footstepsList.Length)]);
            _timeElapsed = 0;
        }
    }

    void FixedUpdate()
    {
        if(!_freezePlayer){

            if(_horizontal == 0)
            {
                _isMoving = false;
                _playerAnimator.Play("Player Idle");
            }
            if(_horizontal == -1)
            {
                _isMoving = true;
                _playerAnimator.Play("Player Walk");
                GetComponent<SpriteRenderer>().flipX = true;
            }
            if(_horizontal == 1){
                _isMoving = true;
                _playerAnimator.Play("Player Walk");
                GetComponent<SpriteRenderer>().flipX = false;
            }
            
            rb.velocity = new Vector2(_horizontal * speed, 0);
        }
        else{
            _playerAnimator.Play("Player Idle");
        }
        
    }

    public void FreezePlayer()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        _freezePlayer = true;
    }

    public void UnfreezePlayer()
    {
        rb.constraints = RigidbodyConstraints.None;
        _freezePlayer = false;
    }

}
