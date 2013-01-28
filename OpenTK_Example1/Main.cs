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
		static uint myBufferHandle = 0;
		
		static void dataPreparation()
		{
			GL.GenBuffers(1, out myBufferHandle);
			
			GL.BindBuffer(BufferTarget.ArrayBuffer, myBufferHandle);
			
			var myTriangleVerticies = new List<Vector2d>()
			{
				new Vector2d(-1, -1),
				new Vector2d(1, -1),
				new Vector2d(0, 1)
			};
			
			GL.BufferData<Vector2d>(BufferTarget.ArrayBuffer,
			                        new IntPtr(
				myTriangleVerticies.Count * sizeof(double) * 2),
			                        myTriangleVerticies.ToArray(),
			                        BufferUsageHint.StaticDraw);
			
		}
		
		static void Main (string [] args)
		{
			var myTKWindow = new GameWindow(640, 480, GraphicsMode.Default, "My First Game");
			
			dataPreparation();
			
			myTKWindow.RenderFrame += (object sender, FrameEventArgs e) => 
			{
				GL.ClearColor(Color4.Blue);
				GL.Clear (ClearBufferMask.ColorBufferBit);
				
				GL.EnableClientState(ArrayCap.VertexArray);
				
				GL.BindBuffer(BufferTarget.ArrayBuffer, myBufferHandle);
				GL.VertexPointer(2, VertexPointerType.Double, 0, 0);
				GL.DrawArrays(BeginMode.Triangles, 0, 3);
				
				GL.DisableClientState(ArrayCap.VertexArray);
				
				myTKWindow.SwapBuffers();
			};
			
			myTKWindow.Run();
			
			GL.DeleteBuffers(1, ref myBufferHandle);
		}
	}
}

