using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Raf.Twain;
using Saraff.Twain;

namespace Test1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var tw = new TwainHelper();
            var sources = tw.GetSources();
            listBoxScanners.DataSource = sources;
            listBoxScanners.DisplayMember = "Name";
            listBoxScanners.ValueMember = "Id";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBoxScanners.SelectedIndex < 0)
            {
                return;
            }

            var tw = new TwainHelper();
            var list = new List<CapEnum>
            {
                new CapEnum()
                {
                    //Captura a dos caras
                    Cap = TwCap.DuplexEnabled,
                    Current = new CapValue() { Name = TwCap.DuplexEnabled.ToString(), RawValue = true }
                },
                new CapEnum()
                {
                    //Captura color (RGB), escala de grises (Gray), negro (BW)
                    Cap = TwCap.IPixelType,
                    Current = new CapValue() { Name = TwCap.IPixelType.ToString(), RawValue = TwPixelType.BW }
                },
                new CapEnum()
                {
                    //Compresion de la imagen para (RGB, Gray, BW) TwCompression.None,  (RGB y Gray) TwCompression.Jpeg, (BW) TwCompression.Group4
                    Cap = TwCap.ICompression,
                    Current = new CapValue() {Name = TwCap.ICompression.ToString(), RawValue = TwCompression.Group4 }
                },
                //new CapEnum()
                //{
                //    //Union de imagenes amverso y reverso Menor calidad(40,50,80,90,100)Mayor calidad
                //    Cap = TwCap.JpegQuality,
                //    Current = new CapValue() {Name = TwCap.JpegQuality.ToString(), RawValue =  80}
                //},
                new CapEnum()
                {
                    Cap = TwCap.XResolution,
                    Current = new CapValue {Name = TwCap.XResolution.ToString(), RawValue = 200f }
                },
                new CapEnum()
                {
                    Cap = TwCap.YResolution,
                    Current = new CapValue() {Name = TwCap.YResolution.ToString(), RawValue = 200f }
                },
                new CapEnum()
                {
                    //Remover paginas en blanco, no quita imagenes en blanco si se captura uniendo imagenes(Merge), a menos que ambas caras sean en blanco
                    Cap = (TwCap)0x809b,
                    //0 = based on size, 1 = deshabilitado, 2 = based on content
                    Current = new CapValue() { Name = 0x809b.ToString(), RawValue = 2 }
                },
                new CapEnum()
                {
                    //0x80c4 = porcentaje 0-100 para documento en blanco basado en contenido
                    //0x809e = porcentaje 1024-1024000 para documento en blanco basado en contenido
                    Cap = (TwCap)0x80c4,
                    Current = new CapValue() { Name = 0x80c4.ToString(), RawValue = 15 }
                },
                //Otros escaners (HP)
                //new CapEnum()
                //{
                //    Cap = TwCap.AutoDiscardBlankPages,
                //    Current = new CapValue() {Name = TwCap.AutoDiscardBlankPages.ToString(), RawValue = TwBP.Auto}
                //},
                new CapEnum()
                {
                    //Union de imagenes amverso y reverso
                    Cap = TwCap.ImageMerge,
                    Current = new CapValue() {Name = TwCap.ImageMerge.ToString(), RawValue = TwIM.FrontOnTop }
                },
            };
            
            var images = new List<byte[]>();
            try
            {
                images = tw.GetImagesFromScanner((Source) listBoxScanners.SelectedItem, list);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.Contains("not supported")
                    ? $"No se soporta la capacidad configurada, {ex.Message}"
                    : $"No se ha podido utilizar el escaner, verifique que este encendido y se encuentre conectado, {ex.Message}");
            }


            for (var i = 0; i < images.Count; i++)
            {
                var image = images[i];
                File.WriteAllBytes($"{DateTime.Now.ToString("yyyy-MMMM-dd")}-imagen-{i}.jpg", image);
            }
        }
    }
}
