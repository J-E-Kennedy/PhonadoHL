using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phonado.Logging
{
    public class SessionPhotoLogDto
    {
        public string PhotoName { get; set; }
        public int TimesGrabbed { get; set; }
        public float SecondsGrabbed { get; set; }
    }
}