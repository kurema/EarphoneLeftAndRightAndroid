using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EarphoneLeftAndRight.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefinitionGraphicalPage : ContentPage
    {
        public DefinitionGraphicalPage()
        {
            InitializeComponent();
        }

        private void Grid_SizeChanged(object sender, EventArgs e)
        {
            if (sender is not Grid g) return;
            if (g.Width == -1 || g.Height == -1) return;

            {
                var m1 = labelLeft.Measure(double.PositiveInfinity, double.PositiveInfinity);
                var m2 = labelRight.Measure(double.PositiveInfinity, double.PositiveInfinity);
                var hr = g.Height * (g.RowDefinitions[0].Height.Value / (g.RowDefinitions[0].Height.Value + g.RowDefinitions[1].Height.Value));//Asume all RowDefinitions uses *.
                var scale = Math.Min(Math.Min(g.Width / m1.Request.Width, g.Width / m2.Request.Width) / 2, Math.Min(hr / m1.Request.Height, hr / m2.Request.Height));
                if ((scale < 0.95 || scale > 1.05) && scale > 0)
                {
                    labelLeft.FontSize = labelRight.FontSize *= scale;
                }
            }

            {
                var m1 = labelLeftArrow.Measure(double.PositiveInfinity, double.PositiveInfinity);
                var m2 = labelRightArrow.Measure(double.PositiveInfinity, double.PositiveInfinity);
                var hr = g.Height * (g.RowDefinitions[1].Height.Value / (g.RowDefinitions[0].Height.Value + g.RowDefinitions[1].Height.Value));
                var scale = Math.Min(Math.Min(g.Width / m1.Request.Width, g.Width / m2.Request.Width) / 2, Math.Min(hr / m1.Request.Height, hr / m2.Request.Height));
                if ((scale < 0.95 || scale > 1.05) && scale > 0)
                {
                    labelLeftArrow.FontSize = labelRightArrow.FontSize *= scale;
                }

                {
                    //labelLeftArrow.TranslateTo(-g.Width / 2, 0, 1000);
                    //labelRightArrow.TranslateTo(g.Width / 2, 0, 1000);

                    static void animateFunc(double v, Label label, double scale,double gridWidth)
                    {
                        var w = label.Width;
                        var h = label.Height;
                        var shiftX = v * (gridWidth / 2 + w) + (v < 0 ? 1 : -1) * w;
                        label.TranslationX = shiftX;
                        //label.Clip = v < 0 ?
                        //    new Xamarin.Forms.Shapes.RectangleGeometry(new Rect(0, 0, w - Math.Max(0, shiftX + gridWidth/2), h)) :
                        //    new Xamarin.Forms.Shapes.RectangleGeometry(new Rect(0, -Math.Min(0, shiftX - gridWidth / 2), w, h));
                        //label.Clip = new Xamarin.Forms.Shapes.RectangleGeometry(new Rect(0, 0, w, h));
                    }

                    new Animation
                    {
                        {0 ,1,new Animation(v=>{
                            animateFunc(-v,labelLeftArrow,scale,g.Width);
                            animateFunc(v,labelRightArrow,scale,g.Width);
                        })
                        }
                    }.Commit(this, "ArrowAnimation", 16, 1200, null, (v, c) => { }, () => true);
                }

            }
        }

        private void TapGestureRecognizer_Tapped_Left(object sender, EventArgs e)
        {
            var tts = DependencyService.Get<Dependency.ITextToSpeech>();
            tts.Clear();
            tts.SpeakLeftAsync();
        }

        private void TapGestureRecognizer_Tapped_Right(object sender, EventArgs e)
        {
            var tts = DependencyService.Get<Dependency.ITextToSpeech>();
            tts.Clear();
            tts.SpeakRightAsync();
        }
    }
}