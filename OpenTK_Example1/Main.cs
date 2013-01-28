using System;
using System.Collections;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OpenTK_Example1
{
	public class Program
	{
		static void Main (string [] args)
		{
			var myTKWindow = new GameWindow(640, 480, GraphicsMode.Default, "My First Game");

			myTKWindow.RenderFrame += (object sender, FrameEventArgs e) => 
			{
				GL.ClearColor(Color4.Blue);
				GL.Clear (ClearBufferMask.ColorBufferBit);

				GL.Begin(BeginMode.Triangles);

				GL.Vertex2(-1, -1);
				GL.Vertex2(1, -1);
				GL.Vertex2(0, 1);

				GL.End();

				myTKWindow.SwapBuffers();
			};

			myTKWindow.Run();
		}
	}
}

