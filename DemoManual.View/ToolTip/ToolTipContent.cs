using System.Windows.Forms;

namespace ToolTip
{
    // a very simple Control to be used as the content of the InteractiveToolTip
    public partial class ToolTipContent : UserControl
    {
        public ToolTipContent()
        {
            InitializeComponent();
        }
    }

    public enum ToolTipStatus 
    {
        Hide,
        Waiting,
        Showing
    }
}
