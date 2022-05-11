using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//This script feeds the inventory of defenses we can use
public class InventoryItemUI : MonoBehaviour
{
	//Displays stock
	public TMP_Text qtyText;
	//Displays the time for the new element to be added to its inventory stock
	[SerializeField] private Image restockProgress;
	//Restock interval
	[SerializeField] private float restockInterval = 5f;
	[SerializeField] public DefenseType defenseType;
	//Time before add the item to the inventory
	private float restockTime;

	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(() => GameManager.SetSelectedDefense(defenseType));
	}

	void Update()
	{
		if (!GameManager.CanPlay) return;

		if (restockTime >= restockInterval)
		{
			qtyText.text = GameManager.DefenseAddInventory(defenseType, 1).ToString();
			restockTime = 0f;
		}
		restockTime += Time.deltaTime;

		restockProgress.fillAmount = restockTime / restockInterval;
	}
}
