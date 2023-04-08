using System;
using System.Runtime.Serialization;
using UnityEngine;

// Класс клетки игрового поля
[Serializable]
public class Cell
{
    [Serializable]
    [Flags]
    public enum Type
    {
        [EnumMember] Invalid,
        [EnumMember] Empty,
        [EnumMember] Mine,
        [EnumMember] Number,
    }

    [NonSerialized] private Vector3Int position;
    private Type typeOfCell;
    private int number;
    private bool revealed;
    private bool flagged;
    private bool exploded;
    private bool hintUsed;

    public Vector3Int Position
    {
        get => position;
        set => position = value;
    }

    public Type TypeOfCell
    {
        get => typeOfCell;
        set => typeOfCell = value;
    }

    public int Number
    {
        get => number;
        set => number = value;
    }

    public bool Revealed
    {
        get => revealed;
        set => revealed = value;
    }

    public bool Flagged
    {
        get => flagged;
        set => flagged = value;
    }

    public bool Exploded
    {
        get => exploded;
        set => exploded = value;
    }

    public bool HintUsed
    {
        get => hintUsed;
        set => hintUsed = value;
    }
}