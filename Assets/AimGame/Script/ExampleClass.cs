// Prints the name of the object camera is directly looking at
using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    Camera cam;

    public Transform target;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        if(target != null)
        {
            Vector3 targetDirection = (target.position - transform.position);
            Quaternion targetRot = Quaternion.LookRotation(targetDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.35f * Time.deltaTime); 
        }
      
    }

    string message;
    void Update()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
            message = "I'm looking at " + hit.transform.name;
        else
            message = "I'm looking at nothing!";
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(50, 50, 400, 200), message);
    }
}