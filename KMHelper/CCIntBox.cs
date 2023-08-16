using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace KMHelper
{
    public class CCIntBox:TextBox
    {
        private string lastText = "";

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            e.Handled = !StringHelper.Number(e.Text);
            base.OnPreviewTextInput(e);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (Text.Length > 0)
            {
                if (Text.Contains(" "))
                {
                    Text = Text.Replace(" ", "");
                    SelectionStart = Text.Length;
                }
                else
                {
                    if (StringHelper.Number(Text))
                    {
                        lastText = Text;
                    }
                    else
                    {
                        Text = lastText;
                    }
                }
            }
            else
            {
                lastText = "";
            }
            base.OnTextChanged(e);
        }
    }

    public class StringHelper 
    {
        public static bool Number(string txt)
        {
            string monval = ("^[0-9]*$");
            return Regex.IsMatch(txt, monval);
        }
    }
}
