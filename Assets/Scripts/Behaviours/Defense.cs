using System;
using System.Collections.Generic;
using UnityEngine;

public class Defense : Character
{
	//[SerializeField] private Transform rod;
	[SerializeField] private DefenseType defenseType;
	[SerializeField] private List<DefenseGraphics> defenseGraphics;

	private bool initialized;
	private new Collider collider;


	public void Initialize(DefenseAttributes attributes)
	{
		defenseType = attributes.defenseType;
		health = attributes.health;
		attackPower = attributes.attackPower;
		attackInterval = attributes.attackInterval;
		initialized = true;
		collider = GetComponent<Collider>();

		DefenseGraphics found = defenseGraphics.Find(x => x.defenseType == attributes.defenseType);

		if (found != null)
			found.armed.SetActive(true);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!initialized) return;

		//Only for traps: it attacks only the first rat trapped
		if (defenseType == DefenseType.Trap && targets.Count > 0) return;

		Character character = other.GetComponent<Character>();

		DefenseGraphics found = defenseGraphics.Find(x => x.defenseType == defenseType);

		if (character != null)
		{
			targets.Add(character);

			if (found.activated != null)
			{
				found.armed.SetActive(false);
				found.activated.SetActive(true);
			}

			attacking = true;
			nextAttack = attackInterval;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!initialized) return;

		if (attacking)
		{
			if (nextAttack >= attackInterval)
			{
				SendAttack();
				nextAttack = 0f;
			}
			else
				nextAttack += Time.deltaTime;
		}
	}
}
