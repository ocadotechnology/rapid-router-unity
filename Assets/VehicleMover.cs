using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Zenject;

public class VehicleMover : MonoBehaviour
{
    enum Steering {
        Forward, 
        Left,
        Right
    }
    public GameObject van;
    bool vanMoving = false;

    [Inject]
    BoardTranslator translator;

    public GameObject explosion;

    int step = 0;

    // Update is called once per frame
    void Update() {
        if (Input.GetKey("up")) {
            StartForward();
        } else if (Input.GetKey("left")) {
            StartLeft();
        } else if (Input.GetKey("right")) {
            StartRight();
        }
    }

	public void Listener(byte[] protobufInstructions) {
		Code code = Code.Parser.ParseFrom (protobufInstructions);
		Google.Protobuf.Collections.RepeatedField<Method> methods = code.Methods;

		foreach (Method method in methods) {
			Google.Protobuf.Collections.RepeatedField<Instruction> instructions = method.Instructions;
			foreach (Instruction instruction in instructions) {
				FollowInstruction (instruction);
			}
		}
	}

	private void FollowInstruction(Instruction instruction) {
		switch (instruction.Type) {
		case Instruction.Types.Type.Call:
			// TODO Call ();
			break;
		case Instruction.Types.Type.Deliver:
			// TODO Deliver ();
			break;
		case Instruction.Types.Type.IfDo:
			// TODO IfDo ();
			break;
		case Instruction.Types.Type.IfDoElse:
			// TODO IfDoElse ();
			break;
		case Instruction.Types.Type.MoveForwards:
			StartForward ();
			break;
		case Instruction.Types.Type.RepeatTimesDo:
			// TODO RepeatTimesDo ();
			break;
		case Instruction.Types.Type.RepeatUntilDo:
			// TODO RepeatUntilDo ();
			break;
		case Instruction.Types.Type.RepeatWhileDo:
			// TODO RepeatWhileDo ();
			break;
		case Instruction.Types.Type.TurnAround:
			// TODO TurnAround ();
			break;
		case Instruction.Types.Type.TurnLeft:
			StartLeft ();
			break;
		case Instruction.Types.Type.TurnRight:
			StartRight ();
			break;
		case Instruction.Types.Type.Wait:
			// TODO Wait ();
			break;
		}
	}

	public void BlocklyListener(string action) {
		if (string.Equals(action, "move_forwards")) {
			StartForward();
		} else if (string.Equals(action, "turn_left")) {
			StartLeft();
		} else if (string.Equals(action, "turn_right")) {
			StartRight();
		}
	}

    public void StartLeft() {
        if (!vanMoving)
        {
            StartCoroutine(Move(van.transform, 1, Steering.Left));
            step++;
        }
    }

    public void StartRight() {
        if (!vanMoving) {
            StartCoroutine(Move(van.transform, 1, Steering.Right));
            step++;
        }
    }

    public void StartForward() {
        if (!vanMoving) {
            StartCoroutine(Move(van.transform, 1, Steering.Forward));
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
        Vector3 vanPosition = van.transform.position + ForwardABit(van.transform, 0.5f);
        Coordinate vanCoord = new Coordinate(vanPosition);
        if (!BoardManager.roadCoordinates.Contains(vanCoord)) {
            var explosionInstance = Instantiate(explosion, van.transform.position, Quaternion.identity);
            Destroy(explosionInstance, 5f);
            van.GetComponent<SpriteRenderer>().DOColor(Color.black, 4f);
        }
    }

    private void CheckIfAtDestination()
    {
        Vector3 vanPosition = van.transform.position + ForwardABit(van.transform, 0.5f);
        Coordinate vanCoord = new Coordinate(translator.translateToGameVector(vanPosition));
        HashSet<Coordinate> dests = BoardManager.currentLevel.destinationCoords;
        if (dests.Contains(vanCoord)) {
            print("You have reached your destination(s) (in a sat nav voice)");
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
        sequence.Append(transform.DOPath(new Vector3[] { ForwardABit(transform, 0.2f), Deg2LocForLeft(transform.rotation.eulerAngles.z) }, duration, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.InOutQuad).SetRelative());
        sequence.Join(transform.DORotateQuaternion(Quaternion.Euler(0, 0, 90), duration).SetEase(Ease.InOutCubic).SetRelative());
        return sequence;
    }

    private Sequence RightSequence(Transform transform, float duration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOPath(new Vector3[] { ForwardABit(transform, 0.2f), Deg2LocForRight(transform.rotation.eulerAngles.z) }, duration, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.InOutQuad).SetRelative());
        sequence.Join(transform.DORotateQuaternion(Quaternion.Euler(0, 0, -90), duration).SetEase(Ease.InOutCubic).SetRelative());
        return sequence;
    }

    private Sequence ForwardSequence(Transform transform, float duration)
    {
        Sequence sequence = DOTween.Sequence();
        var newDirection = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.z), Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.z), 0);
        sequence.Append(transform.DOMove(newDirection, duration).SetRelative());
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

	public static Vector3 ForwardABit(Transform transform, float scalingFactor)
    {
        var vector = ForwardOne(transform);
        vector.Scale(new Vector3(scalingFactor, scalingFactor, scalingFactor));
        return vector;
    }

	private static Vector3 ForwardOne(Transform transform)
    {
        float degrees = transform.rotation.eulerAngles.z;
        var vector = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * degrees), Mathf.Cos(Mathf.Deg2Rad * degrees));
        return vector;
    }
}
