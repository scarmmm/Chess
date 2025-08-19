using UnityEngine;

public class UI_EscapeMenu : GameOver
{
    private bool isActive = false;
    [SerializeField] private GameObject uiMenuEscape;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isActive = !isActive;
            uiMenuEscape.SetActive(isActive);
            Debug.Log("Escape pressed");
        }
    }

    public void Resume()
    {
        if (isActive)
        {
            isActive = false;
            uiMenuEscape.SetActive(isActive);
        }
    }
}
