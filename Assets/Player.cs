using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    enum MoveKey
    {
        up,
        down,
        left,
        right,
        none
    }

    public bool gameActive = false;

    [SerializeField]
    Manager manager;
    [SerializeField]
    CameraManager camera;
    [SerializeField]
    AnimationCurve curve;
    [SerializeField]
    LayerMask hitMask;

    RaycastHit2D hit;

    bool moving;
    bool rotating;

    Vector2 p0;
    Vector2 p1;
    Vector2 p2;

    float totalRotation;

    float pt;
    float rt;

    int fpp = 4;
    int fpr = 5;

    float pStepDistance;
    float rStepDistance;

    float rotationAmount;
    
    float timer;

    MoveKey currentKey;
    
    bool moveNow;

    int numWins;

    // Start is called before the first frame update
    void Start()
    {
        numWins = 0;

        moving = false;
        rotating = false;

        pStepDistance = 1f / fpp;
        Debug.Log(pStepDistance);

        rStepDistance = 1f / fpr;
        Debug.Log(rStepDistance);

        currentKey = MoveKey.none;
        
        moveNow = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameActive)
        {
            if (moving)
            {
                Move();
            }
            if (rotating)
            {
                Rotate();
            }
        }
        else
        {
            moving = false;
            rotating = false;
        }

        if (Input.GetKeyDown(manager.keys["up"]))
        {
            currentKey = MoveKey.up;

            if (!moving)
            {
                moveNow = true;
            }
        }
        else if (Input.GetKeyDown(manager.keys["down"]))
        {
            currentKey = MoveKey.down;

            if (!moving)
            {
                moveNow = true;
            }
        }
        else if (Input.GetKeyDown(manager.keys["left"]))
        {
            currentKey = MoveKey.left;

            if (!moving)
            {
                moveNow = true;
            }
        }
        else if (Input.GetKeyDown(manager.keys["right"]))
        {
            currentKey = MoveKey.right;

            if (!moving)
            {
                moveNow = true;
            }
        }
        else if (!Input.anyKey)
        {
            currentKey = MoveKey.none;
        }
        
        if(gameActive)
        {
            if (!moving && moveNow && currentKey != MoveKey.none)
            {
                if (currentKey == MoveKey.up)
                {
                    StartMove(Vector2.up);
                }
                if (currentKey == MoveKey.down)
                {
                    StartMove(Vector2.down);
                }
                if (currentKey == MoveKey.left)
                {
                    StartMove(Vector2.left);
                }
                if (currentKey == MoveKey.right)
                {
                    StartMove(Vector2.right);
                }

                currentKey = MoveKey.none;
                moveNow = false;
            }

            timer += Time.deltaTime;
        }
    }
    
    private void StartMove(Vector2 direction)
    {
        rStepDistance = 1f / fpr;

        //Raycast 1
        hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, hitMask);
        Debug.DrawRay(transform.position, direction, Color.red);
        if (hit)
        {
            transform.position = new Vector3(hit.point.x, hit.point.y, transform.position.z);

            Vector2 temp = new Vector2(transform.localScale.x * 0.75f, transform.localScale.y * 0.75f);

            temp *= direction;

            transform.position = new Vector2(transform.position.x - (temp.x * 0.01f), transform.position.y - (temp.y * 0.01f));

            p0 = transform.position;
            p1 = transform.position;

            temp = (hit.point - (Vector2)transform.position).normalized;

            if (temp.x > 0 || temp.y > 0)
            {
                p2 = hit.point - new Vector2(Mathf.Abs(temp.x), Mathf.Abs(temp.y));
            }
            else
            {
                p2 = hit.point + new Vector2(Mathf.Abs(temp.x), Mathf.Abs(temp.y));
            }

            p1 = (p2 - p0) / 2;

            float tempDir = Random.Range(0, 2) * 2 - 1;

            if (p1.x == 0)
            {
                p1.x = p0.x + tempDir * 0.25f;
                p1.y += p0.y;

            }
            else
            {
                p1.y = p0.y + tempDir * 0.25f;
                p1.x += p0.x;
            }

            camera.p1 = new Vector3(p2.x, p2.y, -10);

            pt = 0;
            rt = 0;

            rotationAmount = 90;

            Debug.Log("Time:" + timer);

            timer = 0;

            moving = true;
            rotating = true;

            if (hit.transform.gameObject.layer == 9)
            {
                Destroy(hit.transform.gameObject);

                manager.Win();

                rotationAmount = 1000;

                rStepDistance = 1f / 150;

                rotating = true;

                numWins++;

                if(numWins == 3)
                {
                    manager.finalWin = true;
                }
            }
            else
            {
                GetComponent<AudioSource>().Play();
            }
        }
    }

    private void Move()
    {
        transform.position = Vector2.Lerp(Vector2.Lerp(p0, p1, curve.Evaluate(pt)), Vector2.Lerp(p1, p2, curve.Evaluate(pt)), curve.Evaluate(pt));

        if (pt >= 1)
        {
            moving = false;
        }

        pt += pStepDistance;
    }

    private void Rotate()
    {
        float rotation = Mathf.Lerp(0, rotationAmount, curve.Evaluate(1 - rt));

        transform.Rotate(0, 0, rotation - totalRotation);

        totalRotation = rotation;

        if (rt >= 1)
        {
            transform.eulerAngles = new Vector3(0, 0, 45);

            rotating = false;

            moveNow = true;
        }

        rt += rStepDistance;
    }
}
