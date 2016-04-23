using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Canvas))]
public class CrashInventory : MonoBehaviour, IInventory
{
	public Sprite[] fontNumbers;
	public Image[] fruitDigits;
	public Image[] livesDigits;

	public static CrashInventory Instance { get; private set; }
	private Animator animator;

	private int fruits = 0;
	private int lives = 0;

	void Start ()
	{
		Instance = this;
		animator = GetComponent<Animator> ();

		fruitDigits [1].enabled = false;
		livesDigits [0].enabled = false;
	}

	#region IInventory implementation

	public void AddItem (CollectableItemType type)
	{
		switch (type) {
		case CollectableItemType.Life:
			AddLife ();
			break;

		case CollectableItemType.WumpaFruit:
			AddFruit ();
			break;

		case CollectableItemType.Crystal:
			AddCrystal ();
			break;

		case CollectableItemType.Gem:
			AddGem ();
			break;
		}
	}

	public void AddLife ()
	{
		//Platform3DActor.Instance.audioProvider.Play ("life_up");

		lives ++;
		ChangeLifeHUD ();		
	}


	public void RemoveLife ()
	{
		lives--;
		ChangeLifeHUD ();
	}

	void ChangeLifeHUD ()
	{
		animator.SetTrigger ("lifeup");
		lives = Mathf.Clamp (lives, 0, 99);
		if (lives < 10) {
			livesDigits [0].enabled = false;
			livesDigits [1].enabled = true;
			livesDigits [1].sprite = fontNumbers [lives];
		} else
			if (lives < 100) {
			livesDigits [0].enabled = true;
			livesDigits [1].enabled = true;
			livesDigits [0].sprite = fontNumbers [lives / 10];
			livesDigits [1].sprite = fontNumbers [lives % 10];
		}
	}


	public void AddFruit ()
	{
//		if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("In"))
		animator.SetTrigger ("fruitup");

		//Platform3DActor.Instance.audioProvider.Play ("wumpa_eat");
		fruits++;

		if (fruits < 10) {
			fruitDigits [0].enabled = true;
			fruitDigits [1].enabled = false;

			fruitDigits [0].sprite = fontNumbers [fruits];
		} else if (fruits < 100) {
			fruitDigits [0].enabled = true;
			fruitDigits [1].enabled = true;

			fruitDigits [0].sprite = fontNumbers [fruits / 10];
			fruitDigits [1].sprite = fontNumbers [fruits % 10];
		} else {
			fruits = 0;

			fruitDigits [0].enabled = true;
			fruitDigits [1].enabled = false;
			
			fruitDigits [0].sprite = fontNumbers [fruits];

			AddLife ();
		}
	}

	public void AddCrystal ()
	{
		throw new System.NotImplementedException ();
	}

	public void AddGem ()
	{
		throw new System.NotImplementedException ();
	}



	public void ShowInventory ()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("In"))
			return;

		animator.SetTrigger ("show");
	}

	public void HideInventory ()
	{
		throw new System.NotImplementedException ();
	}
	#endregion
}
