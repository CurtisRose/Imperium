﻿using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	// Logic variables
	private bool isSelected;
	private Team team;
	public float movementSpeed;
	private Vector3 destination;
	//private bool isMoving = false;
	private Vector3 testPosition;
	private bool isMoving = false;
	public bool displayWayPointGizmos;

	// Model variables
	private Color startColor = Color.black;
	private Color selectedColor = Color.red;
	Transform sphere;

	Vector3[] path;
	int targetIndex;


	/*********************************************************************************/
	/*	Functions inherited from MonoBehaviour	- Order: Relevance					 */		
	/*********************************************************************************/

	private void Start() {
		this.isSelected = false;
		GetComponent<Renderer> ().material.color = startColor;
		testPosition = transform.position;
		sphere = transform.FindChild ("Sphere");
		sphere.GetComponent<Renderer> ().material.color = startColor;
	}

	//While isMoving = true, units move towards their targets
	//Fixed Update is used because it is a physics update, happens more often
	/*void FixedUpdate(){
		testPosition = transform.position;
		if (isMoving) {
			//Debug.Log ("Unit is moving");
			float step = movementSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, destination, step);
			//Unfortunately this never occurs, I am trying to figure it out.
			//Movement still works though, they just keep adjusting their positions.
			//Debug.Log (testPosition + " " + transform.position);
			if(transform.position.x == testPosition.x && transform.position.z == testPosition.z){
				isMoving = false;
				//Debug.Log ("Unit has stopped moving");
			}
			
		}
	}*/

	/*********************************************************************************/
	/*	Public Functions - Order: Alphabetic										 */		
	/*********************************************************************************/	

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful){
		if(pathSuccessful){
			path = newPath;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}
	
	IEnumerator FollowPath() {
		if(path.GetLength(0) > 0){
			Vector3 currentWaypoint = path[0];
			targetIndex = 0;
			isMoving = true;
			
			while(true){
				if (transform.position.x == currentWaypoint.x && transform.position.z == currentWaypoint.z){
					targetIndex++;
					if(targetIndex >= path.Length){
						yield break;
					}
					currentWaypoint = path[targetIndex];
				}

				transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, movementSpeed * Time.deltaTime);
				yield return null;
			}
		}
	}

	public bool isTeam(Team inTeam) {
		bool isTeam = false;
		if(this.team == inTeam) {
			isTeam = true;
		}
		return isTeam;
	}
	
	public bool isVisible() {
		return GetComponent<Renderer> ().isVisible;
	}

	//Sets the destination to the target and isMoving to true.
	//Movement gets done in Update method.
	public void makeMove(Vector3 destination){ //Maybe should return bool, if unit cannot get to location return false?? otherwise true?
		destination.y = transform.position.y;
		PathRequestManager.RequestPath(transform.position, destination, OnPathFound);		
	}

	/*********************************************************************************/
	/*	Getter and Setter Functions - Order: Alphabetic							 	 */		
	/*********************************************************************************/
	
	public bool getSelected() {
		return this.isSelected;
	}
	
	public void setSelected(bool selected) {

		if (selected) {
			GetComponent<Renderer> ().material.color = selectedColor;
			sphere.GetComponent<Renderer> ().material.color = selectedColor;
		} else {
			GetComponent<Renderer> ().material.color = startColor;
			sphere.GetComponent<Renderer> ().material.color = startColor;
		}
		this.isSelected = selected;
	}
	
	public void setTeam(Team inTeam) {
		this.team = inTeam;
	}

	public void OnDrawGizmos() {
		if (displayWayPointGizmos) {
			if (path != null) {
				for (int i = targetIndex; i < path.Length; i ++) {
					Gizmos.color = Color.cyan;
					Gizmos.DrawCube (path [i], Vector3.one);
					
					if (i == targetIndex) {
						Gizmos.DrawLine (transform.position, path [i]);
					} else {
						Gizmos.DrawLine (path [i - 1], path [i]);
					}
				}
			}
		}
	}
}
