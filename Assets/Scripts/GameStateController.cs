using UnityEngine;
using AssemblyCSharp;
using UnityEngine.EventSystems;

public class GameStateController : Singleton<GameStateController> {

    [SerializeField]
    [Tooltip("Camera used when starting the simulator")]
    private Camera startScreenCamera;

    [SerializeField]
    [Tooltip("Camera system handling different cameras")]
    private GameObject cameraSystem;

    [SerializeField]
    [Tooltip("Canvas holding the menu screen")]
    private Canvas screen;

    [SerializeField]
    [Tooltip("Exit dialog gameobject")]
	public GameObject modalPanel;

    [SerializeField]
    [Tooltip("Resume Button")]
	private GameObject btnResume;

    [SerializeField]
    [Tooltip("Start Button")]
    private GameObject btnStart;

    [SerializeField]
    [Tooltip("Restart Button")]
    private GameObject btnRespawn;

    [SerializeField]
    [Tooltip("Event system handling button presses and menu interaction")]
    private EventSystem eventSystem;

    private Animator screenAnimator;
    private GameState currentState;

    // Use this for initialization	
    void Start() {

        //Set buttons inactive while the game is still in start menu (start button not clicked)
		btnResume.SetActive(false);
        btnRespawn.SetActive(false);

        eventSystem.SetSelectedGameObject (btnStart);
        
		CameraController.Instance.SetSpecialCamera (startScreenCamera);

		currentState = GameState.start;

		screenAnimator = screen.GetComponent<Animator> ();

		Time.timeScale = 0;

	}

	void LateUpdate() {

		bool pauseButtonPressed = Input.GetButtonDown ("Pause");
		if (pauseButtonPressed)
			TogglePause ();
	}

    /// <summary>
    /// Toggles the pause menu on/off.
    /// </summary>
	public void TogglePause() {
		
		if (currentState == GameState.pause) {
			if(screenAnimator.GetBool("Options"))
				return;
			SetState (GameState.running);
		}
		else if(currentState == GameState.running)
			SetState(GameState.pause);
	}

	public void ShowOptions(bool show) {

		screenAnimator.SetBool("Options", show);

		if (show)
			return;

		if (currentState == GameState.start)
			eventSystem.SetSelectedGameObject(btnStart);
		else if (currentState == GameState.pause)
			eventSystem.SetSelectedGameObject(btnResume);
	}

    /// <summary>
    /// Opens or closes the controls overview in the menu.
    /// </summary>
    /// <param name="show"></param>
    public void ShowControls(bool show) {
        screenAnimator.SetBool("Controls", show);

        if (show) {
            return;
        }

        if(currentState == GameState.start) {
            eventSystem.SetSelectedGameObject(btnStart);
        } else if(currentState == GameState.pause) {
            eventSystem.SetSelectedGameObject(btnResume);
        }
    }

	public void SetState(GameState state) {

		switch (state) {

		    case GameState.running:
			    if(currentState == GameState.start) {
                    CameraController.Instance.SetSpecialCamera(null);
				    AudioListener.volume = 0.5f;
			    }

			    screenAnimator.SetBool("Paused", false);

			    Time.timeScale = 1;

			    Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                AudioListener.pause = false;

			    screen.GetComponent<CanvasGroup>().interactable = false;

			    break;
		    case GameState.pause:
			    btnStart.SetActive(false);
			    btnResume.SetActive(true);
                btnRespawn.SetActive(true);

			    eventSystem.SetSelectedGameObject(btnResume);

			    screenAnimator.SetBool("Paused", true);
			
			    Time.timeScale = 0;

		        Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                AudioListener.pause = true;
			
			    screen.GetComponent<CanvasGroup>().interactable = true;

			    break;
		}

		currentState = state;
	}

	public void Exit(){
        ModalDialogueController.Instance.Ask ("Möchten Sie Drohnensimulator wirklich beenden?", Buttons.YesNo, ReallyExit, OnModalDialogueClosed);
		GameObject declineButton = ModalDialogueController.Instance.GetButton (ButtonType.decline).gameObject;

		eventSystem.SetSelectedGameObject (declineButton);
	}

	private void OnModalDialogueClosed() {

		if (currentState == GameState.start)
			eventSystem.SetSelectedGameObject (btnStart);
		else
			eventSystem.SetSelectedGameObject (btnResume);
	}

	private void ReallyExit(){
		Application.Quit ();
	}
}
