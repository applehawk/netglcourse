using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

class Program
{
	
	static void Main(string[] args)
	{
		using(MyWindow N = new MyWindow())
		{
			N.Run(100.0);
		}
	}
	
}

// This class is a powerful window that can be created without using any GUI library.
public class MyWindow : GameWindow
{
	public MyWindow()
		: base()
	{
		Console.WriteLine("Press any key to exit.");
		KeyPress += HandleKeyPressEvent;
		Load += HandleOnLoadEvent;
		RenderFrame += HandleRenderEvent;
	}
	
	public void HandleKeyPressEvent(object sender, KeyPressEventArgs e)
	{
		Exit();
	}
	
	public void HandleOnLoadEvent(object sender, EventArgs e)
	{
		WindowBorder = WindowBorder.Hidden;
		WindowState = WindowState.Fullscreen;
		GL.ClearColor(new Color4(0.1f, 0, 0, 1) );
	}
	
	public void HandleRenderEvent(object sender, FrameEventArgs e)
	{
		GL.Clear(ClearBufferMask.DepthBufferBit | 
		         ClearBufferMask.ColorBufferBit | 
		         ClearBufferMask.AccumBufferBit | 
		         ClearBufferMask.StencilBufferBit);
		SwapBuffers();
	}
	
}