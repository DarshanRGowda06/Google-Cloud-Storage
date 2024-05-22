using AgilePoint.AgilePart.GoogleCloudStorage.DesignTime;
using Ascentn.AgilePart.Shared;
using Ascentn.Workflow.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using static AgilePoint.AgilePart.GoogleCloudStorage.DesignTime.GoogleCloudStorageDTO;

namespace AgilePoint.AgilePart.GoogleCloudStorage
{
    public class GoogleCloudStorageOperation
    {
        readonly Hashtable m_AccessTokenSettings;

        public const string OperationCreateBucket = "GoogleCloudStorage_CreateBucket";
        public const string OperationCreateFolder = "GoogleCloudStorage_CreateFolder";
        public const string OperationDeleteBucket = "GoogleCloudStorage_DeleteBucket";
        public const string OperationDeleteObject = "GoogleCloudStorage_DeleteObject";
        public const string OperationDeleteFolder = "GoogleCloudStorage_DeleteFolder";
        public const string OperationGetBucket = "GoogleCloudStorage_GetBucket";
        public const string OperationGetBucketACL = "GoogleCloudStorage_GetBucketACL";
        public const string OperationGetObjectInformation = "GoogleCloudStorage_GetObject";
        public const string OperationGetObjectACL = "GoogleCloudStorage_GetObjectACL";
        public const string OperationGetObjectMetaData = "GoogleCloudStorage_GetObjectMetaData";
        public const string OperationSetBucketACL = "GoogleCloudStorage_SetBucketACL";
        public const string OperationSetObjectACL = "GoogleCloudStorage_SetObjectACL";
        public const string OperationSetObjectMetaData = "GoogleCloudStorage_SetObjectMetaData";
        public const string OperationRemoveBucketACL = "GoogleCloudStorage_RemoveBucketACL";
        public const string OperationRemoveObjectACL = "GoogleCloudStorage_RemoveObjectACL";
        public string m_Accesskey =string.Empty;
        public const string GOOGLECLOUDSTORAGEURL = "https://storage.googleapis.com";
        public const string GOOGLECLOUDSTORAGECREATEBUCKET = "/storage/v1/b?project={0}";
        public const string GOOGLECLOUDSTORAGEBUCKET = "/storage/v1/b/{0}";
        public const string GOOGLECLOUDSTORAGECREATEFOLDER = "/upload/storage/v1/b/{0}/o?name={1}%2F";
        public const string GOOGLECLOUDSTORAGEGETOBJECTMETATDATA = "?fields=contentType%2CcontentEncoding%2CcontentDisposition%2CcontentLanguage%2CcustomTime%2CcacheControl%2Cmetadata";
        private string projectID = string.Empty;
        GoogleCloudStorageDTO storageDTO = new GoogleCloudStorageDTO();
        AgilePartDebugLog agilePartDebugLog = null;


        private void InitAccessTokenSettings()
        {
            m_Accesskey = m_AccessTokenSettings["AccessToken"] + "";
            projectID = m_AccessTokenSettings["ProjectID"] + "";
        }
        public GoogleCloudStorageOperation(Hashtable AccessTokenSettings, WFSocialOperation currentOperation)
        {
            m_AccessTokenSettings = AccessTokenSettings;
            InitAccessTokenSettings();
            agilePartDebugLog = new AgilePartDebugLog(currentOperation);

        }
        public GoogleCloudStorageOperation(Hashtable AccessTokenSettings)
        {
            m_AccessTokenSettings = AccessTokenSettings;
            InitAccessTokenSettings();

        }

        public List<NameValue> ListAllBuckets()
        {
            string URL = GOOGLECLOUDSTORAGEURL + string.Format(GOOGLECLOUDSTORAGECREATEBUCKET,this.projectID)+ "&fields = items(id, name)";
            string jsonResponse =CallRESTAPI(URL, null, HttpMethod.GET,null);
            JObject jObject = JObject.Parse(jsonResponse);
            JArray bucketlist = (JArray)jObject["items"];
            List<NameValue> response = bucketlist.Select(item =>
            new NameValue
            {
                Name = (string)item["name"],
                Value = (string)item["id"]
            }).ToList();
            return response;
        }

