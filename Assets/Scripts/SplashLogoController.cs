using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashLogoController : MonoBehaviour
{
    public Sprite[] logos;
    public Image splashImage;
    public float imageDuration = 2f;
    public float fadeDuration = 0.5f;

    private void Start()
    {
        splashImage.color = new Color(1, 1, 1, 0);
        splashImage.raycastTarget = false;

        StartCoroutine(PlaySplash());
    }

    private IEnumerator PlaySplash()
    {
        // Reproduce cada logo
        for (int i = 0; i < logos.Length; i++)
        {
            splashImage.sprite = logos[i];

            // Fade In
            yield return StartCoroutine(FadeImage(1, fadeDuration));

            // Esperar
            yield return new WaitForSeconds(imageDuration);

            // Fade Out
            yield return StartCoroutine(FadeImage(0, fadeDuration));
            splashImage.sprite = null;
        }

        Debug.Log("Splash: Animaciones terminadas. Esperando Firebase...");

        // Esperar a que FirebaseInitializer termine todo
        while (!FirebaseInitializer.Instance.IsReady)
            yield return null;

        Debug.Log("Splash: Firebase listo. Cargando Login...");

        SceneManager.LoadScene("Login");
    }

    private IEnumerator FadeImage(float targetAlpha, float duration)
    {
        float startAlpha = splashImage.color.a;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            splashImage.color = new Color(1, 1, 1, newAlpha);
            yield return null;
        }

        splashImage.color = new Color(1, 1, 1, targetAlpha);
    }
}
