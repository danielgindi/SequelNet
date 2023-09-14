using System;

namespace SequelNet;

	internal static class TypeExtensions
	{
		internal static bool IsCollectionType(this Type type)
		{
			foreach (var intr in type.GetInterfaces())
			{
				if (intr.Name == "ICollection" ||
					(intr.Name.StartsWith("ICollection`") && intr.Namespace == "System.Collections.Generic"))
					return true;
			}

			return false;
		}
	}
