using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.UI;

namespace Photo.Controllers
{
    class CutManage : DrawManage
    {
        public double cutPointX { get; set; }   //裁剪区域起点的X值
        public double cutPointY { get; set; }   //Y值
        public double cutWidth { get; set; }    //裁剪宽度
        public double cutHeight { get; set; }   //高度
        public double editCanvas_Width { get; set; }    //画布宽度
        public double editCanvas_Height { get; set; }    //画布高度
        public Rect cutArea { get { return new Rect(cutPointX, cutPointY, cutWidth, cutHeight); } }
        public Rect LeftTopPoint { get { return new Rect(cutPointX - 8, cutPointY - 8, 16, 16); } }
        public Rect RightTopPoint { get { return new Rect(cutPointX + cutWidth - 8, cutPointY - 8, 16, 16); } }
        public Rect RightBottomPoint { get { return new Rect(cutPointX + cutWidth - 8, cutPointY + cutHeight - 8, 16, 16); } }

        public void Draw(CanvasDrawingSession graphics, float scale)
        {
            var stickness = 1;
            var radius = 8;
            CanvasStrokeStyle style = new CanvasStrokeStyle();
            style.DashCap = CanvasCapStyle.Square;
            style.DashStyle = CanvasDashStyle.Dash;
            style.StartCap = CanvasCapStyle.Round;
            style.EndCap = CanvasCapStyle.Round;

            //绘制阴影
            //graphics.FillRectangle(cutArea, Color.FromArgb(0, 0, 0, 0));
            graphics.FillRectangle(new Rect(0, 0, editCanvas_Width, cutPointY), Color.FromArgb(100, 0XFF, 0XFF, 0XFF));
            graphics.FillRectangle(new Rect(0, cutPointY + cutHeight, editCanvas_Width, editCanvas_Height - cutPointY - cutHeight), Color.FromArgb(100, 0XFF, 0XFF, 0XFF));
            graphics.FillRectangle(new Rect(0, cutPointY, cutPointX, cutHeight), Color.FromArgb(100, 0XFF, 0XFF, 0XFF));
            graphics.FillRectangle(new Rect(cutPointX + cutWidth, cutPointY, editCanvas_Width - cutPointX - cutWidth, cutHeight), Color.FromArgb(100, 0XFF, 0XFF, 0XFF));

            graphics.DrawRectangle(cutArea, Colors.White, stickness); //矩形

            if (cutWidth > 50 && cutHeight > 50)  //当满足条件时  绘制九宫格
            {
                graphics.DrawLine((float)cutPointX, (float)(cutPointY + (cutHeight / 3)), (float)(cutPointX + cutWidth), (float)(cutPointY + cutHeight / 3), Colors.White, 0.3f, style);
                graphics.DrawLine((float)cutPointX, (float)(cutPointY + (cutHeight * 2 / 3)), (float)(cutPointX + cutWidth), (float)(cutPointY + cutHeight * 2 / 3), Colors.White, 0.3f, style);
                graphics.DrawLine((float)(cutPointX + cutWidth / 3), (float)cutPointY, (float)(cutPointX + cutWidth / 3), (float)(cutPointY + cutHeight), Colors.White, 0.3f, style);
                graphics.DrawLine((float)(cutPointX + cutWidth * 2 / 3), (float)cutPointY, (float)(cutPointX + cutWidth * 2 / 3), (float)(cutPointY + cutHeight), Colors.White, 0.3f, style);
            }
            graphics.FillCircle((float)cutPointX, (float)cutPointY, radius, Colors.White);  //×
            graphics.DrawLine((float)cutPointX - 4, (float)cutPointY - 4, (float)cutPointX + 4, (float)cutPointY + 4, Colors.Black);
            graphics.DrawLine((float)cutPointX - 4, (float)cutPointY + 4, (float)cutPointX + 4, (float)cutPointY - 4, Colors.Black);
            graphics.FillCircle((float)(cutPointX + cutWidth), (float)cutPointY, radius, Colors.White); //√
            graphics.DrawLine((float)(cutPointX + cutWidth - 4), (float)(cutPointY - 1), (float)(cutPointX + cutWidth), (float)(cutPointY + 3), Colors.Black);
            graphics.DrawLine((float)(cutPointX + cutWidth), (float)(cutPointY + 3), (float)(cutPointX + cutWidth + 4), (float)(cutPointY - 4), Colors.Black);
            graphics.FillCircle((float)(cutPointX + cutWidth), (float)(cutPointY + cutHeight), radius, Colors.White); //缩放
            graphics.DrawLine((float)(cutPointX + cutWidth - 4), (float)(cutPointY + cutHeight - 4), (float)(cutPointX + cutWidth + 4), (float)(cutPointY + cutHeight + 4), Colors.Black);
            graphics.DrawLine((float)(cutPointX + cutWidth - 4), (float)(cutPointY + cutHeight - 4), (float)(cutPointX + cutWidth - 4), (float)(cutPointY + cutHeight), Colors.Black);
            graphics.DrawLine((float)(cutPointX + cutWidth - 4), (float)(cutPointY + cutHeight - 4), (float)(cutPointX + cutWidth), (float)(cutPointY + cutHeight - 4), Colors.Black);
            graphics.DrawLine((float)(cutPointX + cutWidth + 4), (float)(cutPointY + cutHeight + 4), (float)(cutPointX + cutWidth), (float)(cutPointY + cutHeight + 4), Colors.Black);
            graphics.DrawLine((float)(cutPointX + cutWidth + 4), (float)(cutPointY + cutHeight + 4), (float)(cutPointX + cutWidth + 4), (float)(cutPointY + cutHeight), Colors.Black);

        }
        public void Cut(CanvasDrawingSession graphics, float scale)
        {
            graphics.FillRectangle(new Rect(0, 0, editCanvas_Width, cutPointY), Colors.White);
            graphics.FillRectangle(new Rect(0, cutPointY + cutHeight, editCanvas_Width, editCanvas_Height - cutPointY - cutHeight), Colors.White);
            graphics.FillRectangle(new Rect(0, cutPointY, cutPointX, cutHeight), Colors.White);
            graphics.FillRectangle(new Rect(cutPointX + cutWidth, cutPointY, editCanvas_Width - cutPointX - cutWidth, cutHeight), Colors.White);
        }
    }
}
