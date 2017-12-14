using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;

namespace WpfPitchTuner.Helper
{
    public class ShowMessageBoxAction : TriggerAction<DependencyObject>
    {
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ShowMessageBoxAction));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ShowMessageBoxAction));

        public ShowMessageBoxAction()
        {
            // Insert code required for object creation below this point.
        }

        protected override void Invoke(object o)
        {
            MessageBox.Show(Message, Title);
        }
    }
}
