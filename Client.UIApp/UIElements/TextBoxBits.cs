using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace Client.UIApp.UIElements
{
    public class TextBoxBits : WatermarkTextBox
    {
        public TextBoxBits()
        {
            this.Watermark = "Type bits 0 or 1";
            this.ToolTip = this.Watermark;
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-1]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }
    }
}
