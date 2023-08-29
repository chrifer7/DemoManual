#region License

//
// InteractiveToolTip.cs
//
// Copyright (C) 2012-2013 Alex Taylor.  All Rights Reserved.
//
// InteractiveToolTip is published under the terms of the Code Project Open License.
// http://www.codeproject.com/info/cpol10.aspx
#pragma warning disable S125
/*	Preamble

		This License governs Your use of the Work. This License is intended to allow developers to use the 
		Source Code and Executable Files provided as part of the Work in any application in any form.

		The main points subject to the terms of the License are:
			- Source Code and Executable Files can be used in commercial applications;
			- Source Code and Executable Files can be redistributed; and
			- Source Code can be modified to create derivative works.
			- No claim of suitability, guarantee, or any warranty whatsoever is provided. The software is provided "as-is".
			- The Article(s) accompanying the Work may not be distributed or republished without the Author's consent

		This License is entered between You, the individual or other entity reading or otherwise making use of the Work 
		licensed pursuant to this License and the individual or other entity which offers the Work under the terms of this License ("Author").
*/
#pragma warning restore S125

#endregion License

namespace Digitalis.GUI.Controls
{
    #region Usings

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    #endregion Usings

    /// <summary>
    /// Represents a balloon-style <see cref="T:System.Windows.Forms.ToolTip"/> which supports caller-supplied interactive content.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>InteractiveToolTip</b> offers similar behaviour to the modal functionality of <see cref="T:System.Windows.Forms.ToolTip"/>, but replaces the text-content
    /// with a <see cref="T:System.Windows.Forms.Control"/>, which may be used to provide either more complex formatting than is possible with <see cref="T:System.Windows.Forms.ToolTip"/>,
    /// or to implement an interactive control model.
    /// </para>
    /// <para>
    /// The <see cref="T:System.Windows.Forms.Control"/> may be anything you like. Transparent <see cref="F:System.Windows.Forms.Control.BackColor"/>s are supported.
    /// </para>
    /// </remarks>
    public partial class InteractiveToolTip : Component
    {
        #region Inner types

        /// <summary>
        /// Specifies the preferred position of the <see cref="InteractiveToolTip"/>'s stem.
        /// </summary>
        public enum StemPosition
        {
            /// <summary>
            /// The stem should be positioned at the bottom-left corner of the balloon.
            /// </summary>
            BottomLeft,

            /// <summary>
            /// The stem should be positioned in the centre of the bottom edge of the balloon.
            /// </summary>
            BottomCentre,

            /// <summary>
            /// The stem should be positioned at the bottom-right corner of the balloon.
            /// </summary>
            BottomRight,

            /// <summary>
            /// The stem should be positioned at the top-left corner of the balloon.
            /// </summary>
            TopLeft,

            /// <summary>
            /// The stem should be positioned in the centre of the top edge of the balloon.
            /// </summary>
            TopCentre,

            /// <summary>
            /// The stem should be positioned at the top-right corner of the balloon.
            /// </summary>
            TopRight
        }

        private sealed class Win32
        {
            public const string TooltipsClass         = "tooltips_class32";
            public const int TtsAlwaystip             = 0x01;
            public const int TtsNofade                = 0x10;
            public const int TtsNoanimate             = 0x20;
            public const int TtsBalloon               = 0x40;
            public const int TtfIdishwnd              = 0x0001;
            public const int TtfCentertip             = 0x0002;
            public const int TtfTrack                 = 0x0020;
            public const int TtfTransparent           = 0x0100;
            public const int WmSetfont                = 0x30;
            public const int WmGetfont                = 0x31;
            public const int WmPrintclient            = 0x318;
            public const int WmUser                   = 0x0400;
            public const int TtmTrackactivate         = WmUser + 17;
            public const int TtmTrackposition         = WmUser + 18;
            public const int TtmSetmaxtipwidth        = WmUser + 24;
            public const int TtmGetbubblesize         = WmUser + 30;
            public const int TtmAddtool               = WmUser + 50;
            public const int TtmDeltool               = WmUser + 51;
            public const int SwpNosize                = 0x0001;
            public const int SwpNoactivate            = 0x0010;
            public const int SwpNoownerzorder         = 0x200;
            public readonly static IntPtr HwndTopmost = new IntPtr(-1);

