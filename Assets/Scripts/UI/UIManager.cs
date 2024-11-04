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
    public GameObject drawOpponentTutorialPanel;
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
        inputControls = new InputControls();
        drawOpponentTutorialPanel.transform.localScale = Vector3.zero;
    }
    public void ShowMessage(string text)
    {
        messageIndicator.AnimateText(text);
    }
    public void ShowDrawOpponentTutorial()
    {
        drawOpponentTutorialPanel.SetActive(true);
        drawOpponentTutorialPanel.transform.DOScale(Vector3.one, 0.5f).OnComplete(() => drawOpponentTutorialPanel.SetActive(true));
    }

    public void HideDrawOpponentTutorial()
    {
        drawOpponentTutorialPanel.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => drawOpponentTutorialPanel.SetActive(false));
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
        SettingMenu.Instance.PauseGame();
        winPanel.SetActive(true);
        AudioManager.Instance.Play("gamewin");
        //if hold R, restart the game
        inputControls.Player.Restart.performed += RestartGame;
        inputControls.Player.Restart.Enable();
    }
    public void LoseGame()
    {
        SettingMenu.Instance.PauseGame();
        losePanel.SetActive(true);
        AudioManager.Instance.Play("gamelose");
        inputControls.Player.Restart.performed += RestartGame;
        inputControls.Player.Restart.Enable();

    }

    private void RestartGame(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        inputControls.Player.Restart.performed -= RestartGame;
        inputControls.Player.Restart.Disable();  
        SettingMenu.Instance.ResumeGame();
    }


}
