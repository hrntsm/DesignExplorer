using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

class Server
{
    static async Task Main(string[] args)
    {
        Console.WriteLine($"Wake Tunny DesignExplorer Server");
        string port = args.Length > 0 ? args[0] : "8080";
        var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{port}/");
        listener.Start();
        Console.WriteLine($"Listening on http://localhost:{port}/");

        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string filePath = "." + request.Url.LocalPath;
            Console.WriteLine($"Requested path: {filePath}");
            if (request.Url.LocalPath == "/")
            {
                response.Redirect("/index.html");
                Console.WriteLine("Redirected to /index.html");
            }
            else if (File.Exists(filePath))
            {
                string extension = Path.GetExtension(filePath);
                string contentType = GetContentType(extension);

                response.Headers.Add("Content-Type", contentType);

                byte[] buffer = File.ReadAllBytes(filePath);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                Console.WriteLine($"Served file: {filePath}");
            }
            else
            {
                response.StatusCode = 404;
                Console.WriteLine($"File not found: {filePath}");
            }

            response.OutputStream.Close();
        }
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
