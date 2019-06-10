using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseumExhibit : MonoBehaviour
{
    private enum State { START, ENTERED, EXITED };
    private State state;

    private Vector3 finalPosition;

    private void Start()
    {
        state = State.START;

        finalPosition = transform.position;
        Vector3 startPosition = finalPosition;
        startPosition.y += 20.0f;
        transform.position = startPosition;
    }

    public void Enter(float duration)
    {
        if (state == State.START)
        {
            state = State.ENTERED;
            StartCoroutine(Entrance(finalPosition, duration));
        }
    }

    public void Exit(float duration)
    {
        if (state == State.ENTERED)
        {
            state = State.EXITED;
            StartCoroutine(Exit(finalPosition, duration));
        }
    }

    IEnumerator Entrance(Vector3 finalPosition, float duration)
    {
        Vector3 startPosition = finalPosition;
        startPosition.y += 20.0f;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
        {
            Vector3 newPosition = Vector3.Lerp(startPosition, finalPosition, Easing.EaseOutQuart(0.0f, 1.0f, t));
            transform.position = newPosition;
            yield return null;
        }
    }

    IEnumerator Exit(Vector3 startPosition, float duration)
    {
        Vector3 finalPosition = startPosition;
        finalPosition.y += 20.0f;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
        {
            Vector3 newPosition = Vector3.Lerp(startPosition, finalPosition, Easing.EaseInQuart(0.0f, 1.0f, t));
            transform.position = newPosition;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
