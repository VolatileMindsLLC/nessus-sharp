using System;
using System.Collections;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace nessussharp
{
	public class NessusManager : IDisposable
	{
		NessusSession _session = null;

		public NessusManager(NessusSession session) {
			if (session.AuthenticationToken == string.Empty)
				throw new Exception ("Session passed to NessusManager must be authenticated");

			_session = session;
		}

		public JObject ListPolicys() {
			return _session.ExecuteCommand ("GET", "/editor/policy/templates");
		}

		public JObject CreateScan(string policyID, string name, string description, string targets){
			JObject parameters = new JObject ();
			parameters ["uuid"] = policyID;
			parameters ["settings"] = new JObject ();
			parameters ["settings"] ["name"] = name;
			parameters ["settings"] ["description"] = description;
			parameters ["settings"] ["text_targets"] = targets;

			return _session.ExecuteCommand ("POST", "/scans", parameters);
		}

		public JObject StartScan(int id) {
			return _session.ExecuteCommand ("POST", "/scans/" + id + "/launch");
		}

		public JObject GetScans() {
			return _session.ExecuteCommand ("GET", "/scans");
		}

		public void Dispose()
		{
			if (_session != null)
				_session.Dispose ();

			_session = null;
		}
	}
}

