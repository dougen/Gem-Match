using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GemMatch
{
    public class GemPool : MonoBehaviour
    {
        public static GemPool instance;

        public GameObject[] gemPrefabs;

        private Dictionary<string, Queue<GameObject>> pool;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            pool = new Dictionary<string, Queue<GameObject>>();
        }

        /// <summary>
        /// 从对象池中获取宝石对象
        /// </summary>
        /// <param name="gemTag">宝石类型</param>
        /// <returns>如果输入的tag不是宝石类型，则返回null</returns>
        public GameObject GetGem(string gemTag)
        {
            if (pool.ContainsKey(gemTag) && pool[gemTag].Count > 0)
            {
                var gem = pool[gemTag].Dequeue();
                gem.SetActive(true);
                return gem;
            }
            // 如果队列里面没有，则直接获取一个新的
            return GetNewGem(gemTag);
        }

        /// <summary>
        /// 获取一个新的宝石
        /// </summary>
        /// <param name="gemTag">宝石的tag类型</param>
        /// <returns>如果tag合法则返回该宝石的GameObject，否则返回null</returns>
        private GameObject GetNewGem(string gemTag)
        {
            for (var i = 0; i < gemPrefabs.Length; i++)
            {
                if (gemTag == gemPrefabs[i].tag)
                {
                    var gem = Instantiate(gemPrefabs[i]);
                    return gem;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取一个随机类型的宝石
        /// </summary>
        /// <returns>宝石GameObject</returns>
        public GameObject GetRandomGem()
        {
            var randomGem = gemPrefabs[Random.Range(0, gemPrefabs.Length)];
            return GetGem(randomGem.tag);
        }

        /// <summary>
        /// 获取一个随机类型的宝石，可以排除2个特定类型的宝石
        /// </summary>
        /// <param name="tag1">排除宝石类型1</param>
        /// <param name="tag2">排除宝石类型2</param>
        /// <returns>宝石GameObject</returns>
        public GameObject GetRandomGem(string tag1, string tag2)
        {
            var randomGem = gemPrefabs[Random.Range(0, gemPrefabs.Length)];
            while (randomGem.tag == tag1 || randomGem.tag == tag2)
            {
                randomGem = gemPrefabs[Random.Range(0, gemPrefabs.Length)];
            }
            return GetGem(randomGem.tag);
        }

        /// <summary>
        /// 返还宝石到对象池中
        /// </summary>
        /// <param name="gem">需要返还的宝石</param>
        public void ReturnGem(GameObject gem)
        {
            gem.SetActive(false);
            if (!pool.ContainsKey(gem.tag))
            {
                pool[gem.tag] = new Queue<GameObject>();
            }
            pool[gem.tag].Enqueue(gem);
        }
    }
}
