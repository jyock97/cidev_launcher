using System.Runtime.InteropServices;
using System;
using System.Diagnostics;

namespace cidev_launcher.Services
{
    public class EventHookService
    {
        private static EventHookService _instance;
        public static EventHookService Instance
        {
            get
            {
                if (_instance == null) { _instance = new EventHookService(); }
                return _instance;
            }
        }

        private enum Keys
        {
            Tab = 9,
            Esc = 27,
            LWin = 91,
            RWin = 92,
            F4 = 115,
            LShift = 160,
            RShift = 161,
            LCtrl = 162,
            RCtrl = 163,
            LAlt = 164,
            RAlt = 165,
        }

        // Structure contain information about low-level Keyboard input event 
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public int key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }

        public int LastKeyTime { get; private set; }

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int id, HookProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string name);

        private IntPtr ptrKeyboardHook;
        private IntPtr ptrMouseHook;
        private HookProc objKeyboardProcess;
        private HookProc objMouseProcess;
        private int ctrlPressed;
        private int shiftPressed;
        private int altPressed;

        private IntPtr ProcessKeyboardKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

                bool isKeyPressed = (objKeyInfo.flags & 1 << 7) == 0;

                // Registrer if L/RShift, L/RCtrl, L/RAlt is pressed
                if (objKeyInfo.key == (int)Keys.LShift)
                {
                    int tempFlag = isKeyPressed ? 0b10 : 0b00;
                    shiftPressed = tempFlag | (shiftPressed & 0b01);
                }
                else if (objKeyInfo.key == (int)Keys.RShift)
                {
                    int tempFlag = isKeyPressed ? 0b01 : 0b00;
                    shiftPressed = (shiftPressed & 0b10) | tempFlag;
                }
                else if (objKeyInfo.key == (int)Keys.LCtrl)
                {
                    int tempFlag = isKeyPressed ? 0b10 : 0b00;
                    ctrlPressed = tempFlag | (ctrlPressed & 0b01);
                }
                else if (objKeyInfo.key == (int)Keys.RCtrl)
                {
                    int tempFlag = isKeyPressed ? 0b01 : 0b00;
                    ctrlPressed = (ctrlPressed & 0b10) | tempFlag;
                }
                else if (objKeyInfo.key == (int)Keys.LAlt)
                {
                    int tempFlag = isKeyPressed ? 0b10 : 0b00;
                    altPressed = tempFlag | (altPressed & 0b01);
                }
                else if (objKeyInfo.key == (int)Keys.RAlt)
                {
                    int tempFlag = isKeyPressed ? 0b01 : 0b00;
                    altPressed = (altPressed & 0b10) | tempFlag;
                }

                LastKeyTime = Environment.TickCount;

                // Disabling Windows keys 
                if (objKeyInfo.key == (int)Keys.LWin ||
                    objKeyInfo.key == (int)Keys.RWin ||
                    /* Is L/RCtrol Pressed */ (ctrlPressed & 0b11) > 0 && objKeyInfo.key == (int)Keys.Esc ||
                    /* Is L/RAlt Pressed */ (altPressed & 0b11) > 0 && objKeyInfo.key == (int)Keys.Tab ||
                    !GameService.Instance.IsGameRunning && /* Is L/RAlt Pressed */ (altPressed & 0b11) > 0 && objKeyInfo.key == (int)Keys.F4)
                {
                    return (IntPtr)1; // if 0 is returned then All the above keys will be enabled
                }
            }
            return CallNextHookEx(ptrKeyboardHook, nCode, wp, lp);
        }

        private IntPtr ProcessMouseKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                LastKeyTime = Environment.TickCount;
            }
            return CallNextHookEx(ptrMouseHook, nCode, wp, lp);
        }

        public void SetupEventHook()
        {
            ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
            objKeyboardProcess = new HookProc(ProcessKeyboardKey);
            ptrKeyboardHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
            objMouseProcess = new HookProc(ProcessMouseKey);
            ptrMouseHook = SetWindowsHookEx(14, objMouseProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
        }
    }
}
