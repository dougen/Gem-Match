using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace GemMatch
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public const int MAX_ROWS = 8;    // 宝石矩阵的最大行数
        public const int MAX_COLUMNS = 8;  // 宝石矩阵的最大列数

        public GameObject gemBoard;
        public Counter counter;

        [HideInInspector] public GameObject[,] gems;
        [HideInInspector] public GameObject selectedGem;
        [HideInInspector] public bool swapping; // 是否正在交换状态，默认为false
        [HideInInspector] public bool dropping; // 是否正在下落状态，默认为false
        [HideInInspector] public int multiBoomCount;

        private List<GameObject> checkList;   // 该列表保存当前需要检查的宝石
        private List<GameObject> boomList;  // 包含需要消除的宝石列表
        
        private AudioSource audioSource;

        private void Awake()
        {
            // 单例模式
            if (instance != this)
            {
                instance = this;
            }
            // 最开始时 没有选择任何宝石
            selectedGem = null;
            swapping = false;
            dropping = false;
            checkList = new List<GameObject>();
            boomList = new List<GameObject>();
            multiBoomCount = 0;
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            // 更新宝石的坐标
            UpdateGemsCoordinate();

            // 判断是否需要检测,只有当checkList.Count != 0， swapping == false 的时候才进行检测
            // 否则直接跳过
            if (checkList.Count > 0 && !swapping && !dropping)
            {
                CheckBoomGems();
            }

            // 判断是否需要爆炸
            if (boomList.Count > 0 && !swapping && !dropping)
            {
                BoomGems();
            }



        }

        /// <summary>
        /// 自动更新每个宝石的坐标位置
        /// </summary>
        private void UpdateGemsCoordinate()
        {
            for (var i = 0; i < MAX_ROWS; i++)
            {
                for (var j = 0; j < MAX_COLUMNS; j++)
                {
                    if (gems[i, j] == null)
                    {
                        continue;
                    }
                    gems[i, j].GetComponent<GemScript>().Row = i;
                    gems[i, j].GetComponent<GemScript>().Column = j;
                }
            }
        }

        /// <summary>
        /// 检测移动宝石之后是否需要消除
        /// </summary>
        private bool CheckBoomGems()
        {
            // 检测列表里面有内容，则开始检测是否可以消除
            foreach (var gem in checkList)
            {
                var gs = gem.GetComponent<GemScript>();
                // 检测当前宝石所在的行和列是否有相同且相连的宝石
                var rowList = new List<GameObject>();
                var columnList = new List<GameObject>();

                // 检测左边
                for (var i = gs.Column; i >= 0; i--)
                {
                    if (gems[gs.Row, i] == null)
                    {
                        break;
                    }

                    if (gems[gs.Row, i].tag == gs.tag)
                    {
                        rowList.Add(gems[gs.Row, i]);
                    }
                    else
                    {
                        break;
                    }
                }

                // 检测右边
                for (var i = gs.Column + 1; i < MAX_COLUMNS; i++)
                {
                    if (gems[gs.Row, i] == null)
                    {
                        break;
                    }

                    if (gems[gs.Row, i].tag == gs.tag)
                    {
                        rowList.Add(gems[gs.Row, i]);
                    }
                    else
                    {
                        break;
                    }
                }

                // 检测下面
                for (var i = gs.Row; i >= 0; i--)
                {
                    if (gems[i, gs.Column] == null)
                    {
                        break;
                    }

                    if (gems[i, gs.Column].tag == gs.tag)
                    {
                        columnList.Add(gems[i, gs.Column]);
                    }
                    else
                    {
                        break;
                    }
                }

                // 检测下面
                for (var i = gs.Row + 1; i < MAX_ROWS; i++)
                {
                    if (gems[i, gs.Column] == null)
                    {
                        break;
                    }

                    if (gems[i, gs.Column].tag == gs.tag)
                    {
                        columnList.Add(gems[i, gs.Column]);
                    }
                    else
                    {
                        break;
                    }
                }

                if (rowList.Count > 2)
                {
                    foreach (var g in rowList)
                    {
                        if (!boomList.Contains(g))
                        {
                            boomList.Add(g);
                        }
                    }
                }

                if (columnList.Count > 2)
                {
                    foreach (var g in columnList)
                    {
                        if (!boomList.Contains(g))
                        {
                            boomList.Add(g);
                        }
                    }
                }

            }

            // 检测完成后移除所有检测过的宝石
            checkList.Clear();

            // 判断是否可以消除
            if (boomList.Count > 2)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将boomlist列表中的宝石引爆
        /// </summary>
        private void BoomGems()
        {
            DebugPrint.instance.PrintGemList(boomList);
            foreach (var gem in boomList)
            {
                var i = gem.GetComponent<GemScript>().Row;
                var j = gem.GetComponent<GemScript>().Column;
                gems[i, j] = null;
                gem.GetComponent<Animator>().SetTrigger("boom");
            }
            // 统计连消次数
            multiBoomCount++;
            // 爆炸时计分
            counter.Add(boomList.Count);
            // 播放音乐
            audioSource.Play();
            boomList.Clear();
            GemsDrop();
        }

        /// <summary>
        /// 找到所有需要掉下去的宝石，并且让它们动起来
        /// </summary>
        private void GemsDrop()
        {
            // 寻找每列中为null的格子，计算需要下落的距离
            for (var j = 0; j < MAX_COLUMNS; j++)
            {
                var count = 0;  // 统计该列有多少空格子

                for (var i = 0; i < MAX_ROWS; i++)
                {
                    if (gems[i, j] == null)
                    {
                        count++;
                    }
                    else
                    {
                        if (count > 0)
                        {
                            var dropGem = gems[i, j];
                            gems[i - count, j] = dropGem;
                            gems[i, j] = null;
                            checkList.Add(dropGem);
                            SingleGemDrop(dropGem, count);
                        }
                    }
                }

                // 当计算出该列有多少颗宝石掉下去时，立即补充新的宝石进来
                for (var i = 0; i < count; i++)
                {
                    var newGem = GemPool.instance.GetRandomGem();
                    newGem.transform.SetParent(gemBoard.transform);
                    newGem.transform.position = new Vector3(j, MAX_ROWS + i, 0f);
                    var row = MAX_ROWS - count + i;
                    gems[row, j] = newGem;
                    checkList.Add(newGem);
                    SingleGemDrop(newGem, count);
                }
            }
        }

        /// <summary>
        /// 控制某一宝石下落
        /// </summary>
        /// <param name="gem">下落的宝石</param>
        /// <param name="dropStep">下落的距离</param>
        private void SingleGemDrop(GameObject gem, int dropStep)
        {
            // 只要还有任意宝石在掉落，dropping 都为true
            dropping = true;
            var tween = gem.transform.DOMoveY(gem.transform.position.y - dropStep, 0.5f, false);
            tween.SetEase(Ease.InCubic).OnComplete(() => dropping = false);
        }

        /// <summary>
        /// 交换两个相邻的宝石
        /// </summary>
        /// <param name="gem1">交换的宝石1</param>
        /// <param name="gem2">交换的宝石2</param>
        /// <returns>交换成功返回1，不能交换返回0，距离太远返回-1，参数为空返回-2</returns>
        public int SwapGems(GameObject gem1, GameObject gem2)
        {
            // 连消次数归零
            multiBoomCount = 0;
            int returnInt = 1;  // 默认交换成功

            // 宝石不能为null
            if (gem1 != null && gem2 != null)
            {
                // 计算宝石间的距离
                var gs1 = gem1.GetComponent<GemScript>();
                var gs2 = gem2.GetComponent<GemScript>();
                var disX = Mathf.Abs(gs1.Row - gs2.Row);
                var disY = Mathf.Abs(gs1.Column - gs2.Column);
                // disx + disy = 1则可以认为这两个宝石的相对位置在上下左右其中一个
                if ((disX + disY) == 1)
                {
                    Swap(gs1, gs2);

                    var gem1Seq = DOTween.Sequence();
                    var gem2Seq = DOTween.Sequence();

                    // gem1 gem2 相互交换
                    swapping = true;

                    var gem1Tween = gem1.transform.DOMove(new Vector2(gs1.Column, gs1.Row), 0.5f, false);
                    gem1Tween.SetEase(Ease.InOutQuart);
                    gem1Seq.Append(gem1Tween);

                    var gem2Tween = gem2.transform.DOMove(new Vector2(gs2.Column, gs2.Row), 0.5f, false);
                    gem2Tween.SetEase(Ease.InOutQuart);
                    gem2Seq.Append(gem2Tween);

                    // 交换完成后需要检测交换的两个宝石是否满足消除条件
                    checkList.Add(gem1);
                    checkList.Add(gem2);

                    // 强制进行检测是否可以消除
                    // 如果不能交换则又重新换回来
                    if (!CheckBoomGems())
                    {
                        checkList.Clear();
                        Swap(gs1, gs2);
                        // gem1 gem2 相互交换
                        var tween1 = gem1.transform.DOMove(new Vector2(gs1.Column, gs1.Row), 0.5f, false);
                        tween1.SetEase(Ease.InOutQuart);
                        gem1Seq.Append(tween1);
                        var tween2 = gem2.transform.DOMove(new Vector2(gs2.Column, gs2.Row), 0.5f, false);
                        tween2.SetEase(Ease.InOutQuart);
                        gem2Seq.Append(tween2);
                        returnInt = 0;  // 交换失败
                    }
                    gem2Seq.OnComplete(() => swapping = false);
                    gem1Seq.Play();
                    gem2Seq.Play();
                }
                else
                {
                    returnInt = -1; // 距离太长
                }
            }
            else
            {
                returnInt = -2; // 参数为空
            }

            return returnInt;

        }
        

        private void Swap(GemScript gs1, GemScript gs2)
        {
            // 二维数组的位置也要对应改变
            gems[gs1.Row, gs1.Column] = gs2.gameObject;
            gems[gs2.Row, gs2.Column] = gs1.gameObject;

            // 由于gem的row和column字段只会在 update 期间自动更新，所以要马上调用 准确的 row 和 column 的话就得立即手动更新
            var temp = gs1.Row;
            gs1.Row = gs2.Row;
            gs2.Row = temp;

            temp = gs1.Column;
            gs1.Column = gs2.Column;
            gs2.Column = temp;
        }
    }
}
