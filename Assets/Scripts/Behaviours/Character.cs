using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	public List<Character> targets;
	public float initialhealth = 100f;
	public float health;
	public float attackPower;
	public float attackInterval = 1f;
	public float nextAttack;
	public bool attacking;

	public virtual void Start()
	{
		targets = new List<Character>();
		health = initialhealth;
	}

	public virtual void SendAttack()
	{
		if (targets.FindAll(x => x != null).Count > 0)
			targets.ForEach(x => { if (x != null) x.TakeDamage(attackPower); });
		else
			attacking = false;
	}

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
