using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AssemblyCSharp;
using System;

public class MenuButtonEventHandler : Singleton<MenuButtonEventHandler> {

    [SerializeField]
    [Tooltip("Game manager object")]
    private GameObject game;

    [SerializeField]
    [Tooltip("Canvas group for time control")]
    private CanvasGroup timePanelCanvasGroup;

    [SerializeField]
    [Tooltip("How fast the time runnes, 1 is equals to real time")]
    private InputField inpTimeScale;

    [SerializeField]
    [Tooltip("Enables constant hight (no high lose over time)")]
    private Toggle togHeightControl;

    [SerializeField]
    [Tooltip("Enables setting a maximum drone tilt when stabilizing")]
    private Toggle togTiltControl;

    [SerializeField]
    [Tooltip("Maximum drone tilt when stabilization is disabled")]
    private InputField inpMaxTilt;

	void Start() {
        if(game == null) {
            Debug.LogError("Game not assigned!");
        }

        if (timePanelCanvasGroup == null) {
            Debug.LogError("timePanelCanvasGroup not assigned!");
        }

        if (inpTimeScale == null) {
            Debug.LogError("inpTimeScale not assigned!");
        }

        if (togHeightControl == null) {
            Debug.LogError("togHeightControl not assigned!");
        }

        if (togTiltControl == null) {
            Debug.LogError("togTiltControl not assigned!");
        }

        if (inpMaxTilt == null) {
            Debug.LogError("inpMaxTilt not assigned!");
        }
	}
	
	public void OnClick(string action) {
		
		switch (action) {			
		    case "start":
			    GameStateController.Instance.SetState(GameState.running);
			    break;
		    case "resume":
                GameStateController.Instance.SetState(GameState.running);
			    break;
		    case "options":
                GameStateController.Instance.ShowOptions(true);
			    break;
		    case "closeOptions":
                GameStateController.Instance.ShowOptions(false);
			    break;
		    case "exit":
                GameStateController.Instance.Exit();
			    break;
            case "controls":
                GameStateController.Instance.ShowControls(true);
                break;
            case "closeControls":
                GameStateController.Instance.ShowControls(false);
                break;
            case "respawn":
                DroneController.Instance.Respawn();
                GameStateController.Instance.TogglePause();
                break;
        }
	}
	
	public void ChangeTime(string action) {

		CustomTimeProvider ctp = game.GetComponent<CustomTimeProvider> ();
		DateTime dateTime = ctp.GetCurrent ();

		
		switch (action) {			
		    case "addDay":
			    dateTime = dateTime.AddDays(1);
			    break;
		    case "addMonth":
			    dateTime = dateTime.AddMonths(1);
			    break;
		    case "addYear":
			    dateTime = dateTime.AddYears(1);
			    break;
		    case "addHour":
			    dateTime = dateTime.AddHours(1);
			    break;
		    case "addMinute":
			    dateTime = dateTime.AddMinutes(1);
			    break;
		    case "subDay":
			    dateTime = dateTime.AddDays(-1);
			    break;
		    case "subMonth":
			    dateTime = dateTime.AddMonths(-1);
			    break;
		    case "subYear":
			    dateTime = dateTime.AddYears(-1);
			    break;
		    case "subHour":
			    dateTime = dateTime.AddHours(-1);
			    break;
		    case "subMinute":
			    dateTime = dateTime.AddMinutes(-1);
			    break;
		    default: return;
		}

		ctp.SetCurrent (dateTime);
	}

	public void OnToggleSystemTime(bool isChecked) {

		if (isChecked)
			Game.dateTimeProvider = new SystemTimeProvider ();
		else {

			CustomTimeProvider ctp = game.GetComponent<CustomTimeProvider> ();
			ctp.SetCurrent(DateTime.Now);
			Game.dateTimeProvider = ctp;

		}

		timePanelCanvasGroup.interactable = !isChecked;
	}

	public void TimeScaleChanged(string value) {
		
		CustomTimeProvider ctp = game.GetComponent<CustomTimeProvider> ();

		float scale;

		if (!float.TryParse (value, out scale)) {

			ctp.SetTimeScale(1);
			inpTimeScale.text = "1";
			return;
		}

//		DateTime dateTime = ctp.GetCurrent ();

		ctp.SetTimeScale (scale);
	}

	public void ChangeTimeScale(string value) {

		inpTimeScale.text = value;
		TimeScaleChanged (value);
	}

	public void OnToggleHeightControl(bool isChecked) {

		DroneController.Instance.StabilizeHeight = isChecked;

		if (!isChecked) {

			togTiltControl.isOn = true;
		}
	}
	
	public void OnToggleTiltControl(bool isChecked) {

        DroneController.Instance.StabilizeTilt = isChecked;

		if (!isChecked) {

			togHeightControl.isOn = true;
		}

		inpMaxTilt.interactable = isChecked;
	}

	public void MaxTiltChanged(string value) {

		float maxTilt = float.Parse (value);

        DroneController.Instance.MaxTiltAngle = maxTilt;
	}
}