            [StructLayout(LayoutKind.Sequential)]
            public struct Rect
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Toolinfo
            {
                public int cbSize;
                public int uFlags;
                public IntPtr hwnd;
                public IntPtr uId;
                public Rect rect;
                public IntPtr hinst;

                [MarshalAs(UnmanagedType.LPTStr)]
                public string lpszText;

                public System.UInt32 lParam;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Size
            {
                public int cx;
                public int cy;
            }

            [DllImport("User32", SetLastError = true)]
            public static extern int GetWindowRect(IntPtr hWnd, ref Rect lpRect);

            [DllImport("User32", SetLastError = true)]
            public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref Toolinfo lParam);

            [DllImport("User32", SetLastError = true)]
            public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, out Rect lParam);

            [DllImport("User32", SetLastError = true)]
            public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

            [DllImport("User32", SetLastError = true)]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

            [DllImport("User32", SetLastError = true)]
            public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        }

        private class ContentPanel : UserControl
        {
            private IntPtr _toolTipHwnd;

            public ContentPanel(IntPtr toolTipHWnd)
            {
                _toolTipHwnd = toolTipHWnd;
                Win32.SetParent(Handle, toolTipHWnd);
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                // paint the balloon
                Win32.SendMessage(_toolTipHwnd, Win32.WmPrintclient, (int)e.Graphics.GetHdc(), 0);
            }
        }

        private class ToolTipWindow : NativeWindow, IDisposable
        {
            #region Internals

            // the distance from the edge of the balloon to the edge of the stem
            private const int STEM_INSET = 16;
            private static StringFormat _stringFormat = new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };

            private ContentPanel _contentPanel;
            private Win32.Toolinfo _toolInfo;
            private bool _mouseOverToolTip;

            private Win32.Toolinfo CreateTool(string contentSpacing, IWin32Window window, StemPosition stemPosition)
            {
                Win32.Toolinfo ti = new Win32.Toolinfo();

                ti.cbSize   = Marshal.SizeOf(ti);
                ti.uFlags   = Win32.TtfIdishwnd | Win32.TtfTrack | Win32.TtfTransparent;
                ti.uId      = window.Handle;
                ti.hwnd     = window.Handle;
                ti.lpszText = contentSpacing;

                if (StemPosition.BottomCentre == stemPosition || StemPosition.TopCentre == stemPosition)
                    ti.uFlags |= Win32.TtfCentertip;

                if (0 == Win32.SendMessage(Handle, Win32.TtmAddtool, 0, ref ti))
                    throw new Exception();

                // enable multi-line text-layout
                Win32.SendMessage(Handle, Win32.TtmSetmaxtipwidth, 0, SystemInformation.MaxWindowTrackSize.Width);

                return ti;
            }

