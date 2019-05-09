using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

public class LaserPointerWrapper : MonoBehaviour
{
    private SteamVR_LaserPointer pointer;
    public bool attached;

    [EnumFlags]
    public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand;


    // Start is called before the first frame update
    void Start()
    {
        pointer = gameObject.GetComponent<SteamVR_LaserPointer>();
        pointer.PointerIn += OnPointerIn;
        pointer.PointerOut += OnPointerOut;
        pointer.PointerClick += OnPointerClick;
        attached = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPointerIn(object sender, PointerEventArgs e)
    {
        IPointerEnterHandler pointerEnterHandler = e.target.GetComponent<IPointerEnterHandler>();
        //Debug.Log("entered onPointerIn");

        //pointerEnterHandler.OnPointerEnter(new PointerEventData(EventSystem.current));
        //e.target.gameObject.SendMessage("HandHoverUpdate", gameObject.GetComponent<Hand>());
    }

    private void OnPointerOut(object sender, PointerEventArgs e)
    {
        //Debug.Log("entered onPointerOut");
        IPointerExitHandler pointerExitHandler = e.target.GetComponent<IPointerExitHandler>();
       
        //pointerExitHandler.OnPointerExit(new PointerEventData(EventSystem.current));
        //e.target.gameObject.SendMessage("OnHandHoverEnd", gameObject.GetComponent<Hand>());
    }

    private void OnPointerClick(object sender, PointerEventArgs e)
    {
        Debug.Log("entered onPointerClick");
        if (attached)
        {
            Debug.LogWarning("DeAttaching " + e.target.gameObject);
           
            gameObject.GetComponent<Hand>().DetachObject(e.target.gameObject);
            attached = false;
        }
        else
        {
            Debug.LogWarning("Attaching " + e.target.gameObject);
            gameObject.GetComponent<Hand>().AttachObject(e.target.gameObject, GrabTypes.Trigger, attachmentFlags);
            attached = true;
        }
        //gameObject.GetComponent<Hand>().AttachObject(e.target.gameObject, GrabTypes.None, attachmentFlags);
        //e.target.gameObject.SendMessage("HandHoverUpdate", gameObject.GetComponent<Hand>());

    }
}
