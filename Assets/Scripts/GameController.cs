using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Button attack;
    [SerializeField] private Button skip;
    [SerializeField] private Button restartGame;
    [SerializeField] private GameObject miner;
    [SerializeField] private List<GameObject> team1;
    [SerializeField] private List<GameObject> team2;

    private readonly int teamSize = 4;
    private int team1CharactersDisplace = -3;
    private int team2CharactersDisplace = 3;
    private bool canAttackEnemy;
    private int damage;
    private int passageAmount;

    private void Start()
    {
        BuildTeams(); //создать команды
        SetUpUI(); //подготовить кнопки UI

        passageAmount = teamSize * 2; //максиммальное кол-во ходов на обе команды
        StartRandomPassage(); //начать случайный ход
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RegisterClick(); //фикс кликов по дружественным и вражеским существам
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }
    }

    private void BuildTeams()
    {
        int _team1Displace = team1CharactersDisplace;
        int _team2Displace = team2CharactersDisplace;

        for (int i = 0; i < teamSize; i++)
        {
            GameObject _character = Instantiate(miner, spawnPoint.transform.position + new Vector3(team1CharactersDisplace, 0, 0), Quaternion.identity);
            team1.Add(_character);
            team1CharactersDisplace += _team1Displace;
            _character.AddComponent<CharacterAlly>();
            _character.GetComponent<CharacterAlly>().Attack = attack;
            _character.GetComponent<CharacterAlly>().Skip = skip;
        }
        for (int i = 0; i < teamSize; i++)
        {
            GameObject _character = Instantiate(miner, spawnPoint.transform.position + new Vector3(team2CharactersDisplace, 0, 0), Quaternion.Euler(0, 180, 0));
            team2.Add(_character);
            team2CharactersDisplace += _team2Displace;
            _character.AddComponent<CharacterEnemy>();
        }
    }

    private void SetUpUI()
    {
        skip.onClick.AddListener(ContinueRandomPassage);
        attack.onClick.AddListener(AllyCanAttack);
        attack.gameObject.SetActive(false);
        skip.gameObject.SetActive(false);
        restartGame.onClick.AddListener(RestartGame);
    }

    private void ContinueRandomPassage()
    {
        StartCoroutine(PrepareForNextPassage());
    }

    private void AllyCanAttack()
    {
        canAttackEnemy = true;
    }

    private int team1TimesChosen = 0;
    private int team2TimesChosen = 0;

    private void StartRandomPassage()
    {
        int _chooseRandomTeam = Random.Range(0, 2);

        if(_chooseRandomTeam == 0 && team1.ToArray().Length>0 && team1TimesChosen<teamSize)
        {
            int _attacker = Random.Range(0, team1.ToArray().Length);
            team1[_attacker].GetComponent<CharacterAlly>().ShowUIButtons();
            SetCurrentAttacker(team1[_attacker]);
            team1.Remove(team1[_attacker]);
        }
        else if(team2.ToArray().Length > 0 && team2TimesChosen < teamSize)
        {
            int _attacker = Random.Range(0, team2.ToArray().Length);
            SetCurrentAttacker(team2[_attacker]);
            EnemyAttack(_attacker);
        }
        else if(team1.ToArray().Length > 0 || team2.ToArray().Length > 0)
        {
            StartRandomPassage();
        }

        passageAmount -= 1;
    }

    private void EnemyAttack(int _attacker) //этот метод разбить на 2
    {
        if (team1.ToArray().Length > 0)
        {
            int allyToBeAttacked = Random.Range(0, team1.ToArray().Length);
            team1[allyToBeAttacked].GetComponent<CharacterAlly>().ReceiveDamage(damage);
            team2[_attacker].GetComponent<CharacterEnemy>().MakeDamage(damage);
            team2.Remove(team2[_attacker]);               
            StartCoroutine(PrepareForNextPassage());
        }
        else
        {
            NextBattle();
        }
    }

    private void RegisterClick()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null && hit.collider.GetComponent<CharacterAlly>() != null)
        {
            Debug.Log("Ally was clicked!");
            return;
        }
        else if (hit.collider != null && hit.collider.GetComponent<CharacterEnemy>() != null)
        {
            if (canAttackEnemy)
            {
                hit.collider.GetComponent<CharacterEnemy>().ReceiveDamage(damage);
                attacker.GetComponent<CharacterAlly>().MakeDamage(damage);
                canAttackEnemy = false;
                StartCoroutine(PrepareForNextPassage());
            }
        }
    }

    private GameObject attacker;
    private void SetCurrentAttacker(GameObject _attacker)
    {
        attacker = _attacker;
    }

    IEnumerator PrepareForNextPassage()
    {
        yield return new WaitForSeconds(4f);

        if (passageAmount > 0)
        {
            StartRandomPassage();
        }
        else
        {
            NextBattle();
        }
    }

    private void NextBattle()
    {
        restartGame.gameObject.SetActive(true);
    }
    private void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}