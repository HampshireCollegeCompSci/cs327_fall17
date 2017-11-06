using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour {
	
	Button button;

	UILanguages translator;

	void Start () {
		translator = FindObjectOfType<UILanguages>();
		Button ();
	}

	public void Button() {
		if (GetComponent<Text>().text == "MainMenu") {
			string mainMenuText = translator.Translate ("MainMenu");
			GetComponent<Text>().text = mainMenuText;
		}
		if (GetComponent<Text>().text == "HowToPlay") {
			string howToPlayText = translator.Translate ("HowToPlay");
			GetComponent<Text>().text = howToPlayText;
		}
		if (GetComponent<Text>().text == "PlayAgain") {
			string playAgainText = translator.Translate ("PlayAgain");
			GetComponent<Text>().text = playAgainText;
		}
		if (GetComponent<Text>().text == "Settings") {
			string settingsText = translator.Translate ("Settings");
			GetComponent<Text>().text = settingsText;
		}
		if (GetComponent<Text>().text == "Credits") {
			string creditsText = translator.Translate ("Credits");
			GetComponent<Text>().text = creditsText;
		}
	}
	/*
	string MainMenuTranslation () {
		string mainMenuText = translator.Translate ("MainMenu");
		return mainMenuText;
	}

	string PlayAgainTranslation() {
		string playAgainText = translator.Translate ("MainMenu");
		return playAgainText;
	}
	*/

}
