using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GemMatch
{
    public class DisplayState : MonoBehaviour
    {
        private Text stateText;
        // Start is called before the first frame update
        void Start()
        {
            stateText = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            if (DebugPrint.instance.debugFlag)
            {
                stateText.text = $"swapping: {GameManager.instance.swapping}\ndropping: {GameManager.instance.dropping}\nmultiBoomCount:{GameManager.instance.multiBoomCount}";
            }
            else
            {
                stateText.text = "";
            }
        }
    }

}
