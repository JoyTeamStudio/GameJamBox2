using UnityEngine;

public class Door : MonoBehaviour
{
    public float distance;
    public float speed;
    public bool moving;
    private Vector3 target;
    public Vector3 initialPos;

    private void Start()
    {
        initialPos = transform.position;
        target = new Vector3(transform.position.x, transform.position.y - distance, transform.position.z);
    }

    private void Update()
    {
        if (moving)
        {
            transform.Translate(speed * Time.deltaTime * Vector3.down);

            if (Vector3.Distance(transform.position, target) < 0.5f)
            {
                transform.position = target;
                moving = false;
                speed = -speed;

                if (target == initialPos)
                    target = new Vector3(transform.position.x, transform.position.y - distance, transform.position.z);
                else
                    target = initialPos;
            }
        }
    }

    public void TriggerDoor()
    {
        moving = true;
    }
}
