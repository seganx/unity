using UnityEngine;
using UnityEngine.Events;

namespace SeganX.Widgets
{
    public class EventTimer : MonoBehaviour
    {
        [SerializeField] private bool autoStart = true;
        [SerializeField] private bool repeat = true;
        [SerializeField] private float startDelay = 1;
        [SerializeField] private float interval = 1;
        [SerializeField] private UnityEvent targetEvent;
        private float timer = 0;

        private void Start()
        {
            StopTimer();
            timer = startDelay;
            if (autoStart) StartTimer();
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                Tick();
                ResetTimer();
                if (!repeat) StopTimer();
            }
        }

        private void ResetTimer()
        {
            timer = interval;
        }

        public void Tick()
        {
            targetEvent?.Invoke();
        }

        public void StartTimer()
        {
            enabled = true;
        }

        public void StopTimer()
        {
            enabled = false;
        }

        public void DestroyObject()
        {
            Destroy(gameObject);
        }
    }
}