            private StemPosition AdjustStemPosition(StemPosition stemPosition, ref Rectangle toolTipBounds, ref Rectangle screenBounds)
            {
                if (toolTipBounds.Left < screenBounds.Left)
                {
                    // the window is too close to the left edge of the display
                    if (StemPosition.TopCentre == stemPosition || StemPosition.TopRight == stemPosition)
                        stemPosition = StemPosition.TopLeft;
                    else if (StemPosition.BottomCentre == stemPosition || StemPosition.BottomRight == stemPosition)
                        stemPosition = StemPosition.BottomLeft;
                }
                else if (toolTipBounds.Right > screenBounds.Right)
                {
                    // the window is too close to the right edge of the display
                    if (StemPosition.TopCentre == stemPosition || StemPosition.TopLeft == stemPosition)
                        stemPosition = StemPosition.TopRight;
                    else if (StemPosition.BottomCentre == stemPosition || StemPosition.BottomLeft == stemPosition)
                        stemPosition = StemPosition.BottomRight;
                }

                if (toolTipBounds.Top < screenBounds.Top)
                {
                    // the window is too close to the top edge of the display
                    switch (stemPosition)
                    {
                        case StemPosition.BottomLeft:
                            stemPosition = StemPosition.TopLeft;
                            break;

                        case StemPosition.BottomCentre:
                            stemPosition = StemPosition.TopCentre;
                            break;

                        case StemPosition.BottomRight:
                            stemPosition = StemPosition.TopRight;
                            break;
                    }
                }
                else if (toolTipBounds.Bottom > screenBounds.Bottom)
                {
                    // the window is too close to the bottom edge of the display
                    switch (stemPosition)
                    {
                        case StemPosition.TopLeft:
                            stemPosition = StemPosition.BottomLeft;
                            break;

                        case StemPosition.TopCentre:
                            stemPosition = StemPosition.BottomCentre;
                            break;

                        case StemPosition.TopRight:
                            stemPosition = StemPosition.BottomRight;
                            break;
                    }
                }

                return stemPosition;
            }

            private Rectangle CalculateToolTipLocation(string contentSpacing, IWin32Window window, int x, int y, StemPosition stemPosition)
            {
                Rectangle toolTipBounds  = new Rectangle();
                Size toolTipSize         = GetToolTipWindowSize(contentSpacing);
                Win32.Rect windowBounds = new Win32.Rect();

                Win32.GetWindowRect(window.Handle, ref windowBounds);
                x += windowBounds.left;

                if (StemPosition.TopLeft == stemPosition || StemPosition.BottomLeft == stemPosition)
                    toolTipBounds.X = x - STEM_INSET;
                else if (StemPosition.TopCentre == stemPosition || StemPosition.BottomCentre == stemPosition)
                    toolTipBounds.X = x - (toolTipSize.Width / 2);
                else
                    toolTipBounds.X = x - toolTipSize.Width + STEM_INSET;

                if (StemPosition.TopLeft == stemPosition || StemPosition.TopCentre == stemPosition || StemPosition.TopRight == stemPosition)
                    toolTipBounds.Y = windowBounds.bottom - y;
                else
                    toolTipBounds.Y = y + windowBounds.top - toolTipSize.Height;

                toolTipBounds.Width  = toolTipSize.Width;
                toolTipBounds.Height = toolTipSize.Height;

                return toolTipBounds;
            }

            private Size GetToolTipWindowSize(string contentSpacing)
            {
                Win32.Toolinfo ti = new Win32.Toolinfo();
                ti.cbSize          = Marshal.SizeOf(ti);
                ti.uFlags          = Win32.TtfTrack;
                ti.lpszText        = contentSpacing;

                if (0 == Win32.SendMessage(Handle, Win32.TtmAddtool, 0, ref ti))
                    throw new Exception();

                // enable multi-line text-layout
                Win32.SendMessage(Handle, Win32.TtmSetmaxtipwidth, 0, SystemInformation.MaxWindowTrackSize.Width);
                Win32.SendMessage(Handle, Win32.TtmTrackactivate, 1, ref ti);

                Win32.Rect rect = new Win32.Rect();
                Win32.GetWindowRect(Handle, ref rect);

                Win32.SendMessage(Handle, Win32.TtmTrackactivate, 0, ref ti);
                Win32.SendMessage(Handle, Win32.TtmDeltool, 0, ref ti);

                return new Size(rect.right - rect.left, rect.bottom - rect.top);
            }

