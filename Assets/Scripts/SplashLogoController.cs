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

    //Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        splashImage.color = new Color(1, 1, 1, 0);
        splashImage.raycastTarget = false;
        StartCoroutine(PlaySplash());
    }

    private IEnumerator PlaySplash()
    {
        for (int i = 0; i < logos.Length; i++)
        {
            splashImage.sprite = logos[i];

            //Fade in
            yield return StartCoroutine(FadeImage(1, fadeDuration));

            // Mantener imagen visible
            yield return new WaitForSeconds(imageDuration);

            //Fade out
            yield return StartCoroutine(FadeImage(0, fadeDuration));

            //Reset sprite al terminar el fade out
            splashImage.sprite = null;
        }

        //Cargar siguiente escena
        SceneManager.LoadScene("MainMenu");
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
