using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static KindEditor.AppBuilder.KindEditorAppBuilderExtensions;

namespace KindEditor.AppBuilder
{
    public class KindEditorOptions
    {
        /// <summary>
        /// 获取 API 调用地址列表
        /// </summary>
        public Dictionary<string, string> ApiPath => PageList.ToDictionary(m => m.Value.GetMethodInfo().Name, m => Options.ApiPrefix + m.Key);

        /// <summary>
        /// 获取或设置 API 地址前缀
        /// </summary>
        public string ApiPrefix { get; set; } = "/kindeditor";

        /// <summary>
        /// 获取或设置文件保存目录路径
        /// </summary>
        public string FileUploadPath { get; set; } = "./wwwroot/attached";

        /// <summary>
        /// 获取或设置文件浏览目录URL
        /// </summary>
        public string FileResponsePath { get; set; } = "/attached";

        /// <summary>
        /// 获取或设置允许上传的文件分类及其扩展名
        /// </summary>
        public Dictionary<string, string[]> FileTypeExtension { get; set; } = new Dictionary<string, string[]> {
            { "image", "bmp,jpg,jpeg,gif,png,svg,tif,tiff".Split(',') },
            { "flash", "fla,swf".Split(',') },
            { "media", "wav,mp3,wma,ogg,flac,mid,mod,xm,avi,wmv,mp4,mkv,mpg,rm,rmvb,asf".Split(',') },
            { "file", "doc,docx,xls,xlsx,ppt,pptx,txt,zip,rar,gz,torrent,xml,html,lrc,lrcx".Split(',') },
        };

        /// <summary>
        /// 获取或设置默认分类文件夹
        /// </summary>
        public string DefaultTypeFolderName { get; set; } = "image";

        /// <summary>
        /// 获取或设置最大文件大小
        /// </summary>
        public long MaxFileSize { get; set; } = 4 * 1024 * 1024;

        /// <summary>
        /// 获取或设置子文件夹命名格式
        /// </summary>
        public string SubFolderFormat { get; set; } = "yyyyMMdd";

        /// <summary>
        /// 获取或设置保存文件命名格式
        /// </summary>
        public string FileNameFormat { get; set; } = "HHmmssfff";
    }
}