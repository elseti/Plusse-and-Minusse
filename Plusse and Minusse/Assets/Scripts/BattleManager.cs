using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace
{
    public class BattleManager : MonoBehaviour
    {
        public enum BattleState
        {
            START,
            ALLYTURN,
            MONSTERTURN,
            WIN,
            LOSE
        };

        public enum BattleType
        {
            PLUSSE,
            MINUSSE,
            ALL
        }

        public BattleState battleState;
        public BattleType battleType;

        public Animator cameraAnimator;
        
        // triggered by doors
        public void StartBattle(BattleType battleType)
        {
            battleState = BattleState.START;
            this.battleType = battleType;
            // StartCoroutine(PlayerTurn());
            
            if(battleType == BattleType.PLUSSE) PlusseTurn();
            else if(battleType == BattleType.MINUSSE) MinusseTurn();
            else PlusseTurn();
        }

        public void PlusseTurn()
        {
            battleState = BattleState.ALLYTURN;
            battleType = BattleType.PLUSSE;
            cameraAnimator.Play("Plusse");
        }

        public void MinusseTurn()
        {
            battleState = BattleState.ALLYTURN;
            battleType = BattleType.MINUSSE;
            cameraAnimator.Play("Minusse");
        }

        public void AllTurn()
        {
            battleState = BattleState.ALLYTURN;
            battleType = BattleType.ALL;
            PlusseTurn();
        }

        public void MonsterTurn()
        {
            battleState = BattleState.MONSTERTURN;
            cameraAnimator.Play("Monster");
        }

        public void EndBattle()
        {
            
        }
        
        // Button actions
        public void OnAttack()
        {
            // action
            cameraAnimator.Play("Monster");
            
            // show damage
            
            
            if(battleType == BattleType.ALL) MinusseTurn();
        }
        
        
        
    }
}