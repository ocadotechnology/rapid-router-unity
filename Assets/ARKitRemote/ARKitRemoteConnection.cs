using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using System.Text;
using Utils; 

#if UNITY_EDITOR
 
using UnityEditor.Networking.PlayerConnection;

namespace UnityEngine.XR.iOS
{
	public class ARKitRemoteConnection : MonoBehaviour
	{
		EditorConnection editorConnection ;

		int currentPlayerID = -1;
		string guimessage = "none";

		Texture2D remoteScreenYTex;
		Texture2D remoteScreenUVTex;

		bool bTexturesInitialized;

		// Use this for initialization
		void Start () {

			bTexturesInitialized = false;
			//put some defaults so that it doesnt complain
			UnityARCamera scamera = new UnityARCamera ();
			scamera.worldTransform = new UnityARMatrix4x4 (new Vector4 (1, 0, 0, 0), new Vector4 (0, 1, 0, 0), new Vector4 (0, 0, 1, 0), new Vector4 (0, 0, 0, 1));
			Matrix4x4 projMat = Matrix4x4.Perspective (60.0f, 1.33f, 0.1f, 30.0f);
			scamera.projectionMatrix = new UnityARMatrix4x4 (projMat.GetColumn(0),projMat.GetColumn(1),projMat.GetColumn(2),projMat.GetColumn(3));

			UnityARSessionNativeInterface.SetStaticCamera (scamera);


			editorConnection = EditorConnection.instance;
			editorConnection.Initialize ();
			editorConnection.RegisterConnection (PlayerConnected);
			editorConnection.RegisterDisconnection (PlayerDisconnected);
			editorConnection.Register (ConnectionMessageIds.updateCameraFrameMsgId, UpdateCameraFrame);
			editorConnection.Register (ConnectionMessageIds.addPlaneAnchorMsgeId, AddPlaneAnchor);
			editorConnection.Register (ConnectionMessageIds.updatePlaneAnchorMsgeId, UpdatePlaneAnchor);
			editorConnection.Register (ConnectionMessageIds.removePlaneAnchorMsgeId, RemovePlaneAnchor);
			editorConnection.Register (ConnectionMessageIds.screenCaptureYMsgId, ReceiveRemoteScreenYTex);
			editorConnection.Register (ConnectionMessageIds.screenCaptureUVMsgId, ReceiveRemoteScreenUVTex);

		}

		void PlayerConnected(int playerID)
		{
			currentPlayerID = playerID;

		}

		void OnGUI()
		{
			
			if (!bTexturesInitialized) 
			{
				if (currentPlayerID != -1) {
					guimessage = "Connected to ARKit Remote device : " + currentPlayerID.ToString ();

					if (GUI.Button (new Rect ((Screen.width / 2) - 200, (Screen.height / 2) - 200, 400, 100), "Start Remote ARKit Session")) 
					{
						SendInitToPlayer ();
					}
				} 
				else 
				{
					guimessage = "Please connect to player in the console menu";
				}

				GUI.Box (new Rect ((Screen.width / 2) - 200, (Screen.height / 2) + 100, 400, 50), guimessage);
			}

		}

		void PlayerDisconnected(int playerID)
		{
			if (currentPlayerID == playerID) {
				currentPlayerID = -1;
			}
		}

		void OnDestroy()
		{
			#if UNITY_2017_1_OR_NEWER		
			editorConnection.DisconnectAll ();
			#endif
		}


		void InitializeTextures(UnityARCamera camera)
		{
			int yWidth = camera.videoParams.yWidth;
			int yHeight = camera.videoParams.yHeight;
			int uvWidth = yWidth / 2;
			int uvHeight = yHeight / 2;
			remoteScreenYTex = new Texture2D(yWidth,yHeight, TextureFormat.R8, false, true);
			remoteScreenUVTex = new Texture2D(uvWidth,uvHeight, TextureFormat.RG16, false, true);

			bTexturesInitialized = true;
		}

		void UpdateCameraFrame(MessageEventArgs mea)
		{
			serializableUnityARCamera serCamera = mea.data.Deserialize<serializableUnityARCamera> ();

			UnityARCamera scamera = new UnityARCamera ();
			scamera = serCamera;

			InitializeTextures (scamera);

			UnityARSessionNativeInterface.SetStaticCamera (scamera);
			UnityARSessionNativeInterface.RunFrameUpdateCallbacks ();
		}

		void AddPlaneAnchor(MessageEventArgs mea)
		{
			serializableUnityARPlaneAnchor serPlaneAnchor = mea.data.Deserialize<serializableUnityARPlaneAnchor> ();

			ARPlaneAnchor arPlaneAnchor = serPlaneAnchor;
			UnityARSessionNativeInterface.RunAddAnchorCallbacks (arPlaneAnchor);
		}

		void UpdatePlaneAnchor(MessageEventArgs mea)
		{
			serializableUnityARPlaneAnchor serPlaneAnchor = mea.data.Deserialize<serializableUnityARPlaneAnchor> ();

			ARPlaneAnchor arPlaneAnchor = serPlaneAnchor;
			UnityARSessionNativeInterface.RunUpdateAnchorCallbacks (arPlaneAnchor);
		}

		void RemovePlaneAnchor(MessageEventArgs mea)
		{
			serializableUnityARPlaneAnchor serPlaneAnchor = mea.data.Deserialize<serializableUnityARPlaneAnchor> ();

			ARPlaneAnchor arPlaneAnchor = serPlaneAnchor;
			UnityARSessionNativeInterface.RunRemoveAnchorCallbacks (arPlaneAnchor);
		}

		void ReceiveRemoteScreenYTex(MessageEventArgs mea)
		{
			if (!bTexturesInitialized)
				return;
			remoteScreenYTex.LoadRawTextureData(mea.data);
			remoteScreenYTex.Apply ();
			UnityARVideo arVideo = Camera.main.GetComponent<UnityARVideo>();
			if (arVideo) {
				arVideo.SetYTexure(remoteScreenYTex);
			}

		}

		void ReceiveRemoteScreenUVTex(MessageEventArgs mea)
		{
			if (!bTexturesInitialized)
				return;
			remoteScreenUVTex.LoadRawTextureData(mea.data);
			remoteScreenUVTex.Apply ();
			UnityARVideo arVideo = Camera.main.GetComponent<UnityARVideo>();
			if (arVideo) {
				arVideo.SetUVTexure(remoteScreenUVTex);
			}

		}


		void SendInitToPlayer()
		{
			// Input string.
			const string input = "InitARKit";
			byte[] array = Encoding.ASCII.GetBytes(input);
			SendToPlayer (ConnectionMessageIds.fromEditorARKitSessionMsgId, array);
		}

		void SendToPlayer(System.Guid msgId, byte[] data)
		{
			editorConnection.Send (msgId, data);
		}


		// Update is called once per frame
		void Update () {
			
		}

	}
}
#endif