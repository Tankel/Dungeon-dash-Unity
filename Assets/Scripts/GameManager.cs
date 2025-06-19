using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int lives = 5;
    public int collectedCollectibles = 0;
    public int totalCollectibles = 3;

    public GameObject heart1, heart2, heart3, heart4, heart5;
    public GameObject star1, star2, star3;

    private RawImage star1Image, star2Image, star3Image; // Referencias a los RawImages de los objetos star

    private bool hasReachedEndPoint = false; 
    public string gameSceneName = "GameScene";
    public string gameOverSceneName = "GameOverScene";
    public string youWinSceneName = "YouWinScene";
    public string mainMenuSceneName = "MainMenuScene";

    private Color collectedColor = new Color(1f, 0.93f, 0f); // Color amarillo cuando se ha coleccionado un coleccionable

    private void Awake()
    {
        Cursor.visible = false;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        lives = 5;
        heart1.gameObject.SetActive(true);
        heart2.gameObject.SetActive(true);
        heart3.gameObject.SetActive(true);
        heart4.gameObject.SetActive(true);
        heart5.gameObject.SetActive(true);

        // Obtener los componentes RawImage de los objetos star
        star1Image = star1.GetComponentInChildren<RawImage>();
        star2Image = star2.GetComponentInChildren<RawImage>();
        star3Image = star3.GetComponentInChildren<RawImage>();
    }

    private void Update()
    {
        switch (lives)
        {
            case 4:
                heart5.gameObject.SetActive(false);
                break;
            case 3:
                heart4.gameObject.SetActive(false);
                break;
            case 2:
                heart3.gameObject.SetActive(false);
                break;
            case 1:
                heart2.gameObject.SetActive(false);
                break;
            case 0:
                heart1.gameObject.SetActive(false);
                break;
        }
    }

    public void LoseLife()
    {
        lives--;
        if (lives <= 0)
        {
            Invoke("GameOver",0.6f);
        }
    }

    public void CollectCollectible()
    {
        collectedCollectibles++;

        // Cambiar el color de los RawImages de los objetos star
        switch (collectedCollectibles)
        {
            case 1:
                star1Image.color = collectedColor;
                break;
            case 2:
                star2Image.color = collectedColor;
                break;
            case 3:
                star3Image.color = collectedColor;
                break;
        }
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void GameOver()
    {
        Cursor.visible = true;
        SceneManager.LoadScene(gameOverSceneName);
    }

    public void Victory()
    {
        Cursor.visible = true;
        SceneManager.LoadScene(youWinSceneName);
    }

    public void CheckVictory()
    {
        if (hasReachedEndPoint && collectedCollectibles >= totalCollectibles)
        {
            Victory();
        }
    }

    // Método para activar el evento de victoria cuando el jugador llega al punto final
    public void SetReachedEndPoint()
    {
        hasReachedEndPoint = true;
        CheckVictory(); // Verificar si se cumple la condición de victoria
    }
}
