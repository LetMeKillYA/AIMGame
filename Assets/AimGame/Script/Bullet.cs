using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    protected Rigidbody myRigidbody;
    [SerializeField]
    protected Transform parentTransform;

    private Transform myTransform = null;
    private TrailRenderer myTrail = null;

    private void Awake()
    {
        if (myTransform == null)
            myTransform = transform;
        if (myTrail == null)
            myTrail = GetComponent<TrailRenderer>();
    }

    // Use this for initialization
    void Start ()
    {
       

    }

    private void OnEnable()
    {
        
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
        ready = true;

    }

    bool ready = false;
    private void FixedUpdate()
    {
       
    }

    // Update is called once per frame
    void Update ()
    {
        if (ready)
        {
            myRigidbody.velocity = myTransform.forward * 10f;
            ready = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       /* GameObject collidingObj = collision.gameObject;

        if(collidingObj.tag == "Border" || collidingObj.tag == "Target")
        {
            myTransform.rotation = Quaternion.Euler(Vector3.zero);
            myRigidbody.velocity = Vector3.zero;
            gameObject.SetActive(false);
            myTransform.parent = parentTransform;
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject collidingObj = other.gameObject;

        if (collidingObj.tag == "Border" || collidingObj.tag == "Target")
        {
            myTransform.rotation = Quaternion.Euler(Vector3.zero);
            myRigidbody.velocity = Vector3.zero;
            myRigidbody.angularVelocity = Vector3.zero;
            myTrail.Clear();
            gameObject.SetActive(false);
            myTransform.parent = parentTransform;
        }
    }
}
