using Platformer.Player.Inputs;
using Platformer.Player;
using System.Collections;
using UnityEngine;
using Cinemachine;

public class FinishCutsceneManager : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [Header("Движение к двери")]
    [SerializeField] private Transform doorCenter;
    [SerializeField] private float moveSpeed = 0.5f;

    [Header("Чёрные полосы")]
    [SerializeField] private RectTransform topBar;
    [SerializeField] private RectTransform bottomBar;
    [SerializeField] private float barAnimationTime = 3.5f;

    [Header("Камера")]
    [SerializeField] private CinemachineVirtualCamera vCamDoorCloseUp;
    [SerializeField] private float zoomDuration = 4.5f;
    [SerializeField] private float targetOrthographicSize = 0.65f;

    private bool cutsceneRunning = false;
    private Animator playerAnimator;
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (player != null)
        {
            playerAnimator = player.GetComponent<Animator>();
            playerInput = player.GetComponent<PlayerInput>();
            playerMovement = player.GetComponent<PlayerMovement>();
        }
        else
        {
            Debug.LogError("Player не назначен в FinishCutsceneManager!");
        }
    }

    public void StartDoorCutscene(GameObject player)
    {
        if (!cutsceneRunning)
            StartCoroutine(DoorCutsceneRoutine(player));
    }

    private IEnumerator DoorCutsceneRoutine(GameObject player)
    {
        cutsceneRunning = true;
       
        float originalAnimSpeed = playerAnimator != null ? playerAnimator.speed : 1f;

        if (playerInput != null) playerInput.enabled = false;
        if (playerMovement != null) playerMovement.enabled = false;

        if (playerAnimator != null)
        {
            playerAnimator.speed = 0.5f;
            playerAnimator.SetBool("IsJump", false);
            playerAnimator.SetBool("IsAir", false);
            playerAnimator.SetBool("IsAttack", false);
            playerAnimator.SetBool("IsRun", true);
        }

        bool moveDone = false, zoomDone = false, barsDone = false;

        StartCoroutine(MovePlayerRoutine(player, () => { moveDone = true; }));
        StartCoroutine(ZoomCameraRoutine(() => { zoomDone = true; }));
        StartCoroutine(ShowBlackBarsRoutine(() => { barsDone = true; }));

        yield return new WaitUntil(() => moveDone && zoomDone && barsDone);
        yield return new WaitForSeconds(1f);
        
        UIManager.Instance.ShowWindow("VictoryScreen");

        for (int i = 0; i < ScoreManager.Instance.GetStarRating(); i++)
        {
            GameObject.Find($"Star_off_{i + 1}").transform.GetChild(0).gameObject.SetActive(true);
        }

        if (playerAnimator != null)
            playerAnimator.speed = originalAnimSpeed;

        cutsceneRunning = false;
    }

    private IEnumerator MovePlayerRoutine(GameObject player, System.Action onComplete)
    {
        while (Mathf.Abs(player.transform.position.x - doorCenter.position.x) > 0.05f)
        {
            Vector3 targetPos = new Vector3(
                doorCenter.position.x,
                player.transform.position.y,
                player.transform.position.z
            );

            player.transform.position = Vector3.MoveTowards(
                player.transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }
        playerAnimator.SetBool("IsJump", false);
        playerAnimator.SetBool("IsAir", false);
        playerAnimator.SetBool("IsAttack", false);
        playerAnimator.SetBool("IsRun", false);
        onComplete?.Invoke();
    }

    private IEnumerator ZoomCameraRoutine(System.Action onComplete)
    {
        if (vCamDoorCloseUp != null)
        {
            vCamDoorCloseUp.gameObject.SetActive(true);

            float initialSize = vCamDoorCloseUp.m_Lens.OrthographicSize;
            float elapsed = 0f;
            while (elapsed < zoomDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / zoomDuration;
                vCamDoorCloseUp.m_Lens.OrthographicSize = Mathf.Lerp(initialSize, targetOrthographicSize, t);
                yield return null;
            }
            vCamDoorCloseUp.m_Lens.OrthographicSize = targetOrthographicSize;
        }
        onComplete?.Invoke();
    }

    private IEnumerator ShowBlackBarsRoutine(System.Action onComplete)
    {
        float elapsed = 0f;

        Vector2 startPosTop = topBar.anchoredPosition;
        Vector2 endPosTop = new Vector2(topBar.anchoredPosition.x, 0 - topBar.sizeDelta.y/2);

        Vector2 startPosBottom = bottomBar.anchoredPosition;
        Vector2 endPosBottom = new Vector2(bottomBar.anchoredPosition.x, 0 + bottomBar.sizeDelta.y / 2);

        while (elapsed < barAnimationTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / barAnimationTime;

            topBar.anchoredPosition = Vector2.Lerp(startPosTop, endPosTop, t);
            bottomBar.anchoredPosition = Vector2.Lerp(startPosBottom, endPosBottom, t);

            yield return null;
        }

        topBar.anchoredPosition = endPosTop;
        bottomBar.anchoredPosition = endPosBottom;
        onComplete?.Invoke();
    }
}