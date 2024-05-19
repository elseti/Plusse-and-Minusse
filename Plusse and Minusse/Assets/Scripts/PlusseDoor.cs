using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlusseDoor : MonoBehaviour
    {
        private bool _inCollider;

        private void Update()
        {
            if (_inCollider && Input.GetKeyDown(KeyCode.Return))
            {
                GameManager.instance.StartBattle(BattleManager.BattleType.PLUSSE);
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
                _inCollider = false;
            }
        }
    }
}