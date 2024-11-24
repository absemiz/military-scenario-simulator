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
    private HttpListener listener;

    [SerializeField]
    private int port = 8080;

    void Start()
    {
        listener = new HttpListener();

        string root = $"http://localhost:{port}/";
        string api = $"{root}milsimapi/";

        listener.Prefixes.Add(root);
        listener.Prefixes.Add(api);

        listener.Start();
        Debug.Log("Server started at " + root);
        listener.BeginGetContext(new AsyncCallback(OnRequest), listener);
    }

    private void OnRequest(IAsyncResult result)
    {
        var context = listener.EndGetContext(result);
        ProcessRequest(context); 
        listener.BeginGetContext(new AsyncCallback(OnRequest), listener);
    }

    private void ProcessRequest(HttpListenerContext context)
    {
        string path = context.Request.Url.AbsolutePath;
        string rootFilePath = $"{Application.dataPath}/ScenarioEditor/dist";
        Debug.Log(rootFilePath);

        string filePath = "";

        if (path == "/")
        {
            filePath = $"{rootFilePath}/index.html";
            context.Response.ContentType = "text/html";
        }
        else if (path.EndsWith(".js"))
        {
            filePath = $"{rootFilePath}/assets/index-1jdui-py.js";
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
            Debug.Log("Request received.");
            Debug.Log(context.Request);
            InitializationMessage parsedMessage = ParseJsonRequest(context.Request);

            if (parsedMessage != null)
            {
                Debug.Log("Message OK!");

                foreach (var entry in parsedMessage.Entities)
                {
                    Debug.Log($"ID: {entry.ID}, Latitude: {entry.Latitude}, Longitude: {entry.Longitude}");
                }

                foreach (var entry in parsedMessage.Paths)
                {
                    Debug.Log(entry.Points);
                    Debug.Log(entry.ID);
                }

                foreach (var entry in parsedMessage.Waypoints)
                {
                    Debug.Log(entry.Position);
                    Debug.Log(entry.ID);
                }
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
        listener.Stop();
    }
}


