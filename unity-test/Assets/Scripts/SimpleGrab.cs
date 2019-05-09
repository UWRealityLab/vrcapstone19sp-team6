using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Valve.VR.InteractionSystem
{
    public class SimpleGrab : MonoBehaviour
    {
        public Material grabMat;
        private Material originalMat;

        [EnumFlags]
        public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand;

        public void attachToHand(bool attached, GameObject go) {
            if (attached) {
                Debug.Log("attaching" + go);
                if (go.GetComponent<Hand>() != null) {
                    go.GetComponent<Hand>().DetachObject(gameObject);
                }              
            }
            else
            {
                if (go.GetComponent<Hand>() != null)
                {
                    go.GetComponent<Hand>().AttachObject(gameObject, GrabTypes.None, attachmentFlags);
                }
            }
        }

         private void HandHoverUpdate(Hand hand)
         {
            GrabTypes startingGrabType = hand.GetGrabStarting();
            Debug.Log("Entered HandHoverUpdate");
            hand.AttachObject(gameObject, GrabTypes.Trigger, attachmentFlags);
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
            //GetComponent<Renderer>().material = originalMat;
        }

        // Start is called before the first frame update
        void Start()
        {
            //originalMat = GetComponent<Renderer>().material;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
