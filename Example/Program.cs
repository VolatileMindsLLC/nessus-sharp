using System;
using nessusssharp;

namespace Example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (NessusSession session = new NessusSession ("adgmin", "password", "192.168.1.53")) {
			}
		}
	}
}
