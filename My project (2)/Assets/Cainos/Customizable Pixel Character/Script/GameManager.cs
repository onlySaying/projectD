using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public int stageIndex;
    public GameObject[] Stages;
    public Text UIStage;
    public Button UIRestartBtn;
    public Cainos.CustomizablePixelCharacter.PixelCharacterController player;
    //public Player player;


    // Start is called before the first frame update

    public void NextStage()
    {
        if (stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE" + (stageIndex + 1).ToString();
        }
        else
        {
            Time.timeScale = 0;
            Debug.Log("게임 클리어");

            Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            UIRestartBtn.gameObject.SetActive(true);
        }

    }

    public void dead()
    {
        Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
        btnText.text = "restart!";
        UIRestartBtn.gameObject.SetActive(true);
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }


    void PlayerReposition()
    {
        player.transform.position = new Vector3(-1, 3, -1);
        player.VelocityZero();
    }


}