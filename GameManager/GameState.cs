namespace SeganX
{
    public abstract class GameState : Base
    {
        //  Will be called before destroying this object. return time of closing animation
        public virtual float PreClose()
        {
            return UiShowHide.HideAll(transform);
        }

        //  will be called whene back button pressed
        public virtual void Back()
        {
            gameManager.Back(this);
        }

        public virtual void Reset()
        {
            gameObject.name = GetType().Name;
        }

    }

}
