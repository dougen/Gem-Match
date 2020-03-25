using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GemMatch
{
    public class Counter : MonoBehaviour
    {
        public int score = 0;

        public int baseScore;

        private Text scoreText;

        // Start is called before the first frame update
        void Start()
        {
            scoreText = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            scoreText.text = score.ToString();
        }

        public void Add(int boomListCount)
        {
            var gm = GameManager.instance;
            for (var i = 0; i <= boomListCount; i++)
            {
                score += baseScore * (i + 1) * gm.multiBoomCount;
            }
        }
    }
}

