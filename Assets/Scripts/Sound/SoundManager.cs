using System.Collections;
using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] ambientClips;

    [Header("AudioSource")]
    [SerializeField] private AudioSource audioSource;

    [Header("Delay Between Clips")]
    [SerializeField] private float minDelay = 3f;
    [SerializeField] private float maxDelay = 8f;

    [Header("Volume")]
    [SerializeField] private float volume = 0.5f;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = false; // no queremos que se repita automáticamente
        audioSource.volume = volume;

        if (ambientClips.Length > 0)
        {
            StartCoroutine(PlayAmbientLoop());
        }
        else
        {
            Debug.LogWarning("AmbientSoundManager: No se han asignado clips de audio.", this);
        }
    }

    private IEnumerator PlayAmbientLoop()
    {
        while (true)
        {
            // Elegir un clip aleatorio
            AudioClip clip = ambientClips[Random.Range(0, ambientClips.Length)];
            audioSource.clip = clip;
            audioSource.Play();

            // Esperar a que termine el clip
            yield return new WaitForSeconds(clip.length);

            // Esperar un tiempo aleatorio antes de reproducir el siguiente
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
        }
    }
}
