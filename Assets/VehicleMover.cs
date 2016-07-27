using UnityEngine;
using System.Collections;
using DG.Tweening;
using Zenject;
using Road;

public class VehicleMover : MonoBehaviour
{
    GameObject van;

    bool doingSomething = false;

    Level level;

    [Inject]
    BoardTranslator translator;

    int step = 0;

    // Use this for initialization
    void Start()
    {
        van = GameObject.Find("Van");
        this.level = BoardManager.currentLevel;
        van.transform.position =  translator.translateVector(level.origin.coords.vector);
        van.transform.Rotate(new Vector3(0, 0, (int)RoadDrawer.StringToDirection(level.origin.direction)));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("up"))
        {
            if (!doingSomething && step < level.path.Length - 1 && (level.path[step].coords.vector + ForwardOne()) == level.path[step + 1].coords.vector)
            {
                StartCoroutine(Forward());
                step++;
            }
        }
        else if (Input.GetKey("left") && step < level.path.Length - 1 && (level.path[step].coords.vector + Deg2LocForLeft(van.transform.rotation.eulerAngles.z)) == level.path[step].coords.vector)
        {
            if (!doingSomething)
            {
                StartCoroutine(Left());
                step++;
            }
        }
        else if (Input.GetKey("right"))
        {
            if (step < level.path.Length - 1 && (level.path[step].coords.vector + Deg2LocForRight(van.transform.rotation.eulerAngles.z)) == level.path[step].coords.vector)
            {
                if (!doingSomething)
                {
                    StartCoroutine(Right());
                    step++;
                }
            }
        }
    }

    private IEnumerator Left() {
        doingSomething = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(van.transform.DOPath(new Vector3[] { ForwardABit(), Deg2LocForLeft(van.transform.rotation.eulerAngles.z) }, 1, PathType.CatmullRom, PathMode.TopDown2D).SetRelative());
        sequence.Join(van.transform.DORotateQuaternion(Quaternion.Euler(0, 0, 90), 1).SetEase(Ease.OutCubic).SetRelative());
        sequence.Play();
        yield return new WaitForSeconds(1);
        doingSomething = false;
    }

    private IEnumerator Right() {
        doingSomething = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(van.transform.DOPath(new Vector3[] { ForwardABit(), Deg2LocForRight(van.transform.rotation.eulerAngles.z) }, 1, PathType.CatmullRom, PathMode.TopDown2D).SetRelative());
        sequence.Join(van.transform.DORotateQuaternion(Quaternion.Euler(0, 0, -90), 1).SetRelative());
        sequence.Play();
        yield return new WaitForSeconds(1);
        doingSomething = false;
    }

    private IEnumerator Forward() {
        doingSomething = true;
        print(van.transform.rotation.eulerAngles);
        var newDirection = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * van.transform.rotation.eulerAngles.z), Mathf.Cos(Mathf.Deg2Rad * van.transform.rotation.eulerAngles.z), 0);
        van.transform.DOMove(newDirection, 1).SetRelative();
        yield return new WaitForSeconds(1);
        doingSomething = false;
    }

    private void 

    private Vector3 Deg2LocForLeft(float degrees) {
        if (degrees == 0) {
            return new Vector3(-1, 1, 0);
        } else if (degrees == 90) {
            return new Vector3(-1, -1, 0);
        } else if (degrees == 180) {
            return new Vector3(1, -1, 0);
        } else if (degrees == 270) {
            return new Vector3(1, 1, 0);
        } else {
            return new Vector3(0, 0, 0);
        }
    }

    private Vector3 Deg2LocForRight(float degrees) {
        if (degrees == 0) {
            return new Vector3(1, 1, 0);
        } else if (degrees == 270) {
            return new Vector3(1, -1, 0);
        } else if (degrees == 180) {
            return new Vector3(-1, -1, 0);
        } else if (degrees == 90) {
            return new Vector3(-1, 1, 0);
        } else {
            return new Vector3(0, 0, 0);
        }
    }

    private Vector3 ForwardABit() {
        float scalingFactor = 0.5f;
        var vector = ForwardOne();
        vector.Scale(new Vector3(scalingFactor, scalingFactor, scalingFactor));
        return vector;
    }

    private Vector3 ForwardOne() {
        float degrees = van.transform.rotation.eulerAngles.z;
        var vector = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * degrees), Mathf.Cos(Mathf.Deg2Rad * degrees));
        return vector;
    }
}
