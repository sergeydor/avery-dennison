using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace Client.UIApp.UIElements
{
    public class TextBoxHexDigits : WatermarkTextBox
    {
        public TextBoxHexDigits()
        {
          //  Watermark watermark = new Watermark();
            this.Watermark = "Type HEX digits 0..F";
            this.ToolTip = this.Watermark;
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            e.Handled = !IsHexInString(e.Text);
        }
        
        public bool IsHexInString(string test)
        {
            // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }
    }
}
