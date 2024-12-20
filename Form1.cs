using System.Drawing.Printing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ExampleCam
{
    public partial class Form1 : Form
    {
        private VideoCapture? _capture = null;
        private Mat? _frame;
        private bool IsConnect = true;
        private bool IsRecord = true;

        CascadeClassifier _cascadeClassifier = new CascadeClassifier(@"C:\Users\nenea\Downloads\emgucv-master\emgucv-master\opencv\haarcascade_frontalface_default.xml");

        public Form1()
        {
            InitializeComponent();
            buttonStart.Enabled = false;
            buttonFlipV.Enabled = false;
            buttonFlipH.Enabled = false;

        }

        private void ProcessFrame(object sender, EventArgs e)
        {

            if (_capture != null && _capture.Ptr != IntPtr.Zero)
            {
                using (var imageFrame = _capture.QueryFrame().ToImage<Bgr, Byte>())
                {
                    if (imageFrame != null)
                    {
                        var grayframe = imageFrame.Convert<Gray, byte>();
                        var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10);

                        foreach (var face in faces)
                        {
                            imageFrame.Draw(face, new Bgr(Color.MistyRose), 3);
                        }
                        imageBoxLive.Image = imageFrame;

                        if (faces.Length > 0)
                        {
                            Rectangle face_roi = new Rectangle(faces[0].X, faces[0].Y, 190, 190);
                            grayframe.ROI = face_roi;
                            imageBoxFace.Image = grayframe.Copy();
                        }
                    }

                    //bool canCapture = _capture.Retrieve(_frame, 0);
                    //if (canCapture)
                    //{

                    //    imageBox1.Image = _frame;
                    //}

                }
            }
        }


        private async void buttonConnect_Click(object sender, EventArgs e)
        {



            if (IsConnect)
            {
                await WaitConnectCam();
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

        private void Disconnect()
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

        private async Task Connect()
        {
            try
            {
                await Task.Run(() =>
                {
                    _capture = new VideoCapture();
                    _frame = new Mat();

                });

                if (_capture != null)
                {
                    _capture.ImageGrabbed += ProcessFrame;
                    ConnectCam();
                }
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

        private async void ConnectCam()
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

        private async void DisconnectCam()
        {
            CamStatus.Text = "Status: Disconnect";
            CamStatus.BackColor = Color.RosyBrown;
        }

        private async void Recording()
        {
            RecStatus.Text = "Recording: Yes";  
            RecStatus.BackColor = Color.LimeGreen;
        }

        private async void NoRecording()
        {
            RecStatus.Text = "Recording: No";
            RecStatus.BackColor = Color.RosyBrown;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string formatStringClock = "HH:mm:ss";
            string formatStringDate = "yyyy-MMM-dd";

            DateTime dateNow = DateTime.Now;
            statusLabelClock.Text = dateNow.ToString(formatStringClock);
            statusLabelDate.Text = dateNow.ToString(formatStringDate);

        }

        private void groupBoxControl_Enter(object sender, EventArgs e)
        {

        }
    }
}
