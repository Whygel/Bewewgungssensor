using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bewewgungssensor
{
    public  class Obstackle
    {
        public Border Border;
        public Obstackle( Border iBorder)
        {
            
                if (iBorder != null)
            {
                Border = iBorder;
                BorderX = iBorder.X;
                BorderY = iBorder.Y;
            }
        }
        public double BorderX;
        public double BorderY;
    }
}
