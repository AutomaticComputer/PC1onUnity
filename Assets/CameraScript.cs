using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update

    private bool upPressed, downPressed, rightPressed, leftPressed, 
      pageUpPressed, pageDownPressed;

    private Hashtable lastTouches; 
    void Start()
    {
        upPressed = false;
        downPressed = false;
        rightPressed = false;
        leftPressed = false;
        pageUpPressed = false;
        pageDownPressed = false;

        lastTouches = new Hashtable();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = 0.0f; 
        float verticalInput = 0.0f; 
        float depthInput = 0.0f;

        if (Input.GetKey(KeyCode.UpArrow) || upPressed)
            verticalInput = 1.0f;
        if (Input.GetKey(KeyCode.DownArrow) || downPressed)
            verticalInput = -1.0f;
        if (Input.GetKey(KeyCode.RightArrow) || rightPressed)
            horizontalInput = 1.0f;
        if (Input.GetKey(KeyCode.LeftArrow) || leftPressed)
            horizontalInput = -1.0f;
        if (Input.GetKey(KeyCode.PageUp) || pageUpPressed)
            depthInput = 1.0f;
        if (Input.GetKey(KeyCode.PageDown) || pageDownPressed)
            depthInput = -1.0f;

        transform.Translate(new Vector3(horizontalInput, verticalInput, depthInput) * Time.deltaTime * 0.2f);

        Hashtable touches = new Hashtable();
        bool moved = false;
        for(int i = 0; i < Input.touchCount; i++)
        {   
            Touch touch;
            touch = Input.GetTouch(i);
            switch(touch.phase)
            {
                case TouchPhase.Began: 
                    touches.Add(touch.fingerId, touch.position);
                    break;
                case TouchPhase.Moved: 
                    touches.Add(touch.fingerId, touch.position);
                    moved = true;
                    break;
                case TouchPhase.Ended: 
                case TouchPhase.Canceled: 
                    lastTouches.Remove(touch.fingerId);
                    break;
            }
        }
        if (moved)
        {
            if (touches.Count == 1) 
            {
                foreach (int fingerId in touches.Keys)
                {
                    if (!lastTouches.ContainsKey(fingerId))
                        break;
                    Vector2 lastPos = (Vector2) lastTouches[fingerId];
                    Vector2 pos = (Vector2) touches[fingerId];
                    Vector3 lastPos3, pos3;
                    float x, y;
                    lastPos3 = Camera.main.ScreenToWorldPoint(
                        new Vector3(lastPos.x, lastPos.y, -transform.position.z));
                    pos3 = Camera.main.ScreenToWorldPoint(
                    new Vector3(pos.x, pos.y, -transform.position.z));
                    x = Math.Min(lastPos3.x - pos3.x, 100.0f);
                    x = Math.Max(x, -100.0f);
                    y = Math.Min(lastPos3.y - pos3.y, 100.0f);
                    y = Math.Max(y, -100.0f);
                    transform.Translate(new Vector3(x, y, 0));
                }
            }
            if (touches.Count == 2) 
            {
                int i;
                Vector2[] lastPos, pos;
                lastPos = new Vector2[2];
                pos = new Vector2[2];
                i = 0;
                foreach (int fingerId in touches.Keys)
                {
                    if (!lastTouches.ContainsKey(fingerId))
                        break;
                    lastPos[i] = (Vector2) lastTouches[fingerId];
                    pos[i] = (Vector2) touches[fingerId];
                    i++;
                }
                if (i == 2) 
                {
                    float mag = Vector2.Distance(pos[0], pos[1])/Vector2.Distance(lastPos[0], lastPos[1]);
                    mag = Math.Min(mag, 1.25f);
                    mag = Math.Max(mag, 0.8f);
                    transform.Translate(
                        new Vector3(0, 0, transform.position.z*(1.0f/mag-1.0f)));
                }
            }
        }
        foreach (int fingerId in touches.Keys)
        {
            lastTouches[fingerId] = touches[fingerId];
        }
    }

    public void UpButtonDown() 
    {
        upPressed = true;
    }
    public void UpButtonUp() 
    {
        upPressed = false;
    }
    public void DownButtonDown() 
    {
        downPressed = true;
    }
    public void DownButtonUp() 
    {
        downPressed = false;
    }
    public void RightButtonDown() 
    {
        rightPressed = true;
    }
    public void RightButtonUp() 
    {
        rightPressed = false;
    }
    public void LeftButtonDown() 
    {
        leftPressed = true;
    }
    public void LeftButtonUp() 
    {
        leftPressed = false;
    }
    public void PageUpButtonDown() 
    {
        pageUpPressed = true;
    }
    public void PageUpButtonUp() 
    {
        pageUpPressed = false;
    }
    public void PageDownButtonDown() 
    {
        pageDownPressed = true;
    }
    public void PageDownButtonUp() 
    {
        pageDownPressed = false;
    }

}
