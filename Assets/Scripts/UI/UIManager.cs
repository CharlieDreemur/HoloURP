using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
// A singleton class that manages all UI Events
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextIndicator turnIndicator;
    public TextIndicator messageIndicator;
    public GameObject holdCircle;
    public GameObject winPanel;
    public GameObject losePanel;
    public InputControls inputControls;
    public GameObject tutorialPanel;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public void ShowMessage(string text)
    {
        messageIndicator.AnimateText(text);
    }
    public void ShowTurnText()
    {
        turnIndicator.AnimateText("Your Turn");
        tutorialPanel.transform.DOScale(Vector3.one, 0.5f).OnComplete(() => tutorialPanel.SetActive(true));
    }
    public void EndTurn()
    {
        holdCircle.gameObject.SetActive(false);
        tutorialPanel.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => tutorialPanel.SetActive(false));
    }


    public void WinGame()
    {
        winPanel.SetActive(true);
        AudioManager.Instance.Play("gamewin");
        //if hold R, restart the game
        inputControls.Player.EndTurn.performed += ctx =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        };
    }
    public void LoseGame()
    {
        losePanel.SetActive(true);
        AudioManager.Instance.Play("gamelose");
        {
            inputControls.Player.EndTurn.performed += ctx =>
       {
           SceneManager.LoadScene(SceneManager.GetActiveScene().name);
       };
        };
    }


}
