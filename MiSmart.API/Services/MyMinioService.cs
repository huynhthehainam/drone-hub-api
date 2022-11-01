using Microsoft.Extensions.Options;
using Minio;
using MiSmart.Infrastructure.Minio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MiSmart.API.Services;

public class MyMinioService : MinioService
{
    // public MinioSettings Settings { get; set; }
    private MinioClient minioClient;
    public MyMinioService(IOptions<MinioSettings> options) : base(options)
    {
        this.Settings = options.Value;
        this.minioClient = new MinioClient().WithEndpoint(Settings.Hostname).WithCredentials(Settings.AccessKey, Settings.SecretKey).Build();

    }
    
    public async Task<String> CopyObjectAsync(String url, String[] paths)
    {
        String srcPath = GetFilePathFromUrl(url);
        var ext = Path.GetExtension(srcPath);

        var randomFileName = $"{Path.GetRandomFileName()}{ext}";
        var temp = String.Join("/", paths);
        String desPath = $"{temp}/{randomFileName}";
        
        CopySourceObjectArgs source = new CopySourceObjectArgs().WithBucket(Settings.BucketName).WithObject(srcPath);
        CopyObjectArgs copyObjectArgs = new CopyObjectArgs().WithBucket(Settings.BucketName).WithObject(desPath).WithCopyObjectSource(source);

        await minioClient.CopyObjectAsync(copyObjectArgs);

        return $"https://{Settings.Hostname}/{Settings.BucketName}/{desPath}";
    }
}
