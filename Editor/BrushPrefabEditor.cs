using UnityEditor;
using UnityEngine;

namespace SeganX
{
    [CustomEditor(typeof(BrushPrefab))]
    public class BrushPrefabEditor : Editor
    {
        void OnSceneGUI()
        {
            var brushMan = target as BrushPrefab;
            if (brushMan == null || brushMan.prefab == null || brushMan.editMode == BrushPrefab.EditMode.Select)
                return;

            Event evt = Event.current;
            if (evt.button != 0) return;
            if (evt.modifiers != EventModifiers.None && evt.modifiers != EventModifiers.Shift) return;

            switch (evt.type)
            {
                case EventType.MouseUp:
                    {
                        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            Quaternion initRotarion = Quaternion.identity;
                            if ((brushMan.options & BrushPrefab.Options.RandomRotationX) != 0)
                                initRotarion *= Quaternion.Euler(Random.Range(0, 360), 0, 0);
                            if ((brushMan.options & BrushPrefab.Options.RandomRotationY) != 0)
                                initRotarion *= Quaternion.Euler(0, Random.Range(0, 360), 0);
                            if ((brushMan.options & BrushPrefab.Options.RandomRotationZ) != 0)
                                initRotarion *= Quaternion.Euler(0, 0, Random.Range(0, 360));
                            var newone = Instantiate(brushMan.prefab, hit.point, initRotarion, brushMan.transform);
                            newone.SendMessage("OnCreatedByBrushPrefab", evt, SendMessageOptions.DontRequireReceiver);
                            BrushPrefab.lastCreated = newone;
                        }
                        evt.Use();
                    }
                    break;

                case EventType.Layout:
                    {
                        int id = GUIUtility.GetControlID(FocusType.Passive);
                        HandleUtility.AddDefaultControl(id);
                    }
                    break;
            }
        }
    }
}