using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Phonado.Core
{
    public static class ResourceLoader
    {
        /// <summary>
        /// Initial collection of images that is created with Resources
        /// </summary>
        private static List<Sprite> images = new List<Sprite>();
        /// <summary>
        /// Collection that is used to guarantee that images that get used wont get used again
        /// </summary>
        private static HashSet<Sprite> usedImages = new HashSet<Sprite>();
        /// <summary>
        /// Property to check amount of images that is found in list
        /// </summary>
        public static int ImageCount
        {
            get
            {
                if (images.Count == 0)
                {
                    images = GetImages();
                }
                return images.Count;
            }
        }
        
        /// <summary>
        /// Using resources to search through a folder location and return a collection of all sprites
        /// </summary>
        /// <returns>List of sprites found</returns>
        public static List<Sprite> GetImages()
        {
            return Resources.LoadAll<Sprite>("Pictures").ToList();
        }

        /// <summary>
        /// Selecting a random image from the collection, and checking if it has been used before using the Hashset Add to confirm if the image can be used
        /// </summary>
        /// <returns>An image that hasn't been used before</returns>
        public static Sprite SelectNextImage()
        {
            //Populating images here, to confirm that images are all properly populated into Unity's resource folder before grabbing them, also making sure they wont be populated again
            if (images.Count <= 0)
            {
                images = GetImages();
            }
            
            int selectedImageIndex = Random.Range(0, images.Count);
            Sprite selectedImage = images[selectedImageIndex];
            
            //Checking if all images have been used from original collection, and clearing the hashset to allow a new set of the images to be used
            if (usedImages.Count >= images.Count)
            {
                usedImages.Clear();
            }

            //Using hashset add to confirm that an image has not been used yet, if it has, image is reselected until an unused image is found
            while (!usedImages.Add(selectedImage))
            {
                //if add function ends up being false, image already exists, so reselect a new image
                selectedImageIndex = Random.Range(0, images.Count);
                selectedImage = images[selectedImageIndex];
            }

            return selectedImage;
        }
    }
}