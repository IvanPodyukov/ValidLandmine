using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Application = UnityEngine.Application;

// Класс меню
public class Menu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject levelMenu;
    public GameObject optionsMenu;
    public Button continueGameButton;
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Dropdown styleDropdown;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("style"))
        {
            PlayerPrefs.SetString("style", "classic");
        }
        
        continueGameButton.interactable = true;
        if (!File.Exists(Application.persistentDataPath + "/game.dat"))
        {
            continueGameButton.interactable = false;
        }
    }

    // Переход в меню выбора уровня игры
    public void Play()
    {
        mainMenu.SetActive(false);
        levelMenu.SetActive(true);
    }

    // Переход в окно сохранённой игры
    public void ContinueGame()
    {
        PlayerPrefs.SetInt("newgame", 0);
        SceneManager.LoadScene("GameScene");
    }

    // Переход в меню настроек
    public void Options()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        if (Screen.width == 1600)
        {
            resolutionDropdown.value = 0;
        }
        else if (Screen.width == 1920)
        {
            resolutionDropdown.value = 1;
        }
        else
        {
            resolutionDropdown.value = 2;
        }

        if (PlayerPrefs.HasKey("style"))
        {
            styleDropdown.value = PlayerPrefs.GetString("style") == "classic" ? 0 : 1;
        }
        else
        {
            styleDropdown.value = 0;
        }

        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    // Возвращение в главное меню из меню выбора уровня игры
    public void BackFromLevelMenu()
    {
        mainMenu.SetActive(true);
        levelMenu.SetActive(false);
    }
    
    // Возвращение в главное меню из меню настроек
    public void BackFromOptionsMenu()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    // Начало новой игры с уровнем сложности "очень лёгкий"
    public void StartGameVeryEasy()
    {
        PlayerPrefs.SetInt("newgame", 1);
        PlayerPrefs.SetInt("width", 5);
        PlayerPrefs.SetInt("height", 5);
        PlayerPrefs.SetInt("mineCount", 2);
        PlayerPrefs.SetInt("hintCount", 1);
        SceneManager.LoadScene("GameScene");
    }

    // Начало новой игры с уровнем сложности "лёгкий"
    public void StartGameEasy()
    {
        PlayerPrefs.SetInt("newgame", 1);
        PlayerPrefs.SetInt("width", 8);
        PlayerPrefs.SetInt("height", 8);
        PlayerPrefs.SetInt("mineCount", 10);
        PlayerPrefs.SetInt("hintCount", 3);
        SceneManager.LoadScene("GameScene");
    }

    // Начало новой игры с уровнем сложности "средний"
    public void StartGameMedium()
    {
        PlayerPrefs.SetInt("newgame", 1);
        PlayerPrefs.SetInt("width", 16);
        PlayerPrefs.SetInt("height", 16);
        PlayerPrefs.SetInt("mineCount", 40);
        PlayerPrefs.SetInt("hintCount", 5);
        SceneManager.LoadScene("GameScene");
    }

    // Начало новой игры с уровнем сложности "сложный"
    public void StartGameHard()
    {
        PlayerPrefs.SetInt("newgame", 1);
        PlayerPrefs.SetInt("width", 30);
        PlayerPrefs.SetInt("height", 16);
        PlayerPrefs.SetInt("mineCount", 99);
        PlayerPrefs.SetInt("hintCount", 8);
        SceneManager.LoadScene("GameScene");
    }

    // Выход из приложения
    public void Exit()
    {
        Application.Quit();
    }

    // Включение/выключение полноэкранного режима
    public void ChangeFullScreen()
    {
        if (fullscreenToggle.isOn)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
        }
    }

    // Смена разрешения экрана
    public void ChangeResolution()
    {
        if (resolutionDropdown.value == 0)
        {
            Screen.SetResolution(1600, 900, Screen.fullScreen);
        }
        else if (resolutionDropdown.value == 1)
        {
            Screen.SetResolution(1920, 1080, Screen.fullScreen);
        }
        else if (resolutionDropdown.value == 2)
        {
            Screen.SetResolution(2560, 1440, Screen.fullScreen);
        }
    }

    // Смена стиля игрового поля
    public void ChangeStyle()
    {
        if (styleDropdown.value == 0)
        {
            PlayerPrefs.SetString("style", "classic");
        }
        else
        {
            PlayerPrefs.SetString("style", "style1");
        }
    }
}