        internal void CreateBucket(string bucketName)
        {
            string URL = string.Format(GOOGLECLOUDSTORAGEURL + GOOGLECLOUDSTORAGECREATEBUCKET, projectID);
            var payload = new { name = bucketName };
            CallRESTAPI(URL,ShUtil.JsonSerialize(payload), HttpMethod.POST, null);
        }
        internal void DeleteBucket(string bucketName)
        {
            string URL = GOOGLECLOUDSTORAGEURL + GOOGLECLOUDSTORAGEBUCKET;
            URL = string.Format(URL, bucketName);
            CallRESTAPI(URL, null, HttpMethod.DELETE, null);
        }
        internal void CreateFolder(string folderName, string filePath)
        {
            string bucketName = filePath?.Split('/')[0];
            string filepath = filePath?.Substring(bucketName.Length).TrimStart('/');
            filepath = filepath?.Replace(" ", "%20");
            folderName = folderName?.Replace(" ", "%20");
            string encodedfilepath = WebUtility.UrlEncode(filepath).Replace("%2520", "%20") + folderName;
            string URL = GOOGLECLOUDSTORAGEURL+GOOGLECLOUDSTORAGECREATEFOLDER;
            URL = string.Format(URL,bucketName, encodedfilepath);
            CallRESTAPI(URL, "POST");
        }
        internal void DeleteFolder(string folderPath)
        {
            string bucketName = folderPath?.Split('/')[0];
            string filepath = folderPath?.Substring(bucketName.Length).TrimStart('/');
            filepath = filepath.EndsWith("/") ? filepath : filepath + "/";
            filepath = filepath?.Replace(" ", "%20");
            string encodedfilepath = WebUtility.UrlEncode(filepath).Replace("%2520", "%20");
            string URL = GOOGLECLOUDSTORAGEURL + GOOGLECLOUDSTORAGEBUCKET;
            string URL1 = string.Format(URL, bucketName)+ "/o?delimiter=/&prefix="+encodedfilepath;
            URL = string.Format(URL, bucketName) + "/o/" + encodedfilepath;
            string response =CallRESTAPI(URL1, string.Empty, HttpMethod.GET, null);
            JObject jsonObject = JObject.Parse(response);
            bool check = (jsonObject["items"] is JArray itemsArray && itemsArray.Count == 1) ? itemsArray[0]["name"]?.ToString().EndsWith("/") == true : false;
            if (jsonObject["prefixes"] == null && check)
                CallRESTAPI(URL, string.Empty, HttpMethod.DELETE, null);
            else
            {
                throw new InvalidOperationException("Folder is not Empty To Delete");
            }

        }
        internal void DeleteObject(string filePath)
        {
            string bucketName = filePath?.Split('/')[0];
            string filepath = filePath?.Substring(bucketName.Length).TrimStart('/');
            filepath = filepath?.Replace(" ", "%20");
            string encodedfilepath = WebUtility.UrlEncode(filepath).Replace("%2520", "%20");
            string URL = GOOGLECLOUDSTORAGEURL + GOOGLECLOUDSTORAGEBUCKET;
            URL = string.Format(URL, bucketName) + "/o/" + encodedfilepath;
            CallRESTAPI(URL, null, HttpMethod.DELETE, null);
        }
        internal BucketDetails GetBucket(string bucketName)
        {
            string URL = GOOGLECLOUDSTORAGEURL + GOOGLECLOUDSTORAGEBUCKET;
            URL = string.Format(URL, bucketName);
            string response = CallRESTAPI(URL, null, HttpMethod.GET, null);
            string jsonresponse = GetBucketFormatJson(response);
            BucketDetails responsedata = ShUtil.JsonDeserialize<BucketDetails>(jsonresponse);
            return responsedata;
        }
        private string GetBucketFormatJson(string json)
        {
            JObject originalObject = JObject.Parse(json);
            JObject originalLabels = (JObject)originalObject["labels"];
            List<JObject> labelList = new List<JObject>();
            if (originalLabels != null)
            {
                foreach (var label in originalLabels)
                {
                    JObject labelObject = new JObject();
                    labelObject["name"] = label.Key;
                    labelObject["value"] = label.Value.ToString();
                    labelList.Add(labelObject);
                }
                originalObject["labels"] = new JArray(labelList);
            }
            return originalObject.ToString(Newtonsoft.Json.Formatting.Indented);
        }
        internal List<NameValue> GetBucketACL(string bucketName)
        {
            string URL = GOOGLECLOUDSTORAGEURL + GOOGLECLOUDSTORAGEBUCKET + "/acl";
            URL = string.Format(URL, bucketName);
            string response = CallRESTAPI(URL, null, HttpMethod.GET, null);
            dynamic jsonObject = JObject.Parse(response);
            jsonObject.BucketACL = jsonObject.items;
            jsonObject.Remove("items");
            jsonObject.Remove("kind");
            JObject rootObject = new JObject();
            rootObject["ArrayOfBucketACL"] = jsonObject;
            string jsonStringWithRoot = rootObject.ToString();
            return (GetResponseSchema(jsonStringWithRoot));

        }
        private List<NameValue> GetResponseSchema(string jsonString)
        {
            List<NameValue> response = new List<NameValue>();
            string xml = JsonConvert.DeserializeXmlNode(jsonString).OuterXml;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            response.Add(new NameValue(jsonText, doc.OuterXml));
            return response;
        }
        internal FileInformation GetObjectInformation(string filePath)
        {
            string bucketName = filePath?.Split('/')[0];
            string filepath = filePath?.Substring(bucketName.Length).TrimStart('/');
            filepath = filepath?.Replace(" ", "%20");
            string encodedfilepath = WebUtility.UrlEncode(filepath).Replace("%2520", "%20");
            string URL = GOOGLECLOUDSTORAGEURL + GOOGLECLOUDSTORAGEBUCKET+"/o/"+ encodedfilepath;
            URL = string.Format(URL, bucketName);
            string response = CallRESTAPI(URL, null, HttpMethod.GET, null);
            FileInformation responsedata =ShUtil.JsonDeserialize<FileInformation>(response);
            return responsedata;
        }
        internal List<NameValue> GetObjectACL(string filePath)
        {
            string bucketName = filePath?.Split('/')[0];
            string filepath = filePath?.Substring(bucketName.Length).TrimStart('/');
            filepath = filepath?.Replace(" ", "%20");
            string encodedfilepath = WebUtility.UrlEncode(filepath).Replace("%2520", "%20");
            string URL = GOOGLECLOUDSTORAGEURL + GOOGLECLOUDSTORAGEBUCKET + "/o/" + encodedfilepath+"/acl";
            URL = string.Format(URL, bucketName);
            string response = CallRESTAPI(URL, null, HttpMethod.GET, null);
            dynamic jsonObject = JObject.Parse(response);
            jsonObject.FileACL = jsonObject.items;
            jsonObject.Remove("items");
            jsonObject.Remove("kind");
            JObject rootObject = new JObject();
            rootObject["ArrayOfFileACL"] = jsonObject;
            string jsonStringWithRoot = rootObject.ToString();
            return (GetResponseSchema(jsonStringWithRoot));
        }
        internal List<NameValue> GetObjectMetaData(string filePath)
        {
            NameValue fileObj = ParaseBucketNameKey(filePath);
            string URL = GOOGLECLOUDSTORAGEURL + GOOGLECLOUDSTORAGEBUCKET + "/o/" + WebUtility.UrlEncode(fileObj.Value.ToString()).Replace("%2520", "%20") + GOOGLECLOUDSTORAGEGETOBJECTMETATDATA;
            URL = string.Format(URL, fileObj.Name);
            string response = CallRESTAPI(URL, null, HttpMethod.GET, null);           
            return GetObjectMetadataAsNameValue(response);
        }
        internal void SetBucketACL(string bucketName, string emailCategory, string email, string role)
        {
            string URL = GOOGLECLOUDSTORAGEURL + GOOGLECLOUDSTORAGEBUCKET + "/acl";
            URL = string.Format(URL, bucketName);
            string Entity = emailCategory.ToLower() + "-" + email;
            var body = new { entity = Entity, role = role.ToUpper() };
            CallRESTAPI(URL, ShUtil.JsonSerialize(body), HttpMethod.POST, null);
        }
        internal void SetObjectACL(string filePath, string emailCategory,string email, string role)
        {
            NameValue fileObj = ParaseBucketNameKey(filePath);
            string URL = GOOGLECLOUDSTORAGEURL + GOOGLECLOUDSTORAGEBUCKET + "/o/" + WebUtility.UrlEncode(fileObj.Value.ToString()).Replace("%2520", "%20") +"/acl";
            URL = string.Format(URL, fileObj.Name);
            string Entity = emailCategory.ToLower() + "-" + email;
            var body = new { entity = Entity, role = role.ToUpper() };
            CallRESTAPI(URL, ShUtil.JsonSerialize(body), HttpMethod.POST, null);
        }

