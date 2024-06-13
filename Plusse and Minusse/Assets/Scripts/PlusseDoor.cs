using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlusseDoor : MonoBehaviour
    {
        private bool _inCollider;
        private bool _freezeTrigger;

        public void FreezeTrigger()
        {
            _freezeTrigger = true;
        }
        
        public void UnfreezeTrigger()
        {
            _freezeTrigger = false;
        }

        private void Update()
        {
            if (_inCollider && Input.GetKeyDown(KeyCode.Return) && !_freezeTrigger)
            {
                GameManager.instance.StartBattle(BattleManager.BattleType.PLUSSE);
            }
            
            // for testing
            // if (Input.GetKeyDown(KeyCode.P))
            // {
            //     GameManager.instance.StartBattle(BattleManager.BattleType.PLUSSE);
            // }
            // if (Input.GetKeyDown(KeyCode.M))
            // {
            //     GameManager.instance.StartBattle(BattleManager.BattleType.MINUSSE);
            // }
            // if (Input.GetKeyDown(KeyCode.L))
            // {
            //     GameManager.instance.StartBattle(BattleManager.BattleType.ALL);
            // }
            
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
                _inCollider = false;
            }
        }
    }
}