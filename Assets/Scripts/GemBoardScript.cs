using UnityEngine;
using DG.Tweening;

namespace GemMatch
{
    public class GemBoardScript : MonoBehaviour
    {
        public GameObject[] gemPrefabs;   // 所有宝石的prefabs
        public Vector3 startPos;   // 初始宝石从左上角开始

        [HideInInspector] public GameObject[,] gems;    // 所有宝石组成的矩阵

        private void Awake()
        {
            DOTween.Init();
            DOTween.SetTweensCapacity(200, 100);
        }

        private void Start()
        {
            gems = new GameObject[GameManager.MAX_ROWS, GameManager.MAX_COLUMNS];
            InitSwap();
            GameManager.instance.gems = gems;

            DebugPrint.instance.InitDebugCoo();

        }

        /// <summary>
        /// 宝石矩阵初始化
        /// </summary>
        private void InitSwap()
        {
            for (var i = 0; i < GameManager.MAX_ROWS; ++i)
            {
                for (var j = 0; j < GameManager.MAX_COLUMNS; ++j)
                {
                    string existTagLeft = "";
                    string existTagBlow = "";

                    // 判断左边有没有2个相同的宝石,如果有，则把该宝石从随机素组中移除
                    if (j > 1 && gems[i, j - 1].tag == gems[i, j - 2].tag)
                    {
                        existTagLeft = gems[i, j - 1].tag;
                    }
                    // 判断下面有没有2个相同的宝石
                    if (i > 1 && gems[i - 1, j].tag == gems[i - 2, j].tag)
                    {
                        existTagBlow = gems[i - 1, j].tag;
                    }

                    var gem = GemPool.instance.GetRandomGem(existTagLeft, existTagBlow);
                    gem.transform.position = new Vector3(startPos.x + j, startPos.y + i, startPos.z);
                    gem.transform.SetParent(transform, true);
                    gems[i, j] = gem;
                    InitAnimation(gem);
                }
            }
        }

        /// <summary>
        /// 传入一个新生成的宝石对象，对其进行操作实现生成动画。
        /// </summary>
        /// <param name="gem">宝石对象</param>
        private void InitAnimation(GameObject gem)
        {
            gem.transform.localScale = new Vector3(0f, 0f, 0f);
            gem.transform.DOScale(1.0f, 0.5f);
        }
    }
}


