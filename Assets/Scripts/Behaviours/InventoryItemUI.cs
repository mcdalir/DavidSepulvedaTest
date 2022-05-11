using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
	public TMP_Text qtyText;
	[SerializeField] private Image restockProgress;

	[SerializeField] private float restockInterval = 5f;
	[SerializeField] public DefenseType defenseType;
	[SerializeField] private float restockTime;

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
