using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;
using Zenject;
using UnityEngine.XR.iOS;
// using System.Linq;

public class VehicleMover : MonoBehaviour
{
    enum Steering {
        Forward, 
        Left,
        Right
    }

    bool vanMoving = false;

    [Inject]
    BoardTranslator translator;

    public GameObject explosion;

    public GameObject fireworks1;

    public GameObject fireworks2;
    int step = 0;

    public Queue<IEnumerator> actions = new Queue<IEnumerator>();

    public Dictionary<string, ARPlaneAnchor> anchors = new Dictionary<string, ARPlaneAnchor>();

    private List<Vector3> recentPositions = new List<Vector3>();
    private List<Vector3> recentRotations = new List<Vector3>();

    private List<Vector3> recentCenters = new List<Vector3>();
    public string currentAnchor = "";

    void Start() {
        UnityARSessionNativeInterface.ARAnchorAddedEvent += AddAnchor;
        UnityARSessionNativeInterface.ARAnchorUpdatedEvent += UpdateAnchor;
        UnityARSessionNativeInterface.ARAnchorRemovedEvent += RemoveAnchor;
        StartCoroutine("Process", Process());
    }

    public void AddAnchor(ARPlaneAnchor arPlaneAnchor) {
        anchors.Add(arPlaneAnchor.identifier, arPlaneAnchor);
        if (currentAnchor != "") {
            return;
        }
        UpdateBoardWithAnchor(arPlaneAnchor);
        currentAnchor = arPlaneAnchor.identifier;
        // Debug.Log("Anchor Added");
        // Debug.Log("Plane position: " + arPlaneAnchor.extent);
        // Debug.Log("Plane Anchor extent: " + arPlaneAnchor.extent);
        // var positionAR = UnityARMatrixOps.GetPosition (arPlaneAnchor.transform);
        // Debug.Log("positionAR: " + positionAR);
        // GameObject board = GameObject.Find("Board");
        // Debug.Log("Board position: " + board.transform.position);
        // Bounds maxBounds = GetMaxBounds(board);
        // Debug.Log("Board Extents: " + maxBounds.extents);
        // Debug.Log("Board min: " + maxBounds.min);
        // Debug.Log("Board max: " + maxBounds.max);
        // Debug.Log("Board bounds center: " + maxBounds.center);
        // board.transform.position = new Vector3(positionAR.x - maxBounds.min.x, positionAR.y - 1, positionAR.z + 3);
    }

    public void UpdateBoardWithAnchor(ARPlaneAnchor arPlaneAnchor) {
        recentPositions.Add(UnityARMatrixOps.GetPosition(arPlaneAnchor.transform));
        recentRotations.Add(UnityARMatrixOps.GetRotation(arPlaneAnchor.transform).eulerAngles);
        recentCenters.Add(arPlaneAnchor.center);

        KeepLast(8, recentPositions);
        KeepLast(8, recentRotations);
        KeepLast(8, recentCenters);


        GameObject arHolder = GameObject.Find("ARHolder");
        GameObject board = GameObject.Find("Board");
        // var positionAR = UnityARMatrixOps.GetPosition (arPlaneAnchor.transform);
        // Bounds maxBounds = GetMaxBounds(board);
        // board.transform.position = new Vector3(positionAR.x - maxBounds.min.x, positionAR.y - 1, positionAR.z + 3);
        //do coordinate conversion from ARKit to Unity
        arHolder.transform.position = GetAverage(recentPositions);
        arHolder.transform.rotation = UnityARMatrixOps.GetRotation(arPlaneAnchor.transform); //Quaternion.Euler(GetAverageRotation());

        Vector3 center = GetAverage(recentCenters);
        // MeshFilter mf = plane.GetComponentInChildren<MeshFilter> ();

        // if (mf != null) {
        //since our plane mesh is actually 10mx10m in the world, we scale it here by 0.1f
        var scalingFactor = 0.11f;
        var scale = Mathf.Min(arPlaneAnchor.extent.x, arPlaneAnchor.extent.z);
        board.transform.localScale = new Vector3(scale * scalingFactor, scale * scalingFactor, scale * scalingFactor);

        //convert our center position to unity coords
        board.transform.localPosition = new Vector3(-center.x, center.y, -center.z);
        // }
    }

