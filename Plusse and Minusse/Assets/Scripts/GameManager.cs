using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public UnityEvent gameStart;
    public UnityEvent freezePlayer;
    public UnityEvent unfreezePlayer;

    // Player movement functions
    public void FreezePlayer(){
        freezePlayer.Invoke();
    }

    public void UnfreezePlayer(){
        unfreezePlayer.Invoke();
    }

}
