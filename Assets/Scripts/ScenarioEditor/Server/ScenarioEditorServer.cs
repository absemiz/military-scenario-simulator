using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Newtonsoft.Json;

public class ScenarioEditorServer : MonoBehaviour
{
    private HttpListener Listener;

    [SerializeField]
    private int Port = 8080;

    private InitializationMessage LastInitializationMessage = null;

    public InitializationMessage GetInitializationMessage() { return LastInitializationMessage; }

    void Start()
    {
        Listener = new HttpListener();

        string root = $"http://localhost:{Port}/";
        string api = $"{root}milsimapi/";

        Listener.Prefixes.Add(root);
        Listener.Prefixes.Add(api);

        Listener.Start();
        Debug.Log($"Server listening on {root}");
        Debug.Log($"Server listening on {api}");
        Listener.BeginGetContext(new AsyncCallback(OnRequest), Listener);
    }

    private void OnRequest(IAsyncResult result)
    {
        var context = Listener.EndGetContext(result);
        ProcessRequest(context); 
        Listener.BeginGetContext(new AsyncCallback(OnRequest), Listener);
    }

    private void ProcessRequest(HttpListenerContext context)
    {
        string path = context.Request.Url.AbsolutePath;
        string rootFilePath = $"{Application.dataPath}/ScenarioEditor/dist";

        string filePath = "";

        if (path == "/")
        {
            filePath = $"{rootFilePath}/index.html";
            context.Response.ContentType = "text/html";
        }
        else if (path.EndsWith(".js"))
        {
            filePath = $"{rootFilePath}/assets/index-Dp-oFPFX.js";
            context.Response.ContentType = "application/javascript";
        }
        else if (path.EndsWith(".css"))
        {
            filePath = $"{rootFilePath}/assets/index-CgUMRhAo.css";
            context.Response.ContentType = "text/css";
        }
        else if (path.EndsWith(".svg"))
        {
            filePath = $"{rootFilePath}/MarsSymbol.svg";
            context.Response.ContentType = "image/svg+xml"; 
        }
        else if (path == "/milsimapi")
        {
            InitializationMessage parsedMessage = ParseJsonRequest(context.Request);

            if (parsedMessage != null)
            {
                string responseMessage = "{\"status\": \"Message OK!\"}";
                byte[] rawResponseMessage = System.Text.Encoding.UTF8.GetBytes(responseMessage);

                context.Response.OutputStream.Write(rawResponseMessage, 0, rawResponseMessage.Length);

                Debug.Log(parsedMessage.Entities[0].Type);

                LastInitializationMessage = parsedMessage;
            }
            else
            {
                Debug.Log("Parsed message is null.");
            }
        }
        else
        {
            Debug.Log("Error on request.");
        }

        if (File.Exists(filePath))
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            using var fileStream = File.OpenRead(filePath);
            fileStream.CopyTo(context.Response.OutputStream);
        }

        context.Response.Close();
    }

    public static InitializationMessage ParseJsonRequest(HttpListenerRequest request)
    {
        using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
        string jsonString = reader.ReadToEnd();

        return JsonConvert.DeserializeObject<InitializationMessage>(jsonString);
    }

    private void OnApplicationQuit()
    {
        Listener.Stop();
    }
}


