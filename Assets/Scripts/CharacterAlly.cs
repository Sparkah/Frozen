using System.Collections;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAlly : MonoBehaviour, ICharacter
{
    public Button Attack;
    public Button Skip;

    private SkeletonAnimation skeletonAnimation;
    private Vector3 fightingSpot = new Vector3(-1.5f, -4, 0);
    private Vector3 step;
    private Vector3 startPos;
    private float stepX;

    private int steps = 20;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        startPos = transform.position;
        stepX = 0.05f;
        step = new Vector3(stepX, 0, 0);
    }

    /// <summary>
    /// Animations
    /// </summary>
    public void ReceiveDamage(int damage)
    {
        StartCoroutine(MoveTowardsFightingSpot(steps));
        StartCoroutine(ReceiveDamageAnimation());
    }

    public void MakeDamage(int damage)
    {
        StartCoroutine(MoveTowardsFightingSpot(steps));
        StartCoroutine(MakeDamageAnimation());
    }

    IEnumerator ReceiveDamageAnimation()
    {
        stepX = (fightingSpot.x- transform.position.x) / steps;
        step = new Vector3(stepX, 0, 0);
        yield return new WaitForSeconds(1.5f);
        skeletonAnimation.state.SetAnimation(1, Animations.Damage.ToString(), false);
        StartCoroutine(IdleAnimation());
        yield return new WaitForSeconds(1f);
        steps = 20;
        StartCoroutine(MoveTowardsStartingPos(steps));
    }

    IEnumerator MakeDamageAnimation()
    {
        stepX = (fightingSpot.x - transform.position.x) / steps;
        step = new Vector3(stepX, 0, 0);
        yield return new WaitForSeconds(1f);
        skeletonAnimation.state.SetAnimation(1, Animations.Miner_1.ToString(), false);
        StartCoroutine(IdleAnimation());
        yield return new WaitForSeconds(1f);
        steps = 20;
        StartCoroutine(MoveTowardsStartingPos(steps));
    }

    IEnumerator IdleAnimation()
    {
        yield return new WaitForSeconds(1f);
        skeletonAnimation.state.SetAnimation(1, Animations.Idle.ToString(), true);
    }

    IEnumerator MoveTowardsFightingSpot(int _steps)
    {
        transform.position += step;
        transform.localScale *= 1.01f;
        yield return new WaitForSeconds(0.02f);
        if (transform.position.x < fightingSpot.x && _steps > 0)
        {
            _steps -= 1;
            StartCoroutine(MoveTowardsFightingSpot(_steps));
        }
    }

    IEnumerator MoveTowardsStartingPos(int _steps)
    {
        transform.position -= step;
        transform.localScale /= 1.01f;
        yield return new WaitForSeconds(0.02f);
        if (transform.position.x > startPos.x && _steps > 0)
        {
            _steps -= 1;
            StartCoroutine(MoveTowardsStartingPos(_steps));
        }
    }

    /// <summary>
    /// Buttons //немного хардкода
    /// </summary>
    public void ShowUIButtons()
    {
        Attack.gameObject.SetActive(true);
        Skip.gameObject.SetActive(true);
        Attack.onClick.AddListener(ButtonAttackPressed);
        Skip.onClick.AddListener(ButtonSkipPressed);
        GetComponentInChildren<ParticleSystem>().Play();
    }

    public void ButtonAttackPressed()
    {
        Skip.onClick.RemoveListener(ButtonSkipPressed);
        Attack.onClick.RemoveListener(ButtonAttackPressed);
        Attack.gameObject.SetActive(false);
        Skip.gameObject.SetActive(false);
        GetComponentInChildren<ParticleSystem>().Stop();
    }

    public void ButtonSkipPressed()
    {
        Skip.onClick.RemoveListener(ButtonSkipPressed);
        Attack.onClick.RemoveListener(ButtonAttackPressed);
        Attack.gameObject.SetActive(false);
        Skip.gameObject.SetActive(false);
        GetComponentInChildren<ParticleSystem>().Stop();
    }
}