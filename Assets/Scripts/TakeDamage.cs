using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TakeDamage : MonoBehaviour
{
    public float intensity_reference = 0.4f; 
    float intensity; // Corregir el valor inicial de la intensidad
    PostProcessVolume volume;
    Vignette vignette;

    private void Start()
    {
        volume = GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out vignette);
        if (!vignette)
        {
            Debug.Log("Vignette is empty");
        }
        else
        {
            vignette.enabled.Override(false);
        }
        FindObjectOfType<PlayerController>().OnPlayerDamaged += HandlePlayerDamaged;
    }

    // Método para manejar el daño al jugador
    private void HandlePlayerDamaged()
    {
        StartCoroutine(TakeDamageEffect());
    }
    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //    StartCoroutine(TakeDamageEffect()); // Corregir StartCorutine a StartCoroutine
    }

    private IEnumerator TakeDamageEffect()
    {
        intensity = intensity_reference;

        vignette.enabled.Override(true);
        vignette.intensity.Override(intensity);

        yield return new WaitForSeconds(intensity);

        while (intensity > 0)
        {
            intensity -= 0.01f;

            if (intensity < 0)
                intensity = 0;

            vignette.intensity.Override(intensity);

            yield return new WaitForSeconds(0.1f);
        }
        vignette.enabled.Override(false);
    }
}
