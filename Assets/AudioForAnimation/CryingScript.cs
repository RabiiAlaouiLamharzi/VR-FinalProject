using System.Collections;
using UnityEngine;

public class CryingScript : MonoBehaviour
{
    public Animator animator;
    public AudioSource cryingAudio;

    [Header("Crying Variation")]
    public float minPause = 0.5f;
    public float maxPause = 2.0f;
    public Vector2 pitchRange = new Vector2(0.9f, 1.05f);
    public Vector2 volumeRange = new Vector2(0.6f, 1.0f);

    Coroutine cryingRoutine;

    public void StartCrying()
    {
        animator.Play("Crying");

        if (cryingRoutine != null)
            StopCoroutine(cryingRoutine);

        cryingRoutine = StartCoroutine(CryLoop());
    }

    IEnumerator CryLoop()
    {
        while (true)
        {
            cryingAudio.pitch = Random.Range(pitchRange.x, pitchRange.y);
            cryingAudio.volume = Random.Range(volumeRange.x, volumeRange.y);

            cryingAudio.Play();

            yield return new WaitForSeconds(cryingAudio.clip.length);
            yield return new WaitForSeconds(Random.Range(minPause, maxPause));
        }
    }

    public void StopCrying()
    {
        if (cryingRoutine != null)
            StopCoroutine(cryingRoutine);

        cryingRoutine = null;
        cryingAudio.Stop();
    }
}
