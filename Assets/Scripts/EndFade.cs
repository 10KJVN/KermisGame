using UnityEngine;
using System.Collections;

public class EndFade : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float duration = 3f;

    public void FadeToWhite()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        Color start = Color.black;
        Color end = Color.white;

        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            cam.backgroundColor = Color.Lerp(start, end, t / duration);
            yield return null;
        }

        cam.backgroundColor = end;
    }
}