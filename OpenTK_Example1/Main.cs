using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;

namespace OpenGL3
{
	public class HelloGL3: GameWindow
	{

		string vertexShaderSource = @"
            #version 140
 
            // object space to camera space transformation
            uniform mat4 modelview_matrix;            
 
            // camera space to clip coordinates
            uniform mat4 projection_matrix;
 
 
            // incoming vertex position
            in vec3 vertex_position;
 
            // incoming vertex normal
            in vec3 vertex_normal;
 
            // transformed vertex normal
            out vec3 normal;
 
            void main(void)
            {
              //not a proper transformation if modelview_matrix involves non-uniform scaling
              normal = ( modelview_matrix * vec4( vertex_normal, 0 ) ).xyz;
 
              // transforming the incoming vertex position
              gl_Position = projection_matrix * modelview_matrix * vec4( vertex_position, 1 );
            }";
		string fragmentShaderSource = @"
            #version 140
 
            precision highp float;
 
            const vec3 ambient = vec3( 0.1, 0.1, 0.1 );
            const vec3 lightVecNormalized = normalize( vec3( 0.5, 0.5, 2 ) );
            const vec3 lightColor = vec3( 1.0, 0.8, 0.2 );
 
            in vec3 normal;
 
            out vec4 out_frag_color;
 
            void main(void)
            {
              float diffuse = clamp( dot( lightVecNormalized, normalize( normal ) ), 0.0, 1.0 );
              out_frag_color = vec4( ambient + diffuse * lightColor, 1.0 );
            }";

		int vertexShaderHandle,
		fragmentShaderHandle,
		shaderProgramHandle,
		modelviewMatrixLocation,
		projectionMatrixLocation,
		positionVboHandle,
		normalVboHandle,
		indicesVboHandle;

		Matrix4 projectionMatrix, modelviewMatrix;
		
		Vector3[] positionVboData = new OpenTK.Vector3[]{
			new Vector3(-1.0f, -1.0f,  1.0f),
			new Vector3( 1.0f, -1.0f,  1.0f),
			new Vector3( 1.0f,  1.0f,  1.0f),
			new Vector3(-1.0f,  1.0f,  1.0f),
			new Vector3(-1.0f, -1.0f, -1.0f),
			new Vector3( 1.0f, -1.0f, -1.0f), 
			new Vector3( 1.0f,  1.0f, -1.0f),
			new Vector3(-1.0f,  1.0f, -1.0f) };
		
		uint[] indicesVboData = new uint[]{
			// front face
			0, 1, 2, 2, 3, 0,
			// top face
			3, 2, 6, 6, 7, 3,
			// back face
			7, 6, 5, 5, 4, 7,
			// left face
			4, 0, 3, 3, 7, 4,
			// bottom face
			0, 1, 5, 5, 4, 0,
			// right face
			1, 5, 6, 6, 2, 1, };

		public HelloGL3()
			: base( 640, 480, // width, height
			       new GraphicsMode( new ColorFormat( 8, 8, 8, 8 ), 16 ), "OpenGL 3.1 Example", 0,
			       DisplayDevice.Default, 3, 1, // use the default display device, request a 3.1 OpenGL context
			       GraphicsContextFlags.Debug ) //this will help us track down bugs
		{
			/*}
 
        public override void OnLoad( EventArgs e )
        {
            base.OnLoad(e);*/                        
			CreateShaders(); 
			CreateProgram();
			GL.UseProgram( shaderProgramHandle );
			
			QueryMatrixLocations();
			
			float widthToHeight = ClientSize.Width / ( float )ClientSize.Height;

			SetProjectionMatrix( OpenTK.Matrix4.CreatePerspectiveFieldOfView( 1.3f, widthToHeight, 1, 20 ) );
			
			SetModelviewMatrix( OpenTK.Matrix4.RotateX( 0.5f ) * OpenTK.Matrix4.CreateTranslation( 0, 0, -4 ) );
			
			LoadVertexPositions();
			LoadVertexNormals();
			LoadIndexer();
			
			// Other state
			GL.Enable( EnableCap.DepthTest );
			GL.ClearColor( 0, 0.1f, 0.4f, 1 );
		}

		private void CreateShaders()
		{
			vertexShaderHandle = GL.CreateShader( ShaderType.VertexShader );
			fragmentShaderHandle = GL.CreateShader( ShaderType.FragmentShader );
			
			GL.ShaderSource( vertexShaderHandle, vertexShaderSource );
			GL.ShaderSource( fragmentShaderHandle, fragmentShaderSource );
			
			GL.CompileShader( vertexShaderHandle );
			GL.CompileShader( fragmentShaderHandle );
		}

