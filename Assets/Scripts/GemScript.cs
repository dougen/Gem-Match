using UnityEngine;
using UnityEngine.UI;

namespace GemMatch
{
    [RequireComponent(typeof(Animator))]
    public class GemScript : MonoBehaviour
    {
        // 选中后显示选中框
        public Transform selectCursor;
        // 动画播放间隔时间
        public float clipInterval;

        // 宝石在二维矩阵中的位置，跟世界的物理位置无关
        public int Row { get; set; }  // 所在行
        public int Column { get; set; }  // 所在列

        private Animator animator;
        private float timeCount;

        private void Start()
        {
            animator = GetComponent<Animator>();
            timeCount = 0f;
            selectCursor = GameObject.Find("/GemBoard/SelectCursor").transform;
        }

        private void Update()
        {
            timeCount += Time.deltaTime;
            // 达到播放动画时间时，播放动画
            if (timeCount > clipInterval)
            {
                animator.SetTrigger("playing");
                timeCount = 0f;
            }

        }

        /// <summary>
        /// 鼠标点击回调函数
        /// </summary>
        private void OnMouseDown()
        {
            var gm = GameManager.instance.GetComponent<GameManager>();

            // 如果正在交换宝石或者正在消除的时候，不能进行任何操作
            if (gm.swapping || gm.dropping)
            {
                return;
            }

            // 如果已经已经选中一个宝石了，先判断是否在隔壁
            // 如果在隔壁=>直接交换
            // 如果不在或者为空=>换到当前选择的宝石
            switch (gm.SwapGems(gameObject, gm.selectedGem))
            {
                case -2:
                case -1:
                    selectCursor.position = transform.position;
                    if (!selectCursor.GetComponent<SpriteRenderer>().enabled)
                    {
                        selectCursor.GetComponent<SpriteRenderer>().enabled = true;
                    }
                    gm.selectedGem = gameObject;
                    break;
                case 0:
                case 1:
                    gm.selectedGem = null;
                    selectCursor.GetComponent<SpriteRenderer>().enabled = false;
                    break;

            }

        }

        /// <summary>
        /// 在播放完爆炸特效后，销毁自身
        /// </summary>
        public void DestoryGem()
        {
            // gameObject.SetActive(false);
            GemPool.instance.ReturnGem(gameObject);
        }
    }
}
