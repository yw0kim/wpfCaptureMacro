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
    public enum ActionEnum {MouseClick, MouseUnClick, MouseMove, ImgCapture};
    // 영역 지정
    // 캡처 
    class CAction
    {
        public CAction(ActionEnum _action, int _order)
        {
            action = _action;
            order = _order;
            isActionSet = 'x';
            iteration = 0;
            switch (action)
            {
                case ActionEnum.MouseClick:
                    strAction = "마우스 L클릭";
                    break;
                case ActionEnum.MouseUnClick:
                    strAction = "마우스 L클릭해제";
                    break;
                case ActionEnum.MouseMove:
                    strAction = "마우스 이동";
                    break;
                case ActionEnum.ImgCapture:
                    strAction = "이미지 캡처";
                    break;
                //case ActionEnum.StringInput:
                //    strAction = " 문자열 입력";
                //    break;
                //case ActionEnum.KeboardClick:
                //    strAction = " 키 입력";
                //    break;
                default:
                    strAction = "알 수 없는 입력";
                    return;
            }
        }

        public int order { get; set; }
        public string strAction { get; set; }
        public char isActionSet { get; set; }
        public int iteration { get; set; }

        public ActionEnum action;
        public int x, y; // mouseMove, stringInput Target
        public string input;
       
    }
       

    public partial class MainWindow : Window
    {
        private List<CAction> actionList = new List<CAction>();

        public MainWindow()
        {
            InitializeComponent();
            ActionListView.ItemsSource = actionList;
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
            if (ActionListView.SelectedIndex > -1)
            {
                int sel = ActionListView.SelectedIndex;
                actionList.RemoveAt(sel);

                for (int i = sel; i < actionList.Count; i++)
                {
                    actionList[i].order = i + 1;
                }
                
                ActionListView.Items.Refresh();
            }
        }

        private void ActionSet_Btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Simulation_Btn_Click(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < actionList.Count; i++)
                MessageBox.Show("액션 : " + actionList[i].action);
        }

        private void Start_Btn_Click(object sender, RoutedEventArgs e)
        {
            DrawingVisual dv = new DrawingVisual();
            DrawingContext dc = dv.RenderOpen();


            dc.DrawRectangle(new VisualBrush(element), null,

new Rect(new Point(0, 0), new Point(element.ActualWidth, element.ActualHeight)));

       dc.Close();

 


    RenderTargetBitmap target =

new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight,

        96, 96, System.Windows.Media.PixelFormats.Pbgra32);

       
    target.Render(drawingVisual);

        
        }
}