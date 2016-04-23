using UnityEngine;
using System.Collections;

public interface IInventory
{
	void AddItem (CollectableItemType type);
	void AddLife ();
	void AddFruit ();
	void AddCrystal ();
	void AddGem ();

	void RemoveLife ();

	void ShowInventory ();
	void HideInventory ();

}
