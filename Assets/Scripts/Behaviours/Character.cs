using System;
using System.Collections.Generic;
using UnityEngine;

//Base class for all characters
public class Character : MonoBehaviour
{
	//Characters that this one can attack
	public List<Character> targets;
	//Base health
	public float initialhealth = 100f;
	public float health;
	//The amount of damage that this character can give to others
	public float attackPower;
	//Time interval between attacks
	public float attackInterval = 1f;
	public float nextAttack;
	public bool attacking;

	public virtual void Start()
	{
		targets = new List<Character>();
		health = initialhealth;
	}

	//Send attacks to all targets
	public virtual void SendAttack()
	{
		if (targets.FindAll(x => x != null).Count > 0)
			targets.ForEach(x => { if (x != null) x.TakeDamage(attackPower); });
		else
			attacking = false;
	}

	//Receive the damage that other character gave to this one
	public virtual void TakeDamage(float damage)
	{
		health = Mathf.Max(0f, health -= damage);

		if (health <= 0f)
			LeanTween.delayedCall(2f, () =>
			{
				try
				{
					Destroy(gameObject);
				}
				catch (Exception) { }
			});
	}
}