        internal void SetObjectMetaData(string filePath, List<NameValue> metaDataValues)
        {
            NameValue fileObj = ParaseBucketNameKey(filePath);
            string URL = GOOGLECLOUDSTORAGEURL + GOOGLECLOUDSTORAGEBUCKET + "/o/" + WebUtility.UrlEncode(fileObj.Value.ToString()).Replace("%2520", "%20");
            URL = string.Format(URL, fileObj.Name);
            string body = PrepareJSONBody(metaDataValues);
            CallRESTAPI(URL, body, HttpMethod.PATCH, null);
        }

        internal void RemoveBucketACL(string bucketName,string emailCategory, string email)
        {
            string URL = GOOGLECLOUDSTORAGEURL + GOOGLECLOUDSTORAGEBUCKET + "/acl/"+emailCategory+"-"+email;
            URL = string.Format(URL, bucketName);
            CallRESTAPI(URL, null, HttpMethod.DELETE, null);
        }

        internal void RemoveObjectACL(string filePath, string emailCategory, string email)
        {
            string bucketName = filePath?.Split('/')[0];
            string filepath = filePath?.Substring(bucketName.Length).TrimStart('/');
            filepath = filepath?.Replace(" ", "%20");
            string encodedfilepath = WebUtility.UrlEncode(filepath).Replace("%2520", "%20");
            string URL = GOOGLECLOUDSTORAGEURL + GOOGLECLOUDSTORAGEBUCKET + "/o/"+ encodedfilepath + "/acl/" + emailCategory + "-" + email;
            URL = string.Format(URL, bucketName);
            CallRESTAPI(URL, null, HttpMethod.DELETE, null);
        }
        private List<NameValue> GetObjectMetadataAsNameValue(string jsonResponse)
        {
            JObject data = JObject.Parse(jsonResponse);
            if (data["metadata"] != null)
            {
                foreach (JProperty property in data["metadata"].Children())
                {
                    data[property.Name] = property.Value;
                }
                data["metadata"].Parent.Remove();
            }
            string modifiedJson = data.ToString();
            Dictionary<string, string> nameValue = ShUtil.JsonDeserialize<Dictionary<string, string>>(modifiedJson);
            List<NameValue> nameValueList = nameValue
            .Select(kvp => new NameValue(kvp.Key, kvp.Value)).ToList();

            return nameValueList;
        }
        private string PrepareJSONBody(List<NameValue> metaDataValues)
        {
            List<string> Properties = new List<string>
            {
            "contentType",
            "contentEncoding",
            "contentDisposition",
            "contentLanguage",
            "customTime",
            "cacheControl"
            };

            Dictionary<string, string> directProperties = new Dictionary<string, string>();
            Dictionary<string, string> metadataProperties = new Dictionary<string, string>();
            foreach (var item in metaDataValues)
            {
                if (Properties.Contains(item.Name))
                {
                    directProperties[item.Name] = item.Value.ToString();
                }
                else
                {
                    metadataProperties[item.Name] = item.Value.ToString();
                }
            }
            string directPropertiesJson = JsonConvert.SerializeObject(directProperties, Newtonsoft.Json.Formatting.Indented).TrimEnd('}');
            if (directPropertiesJson != "{" && metadataProperties.Count > 0) { directPropertiesJson += ","; }
            string metadataJson = metadataProperties.Count > 0 ? JsonConvert.SerializeObject(metadataProperties, Newtonsoft.Json.Formatting.Indented) + "}" : "";
            metadataJson = string.IsNullOrEmpty(metadataJson) ? "" : "\"metadata\":" + metadataJson;
            if (string.IsNullOrEmpty(metadataJson) && directPropertiesJson != "{") throw new Exception("metadata cannot be empty");
            return directPropertiesJson + Environment.NewLine + metadataJson;

        }
        private NameValue ParaseBucketNameKey(string ObjectName)
        {
            string bucketName = string.Empty;
            string key = string.Empty;

            if (ObjectName.Contains("/") && !Path.HasExtension(ObjectName))
            {
                bucketName = ObjectName.Split('/')[0];
                key = ObjectName.Substring(bucketName.Length).TrimStart('/');
                key = key?.Replace(" ", "%20");
            }
            if (ObjectName.Contains("/") && Path.HasExtension(ObjectName))
            {
                bucketName = ObjectName.Split('/')[0];
                key = ObjectName.Substring(bucketName.Length).TrimStart('/');
                key = key?.Replace(" ", "%20");

            }
            if (!ObjectName.Contains("/") && !Path.HasExtension(ObjectName))
            {
                bucketName = ObjectName;
            }
            NameValue bucketKey = new NameValue();
            bucketKey.Name = bucketName;
            bucketKey.Value = key;
            return bucketKey;
        }
        private bool CheckIsValidMail(string Mail)
        {
            try
            {
                System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(Mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private string CallRESTAPI(string url,string httpmethod)
        {
            agilePartDebugLog.WriteDebugLog($"Url : {url}", "");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = httpmethod;
            string accesstoken = BearerAuthentication.CreateBearerAuthHeader(m_Accesskey).Value.ToString();
            request.Headers.Add("Authorization", accesstoken);
            byte[] requestBodyBytes = Encoding.UTF8.GetBytes("");
            request.ContentLength = requestBodyBytes.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
            }
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string apiResponse = reader.ReadToEnd();
                    return apiResponse;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (Stream errorStream = ex.Response.GetResponseStream())
                    using (StreamReader errorReader = new StreamReader(errorStream))
                    {
                        string errorResponse = errorReader.ReadToEnd();
                        throw new DataException($"Error response: {errorResponse}");
                    }
                }
                else
                {
                    throw new DataException($"Request error: {ex.Message}");

                }
            }
        }
        private string CallRESTAPI(string url, string payload, HttpMethod httpMethod, List<NameValue> RequestHeaders)
        {
          
            try
            {
                Request request = HttpHelper.PrepareRestClientRequest(url, payload, httpMethod, string.Empty);
                AppendHeaders(RequestHeaders, request);
                return HttpHelper.MakeHTTPRequestByRestClient(request);
            }
            catch(Exception ex)
            {
                JObject data = JObject.Parse(ex.Message.ToString());
                string errorMessage = (string)data["error"]["message"];
                throw new InvalidOperationException(errorMessage);
            }
        }
        private (string ResponseData, List<NameValue> ResponseHeaders) CallRestAPI(string url, string payload, HttpMethod httpMethod, List<NameValue> RequestHeaders)
        {
            Request request = HttpHelper.PrepareRestClientRequest(url, payload, httpMethod, string.Empty);
            AppendHeaders(RequestHeaders, request);
            return MakeHTTPRequestByRestClient(request);
        }
        private void AppendHeaders(List<NameValue> ResponseHeaders, Request request)
        {
            string accesstoken = BearerAuthentication.CreateBearerAuthHeader(m_Accesskey).Value.ToString();
            request.RequestHeaders.Add("Authorization", accesstoken);
            if (ResponseHeaders != null)
            {
                foreach (var header in ResponseHeaders)
                {
                    if (!header.Name.IsNullOrEmptyOrWhiteSpace() && header.Value?.ToString().IsNullOrEmptyOrWhiteSpace() != null)
                    {
                        request.RequestHeaders.Add(header.Name, header.Value.ToString());
                    }
                }
            }

        }
        public static (string ResponseData, List<NameValue> ResponseHeaders) MakeHTTPRequestByRestClient(Request HttpRequest)
        {
            IRESTClient Client = null;
            try
            {
                Client = RestFactory.CreateRestClient(HttpRequest);
                Client.RequestServerResponse();
                return (Client.GetResponseData(), Client.GetResponseHeader());
            }
            catch (WebException webEx)
            {
                WebResponse res = webEx.Response;
                string message = "";
                if (res != null)
                {
                    using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                    {
                        message = sr.ReadToEnd();
                    }
                }
                throw new WebException(message, webEx);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Client?.Response != null)
                    Client.Response.Close();

            }
        }
        #region Logger
       
        #endregion
    }

}