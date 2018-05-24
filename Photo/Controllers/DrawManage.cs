using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photo.Controllers
{
    interface DrawManage
    {
        void Draw(CanvasDrawingSession graphics, float scale);
    }
}
