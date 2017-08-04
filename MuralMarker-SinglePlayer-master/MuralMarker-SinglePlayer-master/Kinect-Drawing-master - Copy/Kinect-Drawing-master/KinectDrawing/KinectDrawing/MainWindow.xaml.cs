using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Threading;

namespace KinectDrawing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor _sensor = null;
        private ColorFrameReader _colorReader = null;
        private BodyFrameReader _bodyReader = null;
        private IList<Body> _bodies = null;
        private List<Polyline> _Trails = new List<Polyline>();

        private int _width = 0;
        private int _height = 0;
        private byte[] _pixels = null;
        private WriteableBitmap _bitmap = null;
        private int j = 0;
        private bool amClosed = false;

        public MainWindow()
        {
            InitializeComponent();

            _sensor = KinectSensor.GetDefault();
            _Trails.Add(trail1);
            _Trails.Add(trail2);
            _Trails.Add(trail3);
            _Trails.Add(trail4);
            _Trails.Add(trail5);
            _Trails.Add(trail6);
            _Trails.Add(trail7);
            _Trails.Add(trail8);
            _Trails.Add(trail9);
            _Trails.Add(trail10);
            _Trails.Add(trail11);
            _Trails.Add(trail12);
            _Trails.Add(trail13);
            _Trails.Add(trail14);
            _Trails.Add(trail15);
            _Trails.Add(trail16);
            _Trails.Add(trail17);
            _Trails.Add(trail18);
            _Trails.Add(trail19);
            _Trails.Add(trail20);
            _Trails.Add(trail21);
            _Trails.Add(trail22);
            _Trails.Add(trail23);
            _Trails.Add(trail24);
            _Trails.Add(trail25);
            _Trails.Add(trail26);
            _Trails.Add(trail27);
            _Trails.Add(trail28);
            _Trails.Add(trail29);
            _Trails.Add(trail30);
            _Trails.Add(trail31);
            _Trails.Add(trail32);
            _Trails.Add(trail33);
            _Trails.Add(trail34);
            _Trails.Add(trail35);
            _Trails.Add(trail36);
            _Trails.Add(trail37);
            _Trails.Add(trail38);
            _Trails.Add(trail39);
            _Trails.Add(trail40);


            if (_sensor != null)
            {
                _sensor.Open();

                _width = _sensor.ColorFrameSource.FrameDescription.Width;
                _height = _sensor.ColorFrameSource.FrameDescription.Height;

                _colorReader = _sensor.ColorFrameSource.OpenReader();
                _colorReader.FrameArrived += ColorReader_FrameArrived;

                _bodyReader = _sensor.BodyFrameSource.OpenReader();
                _bodyReader.FrameArrived += BodyReader_FrameArrived;

                _pixels = new byte[_width * _height * 4];
                _bitmap = new WriteableBitmap(_width, _height, 96.0, 96.0, PixelFormats.Bgra32, null);

                _bodies = new Body[_sensor.BodyFrameSource.BodyCount];

                camera.Source = _bitmap;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_colorReader != null)
            {
                _colorReader.Dispose();
            }

            if (_bodyReader != null)
            {
                _bodyReader.Dispose();
            }

            if (_sensor != null)
            {
                _sensor.Close();
            }
        }

        private void ColorReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    frame.CopyConvertedFrameDataToArray(_pixels, ColorImageFormat.Bgra);

                    _bitmap.Lock();
                    //Removing the next line will make the background black
                    Marshal.Copy(_pixels, 0, _bitmap.BackBuffer, _pixels.Length);
                    _bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
                    _bitmap.Unlock();
                }
            }
        }

        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {

            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    frame.GetAndRefreshBodyData(_bodies);

                    Body body = _bodies.Where(b => b.IsTracked).FirstOrDefault();

                    
                    if (body != null)
                    {

                        Joint handRight = body.Joints[JointType.HandRight];
                        Joint handLeft = body.Joints[JointType.HandLeft];

                        if (handRight.TrackingState != TrackingState.NotTracked)
                        {
                            CameraSpacePoint handRightPosition = handRight.Position;
                            ColorSpacePoint handRightPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(handRightPosition);

                            CameraSpacePoint handLeftPosition = handLeft.Position;
                            ColorSpacePoint handLeftPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(handLeftPosition);

                            float x = handRightPoint.X;
                            float y = handRightPoint.Y;
                            float leftY = handLeftPoint.Y;

                            if (leftY < 250)
                            {

                                for (int i = 0; i < 40; i++)
                                    {
                                    _Trails.ElementAt(i).Points.Clear();
                                    }
                        

                                }
                                if (!float.IsInfinity(x) && !float.IsInfinity(y))
                                
                                {
                                if (body.HandRightState == HandState.Open)
                                {
                                    if (amClosed == true)
                                    {

                                        j++;
                                        amClosed = false;
                                    }


                                    // DRAW!
                                    //if (handRight.TrackingState != TrackingState.NotTracked)
                                    //{
                                    Console.Write(j);
                                    _Trails.ElementAt(j).Points.Add(new Point { X = x, Y = y });

                                        Canvas.SetRight(brush, x - brush.Width / 2.0);
                                        Canvas.SetTop(brush, y - brush.Height);
                                        //}
                                    
                                }
                                else if (body.HandRightState == HandState.Closed)
                                {
                                    amClosed = true;

                                }
                                        //Console.Write(currentTrail);
                                    

                                }
                            }
                        }
                    }
                }
            }
        }
    }


