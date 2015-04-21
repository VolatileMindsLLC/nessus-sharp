using System;
using System.Collections;
using System.Xml;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace nessussharp
{
	public class NessusSession : IDisposable
	{
		public string ServerHost { get; set; }
		public int ServerPort { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string AuthenticationToken { get; set; }

		public NessusSession(string username, string password, string host, int port = 8834) {
			this.ServerHost = host;
			this.ServerPort = port;

			JObject response = this.Authenticate (username, password);

			if (response ["error"] != null)
				throw new Exception (response ["error"].Value<string>());

			this.AuthenticationToken = response ["token"].Value<string>();
		}


		public JObject ExecuteCommand(string verb, string uri, JObject parameters = null){

			ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;

			string url = "https://" + this.ServerHost + ":" + this.ServerPort + uri;

			HttpWebRequest request = WebRequest.Create (url) as HttpWebRequest;
			request.Method = verb;
			request.Headers ["X-Cookie"] = "token=" + this.AuthenticationToken;
			request.Accept = "application/json";
			request.ContentType = "application/json";

			if (parameters != null) {
				byte[] parmBytes = System.Text.Encoding.ASCII.GetBytes (parameters.ToString ());
				request.ContentLength = parmBytes.Length;
				request.GetRequestStream ().Write (parmBytes, 0, parmBytes.Length);
			} else if (verb == "POST")
				request.ContentLength = 0;

			string response = string.Empty;
			try {
				using (StreamReader reader = new StreamReader (request.GetResponse ().GetResponseStream ()))
					response = reader.ReadToEnd ();
			} catch (WebException ex) {
				using (StreamReader reader = new StreamReader (ex.Response.GetResponseStream ()))
					response = reader.ReadToEnd ();
			}


			return JObject.Parse (response);
		}

		public JObject Authenticate (string username, string password) {

			if (username == string.Empty || password == string.Empty)
				throw new Exception ("Username and Password required");

			JObject parameters = new JObject ();
			parameters ["username"] = username;
			parameters ["password"] = password;

			return ExecuteCommand ("POST", "/session", parameters);
		}

		public void Dispose()
		{
			JObject parameters = new JObject ();
			parameters ["token"] = this.AuthenticationToken;

			this.ExecuteCommand ("DELETE", "/session", parameters);
		}
	}
}

