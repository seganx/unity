using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public class SimpleAnimation : MonoBehaviour
    {
        public enum Mode { StopPrevious, DiscardNext }

        [System.Serializable]
        public class AnimationData
        {
            public string name = string.Empty;
            public int id = 0;
            public int group = 0;
            public int chance = 1;
        }

        public Animation target = null;
        public List<AnimationData> animations = new List<AnimationData>();

        private List<AnimationData> groups = new List<AnimationData>(32);
        private AnimationState lastState = null;
        private int lastId = -1;
        private int lastGroup = -1;
        private float animLength = -1;

        private AnimationState Play(AnimationData anim)
        {
            lastId = anim.id;
            lastState = target[anim.name];
            target.CrossFade(anim.name, 0.1f, PlayMode.StopAll);
            animLength = lastState.length;
            return lastState;
        }

        public AnimationState PlayById(int id)
        {
            if (lastId == id) return lastState;
            var res = animations.Find(x => x.id == id);
            if (res == null) return null;
            lastGroup = -1;
            return Play(res);
        }

        public AnimationState PlayRandom(int group, Mode mode = Mode.DiscardNext)
        {
            if (lastGroup != group)
            {
                lastGroup = group;
                groups.Clear();
                foreach (var anim in animations)
                    if (anim.group == group)
                        for (int i = 0; i < anim.chance; i++)
                            groups.Add(anim);
            }
            else if (mode == Mode.DiscardNext && animLength > 0) return lastState;

            if (groups.Count < 1) return lastState;
            var res = groups.RandomOne();
            if (lastId == res.id)
            {
                animLength = lastState.length;
                return lastState;
            }

            return Play(res);
        }

        private void Update()
        {
            animLength -= Time.deltaTime;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (target == null)
                target = this.GetComponent<Animation>(true, true);

            lastId = 0;
            animations.Clear();
            foreach (AnimationState state in target)
            {
                var data = new AnimationData();
                data.name = state.clip.name;
                data.id = lastId++;
                animations.Add(data);
            }
        }
#endif        
    }
}
