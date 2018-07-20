using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField]
    protected bool canHit = false;
    [SerializeField]
    private Animator myAnimator;
   
    public GameObject sphere;
    public GameObject blocker;
    [SerializeField]
    protected Material[] colorChange;
    private Renderer myRenderer;
    public bool CanHit { get { return canHit; }  set {canHit = value; } }
    public bool isValid = true;
    public bool isblocked = false;
    public TargetBlock myBlock;
    private float speed = 0.5f;
    // Use this for initialization
    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        speed      = 0.25f;
    }
    void Start ()
    {
        myBlock = new TargetBlock(this);
        startPos = transform.position;
    }

    private Vector3 startPos;
    private void OnEnable()
    {
        if(sphere == null)
            sphere = transform.Find("Sphere").gameObject;

        myRenderer = sphere.GetComponent<Renderer>();
        myRenderer.material = colorChange[0];

     
    }

    Rect sRect,realRect;
    public Vector2 center;
    public float radius;
    private Vector3 curPos;
   
    // Update is called once per frame
    void Update ()
    {

        curPos = transform.position;
        if(radius > 0 && CanHit)
        {
            float dist = Vector3.Distance(startPos, curPos) ;
            if (dist > radius)
            {
                transform.Rotate(Vector3.forward, Random.Range(-45,45));
                speed *= -1;
            }

            transform.Translate(Vector3.up * Time.deltaTime * speed);
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        BlockHit((int)name[8]);

    }

    public void OnHit()
    {
        BlockHit((int)name[8]);
    }
       
    public Vector2 GetScreenPos()
    {
        Rect temp = ScreenRect();

        return new Vector2(temp.x, temp.y);
    }

    public Rect GetScreenRect()
    {
        return ScreenRect();
    }

    public void TargetSpawn()
    {
        CanHit = true;
        if(myAnimator !=null)
            myAnimator.SetBool("up", true);
        myRenderer.material = colorChange[1];
    }

    public void TargetRest()
    {
        CanHit = false;
        if (myAnimator != null)
            myAnimator.SetBool("up", false);
        myRenderer.material = colorChange[0];
    }

    public bool IsCamVisible()
    {
        if (!myRenderer)
            return false;

        return myRenderer.isVisible;
    }

    public void CopyValues(Target t)
    {
       
        transform.localScale = t.transform.localScale;
        transform.rotation   = t.transform.rotation;
        transform.position   = t.transform.position;
        isValid              = t.isValid;
        radius               = t.radius;

    }

    public void BlockHit(int inValue)
    {
        if(CanHit)
        {
            TargetRest();
            EventManager.CallOnTargetHit();
           
        }
        
    }

    public bool IsHighlighted { get { return canHit; } }

    public Vector3 Getposition()
    {
        return transform.position;
    }

    public float margin = 0;
    private Vector3[] pts = new Vector3[8];
    public Rect ScreenRect()
    {

        if (!gameObject.activeSelf)
            return new Rect();

        SphereCollider sC = GetComponent<SphereCollider>();
        Bounds b = sC.bounds;
        Camera cam = Camera.main;

        //The object is behind us
        if (!IsCamVisible())
            return Rect.zero;

        //All 8 vertices of the bounds
        pts[0] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
        pts[1] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
        pts[2] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
        pts[3] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
        pts[4] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
        pts[5] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
        pts[6] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
        pts[7] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));

        //Get them in GUI space
        //for (int i = 0; i < pts.Length; i++)
        //    pts[i].y = Screen.height - pts[i].y;

        //Calculate the min and max positions
        Vector3 min = pts[0];
        Vector3 max = pts[0];
        for (int i = 1; i < pts.Length; i++)
        {
            min = Vector3.Min(min, pts[i]);
            max = Vector3.Max(max, pts[i]);
        }

        //Construct a rect of the min and max positions and apply some margin
        Rect r = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        r.xMin -= margin;
        r.xMax += margin;
        r.yMin -= margin;
        r.yMax += margin;

        r.x = r.x + r.width  / 2f;
        r.y = r.y + r.height / 2f;

        return r;
    }

    public Rect RealRect()
    {
        realRect = ScreenRect();
        realRect.x = realRect.x - realRect.width / 2f; realRect.y = realRect.y - realRect.height / 2f;
        return realRect;
    }

    float wh = 0;
    public float ComputeGravity(Vector2 cursPt)
    {
        if (sRect.width * sRect.height > 5000)
            wh = 6000;
        else
            wh = 5000;

        // wh = sRect.width * sRect.height;
        sRect = ScreenRect();
        center = new Vector2(sRect.x - Screen.width / 2f, sRect.y - Screen.height / 2f);

        float distance = (center - cursPt).magnitude;
        double multiplier = (wh) / (Mathf.Pow(distance, 2) + 1.0f); //(sRect.width * sRect.height)
        //Logging.LogDebug("Multiplier to {0}: {1}", name, multiplier);
        return (float)multiplier;
    }
}
