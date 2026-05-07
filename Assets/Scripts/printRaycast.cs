using UnityEngine;
using UnityEngine.InputSystem;

public class printRaycast : MonoBehaviour
{
    public Transform rayOrigin;
    public LayerMask hitMask;
    public float rayDistance = 80f;
    LineRenderer debugLine;
    void Start()
    {
        debugLine = gameObject.AddComponent<LineRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if(Input.GetButtonDown("Submit") && Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit,rayDistance, hitMask))
        {
            print(hit.normal.normalized);
            Vector3 ned = hit.normal;
            ned *= 10;


            debugLine.startColor = Color.red;
            debugLine.endColor = Color.green;

            debugLine.startWidth = .6f;
            debugLine.endWidth = .2f;

            debugLine.positionCount = 2;

            debugLine.SetPosition(0, hit.point);
            debugLine.SetPosition(1,ned);

        }
    }
}
