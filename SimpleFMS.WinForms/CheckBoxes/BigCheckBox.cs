using System.Drawing;
using System.Windows.Forms;

namespace SimpleFMS.WinForms.CheckBoxes
{
    // Inspriation for code found here
    // http://stackoverflow.com/questions/3166244/how-to-increase-the-size-of-checkbox-in-winforms
    public class BigCheckBox : CheckBox
    {

        public BigCheckBox()
        {
            AutoSize = false;
        }

        public override bool AutoSize
        {
            set { base.AutoSize = false; }
            get { return base.AutoSize; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);


            int h = ClientSize.Height - 2;

            Rectangle rect = new Rectangle(0, 1, h, h);

            ControlPaint.DrawCheckBox(e.Graphics, rect, this.Checked ? ButtonState.Checked : ButtonState.Normal);
        }
    }
}
