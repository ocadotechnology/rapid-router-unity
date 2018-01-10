using UnityEngine;
using Google.Protobuf;
using System;

public class Listener : MonoBehaviour
{

    public CodeExecutor codeExecutor;

    public void Awake()
    {
        codeExecutor = new CodeExecutor();
    }

    public void Listen(string protobufInstructionsString)
    {
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        Code code = JsonParser.Default.Parse<Code>(protobufInstructionsString);

        codeExecutor.Run(code);
    }
}
