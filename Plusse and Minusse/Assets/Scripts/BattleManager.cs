using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class BattleManager : MonoBehaviour
    {
        public enum BattleState
        {
            START,
            PLUSSETURN,
            MINUSSETURN,
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
        public GameObject allyUI;
        public GameObject monsterUI;
        public TextMeshProUGUI term1;
        public TextMeshProUGUI term2;
        public GameObject plusseHPBar;
        public GameObject minusseHPBar;
        public TextMeshProUGUI allyHP;
        public GameObject monsterHPBar;
        public TextMeshProUGUI monsterHP;
        
        public GameObject plusseBG;
        public GameObject minusseBG;
        public GameObject[] choiceButtons;
        public CanvasGroup battleCanvas;

        public int[] choiceRange = new int[] { 0, 10 }; // inclusive
        public int[] term2Range = new int[] { 1, 3 };  // inclusive

        private bool _isCoroutineRunning;
        
        // triggered by doors
        public void StartBattle(BattleType battleType)
        {
            battleState = BattleState.START;
            this.battleType = battleType;
            
            if(battleType == BattleType.PLUSSE) PlusseTurn();
            else if(battleType == BattleType.MINUSSE) MinusseTurn();
            else AllTurn();
        }

        public void PlusseTurn()
        {
            print("in plusse turn");
            battleState = BattleState.PLUSSETURN;
            battleType = BattleType.PLUSSE;
            
            PlusseSetup();
            
            cameraAnimator.Play("Plusse");

            SetChoiceButtons();
            
        }

        public void MinusseTurn()
        {
            print("in minusse turn");
            battleState = BattleState.MINUSSETURN;
            battleType = BattleType.MINUSSE;
            
            MinusseSetup();
            
            cameraAnimator.Play("Minusse");
        }
        
        public void AllTurn()
        {
            print("in all turn");
            battleType = BattleType.ALL;
            PlusseTurn();
        }


        public void MonsterTurn()
        {
            print("in monster turn");
            battleState = BattleState.MONSTERTURN;
            
            MonsterSetup();
            
            cameraAnimator.Play("Monster");

            if(!_isCoroutineRunning) StartCoroutine(MonsterWait(2f));
        }

        public void EndBattle()
        {
            print("in end button");
        }
        
        
        // Helper functions for math operations
        // returns correct index
        public int SetChoiceButtons()
        {
            // create problem and answer
            int operand1 = Random.Range(choiceRange[0], choiceRange[1] - term2Range[1] + 1);
            int operand2 = Random.Range(term2Range[0], term2Range[1] + 1);
            string answer = (operand1 + operand2).ToString();
            term1.text = operand1.ToString();
            term2.text = operand2.ToString();
            
            // assign choices to buttons
            // assign correct answer's index randomly
            int correctIndex = Random.Range(0, 3);
            for (int x = 0; x < choiceButtons.Length; x++)
            {
                if (x == correctIndex)
                {
                    choiceButtons[x].GetComponentInChildren<TextMeshProUGUI>().text = answer;
                    choiceButtons[x].GetComponentInChildren<Button>().onClick.AddListener(OnCorrectButton);
                }
                else
                {
                    // create random choices
                    string random = Random.Range(choiceRange[0], choiceRange[1] + 1).ToString();
                    // while (random1 == random2) random2 = Random.Range(choiceRange[0], choiceRange[1]+1);
                    choiceButtons[x].GetComponentInChildren<TextMeshProUGUI>().text = random;
                    choiceButtons[x].GetComponentInChildren<Button>().onClick.AddListener(OnWrongButton);
                }
            }

            battleCanvas.gameObject.SetActive(true);
            
            return correctIndex;
        }
        
        // Correct / incorrect buttons pressed
        public void OnCorrectButton()
        {
            print("right answer");
            
            // monster take damage
            
            MonsterSetup();
            
            if(!_isCoroutineRunning) StartCoroutine(ActionWait(2f));
        }

        public void OnWrongButton()
        {
            print("wrong answer");
            cameraAnimator.Play("Monster");
            
            MonsterSetup();

            if(!_isCoroutineRunning) StartCoroutine(ActionWait(2f));
        }
        
        // Set up for plusse / minusse / monster UI
        public void PlusseSetup()
        {
            plusseBG.SetActive(true);
            minusseBG.SetActive(false);
            plusseHPBar.SetActive(true);
            minusseHPBar.SetActive(false);
            monsterHPBar.SetActive(false);
            monsterHP.gameObject.SetActive(false);
            allyHP.gameObject.SetActive(true);
            SetChoicesActive(true);
            
            // TODO - set up Plusse status/hp
        }
        
        public void MinusseSetup()
        {
            plusseBG.SetActive(false);
            minusseBG.SetActive(true);
            plusseHPBar.SetActive(false);
            minusseHPBar.SetActive(true);
            monsterHPBar.SetActive(false);
            monsterHP.gameObject.SetActive(false);
            allyHP.gameObject.SetActive(true);
            SetChoicesActive(true);
            
            // TODO - set up Minusse status/hp
        }

        public void MonsterSetup()
        {
            print("in monster setup");
            plusseBG.SetActive(false);
            minusseBG.SetActive(false);
            plusseHPBar.SetActive(false);
            minusseHPBar.SetActive(false);
            monsterHPBar.SetActive(true);
            monsterHP.gameObject.SetActive(true);
            allyHP.gameObject.SetActive(false);
            SetChoicesActive(false);
            
            // TODO - set up monster status/hp
        }

        public void SetChoicesActive(bool active)
        {
            term1.gameObject.SetActive(active);
            term2.gameObject.SetActive(active);
            for (int x = 0; x < choiceButtons.Length; x++)
            {
                choiceButtons[x].SetActive(active);
            }
        }
            
            
        // Helper coroutines
        public IEnumerator ActionWait(float time)
        {
            _isCoroutineRunning = true;
            yield return new WaitForSeconds(time);
            
            _isCoroutineRunning = false;
            
            if (battleType == BattleType.ALL && battleState != BattleState.MINUSSETURN) MinusseTurn();
            else MonsterTurn();
            
            yield return null;
        }
        
        public IEnumerator MonsterWait(float time)
        {
            _isCoroutineRunning = true;
            yield return new WaitForSeconds(time);
            
            _isCoroutineRunning = false;
            
            if(battleType == BattleType.PLUSSE) PlusseTurn();
            else if(battleType == BattleType.MINUSSE) MinusseTurn();
            else AllTurn();

            yield return null;
        }
    }
}