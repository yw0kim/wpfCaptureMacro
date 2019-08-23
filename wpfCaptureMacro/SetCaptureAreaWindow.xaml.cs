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
        Point origMouseDownPoint, dstMoushUpPoint;
        double DragThreshold;
        double sx, sy, dx, dy;
        private Popup _popup;

        public SetCaptureAreaWindow(int _selectedIdx)
        {
            InitializeComponent();
            selectedIdx = _selectedIdx;
            isDraggingSelectionRect = false;
            isLeftMouseButtonDownOnWindow = false;
            DragThreshold = 50.0;
        }

        private void Down_AreaStart(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isLeftMouseButtonDownOnWindow = true;
                origMouseDownPoint = e.GetPosition(this);
                this.CaptureMouse();

                e.Handled = true;
            }
        }

        

        private void ApplyDragSelectionRect()
        {
            // dragSelectionCanvas.Visibility = Visibility.Collapsed;

            double x = Canvas.GetLeft(dragSelectionBorder);
            double y = Canvas.GetTop(dragSelectionBorder);
            double width = dragSelectionBorder.Width;
            double height = dragSelectionBorder.Height;
            Rect dragRect = new Rect(x, y, width, height);

            //
            // Inflate the drag selection-rectangle by 1/10 of its size to 
            // make sure the intended item is selected.
            //
            dragRect.Inflate(width / 10, height / 10);

            EventHandler handler = (s, e) =>
            {

            };
            Button ok = new Button();
            ok.Content = "영역 지정";
            ok.Click += handler;

            _popup = new Popup
            {
                Child = new Button { Content = "영역 지정", ClickMode.Press }
            };
            
        }
        private void Up_AreaEnd(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
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
            // Determine x,y,width and height
            // of the rect inverting the points if necessary.
            // 
    

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
            if (isDraggingSelectionRect)
            {
                //
                // Drag selection is in progress.
                //
                Point curMouseDownPoint = e.GetPosition(this);
                UpdateDragSelectionRect(origMouseDownPoint, curMouseDownPoint);

                e.Handled = true;
            }
            else if (isLeftMouseButtonDownOnWindow)
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
        public captureAreaArg(double _sx, double _sy, double _dx, double _dy, int _selectedIdx)
        {
            sx = _sx;
            sy = _sy;
            dx = _dx;
            dy = _dy;
            selectedIdx = _selectedIdx;
        }
        private double dx;
        public double dX
        {
            get { return dx; }
        }
        private double dy;
        public double dY
        {
            get { return dy; }
        }
        private double sx;
        public double sX
        {
            get { return sx; }
        }
        private double sy;
        public double sY
        {
            get { return sy; }
        }
        private int selectedIdx;
        public int SelectedIdx
        {
            get { return selectedIdx; }
        }
    }
}
