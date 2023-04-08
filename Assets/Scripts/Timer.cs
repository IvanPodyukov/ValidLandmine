using System;
using UnityEngine;
using UnityEngine.UI;

// Класс таймера
public class Timer : MonoBehaviour
{
    public float time;
    public Text timerText;
    private bool stopped;

    private void Awake()
    {
        time = 0;
        timerText.text = "Прошло времени: 0 c.";
    }

    // Обновление таймера
    private void Update()
    {
        if (!stopped)
        {
            time += Time.deltaTime;
            if (Math.Abs(time - Math.Round(time)) < Time.deltaTime)
            {
                timerText.text = "Прошло времени: " + Math.Round(time) + " c.";
            }
        }
    }

    // Остановка таймера
    public void Stop()
    {
        stopped = true;
    }
    
    // Включение таймера
    public void Continue()
    {
        stopped = false;
    }
}