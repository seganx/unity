using UnityEngine.Scripting;

namespace SeganX
{
    public interface IMessages
    {
        [Preserve]
        void OnMessage(Messages.Param param);
    }
}