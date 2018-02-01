using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ScreenFader : MonoBehaviour {
    public Image img;
    public AnimationCurve curve;
    public Canvas fadecanvas;
    private Camera CurrentCam;
    void Start() {
        StartCoroutine(FadeIn());
        CurrentCam = Camera.main;
    }
    //between cams
    public void fadeToCam(Camera cam)
    {
        StartCoroutine(FadeOutCam(cam));
    }
    IEnumerator FadeOutCam(Camera cam)
    {
        fadecanvas.enabled = true;
        float t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }
        //do thing
        CurrentCam.enabled = false;
        CurrentCam = cam;
        CurrentCam.enabled = true;
        StartCoroutine(FadeIn());
    }
    //between scenes
    public void fadeTo(string scene) {
        StartCoroutine(FadeOut(scene));
    }
    IEnumerator FadeIn() {
        float t = 1f;

        while (t > 0)
        {
            t -= Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }
        fadecanvas.enabled = false;
    }

    IEnumerator FadeOut(string scene)
    {
        fadecanvas.enabled = true;
        float t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }
        SceneManager.LoadScene(scene);
    }
    public void fadeToNothing()
    {
        StartCoroutine(FadeOutToNothing());
    }
    IEnumerator FadeOutToNothing()
    {
        fadecanvas.enabled = true;
        float t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }
    }
}
