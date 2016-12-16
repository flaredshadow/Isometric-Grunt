using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Item
{
	public string itemName;

	public Attack itemAttack;

	public int amount = 1, purchaseValue;
}

[Serializable]
public class Potion : Item
{
	public Potion()
	{
		itemName = "Test Potion";
		itemAttack = new HealTest();
		purchaseValue = 5;
	}
}

[Serializable]
public class OtherPotion : Item
{
	public OtherPotion()
	{
		itemName = "Other Test Potion";
		itemAttack = new SquirmingClaws();
		purchaseValue = 5;
	}
}
