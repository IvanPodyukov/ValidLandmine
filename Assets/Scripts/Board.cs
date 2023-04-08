using UnityEngine;
using UnityEngine.Tilemaps;

// Класс игрового поля
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }

    public Tile[] tileUnknown = new Tile[2];
    public Tile[] tileMine = new Tile[2];
    public Tile[] tileExploded = new Tile[2];
    public Tile[] tileFlag = new Tile[2];
    public Tile[] classicTileNums = new Tile[9];
    public Tile[] firstTileNums = new Tile[9];
    private Tile[] tileNums = new Tile[9];

    private int style = 0;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    // Обновление изображения игрового поля в игре
    public void Draw(Cell[,] cells)
    {
        int width = cells.GetLength(0);
        int height = cells.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = cells[x, y];
                tilemap.SetTile(cell.Position, GetTile(cell));
            }
        }
    }

    // Присвоение стиля игрового поля
    public void SetStyle(string style)
    {
        if (style == "classic")
        {
            this.style = 0;
            tileNums = classicTileNums;
        }
        else
        {
            this.style = 1;
            tileNums = firstTileNums;
        }
    }

    private Tile GetTile(Cell cell)
    {
        if (cell.Revealed)
        {
            return GetRevealedTile(cell);
        }

        if (cell.Flagged)
        {
            return tileFlag[style];
        }

        return tileUnknown[style];
    }

    private Tile GetRevealedTile(Cell cell)
    {
        switch (cell.TypeOfCell)
        {
            case Cell.Type.Mine:
                return cell.Exploded? tileExploded[style] : tileMine[style];
            default:
                return tileNums[cell.Number];
        }
    }
}
