using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour
{
    public ParticleSystem collectEffect; // Referencia al sistema de partículas
    public float fadeDuration = 0.5f; // Duración de la transición de opacidad
    private Renderer objectRenderer;
    private bool collected = false; // Variable para controlar si el coleccionable ya ha sido recolectado

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        // Detener el efecto de partículas al inicio
        if (collectEffect != null)
        {
            collectEffect.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entra en contacto es el jugador y si el coleccionable aún no ha sido recolectado
        if (other.CompareTag("Player") && !collected)
        {
            // Recolectar el coleccionable
            GameManager.instance.CollectCollectible();

            // Reproducir el efecto de partículas
            if (collectEffect != null)
            {
                collectEffect.Play();
            }

            // Iniciar la animación de desvanecimiento
            StartCoroutine(FadeOutAndDestroy());

            // Marcar el coleccionable como recolectado para evitar que se recolecte nuevamente
            collected = true;
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float elapsedTime = 0f;
        Color initialColor = objectRenderer.material.color;
        while (elapsedTime < fadeDuration)
        {
            // Calcular el nuevo color con opacidad reducida
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            Color newColor = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            // Aplicar el nuevo color al material del objeto
            objectRenderer.material.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegurarse de que el objeto sea completamente invisible antes de destruirlo
        objectRenderer.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

        // Destruir el objeto después de un breve retraso
        Destroy(gameObject, 0.1f);
    }
}
