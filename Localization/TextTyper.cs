using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SeganX
{
    [DefaultExecutionOrder(1)]
    public class TextTyper : MonoBehaviour
    {
        [SerializeField] private Text target = null;
        [SerializeField] private float delayTime = 0.035f;

        private string[] lines = null;
        private string text = string.Empty;
        private WaitForSeconds typeDelay = null;


        private void Awake()
        {
            typeDelay = new WaitForSeconds(delayTime);
        }

        private void LateUpdate()
        {
            if (text != target.text)
            {
                lines = target.text.Split('\n');
                target.text = text = string.Empty;
                StopAllCoroutines();
                StartCoroutine(TypeText());
            }
        }

        private IEnumerator TypeText()
        {
            int rindex = 0;
            for (int l = 0; l < lines.Length; l++)
            {
                for (int i = lines[l].Length - 1; i >= 0; i--)
                {
                    target.text = text = text.Insert(rindex, string.Empty + lines[l][i]);
                    yield return typeDelay;
                }

                if (l < lines.Length - 1)
                {
                    text = text + '\n';
                    target.text = text;
                    rindex = text.Length;
                }
            }
        }
    }
}
