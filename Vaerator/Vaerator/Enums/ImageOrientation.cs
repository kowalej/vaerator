using System;
using System.Collections.Generic;
using System.Text;

namespace Vaerator.Enums
{
    /// <summary>
    /// Specifies where the image will occur relative to the text on a
    /// </summary>
    public enum ImageOrientation
    {
        /// <summary>
        /// The image to left
        /// </summary>
        ImageToLeft = 0,
        /// <summary>
        /// The image on top
        /// </summary>
        ImageOnTop = 1,
        /// <summary>
        /// The image to right
        /// </summary>
        ImageToRight = 2,
        /// <summary>
        /// The image on bottom
        /// </summary>
        ImageOnBottom = 3,
        /// <summary>
        /// The image and text are centered with the image to the left of text
        /// </summary>
        ImageCenterToLeft = 4,
        /// <summary>
        /// The image and text are centered with the image to the right of text
        /// </summary>
        ImageCenterToRight = 5
    }
}
