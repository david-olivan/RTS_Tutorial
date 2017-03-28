﻿using UnityEngine;
using System.Collections;
using RTS;

public class UserInput : MonoBehaviour {

    //public HUD hud;

	private Player player;

	private void Start () {
		player = transform.root.GetComponent<Player> ();
	}
	private void Update () {
		if (player.human) {
			MoveCamera ();
			RotateCamera ();
            MouseActivity ();
		}
	}

    private void MouseActivity () {
        if (Input.GetMouseButtonDown(0)) { LeftMouseClick (); }
        else if (Input.GetMouseButtonDown(2)) { RightMouseClick (); }
    }
    private void LeftMouseClick () {
        if (player.hud.MouseInBounds()) {
            GameObject hitObject = FindHitObject ();
            Vector3 hitPoint = FindHitPoint ();
            if (hitObject && hitPoint != ResourceManager.InvalidPosition) {
                if (player.SelectedObject) { player.SelectedObject.MouseClick (hitObject, hitPoint, player); }
                else if (hitObject.name != "Ground") {
                    WorldObject worldObject = hitObject.transform.root.GetComponent<WorldObject> ();
                    if (worldObject) {
                        // We already know the player has no selected object
                        player.SelectedObject = worldObject;
                        worldObject.SetSelection (true);
                    }
                }
            }
        }
    }
    private void RightMouseClick () {
        if (player.hud.MouseInBounds() && !Input.GetKey(KeyCode.LeftAlt) && player.SelectedObject) {
            player.SelectedObject.SetSelection (false);
            player.SelectedObject = null;
        }
    }

    private GameObject FindHitObject () {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit)) { return hit.collider.gameObject; }
        return null;
    }
    private Vector3 FindHitPoint() {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) { return hit.point; }
        return ResourceManager.InvalidPosition;
    }

	private void MoveCamera () {
		float xpos = Input.mousePosition.x;
		float ypos = Input.mousePosition.y;
		Vector3 movement = new Vector3 (0, 0, 0);

		// Horizontal camera movement
		if (xpos >= 0 && xpos < ResourceManager.ScrollWidth) {
			movement.x -= ResourceManager.ScrollSpeed;
		} else if (xpos <= Screen.width && xpos > Screen.width - ResourceManager.ScrollWidth) {
			movement.x += ResourceManager.ScrollSpeed;
		}

		// Vertical camera movement
		if (ypos >= 0 && ypos < ResourceManager.ScrollWidth) {
			movement.z -= ResourceManager.ScrollSpeed;
		} else if (ypos <= Screen.height && ypos > Screen.height - ResourceManager.ScrollWidth) {
			movement.z += ResourceManager.ScrollSpeed;
		}

		// Make sure movement is in the direction the camera is pointing
		// but ignore the vertical tilt of the camera to get sensible scrolling
		movement = Camera.main.transform.TransformDirection (movement);
		movement.y = 0;

		// Away from ground movement
		movement.y -= ResourceManager.ScrollSpeed * Input.GetAxis ("Mouse ScrollWheel");

		// Calculate desired camera position based on received input
		Vector3 origin = Camera.main.transform.position;
		Vector3 destination = origin;
		destination.x += movement.x;
		destination.y += movement.y;
		destination.z += movement.z;

		// Limit away from ground movement to be between a minimum and maximum distance
		if (destination.y > ResourceManager.MaxCameraHeight) {
			destination.y = ResourceManager.MaxCameraHeight;
		} else if (destination.y < ResourceManager.MinCameraHeight) {
			destination.y = ResourceManager.MinCameraHeight;
		}

		// If a change in position is detected perform the necessary update
		if (destination != origin) {
			Camera.main.transform.position = Vector3.MoveTowards (origin, destination, Time.deltaTime * ResourceManager.ScrollSpeed);
		}
	}
	private void RotateCamera() {
		Vector3 origin = Camera.main.transform.eulerAngles;
		Vector3 destination = origin;

		// Detect rotation amount is ALT is being held and the Right mouse button is down
		if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButton(1)) {
			destination.x -= Input.GetAxis ("Mouse Y") * ResourceManager.RotateAmount;
			destination.y += Input.GetAxis ("Mouse X") * ResourceManager.RotateAmount;
		}

		// If a change in position is detected perform the necessary update
		if (destination != origin) {
			Camera.main.transform.eulerAngles = Vector3.MoveTowards (origin, destination, Time.deltaTime * ResourceManager.RotateSpeed);
		}
	}
}