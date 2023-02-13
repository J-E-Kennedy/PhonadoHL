using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phonado.Logging
{
    public class GrabLogDto
    {
        public string Hand { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Confidence { get; set; }
        public DateTime Time { get; set; }
        public PositionLogDto Position { get; set; }
    }
}