using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Zenject;

public class DeviceCommunicator : MonoBehaviour {

	[Inject]
	CodeExecutor codeExecutor;

	public void Listen(string protobufInstructionsString) 
	{
		byte[] protobufInstructions = Encoding.Default.GetBytes (protobufInstructionsString);
		Code code = Code.Parser.ParseFrom (protobufInstructions);

		codeExecutor.Run (code);
	}

	[DllImport ("__Internal")]
	private extern static void endLevel(LevelCompleteStatus status);

	public void EndLevel(LevelCompleteStatus status) 
	{
		endLevel(status);
	}
}
