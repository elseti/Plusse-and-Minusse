using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class BattleManager : MonoBehaviour
    {
        public enum BattleState { START, PLUSSETURN, MINUSSETURN, MONSTERTURN, WIN, LOSE };

        public enum BattleType { PLUSSE, MINUSSE, ALL };

        public BattleState battleState;
        public BattleType battleType;

        public CharacterStats plusseStats;
        public CharacterStats minusseStats;
        public CharacterStats monsterStats;
        
        public int attackMultiplier = 1;
        public int ultimateMultiplier = 3;
        public int lowRandomRange = -5;
        public int highRandomRange = 5;

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

        public GameObject allyDamage;
        public GameObject monsterDamage;
        public GameObject plusseSprite;
        public GameObject minusseSprite;
        public GameObject monsterSprite;
        
        public GameObject plusseBG;
        public GameObject minusseBG;
        public GameObject[] choiceButtons;
        public CanvasGroup battleCanvas;

        public int[] choiceRange = new int[] { 0, 10 }; // inclusive
        public int[] term2Range = new int[] { 1, 3 };  // inclusive

        public UnityEvent startBattle;
        public UnityEvent monsterTurn;
        public UnityEvent endBattle;

        private bool _isCoroutineRunning;
        
        // triggered by doors
        public void StartBattle(BattleType battleType)
        {
            battleState = BattleState.START;
            this.battleType = battleType;

            BattleSetup();
            startBattle.Invoke();
            
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

            SetChoiceButtons();
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
            monsterTurn.Invoke();
            
            cameraAnimator.Play("Monster");

            if(!_isCoroutineRunning) StartCoroutine(MonsterWait(2f));
        }

        public void WinBattle()
        {
            print("win");
            cameraAnimator.Play("Hallway");
            GameManager.instance.UnfreezePlayer();
        }

        public void LoseBattle()
        {
            print("lose");
            cameraAnimator.Play("Hallway");
            GameManager.instance.UnfreezePlayer();
        }

        public void EndBattle()
        {
            endBattle.Invoke();
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
            int damage;
            if (battleState == BattleState.PLUSSETURN)
            {
                damage = CalculateBasicAttack(plusseStats, monsterStats);
            }
            else
            {
                damage = CalculateBasicAttack(minusseStats, monsterStats);
            }
            
            MonsterSetup();
            
            if(!_isCoroutineRunning) StartCoroutine(ActionWait(2f, damage));
        }

        public void OnWrongButton()
        {
            print("wrong answer");
            cameraAnimator.Play("Monster");
            
            MonsterSetup();

            if(!_isCoroutineRunning) StartCoroutine(ActionWait(2f, 0));
        }
        
        // Set up for start / plusse / minusse / monster UI
        public void BattleSetup()
        {
            plusseBG.SetActive(false);
            minusseBG.SetActive(false);
            plusseHPBar.SetActive(false);
            minusseHPBar.SetActive(false);
            monsterHPBar.SetActive(false);
            monsterHP.gameObject.SetActive(false);
            allyHP.gameObject.SetActive(false);
            SetChoicesActive(false);
            monsterDamage.gameObject.SetActive(false);
            allyDamage.gameObject.SetActive(false);
            
            // reset curr health
            plusseStats.currHealth = plusseStats.maxHealth;
            minusseStats.currHealth = minusseStats.maxHealth;
            monsterStats.currHealth = monsterStats.maxHealth;
        }
        
        public void PlusseSetup()
        {
            plusseBG.SetActive(true);
            minusseBG.SetActive(false);
            plusseHPBar.SetActive(true);
            minusseHPBar.SetActive(false);
            monsterHPBar.SetActive(false);
            monsterHP.gameObject.SetActive(false);
            allyHP.gameObject.SetActive(true);
            monsterDamage.gameObject.SetActive(false);
            allyDamage.gameObject.SetActive(false);
            SetChoicesActive(true);
            
            // TODO - set up Plusse status/hp
            allyHP.text = plusseStats.currHealth + "/ " + plusseStats.maxHealth;
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
            monsterDamage.gameObject.SetActive(false);
            allyDamage.gameObject.SetActive(false);
            SetChoicesActive(true);
            
            // TODO - set up Minusse status/hp
            allyHP.text = minusseStats.currHealth + "/ " + minusseStats.maxHealth;
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
            monsterHP.text = monsterStats.currHealth + "/ " + monsterStats.maxHealth;
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
        
        
        // Damage calculation
        private int CalculateBasicAttack(CharacterStats attacker, CharacterStats receiver)
        {
            int damage = (attacker.attack * attackMultiplier + Random.Range(lowRandomRange, highRandomRange)) - (receiver.defence + Random.Range(lowRandomRange, highRandomRange));
            if(damage < 0) damage = 0;
            print("damage " + damage);
            return damage;
        }
        
        private int CalculateUltimateAttack(CharacterStats attacker, CharacterStats receiver)
        {
            int damage = (attacker.attack * attackMultiplier * ultimateMultiplier + Random.Range(lowRandomRange, highRandomRange)) - (receiver.defence + Random.Range(lowRandomRange, highRandomRange));
            if(damage < 0) damage = 0;
            print("damage " + damage);
            return damage;
        }
            
            
        // Helper coroutines
        public IEnumerator ActionWait(float time, int damage)
        {
            print("in action wait");
            
            _isCoroutineRunning = true;
            yield return new WaitForSeconds(time);
            
            // monster takes damage, update monster HP.
            monsterDamage.gameObject.SetActive(true);
            monsterDamage.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();
            monsterStats.currHealth -= damage;
            print(monsterStats.currHealth);
            
            _isCoroutineRunning = false;
            
            if (monsterStats.currHealth <= 0)
            {
                // game over, monster die
                monsterStats.currHealth = 0;
                monsterHP.text = 0 + "/ " + monsterStats.maxHealth;
                battleState = BattleState.WIN;
                WinBattle();
            }
            else
            {
                // continue battle
                monsterHP.text = monsterStats.currHealth + "/ " + monsterStats.maxHealth;
                if (battleType == BattleType.ALL && battleState != BattleState.MINUSSETURN) MinusseTurn();
                else MonsterTurn();
            }
            
            yield return null;
        }
        
        public IEnumerator MonsterWait(float time)
        {
            print("in monster wait");
            
            _isCoroutineRunning = true;
            yield return new WaitForSeconds(time);
            
            // ally takes damage
            allyDamage.gameObject.SetActive(true);
            
            _isCoroutineRunning = false;
            
            if(battleType == BattleType.PLUSSE) PlusseTurn();
            else if(battleType == BattleType.MINUSSE) MinusseTurn();
            else AllTurn();

            yield return null;
        }
    }
}