using LeagueChores.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LeagueChores
{
	enum FailureCode
	{
		OK,
		CannotConnect,
		CannotDeserialize,
	};

	internal class Response<T>
	{
		public FailureCode errorCode = FailureCode.OK;
		public HttpStatusCode statusCode;
		public T body;
		public bool ok => errorCode == FailureCode.OK && (int)statusCode >= 200 && (int)statusCode <= 299;

		public Response(HttpStatusCode statusCode, T body)
		{
			this.statusCode = statusCode;
			this.body = body;
		}

		public Response(FailureCode errorCode)
		{
			this.errorCode = errorCode;

			statusCode = 0;
			body = default;
		}
	};

	internal class DefaultResponse : Response<JToken>
	{
		public DefaultResponse(HttpStatusCode statusCode, JToken body) :
			base(statusCode, body)
		{
		}

		public DefaultResponse(FailureCode errorCode) :
			base(errorCode)
		{
		}
	};

	internal class Requester
	{
		private HttpClient m_client;
		private bool m_hasUserData = false;

		public Requester()
		{
		}

		public void SetUserData(string baseAddress, string user, string password)
		{
			if (m_hasUserData)
				return;

			m_client = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } });
			m_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			m_client.Timeout = new TimeSpan(0, 0, 15);

			var baseString = $"{user}:{password}";
			var data = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(baseString));
			m_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", data);
			m_client.BaseAddress = new Uri(baseAddress);

			m_hasUserData = true;
		}

		public void UnsetUserData()
		{
			if (!m_hasUserData)
				return;

			m_client.DefaultRequestHeaders.Authorization = null;
			m_hasUserData = false;
		}

		public async Task<Response<T>> GetAs<T>(string url)
		{
			try
			{
				HttpResponseMessage response = await m_client.GetAsync(url);

				string contents = await response.Content.ReadAsStringAsync();
				T obj = JsonConvert.DeserializeObject<T>(contents);
				if (obj == null)
					return new Response<T>(FailureCode.CannotDeserialize);
				return new Response<T>(response.StatusCode, obj);
			}
			catch
			{
				return new Response<T>(FailureCode.CannotConnect);
			}
		}

		public async Task<DefaultResponse> Get(string url)
		{
			try
			{
				HttpResponseMessage response = await m_client.GetAsync(url);
				string contents = await response.Content.ReadAsStringAsync();

				try
				{
					return new DefaultResponse(response.StatusCode, JToken.Parse(contents));
				}
				catch { }
				return new DefaultResponse(FailureCode.CannotDeserialize);
			}
			catch
			{
				return new DefaultResponse(FailureCode.CannotConnect);
			}
		}

		public async Task<Response<T>> PostAs<T>(string url, string body)
		{
			StringContent inputBody = string.IsNullOrEmpty(body) == false ? new StringContent(body, System.Text.Encoding.UTF8, "application/json") : null;

			try
			{
				HttpResponseMessage response = await m_client.PostAsync(url, inputBody);

				string contents = await response.Content.ReadAsStringAsync();
				T obj = JsonConvert.DeserializeObject<T>(contents);
				if (obj == null)
					return new Response<T>(FailureCode.CannotDeserialize);
				return new Response<T>(response.StatusCode, obj);
			}
			catch
			{
				return new Response<T>(FailureCode.CannotConnect);
			}
		}

		public async Task<DefaultResponse> Post(string url, string body)
		{
			StringContent inputBody = string.IsNullOrEmpty(body) == false ? new StringContent(body, System.Text.Encoding.UTF8, "application/json") : null;

			try
			{
				HttpResponseMessage response = await m_client.PostAsync(url, inputBody);
				string contents = await response.Content.ReadAsStringAsync();

				try
				{
					return new DefaultResponse(response.StatusCode, JToken.Parse(contents));
				}
				catch { }
				return new DefaultResponse(FailureCode.CannotDeserialize);
			}
			catch
			{
				return new DefaultResponse(FailureCode.CannotConnect);
			}
		}

		public async Task<Response<T>> PatchAs<T>(string url, string body)
		{
			StringContent inputBody = string.IsNullOrEmpty(body) == false ? new StringContent(body, System.Text.Encoding.UTF8, "application/json") : null;

			try
			{
				HttpResponseMessage response = await m_client.PatchAsync(url, inputBody);

				string contents = await response.Content.ReadAsStringAsync();
				T obj = JsonConvert.DeserializeObject<T>(contents);
				if (obj == null)
					return new Response<T>(FailureCode.CannotDeserialize);
				return new Response<T>(response.StatusCode, obj);
			}
			catch
			{
				return new Response<T>(FailureCode.CannotConnect);
			}
		}

		public async Task<DefaultResponse> Patch(string url, string body)
		{
			StringContent inputBody = string.IsNullOrEmpty(body) == false ? new StringContent(body, System.Text.Encoding.UTF8, "application/json") : null;

			try
			{
				HttpResponseMessage response = await m_client.PatchAsync(url, inputBody);
				string contents = await response.Content.ReadAsStringAsync();

				try
				{
					return new DefaultResponse(response.StatusCode, JToken.Parse(contents));
				}
				catch { }
				return new DefaultResponse(FailureCode.CannotDeserialize);
			}
			catch
			{
				return new DefaultResponse(FailureCode.CannotConnect);
			}
		}

		public async Task<Response<T>> PutAs<T>(string url, string body)
		{
			StringContent inputBody = new StringContent(body, System.Text.Encoding.UTF8, "application/json");

			try
			{
				HttpResponseMessage response = await m_client.PutAsync(url, inputBody);

				string contents = await response.Content.ReadAsStringAsync();
				T obj = JsonConvert.DeserializeObject<T>(contents);
				if (obj == null)
					return new Response<T>(FailureCode.CannotDeserialize);
				return new Response<T>(response.StatusCode, obj);
			}
			catch
			{
				return new Response<T>(FailureCode.CannotConnect);
			}
		}

		public async Task<DefaultResponse> Put(string url, string body)
		{
			StringContent inputBody = string.IsNullOrEmpty(body) == false ? new StringContent(body, System.Text.Encoding.UTF8, "application/json") : null;

			try
			{
				HttpResponseMessage response = await m_client.PutAsync(url, inputBody);
				string contents = await response.Content.ReadAsStringAsync();

				try
				{
					return new DefaultResponse(response.StatusCode, JToken.Parse(contents));
				}
				catch { }
				return new DefaultResponse(FailureCode.CannotDeserialize);
			}
			catch
			{
				return new DefaultResponse(FailureCode.CannotConnect);
			}
		}

		public bool isValid => m_hasUserData && m_client != null;
	}
}
