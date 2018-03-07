using System;
using UnityEngine;
using Zenject;

public class CodeExecutor
{

    VehicleMover vehicleMover;

    public CodeExecutor()
    {
        vehicleMover = GameObject.Find("VanController").GetComponent<VehicleMover>();
    }

    public CodeExecutor(VehicleMover vehicleMover)
    {
        this.vehicleMover = vehicleMover;
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
            if (method.Name.Equals("start"))
            {
                return method;
            }
        }
        throw new Exception("No Start method found");
    }

    private void Call(Method method)
    {
        var instructions = method.Instructions;
        if (instructions != null)
        {
            foreach (Instruction instruction in method.Instructions)
            {
                FollowInstruction(instruction);
            }
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
                vehicleMover.StartForward();
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
                //vehicleMover.StartLeft ();
                break;
            case Instruction.Types.Type.TurnRight:
                //vehicleMover.StartRight ();
                break;
            case Instruction.Types.Type.Wait:
                // TODO Wait ();
                break;
        }
    }
}

