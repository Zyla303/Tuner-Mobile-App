using SQLiteDB.Resources.Model;
using System.Text.RegularExpressions;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Project1.Models;
using System.Collections.Generic;
using Java.IO;
using static Android.Provider.ContactsContract.CommonDataKinds;
using static System.Net.Mime.MediaTypeNames;
using Java.Lang;
using System;
using Math = System.Math;
using System.Text;
using static Android.Icu.Text.IDNA;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.CommunityToolkit.UI.Views;
using Image = Xamarin.Forms.Image;
using Android.Media;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;
using System.IO;
using Android.Graphics;
using System.Reflection;
using Stream = System.IO.Stream;
using static Android.Provider.MediaStore;
using System.Net.Http;
using System.Web;
using System.Net;

namespace Project1.Views
{
    public partial class RecordPage : ContentPage
    {

        List<NoteToLines> noteDetails;

        SKPaintSurfaceEventArgs args = null;
        public int blocker = 0;

        SKPaint fiveLine = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Black,
        };

        SKPaint pauseLine = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White.WithAlpha(0x00),
            StrokeWidth = 100,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true
        };

        SKPaint NoteSharp = new SKPaint
        {
            Color = SKColors.Black,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true,
            TextSize = 50
        };

        SKPaint FullHalfNote = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true
        };

        SKPaint OtherNotes = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 20,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true
        };

        SKPaint NoteTail = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Black,
            StrokeWidth = 10
        };

        SKPaint NoteLine = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Black
        };

        public int noteId = 0;
        public string noteLen = "";
        public int notePosition = 0;
        public string noteName = "";
        SoundProps noteDetail;

        public int maxStepCounter = 0;
        public int stepCounter = 0;

        private Process.Listen listen = null;
        private string note = "";

        public RecordPage()
        {
            InitializeComponent();
            listen = new Process.Listen();
            listen.OnFrequency += Listen_OnFrequency;
            canvas.PaintSurface += canvasView_PaintSurface;
            listen.Start();

        }

        private void Listen_OnFrequency(object sender, double frequency)
        {

            SoundProps noteInfo = new SoundProps();

            Device.BeginInvokeOnMainThread(() => {

                noteDetail = noteInfo.getNote(frequency);
                if (noteDetail != null)
                {
                    NoteRecorder.Text = string.Format("{0}", TLLL(noteDetail.Note));
                    note = NoteRecorder.Text;
                }
                
            });
        }

        public static string TLLL(string str)
        {
            return Regex.Replace(str,
            @"[0-9]+", match => ((char)(match.Value[0] - '0' + '₀')).ToString());
        }

        public static string ReverseTLLL(string str)
        {
            string split1 = str.Substring(0, str.Length - 1);
            string split2 = str.Substring(str.Length - 1, 1);
            string numb = Regex.Replace(split2,
            ".", match => ((char)(match.Value[0] - '₀' + '0')).ToString());

            return string.Concat(split1, numb);
        }
        async void SaveNote(System.Object sender, System.EventArgs e)
        {
            if(note != "")
            {
                var noteToSave = ReverseTLLL(note);
                noteDetails = (List<NoteToLines>)await Navigation.ShowPopupAsync(new SaveNotePopUp(noteToSave));

                canvas.InvalidateSurface();
            }
            else
            {
                NoteRecorder.Text = string.Format("Play");
            }
        }

        public void ClearCanva(System.Object sender, System.EventArgs e)
        {
            //czyszczenie canvy

        }

        private void canvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (blocker > 0)
            {
                blocker = 0;
            }
            else if(noteDetails != null)
            {
                blocker++;

                SKSurface surface = e.Surface;
                surface = e.Surface;
                SKCanvas canvas = surface.Canvas;

                if (NoteRecorder.Text == string.Format("Play"))
                {
                    canvas.Clear(SKColors.White);
                }

                if (noteDetails != null)
                {


                    float start = 50;
                    var noteHeigthOnLine = 300;
                    var noteWidthOnLine = 175;
                    var noteStep = 25;
                    var noteSlide = 75;
                    var maxStep = 12;

                    float width = e.Info.Width;
                    float height = e.Info.Height;

                    //klucz basowy
                    var emojiChar = StringUtilities.GetUnicodeCharacterCode("𝄢", SKTextEncoding.Utf32);
                    var fontManager = SKFontManager.Default;
                    var emojiTypeface = fontManager.MatchCharacter(emojiChar);
                    var BassSharp = new SKPaint { 
                        Typeface = emojiTypeface,
                        TextSize = 130,
                        IsAntialias = true
                    };

                    if ((noteHeigthOnLine * maxStepCounter) + 250 + noteHeigthOnLine > height)
                    {
                        widthOfCanva.BindingContext = (noteHeigthOnLine * maxStepCounter) + 250 + noteHeigthOnLine;

                    }

                    if (noteDetails != null)
                    {
                        noteId = noteDetails[0].Id;
                        noteLen = noteDetails[0].Len;
                        notePosition = noteDetails[0].Position;
                        noteName = noteDetails[0].Note;
                    }

                    if (notePosition == 1)
                    {
                        stepCounter++;
                    }

                    if (stepCounter == maxStep)
                    {
                        maxStepCounter++;

                        stepCounter = 0;
                    }

                    //Ilosc 5ciolini
                    for (int j = 0; j <= maxStepCounter; j++)
                    {
                       
                        for (float i = 1; i < 6; i++)
                        {
                            
                            var pause = start * i;
                            canvas.DrawLine(50, pause + (noteHeigthOnLine * maxStepCounter), width - 50, pause + (noteHeigthOnLine * maxStepCounter), fiveLine);

                            //Bass Clef
                            SKPoint basspoint = new SKPoint(50, 220+(noteHeigthOnLine * maxStepCounter));
                            canvas.DrawText("𝄢", basspoint, BassSharp);

                        }
                    }

                    var DistFromBaseNote = noteId - 29;

                    var nonSharpCounter = 0;
                    var allNoteLoop = 0;

                    for (int stepOnNote = 0; stepOnNote <= DistFromBaseNote; stepOnNote++)
                    {
                        if (stepOnNote % 12 == 0 && stepOnNote != 0) allNoteLoop++;
                        if (stepOnNote % 13 == 0 && stepOnNote != 0) nonSharpCounter++;

                        var stepOnNoteTemp = stepOnNote - (12 * allNoteLoop);

                        if ((stepOnNoteTemp % 8 == 0) && (stepOnNoteTemp != 0)) nonSharpCounter++;
                    }

                    DistFromBaseNote = (int)Math.Ceiling((double)(DistFromBaseNote + nonSharpCounter) / 2);

                    if (noteName.Substring(1, 1) == "#")
                    {
                        canvas.DrawText("#", noteWidthOnLine + (noteSlide * stepCounter) - 40, noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter)
                            - (noteStep * DistFromBaseNote) + 15, NoteSharp);
                    }


                    //rysowanie nuty
                    if (noteLen == "Semibreve") //cala nuta
                    {
                        canvas.DrawCircle(noteWidthOnLine + (noteSlide * stepCounter), noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - 
                            (noteStep * DistFromBaseNote), 15, FullHalfNote);
                    }
                    else if (noteLen == "Minim") //pol nuta
                    {
                        canvas.DrawCircle(noteWidthOnLine + (noteSlide * stepCounter), 
                            noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote), 
                            15, 
                            FullHalfNote);
                        //rysowanie ogonka w gore
                        if (noteId <= 39)
                        {
                            canvas.DrawLine(noteWidthOnLine + (noteSlide * stepCounter) + 15,
                                noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote),
                                noteWidthOnLine + (noteSlide * stepCounter) + 15,
                                noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote) - 100,
                                NoteTail);

                        }//rysowanie ogonka w dol
                        else if (noteId > 39)
                        {
                            canvas.DrawLine(noteWidthOnLine + (noteSlide * stepCounter) + 15, 
                                noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote), 
                                noteWidthOnLine + (noteSlide * stepCounter) + 15, 
                                noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote) + 100, 
                                NoteTail);
                        }
                    }
                    else if (noteLen == "Crotchet") //cwierc nuta
                    {
                        canvas.DrawCircle(noteWidthOnLine + (noteSlide * stepCounter), noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote), 10, OtherNotes);
                        //rysowanie ogonka w gore
                        if (noteId <= 39)
                        {
                            canvas.DrawLine(noteWidthOnLine + (noteSlide * stepCounter) + 15, noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote), noteWidthOnLine + (noteHeigthOnLine * maxStepCounter) + (noteSlide * stepCounter) + 15, noteHeigthOnLine - (noteStep * DistFromBaseNote) - 100, NoteTail);

                        }//rysowanie ogonka w dol
                        else if (noteId > 39)
                        {
                            canvas.DrawLine(noteWidthOnLine + (noteSlide * stepCounter) + 15, noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote), noteWidthOnLine + (noteSlide * stepCounter) + 15, noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote) + 100, NoteTail);
                        }
                    }
                    else if (noteLen == "Quaver") //ósemka
                    {
                        canvas.DrawCircle(noteWidthOnLine + (noteSlide * stepCounter), noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote), 10, OtherNotes);
                        if (noteId <= 39)
                        {
                            canvas.DrawLine(noteWidthOnLine + (noteSlide * stepCounter) + 15, noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote), noteWidthOnLine + (noteHeigthOnLine * maxStepCounter) + (noteSlide * stepCounter) + 15, noteHeigthOnLine - (noteStep * DistFromBaseNote) - 100, NoteTail);

                        }//rysowanie ogonka w dol
                        else if (noteId > 39)
                        {
                            canvas.DrawLine(noteWidthOnLine + (noteSlide * stepCounter) + 15, noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote), noteWidthOnLine + (noteSlide * stepCounter) + 15, noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote) + 100, NoteTail);
                        }
                    }
                    else if (noteLen == "Semiquaver") //szesnastka
                    {
                        canvas.DrawCircle(noteWidthOnLine + (noteSlide * stepCounter), noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote), 10, OtherNotes);
                        if (noteId <= 39)
                        {
                            canvas.DrawLine(noteWidthOnLine + (noteSlide * stepCounter) + 15, noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote), noteWidthOnLine + (noteHeigthOnLine * maxStepCounter) + (noteSlide * stepCounter) + 15, noteHeigthOnLine - (noteStep * DistFromBaseNote) - 100, NoteTail);

                        }//rysowanie ogonka w dol
                        else if (noteId > 39)
                        {
                            canvas.DrawLine(noteWidthOnLine + (noteSlide * stepCounter) + 15, noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote), noteWidthOnLine + (noteSlide * stepCounter) + 15, noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote) + 100, NoteTail);
                        }
                    }

                    //dopisywanie lini jak brakuje 
                    if (300 + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote) >= 300 + (noteHeigthOnLine * maxStepCounter))
                    {
                        for (int i = 0; i >= DistFromBaseNote; i = i - 2)
                        {
                                canvas.DrawLine(noteWidthOnLine + (noteSlide * stepCounter) - 32, 
                                    noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * i), 
                                    noteWidthOnLine + (noteSlide * stepCounter) + 32, 
                                    noteHeigthOnLine + (noteHeigthOnLine * maxStepCounter) - (noteStep * i), NoteLine);
                        }
                    }
                    else if(350 + (noteHeigthOnLine * maxStepCounter) - (noteStep * DistFromBaseNote) <= 50 + (noteHeigthOnLine * maxStepCounter))
                    {
                        maxStepCounter++;
                        for (int i = 2; i <= DistFromBaseNote; i = i + 2)
                        {

                                canvas.DrawLine(noteWidthOnLine + (noteSlide * stepCounter) - 32,
                                //y0
                                (noteHeigthOnLine * maxStepCounter) - (noteStep * i),

                                noteWidthOnLine + (noteSlide * stepCounter) + 32,
                                //y1
                                (noteHeigthOnLine * maxStepCounter) - (noteStep * i),
                                NoteLine);
                                
                        }
                        maxStepCounter--;
                    }
                }

            }

        }

    }
}

