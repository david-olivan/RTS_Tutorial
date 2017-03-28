using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public string username;
	public bool human;
    public HUD hud;
    public WorldObject SelectedObject { get; set; }

	private void Start () {
        hud = GetComponentInChildren<HUD> ();
	}
	private void Update () {
	}
}