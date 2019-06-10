using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GazeTracker : MonoBehaviour
{
    /*
     * The maximum distance at which the user can interact with something using their gaze.
     */
    public float gazeDistance = 100.0f;

    /*
     * The number of seconds the user must gaze at something before taking an action on it.
     */
    public float gazeTimeToSelect = 5.0f;

    /*
     * The progress bar Image
     */
    public Image progressBar;

    /*
     * The Gaze Menu
     */
    public GazeMenu gazeMenu;

    /*
     * The Gaze Area that deselects the Gaze Menu
     */
    public GazeArea gazeMenuDeselect;

    /*
     * The cursor that displays while the Gaze Menu is open
     */
    public GameObject gazeMenuCursor;

    private GazeInteractable currentGazeInteractable;
    private GazeArea currentGazeArea;
    private float currentGazeTime;

    private readonly int gazeAreaLayerMask = 1 << 9;
    private readonly int gazeAreaContainerLayerMask = 1 << 10;

    private void Start()
    {
        progressBar.fillAmount = 0;
    }

    private void FixedUpdate()
    {
        if (gazeMenu.getSelected())
        {
            gazeMenuCursor.SetActive(true);
            Ray containerRay = new Ray(transform.position, transform.forward);
            RaycastHit containerRaycastHit;

            if (Physics.Raycast(containerRay, out containerRaycastHit, gazeDistance, gazeAreaContainerLayerMask))
            {
                gazeMenuCursor.transform.position = containerRaycastHit.point;
            }
        } else
        {
            gazeMenuCursor.SetActive(false);
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, gazeDistance, gazeAreaLayerMask))
        {
            GameObject raycastHitObject = raycastHit.collider.gameObject;

            if (raycastHitObject.GetComponent<GazeArea>() != null)
            {
                currentGazeArea = raycastHitObject.GetComponent<GazeArea>();
                if (currentGazeArea == gazeMenuDeselect)
                {
                    gazeMenu.Deselect();
                }

                GazeInteractable raycastHitInteractable = raycastHitObject.GetComponent<GazeArea>().getGazeInteractable();

                if (currentGazeInteractable != null && raycastHitInteractable == currentGazeInteractable)
                {
                    // This is the same GazeInteractable. Update the amount of time
                    // spent gazing at this object.
                    currentGazeTime += Time.fixedDeltaTime;

                    // Show the progress bar in the correct position, centered on the
                    // GazeInteractable that corresponds to the current GazeArea
                    progressBar.fillAmount = currentGazeTime / gazeTimeToSelect;
                    if (currentGazeTime / gazeTimeToSelect > 1.0f)
                    {
                        progressBar.fillAmount = 0.0f;
                    }

                    Transform progressBarParent = progressBar.transform.parent;
                    progressBarParent.SetPositionAndRotation(
                            currentGazeInteractable.GetPosition(),
                            currentGazeInteractable.GetRotation());

                    if (currentGazeTime >= gazeTimeToSelect)
                    {
                        currentGazeInteractable.Select();
                    }

                    return;
                }
                else
                {
                    // This is a new GazeInteractable.

                    if (currentGazeInteractable != null)
                    {
                        // There was previously a GazeInteractable, so let it know that
                        // the gaze has left it.
                        currentGazeInteractable.GazeStop();
                    }

                    currentGazeInteractable = raycastHitInteractable;
                    currentGazeInteractable.GazeStart();
                    currentGazeTime = 0.0f;
                    progressBar.fillAmount = 0;

                    return;
                }
            }
        }

        // No GazeInteractable objects are currently under the cursor.

        if (currentGazeInteractable != null)
        {
            // There was previously a GazeInteractable selected.
            // Reset the currentGaze state.
            currentGazeInteractable.GazeStop();
            currentGazeInteractable = null;
            currentGazeArea = null;
            currentGazeTime = 0.0f;
            progressBar.fillAmount = 0;
        }
    }
}
