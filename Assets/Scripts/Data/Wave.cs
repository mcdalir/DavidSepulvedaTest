using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wave
{
	public int ratsMaxAmount;
	public int ratsCurrentAmount;
	public List<Rat> ratsList;
	public float startingInterval;
	public float minInterval;
	public float intervalDivider;
	//public float delay;
}
