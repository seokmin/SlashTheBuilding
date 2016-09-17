using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour {
	public SpriteRenderer[] lives;
	public GameObject bougyoBar;
	public SpriteRenderer wazaRenderer;
	public GameObject wazaBar;

	public Sprite heart_empty;
	public Sprite heart_full;

	public Sprite waza_yet;
	public Sprite waza_full;
	public UnityEngine.UI.Text scoreText;

	public void SetScore(uint score)
	{
		scoreText.text = score.ToString();
	}

	public void SetDifenseGage(float percent)
	{
		bougyoBar.transform.localScale = new Vector3(percent/100.0f, 1.0f, 1.0f);
	}
	public float GetDifenseGage()
	{
		return bougyoBar.transform.localScale.x * 100.0f;
	}
	public void SetWazaGage(float percent)
	{
		wazaBar.transform.localScale = new Vector3(percent / 100.0f, 1.0f, 1.0f);
		if (wazaBar.transform.localScale.x >= 1.0f)
			wazaRenderer.sprite = waza_full;
		else
			wazaRenderer.sprite = waza_yet;
	}
	public float GetWazaGage()
	{
		return wazaBar.transform.localScale.x * 100.0f;
	}
	IEnumerator EndMsg()
	{
		yield return new WaitForSeconds(1.0f);

		uint maxScore = 0u;
		uint curScore = Convert.ToUInt32(scoreText.text);
		if (PlayerPrefs.HasKey("score") == false)
			maxScore = Convert.ToUInt32( scoreText.text);
		else
		{
			maxScore = Math.Max(Convert.ToUInt32(PlayerPrefs.GetString("score")), curScore);
		}
		PlayerPrefs.SetString("score", maxScore.ToString());
		PlayerPrefs.Save();

		EditorUtility.DisplayDialog("쥬금", "점수 : " + curScore + "\n최고 점수 : " + maxScore, "다시 시작");
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	public void SetHeart(int n)
	{
		int x = n;
		x = 3 - x;
		for (int i = 0; i < 3; ++i)
		{
			if(i< x)
			{
				lives[i].sprite = heart_empty;
			}
			else
				lives[i].sprite = heart_full;
		}
		// 게임 끝
		if (n == 0)
		{
			StartCoroutine(EndMsg());
		}
	}
}