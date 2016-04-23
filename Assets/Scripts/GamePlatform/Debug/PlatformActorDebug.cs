using UnityEngine;

[RequireComponent(typeof(PlatformActor))]
public class PlatformActorDebug : MonoBehaviour
{
	private PlatformActor playerActor;

	private float timeScale = 1.0f;
    private float deltaTime;



    void Start ()
	{
		playerActor = GetComponent<PlatformActor> ();
        deltaTime = Time.deltaTime;
    }

    private void Update()
    {
        playerActor.enabled = !playerActor.input.GetButton("Right Shoulder");
        if (!playerActor.enabled)
        {
            if (playerActor.vspeed < 0)
                playerActor.StopVelocity();

            Vector2 DPADinput = playerActor.input.GetDpadAxis();

            if (DPADinput.sqrMagnitude > 0)
            {
                Vector3 direction = DPADinput.x *
                    Camera.main.transform.right + DPADinput.y * Vector3.up;

                playerActor.UpdateMovement(direction * playerActor.speed);
            }
            else
            {
                playerActor.UpdateGroundedHorizontalMovement();
                playerActor.UpdateMovement();
            }
        }

        Vector2 triggerInput = playerActor.input.GetTriggers();

        if (triggerInput.sqrMagnitude > 0f)
        {
            timeScale -= triggerInput.x * deltaTime;
            timeScale += triggerInput.y * deltaTime;
        }

        timeScale = Mathf.Clamp(timeScale, 0.01f, 1f);
        Time.timeScale = timeScale;
    }

    void OnGUI ()
	{
		float topY = Screen.height - 100;

		GUI.Box (new Rect (10, topY + 10, 200, 80), "Actor Machine");

		GUI.TextField (new Rect (20, topY + 40, 180, 20), 
		               string.Format ("State: {0}", playerActor.StateController.CurrentActorState));
		timeScale = GUI.HorizontalSlider (new Rect (20, topY + 70, 180, 20), timeScale, 0.01f, 1.0f);
	}
}
