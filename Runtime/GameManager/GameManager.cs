using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    [DefaultExecutionOrder(-1000)]
    public class GameManager : MonoBehaviour
    {
        protected virtual void LateUpdate()
        {
            //  handle escape key
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (stateStack.Count > 0)
                    stateStack[0].Back();
                else if (CurrentState != null)
                    CurrentState.Back();
            }
        }

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static readonly List<System.Type> typeStack = new List<System.Type>();
        private static readonly List<GameState> stateStack = new List<GameState>();

        public static string StatesPath { get; set; } = "States/";
        public static Canvas Canvas { get; set; } = null;

        public static event System.Action<GameState> OnBackButton = x => { };
        public static event System.Action<GameState> OnOpenState = x => { };

        public static bool IsEmpty => CurrentState == null && CurrentPopup == null && typeStack.Count < 1;
        public static GameState CurrentPopup => stateStack.Count > 0 ? stateStack[0] : null;
        public static GameState CurrentState { get; private set; } = null;

        public static T OpenState<T>(bool resetStack = false) where T : GameState
        {
#if UNITY_EDITOR
            CheckPath();
#endif
            // load prefab
            T prefab = Resources.Load<T>(StatesPath + typeof(T).Name);
            if (prefab == null)
            {
                Debug.LogError("GameManager could not find " + typeof(T).Name);
                return null;
            }

            // close current state
            if (CurrentState != null)
            {
                var delay = CurrentState.PreClose();
                Object.Destroy(CurrentState.gameObject, delay);
            }

            // update type stack
            if (resetStack) typeStack.Clear();
            if (typeStack.Count < 1 || typeStack[0] != typeof(T))
                typeStack.Insert(0, typeof(T));

            // instantiate new state from prefab
            CurrentState = Object.Instantiate<GameState>(prefab);
            CurrentState.name = prefab.name;

            AttachState(CurrentState);

            OnOpenState?.SafeInvoke(CurrentState);
            return CurrentState as T;
        }

        private static GameState CloseCurrentState()
        {
            if (typeStack.Count < 2) return CurrentState;

            typeStack.RemoveAt(0);
            var delay = CurrentState.PreClose();
            Object.Destroy(CurrentState.gameObject, delay);

            var state = Resources.Load<GameState>(StatesPath + typeStack[0].Name);
            CurrentState = Object.Instantiate(state);
            CurrentState.name = state.name;

            AttachState(CurrentState);

            OnOpenState?.SafeInvoke(CurrentState);

            return CurrentState;
        }

        public static T OpenPopup<T>(GameObject prefab) where T : GameState
        {
            if (prefab == null) return null;
            T res = Object.Instantiate(prefab).GetComponent<T>();
            res.name = prefab.name;
            stateStack.Insert(0, res);
            Resources.UnloadUnusedAssets();
            AttachState(res);
            OnOpenState?.SafeInvoke(res);
            return res;
        }

        public static T OpenPopup<T>() where T : GameState
        {
#if UNITY_EDITOR
            CheckPath();
#endif

            T popup = Resources.Load<T>(StatesPath + typeof(T).Name);
            if (popup == null)
            {
                Debug.LogError("game could not find " + typeof(T).Name);
                return null;
            }
            return OpenPopup<T>(popup.gameObject);
        }

        public static bool ClosePopup(GameState popup)
        {
            if (popup != null && stateStack.Remove(popup))
            {
                var delay = popup.PreClose();
                Object.Destroy(popup.gameObject, delay);
                return true;
            }
            return false;
        }

        //  close current popup window and return the remains opened popup
        public static int ClosePopup(bool closeAll = false)
        {
            if (stateStack.Count < 1) return 0;
            ClosePopup(stateStack[0]);
            return closeAll ? ClosePopup(closeAll) : stateStack.Count;
        }

        public static void Back(GameState gameState)
        {
            if (ClosePopup(gameState))
            {
                OnBackButton(CurrentPopup != null ? CurrentPopup : CurrentState);
            }
            else if (CurrentState == gameState)
            {
                CloseCurrentState();
                OnBackButton(CurrentPopup != null ? CurrentPopup : CurrentState);
            }
        }

        private static void AttachState(GameState state)
        {
            if (state == null) return;
            if (state.transform is RectTransform)
            {
                var panelcanvas = state.GetComponent<Canvas>();
                if (panelcanvas == null && Canvas != null)
                    state.transform.SetParent(Canvas.transform, false);
                else if (panelcanvas.renderMode != RenderMode.ScreenSpaceOverlay && panelcanvas.worldCamera == null)
                    panelcanvas.worldCamera = Camera.main;
            }
        }

#if UNITY_EDITOR
        private static void CheckPath()
        {
            var validPath = System.IO.Path.GetFullPath(Application.dataPath + "/Resources/" + StatesPath);
            if (System.IO.Directory.Exists(validPath) == false)
                System.IO.Directory.CreateDirectory(validPath);
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}