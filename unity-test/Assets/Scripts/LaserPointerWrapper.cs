using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

public class LaserPointerWrapper : MonoBehaviour
{
    private SteamVR_LaserPointer pointer;

    // Start is called before the first frame update
    void Start()
    {
        pointer = gameObject.GetComponent<SteamVR_LaserPointer>();
        pointer.PointerIn += OnPointerIn;
        pointer.PointerOut += OnPointerOut;
        pointer.PointerClick += OnPointerClick;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPointerIn(object sender, PointerEventArgs e)
    {
        IPointerEnterHandler pointerEnterHandler = e.target.GetComponent<IPointerEnterHandler>();
        if (pointerEnterHandler == null)
        {
            return;
        }

        pointerEnterHandler.OnPointerEnter(new PointerEventData(EventSystem.current));
        //e.target.gameObject.SendMessage("OnHandHoverBegin", gameObject.GetComponent<Hand>());
    }

    private void OnPointerOut(object sender, PointerEventArgs e)
    {
        IPointerExitHandler pointerExitHandler = e.target.GetComponent<IPointerExitHandler>();
        if (pointerExitHandler == null)
        {
            return;
        }

        pointerExitHandler.OnPointerExit(new PointerEventData(EventSystem.current));
        //e.target.gameObject.SendMessage("OnHandHoverEnd", gameObject.GetComponent<Hand>());
    }

    private void OnPointerClick(object sender, PointerEventArgs e)
    {
        IPointerClickHandler pointerClickHandler = e.target.GetComponent<IPointerClickHandler>();
        if (pointerClickHandler == null)
        {
            return;
        }

        pointerClickHandler.OnPointerClick(new PointerEventData(EventSystem.current));
    }
}
