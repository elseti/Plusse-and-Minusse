using System.Collections;
using System.Collections.Generic;
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
        public Animator golemAnimator; // for blinking effects
        public Animator plusseAnimator;
        public Animator minusseAnimator;
        
        public GameObject allyUI;
        public GameObject monsterUI;
        public TextMeshProUGUI term1;
        public TextMeshProUGUI term2;
        public GameObject plusseHPBar;
        public GameObject minusseHPBar;
        public TextMeshProUGUI allyHP;
        public GameObject monsterHPBar;
        public TextMeshProUGUI monsterHP;
        
        // bar image
        public GameObject minusseHP;
        public GameObject plusseHP;
        public GameObject golemHP;

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
        
        // VFX
        public ParticleSystem minusseAura;
        public ParticleSystem plusseAura;
        public ParticleSystem minusseMonsterHit;
        public ParticleSystem plusseMonsterHit;
        public ParticleSystem minusseDamaged;
        public ParticleSystem plusseDamaged;
        public ParticleSystem monsterExplosion;
        public ParticleSystem gameOverDebuff;
        public ParticleSystem winBuff;

        public UnityEvent startBattle;
        public UnityEvent monsterTurn;
        public UnityEvent endBattle;

        private Coroutine _currentCoroutine;
        
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
            
            PlusseSetup();
            
            cameraAnimator.Play("Plusse");

            SetChoiceButtons();
        }

        public void MinusseTurn()
        {
            print("in minusse turn");
            battleState = BattleState.MINUSSETURN;
            
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

            if(_currentCoroutine!=null) StopCoroutine(_currentCoroutine);
            _currentCoroutine = StartCoroutine(MonsterWait(2f));
        }

        public void WinBattle()
        {
            print("win");
            
            // Todo - victory
            
            cameraAnimator.Play("Hallway");
            GameManager.instance.UnfreezePlayer();
            EndBattle();
        }

        public void LoseBattle()
        {
            print("lose");
            
            // Todo - lose
            
            cameraAnimator.Play("Hallway");
            GameManager.instance.UnfreezePlayer();
            EndBattle();
        }

        public void EndBattle()
        {
            print("in end button");
            
            battleCanvas.gameObject.SetActive(false);
            
            endBattle.Invoke();
            BattleSetup();
            
            GameManager.instance.movementCanvas.gameObject.SetActive(true);
            GameManager.instance.UnfreezePlayer();
            GameManager.instance.PlayBGM(0);
        }
        
        
        // Helper functions for math operations
        // returns correct index
        public int SetChoiceButtons()
        {
            // create problem and answer
            string answer = "";
            if (battleState == BattleState.PLUSSETURN) // addition
            {
                int operand1 = Random.Range(choiceRange[0], choiceRange[1] + 1); // 0, 3-3+1 ; choice range: 1-3, term 2 range: 1-3
                int operand2 = Random.Range(term2Range[0], term2Range[1] + 1);
                answer = (operand1 + operand2).ToString();
                term1.text = operand1.ToString();
                term2.text = operand2.ToString();
            }
            else if (battleState == BattleState.MINUSSETURN) // subtraction
            {
                int operand2 = Random.Range(term2Range[0], term2Range[1] + 1);
                int operand1 = Random.Range(operand2, choiceRange[1] + 1);
                answer = (operand1 - operand2).ToString();
                term1.text = operand1.ToString();
                term2.text = operand2.ToString();
            }

            // assign correct answer's index randomly
            int correctIndex = Random.Range(0, choiceButtons.Length);

            // create a HashSet to store unique choices
            HashSet<int> uniqueChoices = new HashSet<int>();

            // add the correct answer to the HashSet
            uniqueChoices.Add(int.Parse(answer));

            for (int x = 0; x < choiceButtons.Length; x++)
            {
                // Clear previous listeners
                choiceButtons[x].GetComponentInChildren<Button>().onClick.RemoveAllListeners();

                if (x == correctIndex)
                {
                    choiceButtons[x].GetComponentInChildren<TextMeshProUGUI>().text = answer;
                    choiceButtons[x].GetComponentInChildren<Button>().onClick.AddListener(OnCorrectButton);
                }
                else
                {
                    // Generate a unique random choice
                    int randomChoice;
                    do { randomChoice = Random.Range(choiceRange[0], choiceRange[1] + 1); } while (uniqueChoices.Contains(randomChoice));

                    uniqueChoices.Add(randomChoice);
                    choiceButtons[x].GetComponentInChildren<TextMeshProUGUI>().text = randomChoice.ToString();
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
            
            if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
            _currentCoroutine = StartCoroutine(ActionWait(2f, damage));
        }

        public void OnWrongButton()
        {
            GameManager.instance.PlaySFX(2);
            print("wrong answer");
            cameraAnimator.Play("Monster");
            
            MonsterSetup();

            if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
            _currentCoroutine = StartCoroutine(ActionWait(2f, 0));
        }
        
        // Set up for start / plusse / minusse / monster UI
        public void BattleSetup()
        {
            GameManager.instance.movementCanvas.gameObject.SetActive(false);
            
            plusseBG.SetActive(false);
            minusseBG.SetActive(false);
            plusseHPBar.SetActive(false);
            minusseHPBar.SetActive(false);
            monsterHPBar.SetActive(false);
            monsterHP.gameObject.SetActive(false);
            allyHP.gameObject.SetActive(false);
            plusseHP.SetActive(false);
            minusseHP.SetActive(false);
            golemHP.SetActive(false);
            SetChoicesActive(false);
            monsterDamage.gameObject.SetActive(false);
            allyDamage.gameObject.SetActive(false);
            
            // reset curr health
            plusseStats.currHealth = plusseStats.maxHealth;
            minusseStats.currHealth = minusseStats.maxHealth;
            monsterStats.currHealth = monsterStats.maxHealth;
            
            // stop all VFX
            minusseAura.Stop();
            plusseAura.Stop();
            minusseMonsterHit.Stop();
            plusseMonsterHit.Stop();
            minusseDamaged.Stop();
            plusseDamaged.Stop();
            monsterExplosion.Stop();
            gameOverDebuff.Stop();
            winBuff.Stop();
            
            // play BGM
            GameManager.instance.PlayBGM(1);
            
            // resize HP bar
            plusseHP.transform.localScale = new Vector3(1f, 1f, 1f);
            minusseHP.transform.localScale = new Vector3(1f, 1f, 1f);
            golemHP.transform.localScale = new Vector3(1f, 1f, 1f);
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
            plusseHP.SetActive(true);
            minusseHP.SetActive(false);
            golemHP.SetActive(false);
            
            allyHP.text = plusseStats.currHealth + "/" + plusseStats.maxHealth;
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
            minusseHP.SetActive(true);
            plusseHP.SetActive(false);
            golemHP.SetActive(false);
            
            // TODO - set up Minusse status/hp
            allyHP.text = minusseStats.currHealth + "/" + minusseStats.maxHealth;
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
            golemHP.SetActive(true);
            SetChoicesActive(false);
            
            // TODO - set up monster status/hp
            monsterHP.text = monsterStats.currHealth + "/" + monsterStats.maxHealth;
        }
        
        // Characters getting attacked
        public void MinusseDamaged()
        {
            minusseDamaged.Play();
            minusseAnimator.Play("Hit");
            GameManager.instance.PlaySFX(1);
            
            golemHP.SetActive(false);
            minusseBG.SetActive(false);
            plusseHPBar.SetActive(false);
            minusseHPBar.SetActive(true);
            minusseHP.SetActive(true);
            monsterHPBar.SetActive(false);
            monsterHP.gameObject.SetActive(false);
            allyHP.gameObject.SetActive(true);
            monsterDamage.gameObject.SetActive(false);
            allyDamage.gameObject.SetActive(true);
            SetChoicesActive(false);

            if (minusseStats.currHealth < 0)
            {
                plusseHP.transform.localScale = new Vector3(0f, 1f, 1f);
                allyHP.text = "0/" + plusseStats.maxHealth;
            }
            else
            {
                float fraction = (float) minusseStats.currHealth / (float) minusseStats.maxHealth;
                minusseHP.transform.localScale = new Vector3(fraction, 1f, 1f);
                allyHP.text = minusseStats.currHealth + "/" + minusseStats.maxHealth;
            }
        }
        
        public void PlusseDamaged()
        {
            plusseDamaged.Play();
            plusseAnimator.Play("Hit");
            GameManager.instance.PlaySFX(1);
            
            golemHP.SetActive(false);
            minusseBG.SetActive(false);
            plusseHPBar.SetActive(true);
            plusseHP.SetActive(true);
            minusseHPBar.SetActive(false);
            monsterHPBar.SetActive(false);
            monsterHP.gameObject.SetActive(false);
            allyHP.gameObject.SetActive(true);
            monsterDamage.gameObject.SetActive(false);
            allyDamage.gameObject.SetActive(true);
            SetChoicesActive(false);
            
            
            if (plusseStats.currHealth < 0)
            {
                plusseHP.transform.localScale = new Vector3(0f, 1f, 1f);
                allyHP.text = "0/" + plusseStats.maxHealth;
            }
            else
            {
                // HP bar reduce
                
                float fraction = (float) plusseStats.currHealth / (float) plusseStats.maxHealth;
                plusseHP.transform.localScale = new Vector3(fraction, 1f, 1f);
                allyHP.text = plusseStats.currHealth + "/" + plusseStats.maxHealth;
            }
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
            return damage;
        }
            
        // Helper coroutines
        public IEnumerator ActionWait(float time, int damage)
        {
            print("in action wait");
            golemHP.SetActive(false);
            plusseHP.SetActive(false);
            minusseHP.SetActive(false);
            
            // play aura VFX - player powers up
            if (battleState == BattleState.PLUSSETURN && damage != 0)
            {
                plusseAura.Play();
                GameManager.instance.PlaySFX(0);
                
                monsterHPBar.SetActive(false);
                monsterDamage.gameObject.SetActive(false);
                monsterHP.gameObject.SetActive(false);
                
                yield return new WaitForSeconds(time);
                
                // monster takes damage, update monster HP
                
                plusseAura.Stop();
                plusseMonsterHit.Play();
                GameManager.instance.PlaySFX(5);
            }
            else if(battleState == BattleState.MINUSSETURN && damage != 0)
            {
                minusseAura.Play();
                GameManager.instance.PlaySFX(0);
                
                monsterHPBar.SetActive(false);
                monsterDamage.gameObject.SetActive(false);
                monsterHP.gameObject.SetActive(false);
            
                yield return new WaitForSeconds(time);
                
                // monster takes damage, update monster HP
                
                minusseAura.Stop();
                minusseMonsterHit.Play();
                GameManager.instance.PlaySFX(3);
            }
            
            monsterHPBar.SetActive(true);
            monsterHP.gameObject.SetActive(true);
            monsterDamage.gameObject.SetActive(true);
            golemHP.SetActive(true);
            
            monsterDamage.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();
            monsterStats.currHealth -= damage;

            // monster damaged
            cameraAnimator.Play("Monster");
            if(damage != 0) golemAnimator.Play("Hit");
            
            if (monsterStats.currHealth <= 0)
            {
                // game over, monster die
                golemHP.transform.localScale = new Vector3(0f, 1f, 1f);
                
                monsterExplosion.Play();
                GameManager.instance.PlaySFX(4);
                monsterStats.currHealth = 0;
                monsterHP.text = 0 + "/" + monsterStats.maxHealth;
                battleState = BattleState.WIN;

                yield return new WaitForSeconds(time);
                plusseMonsterHit.Stop();
                minusseMonsterHit.Stop();
                monsterExplosion.Stop();

                winBuff.Play();
                GameManager.instance.PlaySFX(6);
                
                yield return new WaitForSeconds(5f);
                
                WinBattle();
            }
            else
            {
                // continue battle
                float fraction = (float) monsterStats.currHealth / (float) monsterStats.maxHealth;
                golemHP.transform.localScale = new Vector3(fraction, 1f, 1f);
                
                golemHP.SetActive(true);
                monsterHP.text = monsterStats.currHealth + "/" + monsterStats.maxHealth;
                yield return new WaitForSeconds(time);
                plusseMonsterHit.Stop();
                minusseMonsterHit.Stop();
                
                if (battleType == BattleType.ALL && battleState != BattleState.MINUSSETURN) MinusseTurn();
                else MonsterTurn();
            }
            
            yield return null;
        }
        
        public IEnumerator MonsterWait(float time)
        {
            print("in monster wait");
            
            // monster powers up
            yield return new WaitForSeconds(time);
            
            // ally takes damage
            int damage = 0;
            if (battleType == BattleType.PLUSSE)
            {
                plusseHP.SetActive(true);
                damage = CalculateBasicAttack(monsterStats, plusseStats);
                plusseStats.currHealth -= damage;
                PlusseDamaged();
                cameraAnimator.Play("Plusse");
            }
            else if (battleType == BattleType.MINUSSE)
            {
                minusseHP.SetActive(true);
                damage = CalculateBasicAttack(monsterStats, minusseStats);
                minusseStats.currHealth -= damage;
                MinusseDamaged();
                cameraAnimator.Play("Minusse");
            }
            else
            {
                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    damage = CalculateBasicAttack(monsterStats, plusseStats);
                    plusseStats.currHealth -= damage;
                    PlusseDamaged();
                    cameraAnimator.Play("Plusse");
                }
                else if (random == 1)
                {
                    damage = CalculateBasicAttack(monsterStats, minusseStats);
                    minusseStats.currHealth -= damage;
                    MinusseDamaged();
                    cameraAnimator.Play("Minusse");
                }
            }
            
            allyDamage.gameObject.SetActive(true);
            allyDamage.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();

            yield return new WaitForSeconds(time);
            
            plusseDamaged.Stop();
            minusseDamaged.Stop();
            
            if (plusseStats.currHealth <= 0 || minusseStats.currHealth <= 0)
            {
                // game over, ally dies
                allyHP.text = 0 + "/" + monsterStats.maxHealth;
                battleState = BattleState.LOSE;

                gameOverDebuff.Play();
                GameManager.instance.PlaySFX(8);
                
                yield return new WaitForSeconds(time);
                
                LoseBattle();
            }
            else
            {
                if(battleType == BattleType.PLUSSE) PlusseTurn();
                else if(battleType == BattleType.MINUSSE) MinusseTurn();
                else AllTurn();
            }
            
            yield return null;
        }
    }
}