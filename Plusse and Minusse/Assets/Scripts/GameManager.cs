using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public UnityEvent freezePlayer;
    public UnityEvent unfreezePlayer;
    public UnityEvent<BattleManager.BattleType> startBattle;

    public AudioClip[] bgmList;
    public AudioSource bgmSource;

    public AudioClip[] sfxList;
    public AudioSource sfxSource;
    
    // Player movement functions
    public void FreezePlayer(){
        freezePlayer.Invoke();
    }

    public void UnfreezePlayer(){
        unfreezePlayer.Invoke();
    }
    
    // Battle functions
    public void StartBattle(BattleManager.BattleType battleType)
    {
        FreezePlayer();
        startBattle.Invoke(battleType);
    }

    public void PlayBGM(int index)
    {
        if (index < bgmList.Length)
        {
            bgmSource.clip = bgmList[index];
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(int index)
    {
        if (index < sfxList.Length)
        {
            sfxSource.clip = sfxList[index];
            sfxSource.Play();
        }
    }
    
    public void StopSFX()
    {
        sfxSource.Stop();
    }
}
