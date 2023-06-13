using System.Reflection;

namespace MailContainerTest.Tests.Helper
{
    public static class CallMethod
    {
        public static TReturn? CallPrivateMethod<TReturn>(this object instance, string methodName, params object[] parameters)
        {
            Type type = instance.GetType();
            MethodInfo? methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);

            return (TReturn?)methodInfo!.Invoke(instance, parameters);
        }
    }
}