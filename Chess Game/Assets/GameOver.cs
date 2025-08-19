using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
   public Text teamIDValue; 
   public virtual void Setup(int teamID)
   {
      gameObject.SetActive(true);
      teamIDValue.text = teamID == 1 ? "Team 2 Wins" : "Team 1 Wins";
   }

   public virtual void Restart()
   {
      SceneManager.LoadScene("SampleScene");
      Debug.Log("Restart button activated");
   }

   public virtual void Quit()
   {
      Debug.Log("Quit called");
      Application.Quit();
   }
}
