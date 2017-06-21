using System.Windows.Forms;

namespace System
{
    internal class KeyEventHandler
    {
        private Action<object, KeyPressEventArgs> cnt_ic1_buffer_textBox_KeyPress;

        public KeyEventHandler(Action<object, KeyPressEventArgs> cnt_ic1_buffer_textBox_KeyPress)
        {
            this.cnt_ic1_buffer_textBox_KeyPress = cnt_ic1_buffer_textBox_KeyPress;
        }
    }
}