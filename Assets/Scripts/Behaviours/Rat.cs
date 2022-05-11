using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : Character
{
	[SerializeField] private float speed;
	[SerializeField] private LayerMask layerMask;


	private void OnTriggerEnter(Collider other)
	{
		Character target = other.GetComponent<Character>();
		if (!targets.Contains(target))
			targets.Add(target);
		attacking = true;
	}

	private void OnTriggerExit(Collider other)
	{
		Character target = other.GetComponent<Character>();
		if (targets.Contains(target))
			targets.Remove(target);
		attacking = false;
	}

	private void Update()
	{
		if (attacking)
		{
			if (nextAttack >= attackInterval)
			{
				SendAttack();
				nextAttack = 0f;
			}
			nextAttack += Time.deltaTime;
		}
		else
			transform.Translate(transform.forward * speed * Time.smoothDeltaTime);
	}

	public void SetAttributes(RatAttributes attr)
	{
		speed = attr.speed;
		attackPower = attr.attackPower;
		initialhealth = health = attr.health;

		GetComponent<Renderer>().material = attr.material;
		transform.localScale = attr.scale;
	}
}
