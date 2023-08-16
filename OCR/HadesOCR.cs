using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tesseract;

namespace HadesAIOCommon.OCR
{
    public class HadesOCR
    {
        private const string TESSDATA = "tessdata\\fast";
        private const string LANGUAGE = "eng";
        private const EngineMode engineMode = EngineMode.Default;
        private const PageIteratorLevel pageIteratorLevel = PageIteratorLevel.Word;

        private const int MAX_CONCURRENT = 128;
        private static readonly Random rand = new();
        private static readonly List<object> mutexs = Enumerable.Repeat(new object(), MAX_CONCURRENT).ToList();

        public static Point? FindTextLocation(Bitmap bitmap, string keyword, out string returnText)
        {
            var words = GetBoudingWords(bitmap, out returnText);
            string[] keywords = keyword.Split(' ').Select(x => x.ToLower()).ToArray();
            if (returnText.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var word in words)
                {
                    if (string.IsNullOrEmpty(word.Text.Trim()))
                    {
                        continue;
                    }
                    if (word.Text.ToLower().Contains(keywords[0]))
                    {
                        return new Point(word.Rect.X1, word.Rect.Y1);
                    }
                }
            }
            return null;
        }
        public static Point? FindTextLocation(string imgPath, string keyword, out string returnText)
        {
            var words = GetBoudingWords(imgPath, out returnText);
            string[] keywords = keyword.Split(' ').Select(x => x.ToLower()).ToArray();
            if (returnText.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var word in words)
                {
                    if (string.IsNullOrEmpty(word.Text.Trim()))
                    {
                        continue;
                    }
                    if (word.Text.Contains(keywords[0], StringComparison.OrdinalIgnoreCase))
                    {
                        return new Point(word.Rect.X1, word.Rect.Y1);
                    }
                }
            }
            return null;
        }

        public static List<BoudingWord> GetBoudingWords(Bitmap bitmap, out string pageText)
        {
            List<BoudingWord> results = new();
            
            //var locker = mutexs[rand.Next(mutexs.Count)];
            //lock (locker)
            //{
            //}

            BitmapToGrayScale(bitmap);
            using var engine = new TesseractEngine(TESSDATA, LANGUAGE, engineMode);
            using var img = PixConverter.ToPix(bitmap);
            using var page = engine.Process(img);
            pageText = page.GetText();
            var iter = page.GetIterator();
            iter.Begin();
            do
            {
                if (iter.TryGetBoundingBox(pageIteratorLevel, out var rect))
                {
                    var curText = iter.GetText(pageIteratorLevel);
                    results.Add(new BoudingWord(curText, rect));
                }
            } while (iter.Next(pageIteratorLevel));

            return results;
        }
        public static List<BoudingWord> GetBoudingWords(string imgPath, out string pageText)
        {
            List<BoudingWord> results = new();

            //var locker = mutexs[rand.Next(mutexs.Count)];
            //lock (locker)
            //{ 
            //}

            using var bitmap = new Bitmap(imgPath);
            BitmapToGrayScale(bitmap);
            using var engine = new TesseractEngine(TESSDATA, LANGUAGE, engineMode);
            using var img = PixConverter.ToPix(bitmap);
            using var page = engine.Process(img);
            pageText = page.GetText();
            var iter = page.GetIterator();
            iter.Begin();
            do
            {
                if (iter.TryGetBoundingBox(pageIteratorLevel, out var rect))
                {
                    var curText = iter.GetText(pageIteratorLevel);
                    results.Add(new BoudingWord(curText, rect));
                }
            } while (iter.Next(pageIteratorLevel));

            return results;
        }

        public static void BitmapToGrayScale(Bitmap Bmp)
        {
            int rgb;
            Color c;
            for (int y = 0; y < Bmp.Height; y++)
            {
                for (int x = 0; x < Bmp.Width; x++)
                {
                    c = Bmp.GetPixel(x, y);
                    rgb = (int)Math.Round(.299 * c.R + .587 * c.G + .114 * c.B);
                    Bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            }
        }

    }



    public class BoudingWord
    {
        public BoudingWord(string text, Rect rect)
        {
            Text = text;
            Rect = rect;
        }

        public string Text { get; set; }
        public Rect Rect { get; set; }
    }

}
