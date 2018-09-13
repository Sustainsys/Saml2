using System;
using System.Collections.Generic;
using System.Text;

namespace Sustainsys.Saml2.Internal
{
    static class CompareHelper
    {
		public static bool ByteArraysEqual(byte[] a, byte[] b)
		{
			if (a == null)
			{
				return b == null;
			}
			if (a.Length != b.Length)
			{
				return false;
			}
			for (int i = 0; i < a.Length; ++i)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
