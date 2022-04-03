using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLContex = Geology.OpenGL.OpenGL;
using GLWin32 = Geology.OpenGL.Win32;
namespace Geology
{
    public class FontGeology
    {
        public enum TypeFont { Horizontal = 0, Vertical }
        TypeFont typeFont;

        int fontBaseList;
        int hdc;
        protected int oglcontext = 0;
        IntPtr myfont;
        string fontName;
        int fontSize;
        string templateString = "0.00000e+00";
        int captionAxisWidth;
        int widthTmplStr;
        int heightTmplStr;
        public TypeFont GetTypeFont { get { return typeFont; } }
        public int GetWidthTemplStr { get { return widthTmplStr; } }
        public int GetWidthCaption { get { return captionAxisWidth; } set { captionAxisWidth = value; } }
        public string GetTemplStr { get { return templateString; } }
        public int GetHeightTemplStr { get { return heightTmplStr; } }
        public int FontSize { get { return fontSize; } }
        public string StyleFont { get { return fontName; } }
        private void CreateFont(IntPtr _hdc, int _oglcontext, TypeFont _typeFont, string _fontName, int _fontSize)
        {
            oglcontext = _oglcontext;
            hdc = (int)_hdc;
            typeFont = _typeFont;
            fontName = _fontName;
            fontSize = _fontSize;
            fontBaseList = GLContex.PrepareFontForText((int)hdc, fontName, fontSize, (int)_typeFont, ref myfont);
            heightTmplStr = GetHeightText(templateString);
            widthTmplStr = GetWidthText(templateString);
            captionAxisWidth = GetWidthText("W");
        }

        public FontGeology(IntPtr _hdc, int _oglcontext, TypeFont _typeFont, string _fontName, int _fontSize)
        {
            CreateFont(_hdc, _oglcontext, _typeFont, _fontName, _fontSize);
        }
        public void NewFont(IntPtr _hdc, int _oglcontext, TypeFont _typeFont, string _fontName, int _fontSize)
        {
            ClearFont();
            CreateFont(_hdc, _oglcontext, _typeFont, _fontName, _fontSize);
        }

        public void PrintText(double x, double y, double z, string text)
        {
            GLContex.PrintText(fontBaseList, x, y, z, text);
        }
        public int GetWidthText(string text)
        {
            return GLContex.GetStringWidth(hdc, text, myfont);
        }
        public int GetHeightText(string text)
        {
            return GLContex.GetStringHeight(hdc, text, myfont);
        }
        public void ClearFont()
        {
            //  GLWin32.wglMakeCurrent((IntPtr)hdc, (IntPtr)oglcontext);

            GLContex.ClearFontText(fontBaseList, myfont);

            //     GLWin32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
        }

        ~FontGeology()
        {
            //  ClearFont();
        }
    }
}
