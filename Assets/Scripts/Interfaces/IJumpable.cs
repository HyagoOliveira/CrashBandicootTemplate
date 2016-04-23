using UnityEngine;
using System.Collections;

public interface IJumpable
{
	void OnJump ();
	void OnHead ();
	float GetJumpHeight ();
}
