using UnityEngine;

namespace SeganX
{
    [DefaultExecutionOrder(-9999)]
    public class GameCanvas : MonoBehaviour
    {
        private void Awake()
        {
            var canvas = GetComponent<Canvas>();
            if (canvas != null)
                GameManager.Canvas = canvas; 
        }
    }
}