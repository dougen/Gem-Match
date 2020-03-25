using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private Text time;
    private int min;
    private int sec;
    private float temp;
    // Start is called before the first frame update
    void Start()
    {
        time = GetComponent<Text>();
        time.text = "00:00";
        min = 0;
        sec = 0;
        temp = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // 每秒进一
        if (temp > 1)
        {
            temp = 0;
            sec++;
            if (sec > 60)
            {
                sec = 0;
                min++;
            }

            // 保持时间 00:00 的格式
            string timeStr;

            if (min < 10)
            {
                timeStr = $"0{min}:";
            }
            else
            {
                timeStr = $"{min}:";
            }

            if (sec < 10)
            {
                timeStr += $"0{sec}";
            }
            else
            {
                timeStr += $"{sec}";
            }

            time.text = timeStr;
        }

        temp += Time.deltaTime;
    }
}
