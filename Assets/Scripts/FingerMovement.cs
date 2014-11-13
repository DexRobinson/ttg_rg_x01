using UnityEngine;
using System.Collections;

public class FingerMovement : MonoBehaviour {
    private bool mouseDown;
    private Vector2 lastPosition = new Vector2();
    private float velocity = 0f;
    private float startTime = 0f;
    private float direction = 0f;
    private float startPosition = 0f;
    private float moveAmount = 0f;
    private bool updateMovement;

	void Update ()
    {
        #region MouseInput
        if (Application.isEditor || Application.isWebPlayer)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                updateMovement = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                mouseDown = true;
                updateMovement = false;
                startTime = Time.time;
                startPosition = Input.mousePosition.y;
                lastPosition = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                mouseDown = false;
                updateMovement = true;
                direction = Input.mousePosition.y - startPosition;
                velocity = direction / (Time.time - startTime);

                if (Mathf.Abs(velocity) > Mathf.Abs(direction) + 400)
                    moveAmount = GUIManager.instance.ScrollPosition.y + velocity * 0.1f;
                else
                    moveAmount = GUIManager.instance.ScrollPosition.y;

                //Debug.Log("Direction: " + direction + ", Velocity: " + velocity + ", Move Amt: " +  moveAmount);
            }

            if (mouseDown)
            {
                GUIManager.instance.ScrollPosition -= lastPosition - (Vector2)Input.mousePosition;
                lastPosition = Input.mousePosition;
            }
            else
            {
                if(updateMovement)
                    GUIManager.instance.ScrollPosition = new Vector2(0, Mathf.Lerp(GUIManager.instance.ScrollPosition.y, moveAmount, Time.deltaTime * 3.0f));

                if(Vector2.Distance(GUIManager.instance.ScrollPosition, new Vector2(GUIManager.instance.ScrollPosition.x, moveAmount)) < 0.01f)
                {
                    updateMovement = false;
                }
            }
        }
        #endregion
        #region Touch Input
        else
        {
            Touch touch = new Touch();

            if (Input.touchCount > 0)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    mouseDown = true;
                    startTime = Time.time;
                    startPosition = touch.position.y;
                    lastPosition = touch.position;
                    updateMovement = false;
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    mouseDown = false;
                    updateMovement = true;
                    direction = touch.position.y - startPosition;
                    velocity = direction / (Time.time - startTime);

                    if (Mathf.Abs(velocity) > Mathf.Abs(direction) + 400)
                        moveAmount = GUIManager.instance.ScrollPosition.y + velocity * 0.1f;
                    else
                        moveAmount = GUIManager.instance.ScrollPosition.y;

                    //Debug.Log("Direction: " + direction + ", Velocity: " + velocity + ", Move Amt: " +  moveAmount);
                }
            }

            if (mouseDown)
            {
                GUIManager.instance.ScrollPosition -= lastPosition - (Vector2)Input.mousePosition;

                lastPosition = touch.position;
            }
            else
            {

                if(updateMovement)
                    GUIManager.instance.ScrollPosition = new Vector2(0, Mathf.Lerp(GUIManager.instance.ScrollPosition.y, moveAmount, Time.deltaTime * 3.0f));

                if (Vector2.Distance(GUIManager.instance.ScrollPosition, new Vector2(GUIManager.instance.ScrollPosition.x, moveAmount)) < 0.01f)
                {
                    updateMovement = false;
                }
            }
        }
        #endregion
    }
        
}
