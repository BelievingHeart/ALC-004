using System;
using System.Collections.Generic;
using System.Linq;
using HalconDotNet;

namespace ALC.Core.Camera
{
    public class ImageCollector
    {
        private Dictionary<string, HObject> _imageBuffer = new Dictionary<string, HObject>();

        public event Action<IList<HObject>> ImagesCollectionDone; 

        public ImageCollector(params HCamera[] cameras)
        {
            var numCameras = cameras.Length;
            
            foreach (var camera in cameras)
            {
                camera.SingleImageAcquired += image =>
                {
                    _imageBuffer[camera.Name] = image;
                    if (_imageBuffer.Count == numCameras)
                    {
                        OnImagesCollectionDone(_imageBuffer.Values.ToList());
                    }
                };
            }
            
        }


        protected virtual void OnImagesCollectionDone(IList<HObject> images)
        {
            ImagesCollectionDone?.Invoke(images);
            ClearImageBuffer();
        }
        
        public void ClearImageBuffer()
        {
            _imageBuffer.Clear();
        }
    }
}