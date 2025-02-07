namespace Hgzn.Mes.Infrastructure.Utilities;

public static class Check
{
    public static T NotNull<T>(T? value, string parameterName)
    {
        if (value == null)
        {
            throw new ArgumentNullException(parameterName);
        }
        return value;
    }
}