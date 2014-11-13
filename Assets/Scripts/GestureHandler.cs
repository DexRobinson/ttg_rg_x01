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
		//Debug.Log (gesture.Velocity);
		//guiManager.swipeVelocity = gesture.Velocity * 0.003f;

		//guiManager.targetScrollPositionY = guiManager.ScrollPosition.y + (gesture.Move.y + (gesture.Velocity * 0.3f));

		//guiManager.ScrollPosition = new Vector2(0, guiManager.ScrollPosition.y + gesture.Move.y);
	}
}
