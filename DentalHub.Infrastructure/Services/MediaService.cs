using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DentalHub.Application.Interfaces;
using DentalHub.Infrastructure.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DentalHub.Infrastructure.Services
{
    public class MediaService : IMediaService
    {
        private readonly Cloudinary _cloud;
        public MediaService( IOptions<CloudinarySettings>config)
        {
            var result = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
                );
            _cloud = new Cloudinary( result );
            
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return  false;

            var deleteParams = new DeletionParams(publicId);


            var result = await _cloud.DestroyAsync(deleteParams);

            return result.Result == "ok";
        }

        public async Task<ImageUploadDto> UploadImageAsync(IFormFile file)
        {
         
            if(file==null || file.Length==0)
            {
                throw new Exception("File is Empty");
            }
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.Name, stream),
                Folder = "DentalHub"
            };
            var result =await _cloud.UploadAsync(uploadParams);

            if(result.Error!=null)
            {
                throw new Exception(result.Error.Message);
            }
            return new ImageUploadDto
            {
                Url = result.SecureUrl.ToString(),
                PublicId=result.PublicId
            };


        }
    }
}
