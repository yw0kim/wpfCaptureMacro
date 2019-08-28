using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Color = System.Drawing.Color;

namespace wpfCaptureMacro
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public enum ActionEnum {OuterIter, OuterNext, InnerIter, InnerNext, LastPixel, ImgCapture, DstPath};
    // 영역 지정
    // 캡처 
    class CAction
    {
        public CAction(ActionEnum _action)
        {
            action = _action;
            isActionSet = 'x';
            isTested = 'x';
            switch (action)
            {
                case ActionEnum.OuterIter:
                    strAction = "화 반복 수 지정";
                    break;
                case ActionEnum.OuterNext:
                    strAction = "다음 화 위치 지정";
                    break;
                case ActionEnum.InnerIter:
                    strAction = "페이지 최대 반복 수 지정";
                    break;
                case ActionEnum.InnerNext:
                    strAction = "다음 페이지 위치 지정";
                    break;
                case ActionEnum.LastPixel:
                    strAction = "마지막 페이지 팝업 위치 지정";
                    break;
                case ActionEnum.ImgCapture:
                    strAction = "이미지 캡처 영역 지정";
                    break;
                case ActionEnum.DstPath:
                    strAction = "캡처 파일 저장 경로 입력";
                    break;
                default:
                    strAction = "알 수 없는 입력";
                    return;
            }
        }


        public string strAction { get; set; }
        public char isActionSet { get; set; }
        public char isTested { get; set; }
        public string strValue { get; set; }

        public ActionEnum action;
        public int iterNum; // iterNum
        public double cx, cy; // mouseClick Target 
        public System.Windows.Point leftTop;//imgCapture Target
        public double width, height;
        public string path; // imgCapture file dst
       
    }


    public partial class MainWindow : Window
    {
        private List<CAction> actionList = new List<CAction>();
        private Bitmap bmp;
        private int fileNameIdx;
        private System.Drawing.Color lastColor;

        public MainWindow()
        {
            InitializeComponent();
            ActionListView.ItemsSource = actionList;
            foreach(ActionEnum actionEnum in Enum.GetValues(typeof(ActionEnum)))
            {
                CAction tmpAction = new CAction(actionEnum);
                actionList.Add(tmpAction);
            }
            fileNameIdx = 1;
            lastColor = Color.FromArgb(255, 68, 138, 255);
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }
        
        public void switchMainWindow()
        {
            App.Current.MainWindow = this;
            this.Show();
        }

        void iterGetEvent(object sender, iterNumArg e)
        {
            actionList[e.SelectedIdx].iterNum = e.IterNum;
            actionList[e.SelectedIdx].strValue = "반복수 : " + e.IterNum.ToString();
            actionList[e.SelectedIdx].isActionSet = 'o';
            ActionListView.Items.Refresh();

            if (actionList[e.SelectedIdx].action == ActionEnum.InnerIter)
            {
                innerTextBlock.Text = "0/" + e.IterNum.ToString();
            }
            else
            {
                outerTextBlock.Text = "0/" + e.IterNum.ToString();
            }
        }

        public void switchIterWindow(ActionEnum actionEnum, int selectedIdx)
        {
                SetIterWindow iterWindow = new SetIterWindow(actionEnum, selectedIdx);
                App.Current.MainWindow = iterWindow;
                iterWindow.goBackMainWindowEvent += new EventHandler<iterNumArg>(iterGetEvent);
                iterWindow.Show();
        }

        void mousePosGetEvent(object sender, mousePosArg e)
        {
            actionList[e.SelectedIdx].cx = e.X;
            actionList[e.SelectedIdx].cy = e.Y;
            actionList[e.SelectedIdx].strValue = "위치 : (" + e.X.ToString() + ", " + e.Y.ToString() + ")";
            actionList[e.SelectedIdx].isActionSet = 'o';
            ActionListView.Items.Refresh();
        }

        public void switchMousePosWindow(ActionEnum actionEnum, int selectedIdx)
        {
                SetMousePosWindow mousePosWindow = new SetMousePosWindow(actionEnum, selectedIdx);
                App.Current.MainWindow = mousePosWindow;
                mousePosWindow.goBackMainWindowEvent += new EventHandler<mousePosArg>(mousePosGetEvent);
                mousePosWindow.Show();
        }

        void areaGetEvent(object sender, captureAreaArg e)
        {
            if (e.SelectedIdx > -1)
            {
                actionList[e.SelectedIdx].leftTop = e.LeftTop;
                actionList[e.SelectedIdx].width = e.Width;
                actionList[e.SelectedIdx].height = e.Height;
                actionList[e.SelectedIdx].isActionSet = 'o';
                actionList[e.SelectedIdx].strValue = "leftTop : " +
                    "(" + e.LeftTop.ToString() + "), width, height : (" +
                    e.Width.ToString() + ", " + e.Height.ToString() + ")";
                ActionListView.Items.Refresh();
            }
        }

        public void switchAreaWindow(int selectedIdx)
        {
            SetCaptureAreaWindow areaWindow = new SetCaptureAreaWindow(selectedIdx);
            App.Current.MainWindow = areaWindow;
            areaWindow.goBackMainWindowEvent += new EventHandler<captureAreaArg>(areaGetEvent);
            areaWindow.Show();
        }

        public void setImgSavePath(int selectedIdx)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            // dialog.InitialDirectory = textbox.Text; // Use current value for initial dir
            dialog.Title = "저장 경로 지정"; // instead of default "Save As"
            dialog.Filter = "Directory|*.this.directory"; // Prevents displaying files
            dialog.FileName = "select"; // Filename will then be "select.this.directory"
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                // Remove fake filename from resulting path
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                // Our final value is in path
                actionList[selectedIdx].strValue = "지정 경로 : " + path;
                actionList[selectedIdx].isActionSet = 'o';
                actionList[selectedIdx].path = path;
                ActionListView.Items.Refresh();
            }
        }

        private void ActionSet_Btn_Click(object sender, RoutedEventArgs e)
        {
            int sel = ActionListView.SelectedIndex;
            if (sel > -1)
            {
                CAction toSetAction = actionList[sel];
                ActionEnum actionEnum = toSetAction.action;

                switch (actionEnum)
                {
                    case ActionEnum.OuterIter:
                    case ActionEnum.InnerIter:
                        switchIterWindow(actionEnum, sel);
                        break;
                    case ActionEnum.OuterNext:
                    case ActionEnum.InnerNext:
                    case ActionEnum.LastPixel:
                        switchMousePosWindow(actionEnum, sel);
                        break;
                    case ActionEnum.ImgCapture:
                        switchAreaWindow(sel);
                        break;
                    case ActionEnum.DstPath:
                        setImgSavePath(sel);
                        break;
                    default:
                        return;
                }

                actionList[sel] = toSetAction;
            }

        }

        private void ActionTest_Btn_Click(object sender, RoutedEventArgs e)
        {
            int sel = ActionListView.SelectedIndex;
            if (sel > -1)
            {
                CAction toSetAction = actionList[sel];
                ActionEnum actionEnum = toSetAction.action;

                switch (actionEnum)
                {
                    case ActionEnum.OuterIter:
                    case ActionEnum.InnerIter:
                        MessageBox.Show(actionList[sel].strValue);
                        break;
                    case ActionEnum.OuterNext:
                    case ActionEnum.InnerNext:
                    case ActionEnum.LastPixel:
                        SetCursorPos((int)actionList[sel].cx, (int)actionList[sel].cy);
                        MessageBox.Show("설정 위치로 커서가 이동하였습니다.");
                        break;
                    case ActionEnum.ImgCapture:
                        Rect dragRect = new Rect(actionList[sel].leftTop.X,
                            actionList[sel].leftTop.Y,
                            actionList[sel].width,
                            actionList[sel].height);
                        SetCaptureAreaWindow areaWindow = new SetCaptureAreaWindow(sel, true, dragRect);
                        App.Current.MainWindow = areaWindow;
                        areaWindow.Show();
                        break;
                    case ActionEnum.DstPath:
                        MessageBox.Show(actionList[sel].strValue);
                        break;
                    default:
                        return;
                }

                actionList[sel] = toSetAction;
            }
        }

        ///// <summary>
        ///// 커서 위치 셋팅
        ///// </summary>
        ///// <param name="x">x좌표</param>
        ///// <param name="y">y좌표</param>
        [System.Runtime.InteropServices.DllImport("User32", EntryPoint = "SetCursorPos")]
        public static extern void SetCursorPos(int x, int y);
        [System.Runtime.InteropServices.DllImport("User32", EntryPoint = "mouse_event")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private void delayClick(double x, double y)
        {
            SetCursorPos((int)x, (int)y);
            mouse_event(0x02, (int)x, (int)y, 0, 0);
            mouse_event(0x04, (int)x, (int)y, 0, 0);
            System.Threading.Thread.Sleep(500);
            this.Dispatcher.Invoke
                (
                (System.Threading.ThreadStart)( () => { }), 
                System.Windows.Threading.DispatcherPriority.ApplicationIdle
                );
        }

        private bool isEqualBmp(byte[] image1Bytes, byte[] image2Bytes)
        {
            var image164 = Convert.ToBase64String(image1Bytes);
            var image264 = Convert.ToBase64String(image2Bytes);

            return string.Equals(image164, image264);
        }

        // return false : end of volume (회차 끝남. 마지막 페이지)
        // return true : 계속 캡처 (마지막 페이지 아님)
        private bool captureSave(string path, System.Windows.Point src, 
            System.Drawing.Size size, System.Windows.Point lastPoint, int innerI)
        {
            bmp = new Bitmap(size.Width, size.Height,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Graphics gr = Graphics.FromImage(bmp);
            gr.CopyFromScreen((int)src.X, (int)src.Y, 0, 0, size);
            
            try
            {
                Color cLast = bmp.GetPixel((int)lastPoint.X - (int)src.X, (int)lastPoint.Y - (int)src.Y);
                if (lastColor.Equals(cLast))
                {
                    delayClick(lastPoint.X, lastPoint.Y);
                    bmp.Dispose();
                    gr.Dispose();
                    return false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                this.Close();
            }

            bmp.Save(path + "\\" + fileNameIdx.ToString() + ".png", ImageFormat.Png);
            fileNameIdx++;

            bmp.Dispose();
            gr.Dispose();
            return true;
        }

        private void Start_Btn_Click(object sender, RoutedEventArgs e)
        {
            // 1화 1페이지에서 시작
            // OuterIter, OuterNext, InnerIter, InnerNext, ImgCapture, DstPath

            for (int i = 0; i < actionList.Count(); i++)
            {
                if (actionList[i].isActionSet == 'x')
                {
                    MessageBox.Show("모든 값을 설정해주세요.");
                }
            }

            int outerIterNum = actionList[(int)ActionEnum.OuterIter].iterNum;
            int innerIterNum = actionList[(int)ActionEnum.InnerIter].iterNum;

            System.Windows.Point outerNextPos = new System.Windows.Point();
            outerNextPos.X = actionList[(int)ActionEnum.OuterNext].cx;
            outerNextPos.Y = actionList[(int)ActionEnum.OuterNext].cy;

            System.Windows.Point innerNextPos = new System.Windows.Point();
            innerNextPos.X = actionList[(int)ActionEnum.InnerNext].cx;
            innerNextPos.Y = actionList[(int)ActionEnum.InnerNext].cy;

            System.Windows.Point captureLeftTop = new System.Windows.Point();
            captureLeftTop = actionList[(int)ActionEnum.ImgCapture].leftTop;
            
            double captureWidth = actionList[(int)ActionEnum.ImgCapture].width;
            double captureHeight = actionList[(int)ActionEnum.ImgCapture].height;

            System.Windows.Point lastPoint = new System.Windows.Point();
            lastPoint.X = actionList[(int)ActionEnum.LastPixel].cx;
            lastPoint.Y = actionList[(int)ActionEnum.LastPixel].cy;

            System.Drawing.Size caputreSize = new System.Drawing.Size(
                (int)captureWidth, (int)captureHeight);

            string savePath = actionList[(int)ActionEnum.DstPath].path;

            int outI = 0;

            innerBar.Orientation = Orientation.Horizontal;
            innerBar.Minimum = 0;
            innerBar.Maximum = innerIterNum;
            innerBar.Value = 0;

            outerBar.Orientation = Orientation.Horizontal;
            outerBar.Minimum = 0;
            outerBar.Maximum = outerIterNum;
            outerBar.Value = 0;



            do // outer 화
            {
                int innerI = 0;
                innerBar.Value = 0;
                
                while (true) // inner page
                {
                    
                    innerI++;
                    innerBar.Value++;
                    innerTextBlock.Text = (innerBar.Value).ToString() + "/" + innerIterNum.ToString();
                    
                    delayClick(innerNextPos.X, innerNextPos.Y);

                    if (innerI < 2)
                    {
                        continue;
                    }

                    if (!captureSave(savePath, 
                        captureLeftTop, caputreSize,lastPoint,
                        innerI))
                    {
                        break;
                    }
                    
                    if (innerI >= innerIterNum)
                    {
                        break;
                    }
                }
                outI++;
                outerBar.Value++;
                outerTextBlock.Text = (outerBar.Value).ToString() + "/" + outerIterNum.ToString();

                delayClick(outerNextPos.X, outerNextPos.Y);
            } while (outI < outerIterNum);
            innerBar.Value = innerBar.Maximum;
            innerTextBlock.Text = (innerBar.Value).ToString() + "/" + innerIterNum.ToString();
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if ((Keyboard.GetKeyStates(Key.Escape) & KeyStates.Down) > 0)
            {
                System.Environment.Exit(-1);
            }
        }
    }
}