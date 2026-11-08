using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Reflection;
using System.Windows.Forms;

namespace BombaOyunu
{
    public partial class Form1 : Form
    {
        private readonly Random random = new Random();
        private PictureBox[] qutular;
        private readonly bool[] acilan = new bool[6];
        private int bombaYeri = -1;
        private int xal;
        private bool oyunBitdi;
        private Image pisikSekli;
        private Image bombaSekli;
        private Stream sesFayli;
        private SoundPlayer partlayis;

        public Form1()
        {
            InitializeComponent();
            qutular = new[] { pictureBox1, pictureBox2, pictureBox3,
                pictureBox4, pictureBox5, pictureBox6 };

            pisikSekli = SekilYukle("cat.png");
            bombaSekli = SekilYukle("bomba.png");
            sesFayli = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("BombaOyunu.Assets.explosion.wav");
            partlayis = new SoundPlayer(sesFayli);
            partlayis.Load();

            for (int i = 0; i < qutular.Length; i++)
            {
                qutular[i].Tag = i;
                qutular[i].Click += Qutu_Click;
                qutular[i].Paint += Qutu_Paint;
            }

            YeniOyun();
        }

        private Image SekilYukle(string ad)
        {
            using (Stream fayl = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("BombaOyunu.Assets." + ad))
            using (Image sekil = Image.FromStream(fayl))
            {
                return new Bitmap(sekil);
            }
        }

        private void YeniOyun()
        {
            partlayis.Stop();
            // Yeni oyunda bomba əvvəlki qutuda qalmasın.
            if (bombaYeri == -1)
                bombaYeri = random.Next(6);
            else
            {
                int yeniYer = random.Next(5);
                if (yeniYer >= bombaYeri)
                    yeniYer++;
                bombaYeri = yeniYer;
            }

            xal = 0;
            oyunBitdi = false;
            lblXal.Text = "0";

            for (int i = 0; i < qutular.Length; i++)
            {
                acilan[i] = false;
                qutular[i].Image = null;
                qutular[i].Enabled = true;
                qutular[i].Invalidate();
            }
        }

        private void Qutu_Paint(object sender, PaintEventArgs e)
        {
            PictureBox qutu = (PictureBox)sender;
            if (qutu.Image != null)
                return;

            using (Font sual = new Font("Arial", 88, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                TextRenderer.DrawText(e.Graphics, "?", sual, qutu.ClientRectangle,
                    Color.Black, TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
        }

        private void Qutu_Click(object sender, EventArgs e)
        {
            PictureBox qutu = (PictureBox)sender;
            int yer = (int)qutu.Tag;
            if (oyunBitdi || acilan[yer])
                return;

            acilan[yer] = true;
            if (yer == bombaYeri)
            {
                qutu.Image = bombaSekli;
                OyunuBitir();
                qutu.Refresh();
                partlayis.Play();
                MessageBox.Show(this, "Oyun bitdi", "Bomba",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            qutu.Image = pisikSekli;
            xal++;
            lblXal.Text = xal.ToString();
            qutu.Refresh();
            lblXal.Refresh();

            if (xal == 5)
            {
                OyunuBitir();
                MessageBox.Show(this,
                    "Oyunu qazandınız, Yeni oyun üçün ok düyməsinə basın",
                    "Təbriklər", MessageBoxButtons.OK, MessageBoxIcon.Information);
                YeniOyun();
            }
        }

        private void OyunuBitir()
        {
            oyunBitdi = true;
            foreach (PictureBox qutu in qutular)
                qutu.Enabled = false;
        }

        private void btnYeniOyun_Click(object sender, EventArgs e)
        {
            YeniOyun();
        }

        private void btnCixis_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            partlayis.Stop();
            partlayis.Dispose();
            sesFayli.Dispose();
            foreach (PictureBox qutu in qutular)
                qutu.Image = null;
            pisikSekli.Dispose();
            bombaSekli.Dispose();
            base.OnFormClosed(e);
        }
    }
}
