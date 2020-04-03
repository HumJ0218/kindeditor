using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KindEditor.AppBuilder
{
	public static partial class KindEditorAppBuilderExtensions
	{
		private static Task fileManagerJson(HttpContext context)
		{
			var dir = context.Request.Query["dir"].ToString();
			var path = context.Request.Query["path"].ToString();
			var order = context.Request.Query["order"].ToString().ToLower();

			var rootPath = Options.FileUploadPath;
			var rootUrl = Options.FileResponsePath;
			var imageFileTypes = Options.FileTypeExtension["image"];

			var currentPath = "";
			var currentUrl = "";
			var currentDirPath = "";
			var moveupDirPath = "";

			var dirPath = rootPath;
			var dirName = dir;
			if (!string.IsNullOrEmpty(dirName))
			{
				if (!Options.FileTypeExtension.ContainsKey(dirName))
				{
					return JsonErrorResponse(context, "目录名称无效");
				}
				dirPath += "/" + dirName;
				rootUrl += "/" + dirName;
				if (!Directory.Exists(dirPath))
				{
					Directory.CreateDirectory(dirPath);
				}
			}

			//根据path参数，设置各路径和URL
			path = string.IsNullOrEmpty(path) ? "" : path;
			if (path == "")
			{
				currentPath = dirPath + "/";
				currentUrl = rootUrl + "/";
				currentDirPath = "";
				moveupDirPath = "";
			}
			else
			{
				currentPath = dirPath + "/" + path;
				currentUrl = rootUrl + "/" + path;
				currentDirPath = path;
				moveupDirPath = Regex.Replace(currentDirPath, @"(.*?)[^\/]+\/$", "$1");
			}

			//不允许使用..移动到上一级目录
			if (Regex.IsMatch(path, @"\.\."))
			{
				return JsonErrorResponse(context, "不允许使用..移动到上一级目录");
			}
			//最后一个字符不是/
			if (path != "" && !path.EndsWith("/"))
			{
				return JsonErrorResponse(context, "路径参数无效");
			}
			//目录不存在或不是目录
			if (!Directory.Exists(currentPath))
			{
				return JsonErrorResponse(context, "目录不存在或不是目录");
			}

			//遍历目录取得文件信息
			var di = new DirectoryInfo(currentPath);
			var dirList = di.EnumerateDirectories();
			var fileList = di.EnumerateFiles();

			dirList = dirList.OrderBy(m => m.Name);

			switch (order)
			{
				case "size":
					{
						fileList = fileList.OrderBy(m => m.Length);
						break;
					}
				case "type":
					{
						fileList = fileList.OrderBy(m => m.Extension);
						break;
					}
				case "name":
				default:
					{
						fileList = fileList.OrderBy(m => m.Name);
						break;
					}
			}

			var dirFileList = new List<object>(dirList.Select(dir =>
			{
				return new
				{
					is_dir = true,
					has_file = dir.EnumerateDirectories().Count() + dir.EnumerateFiles().Count() > 0,
					filesize = 0,
					is_photo = false,
					filetype = "",
					filename = dir.Name,
					datetime = dir.LastWriteTime.ToShortDateString() + " " + dir.LastWriteTime.ToShortTimeString(),
				};
			})).Concat(fileList.Select(file =>
			{
				return new
				{
					is_dir = false,
					has_file = false,
					filesize = file.Length,
					is_photo = imageFileTypes.Contains(file.Extension.TrimStart('.').ToLower()),
					filetype = file.Extension.Substring(1),
					filename = file.Name,
					datetime = file.LastWriteTime.ToShortDateString() + " " + file.LastWriteTime.ToShortTimeString(),
				};
			}));

			var result = new
			{
				moveup_dir_path = moveupDirPath,
				current_dir_path = currentDirPath,
				current_url = currentUrl,
				total_count = dirList.Count() + fileList.Count(),
				file_list = dirFileList,
			};

			return JsonResponse(context, result);
		}
	}
}