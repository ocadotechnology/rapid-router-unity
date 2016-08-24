using UnityEngine;
using System.Collections;
using DG.Tweening;
using Zenject;
using Road;
using UnityEngine.UI;

public class VehicleMover : MonoBehaviour
{
    GameObject van;

    bool doingSomething = false;

    Level level;

    [Inject]
    BoardTranslator translator;

    int step = 0;

    void Start()
    {
        van = GameObject.Find("Van");
        this.level = BoardManager.currentLevel;
        van.transform.position = translator.translateVector(level.origin.coords.vector);
        van.transform.Rotate(new Vector3(0, 0, (int)RoadDrawer.StringToDirection(level.origin.direction)));
        van.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        van.transform.position += ForwardABit(van.transform, 0.5f);
        DOTween.defaultEaseOvershootOrAmplitude = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("up") && !doingSomething)
        {
            StartForward();
        }
        else if (Input.GetKey("left") && !doingSomething)
        {
            StartLeft();
        }
        else if (Input.GetKey("right") && !doingSomething)
        {
            StartRight();
        }
    }

    public void StartLeft() {
        StartCoroutine(Left(van.transform, 1));
        step++;
    }

    public void StartRight() {
        StartCoroutine(Right(van.transform, 1));
        step++;
    }

    public void StartForward() {
        StartCoroutine(Forward(van.transform, 1));
        step++;
    }

    private IEnumerator Left(Transform transform, float duration)
    {
        doingSomething = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOPath(new Vector3[] { ForwardABit(transform, 0.2f), Deg2LocForLeft(transform.rotation.eulerAngles.z) }, duration, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.InOutQuad).SetRelative());
        sequence.Join(transform.DORotateQuaternion(Quaternion.Euler(0, 0, 90), duration).SetEase(Ease.InOutCubic).SetRelative());
        sequence.Play();
        yield return new WaitForSeconds(duration);
        doingSomething = false;
    }

    private IEnumerator Right(Transform transform, float duration)
    {
        doingSomething = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOPath(new Vector3[] { ForwardABit(transform, 0.2f), Deg2LocForRight(transform.rotation.eulerAngles.z) }, duration, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.InOutQuad).SetRelative());
        sequence.Join(transform.DORotateQuaternion(Quaternion.Euler(0, 0, -90), duration).SetEase(Ease.InOutCubic).SetRelative());
        sequence.Play();
        yield return new WaitForSeconds(duration);
        doingSomething = false;
    }

    private IEnumerator Forward(Transform transform, float duration)
    {
        doingSomething = true;
        print(transform.rotation.eulerAngles);
        var newDirection = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.z), Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.z), 0);
        transform.DOMove(newDirection, duration).SetRelative();
        yield return new WaitForSeconds(duration);
        doingSomething = false;
    }

    Vector3 Deg2LocForLeft(float degrees)
    {
        Vector3 vector;
        if (degrees == 0)
        {
            vector = new Vector3(-1, 1, 0);
        }
        else if (degrees == 90)
        {
            vector = new Vector3(-1, -1, 0);
        }
        else if (degrees == 180)
        {
            vector = new Vector3(1, -1, 0);
        }
        else if (degrees == 270)
        {
            vector = new Vector3(1, 1, 0);
        }
        else
        {
            vector = new Vector3(0, 0, 0);
        }
        float scalingFactor = 0.5f;
        vector.Scale(new Vector3(scalingFactor, scalingFactor, scalingFactor));
        return vector;
    }

    private Vector3 Deg2LocForRight(float degrees)
    {
        Vector3 vector;
        if (degrees == 0)
        {
            vector = new Vector3(1, 1, 0);
        }
        else if (degrees == 270)
        {
            vector = new Vector3(1, -1, 0);
        }
        else if (degrees == 180)
        {
            vector = new Vector3(-1, -1, 0);
        }
        else if (degrees == 90)
        {
            vector = new Vector3(-1, 1, 0);
        }
        else
        {
            vector = new Vector3(0, 0, 0);
        }
        float scalingFactor = 0.5f;
        vector.Scale(new Vector3(scalingFactor, scalingFactor, scalingFactor));
        return vector;
    }

    private Vector3 ForwardABit(Transform transform, float scalingFactor)
    {
        var vector = ForwardOne(transform);
        vector.Scale(new Vector3(scalingFactor, scalingFactor, scalingFactor));
        return vector;
    }

    private Vector3 ForwardOne(Transform transform)
    {
        float degrees = transform.rotation.eulerAngles.z;
        var vector = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * degrees), Mathf.Cos(Mathf.Deg2Rad * degrees));
        return vector;
    }
}
