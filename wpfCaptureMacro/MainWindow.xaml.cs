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

namespace wpfCaptureMacro
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public enum ActionEnum {OuterIter, OuterNext, InnerIter, InnerNext, ImgCapture, DstPath};
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

        //public void setMouseClickAction(MainWindow mainWindow)
        //{
            
        //    mainWindow.swtichSetActionWindow();
        //    mainWindow.switchMainWindow();
        //}

        //public void setMouseUnClickAction(MainWindow mainWindow)
        //{
        //    mainWindow.swtichSetActionWindow();
        //    mainWindow.switchMainWindow();
        //}

        //public void setMouseMoveAction(MainWindow mainWindow)
        //{
        //    mainWindow.swtichSetActionWindow();
        //    mainWindow.switchMainWindow();
        //}

        //public void setImgCaptureAction(MainWindow mainWindow)
        //{
        //    mainWindow.swtichSetActionWindow();
        //    mainWindow.switchMainWindow();
        //}

        public string strAction { get; set; }
        public char isActionSet { get; set; }
        public char isTested { get; set; }
        public string strValue { get; set; }

        public ActionEnum action;
        public int iterNum; // iterNum
        public double cx, cy; // mouseClick Target 
        public double sx, sy, dx, dy;//imgCapture Target
        public string path; // imgCapture file dst
       
    }


    public partial class MainWindow : Window
    {
        private List<CAction> actionList = new List<CAction>();
        // public SetActionWindow setActionWindow;
        // public SetIterWindow iterWindow;

        public MainWindow()
        {
            InitializeComponent();
            foreach(ActionEnum actionEnum in Enum.GetValues(typeof(ActionEnum)))
            {
                CAction tmpAction = new CAction(actionEnum);
                actionList.Add(tmpAction);
            }
            
            ActionListView.ItemsSource = actionList;
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
        }

        public void switchIterWindow(ActionEnum actionEnum, int selectedIdx)
        {
            if (actionEnum == ActionEnum.OuterIter || actionEnum == ActionEnum.InnerIter)
            {
                SetIterWindow iterWindow = new SetIterWindow(actionEnum, selectedIdx);
                App.Current.MainWindow = iterWindow;
                iterWindow.goBackMainWindowEvent += new EventHandler<iterNumArg>(iterGetEvent);
                iterWindow.Show();
            }
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
            if (actionEnum == ActionEnum.OuterNext || actionEnum == ActionEnum.InnerNext)
            {
                SetMousePosWindow mousePosWindow = new SetMousePosWindow(actionEnum, selectedIdx);
                App.Current.MainWindow = mousePosWindow;
                mousePosWindow.goBackMainWindowEvent += new EventHandler<mousePosArg>(mousePosGetEvent);
                mousePosWindow.Show();
            }
        }

        void areaGetEvent(object sender, captureAreaArg e)
        {
            actionList[e.SelectedIdx].sx = e.sX;
            actionList[e.SelectedIdx].sy = e.sY;
            actionList[e.SelectedIdx].dx = e.dX;
            actionList[e.SelectedIdx].dy = e.dY;
            actionList[e.SelectedIdx].isActionSet = 'o';
            actionList[e.SelectedIdx].strValue = "위치 : " +
                "(" + e.sX.ToString() + ", " + e.sY.ToString() + "), (" +
                e.dX.ToString() + ", " +e.dY.ToString() + ")";
            ActionListView.Items.Refresh();
        }

        public void switchAreaWindow(int selectedIdx)
        {
            SetCaptureAreaWindow areaWindow = new SetCaptureAreaWindow(selectedIdx);
            App.Current.MainWindow = areaWindow;
            areaWindow.goBackMainWindowEvent += new EventHandler<captureAreaArg>(areaGetEvent);
            areaWindow.Show();
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
                        switchMousePosWindow(actionEnum, sel);
                        break;
                    case ActionEnum.ImgCapture:
                        switchAreaWindow(sel);
                        break;
                    case ActionEnum.DstPath:
                        break;
                    default:
                        return;
                }

                actionList[sel] = toSetAction;
            }

        }

        private void ActionTest_Btn_Click(object sender, RoutedEventArgs e)
        {



        }
        private void Start_Btn_Click(object sender, RoutedEventArgs e)
        {



        }

        /*
public void swtichSetActionWindow(ActionEnum action)
{
    setActionWindow = new SetActionWindow(action);
    App.Current.MainWindow = setActionWindow;
    setActionWindow.Show();
}


private void ActionAdd_Btn_Click(object sender, RoutedEventArgs e)
{
    // MessageBox.Show(ActionMenu.SelectedItem.ToString() + "를 추가했습니다.");
    CAction tmpAction = new CAction((ActionEnum)ActionMenu.SelectedIndex,
                            actionList.Count + 1);

    actionList.Add(tmpAction);

    ActionListView.Items.Refresh();
}

private void ActionDel_Btn_Click(object sender, RoutedEventArgs e)
{
    int sel = ActionListView.SelectedIndex;

    if (sel > -1)
    {
        actionList.RemoveAt(sel);

        for (int i = sel; i < actionList.Count; i++)
        {
            actionList[i].order = i + 1;
        }

        ActionListView.Items.Refresh();
    }
}
*/
    }
}