using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour {

    [SerializeField] protected private float _solidAlpha = 1f;
    [SerializeField] protected private float _clearAlpha = 0f;
    [SerializeField] private float _fadeOnDuration = 2f;
    public float FadeOnDuration { get => _fadeOnDuration; }
    [SerializeField] private float _fadeOffDuration = 2f;
    public float FadeOffDuration { get => _fadeOffDuration; }
    [SerializeField] private MaskableGraphic[] graphicsToFade; // array of ui elements to fade

    protected private void SetAlpha(float alpha) { // loop and set alpha to same transp.
        foreach (MaskableGraphic graphic in graphicsToFade) {
            if (graphic != null) {
                graphic.canvasRenderer.SetAlpha(alpha);
            }
        }
    }

    private void Fade(float targetAlpha, float duration) { // fade
        foreach (MaskableGraphic graphic in graphicsToFade) {
            if (graphic != null) {
                graphic.CrossFadeAlpha(targetAlpha, duration, true);
            }
        }
    }

    public void FadeOff() { //set and fade
        SetAlpha(_solidAlpha);
        Fade(_clearAlpha, _fadeOffDuration);
    }

    public void FadeOn() { //set and reverse
        SetAlpha(_clearAlpha);
        Fade(_solidAlpha, _fadeOnDuration);
    }

    public static IEnumerator FadeCanvas(CanvasGroup canvas, float startAlpha, float endAlpha, float duration) {
        var startTime = Time.time;
        var endTime = Time.time + duration;
        var elapsedTime = 0f;

        
        canvas.alpha = startAlpha;// set the canvas to the start alpha 
        
        while (Time.time <= endTime) {// loop repeatedly until the previously calculated end time
            elapsedTime = Time.time - startTime; // update the elapsed time
            var percentage = 1 / (duration / elapsedTime); // calculate how far along the timeline we are
            if (startAlpha > endAlpha) // if we are fading out/down 
            {
                canvas.alpha = startAlpha - percentage; // calculate the new alpha
            } else // if we are fading in/up
              {
                canvas.alpha = startAlpha + percentage; // calculate the new alpha
            }

            yield return new WaitForEndOfFrame(); // wait for the next frame before continuing the loop
        }
        canvas.alpha = endAlpha; // force the alpha to the end alpha before finishing 
    }
}