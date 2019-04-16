namespace SeganX
{
    public abstract class GameState : Base
    {
        //  Will be called before destroying this object. return time of closing animation
        public virtual float PreClose()
        {
            return 0;
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
