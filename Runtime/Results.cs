public class BoolResult
{
    public bool value;


    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    private static readonly BoolResult result = new();

    public static BoolResult Set(bool value)
    {
        result.value = value;
        return result;
    }
}

public class IntResult
{
    public int value;


    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    private static readonly IntResult result = new();

    public static IntResult Set(int value)
    {
        result.value = value;
        return result;
    }
}

public class ShortResult
{
    public short value;

    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    private static readonly ShortResult result = new();

    public static ShortResult Set(short value)
    {
        result.value = value;
        return result;
    }
}

public class FloatResult
{
    public float value;


    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    private static readonly FloatResult result = new();

    public static FloatResult Set(float value)
    {
        result.value = value;
        return result;
    }
}

public class LongResult
{
    public long value;


    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    private static readonly LongResult result = new();

    public static LongResult Set(long value)
    {
        result.value = value;
        return result;
    }
}