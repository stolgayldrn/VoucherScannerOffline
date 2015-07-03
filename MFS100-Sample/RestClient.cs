using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using RedRose_VoucherScanner.Properties;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace RedRose_VoucherScanner
{
    public class Vendor
    {
        public string id;
        public string fullName;
        public string shopName;

        public override string ToString()
        {
            return fullName +  " - " + shopName;
        }
    }

	public class RestClient
	{
        public static String Authorize(string username, string password)
		{
            Stream responseStream = CallWebServiceGet("authorize", null, username, password);
			StreamReader reader = new StreamReader(responseStream);
			string result = reader.ReadToEnd();
			String retval = JsonConvert.DeserializeObject<String>(result);
			return retval;
		}


        public static List<Vendor> GetVendors(string username, string password)
        {
            Stream responseStream = CallWebServiceGet("/getVendors", null, username, password);
            StreamReader reader = new StreamReader(responseStream);
            string result = reader.ReadToEnd();
            List<Vendor> retval = JsonConvert.DeserializeObject<List<Vendor>>(result);
            return retval;
        }

		public bool ValidateVoucherPage(string barcode, string username, string password)
		{
			NameValueCollection parameters = new NameValueCollection();			
			parameters.Add("barcode", barcode);
            Stream responseStream = null;
            try
            {
                responseStream = CallWebServiceGet("/validateVoucherPage", parameters, username, password);
                StreamReader reader = new StreamReader(responseStream);
                string result = reader.ReadToEnd();
                bool retval = JsonConvert.DeserializeObject<bool>(result);
                return retval;
            }
            finally
            {
                if (responseStream!=null)
                    responseStream.Close();
            }
		}

        public static List<ValidationResult> ValidateVoucherPages(List<String> req, string username, string password)
        {
            Stream responseStream = null;
            try
            {
                responseStream = CallWebServicePost("/validateVoucherPages", req, username, password);
                StreamReader reader = new StreamReader(responseStream);
                string result = reader.ReadToEnd();
                List<ValidationResult> retval = JsonConvert.DeserializeObject<List<ValidationResult>>(result);
                return retval;
            }
            finally
            {
                if (responseStream != null)
                    responseStream.Close();
            }
        }

		private void SampleCall()
		{
			try
			{
				ValidateVoucherPage( "312321321", "1312", "12312");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

        public static Stream CallWebServicePost(string url, Object objectToSend, string username, string password)
        {
            Stream responseStream;
            HttpWebRequest request = WebRequest.Create(Settings.Default.api_base + url) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Credentials = new NetworkCredential(username, password);
            request.PreAuthenticate = true;

            if (objectToSend != null)
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {

                    string entity = JsonConvert.SerializeObject(objectToSend);
                    streamWriter.Write(entity);

                }
            }
            try
            {
                request.ServerCertificateValidationCallback = MyCertHandler;
                
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    responseStream = response.GetResponseStream();
                    return responseStream;
                }
                else
                {
                    string errorMessage = response.GetResponseHeader("error");
                    throw new Exception(errorMessage);
                }
            }
            catch (WebException e)
            {
                if (e.Response != null && e.Response.Headers != null)
                {
                    if (e.Response is HttpWebResponse)
                    {
                        HttpWebResponse resp = (HttpWebResponse)e.Response;
                        throw new Exception(resp.StatusDescription);
                    }
                }
                throw e;
            }
        }

		public static void UploadVouchers(VoucherPack req, string username, string password)
		{
            Stream responseStream = null;
            try
            {
                responseStream = CallWebServicePost("/uploadVouchers", req, username, password);
                StreamReader reader = new StreamReader(responseStream);
                string result = reader.ReadToEnd();
                string resultMessage = JsonConvert.DeserializeObject<string>(result);
                if (!"OK".Equals(resultMessage))
                    throw new InvalidDataException("Result message should have been \"OK\"! Instead received: " + resultMessage);
            }
            finally
            {
                if (responseStream != null)
                    responseStream.Close();
            }
  		}

        private static bool MyCertHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors error)
        {
        // Ignore errors
            return true;
        }

		private static Stream CallWebServiceGet(string url, NameValueCollection parameters, string username, string password)
		{
			Stream responseStream;

            var api_base = Settings.Default.api_base;

			string parameterizedUrl = url;
			if (parameters != null)
			{
				for (int i = 0; i < parameters.Count; i++)
				{
					if (i == 0)
						parameterizedUrl += "?";
					else
						parameterizedUrl += "&";
					parameterizedUrl += parameters.Keys[i] + "=" + Uri.EscapeDataString(parameters[i] != null ? parameters[i] : "");
				}
			}

			HttpWebRequest request = WebRequest.Create(api_base + parameterizedUrl) as HttpWebRequest;
			request.Method = "GET";
			//request.ContentType = "application/json; charset=utf-8";
			//CredentialCache credentialCache = new CredentialCache();

			//credentialCache.Add(new Uri(Settings.Default.api_base), "Basic", new NetworkCredential(username, password));
            request.Credentials = new NetworkCredential(username, password); //credentialCache;
			//request.PreAuthenticate = true;

			try
			{
                request.ServerCertificateValidationCallback = MyCertHandler;
				HttpWebResponse response = request.GetResponse() as HttpWebResponse;
				if (response.StatusCode == HttpStatusCode.OK)
				{
					responseStream = response.GetResponseStream();
					return responseStream;
				}
				else
				{
					string errorMessage = response.GetResponseHeader("error");
					throw new Exception(errorMessage);
				}
			}
			catch (WebException e)
			{
				if (e.Response != null && e.Response.Headers != null)
				{
					if (e.Response is HttpWebResponse)
					{
						HttpWebResponse resp = (HttpWebResponse)e.Response;
                        throw new Exception(resp.StatusDescription);
					}
				}
				throw e;
			}
		}
    }
}
