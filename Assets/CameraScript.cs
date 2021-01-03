using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update

    private bool upPressed, downPressed, rightPressed, leftPressed, 
      pageUpPressed, pageDownPressed;
    void Start()
    {
        upPressed = false;
        downPressed = false;
        rightPressed = false;
        leftPressed = false;
        pageUpPressed = false;
        pageDownPressed = false;
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
