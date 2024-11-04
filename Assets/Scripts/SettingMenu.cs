using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingMenu : MonoBehaviour
{
    public enum ScreenMode
    {
        Windowed,
        Fullscreen,
        Borderless
    }
    public static SettingMenu Instance;
    private ScreenMode currentMode;
    private bool isExpanded = false;
    private float timeScale = 1.0f;
    private bool isPaused = false;
    void Awake()
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
    void Start()
    {
        // Load the saved screen mode or set default to Windowed
        currentMode = (ScreenMode)PlayerPrefs.GetInt("ScreenMode", (int)ScreenMode.Windowed);
        SetScreenMode(currentMode);

        // Set initial timescale
        Time.timeScale = timeScale;
    }
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        isPaused = false;
    }

    void OnGUI()
    {
        // Main toggle button to expand/minimize the settings menu
        if (GUI.Button(new Rect(10, 10, 200, 30), isExpanded ? "Settings ▲" : "Settings ▼"))
        {
            // Toggle expand/minimize state
            isExpanded = !isExpanded;
        }

        // Show additional options if the menu is expanded
        if (isExpanded)
        {
            GUILayout.BeginArea(new Rect(10, 50, 200, 200));
            GUILayout.Label("Screen Mode:");

            // Windowed Button
            if (GUILayout.Button("Windowed"))
            {
                SetScreenMode(ScreenMode.Windowed);
            }

            // Fullscreen Button
            if (GUILayout.Button("Fullscreen"))
            {
                SetScreenMode(ScreenMode.Fullscreen);
            }

            // Borderless Button
            if (GUILayout.Button("Borderless"))
            {
                SetScreenMode(ScreenMode.Borderless);
            }

            // Restart Game Button
            if (GUILayout.Button("Restart Game"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            // Time Scale Slider
            GUILayout.Label("Animation Speed: " + timeScale.ToString("F2"));
            timeScale = GUILayout.HorizontalSlider(timeScale, 1f, 3f);
            if(isPaused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = timeScale;
            }
            GUILayout.EndArea();
        }
    }

    public void SetScreenMode(ScreenMode mode)
    {
        // Set the screen mode based on the chosen option
        switch (mode)
        {
            case ScreenMode.Windowed:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;

            case ScreenMode.Fullscreen:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;

            case ScreenMode.Borderless:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
        }

        // Update the current mode and save it
        currentMode = mode;
        PlayerPrefs.SetInt("ScreenMode", (int)mode);
    }
}
