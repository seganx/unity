namespace SeganX
{
    public enum Party : int
    {
        Friend = 8,
        Enemy = 9
    }

    public static class PartyExtension
    {
        public static int AsInt(this Party self)
        {
            return (int)self;
        }

        public static Party Opponent(this Party self)
        {
            return self == Party.Friend ? Party.Enemy : Party.Friend;
        }

        public static Party TargetParty(this Party self, Party target)
        {
            if (self == Party.Enemy)
                return target == Party.Friend ? Party.Enemy : Party.Friend;
            else
                return target;
        }

        public static bool IsEnemy(this Party self)
        {
            return self == Party.Enemy;
        }

        public static bool IsFriend(this Party self)
        {
            return self == Party.Friend;
        }
    }
}
