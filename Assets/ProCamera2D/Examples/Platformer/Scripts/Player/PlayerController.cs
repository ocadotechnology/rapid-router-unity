using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D.Platformer
{
	[RequireComponent(typeof(SphereCollider))]
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerController : MonoBehaviour
	{
	    public float PlayerSpeed = 5.5f;

	    public MovementAxis Axis;

	    Vector3 _targetVelocity = Vector3.zero;

	    void FixedUpdate()
	    {
	    	switch (Axis) 
	    	{
	    		case MovementAxis.XY:
	    		_targetVelocity = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
	    		break;

	    		case MovementAxis.XZ:
	    		_targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	    		break;

	    		case MovementAxis.YZ:
	    		_targetVelocity = new Vector3(0, Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
	    		break;
	    	}
	        
	        _targetVelocity *= PlayerSpeed;
	        GetComponent<Rigidbody>().AddForce(_targetVelocity, ForceMode.Force);
	    }
	}
}