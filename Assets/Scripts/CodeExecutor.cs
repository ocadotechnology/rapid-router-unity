using System;
using UnityEngine;
using Zenject;

public class CodeExecutor : MonoBehaviour
{
    VehicleMover vehicleMover;

    void Start()
    {
        GameObject vanController = GameObject.Find("VanController");
        VehicleMover vehicleMover = (VehicleMover)vanController.GetComponent(typeof(VehicleMover));
    }

    public void Run(Code code)
    {
        Google.Protobuf.Collections.RepeatedField<Method> methods = code.Methods;

        Method startMethod = GetStart(methods);

        Call(startMethod);
    }

    private Method GetStart(Google.Protobuf.Collections.RepeatedField<Method> methods)
    {
        foreach (Method method in methods)
        {
            if (method.Name.Equals("Start"))
            {
                return method;
            }
        }
        throw new Exception("No Start method found");
    }

    private void Call(Method method)
    {
        foreach (Instruction instruction in method.Instructions)
        {
            FollowInstruction(instruction);
        }
    }

    private void FollowInstruction(Instruction instruction)
    {
        switch (instruction.Type)
        {
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
                vehicleMover.AddMoveForwardAction();
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
                vehicleMover.AddMoveLeftAction();
                break;
            case Instruction.Types.Type.TurnRight:
                vehicleMover.AddMoveRightAction();
                break;
            case Instruction.Types.Type.Wait:
                // TODO Wait ();
                break;
        }
    }
}

