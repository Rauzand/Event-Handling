using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Event_Handling.Objects
{
    public class GreenCircle : BaseObject
    {
        //таймер круга
        public short time;

        //делегат окончания таймера
        public Action<GreenCircle>? OnTimeOver;

        public GreenCircle(float x, float y, float angle) : base(x, y, angle)
        {
            Random random = new();
            time = (short)random.Next(70, 160);
        }

        public override void Render(Graphics g)
        {
            g.FillEllipse(new SolidBrush(Color.ForestGreen), -13, -13, 26, 26);
            g.DrawString($"{time}", new Font("Verdana", 8), new SolidBrush(Color.Green), 10, 10);
        }

        public override GraphicsPath GetGraphicsPath()
        {
            var path = base.GetGraphicsPath();
            path.AddEllipse(-13, -13, 26, 26);
            return path;
        }

        //метод для вызова делегата
        public void TimeOver()
        {
            if (time < 0)
                OnTimeOver?.Invoke(this);
        }
    }
}
