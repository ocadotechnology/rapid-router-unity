using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zenject;

public class Listener : MonoBehaviour {

	[Inject]
	CodeExecutor codeExecutor;

	public void Listen(string protobufInstructionsString) {
		byte[] protobufInstructions = Encoding.Default.GetBytes (protobufInstructionsString);
		Code code = Code.Parser.ParseFrom (protobufInstructions);

		codeExecutor.Run (code);
	}
}
