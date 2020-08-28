using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;
using OpenCvSharp;
using System.Drawing;
using OpenCvSharp.Extensions;
using OpenCvSharp.Text;

namespace xd2
{
    class TextExtractor
    {
        TesseractEngine ocrEngine;

        public TextExtractor(Mat input, out string textResult)
        {
            ocrEngine = new TesseractEngine(@"C:\Program Files (x86)\Tesseract-OCR", "eng", EngineMode.TesseractAndCube);
            textResult = Convert2Text(input);
        }

        public TextExtractor(Mat input, out string textResult, string language)
        {
            ocrEngine = new TesseractEngine(@"C:\Program Files (x86)\Tesseract-OCR", language, EngineMode.Default);
            if (language == "eng") ocrEngine.SetVariable("tessedit_char_whitelist", "1234567890X");
            textResult = Convert2Text(input);
        }

        public TextExtractor(Mat input, out string textResult, string enginePath, string language)
        {
            ocrEngine = new TesseractEngine(enginePath, language, EngineMode.Default);
            if (language == "eng") ocrEngine.SetVariable("tessedit_char_whitelist", "1234567890X");
            textResult = Convert2Text(input);
        }

        public string Convert2Text(Mat input) => ocrEngine.Process(input.ToBitmap()).GetText();
    }
}
