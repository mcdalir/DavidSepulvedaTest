using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[SerializeField] private Image loadingFill;
	[SerializeField] private Image background;
	[SerializeField] private Button playButton;
	[SerializeField] private Button exitButton;
	[SerializeField] private TMP_Text title;

	private void Start()
	{
		playButton.onClick.AddListener(Play);
		exitButton.onClick.AddListener(Exit);
	}

	public void Play()
	{
		LeanTween.moveLocalY(title.gameObject, 0f, .5f)
			.setEaseOutBounce()
			.setOnComplete(() => LoadLevel("Level1", OnLevelLoaded));

		loadingFill.transform.parent.gameObject.SetActive(true);
		playButton.gameObject.SetActive(false);
		exitButton.gameObject.SetActive(false);
	}

	private async void LoadLevel(string level, Action callback)
	{
		AsyncOperation loadingScene = SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);

		while (!loadingScene.isDone)
		{
			loadingFill.fillAmount = loadingScene.progress;
			await Task.Yield();
		}

		loadingFill.fillAmount = 1f;
		callback.Invoke();
	}

	private void UnloadLevel(string level)
	{
		SceneManager.UnloadSceneAsync(level);
	}

	private void OnLevelLoaded()
	{
		LeanTween.delayedCall(1f, () =>
		{
			LeanTween.value(loadingFill.transform.parent.gameObject, 1f, 0f, 1f).setOnUpdate((float val) =>
			{
				loadingFill.color = new Color(loadingFill.color.r, loadingFill.color.g, loadingFill.color.b, val);
				title.color = new Color(title.color.r, title.color.g, title.color.b, val);
				background.color = new Color(background.color.r, background.color.g, background.color.b, val);
			})
			.setOnComplete(() =>
			{
				UnloadLevel(SceneManager.GetActiveScene().name);
			});
		});
	}

	public void Exit()
	{
		Application.Quit();
	}
}
