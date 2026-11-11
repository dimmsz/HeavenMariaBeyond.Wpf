using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HeavenMariaBeyond.Domain.Entities.Sub
{
    public class GirlPath
    {
        public string GirlRootDirPath { get; set; } = string.Empty;
        public string GirlProfileDirPathWithDate { get; set; } = string.Empty;
        public string GirlProfileImageDirPathWithDate { get; set; } = string.Empty;
    }
}
