using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScreenFade : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Image _fadeScreen = null;

    UnityAction _onFadeDone;

    public void FadeIn(float duration, UnityAction onFadeDone = null)
    {
        StartCoroutine(FadeInCor(duration, onFadeDone));
    }

    public IEnumerator FadeInCor(float duration, UnityAction onFadeDone = null)
    {
        _fadeScreen.gameObject.SetActive(true);

        float timer = 0f;
        Color color = Color.black;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(timer / duration);
            _fadeScreen.color = color;
            yield return null;
        }
        color.a = 1f;
        _fadeScreen.color = color;
        yield return null;

        onFadeDone?.Invoke();
    }

    public void FadeOut(float duration, UnityAction onFadeDone = null)
    {
        StartCoroutine(FadeOutCor(duration, onFadeDone));
    }

    public IEnumerator FadeOutCor(float duration, UnityAction onFadeDone = null)
    {
        float timer = duration;
        Color color = Color.black;
        while (timer >= 0f)
        {
            timer -= Time.deltaTime;
            color.a = Mathf.Clamp01(timer / duration);
            _fadeScreen.color = color;
            yield return null;
        }

        _fadeScreen.gameObject.SetActive(false);
        onFadeDone?.Invoke();
    }

    public void FadeTransition(float duration, IEnumerator callback)
    {
        StartCoroutine(FadeTransitionCor(duration, callback));
    }

    public IEnumerator FadeTransitionCor(float duration, IEnumerator callback)
    {
        yield return FadeInCor(duration / 2f);

        yield return StartCoroutine(callback);

        yield return FadeOutCor(duration / 2f);
    }

    public void FadeTransition(float duration, UnityAction callback)
    {
        StartCoroutine(FadeTransitionCor(duration, callback));
    }

    public IEnumerator FadeTransitionCor(float duration, UnityAction callback)
    {
        yield return FadeInCor(duration / 2f);

        callback();

        yield return FadeOutCor(duration / 2f);
    }
}