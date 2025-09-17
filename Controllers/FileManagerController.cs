using FilingSystem.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.StaticFiles;
using System;

namespace FilingSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileManagerController : ControllerBase
    {
        private const string RootFolder = "ContentFolders";

        public static string GetExtension(string file_name)
        {
            string extension = ".";

            for (int i = 0; i < file_name.Length - 1; i++)
            {
                if (file_name[i] == '.')
                {
                    extension = file_name.Remove(0, i + 1);

                }

            }
            return extension;



        }


        [HttpPost("FileUpload")]
        public IActionResult FileUpload(IFormFile file, string folder)
        {

            var current_dir = Directory.GetCurrentDirectory();

            var path = Path.Combine(current_dir, RootFolder, folder);

            if (!Directory.Exists(path))
            {
                return BadRequest("Directory not found");
            }

            using var sr = file.OpenReadStream();

            using FileStream fs = System.IO.File.Create(Path.Combine(path, file.FileName));

            sr.CopyTo(fs);

            return Ok(true);
        }

        [HttpPost("FolderCreate")]
        public bool FolderCreate(string folder_name)
        {
            var current_dir = Directory.GetCurrentDirectory();

            var path = Path.Combine(current_dir, RootFolder, folder_name);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return true;
        }

        [HttpGet("SearchFile")]
        public IActionResult GetFiles(string folder_name)

        {

            string bad_request = "DIRECTORY DOES NOT EXIST";

            List<string> files_name = new List<string>();


            if (!Directory.Exists($@"ContentFolders\{folder_name}"))
            {
                return BadRequest(bad_request);
            }
            else
            {
                string path = $@"C:\Users\tigra\source\repos\FilingSystem.Server\ContentFolders\{folder_name}";

                string[] files = Directory.GetFiles(path);

                foreach (string file in files)
                {
                    files_name.Add(Path.GetFileName(file));
                }


                return Ok(files_name);
            }
        }
        [HttpGet("DownloadFile")]
        public async Task<HttpResponseMessage> DownloadFile(string folder_name, string file_name, string dwnld_path)
        {
            string path = $@"C:\Users\tigra\source\repos\FilingSystem.Server\ContentFolders\{folder_name}\{file_name}";

            string download_path = $@"{dwnld_path}\{file_name}";


            //string mimeType = GetMimeType(GetExtension(file_name));

            if (!System.IO.File.Exists(path))
            {
                var bad_request = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                bad_request.Content = new StringContent("This file does not exist");
            }

            var memory = new MemoryStream();



            var stream = new FileStream(download_path, FileMode.OpenOrCreate);
            byte[] buffer = System.IO.File.ReadAllBytes(path);



            using (stream)
            {
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }

            stream.Close();

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);


            return response;


        }
    }
}