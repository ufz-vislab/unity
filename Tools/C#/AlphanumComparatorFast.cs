// Non-generic version from http://www.dotnetperls.com
using System.Collections.Generic;

public class AlphanumComparatorFast : IComparer<string>
{
	public int Compare(string x, string y)
	{
		var s1 = x;
		var s2 = y;

		int len1 = s1.Length;
		int len2 = s2.Length;
		int marker1 = 0;
		int marker2 = 0;

		/*
		 *
		 * Walk through two the strings with two markers.
		 *
		 * */
		while (marker1 < len1 && marker2 < len2)
		{
			char ch1 = s1[marker1];
			char ch2 = s2[marker2];

			/*
			 *
			 * Some buffers we can build up characters in for each chunk.
			 *
			 * */
			var space1 = new char[len1];
			int loc1 = 0;
			var space2 = new char[len2];
			int loc2 = 0;

			/*
			 *
			 * Walk through all following characters that are digits or characters in BOTH strings
			 * starting at the appropriate marker. Collect char arrays.
			 *
			 * */

			do
			{
				space1[loc1++] = ch1;
				marker1++;

				if (marker1 < len1)
				{
					ch1 = s1[marker1];
				}
				else
				{
					break;
				}
			} while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

			do
			{
				space2[loc2++] = ch2;
				marker2++;

				if (marker2 < len2)
				{
					ch2 = s2[marker2];
				}
				else
				{
					break;
				}
			} while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

			/*
			 *
			 * Here, we compare. If we have collected numbers, compare them numerically.
			 * Otherwise, if we have strings, compare them alphabetically.
			 *
			 * */
			string str1 = new string(space1);
			string str2 = new string(space2);

			int result;
			if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
			{
				int thisNumericChunk = int.Parse(str1);
				int thatNumericChunk = int.Parse(str2);
				result = thisNumericChunk.CompareTo(thatNumericChunk);
			}
			else
			{
				result = str1.CompareTo(str2);
			}

			if (result != 0)
			{
				return result;
			}
		}
		return len1 - len2;
	}
}
