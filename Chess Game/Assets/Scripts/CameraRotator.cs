using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public GameManager gameManager;
    
    void Update()
    {
        // If game mode is AI we lock the camera
        if (gameManager.gameMode == GameManager.GameMode.AIEasy || gameManager.gameMode == GameManager.GameMode.AIMedium )
        {
            transform.position = new Vector3(1f, 11.98f, 4.37f);
            transform.localEulerAngles = new Vector3(78.0f, -90, 0);
            return; // skip further logic
        }

        // Otherwise, switch position/rotation as needed
        var currentState = gameManager.state;
        switch (currentState)
        {
            case GameManager.GameStates.PlayerTurn1:
                transform.position = new Vector3(1f, 11.98f, 4.37f);
                transform.localEulerAngles = new Vector3(78.0f, -90, 0);
                break;

            case GameManager.GameStates.PlayerTurn2:
                transform.position = new Vector3(-5f, 11.98f, 4.37f);
                transform.localEulerAngles = new Vector3(78.0f, 90, 0);
                break;
        }
    }
}
