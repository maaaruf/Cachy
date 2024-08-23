using System.Reflection;

namespace Cachy;

internal static class Helpers
{
    internal static IEnumerable<MethodInfo> GetExtensionMethods(Type extendedType)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSealed && !type.IsGenericType && !type.IsNested)
                {
                    foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false))
                        {
                            var parameters = method.GetParameters();
                            if (parameters.Length > 0 && parameters[0].ParameterType == extendedType)
                            {
                                yield return method;
                            }
                        }
                    }
                }
            }
        }
    }
}
