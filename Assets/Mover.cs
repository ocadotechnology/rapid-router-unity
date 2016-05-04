using UnityEngine;

public class Mover : MonoBehaviour
{

    public float speed = 1.5f;
    float timer = 0;
    private Vector3 targetPosition;

    public AnimationCurve motionCurve;

    Vector3 startPosition;

    // Use this for initialization
    void Start()
    {
        startPosition = transform.position;
        targetPosition = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime / 2;
        float curvedValue = motionCurve.Evaluate(timer);
        Vector3 newPos = Vector3.Lerp(startPosition, targetPosition, curvedValue);
        transform.position = newPos;
    }

}
