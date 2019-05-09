using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class Grab : MonoBehaviour
    {
        public Material grabMat;
        private Material originalMat;

        [EnumFlags]
        public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand;

        
        private void HandHoverUpdate(Hand hand)
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();
            if (startingGrabType != GrabTypes.None)
            {
                hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
            }
        }

            
        private void HandAttachedUpdate(Hand hand)
        {
            if (hand.IsGrabEnding(gameObject))
            {
                hand.DetachObject(gameObject);
            }
        }
        

        private void OnAttachedToHand(Hand hand)
        {
            //GetComponent<Renderer>().material = grabMat;
        }

        private void OnDetachedFromHand(Hand hand)
        {
            GetComponent<Renderer>().material = originalMat;
        }

        // Start is called before the first frame update
        void Start()
        {
            originalMat = GetComponent<Renderer>().material;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}