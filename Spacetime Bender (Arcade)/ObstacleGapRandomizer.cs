using UnityEngine;

public class ObstacleGapRandomizer : MonoBehaviour
{
    public GameObject leftObstacle;
    public GameObject rightObstacle;
    public float maxOffset = 0.4f;

    void Start()
    {
        // Calculate the random offset
        float offset = Random.Range(0, maxOffset);

        // Apply the offset to the left obstacle
        Vector3 leftPosition = leftObstacle.transform.position;
        leftPosition.x += offset;
        leftObstacle.transform.position = leftPosition;

        // Apply the offset to the right obstacle
        Vector3 rightPosition = rightObstacle.transform.position;
        rightPosition.x -= offset;
        rightObstacle.transform.position = rightPosition;
    }
}
