using System.IO;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Extensions;
using Microsoft.AspNetCore.Http;

namespace Kztek_Library.Helpers
{
    public class UploadHelper
    {
        public static async Task<MessageReport> UploadFile(IFormFile file, string path)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                using (var fileStream = new FileStream(path + "/" + file.FileName, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                result = new MessageReport(true, "Upload thành công");
            }
            catch (System.Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return result;
        }

        public static async Task<string> GetFileNameNormalize(IFormFile file, string folderpath = "")
        {
            var extension = Path.GetExtension(file.FileName) ?? "";

            var fileName = Path.GetFileName(string.Format("{0}{1}", StringUtilHelper.RemoveSpecialCharactersVn(file.FileName.Replace(extension, "")).GetNormalizeString(), extension));

            var folder = await AppSettingHelper.GetStringFromAppSetting("FileUpload:CustomerFolder");

            var path = string.Format("{0}{1}/{2}", folder, folderpath, fileName);

            return path;
        }
    }
}