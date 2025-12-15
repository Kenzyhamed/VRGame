using UnityEngine;

public class SnapToPoint : MonoBehaviour
{
  
    public Transform[] snapPoints;

   
    public float snapDistance = 0.2f;

    void Update()
    {
        if (snapPoints == null || snapPoints.Length == 0)
            return;

        Vector3 currentPosition = transform.position;

        Transform closestSnapPoint = null;
        float closestDistance = Mathf.Infinity;

        // Find the nearest snap point
        foreach (Transform point in snapPoints)
        {
            if (point == null) continue;

            float distance = Vector3.Distance(currentPosition, point.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSnapPoint = point;
            }
        }

        // Snap only if close enough
        if (closestSnapPoint != null && closestDistance < snapDistance)
        {
            transform.position = closestSnapPoint.position;
        }
    }
}