            private void DoLayout(IWin32Window window, Control content, StemPosition stemPosition, ref Rectangle toolTipBounds)
            {
                int bubbleSize   = Win32.SendMessage(Handle, Win32.TtmGetbubblesize, 0, ref _toolInfo);
                int bubbleWidth  = bubbleSize & 0xFFFF;
                int bubbleHeight = bubbleSize >> 16;

                // centre our content on the bubble-area of the tooltip
                content.Left = (bubbleWidth - content.Width) / 2;

                if (StemPosition.BottomLeft == stemPosition || StemPosition.BottomCentre == stemPosition || StemPosition.BottomRight == stemPosition)
                {
                    // stem is below the bubble
                    content.Top = (bubbleHeight - content.Height) / 2;
                }
                else
                {
                    // stem is on top of the bubble
                    int bubbleOffset = toolTipBounds.Height - bubbleHeight;
                    content.Top = (bubbleHeight - content.Height) / 2 + bubbleOffset;
                }

                _contentPanel        = new ContentPanel(Handle);
                _contentPanel.Width  = toolTipBounds.Width;
                _contentPanel.Height = toolTipBounds.Height;
                _contentPanel.Controls.Add(content);

                Win32.SetWindowPos(Handle, Win32.HwndTopmost, toolTipBounds.X, toolTipBounds.Y, 0, 0, Win32.SwpNoactivate | Win32.SwpNosize | Win32.SwpNoownerzorder);
            }

            private string GetSizingText(Control content)
            {
                // we can't set the dimensions of the tooltip directly - they are controlled by the space required to render its
                // text-content - so we must fake a string with approximately the same dimensions when rendered as our content
                StringBuilder sb  = new StringBuilder();
                Graphics graphics = Graphics.FromHwnd(Handle);
                Font font         = Font.FromHfont((IntPtr)Win32.SendMessage(Handle, Win32.WmGetfont, 0, 0));

                // use a small font to improve precision
                font = new Font(font.FontFamily, 1.0f);
                Win32.SendMessage(Handle, Win32.WmSetfont, (int)font.ToHfont(), 1);

                Size size = TextRenderer.MeasureText(" ", font);
                int rows  = (content.Height + size.Height - 1) / size.Height;

                for (int n = 0; n < rows; n++)
                {
                    sb.Append("\r\n");
                }

                size = TextRenderer.MeasureText(sb.ToString(), font);

                // pad the width out to match the spacing on the height so the border around the content is of roughly constant size
                int width = content.Width + size.Height - content.Height;

                // we can't do a simple 'how many columns' calculation here, as the text-renderer will apply kerning
                while (size.Width < width)
                {
                    sb.Append(" ");
                    size = TextRenderer.MeasureText(sb.ToString(), font);
                }

                return sb.ToString();
            }

            #endregion Internals

            #region Constructor

