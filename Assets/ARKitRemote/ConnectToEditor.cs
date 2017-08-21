using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using System.Text;
using Utils;

namespace UnityEngine.XR.iOS
{
	
	public class ConnectToEditor : MonoBehaviour
	{
		PlayerConnection playerConnection;
		UnityARSessionNativeInterface m_session;
		int editorID;

		Texture2D frameBufferTex;

		// Use this for initialization
		void Start()
		{
			Debug.Log("STARTING ConnectToEditor");
			editorID = -1;
			playerConnection = PlayerConnection.instance;
			playerConnection.RegisterConnection(EditorConnected);
			playerConnection.RegisterDisconnection(EditorDisconnected);
			playerConnection.Register(ConnectionMessageIds.fromEditorARKitSessionMsgId, InitializeARKit);
			m_session = null;

		}

		void OnGUI()
		{
			if (m_session == null) {	
				GUI.Box (new Rect ((Screen.width / 2) - 200, (Screen.height / 2), 400, 50), "Waiting for editor connection...");
			}
		}


		void InitializeARKit(MessageEventArgs mea)
		{
			Debug.Log("init ARKit");
			#if !UNITY_EDITOR
			Application.targetFrameRate = 60;
			m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();
			ARKitWorldTackingSessionConfiguration config = new ARKitWorldTackingSessionConfiguration();
			config.planeDetection = UnityARPlaneDetection.Horizontal;
			config.alignment = UnityARAlignment.UnityARAlignmentGravity;
			config.getPointCloudData = true;
			config.enableLightEstimation = true;
			m_session.RunWithConfig(config);

		 	UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
			UnityARSessionNativeInterface.ARAnchorAddedEvent += ARAnchorAdded;
			UnityARSessionNativeInterface.ARAnchorUpdatedEvent += ARAnchorUpdated;
			UnityARSessionNativeInterface.ARAnchorRemovedEvent += ARAnchorRemoved;

			#endif
		}

		public void ARFrameUpdated(UnityARCamera camera)
		{
			serializableUnityARCamera serARCamera = camera;
			SendToEditor(ConnectionMessageIds.updateCameraFrameMsgId, serARCamera);

		}

		public void ARAnchorAdded(ARPlaneAnchor planeAnchor)
		{
			serializableUnityARPlaneAnchor serPlaneAnchor = planeAnchor;
			SendToEditor (ConnectionMessageIds.addPlaneAnchorMsgeId, serPlaneAnchor);
		}

		public void ARAnchorUpdated(ARPlaneAnchor planeAnchor)
		{
			serializableUnityARPlaneAnchor serPlaneAnchor = planeAnchor;
			SendToEditor (ConnectionMessageIds.updatePlaneAnchorMsgeId, serPlaneAnchor);
		}

		public void ARAnchorRemoved(ARPlaneAnchor planeAnchor)
		{
			serializableUnityARPlaneAnchor serPlaneAnchor = planeAnchor;
			SendToEditor (ConnectionMessageIds.removePlaneAnchorMsgeId, serPlaneAnchor);
		}

		void EditorConnected(int playerID)
		{
			Debug.Log("connected");

			editorID = playerID;

		}

		void EditorDisconnected(int playerID)
		{
			if (editorID == playerID)
			{
				editorID = -1;
			}

			DisconnectFromEditor ();
			#if !UNITY_EDITOR
			if (m_session != null)
			{
				m_session.Pause();
				m_session = null;
			}
			#endif
		}


		public void SendToEditor(System.Guid msgId, object serializableObject)
		{
			byte[] arrayToSend = serializableObject.SerializeToByteArray ();
			SendToEditor (msgId, arrayToSend);
		}

		public void SendToEditor(System.Guid msgId, byte[] data)
		{
			if (playerConnection.isConnected)
			{
				playerConnection.Send(msgId, data);
			}


		}

		public void DisconnectFromEditor()
		{
			#if UNITY_2017_1_OR_NEWER		
			playerConnection.DisconnectAll();
			#endif
		}


	}

}
