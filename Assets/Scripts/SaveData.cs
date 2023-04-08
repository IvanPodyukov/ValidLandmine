using System;
using System.Collections.Generic;

// Класс сохранённой игры (данных о ней)
[Serializable]
public class SaveData
{
    public float time;
    public int remainingMinesCount;
    public int hintCount;
    public Cell[,] cells;
    public List<Cell> mines;
}
