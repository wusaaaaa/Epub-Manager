using System;

namespace Epub_Manager.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns whether the <paramref name="typeToCheck"/> is assignable from the specified <paramref name="genericType"/>.
        /// </summary>
        /// <param name="genericType">The generic type.</param>
        /// <param name="typeToCheck">The type to check.</param>
        public static bool IsAssignableFromGenericType(this Type genericType, Type typeToCheck)
        {
            var interfaceTypes = typeToCheck.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = typeToCheck.BaseType;
            if (baseType == null) return false;

            return IsAssignableFromGenericType(genericType, baseType);
        }
    }
}