            public ToolTipWindow(Control content, IWin32Window window, int x, int y, StemPosition stemPosition, bool useAnimation, bool useFading)
            {
                this.window = window;

                CreateParams createParams = new CreateParams();
                createParams.ClassName = Win32.TooltipsClass;
                createParams.Style = Win32.TtsAlwaystip | Win32.TtsBalloon;

                if (!useAnimation)
                    createParams.Style |= Win32.TtsNoanimate;

                if (!useFading)
                    createParams.Style |= Win32.TtsNofade;

                CreateHandle(createParams);

                // first, work out the actual stem-position: the supplied value is a hint, but may have to be changed if there isn't enough space to accomodate it
                string contentSpacing = GetSizingText(content);

                // this is where the caller would like us to be
                Rectangle toolTipBounds = CalculateToolTipLocation(contentSpacing, this.window, x, y, stemPosition);
                Screen currentScreen    = Screen.FromHandle(this.window.Handle);
                Rectangle screenBounds  = currentScreen.WorkingArea;

                stemPosition = AdjustStemPosition(stemPosition, ref toolTipBounds, ref screenBounds);

                // and this is where we'll actually end up
                toolTipBounds = CalculateToolTipLocation(contentSpacing, this.window, x, y, stemPosition);
                toolTipBounds.X = Math.Max(0, toolTipBounds.X);
                toolTipBounds.Y = Math.Max(0, toolTipBounds.Y);

                // build the tooltip window
                _toolInfo = CreateTool(contentSpacing, this.window, stemPosition);

                // initial position to force the stem into the correct orientation
                int initialX = screenBounds.X;
                int initialY = screenBounds.Y;

                if (StemPosition.TopLeft == stemPosition || StemPosition.BottomLeft == stemPosition)
                    initialX += STEM_INSET;
                else if (StemPosition.TopCentre == stemPosition || StemPosition.BottomCentre == stemPosition)
                    initialX += screenBounds.Width / 2;
                else
                    initialX += screenBounds.Width - STEM_INSET;

                if (StemPosition.BottomLeft == stemPosition || StemPosition.BottomCentre == stemPosition || StemPosition.BottomRight == stemPosition)
                    initialY += screenBounds.Height;

                Win32.SendMessage(Handle, Win32.TtmTrackposition, 0, (initialY << 16) | initialX);

                // and finally display it
                Win32.SendMessage(Handle, Win32.TtmTrackactivate, 1, ref _toolInfo);
                DoLayout(this.window, content, stemPosition, ref toolTipBounds);

                _contentPanel.MouseEnter += delegate(object sender, EventArgs e)
                {
                    if (null != MouseEnter && !_mouseOverToolTip)
                    {
                        _mouseOverToolTip = true;
                        MouseEnter(this, e);
                    }
                };

                _contentPanel.MouseLeave += delegate(object sender, EventArgs e)
                {
                    // only send the event if the mouse has actually left the balloon and not simply moved from _contentPanel to the caller-supplied content
                    if (null != MouseLeave && _mouseOverToolTip && null == _contentPanel.GetChildAtPoint(_contentPanel.PointToClient(Control.MousePosition)))
                    {
                        _mouseOverToolTip = false;
                        MouseLeave(this, e);
                    }
                };
            }

            ~ToolTipWindow()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    Win32.SendMessage(Handle, Win32.TtmTrackactivate, 0, ref _toolInfo);
                    Win32.SendMessage(Handle, Win32.TtmDeltool, 0, ref _toolInfo);

                    if (null != _contentPanel)
                    {
                        _contentPanel.Controls.Clear();
                        _contentPanel.Dispose();
                    }

