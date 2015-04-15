using System;
using nessusssharp;

namespace Example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (NessusSession session = new NessusSession ("admin", "password", "192.168.1.53")) {
			}
		}
	}
}
