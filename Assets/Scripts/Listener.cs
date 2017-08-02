using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zenject;

public class Listener : MonoBehaviour {

	[Inject]
	VehicleMover vehicleMover;

	public void Listen(string protobufInstructionsString) {
		byte[] protobufInstructions = Encoding.Default.GetBytes (protobufInstructionsString);
		Code code = Code.Parser.ParseFrom (protobufInstructions);
		Google.Protobuf.Collections.RepeatedField<Method> methods = code.Methods;

		Method startMethod = GetStart (methods);

		Google.Protobuf.Collections.RepeatedField<Instruction> instructions = startMethod.Instructions;
		foreach (Instruction instruction in instructions) {
			FollowInstruction (instruction);
		}
	}

	private Method GetStart(Google.Protobuf.Collections.RepeatedField<Method> methods) {
		foreach (Method method in methods) {
			if (method.Name.Equals("Start")) {
				return method;
			}
		}
		throw new System.Exception ("No Start method found");
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
			vehicleMover.StartForward ();
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
			vehicleMover.StartLeft ();
			break;
		case Instruction.Types.Type.TurnRight:
			vehicleMover.StartRight ();
			break;
		case Instruction.Types.Type.Wait:
			// TODO Wait ();
			break;
		}
	}
}
