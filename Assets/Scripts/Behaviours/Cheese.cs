using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheese : Character
{
	public float startingHeight;

	public override void Start()
	{
		base.Start();

		startingHeight = transform.localScale.z;
	}

	public override void TakeDamage(float damage)
	{
		base.TakeDamage(damage);

		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, startingHeight * (health / initialhealth));

		if (health <= 0f)
			GameManager.GameOver();
	}
}
