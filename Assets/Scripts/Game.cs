using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using FileMode = System.IO.FileMode;
using Random = UnityEngine.Random;
using Task = System.Threading.Tasks.Task;

// Класс игры
public class Game : MonoBehaviour
{
    private int width;
    private int height;
    private int mineCount;
    private int remainingMinesCount;
    private int hintCount;

    public Text timerText;
    public Text hintsText;
    public Text minesText;
    public Text pauseText;
    public Text endGameText;
    public Button hintButton;
    public Button pauseButton;
    public Button restartButton;
    private Board board;
    private Timer timer;
    private Cell[,] cells;
    private Tilemap tilemap;
    private List<Cell> mines;
    private bool gameover;
    private bool newgame;
    private bool pause;
    private string path;

    private void Awake()
    {
        string style = PlayerPrefs.GetString("style");
        board = GetComponentInChildren<Board>();
        board.SetStyle(style);
    }

    private void OnEnable()
    {
        timer = timerText.GetComponent<Timer>();
        tilemap = GetComponentInChildren<Tilemap>();
        newgame = PlayerPrefs.GetInt("newgame") != 0;
        path = Application.persistentDataPath + "/game.dat";
    }
    
    private void OnApplicationQuit()
    {
        if (!gameover)
        {
            SaveGame();
        }
    }

