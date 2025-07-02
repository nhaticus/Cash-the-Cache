using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 * Pause Menu and GameOver spawner
 */
public class GameUI : MonoBehaviour
{
    [Header("UI Dependencies")]
    [SerializeField] GameObject pausePrefab;
    [SerializeField] GameObject gameOverPrefab;
    [SerializeField] TaskListScript taskList;

    [Header("Audio")]
    [SerializeField] SingleAudio singleAudio;

    private void Start()
    {
        StartCoroutine(FindPlayer());

        // remove task list if played already
        
    }

    private void Update()
    {
        if ((UserInput.Instance && UserInput.Instance.Pause) || (UserInput.Instance == null && Input.GetKeyDown(KeyCode.P)))
        {
            Pause();
        }
    }

    // connect OnDeath event to Game Over
    GameObject player;
    IEnumerator FindPlayer()
    {
        // keep searching for player
        while(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForSeconds(0.5f);
        }

        /*  Event Subscribing  */
        player.GetComponentInChildren<HealthController>().OnDeath.AddListener(GameOver);
    }

    // creates pause menu
    // Game pausing happens in PauseMenu's Start()
    GameObject pauseRef;
    bool paused = false;
    void Pause()
    {
        paused = !paused;
        if (paused)
        {
            pauseRef = Instantiate(pausePrefab, transform);
            pauseRef.GetComponentInChildren<PauseMenu>().UnPause.AddListener(UnPause);
        }
        else
        {
            pauseRef.GetComponent<PauseMenu>().ReturnToGame();
        }
    }

    void UnPause() // Return to Game in Pause Menu
    {
        paused = false;
    }

    void GameOver()
    {
        StartCoroutine(CreateGameOverScreen());
        GameManager.Instance.SetGameState(GameManager.GameState.Over);

        // unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    float timeTillGameOverReveal = 0.7f;
    float totalFadeInTime = 1f;
    IEnumerator CreateGameOverScreen()
    {
        // play death sound
        singleAudio.PlaySFX("death");

        // create game over screen
        GameObject gameOver = Instantiate(gameOverPrefab, transform);
        gameOver.GetComponent<GameOver>().PlayerLose();
        // hide game over
        CanvasGroup gameOverCanvas = gameOver.GetComponent<CanvasGroup>();
        gameOverCanvas.alpha = 0;

        // fade out from white
        StartCoroutine(CreateWhiteFade());

        // wait for a little then fade in game over
        yield return new WaitForSeconds(timeTillGameOverReveal);

        // fade in canvas
        float fadeTimer = 0;
        while (fadeTimer <= totalFadeInTime)
        {
            fadeTimer += Time.deltaTime;
            gameOverCanvas.alpha = fadeTimer / totalFadeInTime;
            yield return null;
        }
    }

    float fadeInTime = 0.15f;
    float fadeOutTime = 0.75f;
    IEnumerator CreateWhiteFade()
    {
        GameObject newObj = new GameObject(); //Create the GameObject
        newObj.name = "FadeWhite";
        newObj.transform.SetParent(this.transform);
        newObj.transform.localPosition = Vector3.zero;
        Image newImage = newObj.AddComponent<Image>();
        newObj.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
        float timer = 0;
        while(timer < fadeInTime)
        {
            timer += Time.deltaTime;

            // fade in
            Color color = newImage.color;
            color.a = timer / fadeInTime;
            newImage.color = color;

            yield return null;
        }

        timer = fadeOutTime;
        while(timer >= 0)
        {
            timer -= Time.deltaTime;

            // fade out
            Color color = newImage.color;
            color.a = timer / fadeInTime;
            newImage.color = color;

            yield return null;
        }
    }
}
