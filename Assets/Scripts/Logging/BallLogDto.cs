using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phonado.Logging
{
    public class BallLogDto
    {
        public float XLaunch { get; set; }
        public float YLaunch { get; set; }
        public float ZLaunch { get; set; }
        public float XLand { get; set; }
        public float YLand { get; set; }
        public float ZLand { get; set; }
        public DateTime LaunchTime { get; set; }
        public DateTime LandTime { get; set; }
    }
}

