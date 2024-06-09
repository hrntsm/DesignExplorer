using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DesignExplorer
{
    public class Server
    {
        static async Task Main(string[] args)
        {
            LogMessage($"Wake Tunny DesignExplorer Server");
            string port = args.Length > 0 ? args[0] : "8080";
            var listener = new HttpListener();
            listener.Prefixes.Add($"http://127.0.0.1:{port}/");
            listener.Start();
            LogMessage($"Listening on http://127.0.0.1:{port}/");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                string filePath = "." + request.Url.LocalPath;
                LogMessage($"Requested path: {filePath}");
                if (request.Url.LocalPath == "/")
                {
                    response.Redirect("/index.html");
                    LogMessage("Redirected to /index.html", response.StatusCode);
                }
                else if (File.Exists(filePath))
                {
                    string extension = Path.GetExtension(filePath);
                    string contentType = GetContentType(extension);

                    response.Headers.Add("Content-Type", contentType);

                    byte[] buffer = File.ReadAllBytes(filePath);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    LogMessage($"Served file: {filePath}", response.StatusCode);
                }
                else
                {
                    response.StatusCode = 404;
                    LogMessage($"File not found: {filePath}", response.StatusCode);
                }

                response.OutputStream.Close();
            }
        }

        static void LogMessage(string message, int statusCode = 0)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            string logMessage = $"[{timestamp}] {message}";

            if (statusCode != 0)
            {
                logMessage += $" (Status Code: {statusCode})";
            }

            Console.WriteLine($"[{timestamp}] {logMessage}");
        }

        static string GetContentType(string extension)
        {
            switch (extension)
            {
                case ".html":
                    return "text/html";
                case ".css":
                    return "text/css";
                case ".js":
                    return "application/javascript";
                case ".json":
                    return "application/json";
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
