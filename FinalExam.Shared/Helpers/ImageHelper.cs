using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System;


namespace FinalExam.Shared.Helpers
{
    public class ImageHelper
    {
        public static async Task<byte[]> SaveAndResizeProfilePhotoAsync(IFormFile profilePhoto)
        {
            if (profilePhoto == null || profilePhoto.Length == 0)
                throw new ArgumentException("Profile photo cannot be null or empty");

            if (!IsImage(profilePhoto))
                throw new ArgumentException("Profile photo must be an image file"); //Checks if the uploaded file is an image.

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images"); //Constructs directory where image will be saved
            var fileName = Guid.NewGuid() + Path.GetExtension(profilePhoto.FileName); 
            var filePath = Path.Combine(uploads, fileName);  // Combines the directory path with file name and crates full file path

            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            } 

            using (var imageStream = profilePhoto.OpenReadStream())          // Opens a stream to read the uploaded image
            {
                using (var image = await Image.LoadAsync(imageStream))       // Loads the image from stream
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Stretch,
                        Size = new Size(200, 200)
                    }));

                    await image.SaveAsync(filePath);
                }
            }

            return await File.ReadAllBytesAsync(filePath); //return byte[]
        }
        private static bool IsImage(IFormFile profilePhoto)
        {
            return profilePhoto.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase);
        }
    }
}
