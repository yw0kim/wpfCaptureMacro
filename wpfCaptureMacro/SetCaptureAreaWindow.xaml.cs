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

// https://markheath.net/post/how-to-drag-shapes-on-canvas-in-wpf
// https://insurang.tistory.com/13
// https://www.codeproject.com/Articles/148503/Simple-Drag-Selection-in-WPF
namespace wpfCaptureMacro
{
    /// <summary>
    /// SetCaptureAreaWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SetCaptureAreaWindow : Window
    {
        public event EventHandler<captureAreaArg> goBackMainWindowEvent;
        public int selectedIdx;
        bool isDraggingSelectionRect;
        bool isLeftMouseButtonDownOnWindow;
        Point origMouseDownPoint;
        double width, height;
        double DragThreshold;
        private Popup _popup;
        private bool isTest;

        private void areaConfirm(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                okTestBtnClick(sender, e);
            }
        }

        private void okTestBtnClick(object sender, RoutedEventArgs e)
        {
            _popup.IsOpen = false;
            this.Close();
        }

        public SetCaptureAreaWindow(int _selectedIdx)
        {
            InitializeComponent();
            selectedIdx = _selectedIdx;
            isDraggingSelectionRect = false;
            isLeftMouseButtonDownOnWindow = false;
            DragThreshold = 50.0;
            isTest = false;
        }

        public SetCaptureAreaWindow(int _selectedIdx, bool isTest, Rect area)
        {
            InitializeComponent();
            if (isTest)
            {
                isTest = true;
                Canvas.SetLeft(dragSelectionBorder, area.X);
                Canvas.SetTop(dragSelectionBorder, area.Y);
                dragSelectionBorder.Width = area.Width;
                dragSelectionBorder.Height = area.Height;

                Button ok = new Button();
                ok.Content = "캡처 영역 확인";
                ok.Click += okTestBtnClick;
                ok.FontSize = 18;

                _popup = new Popup
                {
                    Child = ok,
                    Placement = PlacementMode.Center,
                    StaysOpen = true,
                    IsOpen = true,
                };

                _popup.HorizontalOffset = area.X + area.Width;
                _popup.VerticalOffset = area.Y + area.Height;

                dragSelectionCanvas.Visibility = Visibility.Visible;

                // MouseLeftButtonDown="Down_AreaStart" MouseLeftButtonUp="Up_AreaEnd" MouseMove="Move_Area"
                MouseLeftButtonDown -= Down_AreaStart;
                MouseLeftButtonDown += areaConfirm;
                MouseLeftButtonUp -= Up_AreaEnd;
                MouseMove -= Move_Area;
            }
        }