                    DestroyHandle();
                }
            }

            #endregion Constructor

            #region API

            public IWin32Window window;

            public event EventHandler MouseEnter;

            public event EventHandler MouseLeave;

            #endregion API
        }

        #endregion Inner types

        #region Internals

        private Timer _durationTimer;
        private ToolTipWindow _currentToolTip;

        #endregion Internals

        #region Properties

        /// <summary>
        /// Gets or sets a value determining whether an animation effect should be used when displaying the <see cref="InteractiveToolTip"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <c>true</c> if window animation should be used; otherwise, <c>false</c>. The default is <c>true</c>.
        /// </para>
        /// </remarks>
        public bool UseAnimation { get; set; }

        /// <summary>
        /// Gets or sets a value determining whether a fade effect should be used when displaying the <see cref="InteractiveToolTip"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <c>true</c> if window fading should be used; otherwise, <c>false</c>. The default is <c>true</c>.
        /// </para>
        /// </remarks>
        public bool UseFading { get; set; }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractiveToolTip"/> class without a specified container.
        /// </summary>
        public InteractiveToolTip()
        {
            InitializeComponent();
            UseAnimation = true;
            UseFading    = true;

            _durationTimer = new Timer();
            components.Add(_durationTimer);

            _durationTimer.Tick += delegate(object sender, EventArgs e)
            {
                Hide();
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractiveToolTip"/> class with a specified container.
        /// </summary>
        /// <param name="container">An <see cref="T:System.ComponentModel.IContainer"/> that represents the container of the <see cref="InteractiveToolTip"/>.</param>
        public InteractiveToolTip(IContainer container)
            : this()
        {
            container.Add(this);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="InteractiveToolTip"/> is reclaimed by garbage collection.
        /// </summary>
        ~InteractiveToolTip()
        {
            Dispose(false);
        }

        #endregion Constructor

        #region API

        /// <summary>
        /// Sets the ToolTip content associated with the specified control, and displays the <see cref="InteractiveToolTip"/>.
        /// </summary>
        /// <param name="content">The content to be displayed in the <see cref="InteractiveToolTip"/>.</param>
        /// <param name="window">The <see cref="T:System.Windows.Forms.Control"/> to display the <see cref="InteractiveToolTip"/> for.</param>
        /// <remarks>
        /// <para>
        /// The <see cref="InteractiveToolTip"/> is displayed until either one of the <b>Show</b> methods is called to display another <see cref="InteractiveToolTip"/>,
        /// or <see cref="Hide"/> is called. It will be drawn with the stem positioned at the bottom-left of the balloon, tip located at the top-left corner of
        /// <paramref name="window"/>. If there is insufficient space on the display for this, the stem and balloon may be repositioned to accomodate this.
        /// </para>
        /// </remarks>
        public void Show(Control content, IWin32Window window)
        {
            Show(content, window, StemPosition.BottomLeft);
        }

        /// <summary>
        /// Sets the ToolTip content associated with the specified control, and displays the <see cref="InteractiveToolTip"/> with its stem in the specified position.
        /// </summary>
        /// <param name="content">The content to be displayed in the <see cref="InteractiveToolTip"/>.</param>
        /// <param name="window">The <see cref="T:System.Windows.Forms.Control"/> to display the <see cref="InteractiveToolTip"/> for.</param>
        /// <param name="stemPosition">The desired position for the stem of the balloon.</param>
        /// <remarks>
        /// <para>
        /// The <see cref="InteractiveToolTip"/> is displayed until either one of the <b>Show</b> methods is called to display another <see cref="InteractiveToolTip"/>,
        /// or <see cref="Hide"/> is called. It will be drawn with the stem positioned at the specified part of the balloon, tip located at either the top-left corner
        /// (if <paramref name="stemPosition"/> specifies the bottom edge) or the bottom-left corner (if <paramref name="stemPosition"/> specifies the top edge) of
        /// <paramref name="window"/>. If there is insufficient space on the display for this, the stem and balloon may be repositioned to accomodate this.
        /// </para>
        /// </remarks>
        public void Show(Control content, IWin32Window window, StemPosition stemPosition)
        {
            Show(content, window, 0, 0, stemPosition, 0);
        }

        /// <summary>
        /// Sets the ToolTip content associated with the specified control, and displays the <see cref="InteractiveToolTip"/> at the specified relative position.
        /// </summary>
        /// <param name="content">The content to be displayed in the <see cref="InteractiveToolTip"/>.</param>
        /// <param name="window">The <see cref="T:System.Windows.Forms.Control"/> to display the <see cref="InteractiveToolTip"/> for.</param>
        /// <param name="location">A <see cref="T:System.Drawing.Point"/> containing the offset, in pixels, relative to the upper-left corner of the associated control window, to display the <see cref="InteractiveToolTip"/>.</param>
        /// <remarks>
        /// <para>
        /// The <see cref="InteractiveToolTip"/> is displayed until either one of the <b>Show</b> methods is called to display another <see cref="InteractiveToolTip"/>,
        /// or <see cref="Hide"/> is called. It will be drawn with the stem positioned at the bottom-left of the balloon, tip located at the specified position relative
        /// to the top-left corner of <paramref name="window"/>. If there is insufficient space on the display for this, the stem and balloon may be repositioned to
        /// accomodate this.
        /// </para>
        /// </remarks>
        public void Show(Control content, IWin32Window window, Point location)
        {
            Show(content, window, location.X, location.Y);
        }

        /// <summary>
        /// Sets the ToolTip content associated with the specified control, and displays the <see cref="InteractiveToolTip"/> at the specified relative position.
        /// </summary>
        /// <param name="content">The content to be displayed in the <see cref="InteractiveToolTip"/>.</param>
        /// <param name="window">The <see cref="T:System.Windows.Forms.Control"/> to display the <see cref="InteractiveToolTip"/> for.</param>
        /// <param name="x">The horizontal offset, in pixels, relative to the upper-left corner of the associated control window, to display the <see cref="InteractiveToolTip"/>.</param>
        /// <param name="y">The vertical offset, in pixels, relative to the upper-left corner of the associated control window, to display the <see cref="InteractiveToolTip"/>.</param>
        /// <remarks>
        /// <para>
        /// The <see cref="InteractiveToolTip"/> is displayed until either one of the <b>Show</b> methods is called to display another <see cref="InteractiveToolTip"/>,
        /// or <see cref="Hide"/> is called. It will be drawn with the stem positioned at the bottom-left of the balloon, tip located at the specified position relative
        /// to the top-left corner of <paramref name="window"/>. If there is insufficient space on the display for this, the stem and balloon may be repositioned to
        /// accomodate this.
        /// </para>
        /// </remarks>
        public void Show(Control content, IWin32Window window, int x, int y)
        {
            Show(content, window, x, y, StemPosition.BottomLeft, 0);
        }

        /// <summary>
        /// Sets the ToolTip content associated with the specified control, and displays the <see cref="InteractiveToolTip"/> at the specified relative position.
        /// </summary>
        /// <param name="content">The content to be displayed in the <see cref="InteractiveToolTip"/>.</param>
        /// <param name="window">The <see cref="T:System.Windows.Forms.Control"/> to display the <see cref="InteractiveToolTip"/> for.</param>
        /// <param name="location">A <see cref="T:System.Drawing.Point"/> containing the offset, in pixels, relative to the upper-left corner of the associated control window, to display the <see cref="InteractiveToolTip"/>.</param>
        /// <param name="stemPosition">The desired position for the stem of the balloon.</param>
        /// <param name="duration">The time in milliseconds for which the <see cref="InteractiveToolTip"/> should be displayed, or zero for indefinite display.</param>
        /// <remarks>
        /// <para>
        /// The <see cref="InteractiveToolTip"/> is displayed until either one of the <b>Show</b> methods is called to display another <see cref="InteractiveToolTip"/>,
        /// or <see cref="Hide"/> is called, or the specified <paramref name="duration"/> is exceeded. It will be drawn with the stem positioned at the specified part
        /// of the balloon, tip located at the specified position relative to the top-left corner of <paramref name="window"/>. If there is insufficient space on the
        /// display for this, the stem and balloon may be repositioned to accomodate this.
        /// </para>
        /// <para>
        /// If the mouse is moved over the <see cref="InteractiveToolTip"/>, the duration timer will be halted. It will resume - with its original value - when the mouse
        /// leaves the <see cref="InteractiveToolTip"/> again.
        /// </para>
        /// </remarks>
        public void Show(Control content, IWin32Window window, Point location, StemPosition stemPosition, int duration)
        {
            Show(content, window, location.X, location.Y, stemPosition, duration);
        }

        /// <summary>
        /// Sets the ToolTip content associated with the specified control, and displays the <see cref="InteractiveToolTip"/> at the specified relative position.
        /// </summary>
        /// <param name="content">The content to be displayed in the <see cref="InteractiveToolTip"/>.</param>
        /// <param name="window">The <see cref="T:System.Windows.Forms.Control"/> to display the <see cref="InteractiveToolTip"/> for.</param>
        /// <param name="x">The horizontal offset, in pixels, relative to the upper-left corner of the associated control window, to display the <see cref="InteractiveToolTip"/>.</param>
        /// <param name="y">The vertical offset, in pixels, relative to the upper-left corner of the associated control window, to display the <see cref="InteractiveToolTip"/>.</param>
        /// <param name="stemPosition">The desired position for the stem of the balloon.</param>
        /// <param name="duration">The time in milliseconds for which the <see cref="InteractiveToolTip"/> should be displayed, or zero for indefinite display.</param>
        /// <remarks>
        /// <para>
        /// The <see cref="InteractiveToolTip"/> is displayed until either one of the <b>Show</b> methods is called to display another <see cref="InteractiveToolTip"/>,
        /// or <see cref="Hide"/> is called, or the specified <paramref name="duration"/> is exceeded. It will be drawn with the stem positioned at the specified part
        /// of the balloon, tip located at the specified position relative to the top-left corner of <paramref name="window"/>. If there is insufficient space on the
        /// display for this, the stem and balloon may be repositioned to accomodate this.
        /// </para>
        /// <para>
        /// If the mouse is moved over the <see cref="InteractiveToolTip"/>, the duration timer will be halted. It will resume - with its original value - when the mouse
        /// leaves the <see cref="InteractiveToolTip"/> again.
        /// </para>
        /// </remarks>
        public void Show(Control content, IWin32Window window, int x, int y, StemPosition stemPosition, int duration)
        {
            if (null == content || null == window)
                throw new ArgumentNullException();

            Hide();
            _currentToolTip = new ToolTipWindow(content, window, x, y, stemPosition, UseAnimation, UseFading);

            if (duration > 0)
            {
                _currentToolTip.MouseEnter += delegate(object sender, EventArgs e)
                {
                    _durationTimer.Stop();
                };

                _currentToolTip.MouseLeave += delegate(object sender, EventArgs e)
                {
                    if (duration > 0)
                        _durationTimer.Start();
                };

                _durationTimer.Interval = duration;
                _durationTimer.Start();
            }

            if (null != ToolTipShown)
                ToolTipShown(this, new InteractiveToolTipEventArgs(window));
        }

        /// <summary>
        /// Hides the current <see cref="InteractiveToolTip"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the <see cref="InteractiveToolTip"/> is not visible, the method just returns.
        /// </para>
        /// </remarks>
        public void Hide()
        {
            _durationTimer.Stop();

            ToolTipWindow toolTip = _currentToolTip;
            IWin32Window window;

            if (null != toolTip)
            {
                _currentToolTip = null;
                window = toolTip.window;
                toolTip.Dispose();

                if (null != ToolTipHidden)
                    ToolTipHidden(this, new InteractiveToolTipEventArgs(window));
            }
        }

        /// <summary>
        /// Occurs when an <see cref="InteractiveToolTip"/> is shown.
        /// </summary>
        public event InteractiveToolTipEventHandler ToolTipShown;

        /// <summary>
        /// Occurs when an <see cref="InteractiveToolTip"/> is hidden.
        /// </summary>
        public event InteractiveToolTipEventHandler ToolTipHidden;

        #endregion API
   }

    /// <summary>
    /// Represents a method which is invoked when an <see cref="InteractiveToolTip"/> is shown.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event-args instance containing the event data.</param>
    public delegate void InteractiveToolTipEventHandler(object sender, InteractiveToolTipEventArgs e);

    /// <summary>
    /// Provides data for the <see cref="InteractiveToolTip.ToolTipShown"/> and <see cref="InteractiveToolTip.ToolTipHidden"/> events.
    /// </summary>
    public class InteractiveToolTipEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the window representing the content of the <see cref="InteractiveToolTip"/>.
        /// </summary>
        public IWin32Window Window { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractiveToolTipEventArgs"/> class with the specified parameters.
        /// </summary>
        /// <param name="window">The window representing the content of the <see cref="InteractiveToolTip"/>.</param>
        public InteractiveToolTipEventArgs(IWin32Window window)
            : base()
        {
            Window = window;
        }
    }
}