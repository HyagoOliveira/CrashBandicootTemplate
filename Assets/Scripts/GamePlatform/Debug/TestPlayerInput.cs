using UnityEngine;


[RequireComponent(typeof(PlayerInput))]
public class TestPlayerInput : MonoBehaviour
{
    public PlayerInput input;

    // Use this for initialization
    void Start()
    {
        input = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < input.buttonsNames.Length; i++)
        {
            if (input.GetButtonDown(input.buttonsNames[i].VirtualName))
                print(input.buttonsNames[i].VirtualName + " pressionado.");
        }

        print("DPad: " + input.GetDpadAxis());
    }
}

