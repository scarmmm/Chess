using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public GameManager gameManager;
    
    void Update()
    {
        var currentState = gameManager.state;
        switch (currentState)
        {
            case GameManager.GameStates.PlayerTurn1:
                //transform.position = new Vector3(-0.75999999f, 0.349999994f, 3.97799993f);
                transform.position = new Vector3(1f, 11.98f, 4.37f);
                //transform.localEulerAngles = new Vector3(34.953f, -90, 0);
                transform.localEulerAngles = new Vector3(78.0f, -90, 0);
                break;

            case GameManager.GameStates.PlayerTurn2:
                //transform.position = new Vector3(-0.75999999f, 0.349999994f, 3.97799993f);
                transform.position = new Vector3(-5f, 11.98f, 4.37f);
                //transform.localEulerAngles = new Vector3(34.953f, -90, 0);
                transform.localEulerAngles = new Vector3(78.0f, 90, 0);
                break;
        }
    }

    
}