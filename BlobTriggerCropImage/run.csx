using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
public static string GetEnvironmentVariable(string name)
{
    return name + ": " + 
        System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
}
public static void Run(Stream myBlob, string blobname, Stream croppedimage,out object processDocument, TraceWriter log)
{ 
    log.Info($" C# Blob trigger Function ");
    log.Info($" Name of the uploaded image : {blobname} \n Size: {myBlob.Length} Bytes ");
    int width = 240;
    int height = 240;
    bool smartCropping = true;
    string _apiKey = System.Environment.GetEnvironmentVariable("AZURE_COMPVISION_apiKey",EnvironmentVariableTarget.Process);
    string _apiUrlBase = System.Environment.GetEnvironmentVariable("AZURE_COMPVISION_URL",EnvironmentVariableTarget.Process);
    _apiUrlBase = _apiUrlBase + "generateThumbnail";
    using (var httpClient = new HttpClient())
    {
        httpClient.BaseAddress = new Uri(_apiUrlBase);
        httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
        using (HttpContent content = new StreamContent(myBlob))
        {
            //get response headers
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");
            var uri = $"{_apiUrlBase}?width={width}&height={height}&smartCropping={smartCropping.ToString()}";
            var response = httpClient.PostAsync(uri, content).Result;
            var responseBytes = response.Content.ReadAsByteArrayAsync().Result;
            // write to output blob storage
            croppedimage.Write(responseBytes, 0, responseBytes.Length);
            processDocument = "{\"imagename\": \""+blobname+"\",\"imagecropped\": true }";
        }
    }
    log.Info("Image Cropped");
}
