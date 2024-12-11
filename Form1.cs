using System.Drawing.Printing;
using Emgu.CV;

namespace ExampleCam
{
    public partial class Form1 : Form
    {
        private VideoCapture? _capture = null;
        private Mat? _frame;
        private bool IsConnect = true;
        private bool IsRecord = true;
        
        

        private void ProcessFrame(object sender, EventArgs e)
        {
            
            if (_capture != null && _capture.Ptr != IntPtr.Zero)
            {
                
                bool canCapture = _capture.Retrieve(_frame, 0);
                if (canCapture)
                {
                    
                    imageBox1.Image = _frame;
                }

            }
        }
        public Form1()
        {
            InitializeComponent();
            buttonStart.Enabled = false;
            buttonFlipV.Enabled = false;
            buttonFlipH.Enabled = false;

        }
        
        private async void buttonConnect_Click(object sender, EventArgs e) 
        {
           
        
            
            if (IsConnect)
            {
                await Connect();
                buttonConnect.Text = "Disconnect";

                buttonStart.Enabled = true;
                buttonFlipV.Enabled = true;
                buttonFlipH.Enabled = true;
            }
            else
           {
                Disconnect(); 
                buttonConnect.Text = "Connect";
                buttonStart.Enabled = false;
                buttonFlipV.Enabled = false;
                buttonFlipH.Enabled = false;

            }
            IsConnect = !IsConnect;


        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            
            try
            {
                if (_capture != null)
                {
                    _capture.Pause();
                    _capture.ImageGrabbed -= ProcessFrame;
                    _capture.Dispose();
                    
                }
                if (_frame != null)
                {
                    _frame.Dispose();
                }

                DisconnectCam();
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }

        }

        private void Disconnect ()
        {
            try
            {
                if (_capture != null)
                {
                    _capture.Pause();
                    _capture.ImageGrabbed -= ProcessFrame;
                    _capture.Dispose();

                }
                if (_frame != null)
                {
                    _frame.Dispose();
                }

                DisconnectCam();
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }

        }
        
        private async Task Connect ()
        {
            try
            {
                await WaitConnectCam();
                _capture = new VideoCapture();
                _capture.ImageGrabbed += ProcessFrame;
                _frame = new Mat();
                ConnectCam();

            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            
            if (IsRecord)
            {

                if (_capture != null)
                {
                    _capture.Start();
                }
                Recording();
                buttonStart.Text = "Pause";
                buttonConnect.Enabled = false;
            }
            else
            {
                if (_capture != null)
                {
                    _capture.Pause();
                }
                NoRecording();
                buttonStart.Text = "Start";
                buttonConnect.Enabled = true;
            }
            IsRecord = !IsRecord;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                _capture.Pause();
            }
        }

        private void buttonFlipV_Click(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                _capture.FlipHorizontal = !_capture.FlipHorizontal;
            }

        }

        private void buttonFlipH_Click(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                _capture.FlipVertical = !_capture.FlipVertical;
            }

        }

        private void ConnectCam()
        {
            CamStatus.Text = "Status:Connect";
            CamStatus.BackColor = Color.LimeGreen;
        }

        private async Task WaitConnectCam()
        {
            CamStatus.Text = "Status:Waiting";
            CamStatus.BackColor = Color.CadetBlue;
            await Task.Delay(1000);
        }

        private void DisconnectCam()
        {
            CamStatus.Text = "Status: Disconnect";
            CamStatus.BackColor = Color.RosyBrown;
        }

        private void Recording ()
        {
            RecStatus.Text = "Recording: Yes";
            RecStatus.BackColor = Color.LimeGreen;
        }

        private void NoRecording()
        {
            RecStatus.Text = "Recording: No";
            RecStatus.BackColor = Color.RosyBrown;
        }

    }
}
