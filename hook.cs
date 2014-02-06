using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;
using System.Drawing;

class MensajesWindows
{

	const uint WM_SETTEXT = 0x000C;
	private const int WH_KEYBOARD_LL = 13;
	private const int WM_KEYDOWN = 0x0100;

	private static LowLevelKeyboardProc windows_procedure = funcionHook;
	private static IntPtr idHook = IntPtr.Zero;

	public static void Main()
	{
		idHook = Hook(windows_procedure);
		Application.Run();
		UnhookWindowsHookEx(idHook);
	}

	private static IntPtr Hook(LowLevelKeyboardProc proc)
	{
		using (Process procesoActual = Process.GetCurrentProcess())
		using (ProcessModule moduloActual = procesoActual.MainModule)
		{
			return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(moduloActual.ModuleName), 0);
		}
	}

	/*
		Obtiene el manejador de la ventana dado el nombre del proceso.
	*/
	public static IntPtr VentanaPorNombre(string wName)
	{
		IntPtr hWnd = IntPtr.Zero;
		foreach (Process pList in Process.GetProcesses())
		{
			if (pList.MainWindowTitle.Contains(wName))
			{
				hWnd = pList.MainWindowHandle;
			}
		}
		return hWnd;
	}


	private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);


	/*
		Callback para el hook
	*/
	private static IntPtr funcionHook( int nCode, IntPtr wParam, IntPtr lParam)
	{
		
		if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
		{
			//Mensaje de fallo
			StringBuilder escribir = new StringBuilder("No puedes realizar esta operacion");

			int vkCode = Marshal.ReadInt32(lParam);
			Console.WriteLine((Keys)vkCode);

			if( vkCode == 76) {

				/*
					--- Acerca del envio de mensajes a los controles ----
					
					Puedo obtener el handler de la ventana por el nombre
					IntPtr manejadorVentana = VentanaPorNombre ("Bloc");	

					En el codigo de abajo se envia el mensaje al control con el respectivo handle
				*/
				IntPtr manejadorVentana = new IntPtr(0x00010364);
				//IntPtr manejadorVentana = VentanaPorNombre ("Bloc");	
				SendMessage(manejadorVentana,WM_SETTEXT, escribir.Capacity, escribir);
			}
		

		}
		return CallNextHookEx(idHook, nCode, wParam, lParam);


	}

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr SetWindowsHookEx(int idHook,
		LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool UnhookWindowsHookEx(IntPtr hhk);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
		IntPtr wParam, IntPtr lParam);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr GetModuleHandle(string lpModuleName);


	[System.Runtime.InteropServices.DllImport("user32.dll")]
	private static extern IntPtr WindowFromPoint(Point pnt);


	[System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
	public static extern int RegisterWindowMessage(string lpString);

	[System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = System.Runtime.InteropServices.CharSet.Auto)] //
	public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

	[System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr SendMessage(int hWnd, int Msg, int wparam, int lparam);


}