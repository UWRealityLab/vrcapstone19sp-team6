using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedText : MonoBehaviour
{
    public bool enableOpacity = true;
    public bool reverseDirection = false;

    private enum State { START, ENTERED, EXITED };
    private State state;

    private Vector3 finalPosition;
    private Color finalColor;

    private void Start()
    {
        state = State.START;
        finalPosition = transform.position;
        Renderer renderer = GetComponent<Renderer>();
        if (enableOpacity) {
            finalColor = renderer.material.color;
        }

        Reset();
    }

    public void Reset()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (enableOpacity)
        {
            renderer.material.color = Color.clear;
        }

        Vector3 startPosition = finalPosition;
        startPosition.y += reverseDirection ? 10.0f : -10.0f;
        transform.position = startPosition;
    }

    public void Enter(float duration)
    {
        if (state == State.START)
        {
            state = State.ENTERED;
            StartCoroutine(RiseFadeEntrance(finalPosition, finalColor, duration));
        }
    }

    public void Exit(float duration)
    {
        if (state == State.ENTERED)
        {
            state = State.EXITED;
            StartCoroutine(RiseFadeExit(finalPosition, finalColor, duration));
        }
    }

    IEnumerator RiseFadeEntrance(Vector3 finalPosition, Color finalColor, float duration)
    {
        Vector3 startPosition = finalPosition;
        startPosition.y += reverseDirection ? 10.0f : -10.0f;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
        {
            Color newColor = new Color(finalColor.r, finalColor.g, finalColor.b, Mathf.Lerp(0.0f, 1.0f, Easing.EaseOutQuart(0.0f, 1.0f, t)));
            GetComponent<Renderer>().material.color = newColor;
            Vector3 newPosition = Vector3.Lerp(startPosition, finalPosition, Easing.EaseOutQuart(0.0f, 1.0f, t));
            transform.position = newPosition;
            yield return null;
        }
    }

    IEnumerator RiseFadeExit(Vector3 startPosition, Color startColor, float duration)
    {
        Vector3 finalPosition = startPosition;
        finalPosition.y += reverseDirection ? -6.0f : 6.0f;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
        {
            Color newColor = new Color(finalColor.r, finalColor.g, finalColor.b, Mathf.Lerp(1.0f, 0.0f, Easing.EaseInQuart(0.0f, 1.0f, t)));
            GetComponent<Renderer>().material.color = newColor;
            Vector3 newPosition = Vector3.Lerp(startPosition, finalPosition, Easing.EaseInQuart(0.0f, 1.0f, t));
            transform.position = newPosition;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
