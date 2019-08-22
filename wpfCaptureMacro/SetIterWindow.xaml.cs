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
using System.Windows.Shapes;

namespace wpfCaptureMacro
{
    /// <summary>
    /// SetIterWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// http://blog.naver.com/PostView.nhn?blogId=ronghuan&logNo=110031908726 // window간 데이터
    /// 

    public partial class SetIterWindow : Window
    {
        public event EventHandler<iterNumArg> goBackMainWindowEvent;
        public int selectedIdx;
        public ActionEnum actionEnum;

        public SetIterWindow(ActionEnum _actionEnum, int _selectedIdx)
        {
            InitializeComponent();
            actionEnum = _actionEnum;
            if (actionEnum == ActionEnum.OuterIter)
            {
                IterTextBlock.Text = "총 몇 화인지 입력";
            } else if (actionEnum == ActionEnum.InnerIter)
            {
                IterTextBlock.Text = "최대 몇 페이지인지 입력";
            }
            selectedIdx = _selectedIdx;
        }

        private void inputBtn_Click(object sender, RoutedEventArgs e)
        {
            int input;
            try
            {
                 input = Int32.Parse(IterTextBox.Text);

            }
            catch (FormatException)
            {
                IterTextBlock.Text += "\n 입력 오류 재입력.";
                return; 
            }
            goBackMainWindowEvent(this, new iterNumArg(input, this.selectedIdx));
            this.Close();
        }

        private void IterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

    public class iterNumArg : EventArgs
    {
        public iterNumArg(int _iterNum, int _selectedIdx)
        {
            iterNum = _iterNum;
            selectedIdx = _selectedIdx;
        }
        private int iterNum;
        public int IterNum
        {
            get { return iterNum; }
        }

        private int selectedIdx;
        public int SelectedIdx
        {
            get { return selectedIdx; }
        }
    }
}
