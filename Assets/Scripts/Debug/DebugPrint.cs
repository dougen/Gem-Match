using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GemMatch
{
    public class DebugPrint : MonoBehaviour
    {
        public GameObject cooPrefab;
        public bool debugFlag;

        public static DebugPrint instance;

        private Transform cooParent;
        private GameObject[,] coos;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            cooParent = GameObject.Find("/UICanvas/DebugPanel").transform;
            coos = new GameObject[GameManager.MAX_ROWS, GameManager.MAX_COLUMNS];
        }

        private void Start()
        {

        }

        private void Update()
        {
            // 实时更新宝石的矩阵位置信息
            if (debugFlag)
            {
                for (var i = 0; i < GameManager.MAX_ROWS; i++)
                {
                    for (var j = 0; j < GameManager.MAX_COLUMNS; j++)
                    {
                        var coo = coos[i, j];
                        coo.GetComponent<Text>().enabled = true;
                        coo.GetComponent<Outline>().enabled = true;
                        var cooText = coo.GetComponent<Text>();
                        var gem = GameManager.instance.gems[i, j];
                        if (gem != null)
                        {
                            cooText.text = $"<color=cyan>[{gem.GetComponent<GemScript>().Row}, {gem.GetComponent<GemScript>().Column}]</color>";
                        }
                        else
                        {
                            cooText.text = "null";
                        }
                    }
                }
            }
            else
            {
                foreach (var coo in coos)
                { 
                    coo.GetComponent<Text>().enabled = false;
                    coo.GetComponent<Outline>().enabled = false;
                }
            }

        }

        public void PrintGemList(List<GameObject> gemList)
        {
            var str = $"BoomList({gemList.Count}): ";
            foreach (var gem in gemList)
            {
                str += gem.tag + "[" + gem.GetComponent<GemScript>().Row + ", " + gem.GetComponent<GemScript>().Column + "];";
            }
            Debug.Log(str);
        }

        public void InitDebugCoo()
        {
            for (var i = 0; i < GameManager.MAX_ROWS; i++)
            {
                for (var j = 0; j < GameManager.MAX_COLUMNS; j++)
                {
                    var coo = Instantiate(cooPrefab);
                    coo.GetComponent<RectTransform>().SetParent(cooParent, false);
                    coo.transform.position = new Vector3(j, i, 0);
                    coos[i, j] = coo;
                    var gem = GameManager.instance.gems[i, j];
                    var cooText = coo.GetComponent<Text>();
                    if (gem != null)
                    {
                        cooText.text = $"<color=cyan>[{gem.GetComponent<GemScript>().Row}, {gem.GetComponent<GemScript>().Column}]</color>";
                    }
                    else
                    {
                        cooText.text = "null";
                    }
                }
            }
        }
    }
}

