using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	protected static GameManager instance;

	[Header("References")]
	[SerializeField] private Camera mainCamera;
	[SerializeField] private Camera UICamera;
	[SerializeField] private GameObject graphy;
	[SerializeField] private GameObject ratPrefab;
	[SerializeField] private GameObject defensePrefab;
	[SerializeField] private GameObject inventoryUI;
	[SerializeField] private Cheese cheese;
	[SerializeField] private TMP_Text gameOverText;

	[Header("Game variables")]
	[SerializeField] private List<InventoryItem> defenseInventory;
	[SerializeField] private DefenseType selectedDefense;
	[SerializeField] private List<DefenseAttributes> defenseAttributes;
	[SerializeField] private List<RatAttributes> ratAttributes;
	[SerializeField] private List<Wave> waves;
	//private List<Defense> activeDefenses;
	[SerializeField] private float currentInterval;
	[SerializeField] private float elapsedFromLastRat;
	private int currentWave;
	[SerializeField] private bool canWave;
	[SerializeField] private bool canPlay;
	public static bool CanPlay => instance.canPlay;

	private void Awake()
	{
		if (instance != null)
			Destroy(instance);
		instance = this;
		//activeDefenses = new List<Defense>();
		selectedDefense = DefenseType.None;
		gameOverText.transform.localScale = Vector3.zero;
		inventoryUI.gameObject.SetActive(false);
	}

	private void Start()
	{
		//Show performance information
		graphy.SetActive(true);

		//Get ready text
		LeanTween.delayedCall(1f, () =>
		{
			instance.gameOverText.text = "Get ready!";
			LeanTween.scale(instance.gameOverText.gameObject, Vector3.one, .25f);
			LeanTween.delayedCall(2f, () =>
			{
				LeanTween.scale(instance.gameOverText.gameObject, Vector3.zero, .25f);
			});
		});

		//Start game
		LeanTween.delayedCall(3.5f, () =>
		{
			instance.gameOverText.text = "Don't let them eat me!";
			LeanTween.scale(instance.gameOverText.gameObject, Vector3.one, .25f);

			LeanTween.delayedCall(2.25f, () =>
			{
				LeanTween.cancel(instance.gameOverText.gameObject);
				LeanTween.scale(instance.gameOverText.gameObject, Vector3.zero, .25f)
				.setOnComplete(() =>
				{
					currentInterval = waves[0].startingInterval;
					canWave = canPlay = true;
					inventoryUI.gameObject.SetActive(true);
				});
			});
		});
	}

	private void Update()
	{

		//Instantiate defense characters
		if (Input.GetMouseButtonDown(0) && canPlay)
		{
			Vector3 mousePos = Input.mousePosition;

			mousePos.z = mainCamera.transform.position.y;

			Vector3 pos = mainCamera.ScreenToWorldPoint(mousePos);

			pos = new Vector3(Mathf.Round(pos.x), 0f, Mathf.Round(pos.z));

			if (pos.x >= -4f && pos.x <= 4f && pos.z >= -4f && pos.z <= 10f)
			{
				InventoryItem found = defenseInventory.Find(x => x.defenseType == selectedDefense && x.stock > 0);

				if (found != null)
				{
					Defense defense = Instantiate(defensePrefab, pos, Quaternion.identity).GetComponent<Defense>();
					defense.Initialize(defenseAttributes.Find(x => x.defenseType == selectedDefense));
					defenseInventory.Find(x => x.defenseType == selectedDefense).stock--;
					UpdateInventory();
				}
			}
		}

		//check if can instantiate rats and/or set a new wave
		if (elapsedFromLastRat >= currentInterval && canWave)
		{
			if (waves.Count > currentWave + 1 && waves[currentWave].ratsCurrentAmount >= waves[currentWave].ratsMaxAmount)
			{
				currentWave++;
				currentInterval = waves[currentWave].startingInterval;
				canWave = false;
				LeanTween.delayedCall(2f, () =>
				{
					canWave = true;
				});
				return;
			}

			//Instantiate rats
			int ins = Random.Range(1, Mathf.Min(3, waves[currentWave].ratsMaxAmount - waves[currentWave].ratsCurrentAmount));
			for (int i = 0; i < ins; i++)
			{
				Rat newRat = Instantiate(ratPrefab).GetComponent<Rat>();
				newRat.transform.position = new Vector3(Mathf.Round(Random.Range(-4f, 4f)), 0f, 14f);
				newRat.SetAttributes(ratAttributes[Random.Range(0, 3)]);
				waves[currentWave].ratsList.Add(newRat);
			}
			waves[currentWave].ratsCurrentAmount += ins;
			currentInterval = Mathf.Max(waves[currentWave].minInterval, currentInterval /= waves[currentWave].intervalDivider);

			elapsedFromLastRat = 0f;

			//Waves end
			if (waves.Count > 0 && currentWave + 1 >= waves.Count && waves[currentWave].ratsCurrentAmount >= waves[currentWave].ratsMaxAmount)
				canWave = false;
		}
		else if (elapsedFromLastRat >= currentInterval)
		{
			//Check if game is over
			if (currentWave + 1 >= waves.Count)
			{
				//amount of destroyed rats
				int ratsAlive = 0;
				waves.ForEach(w =>
				{
					w.ratsList.ForEach(r => ratsAlive += r != null ? 1 : 0);
				});

				if (waves[waves.Count - 1].ratsList.Count >= waves[waves.Count - 1].ratsMaxAmount && ratsAlive == 0)
					GameOver();
			}
		}

		elapsedFromLastRat += Time.deltaTime;
	}

	//Update inventory UI
	private void UpdateInventory()
	{
		defenseInventory.ForEach(x =>
		{
			foreach (InventoryItemUI inventoryItemUI in FindObjectsOfType<InventoryItemUI>())
			{
				if (inventoryItemUI.defenseType == x.defenseType)
					inventoryItemUI.qtyText.text = x.stock.ToString();
			}
		});
	}

	//Add stock to inventory
	public static int DefenseAddInventory(DefenseType defenseType, int amount)
	{
		InventoryItem found = instance.defenseInventory.Find(x => x.defenseType == defenseType);

		if (found == null)
			return 0;

		return found.stock += amount;
	}

	//Select a defense to instantiate
	public static void SetSelectedDefense(DefenseType defenseType)
	{
		instance.selectedDefense = defenseType;
	}

	//Game is over and check if player win or lose the game
	public static void GameOver()
	{
		if (!instance.canPlay) return;

		instance.canPlay = instance.canWave = false;

		instance.gameOverText.text = instance.cheese.health > 0f ? "You win!" : "You lose!";

		LeanTween.scale(instance.gameOverText.gameObject, Vector3.one, .25f);

		//Destroy existing rats
		LeanTween.delayedCall(5f, () =>
		{
			instance.waves.ForEach(wave =>
			{
				if (wave != null)
				{
					wave.ratsList.ForEach(rat =>
					{
						if (rat != null)
							Destroy(rat.gameObject);
					});
					wave.ratsList.Clear();
				}
			});
		});

		//Come back to main menu after 8 seconds from game over
		LeanTween.delayedCall(8f, () =>
		{
			SceneManager.LoadSceneAsync("MainMenu");
		});
	}
}
