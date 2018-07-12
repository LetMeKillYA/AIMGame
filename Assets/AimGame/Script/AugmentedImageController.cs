using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using GoogleARCore;
using UnityEngine.UI;

public class AugmentedImageController : MonoBehaviour
{

    /// <summary>
    /// A prefab for visualizing an AugmentedImage.
    /// </summary>
    public GameObject AugmentedPrefab;

    /// <summary>
    /// The overlay containing the fit to scan user guide.
    /// </summary>
    public GameObject FitToScanOverlay;
    public Text debug;

    private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();

    public void OnEnable()
    {
        FitToScanOverlay.SetActive(true);
    }
    /// <summary>
    /// The Unity Update method.
    /// </summary>
    Anchor anchor = null;
    public void Update()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
              
        // Check that motion tracking is tracking.
        if (Session.Status != SessionStatus.Tracking || !gameObject.activeSelf)
        {
            return;
        }


        //debug.text = " " + Session.Status+"/"+anchor;
        // Get updated augmented images for this frame.
        Session.GetTrackables<AugmentedImage>(m_TempAugmentedImages, TrackableQueryFilter.Updated);

        // Create visualizers and anchors for updated augmented images that are tracking and do not previously
        // have a visualizer. Remove visualizers for stopped images.
        foreach (var image in m_TempAugmentedImages)
        {
            if (image.Name == "Earth")
            {
               
                //Debug.Log(image.TrackingState + "/" + AugmentedPrefab.activeSelf);
            }
                
            if (image.TrackingState == TrackingState.Tracking && anchor == null)
            {
                AugmentedPrefab.SetActive(true);
                anchor = image.CreateAnchor(image.CenterPose);
                AugmentedPrefab.transform.parent = anchor.transform;
                FitToScanOverlay.SetActive(false);
                //if(GameControl.GetInstance().aimAssistType != AssistType.BaseTest)
                StartCoroutine("ResetUIColliders");
                Debug.Log("In create Anchor *****************");
            }
            else if (image.TrackingState == TrackingState.Paused && anchor!=null)
            {
                AugmentedPrefab.transform.parent = null;
                AugmentedPrefab.SetActive(false);
                FitToScanOverlay.SetActive(true);
                anchor = null;

                Debug.Log("In destroy Anchor *****************");
            }
            else if(image.TrackingState == TrackingState.Tracking && GameControl.GetInstance() != null && !GameControl.GetInstance().IsActive())
            {
                AugmentedPrefab.transform.parent = null;
                AugmentedPrefab.SetActive(false);
                FitToScanOverlay.SetActive(true);
                anchor = null;
                Debug.Log("In destroy Anchor *****************");
            }
            
        }
                
       
    }

    IEnumerator ResetUIColliders()
    {
        yield return new WaitForSeconds(1f);

        TouchObjPool.GetInstance().ResetTargets();
        MenuManager.GetInstance().PurgeRound();
        GameControl.GetInstance().CallNextSet();
        GameControl.GetInstance().ActivateTargetObects();
    }
}

