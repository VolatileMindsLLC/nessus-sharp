using System;
using nessussharp;
using Newtonsoft.Json.Linq;

namespace Example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (NessusSession session = new NessusSession ("admin", "password", "192.168.1.53")) {
				using (NessusManager manager = new NessusManager (session)) {
					JObject policies = manager.ListPolicys ();

					string policyID = string.Empty;
					foreach (JToken token in policies["templates"]) {
						if (token ["name"].Value<string> () == "discovery") {
							policyID = token ["uuid"].Value<string> ();
							break;
						}
					}

					JObject scanResponse = manager.CreateScan (policyID, "Example Scan", "This is an example scan", "192.168.1.0/24");

					int scanID = scanResponse ["scan"] ["id"].Value<int> ();

					JObject resp = manager.StartScan (scanID);

					JObject scans = manager.GetScans ();

					foreach (JToken scan in scans["scans"]) {
						if (scan ["id"].Value<int> () == scanID) {
							if (scan ["status"] == "running")
								break;
						}
					}

					Console.WriteLine (scans.ToString ());
				}
			}
		}
	}
}
