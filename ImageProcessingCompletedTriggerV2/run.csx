#r "Microsoft.Azure.Documents.Client"
#r "Newtonsoft.Json"
#r "SendGrid"
using System;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using Microsoft.Azure.Documents; 
using System.Collections.Generic; 
 
public static void Run(IReadOnlyList<Document> input,out Mail message, TraceWriter log)
{
    var imagename="";
    var imagecropped="";
    var imagemetadata="";
    var messagecontent = "";
    if (input != null && input.Count > 0) 
    {
         log.Verbose("Documents modified " + input.Count);
        for(int k = 0;k<input.Count;++k ) {
            log.Verbose("Document Id " + input[k].Id); 
            log.Verbose("Document is " + input[k].ToString());
            dynamic d = Newtonsoft.Json.Linq.JObject.Parse(input[k].ToString());
            imagename =  d["imagename"];
            imagecropped = d["imagecropped"];
            imagemetadata = d["metadata"];
            log.Info(imagecropped + " , " + imagemetadata);
            if(imagecropped=="True")
                messagecontent = messagecontent +  " Image " + imagename + " has been cropped  "+ Environment.NewLine;
            if(imagemetadata == "True")
                messagecontent =  messagecontent + " Image " + imagename + " metadata has been extracted "+Environment.NewLine;  
            imagecropped="";
            imagemetadata="";  
        }
    }
    message = new Mail
    {          
        Subject = "[Image Processing Completed]"          
    };
    var personalization = new Personalization();
    personalization.AddTo(new Email("vereshjain@gmail.com"));   
    Content content = new Content
    { 
        Type = "text/plain",  
        Value = messagecontent
    };
    message.AddContent(content);
    message.AddPersonalization(personalization);
} 
