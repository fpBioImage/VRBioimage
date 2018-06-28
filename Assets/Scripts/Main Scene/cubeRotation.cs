using UnityEngine;
using System.Collections;

/* This script controls the rotation of the 3D model
 * loaded into the viewer.
 * 
 * It supports rotation with the arrow keys, by clicking
 * and dragging with the mouse, and with touch controls.
 * 
 * Note that the touch controls are still very buggy, and
 * only work well on certain phones/tablets. 
*/

public class cubeRotation : MonoBehaviour {

	// Variables
	public float rotateSpeed = 2000f;
	private Transform camTransform;

	public float mouseSensitivity = 5.0f;
	private bool freezeRotation = false;

	public void setFreezeRotation(bool input){
		freezeRotation = input;
	}

	// Use this for initialization
	void Start () {
		camTransform = Camera.main.transform;
		rotateSpeed = 2000.0f;
		//mouseSensitivity = 5.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!variables.showBindingBox) {
			freezeRotation = false;
		}

		// Keyboard arrow control
		if (!variables.freezeAll) {
			Vector3 vertRotAxis = transform.InverseTransformDirection (camTransform.TransformDirection (Vector3.right)).normalized;

			float horizontalRot = Input.GetAxis ("H2") * -rotateSpeed;
			float verticalRot = Input.GetAxis ("V2") * rotateSpeed;

			transform.Rotate (vertRotAxis, verticalRot, Space.Self);
			transform.Rotate (Vector3.up, horizontalRot, Space.World);
		}

		// Mouse click control
		if (Input.GetMouseButton (0) && variables.freezeMouse && !freezeRotation) {
			// Rotate with the mouse
			Vector3 vertRotAxis = transform.InverseTransformDirection (camTransform.TransformDirection (Vector3.right)).normalized;

			float horizontalRot = Input.GetAxis ("Mouse X") * -mouseSensitivity;
			float verticalRot = Input.GetAxis ("Mouse Y") * mouseSensitivity;

			transform.Rotate (vertRotAxis, verticalRot, Space.Self);
			transform.Rotate (Vector3.up, horizontalRot, Space.World);
		}

		if (variables.vr) {
			transform.Rotate (Vector3.up, 15.0f * Time.deltaTime);
		}

	}
		
}
