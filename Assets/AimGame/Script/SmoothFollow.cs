// Smooth Follow from Standard Assets
// Converted to C# because I fucking hate UnityScript and it's inexistant C# interoperability
// If you have C# code and you want to edit SmoothFollow's vars ingame, use this instead.
using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 offsetPosition;

    [SerializeField]
    private Space offsetPositionSpace = Space.Self;

    [SerializeField]
    private bool lookAt = true;

    [SerializeField]
    private float damping = 1;

    [SerializeField]
    private PlayerInputController player;

    private Vector3 prevPosition;

    private void Update()
    {
       Refresh();
    }

    Vector3 tempPos;
    float lerper = 0;
    public float dampingMultiplier = 1f;
    public void Refresh()
    {

       // if (Vector3.Distance(prevPosition, transform.position) < 10)
       //     return;
        

        if (target == null)
        {
            
            Debug.LogWarning("Missing target ref !", this);

            return;
        }
        damping = PlayerInputController.sFactor;
        // compute position
        if (offsetPositionSpace == Space.Self)
        {
            lerper += Time.deltaTime;
            tempPos = target.TransformPoint(offsetPosition);
            if (player.CheckCanMove())
            {
               
                // float distance = Vector3.Distance(transform.position, Vector2.zero);

                if (player.CursorsApart() && damping == 1)
                {
                    transform.position = Vector3.Lerp(transform.position, tempPos, lerper*2);
                    Debug.Log("Apart");
                }
                else
                {
                    lerper = 0;
                    transform.position = Vector3.Lerp(transform.position, tempPos, damping);
                    //Debug.Log("Close");
                }
                    
                /*if (damping != 1)
                    transform.position = Vector3.Lerp(transform.position, tempPos, 1 * damping);
                else if (Vector3.Distance(transform.position,tempPos) > 1)
                    transform.position = Vector3.Lerp(transform.position, tempPos, Time.deltaTime);
                else
                    transform.position = Vector3.Lerp(transform.position, tempPos, 1);*/
               
            }
            else if (player.CursorsApart() && damping == 1)
            {
                if(lerper < 0.5f)
                    transform.position = Vector3.Lerp(transform.position, tempPos, lerper);
                else
                    transform.position = tempPos;
            }
                
                //else
                //     transform.position = transform.position;

        }
        else
        {
            transform.position = target.position + offsetPosition;
        }

        // compute rotation
        if (lookAt)
        {
            transform.LookAt(target);
        }
        else
        {
            transform.rotation = target.rotation;
        }

        prevPosition = transform.position;
    }

  
}