    private void Start()
    {
        if (newgame)
        {
            NewGame();
        }
        else
        {
            LoadGame();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    cells[i, j].Position = new Vector3Int(i, j, 0);
                }
            }
        }

        Camera.main.transform.position = new Vector3(width / 2.5f, height / 2.5f, -10f);
        board.Draw(cells);
    }

    // Настройка объектов интерфейса перед началом игры
    private void SetObjectSettings()
    {
        gameover = false;
        hintsText.text = "Осталось подсказок: " + hintCount;
        minesText.text = "Осталось мин: " + remainingMinesCount;
        pauseText.gameObject.SetActive(false);
        pause = false;
        timer.time = 0;
        timer.Continue();
        endGameText.gameObject.SetActive(false);
        hintButton.gameObject.SetActive(true);
        hintButton.interactable = true;
        pauseButton.interactable = true;
        restartButton.interactable = true;
    }

    // Начало новой игры
    public void NewGame()
    {
        width = PlayerPrefs.GetInt("width");
        height = PlayerPrefs.GetInt("height");
        mineCount = PlayerPrefs.GetInt("mineCount");
        remainingMinesCount = mineCount;
        hintCount = PlayerPrefs.GetInt("hintCount");
        cells = new Cell[width, height];
        mines = new List<Cell>();
        GenerateCells();
        GenerateMines();
        GenerateNumbers();
        SetObjectSettings();
        board.Draw(cells);
    }

    // Загрузка сохранённой игры
    private void LoadGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);
        SaveData data = (SaveData)bf.Deserialize(file);
        file.Close();
        cells = data.cells;
        width = cells.GetLength(0);
        height = cells.GetLength(1);
        mines = data.mines;
        timer.time = data.time;
        remainingMinesCount = data.remainingMinesCount;
        hintCount = data.hintCount;
        SetObjectSettings();
        if (remainingMinesCount == 0)
        {
            hintButton.interactable = false;
        }
    }

    // Сохранение незавершённой игры
    private void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        FileStream file = File.Create(path);
        SaveData data = new SaveData();
        data.cells = cells;
        data.remainingMinesCount = remainingMinesCount;
        data.time = timer.time;
        data.hintCount = hintCount;
        data.mines = mines;
        bf.Serialize(file, data);
        file.Close();
    }

    // Генерация клеток игрового поля
    private void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.Position = new Vector3Int(x, y, 0);
                cell.TypeOfCell = Cell.Type.Empty;
                cells[x, y] = cell;
            }
        }
    }

    // Генерация мин
    private void GenerateMines()
    {
        for (int i = 0; i < mineCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            while (cells[x, y].TypeOfCell == Cell.Type.Mine)
            {
                x = Random.Range(0, width);
                y = Random.Range(0, height);
            }

            cells[x, y].TypeOfCell = Cell.Type.Mine;

            mines.Add(cells[x, y]);
        }
    }

    // Генерация "числовых" клеток
    private void GenerateNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = cells[x, y];

                if (cell.TypeOfCell == Cell.Type.Mine)
                {
                    continue;
                }

                cell.Number = CountMines(x, y);

                if (cell.Number > 0)
                {
                    cell.TypeOfCell = Cell.Type.Number;
                }
            }
        }
    }

    // Подсчет количества мин в соседних клетках для клетки
    private int CountMines(int cellX, int cellY)
    {
        int count = 0;

        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0)
                {
                    continue;
                }

                int x = cellX + adjacentX;
                int y = cellY + adjacentY;

                if (x < 0 || x >= width || y < 0 || y >= height)
                {
                    continue;
                }

                if (cells[x, y].TypeOfCell == Cell.Type.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // Событие update, обработка нажатий
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            BackToMenu();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }

        if (!gameover)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Flag();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Reveal();
            }
        }
    }

    // Переход в главное меню
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainScene");
    }

    // Включение/выключение паузы
    public void Pause()
    {
        Time.timeScale = 1 - Time.timeScale;
        pause = !pause;
        pauseText.gameObject.SetActive(pause);
        tilemap.gameObject.SetActive(!pause);
        hintButton.interactable = !pause;
        restartButton.interactable = !pause;
    }

    // Открытие клетку
    private void Reveal()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.TypeOfCell == Cell.Type.Invalid || cell.Revealed || cell.Flagged)
        {
            return;
        }

        switch (cell.TypeOfCell)
        {
            case Cell.Type.Mine:
                Explode(cell);
                break;
            case Cell.Type.Empty:
                Flood(cell);
                CheckWinCondition();
                break;
            default:
                cell.Revealed = true;
                CheckWinCondition();
                break;
        }

        cell.Revealed = true;
        board.Draw(cells);
    }

    // Открывшаяся клетка оказалась пустой
    private void Flood(Cell cell)
    {
        if (cell.Revealed) return;
        if (cell.TypeOfCell == Cell.Type.Mine || cell.TypeOfCell == Cell.Type.Invalid) return;

        cell.Revealed = true;

        if (cell.TypeOfCell == Cell.Type.Empty)
        {
            Flood(GetCell(cell.Position.x - 1, cell.Position.y));
            Flood(GetCell(cell.Position.x + 1, cell.Position.y));
            Flood(GetCell(cell.Position.x, cell.Position.y - 1));
            Flood(GetCell(cell.Position.x, cell.Position.y + 1));
        }
    }

    // Открывшаяся клетка оказалась заминированной
    private void Explode(Cell cell)
    {
        gameover = true;

        cell.Revealed = true;
        cell.Exploded = true;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = cells[x, y];
                if (cell.TypeOfCell == Cell.Type.Mine)
                {
                    cell.Revealed = true;
                }
            }
        }

        timer.Stop();
        hintButton.gameObject.SetActive(false);
        pauseButton.interactable = false;
        endGameText.text = "Поражение";
        endGameText.color = Color.red;
        endGameText.gameObject.SetActive(true);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    // Пометка клетки как "заминированной"
    private void Flag()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.TypeOfCell == Cell.Type.Invalid || cell.Revealed)
        {
            return;
        }

        if (cell.HintUsed)
        {
            cell.HintUsed = false;
            cell.Flagged = true;
            remainingMinesCount -= 1;
        }
        else
        {
            cell.Flagged = !cell.Flagged;
            if (cell.Flagged)
            {
                remainingMinesCount -= 1;
            }
            else
            {
                remainingMinesCount += 1;
            }
        }

        minesText.text = "Осталось мин: " + remainingMinesCount;
        board.Draw(cells);
    }

    // Проверка на конец игры (победу пользователя)
    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = cells[x, y];
                if (cell.TypeOfCell != Cell.Type.Mine && !cell.Revealed)
                {
                    return;
                }
            }
        }

        gameover = true;
        timer.Stop();
        hintButton.gameObject.SetActive(false);
        pauseButton.interactable = false;
        endGameText.text = "Победа";
        endGameText.color = Color.green;
        endGameText.gameObject.SetActive(true);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = cells[x, y];
                if (cell.TypeOfCell == Cell.Type.Mine)
                {
                    cell.Flagged = true;
                }
            }
        }

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    // Использование подсказки
    public void UseHint()
    {
        hintCount -= 1;
        hintsText.text = "Осталось подсказок: " + hintCount;
        int x = Random.Range(0, mineCount);
        while (mines[x].Flagged)
        {
            x = Random.Range(0, mineCount);
        }

        Cell cell = mines[x];
        cell.HintUsed = true;
        Task.Run(() => ShowHint(cell));
        if (hintCount == 0)
        {
            hintButton.interactable = false;
        }
    }

    // Отображение подсказки на игровом поле
    private void ShowHint(Cell cell)
    {
        while (cell.HintUsed)
        {
            cell.Flagged = !cell.Flagged;
            board.Draw(cells);
            Thread.Sleep(1000);
        }
    }
    
    private Cell GetCell(int x, int y)
    {
        if (IsValid(x, y))
        {
            return cells[x, y];
        }

        return new Cell() { TypeOfCell = Cell.Type.Invalid };
    }

    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < width;
    }
}