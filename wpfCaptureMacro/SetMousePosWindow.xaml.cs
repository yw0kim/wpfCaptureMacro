using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wpfCaptureMacro
{
    /// <summary>
    /// SetMousePosWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SetMousePosWindow : Window
    {
        public event EventHandler<mousePosArg> goBackMainWindowEvent;
        public int selectedIdx;
        public ActionEnum actionEnum;
        private double x, y;
        private Popup _popup;

        public SetMousePosWindow(ActionEnum _actionEnum, int _selectedIdx)
        {
            InitializeComponent();
            actionEnum = _actionEnum;
            if (actionEnum == ActionEnum.OuterNext)
            {
                MouseClickTextBlock.Text += "\n다음 화 위치";
            }
            else if (actionEnum == ActionEnum.InnerNext)
            {
                MouseClickTextBlock.Text += "\n다음 페이지 위치";
            }
            else if (actionEnum == ActionEnum.LastPixel)
            {
                MouseClickTextBlock.Text += "\n마지막 페이지 파랑색 위치";
            }
            selectedIdx = _selectedIdx;
        }
    

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            goBackMainWindowEvent(this, new mousePosArg(this.x, this.y, this.selectedIdx));
            _popup.IsOpen = false;
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            _popup.IsOpen = false;
            this.Close();
        }

        // https://code-examples.net/ko/q/6ad87 Capture
        // http://hoons.net/board/qanet3/content/45240 Loaded
        // https://stackoverflow.com/questions/4287635/how-to-getmouseposition-anywhere-on-the-screen-outside-the-bounds-of-window-or 
        // popup
        private void SetMousePosLoaded(object sender, RoutedEventArgs e)
        {
            //Label label = new Label();
            //label.Width = 100;
            //label.Height = 30;

            //this.Content = label;

            _popup = new Popup
            {
                Child = new TextBlock { Text = "=))", Background = Brushes.White },
                //Placement = PlacementMode.AbsolutePoint,
                Placement = PlacementMode.Center,
                StaysOpen = true,
                IsOpen = true
            };

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += delegate
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    Mouse.Capture(this);
                    Point pointToWindow = Mouse.GetPosition(this);
                    Point pointToScreen = PointToScreen(pointToWindow);
                    _popup.HorizontalOffset = pointToScreen.X;
                    _popup.VerticalOffset = pointToScreen.Y;
                    // label.Content = pointToScreen.ToString();

                    Mouse.Capture(null);
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        this.x = pointToScreen.X;
                        this.y = pointToScreen.Y;
                        MouseClickTextBlock.Text += "\n 확인 누르세요.";

                        timer.Stop();
                        return;
                    }
                }));
            };
            timer.Interval = 1;
            timer.Start();

            //label = new Label();
            //label.Width = 100;
            //label.Height = 30;

            //this.Content = label;

            //_popup = new Popup
            //{
            //    Child = new TextBlock { Text = "=))", Background = Brushes.White },
            //    //Placement = PlacementMode.AbsolutePoint,
            //    Placement = PlacementMode.Center,
            //    StaysOpen = true,
            //    IsOpen = true
            //};
            //MouseMove += MouseMoveMethod;
            //CaptureMouse();
        }

        //private void MouseMoveMethod(object sender, MouseEventArgs e)
        //{
        //    var relativePosition = e.GetPosition(this);
        //    var point = PointToScreen(relativePosition);
        //    label.Content = point.ToString();
        //    _popup.HorizontalOffset = point.X;
        //    _popup.VerticalOffset = point.Y;
        //    if (Mouse.LeftButton == MouseButtonState.Pressed)
        //    {
        //        this.x = point.X;
        //        this.y = point.Y;
        //        return;
        //    }
        //}
    }

    public class mousePosArg : EventArgs
    {
        public mousePosArg(double _x, double _y, int _selectedIdx)
        {
            x = _x;
            y = _y;
            selectedIdx = _selectedIdx;
        }
        private double x;
        public double X
        {
            get { return x; }
        }
        private double y;
        public double Y
        {
            get { return y; }
        }
        private int selectedIdx;
        public int SelectedIdx
        {
            get { return selectedIdx; }
        }
    }
}
