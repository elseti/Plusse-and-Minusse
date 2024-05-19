using System;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string textPath;
    public bool isRepeatable;
    
    private bool _inCollider;
    private bool _isDialoguePlaying;
    
    private void Update()
    {
        if (_inCollider && Input.GetKeyDown(KeyCode.Return) && !_isDialoguePlaying)
        {
            print("dialogue start");
            PlayScript();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _inCollider = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isDialoguePlaying = false;
            _inCollider = false;
        }
    }
    
    public void PlayScript()
    {
        print("freeze");
        GameManager.instance.FreezePlayer();
        _isDialoguePlaying = true;
        DialogueManager.instance.LoadDialogueList(textPath);
    }

}
