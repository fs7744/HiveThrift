using System;
using System.Collections;

namespace Thrift.Collections
{
	public class TCollections
	{
		public TCollections()
		{
		}

		public static bool Equals(IEnumerable first, IEnumerable second)
		{
			bool i;
			if (first == null && second == null)
			{
				return true;
			}
			if (first == null || second == null)
			{
				return false;
			}
			IEnumerator enumerator = first.GetEnumerator();
			IEnumerator enumerator1 = second.GetEnumerator();
			bool flag = enumerator.MoveNext();
			for (i = enumerator1.MoveNext(); flag && i; i = enumerator1.MoveNext())
			{
				IEnumerable current = enumerator.Current as IEnumerable;
				IEnumerable enumerable = enumerator1.Current as IEnumerable;
				if (current == null || enumerable == null)
				{
					if (current == null ^ enumerable == null)
					{
						return false;
					}
					if (!object.Equals(enumerator.Current, enumerator1.Current))
					{
						return false;
					}
				}
				else if (!TCollections.Equals(current, enumerable))
				{
					return false;
				}
				flag = enumerator.MoveNext();
			}
			return flag == i;
		}

		public static int GetHashCode(IEnumerable enumerable)
		{
			if (enumerable == null)
			{
				return 0;
			}
			int num = 0;
			foreach (object obj in enumerable)
			{
				IEnumerable enumerable1 = obj as IEnumerable;
				num = num * 397 ^ (enumerable1 == null ? obj.GetHashCode() : TCollections.GetHashCode(enumerable1));
			}
			return num;
		}
	}
}