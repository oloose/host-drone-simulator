using UnityEngine;
using AssemblyCSharp;
using UnityEngine.UI;
using UnityEngine.Events;

public class ModalDialogueController : Singleton<ModalDialogueController> {

	private Button btnAccept;
	private Button btnDecline;
	private Text txtQuestion;

	private Animator animator;

	void Start() {

		animator = GetComponent<Animator> ();

		Button[] buttons = GetComponentsInChildren<Button> ();
		
		foreach (Button button in buttons) {
			if(button.gameObject.name == "btnAccept")
				btnAccept = button;
			else if(button.gameObject.name == "btnDecline")
				btnDecline = button;
		}
		
		Text[] texts = GetComponentsInChildren<Text> ();
		
		foreach (Text text in texts) {
			if(text.gameObject.name == "txtQuestion")
				txtQuestion = text;
		}

//		gameObject.SetActive (false);
	}


//	public delegate void ButtonClickedEventHandler(ButtonType button);

	public void Ask(string question, Buttons buttons, UnityAction acceptEvent, UnityAction declineEvent) {

		txtQuestion.text = question;

		btnAccept.onClick.RemoveAllListeners ();
		btnAccept.onClick.AddListener (CloseDialogue);
		btnAccept.onClick.AddListener (acceptEvent);

		btnDecline.onClick.RemoveAllListeners();
		btnDecline.onClick.AddListener (CloseDialogue);
		btnDecline.onClick.AddListener (declineEvent);

		transform.SetAsLastSibling ();

		//		gameObject.SetActive (true);
		
		SetButtons (buttons);

		animator.SetBool ("DisplayDialogue", true);
	}

	public void CloseDialogue() {
		
		animator.SetBool ("DisplayDialogue", false);

//		gameObject.SetActive (false);
	}

	private void SetButtons(Buttons buttons) {

		switch (buttons) {
		case Buttons.YesNo:
			SetButtonText(btnAccept, "Ja");
			SetButtonText(btnDecline, "Nein");
			break;
		case Buttons.OkCancel:
			SetButtonText(btnAccept, "OK");
			SetButtonText(btnDecline, "Abbrechen");
			break;
		}
	}

	private void SetButtonText(Button button, string newText) {

		Text text = button.GetComponentInChildren<Text> ();
		text.text = newText;
	}

	public Button GetButton(ButtonType type) {

		switch (type) {

		case ButtonType.accept:
			return btnAccept;
		case ButtonType.decline:
			return btnDecline;
		}

		return null;
	}
}
