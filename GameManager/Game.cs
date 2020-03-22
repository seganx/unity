using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public partial class Game : GameManager
    {
        private static Game instance = null;
        public static Game Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Game>();
                    if (instance == null)
                    {
                        instance = new GameObject().AddComponent<Game>();
                        instance.gameObject.name = instance.GetType().Name;
                    }
                }
                return instance;
            }
        }
    }
}
