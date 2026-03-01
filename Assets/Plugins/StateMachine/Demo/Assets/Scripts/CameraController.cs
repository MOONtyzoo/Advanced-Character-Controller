using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float desiredHeight;
    [SerializeField] private float trackingSpeed;

    private void FixedUpdate()
    {
        float goalX = target.transform.position.x;
        Vector3 currentPosition = transform.position;

        Vector3 nextPosition;
        nextPosition.x = Mathf.Lerp(currentPosition.x, goalX, trackingSpeed * Time.fixedDeltaTime);
        nextPosition.y = desiredHeight;
        nextPosition.z = currentPosition.z;

        transform.position = nextPosition;
    }
}
