using System;
using System.Collections.Generic;
using System.Drawing;
using Tesseract;

namespace HadesAIOCommon.OCR
{
    public class HadesOCR
    {
        private const string TESSDATA = "tessdata";
        private const string LANGUAGE = "eng";
        private const EngineMode engineMode = EngineMode.Default;
        private const PageIteratorLevel pageIteratorLevel = PageIteratorLevel.Word;


        public static Point? FindTextLocation(Bitmap bitmap, string keyword, out string returnText)
        {
            var words = GetBoudingWords(bitmap, out returnText);
            if (returnText.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var word in words)
                {
                    if (keyword.StartsWith(word.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        return new Point(word.Rect.X1, word.Rect.X2);
                    }
                }
            }
            return null;
        }


        public static List<OCRWord> GetBoudingWords(Bitmap bitmap, out string pageText)
        {
            List<OCRWord> results = new();
            using (var engine = new TesseractEngine(TESSDATA, LANGUAGE, engineMode))
            {
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
                        results.Add(new OCRWord(curText, rect));
                    }
                } while (iter.Next(pageIteratorLevel));
            }

            return results;
        }

        public static List<OCRWord> GetBoudingWords(string imgPath, out string pageText)
        {
            List<OCRWord> results = new();
            using (var engine = new TesseractEngine(TESSDATA, LANGUAGE, engineMode))
            {
                using var img = Pix.LoadFromFile(imgPath);
                using var page = engine.Process(img);
                pageText = page.GetText();
                var iter = page.GetIterator();
                iter.Begin();
                do
                {
                    if (iter.TryGetBoundingBox(pageIteratorLevel, out var rect))
                    {
                        var curText = iter.GetText(pageIteratorLevel);
                        results.Add(new OCRWord(curText, rect));
                    }
                } while (iter.Next(pageIteratorLevel));
            }

            return results;
        }



    }



    public class OCRWord
    {
        public OCRWord(string text, Rect rect)
        {
            Text = text;
            Rect = rect;
        }

        public string Text { get; set; }
        public Rect Rect { get; set; }
    }

}
