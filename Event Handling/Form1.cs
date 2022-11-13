using Event_Handling.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Event_Handling
{
    public partial class Form1 : Form
    {
        public List<BaseObject> objects = new();
        public Player player;
        public Marker? marker;
        public ushort score = 0;

        public Form1()
        {
            InitializeComponent();

            Random random = new();
            player = new Player(pbMain.Width / 2, pbMain.Height / 2, 0);

            player.OnOverlap += (p, obj) =>
            {
                txtLog.Text = $"[{DateTime.Now:HH:mm:ss:ff}] Игрок пересекся с {obj}\n" + txtLog.Text;
            };

            player.OnMarkerOverlap += (m) =>
            {
                objects.Remove(m);
                marker = null;
            };

            //при пересечении с кругом обновление координат
            player.OnGreenCircleOverlap += (circle) =>
            {
                circle.X = random.Next(13, 607);
                circle.Y = random.Next(13, 407);
                score++;
                labelScore.Text = $"Счёт: {score}";
            };

            objects.Add(player);
            objects.Add(new GreenCircle(random.Next(13, 607), random.Next(13, 407), 0));
            objects.Add(new GreenCircle(random.Next(13, 607), random.Next(13, 407), 0));

            //поиск кругов в списке объектов
            foreach (var obj in objects.ToList())
            {
                if (obj is GreenCircle circle)
                {
                    //при истечении таймера обновыление координат и таймера
                    circle.OnTimeOver += (c) =>
                    {
                        Random random = new();
                        c.time = (short)random.Next(70, 160);
                        c.X = random.Next(13, 607);
                        c.Y = random.Next(13, 407);
                    };
                }
            }
        }

        private void PbMain_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);

            UpdatePlayer();

            foreach (var obj in objects.ToList())
            {
                if (obj != player && player.Overlaps(obj, g))
                {
                    player.Overlap(obj);
                    obj.Overlap(player);
                }

                //изменение счётчика таймера и проверка на истечение таймера
                if (obj is GreenCircle circle)
                {
                    circle.time--;
                    circle.TimeOver();
                }
            }

            foreach (var obj in objects)
            {
                g.Transform = obj.GetTransform();
                obj.Render(g);
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            pbMain.Invalidate();
        }

        private void UpdatePlayer()
        {
            if (marker != null)
            {
                float dx = marker.X - player.X;
                float dy = marker.Y - player.Y;
                float length = MathF.Sqrt(dx * dx + dy * dy);
                dx /= length;
                dy /= length;

                player.vX += dx * 0.5f;
                player.vY += dy * 0.5f;

                player.Angle = 90 - MathF.Atan2(player.vX, player.vY) * 180 / MathF.PI;
            }

            player.vX += -player.vX * 0.1f;
            player.vY += -player.vY * 0.1f;

            player.X += player.vX;
            player.Y += player.vY;
        }

        private void PbMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (marker == null)
            {
                marker = new Marker(0, 0, 0);
                objects.Add(marker);
            }

            marker.X = e.X;
            marker.Y = e.Y;
        }
    }
}
