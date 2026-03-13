using UnityEngine;

public class IdleObjectMovement : MonoBehaviour
{
    public Vector3 moveAxis;

    public Vector3 moveAxisLength;
    public Vector3 moveAxisSpeed;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        float xPos = originalPosition.x + (CalculateSin(moveAxisSpeed.x, moveAxisLength.x) * moveAxis.x);
        float yPos = originalPosition.y + (CalculateSin(moveAxisSpeed.y, moveAxisLength.y) * moveAxis.y);
        float zPos = originalPosition.z + (CalculateSin(moveAxisSpeed.z, moveAxisLength.z) * moveAxis.z);

        transform.position = new Vector3(xPos, yPos, zPos);
    }

    private float CalculateSin(float speed, float length)
    {
        return Mathf.Sin(Time.time * speed) * length;
    }
}
