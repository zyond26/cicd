using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon;

public class MinioSettings
{
    public string Endpoint { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string BucketName { get; set; }
}

public class MinioService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public MinioService(IConfiguration configuration)
    {
        var endpoint = configuration["MinioSettings:Endpoint"];
        var accessKey = configuration["MinioSettings:AccessKey"];
        var secretKey = configuration["MinioSettings:SecretKey"];
        _bucketName = configuration["MinioSettings:BucketName"];

        var config = new AmazonS3Config
        {
            ServiceURL = endpoint,
            ForcePathStyle = true // Cần thiết với MinIO
        };

        _s3Client = new AmazonS3Client(accessKey, secretKey, config);
    }

    public async Task UploadFileAsync(Stream fileStream, string fileName)
    {
        var fileTransfer = new TransferUtility(_s3Client);
        await fileTransfer.UploadAsync(fileStream, _bucketName, fileName);
    }
}
