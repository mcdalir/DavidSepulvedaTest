using System;
using System.Collections.Generic;
using UnityEngine;

//Character that we can use to protect the cheese
public class Defense : Character
{
	[SerializeField] private DefenseType defenseType;
	//Graphics to be displayed regarding defenseType
	[SerializeField] private List<DefenseGraphics> defenseGraphics;

	//Initialize this character and set its attributes
	private bool initialized;
	public void Initialize(DefenseAttributes attributes)
	{
		defenseType = attributes.defenseType;
		health = attributes.health;
		attackPower = attributes.attackPower;
		attackInterval = attributes.attackInterval;
		initialized = true;

		DefenseGraphics found = defenseGraphics.Find(x => x.defenseType == attributes.defenseType);

		if (found != null)
			found.armed.SetActive(true);
	}

	//Detect an upcoming enemy
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

	//Attack listed enemies while there are at least one colliding with this character
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
