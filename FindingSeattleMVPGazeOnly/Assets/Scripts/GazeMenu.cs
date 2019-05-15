using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeMenu : GazeInteractable
{
    public GameObject menuContainer;
    public Color menuItemColor;
    public GameObject[] showOnMenuSelect;

    private readonly float CLOSED_SCALE = 0.08f;
    private readonly float OPEN_SCALE = 0.55f;
    private bool isSelected;

    private Vector3[] menuItemPositions;
    private Vector3 menuButtonPosition;

    protected override void Start()
    {
        menuButtonPosition = transform.Find("MenuButton").position;
        menuItemPositions = new Vector3[showOnMenuSelect.Length];
        for (int i = 0; i < showOnMenuSelect.Length; i++)
        {
            menuItemPositions[i] = showOnMenuSelect[i].transform.position;
        }
        base.Start();
    }

    public override void Select()
    {
        isSelected = true;

        StartCoroutine(ScaleMenu(CLOSED_SCALE, OPEN_SCALE, 0.8f));
        for (int i = 0; i < showOnMenuSelect.Length; i++)
        {
            showOnMenuSelect[i].SetActive(true);
            StartCoroutine(AnimateMenuItem(showOnMenuSelect[i], menuButtonPosition, menuItemPositions[i], Color.clear, menuItemColor, 0.8f, false));
        }

        base.Select();
    }

    public override void Deselect()
    {
        isSelected = false;

        StartCoroutine(ScaleMenu(OPEN_SCALE, CLOSED_SCALE, 0.8f));

        for (int i = 0; i < showOnMenuSelect.Length; i++)
        {
            StartCoroutine(AnimateMenuItem(showOnMenuSelect[i], menuItemPositions[i], menuButtonPosition, menuItemColor, Color.clear, 0.5f, true));
        }

        base.Deselect();
    }

    public override Vector3 GetPosition()
    {
        return transform.Find("MenuButton").position;
    }

    public override Quaternion GetRotation()
    {
        return transform.Find("MenuButton").rotation * Quaternion.Euler(90.0f, 0.0f, 0.0f);
    }

    public bool getSelected()
    {
        return isSelected;
    }

    IEnumerator ScaleMenu(float startScale, float finishScale, float duration)
    {
        Vector3 startScaleVector = new Vector3(startScale, startScale, 1f);
        Vector3 finishScaleVector = new Vector3(finishScale, finishScale, 1f);
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
        {
            menuContainer.transform.localScale = Vector3.Lerp(startScaleVector, finishScaleVector, Easing.EaseOutQuart(0.0f, 1.0f, t));
            yield return null;
        }
    }

    IEnumerator AnimateMenuItem(GameObject menuItem, Vector3 startPosition, Vector3 endPosition, Color startColor, Color endColor, float duration, bool disableAfterAnimation)
    {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
        {
            Color newColor = Color.Lerp(startColor, endColor, Easing.EaseOutQuart(0.0f, 1.0f, t));
            menuItem.GetComponent<Renderer>().material.color = newColor;
            menuItem.transform.Find("Description").GetComponent<Renderer>().material.color = newColor;
            menuItem.transform.position = Vector3.Lerp(startPosition, endPosition, Easing.EaseOutQuart(0.0f, 1.0f, t));
            yield return null;
        }
        if (disableAfterAnimation)
        {
            menuItem.SetActive(false);
        }
    }
}
