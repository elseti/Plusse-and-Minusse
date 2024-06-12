using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
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
    
    // settings
    public BattleManager battleManager;
    public Canvas movementCanvas;
    public Canvas settingCanvas;
    public TextMeshProUGUI lowChoice;
    public TextMeshProUGUI highChoice;
    public TextMeshProUGUI lowTerm;
    public TextMeshProUGUI highTerm;
    public TextMeshProUGUI errorText;

    
    
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
    
    // Settings functions
    public void OnSettingsClicked()
    {
        movementCanvas.gameObject.SetActive(false);
        settingCanvas.gameObject.SetActive(true);
        FreezePlayer();
    }
    
    public void OnSettingsSaved()
    {
        try
        {
            int lowChoiceSet = int.Parse(lowChoice.text.Substring(0, lowChoice.text.Length - 1));
            int highChoiceSet = int.Parse(highChoice.text.Substring(0, highChoice.text.Length - 1));
            int lowTermSet = int.Parse(lowTerm.text.Substring(0, lowTerm.text.Length - 1));
            int highTermSet = int.Parse(highTerm.text.Substring(0, highTerm.text.Length - 1));

            bool inRange = (lowChoiceSet / 1000) < 1 && (highChoiceSet / 1000) < 1 && (lowTermSet / 1000) < 1 &&
                           (highTermSet / 1000) < 1;

            if (lowChoiceSet > highChoiceSet || lowTermSet > highTermSet || !inRange) 
            {
                errorText.text = "Invalid range.";
            }
            else
            {
                battleManager.choiceRange[0] = lowChoiceSet;
                battleManager.choiceRange[1] = highChoiceSet;
                battleManager.term2Range[0] = lowTermSet;
                battleManager.term2Range[1] = highTermSet;
                
                errorText.text = "Saved!";
            }
        }
        catch (Exception e)
        {
            print(e);
            errorText.text = "Invalid input.";
        }
    }

    public void OnSettingsQuit()
    {
        movementCanvas.gameObject.SetActive(true);
        settingCanvas.gameObject.SetActive(false);
        UnfreezePlayer();
        errorText.text = "";
    }

    
}
