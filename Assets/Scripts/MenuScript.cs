using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuScript : MonoBehaviour
{
    public string PLAYER = "MainMenu"; // Constante para el nombre de la animación del menú
    string sceneName = "LevelScene";
    Animator animator;
    CanvasGroup transitionImageCanvasGroup;

    // Método para iniciar el juego
    public void StartGame()
    {
        animator = GetComponentInParent<Animator>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        Animator playerAnimator = playerObject.GetComponent<Animator>();

        playerAnimator.CrossFadeInFixedTime(PLAYER, 0.2f);

        StartCoroutine(FadeToBlackAndLoadScene(sceneName));
    }

    // Método para cargar la escena del juego
    private IEnumerator FadeToBlackAndLoadScene(string sceneName)
    {
        animator.SetTrigger("FadeOut");

        // Desactivar la interactividad de la imagen de transición
        transitionImageCanvasGroup = GetComponentInChildren<CanvasGroup>();
        if (transitionImageCanvasGroup != null)
            transitionImageCanvasGroup.blocksRaycasts = false;

        yield return new WaitForSeconds(2.0f); // Esperar un segundo para que la animación se reproduzca

        Debug.Log("Cambiar escena");
        SceneManager.LoadScene(sceneName);

        // Esperar a que la escena se cargue completamente
        yield return new WaitForSeconds(0.5f);

        // Volver a activar la interactividad de la imagen de transición
        if (transitionImageCanvasGroup != null)
            transitionImageCanvasGroup.blocksRaycasts = true;
    }

    // Método para salir del juego
    public void Exit()
    {
        Application.Quit();
        Debug.Log("Aquí se cierra el juego");
    }
}