    private void KeepLast<T>(int n, List<T> list) {
        if (list.Count > n) {
            list.RemoveRange(0, list.Count - n);
        }
    }

    private Vector3 GetAverage(List<Vector3> vectors) {
        Vector3 average = new Vector3(0, 0, 0);
        foreach (Vector3 vector in vectors)
        {
            average += vector;
        }
        return average / (float)vectors.Count;
    }

    public void UpdateAnchor(ARPlaneAnchor arPlaneAnchor) {
        if (anchors.ContainsKey (arPlaneAnchor.identifier)) {
				ARPlaneAnchor arpag = anchors [arPlaneAnchor.identifier];
				// UnityARUtility.UpdatePlaneWithAnchorTransform (arpag.gameObject, arPlaneAnchor);
				// arpag.planeAnchor = arPlaneAnchor;
				anchors [arPlaneAnchor.identifier] = arPlaneAnchor;
            if (currentAnchor == arPlaneAnchor.identifier) {
                UpdateBoardWithAnchor(arPlaneAnchor);
            }
		}
    }

    public void RemoveAnchor(ARPlaneAnchor arPlaneAnchor) {
        if (anchors.ContainsKey (arPlaneAnchor.identifier)) {
				ARPlaneAnchor arpag = anchors [arPlaneAnchor.identifier];
				anchors.Remove (arPlaneAnchor.identifier);
                if (currentAnchor == arPlaneAnchor.identifier) {
                    recentPositions.Clear();
                    recentRotations.Clear();
                    recentCenters.Clear();
                if (anchors.Keys.Count > 0) {
                        var e = anchors.GetEnumerator();
                        e.MoveNext();
                        var newCurrentAnchor = e.Current.Value;
                        currentAnchor = newCurrentAnchor.identifier;
                        UpdateBoardWithAnchor(newCurrentAnchor);
                    } else {
                        currentAnchor = "";
                    }
                }
		}
    }

