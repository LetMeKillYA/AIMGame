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

        int round = MenuManager.GetInstance().modularNo;

        

        

        //debug.text = " " + Session.Status+"/"+anchor;
        // Get updated augmented images for this frame.
        Session.GetTrackables<AugmentedImage>(m_TempAugmentedImages, TrackableQueryFilter.Updated);

        // Create visualizers and anchors for updated augmented images that are tracking and do not previously
        // have a visualizer. Remove visualizers for stopped images.
        foreach (var image in m_TempAugmentedImages)
        {

            if(anchor == null)
            {
                /*if (round > 0 && round % 4 <= 2)
                {
                    if (image.Name != "Mars")
                        return;
                }
                if (round > 0 && round % 4 > 2)
                {
                    if (image.Name != "Earth")
                        return;
                }*/
                
                if (image.TrackingState == TrackingState.Tracking)
                {
                    AugmentedPrefab.SetActive(true);
                    anchor = image.CreateAnchor(image.CenterPose);
                    AugmentedPrefab.transform.parent = anchor.transform;
                    AugmentedPrefab.transform.localPosition = image.CenterPose.position;
                    FitToScanOverlay.SetActive(false);
                    //if(GameControl.GetInstance().aimAssistType != AssistType.BaseTest)
                    StartCoroutine("ResetUIColliders");
                    Debug.Log("In create Anchor *****************");
                }
            }
            else
            {
                if (image.TrackingState == TrackingState.Paused)
                {
                    AugmentedPrefab.transform.parent = null;
                    AugmentedPrefab.SetActive(false);
                   // FitToScanOverlay.SetActive(true);
                    DeleteAnchor();

                    Debug.Log("In destroy Anchor *****************");
                }

                /*if (round > 0 && round % 4 <= 2)
                {
                    if (image.Name != "Mars")
                        return;
                }
                if (round > 0 && round % 4 > 2)
                {
                    if (image.Name != "Earth")
                        return;
                }

                AugmentedPrefab.transform.localPosition = image.CenterPose.position;*/
            }
            

            if (image.TrackingState != TrackingState.Tracking && (GameControl.GetInstance() != null && !GameControl.GetInstance().IsActive()))
            {
                AugmentedPrefab.transform.parent = null;
                AugmentedPrefab.SetActive(false);
                //FitToScanOverlay.SetActive(true);
                DeleteAnchor();
                Debug.Log("In destroy Anchor *****************");
            }
            
        }

       

    }

    public void DeleteAnchor()
    {
        if(anchor != null)
            Destroy(anchor.gameObject);
        anchor = null;
        FitToScanOverlay.SetActive(true);
    }

    IEnumerator ResetUIColliders()
    {
        yield return new WaitForSeconds(1f);

        TouchObjPool.GetInstance().ResetTargets();
        MenuManager.GetInstance().PurgeRound();
        //GameControl.GetInstance().CallNextSet(true);
        GameControl.GetInstance().ActivateTargetObects();
    }
}

