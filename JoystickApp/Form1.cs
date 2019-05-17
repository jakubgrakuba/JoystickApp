using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SlimDX.DirectInput;
using System.Runtime.InteropServices;

namespace JoystickApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            GetSticks();
            Sticks = GetSticks();
            timer1.Enabled = true;
        }

        DirectInput Input = new DirectInput();
        SlimDX.DirectInput.Joystick stick;
        Joystick[] Sticks;
        bool MouseClicked = false;

        int yValue = 0;
        int xValue = 0;
        int zValue = 0;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void mouse_event(uint flag, uint _x, uint _y, uint btn, uint exInfo);
        private const int MOUSEEVENT_LEFTDOWN = 0x02;
        private const int MOUSEEVENT_LEFTUP = 0x04;
        public Joystick[] GetSticks()
        {
            List<SlimDX.DirectInput.Joystick> sticks = new List<SlimDX.DirectInput.Joystick>();
            foreach(DeviceInstance device in Input.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
            {
                try
                {
                    stick = new SlimDX.DirectInput.Joystick(Input, device.InstanceGuid);
                    stick.Acquire();

                    foreach (DeviceObjectInstance deviceObject in stick.GetObjects())
                    {
                        if((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                        {
                            stick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-100,100);
                        }
                    }
                    sticks.Add(stick);
                }
                catch (DirectInputException)
                {

                    
                }
            }
            return sticks.ToArray();
        }

        void stickHandle(Joystick stick, int id)
        {
            JoystickState state = new JoystickState();
            state = stick.GetCurrentState();

            yValue = state.Y;
            xValue = state.X;
            zValue = state.Z;
            MouseMove(xValue, yValue);

            bool[] buttons = state.GetButtons();

            for (int i = 0; i < 16; i++)
            {
                if(buttons[i])
                    label1.Text = "button " + i.ToString()+ " " + buttons[i].ToString();
            }
            
            //please set joystic in DInput mode
            //buttons to linked from farmer
            // wasd - kierowanie
            //1 mouse - left click
            //2 esc - options
            //3 q - release 
            //4 tab - tractor swith
            //5 b - activate machine
            //6 h - auto pilot
            //7 i - map
            //8 p - shop
            //9 space horn
            //10 accelerat
            //11 brake

            if (id == 0)
            {
                if(buttons[0])
                {
                    if(MouseClicked == false)
                    {
                        mouse_event(MOUSEEVENT_LEFTDOWN, 0, 0, 0, 0);
                        MouseClicked = true;
                    }
                }
                else
                {
                    if (MouseClicked == true)
                    {
                        mouse_event(MOUSEEVENT_LEFTUP, 0, 0, 0, 0);
                        MouseClicked = false;
                    }
                }

              //  if(buttons[1])
              //  {
              //      SendKeys.Send("{ESC}");
              //  }
            }
        }

        public void MouseMove(int posx, int posy)
        {
            Cursor.Position = new Point(Cursor.Position.X + posx/2, Cursor.Position.Y + posy/2);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < Sticks.Length; i++)
            {
                stickHandle(Sticks[i], i);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Joystick[] joystick = GetSticks();
        }
    }
}
