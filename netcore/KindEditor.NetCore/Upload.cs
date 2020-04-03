using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KindEditor.AppBuilder
{
	public static partial class KindEditorAppBuilderExtensions
	{
		private static Task uploadJson(HttpContext context)
		{
			var dirName = context.Request.Query["dir"];
			var imgFile = context.Request.Form.Files["imgFile"];

			var savePath = Options.FileUploadPath;
			var saveUrl = Options.FileResponsePath;
			var extTable = Options.FileTypeExtension;
			var maxSize = Options.MaxFileSize;
			var defaultTypeFolderName = Options.DefaultTypeFolderName;
			var subFolderFormat = Options.SubFolderFormat;
			var fileNameFormat = Options.FileNameFormat;

			//	校验
			if (imgFile == null)
			{
				return JsonErrorResponse(context, "请选择文件。");
			}

			var dirPath = savePath;
			if (!Directory.Exists(dirPath))
			{
				return JsonErrorResponse(context, "上传目录不存在。");
			}

			if (string.IsNullOrEmpty(dirName))
			{
				dirName = defaultTypeFolderName;
			}
			if (!extTable.ContainsKey(dirName))
			{
				return JsonErrorResponse(context, "目录名不正确。");
			}

			var fileName = imgFile.FileName;
			var fileExt = Path.GetExtension(fileName).ToLower();

			using var inputStream = imgFile.OpenReadStream();
			if (inputStream == null || inputStream.Length > maxSize)
			{
				return JsonErrorResponse(context, "上传文件大小超过限制。");
			}

			if (string.IsNullOrEmpty(fileExt) || !extTable[dirName].Contains(fileExt.TrimStart('.')))
			{
				return JsonErrorResponse(context, "上传文件扩展名不正确。\r\n只允许 " + string.Join(", ", extTable[dirName]) + " 格式。");
			}

			//	创建文件夹
			var now = DateTime.Now;

			dirPath += "/" + dirName;
			saveUrl += "/" + dirName;
			if (!Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
			}

			var ymd = now.ToString(subFolderFormat);
			dirPath += "/" + ymd;
			saveUrl += "/" + ymd;
			if (!Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
			}

			var newFileName = now.ToString(fileNameFormat) + fileExt;
			var filePath = dirPath + "/" + newFileName;

			//	保存文件
			using var fileStream = File.Create(filePath);
			inputStream.CopyTo(fileStream);
			fileStream.Close();
			inputStream.Close();

			//	文件访问url
			var fileUrl = saveUrl + "/" + newFileName;

			return JsonResponse(context, new
			{
				error = 0,
				url = fileUrl,
			});
		}
	}
}