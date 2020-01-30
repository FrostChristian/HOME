using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionFader : ScreenFader {
    [SerializeField] private float _lifetime = 1f; // how long will transition take
    [SerializeField] private float _delay = 0.3f; // how long still we start fading off or on
    public float Delay { get { return _delay; } }

    protected void Awake() {
        _lifetime = Mathf.Clamp(_lifetime, FadeOnDuration + FadeOffDuration + _delay, 15f); // set lifetime minimum´, FadeOn + FadeOff + Delay
    }

    private IEnumerator PlayRoutine() {
        SetAlpha(_clearAlpha);
        yield return new WaitForSecondsRealtime(_delay);
        FadeOn();

        float onTime = _lifetime - (FadeOffDuration + _delay); // subtract from lifetime
        //Debug.Log("onTime" + onTime);
        yield return new WaitForSecondsRealtime(onTime);
        //Debug.Log("onTime" + onTime);
        FadeOff();
        Destroy(gameObject, FadeOffDuration);

    }

    public void Play() {
        StartCoroutine(PlayRoutine());
    }

    public static void PlayTransition(TransitionFader transitionPrefab) {
        if (transitionPrefab != null) {
            TransitionFader instance = Instantiate(transitionPrefab, Vector3.zero, Quaternion.identity);
            instance.Play();
        }
    }

}
