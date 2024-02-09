#if SX_PARSI
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace SeganX
{
    [DefaultExecutionOrder(1)]
    public class TextTyper : MonoBehaviour
    {
        [SerializeField] private Text target = null;
        [SerializeField] private float delayTime = 0.01f;

        private string[] lines = null;
        private string text = string.Empty;
        private string targetText = null;
        private WaitForSecondsRealtime typeDelay = null;
        private Action onFinish;

        public void Setup(Action onFinish)
        {
            this.onFinish = onFinish;
        }

        public void Skip()
        {
            StopAllCoroutines();
            target.text = text = targetText;
            if (onFinish != null)
                onFinish();
        }

        private void Awake()
        {
            typeDelay = new WaitForSecondsRealtime(delayTime * 0.75f);
        }

        private void LateUpdate()
        {
            if (text != target.text)
            {
                targetText = target.text;
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
                for (int i = lines[l].Length - 1; i >= 0 && target.text == text; i--)
                {
                    target.text = text = text.Insert(rindex, string.Empty + lines[l][i]);
                    yield return typeDelay;
                }

                if (l < lines.Length - 1 && target.text == text)
                {
                    text = text + '\n';
                    target.text = text;
                    rindex = text.Length;
                }
            }

            if (onFinish != null)
                onFinish();
        }

        private void Reset()
        {
            if (target == null)
                target = transform.GetComponent<Text>(true, true);
        }
    }
}
#endif