        private void Down_AreaStart(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !isTest)
            {
                isLeftMouseButtonDownOnWindow = true;
                origMouseDownPoint = e.GetPosition(this);
                this.CaptureMouse();

                e.Handled = true;
                if (_popup != null)
                    _popup.IsOpen = false;
            }
        }

        private void okButtonClick(object sender, RoutedEventArgs e)
        {
            goBackMainWindowEvent(this, new captureAreaArg(this.origMouseDownPoint, this.width, this.height, this.selectedIdx));
            _popup.IsOpen = false;
            this.Close();
        }

        private void ApplyDragSelectionRect()
        {
            // dragSelectionCanvas.Visibility = Visibility.Collapsed;

            double x = Canvas.GetLeft(dragSelectionBorder);
            double y = Canvas.GetTop(dragSelectionBorder);
            double width = dragSelectionBorder.Width;
            double height = dragSelectionBorder.Height;
            // Rect dragRect = new Rect(x, y, width, height);

            //
            // Inflate the drag selection-rectangle by 1/10 of its size to 
            // make sure the intended item is selected.
            //
            // dragRect.Inflate(width / 10, height / 10);

            origMouseDownPoint = new Point(x, y);
            this.width = width;
            this.height = height;

            Button ok = new Button();
            ok.Content = "영역 지정 확인";
            ok.Click += okButtonClick;
            ok.FontSize = 18;

            _popup = new Popup
            {
                Child = ok,
                Placement = PlacementMode.Center,
                StaysOpen = true,
                IsOpen = true,
            };

            Mouse.Capture(this);
            Point pointToWindow = Mouse.GetPosition(this);
            Point pointToScreen = PointToScreen(pointToWindow);
            _popup.HorizontalOffset = pointToScreen.X;
            _popup.VerticalOffset = pointToScreen.Y;

        }

        private void Up_AreaEnd(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !isTest)
            {
                if (isDraggingSelectionRect)
                {
                    //
                    // Drag selection has ended, apply the 'selection rectangle'.
                    //

                    isDraggingSelectionRect = false;
                    ApplyDragSelectionRect();

                    e.Handled = true;
                }

                if (isLeftMouseButtonDownOnWindow)
                {
                    isLeftMouseButtonDownOnWindow = false;
                    this.ReleaseMouseCapture();

                    e.Handled = true;
                }
            }
        }

        private void UpdateDragSelectionRect(Point pt1, Point pt2)
        {
            double x, y, width, height;

            width = (pt2 - pt1).X;
            height = (pt2 - pt1).Y;

            if (width < 0)
            {
                x = pt2.X;
                width = Math.Abs(width);
            } else
            {
                x = pt1.X;
            }

            if (height < 0)
            {
                y = pt2.Y;
                height = Math.Abs(height);
            }
            else
            {
                y = pt1.Y;
            }
            //
            // Update the coordinates of the rectangle used for drag selection.
            //
            Canvas.SetLeft(dragSelectionBorder, x);
            Canvas.SetTop(dragSelectionBorder, y);
            dragSelectionBorder.Width = width;
            dragSelectionBorder.Height = height;
        }

        private void InitDragSelectionRect(Point pt1, Point pt2)
        {
            UpdateDragSelectionRect(pt1, pt2);

            dragSelectionCanvas.Visibility = Visibility.Visible;
        }

        private void Move_Area(object sender, MouseEventArgs e)
        {
            if (isDraggingSelectionRect && !isTest)
            {
                //
                // Drag selection is in progress.
                //
                Point curMouseDownPoint = e.GetPosition(this);
                UpdateDragSelectionRect(origMouseDownPoint, curMouseDownPoint);

                e.Handled = true;
            }
            else if (isLeftMouseButtonDownOnWindow && !isTest)
            {
                //
                // The user is left-dragging the mouse,
                // but don't initiate drag selection until
                // they have dragged past the threshold value.
                //
                Point curMouseDownPoint = e.GetPosition(this);
                var dragDelta = curMouseDownPoint - origMouseDownPoint;
                double dragDistance = Math.Abs(dragDelta.Length);
                if (dragDistance > DragThreshold)
                {
                    //
                    // When the mouse has been dragged more than
                    // the threshold value commence drag selection.
                    //
                    isDraggingSelectionRect = true;

                    //
                    //  Clear selection immediately
                    //  when starting drag selection.
                    //

                    InitDragSelectionRect(origMouseDownPoint, curMouseDownPoint);
                }

                e.Handled = true;
            }
        }
    }

    public class captureAreaArg : EventArgs
    {
        public captureAreaArg(Point _leftTop, double _width, double _height, int _selectedIdx)
        {
            width = _width;
            height = _height;
            leftTop = _leftTop;
            selectedIdx = _selectedIdx;
        }
        private double width;
        public double Width
        {
            get { return width; }
        }
        private double height;
        public double Height
        {
            get { return height; }
        }
        private Point leftTop;
        public Point LeftTop
        {
            get { return leftTop; }
        }
        private int selectedIdx;
        public int SelectedIdx
        {
            get { return selectedIdx; }
        }
    }
}
