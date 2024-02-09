using System.Threading;
using UnityEditor;
using UnityEngine;

namespace SeganX
{
    [InitializeOnLoad]
    public class EditorPanicPause : MonoBehaviour
    {
        private static Thread panicThread = null;

        static EditorPanicPause()
        {
            if (panicThread != null) return;

            //panicThread = new Thread(PanicWorkingThread);
            //panicThread.Start();
        }

        static void PanicWorkingThread()
        {
            Debug.Log("Start panic thread");
            while (EditorApplication.isPlaying)
            {
                Thread.Sleep(20);
                if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Escape))
                {
                    EditorApplication.isPaused = true;
                    break;
                }
            }

            Debug.Log("End panic thread");
            panicThread = null;
        }
    }
}