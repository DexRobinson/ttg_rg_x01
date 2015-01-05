using UnityEngine;
using System.Collections;

public class GestureHandler : MonoBehaviour {
	public GUIManager guiManager;

	public static float maxDistance;

	public static void InitMaxDistance()
	{
		maxDistance = RecipeManager.allIngridents.Count * 79;
	}

	void OnDrag(DragGesture gesture) 
	{
		// Drag/displacement since last frame
		guiManager.ScrollPosition += gesture.DeltaMove;
		guiManager.targetScrollPositionY += gesture.DeltaMove.y;
	}

	void OnSwipe(SwipeGesture gesture)
	{
        float velocity = -gesture.Velocity;
        float move = gesture.Move.y;
        
        if (gesture.Direction == FingerGestures.SwipeDirection.Up ||
            gesture.Direction == FingerGestures.SwipeDirection.UpperDiagonals ||
            gesture.Direction == FingerGestures.SwipeDirection.UpperLeftDiagonal ||
            gesture.Direction == FingerGestures.SwipeDirection.UpperRightDiagonal ||
            gesture.Direction == FingerGestures.SwipeDirection.Vertical)
        {
            velocity = gesture.Velocity;
            move = gesture.Move.y;
        }

        Debug.Log("Swipe Velocity: " + velocity);
        Debug.Log("Move: " + move);

        float ratio = Mathf.Abs(velocity) / Mathf.Abs(move);
        Debug.Log("Ratio: " + ratio);

        if (Mathf.Abs(velocity) > 1000 && ratio > 5)
        {
            guiManager.swipeVelocity = gesture.Velocity * 0.003f;

            guiManager.targetScrollPositionY = guiManager.ScrollPosition.y + (move + (velocity * 0.3f));
            guiManager.targetScrollPositionY = Mathf.Clamp(guiManager.targetScrollPositionY, 0, maxDistance);
        }
        //guiManager.ScrollPosition = new Vector2(0, guiManager.ScrollPosition.y + gesture.Move.y);
	}
}
