
// minio controller ===> update minio to bucket cloud

using Microsoft.AspNetCore.Mvc;

public class FileController : Controller
{
    private readonly MinioService _minioService;

    public FileController(MinioService minioService)
    {
        _minioService = minioService;
    }

    [HttpGet]
    public IActionResult Upload()
    {
        return View(); // Trả về giao diện upload
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            await _minioService.UploadFileAsync(stream, file.FileName);
            ViewBag.Message = "Upload thành công!";
        }
        return View();
    }
}