    Bounds GetMaxBounds(GameObject g) {
        var b = new Bounds(g.transform.position, Vector3.zero);
        foreach (Renderer r in g.GetComponentsInChildren<Renderer>()) {
            b.Encapsulate(r.bounds);
        }
        return b;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("up")) {
            StartForward();
        } else if (Input.GetKeyDown("left")) {
            StartLeft();
        } else if (Input.GetKeyDown("right")) {
            StartRight();
        }
    }

    public IEnumerator Process() {
        while (true) {
            if (actions.Count > 0) {
                yield return StartCoroutine(actions.Dequeue());
            } else {
                yield return null;
            }
        }
    }

    public void AddMoveLeftAction() {
        actions.Enqueue(Move(transform, 1, Steering.Left));
    }

    public void AddMoveRightAction() {
        actions.Enqueue(Move(transform, 1, Steering.Right));
    }

    public void AddMoveForwardAction() {
        actions.Enqueue(Move(transform, 1, Steering.Forward));
    }

    public void StartLeft() {
        if (!vanMoving)
        {
            StartCoroutine(Move(transform, 1, Steering.Left));
            step++;
        }
    }

    public void StartRight() {
        if (!vanMoving) {
            StartCoroutine(Move(transform, 1, Steering.Right));
            step++;
        }
    }

    public void StartForward() {
        if (!vanMoving) {
            StartCoroutine(Move(transform, 1, Steering.Forward));
            step++;
        }
    }

    private IEnumerator Move(Transform transform, float duration, Steering direction) {
        vanMoving = true;
        Sequence sequence = GetSequenceForDirection(transform, duration, direction);
        sequence.Play();
        yield return new WaitForSeconds(duration);
        CheckIfOffRoading(direction);
        CheckIfAtDestination();
        vanMoving = false;
    }

    private void CheckIfOffRoading(Steering direction) {
        Vector3 vanPosition = transform.localPosition + ForwardABit(transform, 0.5f);
        Coordinate vanCoord = new Coordinate(vanPosition);
        if (!BoardManager.roadCoordinates.Contains(vanCoord)) {
            // var explosionInstance = Instantiate(explosion, transform.position, Quaternion.identity);
            // Destroy(explosionInstance, 5f);
            // GetComponent<SpriteRenderer>().DOColor(Color.black, 4f);
        }
    }

    private void CheckIfAtDestination()
    {
        Vector3 vanPosition = transform.localPosition + ForwardABit(transform, 0.5f);
        Coordinate vanCoord = new Coordinate(translator.translateToGameVector(vanPosition));
        HashSet<Coordinate> dests = BoardManager.currentLevel.destinationCoords;
        if (dests.Contains(vanCoord)) {
            print("You have reached your destination(s) (in a sat nav voice)");
            GameObject f1 = Instantiate(fireworks1, transform.position, transform.rotation);
            GameObject f2 = Instantiate(fireworks2, transform.position, transform.rotation);
            f1.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            f2.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
    }

    private Sequence GetSequenceForDirection(Transform transform, float duration, Steering direction)
    {
        switch (direction)
        {
            case Steering.Forward:
                return ForwardSequence(transform, duration);
            case Steering.Left:
                return LeftSequence(transform, duration);
            case Steering.Right:
                return RightSequence(transform, duration);
            default:
                return DOTween.Sequence();
        }
    }

    private Sequence LeftSequence(Transform transform, float duration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(transform.DOLocalPath(new Vector3[] { ForwardABit(transform, 0.2f), Deg2LocForLeft((float)Math.Round(transform.localEulerAngles.y, 0)) }, duration, PathType.CatmullRom, PathMode.TopDown2D)
            .SetEase(Ease.InOutQuad)
            .SetRelative());
        sequence
            .Join(transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 90), duration)
            .SetEase(Ease.InOutCubic)
            .SetRelative());
        return sequence;
    }

    private Sequence RightSequence(Transform transform, float duration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(transform.DOLocalPath(new Vector3[] { ForwardABit(transform, 0.2f), Deg2LocForRight((float)Math.Round(transform.localEulerAngles.y, 0)) }, duration, PathType.CatmullRom, PathMode.TopDown2D)
            .SetEase(Ease.InOutQuad)
            .SetRelative());
        sequence
            .Join(transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -90), duration)
            .SetEase(Ease.InOutCubic)
            .SetRelative());
        return sequence;
    }

    private Sequence ForwardSequence(Transform transform, float duration)
    {
        Sequence sequence = DOTween.Sequence();
        // var newDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * transform.localEulerAngles.z), 0, -Mathf.Sin(Mathf.Deg2Rad * transform.localEulerAngles.z));
        sequence.Append(transform.DOLocalMove(ForwardOne(transform), duration).SetRelative());
        return sequence;
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
            vector = new Vector3(1, 1, 0);
        }
        else if (degrees == 180)
        {
            vector = new Vector3(1, -1, 0);
        }
        else if (degrees == 270)
        {
            vector = new Vector3(-1, -1, 0);
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
            vector = new Vector3(-1, 1, 0);
        }
        else if (degrees == 180)
        {
            vector = new Vector3(-1, -1, 0);
        }
        else if (degrees == 90)
        {
            vector = new Vector3(1, -1, 0);
        }
        else
        {
            vector = new Vector3(0, 0, 0);
        }
        float scalingFactor = 0.5f;
        vector.Scale(new Vector3(scalingFactor, scalingFactor, scalingFactor));
        return vector;
    }

	public static Vector3 ForwardABit(Transform transform, float scalingFactor)
    {
        var vector = ForwardOne(transform);
        vector.Scale(new Vector3(scalingFactor, scalingFactor, scalingFactor));
        return vector;
    }

	private static Vector3 ForwardOne(Transform transform)
    {
        float degrees = transform.localEulerAngles.y;
        var vector = new Vector3(Mathf.Sin(Mathf.Deg2Rad * degrees), Mathf.Cos(Mathf.Deg2Rad * degrees));
        return vector;
    }

    public void SolveLevel13() {
        AddMoveLeftAction();
        AddMoveRightAction();
        AddMoveForwardAction();
        AddMoveLeftAction();
        AddMoveRightAction();
        AddMoveForwardAction();
        AddMoveForwardAction();
        AddMoveRightAction();
        AddMoveForwardAction();
        AddMoveForwardAction();
        AddMoveForwardAction();
    }
}
