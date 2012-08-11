using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sematena.AudioVideoLib
{
    class LibVlcHelpers
    {
        public static bool RetOk(int libVlcReturnValue)
        {
            return (libVlcReturnValue == 0);
        }
    }
}
