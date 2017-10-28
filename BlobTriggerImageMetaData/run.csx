#r "NewtonSoft.Json"
using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
 
public static void Run(Stream myBlob, string blobname, out object outputDocument,out object processDocument,
 TraceWriter log)
{ 
    log.Info($"C# Azure function for MetaData triggered");
    log.Info($"Name of the uploaded image : {blobname} \n Size: {myBlob.Length} Bytes");
    string _apiKey = System.Environment.GetEnvironmentVariable("AZURE_COMPVISION_apiKey",EnvironmentVariableTarget.Process);
    string _apiUrlBase = System.Environment.GetEnvironmentVariable("AZURE_COMPVISION_URL",EnvironmentVariableTarget.Process);
    _apiUrlBase = _apiUrlBase + "analyze";
    log.Info(_apiUrlBase);
    using (var httpClient = new HttpClient())  
    {
        httpClient.BaseAddress = new Uri(_apiUrlBase);
        httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
        using (HttpContent content = new StreamContent(myBlob))
        {
            //get response headers
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");
            var uri = $"{_apiUrlBase}?visualFeatures=Categories,Description,Color&language=en";
            var response = httpClient.PostAsync(uri, content).Result;
            string metadata = response.Content.ReadAsStringAsync().Result;
            var obj = JObject.Parse(metadata);
            obj.Add(new JProperty("imagename",blobname));
            log.Info("MetaData Response" + obj);
            outputDocument = obj.ToString();  
            processDocument = "{\"imagename\": \""+blobname+"\",\"metadata\": true }";
        }
    }
    log.Info("MetaData of the image extracted" + blobname); 
}