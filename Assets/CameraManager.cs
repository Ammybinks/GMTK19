using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    AnimationCurve curve;


    Vector2 p0;
    public Vector2 p1;

    float t;

    int fpm = 3;

    float stepDistance;

    bool moving;

    // Start is called before the first frame update
    void Start()
    {
        stepDistance = 1f / fpm;
        Debug.Log(stepDistance);
    }

    // Update is called once per frame
    void Update()
    {
        if(moving)
        {
            Vector2 temp = Vector2.Lerp(p0, p1, curve.Evaluate(t));

            transform.position = new Vector3(temp.x, temp.y, -10);

            if (t >= 1)
            {
                p0 = Vector2.zero;
                p1 = Vector2.zero;

                moving = false;

                t = 0;
            }

            t += stepDistance;
        }
        else if (p1 != Vector2.zero)
        {
            p0 = transform.position;

            moving = true;
        }
    }
}
