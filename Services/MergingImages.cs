using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;


namespace SuccBot.Services
{
    public static class MergingImages
    {
        public static IImageProcessingContext<TPixel> ApplyScalingWaterMark<TPixel>(
            this IImageProcessingContext<TPixel> processingContext,
            Stream avatarStream, int positionX = 50, int positionY = 50)
            where TPixel : struct, IPixel<TPixel>
        {
            return processingContext.Apply(
                image =>
                {
                    using (var logo = Image.Load<TPixel>(avatarStream))
                    {
                        bool worh = image.Width > image.Height;
                        double coff = worh
                                    ? image.Width * 0.2 / Math.Min(logo.Width, logo.Height)
                                    : image.Height * 0.2 / Math.Max(logo.Width, logo.Height);

                        var size = new Size((int)(logo.Width * coff), (int)(logo.Height * coff));
                        var location = new Point(image.Width - size.Width - positionX, image.Height - size.Height - positionY);

                        // Resize the logo
                        logo.Mutate(i => i.Resize(size));

                        image.Mutate(i => i.DrawImage(logo, PixelBlenderMode.Normal, 1, location));
                    }
                });
        }
    }
}