using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public abstract class GameManager : MonoBehaviour
    {
        [SerializeField] private string prefabPath = "Menus/";
        [SerializeField] private Canvas canvas = null;

        private GameState currentState = null;
        private List<System.Type> typeStack = new List<System.Type>();
        private List<GameState> stateStack = new List<GameState>();

        public System.Action<GameState> OnBackButton = new System.Action<GameState>(x => { });
        public System.Action<GameState> OnOpenState = new System.Action<GameState>(x => { });

        public bool IsEmpty { get { return currentState == null && CurrentPopup == null && typeStack.Count < 1; } }
        public Canvas Canvas { get { return canvas; } }
        public GameState CurrentPopup { get { return stateStack.Count > 0 ? stateStack[0] : null; } }
        public GameState CurrentState { get { return currentState; } }

        public T OpenState<T>(bool resetStack = false) where T : GameState
        {
            // load prefab
            T prefab = Resources.Load<T>(prefabPath + typeof(T).Name);
            if (prefab == null)
            {
                Debug.LogError("GameManager could not find " + typeof(T).Name);
                return null;
            }

            // close current state
            if (currentState != null)
            {
                var delay = currentState.PreClose();
                Destroy(currentState.gameObject, delay);
            }

            // update type stack
            if (resetStack) typeStack.Clear();
            if (typeStack.Count < 1 || typeStack[0] != typeof(T))
                typeStack.Insert(0, typeof(T));

            // instantiate new state from prefab
            currentState = Instantiate<GameState>(prefab);
            currentState.name = prefab.name;

            AttachState(currentState);

            OnOpenState(currentState);
            return currentState as T;
        }

        private GameState CloseCurrentState()
        {
            if (typeStack.Count < 2) return currentState;

            typeStack.RemoveAt(0);
            var delay = currentState.PreClose();
            Destroy(currentState.gameObject, delay);

            var state = Resources.Load<GameState>(prefabPath + typeStack[0].Name);
            currentState = Instantiate(state) as GameState;
            currentState.name = state.name;
            AttachState(currentState);

            OnOpenState(currentState);

            return currentState;
        }

        public T OpenPopup<T>(GameObject prefab) where T : GameState
        {
            if (prefab == null) return null;
            T res = Instantiate<GameObject>(prefab).GetComponent<T>();
            res.name = prefab.name;
            stateStack.Insert(0, res);
            Resources.UnloadUnusedAssets();
            AttachState(res);
            OnOpenState(res);
            return res;
        }

        public T OpenPopup<T>() where T : GameState
        {
            T popup = Resources.Load<T>(prefabPath + typeof(T).Name);
            if (popup == null)
            {
                Debug.LogError("GameManager could not find " + typeof(T).Name);
                return null;
            }
            return OpenPopup<T>(popup.gameObject);
        }

        public bool ClosePopup(GameState popup)
        {
            if (popup != null && stateStack.Remove(popup))
            {
                var delay = popup.PreClose();
                Destroy(popup.gameObject, delay);
                return true;
            }
            return false;
        }

        //  close current popup window and return the remains opened popup
        public int ClosePopup(bool closeAll = false)
        {
            if (stateStack.Count < 1) return 0;
            ClosePopup(stateStack[0]);
            return closeAll ? ClosePopup(closeAll) : stateStack.Count;
        }

        public GameManager Back(GameState gameState)
        {
            if (ClosePopup(gameState))
            {
                OnBackButton(CurrentPopup != null ? CurrentPopup : currentState);
            }
            else if (currentState == gameState)
            {
                CloseCurrentState();
                OnBackButton(CurrentPopup != null ? CurrentPopup : currentState);
            }
            return this;
        }

        private void AttachState(GameState panel)
        {
            if (panel == null) return;

            if (canvas.worldCamera == null)
                canvas.worldCamera = Camera.main;

            if (panel.transform is RectTransform)
            {
                var panelcanvas = panel.GetComponent<Canvas>();
                if (panelcanvas == null)
                    panel.transform.SetParent(canvas.transform, false);
                else if (panelcanvas.worldCamera == null)
                    panelcanvas.worldCamera = canvas.worldCamera;
            }
        }

        protected virtual void LateUpdate()
        {
            //  handle escape key
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (stateStack.Count > 0)
                    stateStack[0].Back();
                else if (currentState != null)
                    currentState.Back();
            }
        }

        protected virtual void Reset()
        {
            var validPath = Application.dataPath + "/Resources/" + prefabPath;
            if (System.IO.Directory.Exists(validPath) == false)
                System.IO.Directory.CreateDirectory(validPath);
        }

#if UNITY_EDITOR && OFF
        void OnGUI()
        {
            string str = "Popup Stack:\n";
            foreach (var item in popupStack)
                str += item.name + "\n";
            str += "Page Stack:\n";
            foreach (var item in pageStack)
                str += item.Name + "\n";
            GUI.Box(new Rect(0, 0, 100, 100), str);
        }
#endif
    }
}