		private void CreateProgram()
		{
			shaderProgramHandle = GL.CreateProgram();
			
			GL.AttachShader( shaderProgramHandle, vertexShaderHandle );
			GL.AttachShader( shaderProgramHandle, fragmentShaderHandle );
			
			GL.LinkProgram( shaderProgramHandle );
			
			string programInfoLog;
			GL.GetProgramInfoLog( shaderProgramHandle, out programInfoLog );
			Console.WriteLine( programInfoLog );
		}
		private void QueryMatrixLocations()
		{
			projectionMatrixLocation = GL.GetUniformLocation( shaderProgramHandle, "projection_matrix" );
			modelviewMatrixLocation = GL.GetUniformLocation( shaderProgramHandle, "modelview_matrix" );
		}
		
		private void SetModelviewMatrix( OpenTK.Matrix4 matrix )
		{
			modelviewMatrix = matrix;
			GL.UniformMatrix4( modelviewMatrixLocation, false, ref modelviewMatrix );
		}
		
		private void SetProjectionMatrix( OpenTK.Matrix4 matrix )
		{
			projectionMatrix = matrix;
			GL.UniformMatrix4( projectionMatrixLocation, false, ref projectionMatrix );
		}
		
		private void LoadVertexPositions()
		{
			GL.GenBuffers( 1, out positionVboHandle );
			GL.BindBuffer( BufferTarget.ArrayBuffer, positionVboHandle );
			GL.BufferData<OpenTK.Vector3>( BufferTarget.ArrayBuffer,
			                       new IntPtr( positionVboData.Length * Vector3.SizeInBytes ),
			                       positionVboData, BufferUsageHint.StaticDraw );
			
			GL.EnableVertexAttribArray( 0 );
			GL.BindAttribLocation( shaderProgramHandle, 0, "vertex_position" );
			GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0 );            
		}
		
		private void LoadVertexNormals()
		{
			GL.GenBuffers( 1, out normalVboHandle );
			GL.BindBuffer( BufferTarget.ArrayBuffer, normalVboHandle );
			GL.BufferData<Vector3>( BufferTarget.ArrayBuffer,
			                       new IntPtr( positionVboData.Length * Vector3.SizeInBytes ),
			                       positionVboData, BufferUsageHint.StaticDraw );
			
			GL.EnableVertexAttribArray( 1 );            
			GL.BindAttribLocation( shaderProgramHandle, 1, "vertex_normal" );            
			GL.VertexAttribPointer( 1, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0 );
		}
		
		private void LoadIndexer()
		{
			GL.GenBuffers( 1, out indicesVboHandle );
			GL.BindBuffer( BufferTarget.ElementArrayBuffer, indicesVboHandle );
			GL.BufferData<uint>( BufferTarget.ElementArrayBuffer, 
			                    new IntPtr( indicesVboData.Length * Vector3.SizeInBytes ),
			                    indicesVboData, BufferUsageHint.StaticDraw );
		}
		
		protected override void OnUpdateFrame( FrameEventArgs e )
		{
			SetModelviewMatrix( Matrix4.RotateY( ( float )e.Time ) * modelviewMatrix );
			
			if( Keyboard[ OpenTK.Input.Key.Escape ] )
				Exit();
		}
		
		protected override void OnRenderFrame( FrameEventArgs e )
		{
			GL.Viewport( 0, 0, Width, Height );
			GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
			
			GL.DrawElements( BeginMode.Triangles, indicesVboData.Length,
			                DrawElementsType.UnsignedInt, IntPtr.Zero );
			
			GL.Flush();
			SwapBuffers();
		}
		
		protected override void OnResize( EventArgs e )
		{
			float widthToHeight = ClientSize.Width / ( float )ClientSize.Height;
			SetProjectionMatrix( Matrix4.Perspective( 1.3f, widthToHeight, 1, 20 ) );
		}
	}
	
	public class Program
	{
		[STAThread]
		public static void Main()
		{
			using( HelloGL3 win = new HelloGL3() )
			{
				string version = GL.GetString( StringName.Version );
				string shaderVersion = GL.GetString (StringName.ShadingLanguageVersion );
				string vendor = GL.GetString (StringName.Vendor );
				Console.WriteLine( version );
				Console.WriteLine( shaderVersion );
				Console.WriteLine( vendor );

				if( version.StartsWith( "3.1" ) ) win.Run();
				else Console.WriteLine( "Requested OpenGL version not available." );
			}
		}
	}
}