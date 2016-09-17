using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MsgBox : MonoBehaviour
{
	// 200x300 px window will apear in the center of the screen.
	private Rect windowRect = new Rect((Screen.width - 200) / 2, (Screen.height - 300) / 2, 200, 200);
	// Only show it if needed.
	private bool show = false;
	string curStr;
	string maxStr;

	void OnGUI()
	{
		if (show)
			windowRect = GUI.Window(0, windowRect, DialogWindow, "Game Over");
	}

	// This is the actual window.
	void DialogWindow(int windowID)
	{
		Time.timeScale = 0.0f;
		float y = 20;
		GUIStyle style = new GUIStyle();
		style.fontSize = 20;
		GUI.Label(new Rect(5, y, windowRect.width, 20), curStr,style);
		GUI.Label(new Rect(5, y+30, windowRect.width, 20), maxStr, style);

		if (GUI.Button(new Rect(5, y+70, windowRect.width - 10, 50), "Restart"))
		{
			Time.timeScale = 1.0f;
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			show = false;
		}

		if (GUI.Button(new Rect(5, y+125, windowRect.width - 10, 50), "Exit"))
		{
			Application.Quit();
			show = false;
		}
	}

	// To open the dialogue from outside of the script.
	public void Open(uint curScore, uint maxScore)
	{
		curStr = "점수 : " + curScore;
		maxStr = "최고 점수 : " + maxScore;
		show = true;
	}
}