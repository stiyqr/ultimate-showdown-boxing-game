using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using Windows.Kinect;


using Joint = Windows.Kinect.Joint;
using Point = System.Drawing.Point;
using static GameInput;
using TMPro;
using Unity.VisualScripting;

public class Connector : MonoBehaviour {


    [SerializeField] private Transform kinectMsgBox;
    [SerializeField] private TextMeshProUGUI punchLText;
    [SerializeField] private TextMeshProUGUI punchRText;
    [SerializeField] private TextMeshProUGUI dodgeLText;
    [SerializeField] private TextMeshProUGUI dodgeRText;
    [SerializeField] private TextMeshProUGUI guardText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI kinectDetectedText;
    [SerializeField] private TextMeshProUGUI bodyframeDetectedText;
    [SerializeField] private TextMeshProUGUI gestureReadyText;
    [SerializeField] private TextMeshProUGUI gestureNotReadyText;


    public event EventHandler OnKinectPunch_L;
    public event EventHandler OnKinectPunch_R;
    public event EventHandler<OnKinectDodge_LEventArgs> OnKinectDodge_L;
    public class OnKinectDodge_LEventArgs : EventArgs {
        public bool isDodging_L;
    }
    public event EventHandler<OnKinectDodge_REventArgs> OnKinectDodge_R;
    public class OnKinectDodge_REventArgs : EventArgs {
        public bool isDodging_R;
    }
    public event EventHandler<OnKinectGuardEventArgs> OnKinectGuard;
    public class OnKinectGuardEventArgs : EventArgs {
        public bool isGuard;
    }
    public event EventHandler OnKinectPauseAction;
    public event EventHandler OnKinectIdle;


    public uint count = 0;
    private KinectSensor kinectSensor = null;

    //private readonly int bytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
    private ColorFrameReader colorFrameReader = null;
    private BodyFrameReader bodyFrameReader = null;
    private FrameDescription frameDescription = null;
    //private WriteableBitmap colorBitmap = null;

    public int boneThickness = 6;
    public int jointThickness = 15;
    public double Xratio = 1280.0 / 512;
    public double Yratio = 720.0 / 424;

    private const double HandSize = 30;
    private const double JointThickness = 3;
    private const double ClipBoundsThickness = 10;
    private const float InferredZPositionClamp = 0.1f;

    //private DrawingGroup drawingGroup;
    //private DrawingImage imageSource;


    private Body[] bodies = null;
    private List<Tuple<JointType, JointType>> bones;
    private int displayWidth;
    private int displayHeight;
    private List<Pen> bodyColors;
    private string statusText = null;

    //private readonly Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
    //private readonly Brush handOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));
    //private readonly Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));
    //private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
    //private readonly Brush inferredJointBrush = Brushes.Yellow;
    //private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

    private CoordinateMapper coordinateMapper = null;
    private MultiSourceFrameReader multiFrameSourceReader = null;
    //private WriteableBitmap bitmap = null;
    private uint bitmapBackBufferSize = 0;
    private DepthSpacePoint[] colorMappedToDepthPoints = null;
    [DllImport("User32.Dll")]
    public static extern long SetCursorPos(int x, int y);

    [DllImport("User32.Dll")]
    public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT {
        public int x;
        public int y;

        public POINT(int X, int Y) {
            x = X;
            y = Y;
        }
    }


    bool is_left_punch = false;
    bool is_right_punch = false;
    bool is_left_dodge = false;
    bool is_right_dodge = false;
    bool is_defense = false;
    bool is_pause = false;
    bool gesture_ready = false;


    private void Start() {
        this.kinectSensor = KinectSensor.GetDefault();
        this.frameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);


        //this.multiFrameSourceReader = this.kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Depth | FrameSourceTypes.Color | FrameSourceTypes.BodyIndex);
        //this.multiFrameSourceReader.MultiSourceFrameArrived += this.Reader_MultiSourceFrameArrived;
        //this.coordinateMapper = this.kinectSensor.CoordinateMapper;

