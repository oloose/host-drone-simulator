using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class GameController : Singleton<GameController> {

    [SerializeField]
    [Tooltip("Player gameobject")]
	private GameObject player;
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetButtonDown ("Respawn")) {

			// ignore button press in pause menu
			if(Time.deltaTime == 0) {
                return;
            }

            DroneController.Instance.Respawn();
		}
	}
}
