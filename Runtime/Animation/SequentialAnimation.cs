using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public class SequentialAnimation : MonoBehaviour
    {
        [System.Serializable]
        public class AnimationData
        {
            public string name = string.Empty;
        }

        public Animation target = null;
        public List<AnimationData> animations = new List<AnimationData>();

        private int currIndex = 0;
        private AnimationState currState = null;

        private AnimationState Play(AnimationData anim)
        {
            currState = target[anim.name];
            target.CrossFade(anim.name, 0.1f, PlayMode.StopAll);
            return currState;
        }

        private void Update()
        {
            if (target == null) return;
            if (target.isPlaying == false)
            {
                currIndex = ++currIndex % animations.Count;
                Play(animations[currIndex]);
            }
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (target == null)
                target = this.GetComponent<Animation>(true, true);

            animations.Clear();
            foreach (AnimationState state in target)
            {
                var data = new AnimationData();
                data.name = state.clip.name;
                animations.Add(data);
            }
        }
#endif        
    }
}