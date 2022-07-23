using UnityEngine;

public class TouchHand : MonoBehaviour
{
    public void MoveTouchHand(float posX, float posY)
    {
        Vector2 newPosition = new Vector2(transform.position.x + posX, transform.position.y + posY);
        this.transform.position = newPosition;
    }
}