        FrameDescription depthFrameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;
        int depthWidth = depthFrameDescription.Width;
        int depthHeight = depthFrameDescription.Height;

        FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.FrameDescription;
        int colorWidth = colorFrameDescription.Width;
        int colorHeight = colorFrameDescription.Height;

        this.colorMappedToDepthPoints = new DepthSpacePoint[colorWidth * colorHeight];
        //this.bitmap = new WriteableBitmap(colorWidth, colorHeight, 96.0, 96.0, PixelFormats.Bgra32, null);
        //this.bitmapBackBufferSize = (uint)((this.bitmap.BackBufferStride * (this.bitmap.PixelHeight - 1)) + (this.bitmap.PixelWidth * this.bytesPerPixel));


        // open the reader for the color frames
        //this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();
        //this.colorFrameReader.FrameArrived += this.Reader_ColorFrameArrived;

        this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();
        this.bodyFrameReader.FrameArrived += bodyFrameReader_FrameArrived;
        this.coordinateMapper = this.kinectSensor.CoordinateMapper;
        //
        this.bones = new List<Tuple<JointType, JointType>>();
        //
        colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);
        //this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

        //this.drawingGroup = new DrawingGroup();
        this.displayWidth = frameDescription.Width / 2;
        this.displayHeight = frameDescription.Height / 2;

        #region Add bones
        // Torso
        this.bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
        this.bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
        this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
        this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
        this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
        this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
        this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
        this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

        // Right Arm
        this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
        this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
        this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
        this.bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
        this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

        // Left Arm
        this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
        this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
        this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
        this.bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
        this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));
        #endregion

        //this.bodyColors = new List<Pen>();
        //this.bodyColors.Add(new Pen(Brushes.Red, 6));
        //this.bodyColors.Add(new Pen(Brushes.Orange, 6));
        //this.bodyColors.Add(new Pen(Brushes.Green, 6));
        //this.bodyColors.Add(new Pen(Brushes.Blue, 6));
        //this.bodyColors.Add(new Pen(Brushes.Indigo, 6));
        //this.bodyColors.Add(new Pen(Brushes.Violet, 6));


        //this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;
        this.kinectSensor.Open();
        //this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText: Properties.Resources.NoSensorStatusText;
        //this.imageSource = new DrawingImage(this.drawingGroup);
        //this.DataContext = this;
        //this.InitializeComponent();

    }

    private void Update() {
        bool dataReceived = false;

        using (BodyFrame bodyFrame = bodyFrameReader.AcquireLatestFrame()) {
            if (bodyFrame != null) {
                if (this.bodies == null) {
                    this.bodies = new Body[bodyFrame.BodyCount];
                }

                // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                // As long as those body objects are not disposed and not set to null in the array,
                // those body objects will be re-used.
                bodyFrame.GetAndRefreshBodyData(this.bodies);
                dataReceived = true;
            }
        }

        if (dataReceived) {
            //using (DrawingContext dc = this.drawingGroup.Open()) {
            // Draw a transparent background to set the render size
            //dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));

            int penIndex = 0;
            foreach (Body body in this.bodies) {
                // Pen drawPen = this.bodyColors[penIndex++];

                if (body.IsTracked) {
                    //this.DrawClippedEdges(body, dc);

                    IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                    // convert the joint points to depth (display) space
                    Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                    ///camera (x, y, z)
                    /////頭HEAD
                    //身體SPINE_MID
                    //手腕WRIST_RIGHT/LEFT
                    //肩膀SHOULDER_RIGHT/LEFT
                    CameraSpacePoint HEAD = joints[JointType.Head].Position;
                    CameraSpacePoint SPINE_MID = joints[JointType.SpineMid].Position;
                    CameraSpacePoint WRIST_RIGHT = joints[JointType.WristRight].Position;
                    CameraSpacePoint WRIST_LEFT = joints[JointType.WristLeft].Position;
                    CameraSpacePoint SHOULDER_RIGHT = joints[JointType.ShoulderRight].Position;
                    CameraSpacePoint SHOULDER_LEFT = joints[JointType.ShoulderLeft].Position;

                    /*bool is_left_punch = false;
                    bool is_right_punch = false;
                    bool is_left_dodge = false;
                    bool is_right_dodge = false;
                    bool is_defense = false;
                    bool gesture_ready = false;*/

                    //actionText.Text = "none";
                    //if (is_left_punch) actionText.Text = "left punch!";
                    //else if (is_right_punch) actionText.Text = "right punch!";
                    //else if (is_left_dodge) actionText.Text = "left dodge!";
                    //else if (is_right_dodge) actionText.Text = "right dodge!";
                    //else if (is_defense) actionText.Text = "defense!";
                    //else if (is_pause) actionText.Text = "pause";

                    //左拳
                    double differenceLPY = Math.Abs(WRIST_LEFT.Y - SHOULDER_LEFT.Y);
                    double differenceLPZ = Math.Abs(WRIST_LEFT.Z - SHOULDER_LEFT.Z);
                    if (differenceLPY < 0.2 && differenceLPZ > 0.33) {
                        if (!is_left_punch && !is_right_punch && !is_left_dodge && !is_right_dodge && !is_defense && !is_pause) {
                            gesture_ready = true;
                        }
                        else {
                            gesture_ready = false;
                        }
                        if (gesture_ready) {
                            count++;
                            is_left_punch = true;
                            //actionText.Text = "left punch!";
                            //MessageBox.Show("left punch!");

                            //callbox();
                        }
                    }
                    else {
                        is_left_punch = false;
                    }
                    //右拳(右腕和右肩距離 Y.Z)
                    double differenceRPY = Math.Abs(WRIST_RIGHT.Y - SHOULDER_RIGHT.Y);
                    double differenceRPZ = Math.Abs(WRIST_RIGHT.Z - SHOULDER_RIGHT.Z);
                    if (differenceRPY < 0.2 && differenceRPZ > 0.33) {
                        if (!is_left_punch && !is_right_punch && !is_left_dodge && !is_right_dodge && !is_defense && !is_pause) {
                            gesture_ready = true;
                        }
                        else {
                            gesture_ready = false;
                        }
                        if (gesture_ready) {
                            count++;
                            is_right_punch = true;
                            //actionText.Text = "right punch!";
                            //MessageBox.Show("right punch!");
                        }
                    }
                    else {
                        is_right_punch = false;
                    }
                    //左閃(頭和SPINE的X)
                    double differenceHX = HEAD.X - SPINE_MID.X;
                    if (differenceHX < -0.15) {
                        if (is_left_dodge || (!is_left_punch && !is_right_punch && !is_right_dodge && !is_defense && !is_pause)) {
                            gesture_ready = true;
                        }
                        else {
                            gesture_ready = false;
                        }
                        if (gesture_ready) {
                            count++;
                            is_left_dodge = true;
                            //actionText.Text = "left dodge!";
                            //MessageBox.Show("left dodge!");
                        }
                    }
                    else {
                        is_left_dodge = false;
                    }
                    //右閃
                    if (differenceHX > 0.15) {
                        if (is_right_dodge || (!is_left_punch && !is_right_punch && !is_left_dodge && !is_defense && !is_pause)) {
                            gesture_ready = true;
                        }
                        else {
                            gesture_ready = false;
                        }
                        if (gesture_ready) {
                            count++;
                            is_right_dodge = true;
                            //actionText.Text = "right dodge!";
                            //MessageBox.Show("right dodge!");
                        }
                    }
                    else {
                        is_right_dodge = false;
                    }
                    //防禦(雙手腕高過頭)
                    double differenceYL = Math.Abs(WRIST_LEFT.Y - HEAD.Y);
                    double differenceYR = Math.Abs(WRIST_RIGHT.Y - HEAD.Y);
                    if (differenceYL < 0.1 && differenceYR < 0.1) {
                        if (is_defense || (!is_left_punch && !is_right_punch && !is_left_dodge && !is_right_dodge && !is_pause)) {
                            gesture_ready = true;
                        }
                        else {
                            gesture_ready = false;
                        }
                        if (gesture_ready) {
                            count++;
                            is_defense = true;
                            //actionText.Text = "defense!";
                            //MessageBox.Show("defense!");
                        }
                    }
                    else {
                        is_defense = false;
                    }

                    double differenceXX = Math.Abs(WRIST_LEFT.X - WRIST_RIGHT.X);
                    if (differenceXX >= 1) {
                        if (!is_left_punch && !is_right_punch && !is_left_dodge && !is_right_dodge && !is_defense && !is_pause) {
                            gesture_ready = true;
                        }
                        else {
                            gesture_ready = false;
                        }
                        if (gesture_ready) {
                            count++;
                            is_pause = true;
                            //actionText.Text = "defense!";
                            //MessageBox.Show("defense!");
                        }
                    }
                    else {
                        is_pause = false;
                    }
                    //cc.Text = count.ToString();

                    //HandState handState;
                    //Point handPosition;
                    //double positionX = WRIST_RIGHT.X;
                    //double positionY = WRIST_RIGHT.Y;
                    //int px = Convert.ToInt32(positionX * 1000);
                    //int py = Convert.ToInt32(positionY * -1000);
                    //SetCursorPos(px, py);


                    //找初始定位點
                    //
                    //else { actionText.Text = "none"; }
                    //foreach (JointType jointType in joints.Keys) {

                    //    // sometimes the depth(Z) of an inferred joint may show as negative
                    //    // clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                    //    CameraSpacePoint position = joints[jointType].Position;
                    //    if (position.Z < 0) {
                    //        position.Z = InferredZPositionClamp;
                    //    }

                    //    DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(position);
                    //    jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                    //}

                    //this.DrawBody(joints, jointPoints, dc, drawPen);

                    //this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                    //this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);
                }
            }

            // prevent drawing outside of our render area
            //this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
            //dc.Close();
            //}

            if (gesture_ready) {
                if (is_left_punch) {
                    //System.Windows.Forms.MessageBox.Show("left punch!");
                    //SendKeys.SendWait("{H}");
                    //Debug.Log("left dodge");
                    OnKinectPunch_L?.Invoke(this, EventArgs.Empty);
                }
                else if (is_right_punch) {
                    //System.Windows.Forms.MessageBox.Show("right punch!");
                    //Debug.Log("right punch");
                    OnKinectPunch_R?.Invoke(this, EventArgs.Empty);
                }
                else if (is_left_dodge) {
                    //System.Windows.Forms.MessageBox.Show("left dodge!");
                    //Debug.Log("left dodge");
                    OnKinectDodge_L?.Invoke(this, new OnKinectDodge_LEventArgs { isDodging_L = true });
                }
                else if (is_right_dodge) {
                    //System.Windows.Forms.MessageBox.Show("right dodge!");
                    //Debug.Log("right dodge");
                    OnKinectDodge_R?.Invoke(this, new OnKinectDodge_REventArgs { isDodging_R = true });
                }
                else if (is_defense) {
                    //System.Windows.Forms.MessageBox.Show("defense!");
                    //Debug.Log("defense");
                    OnKinectGuard?.Invoke(this, new OnKinectGuardEventArgs { isGuard = true });
                }
                else if (is_pause) {
                    //System.Windows.Forms.MessageBox.Show("pause!");
                    //Debug.Log("pause");
                    OnKinectPauseAction?.Invoke(this, EventArgs.Empty);
                }
                else {
                    OnKinectIdle?.Invoke(this, EventArgs.Empty);
                }
            }
            else {
                OnKinectIdle?.Invoke(this, EventArgs.Empty);
            }

        }
    }


    private void bodyFrameReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e) {
        bool dataReceived = false;

        using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame()) {
            if (bodyFrame != null) {
                if (this.bodies == null) {
                    this.bodies = new Body[bodyFrame.BodyCount];
                }

                // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                // As long as those body objects are not disposed and not set to null in the array,
                // those body objects will be re-used.
                bodyFrame.GetAndRefreshBodyData(this.bodies);
                dataReceived = true;
            }
        }

        if (dataReceived) {
            //using (DrawingContext dc = this.drawingGroup.Open()) {
            // Draw a transparent background to set the render size
            //dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));

            int penIndex = 0;
            foreach (Body body in this.bodies) {
                //Pen drawPen = this.bodyColors[penIndex++];

                if (body.IsTracked) {
                    //this.DrawClippedEdges(body, dc);

                    IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                    // convert the joint points to depth (display) space
                    Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                    ///camera (x, y, z)
                    /////頭HEAD
                    //身體SPINE_MID
                    //手腕WRIST_RIGHT/LEFT
                    //肩膀SHOULDER_RIGHT/LEFT
                    CameraSpacePoint HEAD = joints[JointType.Head].Position;
                    CameraSpacePoint SPINE_MID = joints[JointType.SpineMid].Position;
                    CameraSpacePoint WRIST_RIGHT = joints[JointType.WristRight].Position;
                    CameraSpacePoint WRIST_LEFT = joints[JointType.WristLeft].Position;
                    CameraSpacePoint SHOULDER_RIGHT = joints[JointType.ShoulderRight].Position;
                    CameraSpacePoint SHOULDER_LEFT = joints[JointType.ShoulderLeft].Position;

                    /*bool is_left_punch = false;
                    bool is_right_punch = false;
                    bool is_left_dodge = false;
                    bool is_right_dodge = false;
                    bool is_defense = false;
                    bool gesture_ready = false;*/

                    //actionText.Text = "none";
                    //if (is_left_punch) actionText.Text = "left punch!";
                    //else if (is_right_punch) actionText.Text = "right punch!";
                    //else if (is_left_dodge) actionText.Text = "left dodge!";
                    //else if (is_right_dodge) actionText.Text = "right dodge!";
                    //else if (is_defense) actionText.Text = "defense!";
                    //else if (is_pause) actionText.Text = "pause";

                    //左拳
                    double differenceLPY = Math.Abs(WRIST_LEFT.Y - SHOULDER_LEFT.Y);
                    double differenceLPZ = Math.Abs(WRIST_LEFT.Z - SHOULDER_LEFT.Z);
                    if (differenceLPY < 0.2 && differenceLPZ > 0.33) {
                        if (!is_left_punch && !is_right_punch && !is_left_dodge && !is_right_dodge && !is_defense && !is_pause) {
                            gesture_ready = true;
                        }
                        else {
                            gesture_ready = false;
                        }
                        if (gesture_ready) {
                            count++;
                            is_left_punch = true;
                            //actionText.Text = "left punch!";
                            //MessageBox.Show("left punch!");

                            //callbox();
                        }
                    }
                    else {
                        is_left_punch = false;
                    }
                    //右拳(右腕和右肩距離 Y.Z)
                    double differenceRPY = Math.Abs(WRIST_RIGHT.Y - SHOULDER_RIGHT.Y);
                    double differenceRPZ = Math.Abs(WRIST_RIGHT.Z - SHOULDER_RIGHT.Z);
                    if (differenceRPY < 0.2 && differenceRPZ > 0.33) {
                        if (!is_left_punch && !is_right_punch && !is_left_dodge && !is_right_dodge && !is_defense && !is_pause) {
                            gesture_ready = true;
                        }
                        else {
                            gesture_ready = false;
                        }
                        if (gesture_ready) {
                            count++;
                            is_right_punch = true;
                            //actionText.Text = "right punch!";
                            //MessageBox.Show("right punch!");
                        }
                    }
                    else {
                        is_right_punch = false;
                    }
                    //左閃(頭和SPINE的X)
                    double differenceHX = HEAD.X - SPINE_MID.X;
                    if (differenceHX < -0.15) {
                        if (is_left_dodge || (!is_left_punch && !is_right_punch && !is_right_dodge && !is_defense && !is_pause)) {
                            gesture_ready = true;
                        }
                        else {
                            gesture_ready = false;
                        }
                        if (gesture_ready) {
                            count++;
                            is_left_dodge = true;
                            //actionText.Text = "left dodge!";
                            //MessageBox.Show("left dodge!");
                        }
                    }
                    else {
                        is_left_dodge = false;
                    }
                    //右閃
                    if (differenceHX > 0.15) {
                        if (is_right_dodge || (!is_left_punch && !is_right_punch && !is_left_dodge && !is_defense && !is_pause)) {
                            gesture_ready = true;
                        }
                        else {
                            gesture_ready = false;
                        }
                        if (gesture_ready) {
                            count++;
                            is_right_dodge = true;
                            //actionText.Text = "right dodge!";
                            //MessageBox.Show("right dodge!");
                        }
                    }
                    else {
                        is_right_dodge = false;
                    }
                    //防禦(雙手腕高過頭)
                    double differenceYL = Math.Abs(WRIST_LEFT.Y - HEAD.Y);
                    double differenceYR = Math.Abs(WRIST_RIGHT.Y - HEAD.Y);
                    if (differenceYL < 0.1 && differenceYR < 0.1) {
                        //if (!is_left_punch && !is_right_punch && !is_left_dodge && !is_right_dodge && !is_defense && !is_pause) {
                        if (is_defense || (!is_left_punch && !is_right_punch && !is_left_dodge && !is_right_dodge && !is_pause)) {
                            gesture_ready = true;
                        }
                        else {
                            gesture_ready = false;
                        }
                        if (gesture_ready) {
                            count++;
                            is_defense = true;
                            //actionText.Text = "defense!";
                            //MessageBox.Show("defense!");
                        }
                    }
                    else {
                        is_defense = false;
                    }

                    double differenceXX = Math.Abs(WRIST_LEFT.X - WRIST_RIGHT.X);
                    if (differenceXX >= 1) {
                        if (!is_left_punch && !is_right_punch && !is_left_dodge && !is_right_dodge && !is_defense && !is_pause) {
                            gesture_ready = true;
                        }
                        else {
                            gesture_ready = false;
                        }
                        if (gesture_ready) {
                            count++;
                            is_pause = true;
                            //actionText.Text = "defense!";
                            //MessageBox.Show("defense!");
                        }
                    }
                    else {
                        is_pause = false;
                    }
                    //cc.Text = count.ToString();

                    //HandState handState;
                    //Point handPosition;
                    //double positionX = WRIST_RIGHT.X;
                    //double positionY = WRIST_RIGHT.Y;
                    //int px = Convert.ToInt32(positionX * 1000);
                    //int py = Convert.ToInt32(positionY * -1000);
                    //SetCursorPos(px, py);


                    //找初始定位點
                    //
                    //else { actionText.Text = "none"; }
                    //foreach (JointType jointType in joints.Keys) {

                    //    // sometimes the depth(Z) of an inferred joint may show as negative
                    //    // clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                    //    CameraSpacePoint position = joints[jointType].Position;
                    //    if (position.Z < 0) {
                    //        position.Z = InferredZPositionClamp;
                    //    }

                    //    DepthSpacePoint depthSpacePoint = this.coordinateMapper.MapCameraPointToDepthSpace(position);
                    //    jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                    //}

                    //this.DrawBody(joints, jointPoints, dc, drawPen);

                    //this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                    //this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);
                }
            }

            // prevent drawing outside of our render area
            //this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
            //dc.Close();
            //}

            if (gesture_ready) {
                if (is_left_punch) {
                    //System.Windows.Forms.MessageBox.Show("left punch!");
                    //SendKeys.SendWait("{H}");
                    //Debug.Log("H");
                    OnKinectPunch_L?.Invoke(this, EventArgs.Empty);
                }
                else if (is_right_punch) {
                    //System.Windows.Forms.MessageBox.Show("right punch!");
                    //Debug.Log("K");
                    OnKinectPunch_R?.Invoke(this, EventArgs.Empty);
                }
                else if (is_left_dodge) {
                    //System.Windows.Forms.MessageBox.Show("left dodge!");
                    //Debug.Log("G");
                    OnKinectDodge_L?.Invoke(this, new OnKinectDodge_LEventArgs { isDodging_L = true });
                }
                else if (is_right_dodge) {
                    //System.Windows.Forms.MessageBox.Show("right dodge!");
                    //Debug.Log("L");
                    OnKinectDodge_R?.Invoke(this, new OnKinectDodge_REventArgs { isDodging_R = true });
                }
                else if (is_defense) {
                    //System.Windows.Forms.MessageBox.Show("defense!");
                    //Debug.Log("J");
                    OnKinectGuard?.Invoke(this, new OnKinectGuardEventArgs { isGuard = true });
                }
                else if (is_pause) {
                    //System.Windows.Forms.MessageBox.Show("pause!");
                    //Debug.Log("P");
                    OnKinectPauseAction?.Invoke(this, EventArgs.Empty);
                }
                else {
                    OnKinectIdle?.Invoke(this, EventArgs.Empty);
                }
            }
            else {
                OnKinectIdle?.Invoke(this, EventArgs.Empty);
            }

        }
    }

    //private void callbox() {
    //    SendKeys.SendWait("{UP}");
    //    //System.Windows.Forms.MessageBox.Show("test!");
    //}
    //private void Reader_ColorFrameArrived(object sender, ColorFrameArrivedEventArgs e) {
    //    // ColorFrame is IDisposable
    //    using (ColorFrame colorFrame = e.FrameReference.AcquireFrame()) {
    //        if (colorFrame != null) {
    //            FrameDescription colorFrameDescription = colorFrame.FrameDescription;

    //            using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer()) {
    //                this.colorBitmap.Lock();

    //                // verify data and write the new color frame data to the display bitmap
    //                if ((colorFrameDescription.Width == this.colorBitmap.PixelWidth) && (colorFrameDescription.Height == this.colorBitmap.PixelHeight)) {
    //                    colorFrame.CopyConvertedFrameDataToIntPtr(
    //                        this.colorBitmap.BackBuffer,
    //                        (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
    //                        ColorImageFormat.Bgra);

    //                    this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));
    //                }

    //                this.colorBitmap.Unlock();
    //            }
    //        }
    //    }
    //}


    //private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e) {
    //    int depthWidth = 0;
    //    int depthHeight = 0;

    //    DepthFrame depthFrame = null;
    //    ColorFrame colorFrame = null;
    //    BodyIndexFrame bodyIndexFrame = null;
    //    bool isBitmapLocked = false;

    //    MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();

    //    // If the Frame has expired by the time we process this event, return.
    //    if (multiSourceFrame == null) {
    //        return;
    //    }

    //    // We use a try/finally to ensure that we clean up before we exit the function.  
    //    // This includes calling Dispose on any Frame objects that we may have and unlocking the bitmap back buffer.
    //    try {
    //        depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame();
    //        colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame();
    //        bodyIndexFrame = multiSourceFrame.BodyIndexFrameReference.AcquireFrame();

    //        // If any frame has expired by the time we process this event, return.
    //        // The "finally" statement will Dispose any that are not null.
    //        if ((depthFrame == null) || (colorFrame == null) || (bodyIndexFrame == null)) {
    //            return;
    //        }

    //        // Process Depth
    //        FrameDescription depthFrameDescription = depthFrame.FrameDescription;

    //        depthWidth = depthFrameDescription.Width;
    //        depthHeight = depthFrameDescription.Height;

    //        // Access the depth frame data directly via LockImageBuffer to avoid making a copy
    //        using (KinectBuffer depthFrameData = depthFrame.LockImageBuffer()) {
    //            this.coordinateMapper.MapColorFrameToDepthSpaceUsingIntPtr(
    //                depthFrameData.UnderlyingBuffer,
    //                depthFrameData.Size,
    //                this.colorMappedToDepthPoints);
    //        }

    //        // We're done with the DepthFrame 
    //        depthFrame.Dispose();
    //        depthFrame = null;

    //        // Process Color

    //        // Lock the bitmap for writing
    //        this.bitmap.Lock();
    //        isBitmapLocked = true;

    //        colorFrame.CopyConvertedFrameDataToIntPtr(this.bitmap.BackBuffer, this.bitmapBackBufferSize, ColorImageFormat.Bgra);

    //        // We're done with the ColorFrame 
    //        colorFrame.Dispose();
    //        colorFrame = null;

    //        // We'll access the body index data directly to avoid a copy
    //        using (KinectBuffer bodyIndexData = bodyIndexFrame.LockImageBuffer()) {
    //            unsafe {
    //                byte* bodyIndexDataPointer = (byte*)bodyIndexData.UnderlyingBuffer;

    //                int colorMappedToDepthPointCount = this.colorMappedToDepthPoints.Length;

    //                fixed (DepthSpacePoint* colorMappedToDepthPointsPointer = this.colorMappedToDepthPoints) {
    //                    // Treat the color data as 4-byte pixels
    //                    uint* bitmapPixelsPointer = (uint*)this.bitmap.BackBuffer;

    //                    // Loop over each row and column of the color image
    //                    // Zero out any pixels that don't correspond to a body index
    //                    for (int colorIndex = 0; colorIndex < colorMappedToDepthPointCount; ++colorIndex) {
    //                        float colorMappedToDepthX = colorMappedToDepthPointsPointer[colorIndex].X;
    //                        float colorMappedToDepthY = colorMappedToDepthPointsPointer[colorIndex].Y;

    //                        // The sentinel value is -inf, -inf, meaning that no depth pixel corresponds to this color pixel.
    //                        if (!float.IsNegativeInfinity(colorMappedToDepthX) &&
    //                            !float.IsNegativeInfinity(colorMappedToDepthY)) {
    //                            // Make sure the depth pixel maps to a valid point in color space
    //                            int depthX = (int)(colorMappedToDepthX + 0.5f);
    //                            int depthY = (int)(colorMappedToDepthY + 0.5f);

    //                            // If the point is not valid, there is no body index there.
    //                            if ((depthX >= 0) && (depthX < depthWidth) && (depthY >= 0) && (depthY < depthHeight)) {
    //                                int depthIndex = (depthY * depthWidth) + depthX;

    //                                // If we are tracking a body for the current pixel, do not zero out the pixel
    //                                if (bodyIndexDataPointer[depthIndex] != 0xff) {
    //                                    continue;
    //                                }
    //                            }
    //                        }

    //                        bitmapPixelsPointer[colorIndex] = 0;
    //                    }
    //                }

    //                this.bitmap.AddDirtyRect(new Int32Rect(0, 0, this.bitmap.PixelWidth, this.bitmap.PixelHeight));
    //            }
    //        }
    //    }
    //    finally {
    //        if (isBitmapLocked) {
    //            this.bitmap.Unlock();
    //        }

    //        if (depthFrame != null) {
    //            depthFrame.Dispose();
    //        }

    //        if (colorFrame != null) {
    //            colorFrame.Dispose();
    //        }

    //        if (bodyIndexFrame != null) {
    //            bodyIndexFrame.Dispose();
    //        }
    //    }
    //}

}