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
            [HideInInspector] public string title = string.Empty;
            [HideInInspector] public string name = string.Empty;
            public AnimationClip clip = null;
            public int id = 0;
            public int group = 0;
            public int chance = 1;
        }

        public Animation target = null;
        public List<AnimationData> animations = new List<AnimationData>();

        private List<AnimationData> groups = new List<AnimationData>(32);
        private int lastGroup = -1;
        private float animLength = -1;

        public AnimationState CurrentState { get; private set; } = null;
        public int CurrentId { get; private set; } = -1;

        private AnimationState Play(AnimationData anim)
        {
            CurrentId = anim.id;
            target.Play(anim.name);
            CurrentState = target[anim.name];
            animLength = CurrentState.length;
            return CurrentState;
        }

        public AnimationState PlayById(int id, bool justOnce = false)
        {
            if (justOnce && CurrentId == id) return CurrentState;
            var res = animations.Find(x => x.id == id);
            if (res == null) return null;
            lastGroup = -1;
            return Play(res);
        }

        public AnimationState PlayRandom(int group, Mode mode = Mode.StopPrevious)
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
            else if (mode == Mode.DiscardNext && animLength > 0) return CurrentState;

            if (groups.Count < 1) return CurrentState;
            var res = groups.RandomOne();
            if (CurrentId == res.id)
            {
                animLength = CurrentState.length;
                return CurrentState;
            }

            return Play(res);
        }

        public void Stop()
        {
            if (target.isPlaying)
            {
                target.Rewind();
                target.Stop();
                CurrentId = -1;
            }
        }

        private void Update()
        {
            animLength -= Time.deltaTime;
        }


#if UNITY_EDITOR
        private List<AnimationData> tmp = new List<AnimationData>();
        private void OnValidate()
        {
            if (target == null)
                target = this.GetComponent<Animation>(true, true);

            foreach (var item in animations)
            {
                if (item.clip == null)
                {
                    SafeRemove(target, item.name);
                    item.name = "-- No Clip!";
                }
                else
                {
                    item.name = item.clip.name;
                    target.AddClip(item.clip, item.clip.name);
                }

                item.title = item.group + ":" + item.id + ":" + item.name;
            }

            var trashes = tmp.FindAll(x => animations.Exists(a => a.name == x.name) == false);
            foreach (var item in trashes)
                SafeRemove(target, item.name);

            tmp.Clear();
            foreach (var item in animations)
                tmp.Add(new AnimationData() { name = item.name, clip = item.clip });
        }

        private void SafeRemove(Animation target, string clip)
        {
            if (target.GetClip(clip) != null)
                target.RemoveClip(clip);
        }
#endif
    }
}
