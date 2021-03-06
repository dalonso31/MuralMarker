﻿using Microsoft.Kinect;
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
        private List<DrawingBrush> _brushes = new List<DrawingBrush>();
        private List<Polyline> _Trails = new List<Polyline>();
        private int _width = 0;
        private int _height = 0;
        private byte[] _pixels = null;
        private WriteableBitmap _bitmap = null;

        public MainWindow()
        {
            InitializeComponent();

            _sensor = KinectSensor.GetDefault();

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
                int amount = _sensor.BodyFrameSource.BodyCount;
                _bodies = new Body[amount];
                _brushes.Add(brush);
                _brushes.Add(brush2);
                _brushes.Add(brush3);
                _brushes.Add(brush4);
                _brushes.Add(brush5);
                _brushes.Add(brush6);
                _Trails.Add(trail);
                _Trails.Add(trail2);
                _Trails.Add(trail3);
                _Trails.Add(trail4);
                _Trails.Add(trail5);
                _Trails.Add(trail7);

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
                   // Marshal.Copy(_pixels, 0, _bitmap.BackBuffer, _pixels.Length);
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

                    // Body body2 = _bodies.Where(b => b.IsTracked).Last();
                    int i = 0;
                    Console.Write("number of bodies is" + _bodies.Count());
                    foreach (Body individual in _bodies.Where(b => b.IsTracked))
                    {
                        Joint handRight = individual.Joints[JointType.HandRight];
                        Joint handLeft = individual.Joints[JointType.HandLeft];

                        if (handRight.TrackingState != TrackingState.NotTracked)
                        {
                            CameraSpacePoint handRightPosition = handRight.Position;
                            ColorSpacePoint handRightPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(handRightPosition);

                            CameraSpacePoint handLeftPosition = handLeft.Position;
                            ColorSpacePoint handLeftPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(handLeftPosition);

                            float x = handRightPoint.X;
                            float y = handRightPoint.Y;
                            float leftY = handLeftPoint.Y;
                            
                            Console.WriteLine("X: " + x + " Y: " + y);
                            if (!float.IsInfinity(x) && !float.IsInfinity(y))
                            {
                                // DRAW!

                                
                                    _Trails.ElementAt(i).Points.Add(new Point { X = x, Y = y });

                                    Canvas.SetLeft(_brushes.ElementAt(i), x - _brushes.ElementAt(i).Width);
                                    Canvas.SetTop(_brushes.ElementAt(i), y - _brushes.ElementAt(i).Height);
                                }

                            if (leftY<250)
                                {

                                _Trails.ElementAt(i).Points.Clear();
                                }

                                /* if (handLeft.TrackingState != TrackingState.NotTracked)
                                 {
                                     CameraSpacePoint handLeftPosition = handLeft.Position;
                                     ColorSpacePoint handLeftPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(handRightPosition);

                                     float x = handRPoint.X;
                                     float y = handRightPoint.Y;

                                     if (!float.IsInfinity(x) && !float.IsInfinity(y))
                                     {
                                         // DRAW!

                                         {
                                             _Trails.ElementAt(i).Points.Add(new Point { X = x, Y = y });

                                             Canvas.SetLeft(_brushes.ElementAt(i), x - _brushes.ElementAt(i).Width);
                                             Canvas.SetTop(_brushes.ElementAt(i), y - _brushes.ElementAt(i).Height);





                                             i++;
                                         }


                                         //Body body3 = _bodies.Where(b => b.IsTracked).ElementAt(2);

                                         /* if (body != null)
                                         {
                                             Joint handRight = body.Joints[JointType.HandRight];

                                             if (handRight.TrackingState != TrackingState.NotTracked)
                                             {
                                                 CameraSpacePoint handRightPosition = handRight.Position;
                                                 ColorSpacePoint handRightPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(handRightPosition);

                                                 float x = handRightPoint.X;
                                                 float y = handRightPoint.Y;

                                                 if (!float.IsInfinity(x) && !float.IsInfinity(y))
                                                 {
                                                     // DRAW!
                                                     trail.Points.Add(new Point { X = x, Y = y });

                                                     Canvas.SetLeft(brush, x - brush.Width / 2.0);
                                                     Canvas.SetTop(brush, y - brush.Height);
                                                 }
                                             }
                                         }

                                         if (body2 != null)
                                         {
                                             Joint handRight = body2.Joints[JointType.HandRight];
                                             if (handRight.TrackingState != TrackingState.NotTracked)
                                             {
                                                 CameraSpacePoint handRightPosition = handRight.Position;
                                                 ColorSpacePoint handRightPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(handRightPosition);

                                                 float x = handRightPoint.X;
                                                 float y = handRightPoint.Y;

                                                 if (!float.IsInfinity(x) && !float.IsInfinity(y))
                                                 {
                                                     // DRAW!
                                                     trail6.Points.Add(new Point { X = x, Y = y });

                                                     Canvas.SetLeft(brush2, x - brush2.Width / 2.0);
                                                     Canvas.SetTop(brush2, y - brush2.Height);
                                                 }
                                             }
                                         }
                                         */


                            }

                        i++;

                    }



                }
                } 
            }
        }
    }



